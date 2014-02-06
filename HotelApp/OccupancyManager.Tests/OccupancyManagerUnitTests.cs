#region

using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using HotelCorp.HotelApp.Services.Access;
using JamesMeyer.IocContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace HotelCorp.HotelApp.Services.Managers.Tests {
    [TestClass]
    public class OccupancyManagerUnitTests {
        [ClassInitialize]
        public static void ClassInit(TestContext context) {}

        [TestMethod]
        public void InstantiateOccupancyManager_Wcf() {
            // Arrange

            // Act
            var manager = new OccupancyManager_Wpf();

            // Assert
            Assert.IsNotNull(manager);
        }

        [TestMethod]
        public void GenerateBasicHotel_2x3x2() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IRoomAccess, RoomAccess>();

            var manager = new OccupancyManager_Wpf(resolver);

            // Act
            manager.GenerateBasicHotel(2, 3, 2);

            // Assert
            List<Room> rooms = manager.GetAllRooms();
            Assert.AreEqual(2*3*2, rooms.Count);
        }
    }

    public class MockRoomAccess : IRoomAccess {
        #region Implementation of IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {}

        #endregion

        #region Implementation of IRoomAccess

        public void GenerateBasicHotel(uint xSize, uint ySize, uint zSize) {}

        public List<string> GetRoomNumbers() {
            throw new NotImplementedException();
        }

        public void UnassignGuestFromRoom(Access.Guest guest, string roomNumber) {
            throw new NotImplementedException();
        }

        public List<Access.Guest> GetGuestList() {
            throw new NotImplementedException();
        }

        public List<Access.Room> GetAllEmptyRooms() {
            throw new NotImplementedException();
        }

        public List<Access.Room> GetAllAssignedRooms() {
            throw new NotImplementedException();
        }

        public List<Access.Room> GetRoomList() {
            return new List<Access.Room>
                       {
                           new Access.Room("1-1-1", new Point3D(1, 1, 1)),
                           new Access.Room("1-1-2", new Point3D(1, 1, 2)),
                           new Access.Room("2-1-1", new Point3D(2, 1, 1))
                       };
        }

        public void AssignGuestToRoom(Access.Guest guest, string roomNumber) {
            throw new NotImplementedException();
        }

        #endregion
    }
}