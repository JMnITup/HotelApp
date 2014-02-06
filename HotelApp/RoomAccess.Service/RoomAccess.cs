#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Windows.Media.Media3D;
using JamesMeyer.IocContainer;

#endregion

namespace HotelCorp.HotelApp.Services.Access {
    public class RoomAccess : IRoomAccess {
        /// <summary>
        ///     Represents an underlying data store.  This could be replaced with any store implementation
        /// </summary>
        public static List<Room> HotelMap = new List<Room>();

        private readonly InterfaceResolver _ioc = new InterfaceResolver();

        public RoomAccess() {}

        public RoomAccess(InterfaceResolver resolver) {
            _ioc = resolver;
        }

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

        public List<string> GetRoomNumbers() {
            return HotelMap.ConvertAll(n => n.RoomNumber);
        }

        public Room AssignGuestToRoom(Guest guest, string roomNumber) {
            VerifyGuest(guest);
            Room room = GetRoomByRoomNumber(roomNumber);
            room.Guest = guest;
            return room;
        }

        public void UnassignGuestFromRoom(Guest guest, string roomNumber) {
            VerifyGuest(guest);
            Room room = GetRoomByRoomNumber(roomNumber);
            if (!IsSpecifiedGuestInRoom(guest, room)) {
                throw new FaultException("Guest is not checked into room");
            }
            room.Guest = null;
        }

        public List<Guest> GetGuestList() {
            return HotelMap.FindAll(room => room.Guest != null).ConvertAll(input => input.Guest);
        }

        public List<Room> GetAllEmptyRooms() {
            return HotelMap.FindAll(room => room.Guest == null);
        }

        public List<Room> GetAllAssignedRooms() {
            return HotelMap.FindAll(room => room.Guest != null);
        }

        public List<Room> GetRoomList() {
            return HotelMap;
        }

        protected bool IsSpecifiedGuestInRoom(Guest guest, Room room) {
            if (guest.FirstName == room.Guest.FirstName && guest.LastName == room.Guest.LastName) {
                return true;
            }
            return false;
        }

        protected Room GetRoomByRoomNumber(string roomNumber) {
            List<Room> rooms = HotelMap.FindAll(room => room.RoomNumber == roomNumber);
            if (rooms.Count < 1) {
                throw new FaultException("Requested room does not exist");
            }
            if (rooms.Count > 1) {
                throw new FaultException("There are multiple rooms with that number, cannot proceed");
            }
            return rooms[0];
        }

        protected void VerifyGuest(Guest guest) {
            if (guest == null) {
                throw new FaultException("No guest provided");
            }
            if (guest.FirstName.Trim() == "" && guest.LastName.Trim() == "") {
                throw new FaultException("Guests must have a name");
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {}

        #endregion
    }
}