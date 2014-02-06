#region

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Serialization;
using HelixToolkit.Wpf;
using HotelCorp.HotelApp.Services.Managers;

#endregion

namespace HotelCorp.HotelApp {
    public class MainViewModel : INotifyPropertyChanged {
        private readonly Color[] palette = new[]
                                               {
                                                   Colors.SeaGreen,
                                                   Colors.OrangeRed,
                                                   Colors.MidnightBlue,
                                                   Colors.Firebrick,
                                                   Colors.Gold,
                                                   Colors.CornflowerBlue,
                                                   Colors.Red,
                                                   Colors.LightBlue,
                                                   Colors.Tomato,
                                                   Colors.YellowGreen,
                                                   Colors.DarkCyan,
                                                   Colors.Orange,
                                                   Colors.DeepSkyBlue,
                                                   Colors.DarkOrchid
                                               };

        private readonly XmlSerializer serializer = new XmlSerializer(typeof(List<RoomVoxel>), new[] {typeof(RoomVoxel)});

        public MainViewModel() {
            CurrentColor = GetPaletteColor();
            Model = new Model3DGroup();
            Voxels = new List<RoomVoxel>();
            Highlighted = new List<Model3D>();
            ModelToVoxel = new Dictionary<Model3D, RoomVoxel>();
            OriginalMaterial = new Dictionary<Model3D, Material>();
            UpdateModel();
        }

        public List<RoomVoxel> Voxels { get; set; }
        public Color CurrentColor { get; set; }
        public Model3DGroup Model { get; set; }

        public Dictionary<Model3D, RoomVoxel> ModelToVoxel { get; private set; }
        public Dictionary<Model3D, Material> OriginalMaterial { get; private set; }

        public List<Model3D> Highlighted { get; set; }
        public Model3D PreviewModel { get; set; }

        public int PaletteIndex { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public void Save(string fileName) {
            using (XmlWriter w = XmlWriter.Create(fileName, new XmlWriterSettings {Indent = true})) {
                serializer.Serialize(w, Voxels);
            }
        }

        public bool TryLoad(string fileName) {
            try {
                using (XmlReader r = XmlReader.Create(fileName)) {
                    object v = serializer.Deserialize(r);
                    Voxels = v as List<RoomVoxel>;
                }
                UpdateModel();
                return true;
            } catch {
                return false;
            }
        }

        public Color GetPaletteColor() {
            return palette[PaletteIndex%palette.Length];
        }

        public void UpdateModel() {
            Model.Children.Clear();
            ModelToVoxel.Clear();
            OriginalMaterial.Clear();
            foreach (RoomVoxel v in Voxels) {
                GeometryModel3D m = CreateVoxelModel3D(v);
                OriginalMaterial.Add(m, m.Material);
                Model.Children.Add(m);
                ModelToVoxel.Add(m, v);
            }
            RaisePropertyChanged("Model");
        }

        private static GeometryModel3D CreateVoxelModel3D(RoomVoxel v) {
            double size = 0.98*v.Scale;
            var m = new GeometryModel3D();
            var mb = new MeshBuilder();
            mb.AddBox(new Point3D(0, 0, 0), size, size, size);
            m.Geometry = mb.ToMesh();
            m.Material = MaterialHelper.CreateMaterial(v.Colour, v.Scale*.7);
            m.Transform = new TranslateTransform3D(v.Position.X, v.Position.Y, v.Position.Z);
            return m;
        }

        protected void RaisePropertyChanged(string property) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        ///     Adds the a voxel adjacent to the specified model.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="guest"></param>
        /// <param name="roomNumber"></param>
        public void Add(Model3D source, Vector3D normal, Guest guest, string roomNumber) {
            if (!ModelToVoxel.ContainsKey(source)) {
                return;
            }
            RoomVoxel v = ModelToVoxel[source];
            AddVoxel(v.Position + normal, guest: guest, roomNumber: roomNumber);
        }

        /// <summary>
        ///     Adds a voxel at the specified position.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="scale"></param>
        /// <param name="guest"></param>
        /// <param name="roomNumber"></param>
        public void AddVoxel(Point3D p, double scale = 1.00, Guest guest = null, string roomNumber = "") {
            Voxels.Add(new RoomVoxel(p, CurrentColor, scale, guest, roomNumber));
            UpdateModel();
        }

        /// <summary>
        ///     Highlights the specified voxel model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void HighlightVoxel(Model3D model) {
            foreach (GeometryModel3D m in Model.Children) {
                if (!ModelToVoxel.ContainsKey(m)) {
                    continue;
                }
                RoomVoxel v = ModelToVoxel[m];
                Material om = OriginalMaterial[m];

                // highlight color
                Color hc = Color.FromArgb(0x90, (byte) (v.Colour.R + 100), v.Colour.G, v.Colour.B);
                m.Material = m == model ? MaterialHelper.CreateMaterial(hc) : om;
            }
        }

        public RoomVoxel GetVoxel(Model3D source) {
            if (source == null || !ModelToVoxel.ContainsKey(source)) {
                return null;
            }
            RoomVoxel v = ModelToVoxel[source];
            return v;
        }

        /// <summary>
        ///     Shows a preview voxel adjacent to the specified model (source).
        ///     If source is null, hide the preview.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="normal">The normal.</param>
        public void PreviewVoxel(Model3D source, Vector3D normal = default(Vector3D)) {
            if (PreviewModel != null) {
                Model.Children.Remove(PreviewModel);
            }
            PreviewModel = null;
            if (source == null) {
                return;
            }
            if (!ModelToVoxel.ContainsKey(source)) {
                return;
            }
            RoomVoxel v = ModelToVoxel[source];
            Color previewColor = Color.FromArgb(0x80, CurrentColor.R, CurrentColor.G, CurrentColor.B);
            var pv = new RoomVoxel(v.Position + normal, previewColor);
            PreviewModel = CreateVoxelModel3D(pv);
            Model.Children.Add(PreviewModel);
        }

        public void Remove(Model3D model) {
            if (!ModelToVoxel.ContainsKey(model)) {
                return;
            }
            RoomVoxel v = ModelToVoxel[model];
            Voxels.Remove(v);
            UpdateModel();
        }

        public void Clear() {
            Voxels.Clear();
            UpdateModel();
        }
    }
}