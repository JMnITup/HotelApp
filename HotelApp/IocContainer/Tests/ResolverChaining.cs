#region Using declarations

using Bridgepoint.Enterprise.Common.IocContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace IocContainer.Tests {
    [TestClass]
    public class ResolverChaining {
        [TestMethod]
        [TestCategory("Unit")]
        public void ConstructChainedResolverClassWithResolver() {
            // Arrange
            var resolver = new InterfaceResolver();

            // Act
            var chainedResolverClass = new ChainedResolverClass(resolver);

            // Assert
            Assert.IsNotNull(chainedResolverClass);
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void ConstructChainedResolverClassWithoutResolver() {
            // Arrange

            // Act
            var chainedResolverClass = new ChainedResolverClass();

            // Assert
            Assert.IsNotNull(chainedResolverClass);
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void ChainedResolverClassWithDefaultConstructorResolvesNewInstance() {
            // Arrange
            IChainedResolverClass chainedResolverClass = new ChainedResolverClass();

            // Act
            IChainedResolverClass newResolver = chainedResolverClass.GetNewChainedResolverClass();

            // Assert
            Assert.IsNotNull(newResolver);
            Assert.AreNotSame(chainedResolverClass, newResolver);
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void ChainedResolverClassWithResolverConstructorResolvesNewInstance() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IChainedResolverClass, ChainedResolverClass>();
            IChainedResolverClass chainedResolverClass = new ChainedResolverClass(resolver);

            // Act
            IChainedResolverClass newResolver = chainedResolverClass.GetNewChainedResolverClass();

            // Assert
            Assert.IsNotNull(newResolver);
            Assert.AreNotSame(chainedResolverClass, newResolver);
        }


        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(RegistrationMissingException))]
        public void ChainedResolverClassWithResolverWithUnregisteredDependencyFails() {
            // Arrange
            var resolver = new InterfaceResolver();
            IChainedResolverClass chainedResolverClass = new ChainedResolverClass(resolver);

            // Act
            IChainedResolverClass newResolver = chainedResolverClass.GetNewChainedResolverClass();

            // Assert
            Assert.Fail("Exception expected");
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void ChainedResolverClassWithDefaultConstructorPropegatesResolverDownward() {
            // Arrange
            IChainedResolverClass chainedResolverClass = new ChainedResolverClass();

            // Act
            IChainedResolverClass newResolver =
                chainedResolverClass.GetNewChainedResolverClass().GetNewChainedResolverClass();

            // Assert
            Assert.AreSame(chainedResolverClass.GetResolver(), newResolver.GetResolver());
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void ChainedResolverClassWithResolverConstructorPropegatesResolverDownward() {
            // Arrange
            var resolver = new InterfaceResolver();
            resolver.Register<IChainedResolverClass, ChainedResolverClass>();
            IChainedResolverClass chainedResolverClass = new ChainedResolverClass(resolver);

            // Act
            /*IChainedResolverClass newResolver =
                chainedResolverClass.GetNewChainedResolverClass().GetNewChainedResolverClass();*/
            IChainedResolverClass newResolver =
                chainedResolverClass.GetNewChainedResolverClass();
            newResolver = newResolver.GetNewChainedResolverClass();

            // Assert
            Assert.AreSame(resolver, newResolver.GetResolver());
            Assert.AreSame(chainedResolverClass.GetResolver(), newResolver.GetResolver());
        }


        /************************************************* Nested classes ***********************************************/

        #region Nested type: ChainedResolverClass

        public class ChainedResolverClass : IChainedResolverClass {
            protected readonly InterfaceResolver Resolver;


            public ChainedResolverClass(InterfaceResolver resolver) {
                Resolver = resolver;
            }

            public ChainedResolverClass() {
                Resolver = new InterfaceResolver();
                Resolver.Register<IChainedResolverClass, ChainedResolverClass>();
            }

            #region Implementation of IChainedResolverClass

            public IChainedResolverClass GetNewChainedResolverClass() {
                return Resolver.Resolve<IChainedResolverClass>();
            }

            public InterfaceResolver GetResolver() {
                return Resolver;
            }

            #endregion
        }

        #endregion

        #region Nested type: IChainedResolverClass

        public interface IChainedResolverClass {
            IChainedResolverClass GetNewChainedResolverClass();
            InterfaceResolver GetResolver();
        }

        #endregion
    }
}