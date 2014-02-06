#region

using System;
using System.Collections.Generic;
using System.ServiceModel;
using HotelCorp.HotelApp.Services.Access;
using HotelCorp.HotelApp.Services.Engines;
using JamesMeyer.IocContainer;
using ServiceModelEx.Hosting;

#endregion

namespace HotelCorp.HotelApp.Services.Managers {
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class OccupancyManager_Wpf : IOccupancyManager_Wpf {
        private readonly InterfaceResolver _ioc = new InterfaceResolver();

        public OccupancyManager_Wpf() {
            _ioc.Register<IRoomAccess, RoomAccess>().AsDelegate(InProcFactory.CreateInstance<RoomAccess, IRoomAccess>);
            _ioc.Register<IReservingEngine, ReservingEngine>().AsDelegate(InProcFactory.CreateInstance<ReservingEngine, IReservingEngine>);
        }

        public OccupancyManager_Wpf(InterfaceResolver resolver) {
            _ioc = resolver;
        }

        #region Implementation of IOccupancyManager_Wpf

        public List<Room> GenerateBasicHotel(uint roomCountHorizontal, uint roomCountDepth, uint roomCountVerticle) {
            using (var roomAccessService = _ioc.Resolve<IRoomAccess>()) {
                roomAccessService.GenerateBasicHotel(roomCountHorizontal, roomCountDepth, roomCountVerticle);
            }
            return GetAllRooms();
        }

        public Room CheckinGuest(Guest guest, string roomNumber = null) {
            if (roomNumber == null) {
                using (var reservingEngine = _ioc.Resolve<IReservingEngine>()) {
                    return Repackage.Room(reservingEngine.PerformAutoCheckin(Repackage.Guest(guest)));
                }
            }
            return null;
        }

        public void CheckoutGuest(Guest guest, string roomNumber) {
            if (roomNumber != null) {
                using (var roomAccess = _ioc.Resolve<IRoomAccess>()) {
                    roomAccess.UnassignGuestFromRoom(Repackage.Guest(guest), roomNumber);
                }
            }
        }

        public void FindGuest(string firstName, string lastName) {
            throw new NotImplementedException();
        }

        public List<Room> GetAllRooms() {
            using (var roomAccessService = _ioc.Resolve<IRoomAccess>()) {
                roomAccessService.GetRoomList();
                List<Access.Room> accessRoomList = roomAccessService.GetRoomList();
                return Repackage.RoomList(accessRoomList);
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {}

        #endregion
    }
}