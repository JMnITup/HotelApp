#region Using declarations

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using IocContainer;

#endregion

namespace Bridgepoint.Enterprise.Common.IocContainer {
    /// <summary>
    ///     Instanced interface resolver used to register and resolve requests for interface implementations - used by Assembly container
    /// </summary>
    public class InterfaceResolver {
        protected readonly ConcurrentDictionary<Type, string> NameDictionary = new ConcurrentDictionary<Type, string>();

        protected readonly ConcurrentDictionary<Tuple<Type, string>, Func<object>> ProviderDictionary =
            new ConcurrentDictionary<Tuple<Type, string>, Func<object>>();

        /// <summary>
        ///     Clears all registrations
        /// </summary>
        public void ClearRegistrations() {
            NameDictionary.Clear();
            ProviderDictionary.Clear();
        }

        /// <summary>
        ///     Registers an interface to a concrete class resolution
        /// </summary>
        /// <typeparam name="TS">Interface to be registered</typeparam>
        /// <typeparam name="TC">Concrete class to resolve to</typeparam>
        /// <returns>Registration object allowing for fluent interface modification of registration</returns>
        public Registration Register<TS, TC>() where TC : TS {
            return Register<TS, TC>(typeof(TS).FullName);
        }

        /// <summary>
        ///     Registers an interface to a concrete class resolution using a specific name, allows for registering the same interface to different concrete classes based on context
        /// </summary>
        /// <param name="name">Name to use</param>
        /// <typeparam name="TS">Interface to be registered</typeparam>
        /// <typeparam name="TC">Concrete class to resolve to</typeparam>
        /// <returns>Registration object allowing for fluent interface modification of registration</returns>
        public Registration Register<TS, TC>(string name) where TC : TS {
            if (!NameDictionary.ContainsKey(typeof(TS))) {
                NameDictionary[typeof(TS)] = name;
            }
            return new Registration(this, name, typeof(TC), typeof(TS));
        }


        /// <summary>
        ///     Resolves an interface request to the registered class, based on named registration
        /// </summary>
        /// <typeparam name="T">Interface to resolve</typeparam>
        /// <param name="name">Named registration to use</param>
        /// <returns>Instance of object registered to interface and name</returns>
        public T Resolve<T>(string name) where T : class {
            try {
                // TODO: Load test this to see if handling adds significant overhead - if so, unneeded
                return (T) ProviderDictionary[new Tuple<Type, string>(typeof(T), name)]();
            } catch (KeyNotFoundException ex) {
                throw new RegistrationMissingException("Interface " + typeof(T).FullName + ":" + name + " not registered, cannot resolve", ex);
            }
        }

        /// <summary>
        ///     Resolves an interface request to the registered class
        /// </summary>
        /// <typeparam name="T">Interface to resolve</typeparam>
        /// <exception cref="RegistrationMissingException"></exception>
        /// <returns>Instance of object registered to interface</returns>
        public T Resolve<T>() where T : class {
            // TODO: performance test catching this versus throwing as a KeyNotFoundException.  If difference is significant enough, it might be worth just throwing without handling
            try {
                return Resolve<T>(NameDictionary[typeof(T)]);
            } catch (KeyNotFoundException ex) {
                if (!NameDictionary.ContainsKey(typeof(T))) {
                    throw new RegistrationMissingException(
                        "Interface " + typeof(T).FullName + " not registered anonymously, cannot resolve anonymously",
                        ex);
                }
                throw new RegistrationMissingException(
                    "Interface " + typeof(T).FullName +
                    " not registered or needed constructor value is not defined, cannot resolve", ex);
            }
        }

        #region Nested type: Registration

        /// <summary>
        ///     Return type allowing for fluent interface usage of resolve syntax - e.g. Container.Register<ISomeInterface>.AsInstance(MockInstance);
        /// </summary>
        public class Registration {
            private readonly Dictionary<string, Func<object>> _args;
            private readonly InterfaceResolver _interfaceResolver;
            private readonly string _name;
            private readonly Type _concreteType;
            private readonly Type _interfaceType;

            internal Registration(InterfaceResolver interfaceResolver, string name, Type concreteType, Type interfaceType) {
                _interfaceResolver = interfaceResolver;
                _name = name;
                _concreteType = concreteType;
                _interfaceType = interfaceType;

                // TODO: Old line - ConstructorInfo c = concreteType.GetConstructors().First();

                ConstructorInfo c;
                c = concreteType.GetConstructor(new[] {typeof(InterfaceResolver)});
                if (c == null) {
                    c = concreteType.GetConstructor(new Type[] {});
                }
                if (c == null) {
                    c = concreteType.GetConstructors().First();
                }
                Debug.Assert(c != null, "Cannot resolve object with no constructors");

                _args = c.GetParameters()
                         .ToDictionary<ParameterInfo, string, Func<object>>(
                             x => x.Name,
                             x =>
                             (() =>
                              // TODO: Original line - interfaceResolver.ProviderDictionary[interfaceResolver.NameDictionary[x.ParameterType]]()
                              {
                                  Type pType = x.ParameterType;
                                  if (pType == typeof(InterfaceResolver)) {
                                      return interfaceResolver;
                                  }
                                  string nameMap = interfaceResolver.NameDictionary[pType];
                                  object provider = interfaceResolver.ProviderDictionary[new Tuple<Type, string>(pType, nameMap)]();
                                  return provider;
                              }
                             )
                    );
                interfaceResolver.ProviderDictionary[new Tuple<Type, string>(interfaceType, name)] = () => c.Invoke(_args.Values.Select(x => x()).ToArray());
            }


            /// <summary>
            ///     Registeres a specific object instance to be returned when interface is resolved
            /// </summary>
            /// <param name="instance">Object instance to register</param>
            /// <returns>Registration</returns>
            public Registration AsInstance(object instance) {
                _interfaceResolver.ProviderDictionary[new Tuple<Type, string>(_interfaceType, _name)] = () => instance;
                return this;
            }

            /// <summary>
            ///     Registers the class as a singleton - first request will instantiate the class, all further resolves will return the same instance
            /// </summary>
            /// <returns>Registration</returns>
            public Registration AsSingleton() {
                object value = null;
                Func<object> service = _interfaceResolver.ProviderDictionary[new Tuple<Type, string>(_interfaceType, _name)];
                _interfaceResolver.ProviderDictionary[new Tuple<Type, string>(_interfaceType, _name)] = () => value ?? (value = service());
                return this;
            }

            /// <summary>
            ///     Registers the class resolution as a function with a Func&lt;object&gt; return value
            /// </summary>
            /// <param name="function">Function to perform when type is resolved, must be a Func&lt;object&gt;</param>
            /// <returns>Registration</returns>
            public Registration AsDelegate(Func<object> function) {
                _interfaceResolver.ProviderDictionary[new Tuple<Type, string>(_interfaceType, _name)] = function;
                return this;
            }

            /* TODO: Rendered non-functional with changes, making functional requires slight re-tooling but I don't see a need to implement currently
             * /// <summary>
            ///     Defines internal dependency to use for resolution within object resolution
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="component"></param>
            /// <returns></returns>
            public Registration WithDependency(string parameter, string component) {
                // TODO: Find dependency name matching specified component - _args[parameter] = () => _interfaceResolver.ProviderDictionary[component]();
                return this;
            }
             */

            /// <summary>
            ///     Registers a class with parameters matching the First constructor of the concrete class - resolved instances will pass these parameters into constructor on resolution
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public Registration WithConstructorValue(string parameter, object value) {
                _args[parameter] = () => value;
                return this;
            }

            public Registration WithConstructor(Type[] types, object[] values) {
                Type t = _concreteType;
                ConstructorInfo c = t.GetConstructor(types);
                if (c == null) {
                    throw new RegistrationMissingException(
                        "Attempt to initialize " + _concreteType + ":" + _name + " with non-existant public constructor: " +
                        types, null);
                }
                _interfaceResolver.ProviderDictionary[new Tuple<Type, string>(_interfaceType, _name)] = () => c.Invoke(values);
                return this;
            }
        }

        #endregion
    }
}