#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace HotelCorp.HotelApp.Services.Managers {
    public class Repackage {
        public static Room Room(Access.Room from) {
            if (@from == null) {
                return null;
            }
            var toRoom = new Room(@from.RoomNumber, @from.Location) {Guest = Guest(@from.Guest)};
            return toRoom;
        }

        public static List<Room> RoomList(List<Access.Room> from) {
            if (@from == null) {
                return null;
            }
            List<Room> list = @from.Select(Room).ToList();
            return list;
        }

        public static Guest Guest(Access.Guest from) {
            if (@from == null) {
                return null;
            }
            var toGuest = new Guest(@from.FirstName, @from.LastName);
            return toGuest;
        }

        public static Access.Guest Guest(Guest from) {
            if (@from == null) {
                return null;
            }
            var toGuest = new Access.Guest(@from.FirstName, @from.LastName);
            return toGuest;
        }
    }
}