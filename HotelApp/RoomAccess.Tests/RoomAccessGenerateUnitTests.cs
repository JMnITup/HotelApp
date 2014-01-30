#region

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace HotelCorp.HotelApp.Services.Access.Tests {
    [TestClass]
    public class RoomAccessGenerateUnitTests {
        [TestMethod]
        [TestCategory("Unit")]
        public void InstantiateRoomAccessClass() {
            // Arrange

            // Act
            IRoomAccess service = new RoomAccess();

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Generate1DimensionalHotel() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            const uint x = 10;
            const uint y = 1;
            const uint z = 1;

            // Act
            service.GenerateBasicHotel(x, y, z);

            // Assert
            Assert.AreEqual((int) (x*y*z), RoomAccess.HotelMap.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Generated1DimensionalHotelHasUniqueRoomNumbers() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            const uint x = 10;
            const uint y = 1;
            const uint z = 1;

            // Act
            service.GenerateBasicHotel(x, y, z);

            // Assert
            VerifyThatHotelHasUniqueRoomNumbers(RoomAccess.HotelMap);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Generate2DimensionalHotel() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            const uint x = 10;
            const uint y = 10;
            const uint z = 1;

            // Act
            service.GenerateBasicHotel(x, y, z);

            // Assert
            Assert.AreEqual((int) (x*y*z), RoomAccess.HotelMap.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Generated2DimensionalHotelHasUniqueRoomNumbers() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            const uint x = 10;
            const uint y = 10;
            const uint z = 1;

            // Act
            service.GenerateBasicHotel(x, y, z);

            // Assert
            VerifyThatHotelHasUniqueRoomNumbers(RoomAccess.HotelMap);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Generate3DimensionalHotel() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            const uint x = 10;
            const uint y = 10;
            const uint z = 10;

            // Act
            service.GenerateBasicHotel(x, y, z);

            // Assert
            Assert.AreEqual((int) (x*y*z), RoomAccess.HotelMap.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Generated3DimensionalHotelHasUniqueRoomNumbers() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            const uint x = 10;
            const uint y = 10;
            const uint z = 10;

            // Act
            service.GenerateBasicHotel(x, y, z);

            // Assert
            VerifyThatHotelHasUniqueRoomNumbers(RoomAccess.HotelMap);
        }

        private void VerifyThatHotelHasUniqueRoomNumbers(List<Room> map) {
            Assert.IsFalse(RoomAccess.HotelMap.GroupBy(r => r.RoomNumber).Any(c => c.Count() > 1), "Duplicate room numbers found");
        }
    }
}