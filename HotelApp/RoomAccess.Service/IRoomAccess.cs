#region

using System.ServiceModel;

#endregion

namespace HotelCorp.HotelApp.Services.Access {
    [ServiceContract]
    public interface IRoomAccess {
        [OperationContract]
        void GenerateBasicHotel(uint xSize, uint ySize, uint zSize);
    }
}