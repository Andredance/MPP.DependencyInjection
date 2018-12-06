using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DIConfiguration
    {
        internal Dictionary<Type, List<Type>> DContainer { get; set; }
        internal Dictionary<Type, bool> IsSingleton { get; set; }
        internal Dictionary<Type, SingletonContainer> SContainer { get; set; }

        public void Register<TDependency, TImplementation>(bool isSingleton)
        {
            Type tDependency = typeof(TDependency);
            Type tImplementation = typeof(TImplementation);

            if (!DContainer.ContainsKey(tDependency))
            {
                DContainer[tDependency] = new List<Type>();
            }

            if (!DContainer[tDependency].Contains(tImplementation))
            {
                DContainer[tDependency].Add(tImplementation);
            }

            IsSingleton[tDependency] = isSingleton;
            
            if (IsSingleton[tDependency])
            {
                SContainer[tDependency] = new SingletonContainer(tImplementation);
            }
        }

        public void Register(Type tDependency, Type tImplementation)
        {
            if (!DContainer.ContainsKey(tDependency))
            {
                DContainer[tDependency] = new List<Type>();
            }

            if (!DContainer[tDependency].Contains(tImplementation))
            {
                DContainer[tDependency].Add(tImplementation);
            }

            IsSingleton[tDependency] = false;
        }

        public DIConfiguration()
        {
            DContainer = new Dictionary<Type, List<Type>>();
            SContainer = new Dictionary<Type, SingletonContainer>();
            IsSingleton = new Dictionary<Type, bool>();
        }
    }
}
