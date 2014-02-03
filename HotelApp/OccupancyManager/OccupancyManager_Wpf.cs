﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media.Media3D;
using HotelCorp.HotelApp.Services.Access;
using JamesMeyer.IocContainer;
using ServiceModelEx.Hosting;

namespace HotelCorp.HotelApp.Services.Managers {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "OccupancyManager_Wpf" in both code and config file together.
    public class OccupancyManager_Wpf : IOccupancyManager_Wpf {
        private readonly InterfaceResolver _ioc = new InterfaceResolver();

        public OccupancyManager_Wpf() {
            _ioc.Register<IRoomAccess, RoomAccess>().AsDelegate(InProcFactory.CreateInstance<RoomAccess, IRoomAccess>);
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

        public void CheckingGuest(Guest guest, string roomNumber) {
            throw new NotImplementedException();
        }

        public void CheckoutGuest(Guest guest, string roomNumber) {
            throw new NotImplementedException();
        }

        public void FindGuest(string firstName, string lastName) {
            throw new NotImplementedException();
        }

        public List<Room> GetAllRooms() {
            using (var roomAccessService = _ioc.Resolve<IRoomAccess>()) {
                roomAccessService.GetRoomList();
                var accessRoomList = roomAccessService.GetRoomList();
                return Repackage.RoomList(accessRoomList);
            }
        }

        #endregion
    }


}
