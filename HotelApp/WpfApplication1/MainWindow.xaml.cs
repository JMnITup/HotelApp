// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#region

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using HotelCorp.HotelApp.Services.Managers;
using JamesMeyer.IocContainer;
using ServiceModelEx.Hosting;

#endregion

namespace HotelCorp.HotelApp {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly InterfaceResolver resolver = new InterfaceResolver();
        private readonly MainViewModel vm = new MainViewModel();

        public MainWindow() {
            InitializeComponent();

            resolver.Register<IOccupancyManager_Wpf, OccupancyManager_Wpf>()
                    .AsDelegate(InProcFactory.CreateInstance<OccupancyManager_Wpf, IOccupancyManager_Wpf>);

            DataContext = vm;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            view1.ZoomExtents(500);
            view1.Focus();
        }

        protected override void OnClosed(EventArgs e) {
            //vm.Save("MyModel.xml");
            base.OnClosed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            switch (e.Key) {
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

        private Model3D FindSource(Point p, out Vector3D normal) {
            IList<Viewport3DHelper.HitResult> hits = view1.Viewport.FindHits(p);

            foreach (Viewport3DHelper.HitResult h in hits) {
                if (h.Model == vm.PreviewModel) {
                    continue;
                }
                normal = h.Normal;
                return h.Model;
            }
            normal = new Vector3D();
            return null;
        }

        private void view1_MouseDown(object sender, MouseButtonEventArgs e) {
            Point p = Mouse.GetPosition(view1);
            Vector3D n;
            Model3D source = FindSource(p, out n);
            Voxel hoverVoxel = vm.GetVoxel(source);
            if (hoverVoxel != null && hoverVoxel.Guest != null) {
                using (var svc = resolver.Resolve<IOccupancyManager_Wpf>()) {
                    svc.CheckoutGuest(hoverVoxel.Guest, hoverVoxel.RoomNumber);
                    RebuildMap();
                    tbInformation.Text = "Guest " + hoverVoxel.Guest.FirstName + " " + hoverVoxel.Guest.LastName + " checked out of room " +
                                         hoverVoxel.RoomNumber;
                }
            }
            /*bool shift = (Keyboard.IsKeyDown(Key.LeftShift));
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
            //CaptureMouse();*/
        }

        private void view1_MouseMove(object sender, MouseEventArgs e) {
            UpdatePreview();
        }

        private void UpdatePreview() {
            Point p = Mouse.GetPosition(view1);
            bool shift = (Keyboard.IsKeyDown(Key.LeftShift));
            Vector3D n;
            Model3D source = FindSource(p, out n);
            //if (shift)
            //{
            vm.PreviewVoxel(null);
            vm.HighlightVoxel(source);
            Voxel hoverVoxel = vm.GetVoxel(source);
            if (hoverVoxel != null) {
                lblRoomDetails.Content = "Room: " + hoverVoxel.RoomNumber;
                if (hoverVoxel.Guest != null) {
                    lblRoomDetails.Content += "  Guest: " + hoverVoxel.Guest.FirstName + " " + hoverVoxel.Guest.LastName;
                }
            } else {
                lblRoomDetails.Content = "";
            }

            //}
            //else
            //{
            //    vm.PreviewVoxel(source, n);
            //    vm.HighlightVoxel(null);
            //}
        }

        private void view1_MouseUp(object sender, MouseButtonEventArgs e) {
            // ReleaseMouseCapture();
        }

        private void view1_KeyUp(object sender, KeyEventArgs e) {
            // Should update preview voxel when shift is released
            UpdatePreview();
        }

        private void view1_KeyDown(object sender, KeyEventArgs e) {
            // Should update preview voxel when shift is pressed
            UpdatePreview();
        }

        private void GenerateNewHotel(object sender, RoutedEventArgs e) {
            using (var svc = resolver.Resolve<IOccupancyManager_Wpf>()) {
                uint x, y, z;
                x = UInt32.Parse(GenX.Text);
                y = UInt32.Parse(GenY.Text);
                z = UInt32.Parse(GenZ.Text);
                List<Room> hotelMap = svc.GenerateBasicHotel(x, y, z);
                RebuildMap(hotelMap);
            }
            CheckinBtn.IsEnabled = true;
            UpdatePreview();
        }

        private void CheckinGuest(object sender, RoutedEventArgs e) {
            using (var svc = resolver.Resolve<IOccupancyManager_Wpf>()) {
                var guest = new Guest(txtFirstName.Text, txtLastName.Text);
                txtFirstName.Text = "";
                txtLastName.Text = "";
                Room room = null;
                try {
                    room = svc.CheckinGuest(guest);
                } catch (FaultException ex) {
                    MessageBox.Show(ex.Message);
                }
                RebuildMap();
                if (room != null) {
                    tbInformation.Text = "Guest " + room.Guest.FirstName + " " + room.Guest.LastName + " checked into room " + room.RoomNumber;
                } else {
                    tbInformation.Text = "";
                }
            }
        }

        private void RebuildMap(List<Room> roomList = null) {
            if (roomList == null) {
                using (var svc = resolver.Resolve<IOccupancyManager_Wpf>()) {
                    roomList = svc.GetAllRooms();
                }
            }
            vm.Clear();
            roomList.ForEach(room =>
                                 {
                                     double scale = room.Guest == null ? 0.2 : 1;
                                     vm.AddVoxel(room.Location, scale, room.Guest, room.RoomNumber);
                                 });
            UpdatePreview();
            tbInformation.Text = "";
        }
    }
}