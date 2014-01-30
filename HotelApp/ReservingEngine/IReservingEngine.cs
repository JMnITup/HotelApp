﻿#region

using System;
using System.ServiceModel;
using HotelCorp.HotelApp.Services.Access;

#endregion

namespace HotelCorp.HotelApp.Services.Engines {
    [ServiceContract]
    public interface IReservingEngine : IDisposable {
        Room PerformAutoCheckin(Guest newGuest);
    }
}