﻿#region

using System.Collections.Generic;
using System.ServiceModel;

#endregion

namespace HotelCorp.HotelApp.Services.Access {
    [ServiceContract]
    public interface IRoomAccess {
        [OperationContract]
        void GenerateBasicHotel(uint xSize, uint ySize, uint zSize);

        [OperationContract]
        List<string> GetRoomNames();

        [OperationContract]
        void AssignGuestToRoom(Guest guest, string roomNumber);

        [OperationContract]
        void UnassignGuestFromRoom(Guest guest, string roomNumber);
    }
}