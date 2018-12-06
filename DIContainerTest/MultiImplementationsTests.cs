using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInjectionContainer;
using DIContainerTest.Implementations;
using DIContainerTest.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace DIContainerTest
{
    [TestClass]
    public class MultiImplementationsTests
    {
        private DIProvider provider;
        private DIConfiguration config;

        [TestInitialize]
        public void Initialize()
        {
            config = new DIConfiguration();
            config.Register<INothing, FirstImplementation>(true);
            config.Register<INothing, FirstImplementation2>(true);
            config.Register<IFirst, FirstImplementation3>(false);
            config.Register<AnotherFirst, AnotherFirst>(false);
            config.Register<AnotherFirst, AnotherFirst2>(false);
            config.Register<ISecond<IFirst>, SecondImplementation<IFirst>>(false);
            config.Register<ISecond<IFirst>, SecondImplementation2<IFirst>>(false);
            provider = new DIProvider(config);
        }

        [TestMethod]
        public void SimpleMultiImplTest()
        {
            var impls = provider.Resolve<IEnumerable<INothing>>();
            foreach (var impl in impls)
            {
                Assert.IsInstanceOfType(impl, typeof(INothing));
            }
        }

        [TestMethod]
        public void ClassInheritanceMultiImplTest()
        {
            var impls = provider.Resolve<IEnumerable<AnotherFirst>>();
            foreach (var impl in impls)
            {
                Assert.IsInstanceOfType(impl, typeof(AnotherFirst));
            }
        }

        [TestMethod]
        public void GenericClassMultiImplTest()
        {
            var impls = provider.Resolve<IEnumerable<ISecond<IFirst>>>();
            foreach (var impl in impls)
            {
                Assert.IsInstanceOfType(impl, typeof(ISecond<IFirst>));
            }
        }

        [TestMethod]
        public void MultiImplSingletonTest()
        {
            var impls = (IList)provider.Resolve<IEnumerable<INothing>>();
            Assert.ReferenceEquals(impls[0], impls[1]);
        }
    }
}
