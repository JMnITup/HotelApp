#region

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace HotelCorp.HotelApp.Services.Access.Tests {
    [TestClass]
    public class RoomAccessGetInfoUnitTests {
        [TestMethod]
        [TestCategory("Unit")]
        public void GetListOfRoomNumbers() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            service.GenerateBasicHotel(10, 10, 10);

            // Act
            List<string> list = service.GetRoomNumbers();

            // Assert
            Assert.AreEqual(10*10*10, list.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetListOfGuests() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            service.GenerateBasicHotel(3, 2, 3);

            var guest1 = new Guest(fname: "Some", lname: "Body");
            string roomNumber1 = service.GetRoomNumbers()[4];
            service.AssignGuestToRoom(guest1, roomNumber1);

            var guest2 = new Guest(fname: "Noone", lname: "Special");
            string roomNumber2 = service.GetRoomNumbers()[3];
            service.AssignGuestToRoom(guest2, roomNumber2);

            var guest3 = new Guest(fname: "Short", lname: "Timer");
            string roomNumber3 = service.GetRoomNumbers()[5];
            service.AssignGuestToRoom(guest3, roomNumber3);
            service.UnassignGuestFromRoom(guest3, roomNumber3);

            // Act
            List<Guest> list = service.GetGuestList();

            // Assert
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list.FindAll(guest => guest.FirstName == "Some" && guest.LastName == "Body").Count);
            Assert.AreEqual(1, list.FindAll(guest => guest.FirstName == "Noone" && guest.LastName == "Special").Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetListOfEmptyRooms() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            service.GenerateBasicHotel(3, 2, 3);

            var guest1 = new Guest(fname: "Some", lname: "Body");
            string roomNumber1 = service.GetRoomNumbers()[4];
            service.AssignGuestToRoom(guest1, roomNumber1);

            var guest2 = new Guest(fname: "Noone", lname: "Special");
            string roomNumber2 = service.GetRoomNumbers()[3];
            service.AssignGuestToRoom(guest2, roomNumber2);

            var guest3 = new Guest(fname: "Short", lname: "Timer");
            string roomNumber3 = service.GetRoomNumbers()[5];
            service.AssignGuestToRoom(guest3, roomNumber3);
            service.UnassignGuestFromRoom(guest3, roomNumber3);

            // Act
            List<Room> list = service.GetAllEmptyRooms();

            // Assert
            Assert.AreEqual(3*2*3 - 2, list.Count, "Expected number of empty rooms incorrect");
            Assert.IsTrue(list.All(room => room.Guest == null), "Not all rooms returned are empty");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetListOfAssignedRooms() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            service.GenerateBasicHotel(3, 2, 3);

            var guest1 = new Guest(fname: "Some", lname: "Body");
            string roomNumber1 = service.GetRoomNumbers()[4];
            service.AssignGuestToRoom(guest1, roomNumber1);

            var guest2 = new Guest(fname: "Noone", lname: "Special");
            string roomNumber2 = service.GetRoomNumbers()[3];
            service.AssignGuestToRoom(guest2, roomNumber2);

            var guest3 = new Guest(fname: "Short", lname: "Timer");
            string roomNumber3 = service.GetRoomNumbers()[5];
            service.AssignGuestToRoom(guest3, roomNumber3);
            service.UnassignGuestFromRoom(guest3, roomNumber3);

            // Act
            List<Room> list = service.GetAllAssignedRooms();

            // Assert
            Assert.AreEqual(2, list.Count, "Expected number of assigned rooms incorrect");
            Assert.IsTrue(list.All(room => room.Guest != null), "Not all rooms returned are occupied");
        }
    }
}