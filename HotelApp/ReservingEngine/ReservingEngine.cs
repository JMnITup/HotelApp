#region

using System;
using System.Collections.Generic;
using System.Transactions;
using System.Windows.Media.Media3D;
using HotelCorp.HotelApp.Services.Access;
using JamesMeyer.IocContainer;
using ServiceModelEx;

#endregion

namespace HotelCorp.HotelApp.Services.Engines {
    public class ReservingEngine : IReservingEngine {
        public InterfaceResolver Ioc = new InterfaceResolver();

        public ReservingEngine() {
            Ioc.Register<IRoomAccess, RoomAccess>().AsDelegate(InProcFactory.CreateInstance<RoomAccess, IRoomAccess>);
        }

        public ReservingEngine(InterfaceResolver resolver) {
            Ioc = resolver;
        }

        #region Implementation of IReservingEngine

        #endregion

        #region Implementation of IReservingEngine

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {}

        public Room PerformAutoCheckin(Guest newGuest) {
            Room bestRoom = null;
            using (var transaction = new TransactionScope()) {
                using (var roomAccess = Ioc.Resolve<IRoomAccess>()) {
                    List<Room> emptyList = roomAccess.GetAllEmptyRooms();
                    if (emptyList.Count <= 0) {
                        throw new Exception("No vacancy, cannot checkin guest");
                    }

                    List<Room> filledList = roomAccess.GetAllAssignedRooms();

                    if (filledList.Count == 0) {
                        bestRoom = GetPreferredInitialRoom(emptyList);
                    } else {
                        bestRoom = GetFurthestEmptyRoom(emptyList, filledList);
                    }

                    roomAccess.AssignGuestToRoom(newGuest, bestRoom.RoomNumber);
                }
            }
            return bestRoom;
        }

        private Room GetPreferredInitialRoom(List<Room> roomList) {
            double maxDistance = 0;
            Room bestRoom = null;
            foreach (Room emptyRoom in roomList) {
                foreach (Room emptyRoom2 in roomList) {
                    double dist = FindDistanceBetweenRooms(emptyRoom, emptyRoom2);
                    if (dist > maxDistance) {
                        maxDistance = dist;
                        bestRoom = emptyRoom;
                    }
                }
            }
            return bestRoom;
        }

        protected Room GetFurthestEmptyRoom(List<Room> emptyList, List<Room> filledList) {
            double highestMinDistance = -1;
            Room bestRoom = null;
            foreach (Room emptyRoom in emptyList) {
                double minRoomDistance = -1;
                foreach (Room filledRoom in filledList) {
                    double dist = FindDistanceBetweenRooms(emptyRoom, filledRoom);
                    if (minRoomDistance < 0) {
                        minRoomDistance = dist;
                    }
                    if (dist <= highestMinDistance) {
                        minRoomDistance = dist;
                        break;
                    }
                }
                if (minRoomDistance > highestMinDistance) {
                    highestMinDistance = minRoomDistance;
                    bestRoom = emptyRoom;
                }
            }
            return bestRoom;
        }

        private double FindDistanceBetweenRooms(Room room1, Room room2) {
            Vector3D x = room1.Location - room2.Location;
            return Math.Abs(x.Length);
        }

        #endregion
    }
}