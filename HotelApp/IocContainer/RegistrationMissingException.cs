#region Using declarations

using System;

#endregion

namespace IocContainer {
    public class RegistrationMissingException : Exception {
        public RegistrationMissingException(string message, Exception innerException) : base(message, innerException) {}
    }
}