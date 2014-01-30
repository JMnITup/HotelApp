#region Using declarations

using System;
using Bridgepoint.Enterprise.Common.IocContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace IocContainer.Tests {
    [TestClass]
    public class ConstructorTests {
        [TestMethod]
        [TestCategory("Unit")]
        public void UsesDefaultConstructorWhenNotFirst() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IConstructorTestClass, ConstructorTestClassWithManyConstructors>();

            // Act
            var instance = resolver.Resolve<IConstructorTestClass>();

            // Assert
            Assert.AreEqual("default", instance.Value);
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void NonFirstConstructorWithMultipleArgumentsResolves() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IConstructorTestClass, ConstructorTestClassWithManyConstructors>()
                    .WithConstructor(new[] {typeof(string), typeof(int)}, new object[] {"something", 3});

            // Act
            var instance = resolver.Resolve<IConstructorTestClass>();

            // Assert
            Assert.AreEqual("string,int", instance.Value);
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void UsesInterfaceResolverConstructorOverDefault() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IConstructorTestClass, ConstructorTestClassWithDefaultAndResolver>();

            // Act
            var instance = resolver.Resolve<IConstructorTestClass>();

            // Assert
            Assert.AreEqual("InterfaceResolver", instance.Value);
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void FirstResolveWithNoDefaultResultsInFirstConstructor() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IConstructorTestClass, ConstructorTestClassWithNoDefault>()
                    .WithConstructorValue("string1", "test")
                    .WithConstructorValue("int1", 5);

            // Act
            var instance = resolver.Resolve<IConstructorTestClass>();

            // Assert
            Assert.AreEqual("string,int(first)", instance.Value);
        }

        
        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(RegistrationMissingException))]
        public void UnregisteredConstructorThrowsRegistrationException()
        {
            // Arrange
            var c = new InterfaceResolver();
            c.Register<IConstructorTestClass, ConstructorTestClassOnlyDefault>().WithConstructor(new Type[] {typeof(string)}, new object[] {""});

            // Act
            var resolve = c.Resolve<IConstructorTestClass>();

            // Assert
            Assert.Fail("An exception was not thrown when it should have been");
        }

        /************************************************* Nested classes ***********************************************/

        #region Nested type: ConstructorTestClassOnlyDefault

        public class ConstructorTestClassOnlyDefault : IConstructorTestClass {
            public ConstructorTestClassOnlyDefault() {
                Value = "default(first)";
            }

            #region IConstructorTestClass Members

            public string Value { get; set; }

            #endregion
        }

        #endregion

        #region Nested type: ConstructorTestClassWithDefaultAndResolver

        public class ConstructorTestClassWithDefaultAndResolver : IConstructorTestClass {
            public ConstructorTestClassWithDefaultAndResolver() {
                Value = "default(first)";
            }

            public ConstructorTestClassWithDefaultAndResolver(InterfaceResolver resolver) {
                Value = "InterfaceResolver";
            }

            #region IConstructorTestClass Members

            public string Value { get; set; }

            #endregion
        }

        #endregion

        #region Nested type: ConstructorTestClassWithManyConstructors

        public class ConstructorTestClassWithManyConstructors : IConstructorTestClass {
            public ConstructorTestClassWithManyConstructors(int int1) {
                Value = "int(first)";
            }

            public ConstructorTestClassWithManyConstructors(int int1, int int2) {
                Value = "int,int";
            }

            public ConstructorTestClassWithManyConstructors() {
                Value = "default";
            }

            public ConstructorTestClassWithManyConstructors(string string1, int int1) {
                Value = "string,int";
            }

            public ConstructorTestClassWithManyConstructors(int int1, string string1) {
                Value = "int,string";
            }

            #region Implementation of IConstructorTestClass

            #endregion

            #region IConstructorTestClass Members

            public string Value { get; set; }

            #endregion
        }

        #endregion

        #region Nested type: ConstructorTestClassWithNoDefault

        public class ConstructorTestClassWithNoDefault : IConstructorTestClass {
            public ConstructorTestClassWithNoDefault(string string1, int int1) {
                Value = "string,int(first)";
            }

            public ConstructorTestClassWithNoDefault(int int1, string string1) {
                Value = "int,string";
            }

            #region IConstructorTestClass Members

            public string Value { get; set; }

            #endregion
        }

        #endregion

        #region Nested type: IConstructorTestClass

        public interface IConstructorTestClass {
            string Value { get; set; }
        }

        #endregion
    }
}