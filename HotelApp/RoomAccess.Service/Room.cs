#region

using System.Runtime.Serialization;
using System.Windows.Media.Media3D;

#endregion

namespace HotelCorp.HotelApp.Services.Access {
    [DataContract]
    public class Room {
        [DataMember] public Guest Guest;
        [DataMember] public Point3D Location;
        [DataMember] public string RoomNumber;

        public Room(string roomNumber, Point3D location) {
            RoomNumber = roomNumber;
            Location = location;
        }
    }
}