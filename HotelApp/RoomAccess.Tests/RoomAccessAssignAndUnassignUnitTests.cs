#region

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace HotelCorp.HotelApp.Services.Access.Tests {
    [TestClass]
    public class RoomAccessAssignAndUnassignUnitTests {
        [TestMethod]
        [TestCategory("Unit")]
        public void CheckInGuestIntoEmptyRoom() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            service.GenerateBasicHotel(2, 2, 2);
            var guest = new Guest(fname: "Some", lname: "Body");
            string roomNumber = service.GetRoomNumbers()[4];

            // Act
            service.AssignGuestToRoom(guest, roomNumber);

            // Assert
            Assert.AreEqual("Some", RoomAccess.HotelMap.Find(room => room.RoomNumber == roomNumber).Guest.FirstName, "Assigned guest's first name doesn't match");
            Assert.AreEqual("Body", RoomAccess.HotelMap.Find(room => room.RoomNumber == roomNumber).Guest.LastName, "assigned guest's last name doesn't match");
            Assert.IsTrue(RoomAccess.HotelMap.FindAll(room => room.RoomNumber != roomNumber).TrueForAll(room => room.Guest == null),
                          "Guest assigned to wrong rooms");
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(Exception))]
        public void CheckInGuestIntoNonExistantRoom() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            service.GenerateBasicHotel(2, 2, 2);
            var guest = new Guest(fname: "Some", lname: "Body");
            string roomNumber = "IAmNotARoom";

            // Act
            service.AssignGuestToRoom(guest, roomNumber);

            // Assert
            Assert.Fail("Expected exception was not thrown");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CheckOutGuest() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            service.GenerateBasicHotel(2, 2, 2);

            var guest1 = new Guest(fname: "Some", lname: "Body");
            string roomNumber1 = service.GetRoomNumbers()[4];
            service.AssignGuestToRoom(guest1, roomNumber1);

            var guest2 = new Guest(fname: "Noone", lname: "Special");
            string roomNumber2 = service.GetRoomNumbers()[3];
            service.AssignGuestToRoom(guest2, roomNumber2);

            // Act
            service.UnassignGuestFromRoom(guest1, roomNumber1);

            // Assert
            Assert.IsNull(RoomAccess.HotelMap.Find(room => room.RoomNumber == roomNumber1).Guest, "Guest still exists in room");
            Assert.AreEqual("Noone", RoomAccess.HotelMap.Find(room => room.RoomNumber == roomNumber2).Guest.FirstName, "Other Guest was checked out");
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(Exception))]
        public void CheckOutWrongGuestFromRoom() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            service.GenerateBasicHotel(2, 2, 2);

            var guest1 = new Guest(fname: "Some", lname: "Body");
            string roomNumber1 = service.GetRoomNumbers()[4];
            service.AssignGuestToRoom(guest1, roomNumber1);

            var guest2 = new Guest(fname: "Noone", lname: "Special");
            string roomNumber2 = service.GetRoomNumbers()[3];
            service.AssignGuestToRoom(guest2, roomNumber2);

            // Act
            service.UnassignGuestFromRoom(guest2, roomNumber1);

            // Assert
            Assert.Fail("Expected exception was not thrown");
        }
    }
}