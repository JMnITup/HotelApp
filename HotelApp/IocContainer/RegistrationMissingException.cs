#region

using System;

#endregion

namespace JamesMeyer.IocContainer {
    public class RegistrationMissingException : Exception {
        public RegistrationMissingException(string message, Exception innerException) : base(message, innerException) {}
    }
}