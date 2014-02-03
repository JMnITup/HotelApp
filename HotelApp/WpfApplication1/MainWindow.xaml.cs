// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using HotelCorp.HotelApp.Services.Access;
using HotelCorp.HotelApp.Services.Engines;
using HotelCorp.HotelApp.Services.Managers;
using JamesMeyer.IocContainer;
using ServiceModelEx.Hosting;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel vm = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            //vm.TryLoad("MyModel.xml");
            vm.AddVoxel(new Point3D(0, 0, 0));
            vm.AddVoxel(new Point3D(0, 0, 1));
            vm.AddVoxel(new Point3D(0, 1, 0));
            vm.AddVoxel(new Point3D(1, 0, 0));
            DataContext = vm;
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomExtents(500);
            view1.Focus();
        }

        protected override void OnClosed(EventArgs e)
        {
            vm.Save("MyModel.xml");
            base.OnClosed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Space:
                    vm.PaletteIndex++;
                    vm.CurrentColor = vm.GetPaletteColor();
                    break;
                case Key.A:
                    view1.ZoomExtents(500);
                    break;
                case Key.C:
                    vm.Clear();
                    break;
            }
        }

        Model3D FindSource(Point p, out Vector3D normal)
        {
            var hits = Viewport3DHelper.FindHits(view1.Viewport, p);

            foreach (var h in hits)
            {
                if (h.Model == vm.PreviewModel)
                    continue;
                normal = h.Normal;
                return h.Model;
            }
            normal = new Vector3D();
            return null;
        }

        private void view1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool shift = (Keyboard.IsKeyDown(Key.LeftShift));
            var p = e.GetPosition(view1);

            Vector3D n;
            var source = FindSource(p, out n);
            if (source != null)
            {
                if (shift)
                    vm.Remove(source);
                else
                    vm.Add(source, n);
            }
            else
            {
                var ray = Viewport3DHelper.Point2DtoRay3D(view1.Viewport, p);
                if (ray != null)
                {
                    var pi = ray.PlaneIntersection(new Point3D(0, 0, 0.5), new Vector3D(0, 0, 1));
                    if (pi.HasValue)
                    {
                        var pRound = new Point3D(Math.Round(pi.Value.X), Math.Round(pi.Value.Y),0);
                    //    var pRound = new Point3D(Math.Floor(pi.Value.X), Math.Floor(pi.Value.Y), Math.Floor(pi.Value.Z));
                        //var pRound = new Point3D((int)pi.Value.X, (int)pi.Value.Y, (int)pi.Value.Z);
                        vm.AddVoxel(pRound);
                    }
                }
            }
            UpdatePreview();
            //CaptureMouse();
        }

        private void view1_MouseMove(object sender, MouseEventArgs e)
        {
            UpdatePreview();
        }

        void UpdatePreview()
        {
            var p = Mouse.GetPosition(view1);
            bool shift = (Keyboard.IsKeyDown(Key.LeftShift));
            Vector3D n;
            var source = FindSource(p, out n);
            if (shift)
            {
                vm.PreviewVoxel(null);
                vm.HighlightVoxel(source);
            }
            else
            {
                vm.PreviewVoxel(source, n);
                vm.HighlightVoxel(null);
            }

        }

        private void view1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //  ReleaseMouseCapture();
        }

        private void view1_KeyUp(object sender, KeyEventArgs e)
        {
            // Should update preview voxel when shift is released
            UpdatePreview();
        }

        private void view1_KeyDown(object sender, KeyEventArgs e)
        {
            // Should update preview voxel when shift is pressed
            UpdatePreview();
        }

        private void GenerateNewHotel(object sender, RoutedEventArgs e) {
            // TODO: update this to actually use the service
            var resolver = new InterfaceResolver();
            resolver.Register<IRoomAccess, RoomAccess>();
            resolver.Register<IOccupancyManager_Wpf, OccupancyManager_Wpf>();
            resolver.Register<IReservingEngine, ReservingEngine>();

            using (var svc = resolver.Resolve<IOccupancyManager_Wpf>()) {//InProcFactory.CreateInstance<OccupancyManager_Wpf, IOccupancyManager_Wpf>()) {
                uint x, y, z;
                x = UInt32.Parse(GenX.Text);
                y = UInt32.Parse(GenY.Text);
                z = UInt32.Parse(GenZ.Text);
                var hotelMap = svc.GenerateBasicHotel(x, y, z);
                vm.Clear();
                hotelMap.ForEach(room =>
                                    {
                                        if (room.Guest == null) {
                                            vm.AddVoxel(room.Location);
                                        }
                                    });
            }
            UpdatePreview();
        }
    }
}