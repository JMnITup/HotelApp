#region

using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using HotelCorp.HotelApp.Services.Access;
using JamesMeyer.IocContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace HotelCorp.HotelApp.Services.Engines.Tests {
    [TestClass]
    public class ReservingServiceUnitTests {
        [TestMethod]
        [TestCategory("Unit")]
        public void InstantiateReservingServiceClass() {
            // Arrange

            // Act
            IReservingEngine service = new ReservingEngine();

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void InstantiateReservingServiceClassWithResolver() {
            // Arrange
            var resolver = new InterfaceResolver();

            // Act
            IReservingEngine service = new ReservingEngine(resolver);

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void PerformCheckin() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IRoomAccess, RoomAccess>();
            var roomAccess = resolver.Resolve<IRoomAccess>();

            roomAccess.GenerateBasicHotel(8, 2, 4);

            var guest1 = new Guest(fname: "Some", lname: "Body");
            string roomNumber1 = roomAccess.GetRoomNumbers()[4];
            roomAccess.AssignGuestToRoom(guest1, roomNumber1);

            var guest2 = new Guest(fname: "Noone", lname: "Special");
            string roomNumber2 = roomAccess.GetRoomNumbers()[3];
            roomAccess.AssignGuestToRoom(guest2, roomNumber2);

            IReservingEngine service = new ReservingEngine(resolver);

            var newGuest = new Guest(fname: "New", lname: "Guest");

            // Act
            Room x = service.PerformAutoCheckin(newGuest);

            // Assert
            Assert.AreEqual("8,1,1", x.Location.ToString());
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(Exception))]
        public void PerformCheckinToFullHotel() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IRoomAccess, RoomAccess>();
            var roomAccess = resolver.Resolve<IRoomAccess>();

            roomAccess.GenerateBasicHotel(1, 2, 1);

            var guest1 = new Guest(fname: "Some", lname: "Body");
            string roomNumber1 = roomAccess.GetRoomNumbers()[0];
            roomAccess.AssignGuestToRoom(guest1, roomNumber1);

            var guest2 = new Guest(fname: "Noone", lname: "Special");
            string roomNumber2 = roomAccess.GetRoomNumbers()[1];
            roomAccess.AssignGuestToRoom(guest2, roomNumber2);

            IReservingEngine service = new ReservingEngine(resolver);

            var newGuest = new Guest(fname: "New", lname: "Guest");

            // Act
            Room x = service.PerformAutoCheckin(newGuest);

            // Assert
            Assert.Fail("Expected Exception was not thrown");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void PerformCheckinToEmptyHotel() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IRoomAccess, RoomAccess>();
            var roomAccess = resolver.Resolve<IRoomAccess>();

            roomAccess.GenerateBasicHotel(8, 2, 4);

            IReservingEngine service = new ReservingEngine(resolver);

            var newGuest = new Guest(fname: "New", lname: "Guest");

            // Act
            Room x = service.PerformAutoCheckin(newGuest);

            // Assert
            Assert.AreEqual("1,1,1", x.Location.ToString());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CheckinTo10X10_first() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IRoomAccess, RoomAccess>();
            var roomAccess = resolver.Resolve<IRoomAccess>();

            roomAccess.GenerateBasicHotel(10, 10, 1);

            IReservingEngine service = new ReservingEngine(resolver);

            var newGuest = new Guest(fname: "New", lname: "Guest");

            // Act
            Room x = service.PerformAutoCheckin(newGuest);

            // Assert
            Assert.AreEqual("1,1,1", x.Location.ToString());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CheckinTo10X10_second() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IRoomAccess, RoomAccess>();
            var roomAccess = resolver.Resolve<IRoomAccess>();

            roomAccess.GenerateBasicHotel(10, 10, 1);

            IReservingEngine service = new ReservingEngine(resolver);

            var newGuest = new Guest(fname: "New", lname: "Guest");
            Room x = service.PerformAutoCheckin(newGuest);

            // Act
            x = service.PerformAutoCheckin(newGuest);

            // Assert
            Assert.AreEqual("10,10,1", x.Location.ToString());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CheckinTo10X10_third() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IRoomAccess, RoomAccess>();
            var roomAccess = resolver.Resolve<IRoomAccess>();

            roomAccess.GenerateBasicHotel(10, 10, 1);

            IReservingEngine service = new ReservingEngine(resolver);

            var newGuest = new Guest(fname: "New", lname: "Guest");

            Room x = service.PerformAutoCheckin(newGuest);
            x = service.PerformAutoCheckin(newGuest);

            // Act
            x = service.PerformAutoCheckin(newGuest);

            // Assert
            Assert.AreEqual("1,10,1", x.Location.ToString());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CheckinTo10X10_fourth() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IRoomAccess, RoomAccess>();
            var roomAccess = resolver.Resolve<IRoomAccess>();

            roomAccess.GenerateBasicHotel(10, 10, 1);

            IReservingEngine service = new ReservingEngine(resolver);

            var newGuest = new Guest(fname: "New", lname: "Guest");

            Room x = service.PerformAutoCheckin(newGuest);
            x = service.PerformAutoCheckin(newGuest);
            x = service.PerformAutoCheckin(newGuest);

            // Act
            x = service.PerformAutoCheckin(newGuest);

            // Assert
            Assert.AreEqual("10,1,1", x.Location.ToString());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CheckinTo10X10_fifth() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IRoomAccess, RoomAccess>();
            var roomAccess = resolver.Resolve<IRoomAccess>();

            roomAccess.GenerateBasicHotel(10, 10, 1);

            IReservingEngine service = new ReservingEngine(resolver);

            var newGuest = new Guest(fname: "New", lname: "Guest");

            Room x = service.PerformAutoCheckin(newGuest);
            x = service.PerformAutoCheckin(newGuest);
            x = service.PerformAutoCheckin(newGuest);
            x = service.PerformAutoCheckin(newGuest);

            // Act
            x = service.PerformAutoCheckin(newGuest);

            // Assert
            Assert.AreEqual("5,5,1", x.Location.ToString());
        }

        public class MockRoomAccess : IRoomAccess {
            #region Implementation of IDisposable

            /// <summary>
            ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() {}

            #endregion

            #region Implementation of IRoomAccess

            public void GenerateBasicHotel(uint xSize, uint ySize, uint zSize) {
                throw new NotImplementedException();
            }

            public List<string> GetRoomNumbers() {
                throw new NotImplementedException();
            }

            public void UnassignGuestFromRoom(Guest guest, string roomNumber) {
                throw new NotImplementedException();
            }

            public List<Guest> GetGuestList() {
                throw new NotImplementedException();
            }

            public List<Room> GetAllEmptyRooms() {
                throw new NotImplementedException();
            }

            public List<Room> GetAllAssignedRooms() {
                return new List<Room>
                           {
                               new Room("1-1-1", new Point3D(1, 1, 1)),
                               new Room("1-1-2", new Point3D(1, 1, 2)),
                               new Room("3-1-1", new Point3D(3, 1, 1)),
                           };
            }

            public List<Room> GetRoomList() {
                throw new NotImplementedException();
            }

            public void AssignGuestToRoom(Guest guest, string roomNumber) {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}