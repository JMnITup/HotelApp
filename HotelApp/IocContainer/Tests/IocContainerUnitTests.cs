#region Using declarations

using System;
using Bridgepoint.Enterprise.Common.IocContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace IocContainer.Tests {
    [TestClass]
    public class IocContainerUnitTests {
        [TestMethod]
        [TestCategory("Unit")]
        public void ResolveAnonymousRegistrationWithNoDefinedConstructors() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeWithNoDefinedConstructors>();

            // Act
            var m = c.Resolve<IRootType>();

            // Assert
            Assert.AreEqual(0, m.GetFinalValue());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ResolveNamedRegistrationWithNoDefinedConstructors() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeWithNoDefinedConstructors>("named");

            // Act
            var m = c.Resolve<IRootType>("named");

            // Assert
            Assert.AreEqual(0, m.GetFinalValue());
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void ResolveSameNamedRegistrationOnDifferentInterfacesWithNoDefinedConstructors() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeWithNoDefinedConstructors>("named");
            c.Register<IRootType2, ConcreteType2WithNoDefinedConstructors>("named");

            // Act
            var m = c.Resolve<IRootType>("named");
            var n = c.Resolve<IRootType2>("named");

            // Assert
            Assert.AreEqual(0, m.GetFinalValue());
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void ResolveAnonymousSubDependency() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeWithNoDefinedConstructors>();
            c.Register<IDisplay, NodeDisplay>();

            // Act
            var m = c.Resolve<IDisplay>();

            // Assert
            Assert.AreEqual("$0.00", m.Format("C2"));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void WithConstructorValueBuildsWithNamedParameter() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeTwo>("named").WithConstructorValue("internalValue", 5);

            // Act
            int i = c.Resolve<IRootType>("named").GetFinalValue();

            // Assert
            Assert.AreEqual(5, i);
        }

        /* - Removed because WithDependency was removed for now
        [TestMethod]
        [TestCategory("Unit")]
        [Ignore]
        public void NamedSubDependencyFunctions() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeTwo>("seven").WithConstructorValue("internalValue", 7);
            c.Register<IRootType, ConcreteTypeTwo>("nine").WithConstructorValue("internalValue", 9);
            c.Register<IRootType, Combine>("add").WithDependency("m1", "seven").WithDependency("m2", "nine");

            // Act
            int i = c.Resolve<IRootType>("add").GetFinalValue();

            // Assert
            Assert.AreEqual(16, i);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [Ignore]
        public void NamedSubDependencyOutOfOrderWorks() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, Combine>("add").WithDependency("m1", "five").WithDependency("m2", "six");
            c.Register<IRootType, ConcreteTypeTwo>("five").WithConstructorValue("internalValue", 5);
            c.Register<IRootType, ConcreteTypeTwo>("six").WithConstructorValue("internalValue", 6);

            // Act
            int i = c.Resolve<IRootType>("add").GetFinalValue();

            // Assert
            Assert.AreEqual(11, i);
        }
         */


        [TestMethod]
        [TestCategory("Unit")]
        public void AnonymousNonSingletonDoNotResolveToSameObject() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeWithNoDefinedConstructors>().AsSingleton();

            // Act
            var resolve1 = c.Resolve<IRootType>();
            var resolve2 = c.Resolve<IRootType>();

            // Assert
            Assert.AreSame(resolve1, resolve2);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AnonymousSingletonResolvesToSameObject() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeWithNoDefinedConstructors>().AsSingleton();

            // Act
            var resolve1 = c.Resolve<IRootType>();
            var resolve2 = c.Resolve<IRootType>();

            // Assert
            Assert.AreSame(resolve1, resolve2);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void NamedSingletonResolvesToSameObject() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeWithNoDefinedConstructors>("namedRegistration").AsSingleton();

            // Act
            var resolve1 = c.Resolve<IRootType>("namedRegistration");
            var resolve2 = c.Resolve<IRootType>("namedRegistration");

            // Assert
            Assert.AreSame(resolve1, resolve2);
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void AnonymousInstanceResolvesToProvidedObject() {
            // Arrange
            var instance = new ConcreteTypeThree(28);
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeThree>().AsInstance(instance);
            instance.ChangeValue(55);

            // Act
            var resolve1 = c.Resolve<IRootType>();
            var resolve2 = c.Resolve<IRootType>();

            // Assert
            Assert.AreSame(resolve1, resolve2);
            Assert.AreSame(resolve1, instance);
            Assert.AreEqual(55, resolve1.GetFinalValue());
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void RegisteringClassResolvesCorrectly() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeWithNoDefinedConstructors>();

            // Act
            var resolve = c.Resolve<IRootType>();

            // Assert
            Assert.AreEqual(typeof(ConcreteTypeWithNoDefinedConstructors), resolve.GetType());
        }


        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(RegistrationMissingException))]
        public void ClearRegistrationClearsPreviouslyRegisteredClass() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeWithNoDefinedConstructors>();
            c.ClearRegistrations();

            // Act
            var resolve = c.Resolve<IRootType>();

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(RegistrationMissingException))]
        public void UnregisteredNamedClassWithRegisteredAnonThrowsException() {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IRootType, ConcreteTypeWithNoDefinedConstructors>();

            // Act
            var resolve = c.Resolve<IRootType>("unregisteredName");

            // Assert
            Assert.Fail("An exception was not thrown when it should have been");
        }


        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(RegistrationMissingException))]
        public void UnregisteredNamedClassWithoutRegisteredAnonThrowsException() {
            // Arrange
            var c = new InterfaceResolver();

            // Act
            var resolve = c.Resolve<IRootType>("unregisteredName");

            // Assert
            Assert.Fail("An exception was not thrown when it should have been");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void RegisterAsDelegate() {
            // Arrange
            var c = new InterfaceResolver();


            // Act
            InterfaceResolver.Registration registration = c.Register<IRootType, ConcreteTypeWithNoDefinedConstructors>().AsDelegate(() => null);

            // Assert
            Assert.AreEqual(typeof(InterfaceResolver.Registration), registration.GetType());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ResolveRegisteredDelegate() {
            // Arrange
            var c = new InterfaceResolver();
            const int i = 84;
            c.Register<IRootType, ConcreteTypeThree>().AsDelegate(() => new ConcreteTypeThree(i));

            // Act
            var resolve = c.Resolve<IRootType>();

            // Assert
            Assert.AreEqual(typeof(ConcreteTypeThree), resolve.GetType());
            Assert.AreEqual(i, resolve.GetFinalValue());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ResolveRegisteredDelegateWithNewFunctionReturnsDistinctObjects() {
            // Arrange
            var c = new InterfaceResolver();
            const int i = 84;
            c.Register<IRootType, ConcreteTypeThree>().AsDelegate(() => new ConcreteTypeThree(i));

            // Act
            var resolve = c.Resolve<IRootType>();
            var resolve2 = c.Resolve<IRootType>();

            // Assert
            Assert.AreNotSame(resolve, resolve2);
            Assert.AreEqual(i, resolve.GetFinalValue());
            Assert.AreEqual(i, resolve2.GetFinalValue());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ResolveRegisteredDelegateWithSpecificObjectReturnsSameObject() {
            // Arrange
            var c = new InterfaceResolver();
            const int i = 84;
            var d = new ConcreteTypeThree(i);
            c.Register<IRootType, ConcreteTypeThree>().AsDelegate(() => d);

            // Act
            var resolve = c.Resolve<IRootType>();
            var resolve2 = c.Resolve<IRootType>();

            // Assert
            Assert.AreSame(resolve, resolve2);
            Assert.AreEqual(i, resolve.GetFinalValue());
            Assert.AreEqual(i, resolve2.GetFinalValue());
        }

        #region Nested type: Combine

        public class Combine : IRootType {
            private readonly IRootType _m1;
            private readonly IRootType _m2;

            public Combine(IRootType m1, IRootType m2) {
                _m1 = m1;
                _m2 = m2;
            }

            #region IRootType Members

            public int GetFinalValue() {
                return _m1.GetFinalValue() + _m2.GetFinalValue();
            }

            public void ChangeValue(int newValue) {
                throw new Exception("Pointless but purposeful disallowing of ChangeValue");
            }

            #endregion
        }

        #endregion

        #region Nested type: ConcreteType2WithNoDefinedConstructors

        public class ConcreteType2WithNoDefinedConstructors : IRootType2 {
            #region IRootType2 Members

            public int GetFinalValue() {
                return 0;
            }

            public void ChangeValue(int newValue) {
                throw new Exception("Pointless but purposeful disallowing of ChangeValue");
            }

            #endregion
        }

        #endregion

        #region Nested type: ConcreteTypeThree

        public class ConcreteTypeThree : IRootType {
            private int _internalValue;

            public ConcreteTypeThree(int internalValue) {
                _internalValue = internalValue;
            }

            #region IRootType Members

            public int GetFinalValue() {
                return _internalValue;
            }

            public void ChangeValue(int newValue) {
                _internalValue = newValue;
            }

            #endregion
        }

        #endregion

        #region Nested type: ConcreteTypeTwo

        public class ConcreteTypeTwo : IRootType {
            private readonly int _internalValue;

            public ConcreteTypeTwo(int internalValue) {
                _internalValue = internalValue;
            }

            #region IRootType Members

            public int GetFinalValue() {
                return _internalValue;
            }

            public void ChangeValue(int newValue) {
                throw new Exception("Pointless but purposeful disallowing of ChangeValue");
            }

            #endregion
        }

        #endregion

        #region Nested type: ConcreteTypeWithNoDefinedConstructors

        public class ConcreteTypeWithNoDefinedConstructors : IRootType {
            #region IRootType Members

            public int GetFinalValue() {
                return 0;
            }

            public void ChangeValue(int newValue) {
                throw new Exception("Pointless but purposeful disallowing of ChangeValue");
            }

            #endregion
        }

        #endregion

        #region Nested type: IConstructorTestClass

        public interface IConstructorTestClass {
            string Value { get; set; }
        }

        #endregion

        #region Nested type: IDisplay

        public interface IDisplay {
            string Format(string format);
        }

        #endregion

        #region Nested type: IRootType

        public interface IRootType {
            int GetFinalValue();
            void ChangeValue(int newValue);
        }

        #endregion

        #region Nested type: IRootType2

        public interface IRootType2 {
            int GetFinalValue();
            void ChangeValue(int newValue);
        }

        #endregion

        #region Nested type: NodeDisplay

        public class NodeDisplay : IDisplay {
            private readonly IRootType _nodeType;

            public NodeDisplay(IRootType nodeType) {
                _nodeType = nodeType;
            }

            #region IDisplay Members

            public string Format(string format) {
                return _nodeType.GetFinalValue().ToString(format);
            }

            #endregion
        }

        #endregion

        /************************************************* Nested classes ***********************************************/
    }
}