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
        private Type type;

        public SingletonContainer(Type type)
        {
            this.type = type;
            instance = null;
        }

        public object GetInstance(DIProvider provider)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = provider.Resolve(type);
                }
            }
            return instance;
        }
    }
}
