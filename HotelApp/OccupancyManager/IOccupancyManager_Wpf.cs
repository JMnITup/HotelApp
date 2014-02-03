#region

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Windows.Media.Media3D;

#endregion

namespace HotelCorp.HotelApp.Services.Managers {
    [ServiceContract]
    public interface IOccupancyManager_Wpf {
        [OperationContract]
        List<Room> GenerateBasicHotel(uint roomCountHorizontal, uint roomCountDepth, uint roomCountVerticle);

        [OperationContract]
        void CheckingGuest(Guest guest, string roomNumber);

        [OperationContract]
        void CheckoutGuest(Guest guest, string roomNumber);

        [OperationContract]
        void FindGuest(string firstName, string lastName);

        [OperationContract]
        List<Room> GetAllRooms();
    }

    [DataContract]
    public class Room {
        [DataMember] public Guest Guest;
        [DataMember] public Point3D Location;
        [DataMember] public string RoomNumber;

        public Room(string roomNumber, Point3D location) {
            RoomNumber = roomNumber;
            Location = location;
        }

        public static implicit operator Room(Access.Room fromRoom) {
            if (fromRoom == null) {
                return null;
            }
            var toRoom = new Room(fromRoom.RoomNumber, fromRoom.Location) {Guest = fromRoom.Guest};
            return toRoom;
        }
    }

    [DataContract]
    public class Guest {
        [DataMember] public string FirstName;
        [DataMember] public string LastName;

        public Guest(string fname, string lname) {
            FirstName = fname;
            LastName = lname;
        }

        public static implicit operator Guest(Access.Guest fromGuest) {
            if (fromGuest == null) {
                return null;
            }
            var toGuest = new Guest(fromGuest.FirstName, fromGuest.LastName);
            return toGuest;
        }
    }
}