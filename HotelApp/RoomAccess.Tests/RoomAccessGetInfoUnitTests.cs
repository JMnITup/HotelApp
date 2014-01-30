#region

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace HotelCorp.HotelApp.Services.Access.Tests {
    [TestClass]
    public class RoomAccessGetInfoUnitTests {
        [TestMethod]
        [TestCategory("Unit")]
        public void GetListOfRooms() {
            // Arrange
            IRoomAccess service = new RoomAccess();
            service.GenerateBasicHotel(10, 10, 10);

            // Act
            List<string> list = service.GetRoomNames();

            // Assert
            Assert.AreEqual(10*10*10, list.Count);
        }
    }
}