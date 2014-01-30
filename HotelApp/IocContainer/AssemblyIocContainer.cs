namespace Bridgepoint.Enterprise.Common.IocContainer {

    #region

    #endregion

    /// <summary>
    /// </summary>
    public class AssemblyIocContainer {
        /// <summary>
        ///     static instance of resolver and static methods to access it - used for global interface resolution
        /// </summary>
        public static readonly InterfaceResolver Resolver = new InterfaceResolver();

        static AssemblyIocContainer() {
            DefaultRegistration();
        }

        // TODO: update to make this the only aspect to exist in each relevant project
        protected static void DefaultRegistration() {}

        /// <summary>
        ///     Resets container to default state
        /// </summary>
        public static void ResetContainer() {
            Resolver.ClearRegistrations();
            DefaultRegistration();
        }


        /// <summary>
        ///     Registers an interface to a concrete class resolution
        /// </summary>
        /// <typeparam name="TS">Interface to be registered</typeparam>
        /// <typeparam name="TC">Concrete class to resolve to</typeparam>
        /// <returns>Registration object allowing for fluent interface modification of registration</returns>
        public static InterfaceResolver.Registration Register<TS, TC>() where TC : TS {
            return Resolver.Register<TS, TC>();
        }

        /// <summary>
        ///     Registers an interface to a concrete class resolution using a specific name, allows for registering the same interface to different concrete classes based on context
        /// </summary>
        /// <param name="name">Name to use</param>
        /// <typeparam name="TS">Interface to be registered</typeparam>
        /// <typeparam name="TC">Concrete class to resolve to</typeparam>
        /// <returns>Registration object allowing for fluent interface modification of registration</returns>
        public static InterfaceResolver.Registration Register<TS, TC>(string name) where TC : TS {
            return Resolver.Register<TS, TC>(name);
        }

        /// <summary>
        ///     Resolves an interface request to the registered class, based on named registration
        /// </summary>
        /// <typeparam name="T">Interface to resolve</typeparam>
        /// <param name="name">Named registration to use</param>
        /// <returns>Instance of object registered to interface and name</returns>
        public static T Resolve<T>(string name) where T : class {
            return Resolver.Resolve<T>(name);
        }

        /// <summary>
        ///     Resolves an interface request to the registered class
        /// </summary>
        /// <typeparam name="T">Interface to resolve</typeparam>
        /// <returns>Instance of object registered to interface</returns>
        public static T Resolve<T>() where T : class {
            return Resolver.Resolve<T>();
        }
    }
}