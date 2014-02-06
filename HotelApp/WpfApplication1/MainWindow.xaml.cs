#region

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
        private readonly Random random = new Random();
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
            //view1.ZoomExtents(1000);
            view1.Focus();
            GenerateNewHotel(sender, e);
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e) {}

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
            RoomVoxel hoverRoomVoxel = vm.GetVoxel(source);
            if (hoverRoomVoxel != null && hoverRoomVoxel.Guest != null) {
                using (var svc = resolver.Resolve<IOccupancyManager_Wpf>()) {
                    svc.CheckoutGuest(hoverRoomVoxel.Guest, hoverRoomVoxel.RoomNumber);
                    RebuildMap();
                    tbInformation.Text = "Guest " + hoverRoomVoxel.Guest.FirstName + " " + hoverRoomVoxel.Guest.LastName + " checked out of room " +
                                         hoverRoomVoxel.RoomNumber;
                }
            }
        }

        private void view1_MouseMove(object sender, MouseEventArgs e) {
            UpdatePreview();
        }

        private void UpdatePreview() {
            Point p = Mouse.GetPosition(view1);
            bool shift = (Keyboard.IsKeyDown(Key.LeftShift));
            Vector3D n;
            Model3D source = FindSource(p, out n);
            vm.PreviewVoxel(null);
            vm.HighlightVoxel(source);
            RoomVoxel hoverRoomVoxel = vm.GetVoxel(source);
            if (hoverRoomVoxel != null) {
                lblRoomDetails.Text = "Room: " + hoverRoomVoxel.RoomNumber;
                if (hoverRoomVoxel.Guest != null) {
                    lblRoomDetails.Text += "  Guest: " + hoverRoomVoxel.Guest.FirstName + " " + hoverRoomVoxel.Guest.LastName;
                }
            } else {
                lblRoomDetails.Text = "";
            }
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
                try {
                    x = UInt32.Parse(GenX.Text);
                    y = UInt32.Parse(GenY.Text);
                    z = UInt32.Parse(GenZ.Text);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    return;
                }
                List<Room> hotelMap = svc.GenerateBasicHotel(x, y, z);
                double avgx = 0;
                double avgy = 0;
                double avgz = 0;
                foreach (Room room in hotelMap) {
                    avgx += room.Location.X;
                    avgy += room.Location.Y;
                    avgz += room.Location.Z;
                }
                avgx = avgx/hotelMap.Count;
                avgy = avgy/hotelMap.Count;
                avgz = avgz/hotelMap.Count;
                RebuildMap(hotelMap);
                view1.Camera.Position = new Point3D(0, 10, 10);
                view1.LookAt(new Point3D(avgx, avgy, avgz), 18, 0);
            }
            CheckinBtn.IsEnabled = true;
            CheckinRandomBtn.IsEnabled = true;
            UpdatePreview();
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
                                     double scale = room.Guest == null ? 0.3 : 1;
                                     vm.AddVoxel(room.Location, scale, room.Guest, room.RoomNumber);
                                 });
            UpdatePreview();
            tbInformation.Text = "";
        }

        private void CheckinGuestClick(object sender, RoutedEventArgs e) {
            CheckinGuest(txtFirstName.Text, txtLastName.Text);
        }

        private void CheckinGuestRandomClick(object sender, RoutedEventArgs e) {
            CheckinGuest(BuildRandomName(), BuildRandomName());
        }

        private void CheckinGuest(string firstName, string lastName) {
            using (var svc = resolver.Resolve<IOccupancyManager_Wpf>()) {
                var guest = new Guest(firstName, lastName);
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

        private string BuildRandomName() {
            var builder = new StringBuilder();
            char ch;
            for (int i = 0; i < random.Next(1, 12); i++) {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26*random.NextDouble() + 65)));
                builder.Append(ch);
            }
            string randomstring = builder.ToString();
            return randomstring;
        }

        private void GenX_TextChanged(object sender, TextChangedEventArgs e) {}
    }
}