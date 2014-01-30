#region

using System.Collections.Generic;
using System.Windows.Media.Media3D;

#endregion

namespace HotelCorp.HotelApp.Services.Access {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RoomAccess" in both code and config file together.
    public class RoomAccess : IRoomAccess {
        /// <summary>
        ///     Represents an underlying data store.  This could be replaced with any store implementation
        /// </summary>
        public static List<Room> HotelMap = new List<Room>();

        public void GenerateBasicHotel(uint xSize, uint ySize, uint zSize) {
            HotelMap = new List<Room>();
            for (int x = 1; x <= xSize; x++) {
                for (int y = 1; y <= ySize; y++) {
                    for (int z = 1; z <= zSize; z++) {
                        string roomNumber = x + "-" + y + "-" + z;
                        HotelMap.Add(new Room(roomNumber, new Point3D(x, y, z)));
                    }
                }
            }
        }
    }

    public class Room {
        public Guest Guest;
        public Point3D Location;
        public string RoomNumber;

        public Room(string roomNumber, Point3D location) {
            RoomNumber = roomNumber;
            Location = location;
        }
    }

    public class Guest {
        public string FirstName;
        public string LastName;
    }
}