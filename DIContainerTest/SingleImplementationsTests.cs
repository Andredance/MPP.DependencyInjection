using System;
using System.Collections.Generic;
using DependencyInjectionContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using DIContainerTest.Interfaces;
using DIContainerTest.Implementations;

namespace DependencyInjectionContainerUnitTests
{
    [TestClass]
    public class DependencyProviderUnitTestsClass
    {
        private DIProvider provider;
        private DIConfiguration config;

        [TestInitialize]
        public void Initialize()
        {
            config = new DIConfiguration();
            config.Register<IFirst, FirstImplementation>(false);
            config.Register<AnotherFirst, AnotherFirst>(false);
            config.Register<ISecond<IFirst>, SecondImplementation<IFirst>>(false);
            config.Register<IThird<IFirst, ISecond<IFirst>>, ThirdImplementation<IFirst, ISecond<IFirst>>>(false);
            //config.Register<AnotherSecond<IFirst>, AnotherSecond<IFirst>>(false);
            config.Register(typeof(AnotherSecond<>), typeof(AnotherSecond<>));
            config.Register<INothing, FirstImplementation2>(true);
            config.Register<ISecond<INothing>, SecondImplementation2<INothing>>(true);
            provider = new DIProvider(config);
        }

        [TestMethod]
        public void SingleDependencyTest()
        {
            var first = provider.Resolve<IFirst>();
            Assert.IsInstanceOfType(first, typeof(IFirst));
        }

        [TestMethod]
        public void DoubleDependencyTest()
        {
            var anotherFirst = provider.Resolve<AnotherFirst>();
            Assert.IsInstanceOfType(anotherFirst.FirstImpl, typeof(IFirst));
        }

        [TestMethod]
        public void SimpleGenericClassTest()
        {
            var second =(SecondImplementation<IFirst>)provider.Resolve<ISecond<IFirst>>();
            Assert.IsInstanceOfType(second.SecondImpl, typeof(IFirst));
        }

        [TestMethod]
        public void SelfRegistrationTest()
        {
            var anotherFirst = provider.Resolve<AnotherFirst>();
            Assert.IsInstanceOfType(anotherFirst, typeof(AnotherFirst));
        }

        [TestMethod]
        public void ComplicatedGenericClassTest()
        {
            var third = (ThirdImplementation<IFirst, ISecond<IFirst>>)provider.Resolve<IThird<IFirst, ISecond<IFirst>>>();
            Assert.IsInstanceOfType(third.ThirdImpl1, typeof(IFirst));
            Assert.IsInstanceOfType(third.ThirdImpl2, typeof(ISecond<IFirst>));
            Assert.IsInstanceOfType(((SecondImplementation<IFirst>)third.ThirdImpl2).SecondImpl, typeof(IFirst));
        }

        [TestMethod]
        public void GenericConstructorParameterTest()
        {
            var anotherSecond = provider.Resolve<AnotherSecond<IFirst>>();
            Assert.IsInstanceOfType(anotherSecond, typeof(AnotherSecond<IFirst>));
            Assert.IsInstanceOfType(anotherSecond.SecondParamImpl, typeof(ISecond<IFirst>));
        }

        [TestMethod]
        public void SimpleMultiImplementationTest()
        {
            var iFirstImpls = provider.Resolve<IEnumerable<IFirst>>();
            foreach (var impl in iFirstImpls)
            {
                Assert.IsInstanceOfType(impl, typeof(IFirst));
            }
        }

        [TestMethod]
        public void SimpleSingletonTest()
        {
            var nothing1 = provider.Resolve<INothing>();
            var nothing2 = provider.Resolve<INothing>();
            Assert.ReferenceEquals(nothing1, nothing2);
        }

        [TestMethod]
        public void GenericSingletonTest()
        {
            var iSecondNothing1 = provider.Resolve<ISecond<INothing>>();
            var iSecondNothing2 = provider.Resolve<ISecond<INothing>>();
            Assert.ReferenceEquals(iSecondNothing1, iSecondNothing2);
            Assert.ReferenceEquals(((SecondImplementation2<INothing>)iSecondNothing1).SecondImpl,
                ((SecondImplementation2<INothing>)iSecondNothing2).SecondImpl);
        }
    }
}