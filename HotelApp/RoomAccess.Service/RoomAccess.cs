﻿#region

using System;
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

        public List<string> GetRoomNames() {
            return HotelMap.ConvertAll(n => n.RoomNumber);
        }

        public void AssignGuestToRoom(Guest guest, string roomNumber) {
            VerifyGuest(guest);
            Room room = GetRoomByRoomNumber(roomNumber);
            room.Guest = guest;
        }

        public void UnassignGuestFromRoom(Guest guest, string roomNumber) {
            VerifyGuest(guest);
            Room room = GetRoomByRoomNumber(roomNumber);
            if (!IsSpecifiedGuestInRoom(guest, room)) {
                throw new Exception("Guest is not checked into room");
            }
            room.Guest = null;
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
                throw new Exception("Requested room does not exist");
            }
            if (rooms.Count > 1) {
                throw new Exception("There are multiple rooms with that number, cannot proceed");
            }
            return rooms[0];
        }

        protected void VerifyGuest(Guest guest) {
            if (guest == null) {
                throw new Exception("No guest provided");
            }
            if (guest.FirstName.Trim() == "" && guest.LastName.Trim() == "") {
                throw new Exception("Guests must have a name");
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

        public Guest(string fname, string lname) {
            FirstName = fname;
            LastName = lname;
        }
    }
}