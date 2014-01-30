#region

using System.Runtime.Serialization;

#endregion

namespace HotelCorp.HotelApp.Services.Access {
    [DataContract]
    public class Guest {
        [DataMember] public string FirstName;
        [DataMember] public string LastName;

        public Guest(string fname, string lname) {
            FirstName = fname;
            LastName = lname;
        }
    }
}