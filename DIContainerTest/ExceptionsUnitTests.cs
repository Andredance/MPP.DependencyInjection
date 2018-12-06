using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInjectionContainer;
using DIContainerTest.Implementations;
using DIContainerTest.Interfaces;

namespace DIContainerTest
{
    [TestClass]
    public class ExceptionsUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TDependencyReferenceTypeExceptionTest()
        {
            DIConfiguration config = new DIConfiguration();
            config.Register<int, int>(false);
            DIProvider provider = new DIProvider(config);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TImplementationAbstratClassExceprionTest()
        {
            DIConfiguration config = new DIConfiguration();
            config.Register<INothing, AbstractClass>(false);
            DIProvider provider = new DIProvider(config);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void  InheritanceExceptionTest()
        {
            DIConfiguration config = new DIConfiguration();
            config.Register<INothing, AnotherFirst>(false);
            DIProvider provider = new DIProvider(config);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GenericArgsRegistrationExceptionTest()
        {
            DIConfiguration config = new DIConfiguration();
            config.Register<ISecond<IFirst>, SecondImplementation<IFirst>>(false);
            DIProvider provider = new DIProvider(config);
            provider.Resolve<ISecond<IFirst>>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConstructorExistanceExceptionTest()
        {
            DIConfiguration config = new DIConfiguration();
            config.Register<AnotherFirst, AnotherFirst>(false);
            DIProvider provider = new DIProvider(config);
            provider.Resolve<AnotherFirst>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RecursionControlExceptionTest()
        {
            DIConfiguration config = new DIConfiguration();
            config.Register<IFirst, FirstImplementation4>(false);
            config.Register<AnotherFirst, AnotherFirst2>(false);
            DIProvider provider = new DIProvider(config);
            provider.Resolve<AnotherFirst>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NotRegisteredDependencyExceptionTest()
        {
            DIConfiguration config = new DIConfiguration();
            DIProvider provider = new DIProvider(config);
            provider.Resolve<IFirst>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ImplementationCountExceptionTest()
        {
            DIConfiguration config = new DIConfiguration();
            config.Register<IFirst, FirstImplementation>(false);
            config.Register<IFirst, FirstImplementation2>(false);
            DIProvider provider = new DIProvider(config);
            provider.Resolve<IFirst>();
        }
    }
}
