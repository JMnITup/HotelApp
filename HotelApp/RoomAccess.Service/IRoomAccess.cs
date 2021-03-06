﻿#region

using System;
using System.Collections.Generic;
using System.ServiceModel;

#endregion

namespace HotelCorp.HotelApp.Services.Access {
    [ServiceContract]
    public interface IRoomAccess : IDisposable {
        [OperationContract]
        void GenerateBasicHotel(uint xSize, uint ySize, uint zSize);

        [OperationContract]
        List<string> GetRoomNumbers();

        [OperationContract]
        Room AssignGuestToRoom(Guest guest, string roomNumber);

        [OperationContract]
        void UnassignGuestFromRoom(Guest guest, string roomNumber);

        [OperationContract]
        List<Guest> GetGuestList();

        [OperationContract]
        List<Room> GetAllEmptyRooms();

        [OperationContract]
        List<Room> GetAllAssignedRooms();

        [OperationContract]
        List<Room> GetRoomList();
    }
}