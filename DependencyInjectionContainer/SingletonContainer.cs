using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class SingletonContainer
    {
        private readonly object syncRoot = new object();
        private volatile object instance;
        private Type implementationType;

        public SingletonContainer(Type implType)
        {
            instance = null;
            implementationType = implType;
        }

        public object GetInstance(DIProvider provider)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = provider.CreateInstance(implementationType);
                }
            }
            return instance;
        }
    }
}
