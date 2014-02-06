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

}