using HotelCorp.HotelApp.Services.Access;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void PerformCheckin() {
            // Arrange
            IRoomAccess roomAccess = new RoomAccess();
            roomAccess.GenerateBasicHotel(2, 2, 2);
            var guest = new Guest(fname: "Some", lname: "Body");
            string roomNumber = roomAccess.GetRoomNumbers()[4];

            IReservingEngine service = new ReservingEngine();
            

            // Act
            //service.GetHotelMap();

            // Assert
            Assert.IsNotNull(service);
            Assert.Fail("Incomplete");
        }
    }
}
