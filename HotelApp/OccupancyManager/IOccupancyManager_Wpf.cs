#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Windows.Media.Media3D;

#endregion

namespace HotelCorp.HotelApp.Services.Managers {
    [ServiceContract]
    public interface IOccupancyManager_Wpf : IDisposable {
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
    }

    [DataContract]
    public class Guest {
        [DataMember] public string FirstName;
        [DataMember] public string LastName;

        public Guest(string fname, string lname) {
            FirstName = fname;
            LastName = lname;
        }
    }
}