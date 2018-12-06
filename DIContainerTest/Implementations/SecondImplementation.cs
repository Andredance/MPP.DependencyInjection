using DIContainerTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainerTest.Implementations
{
    class SecondImplementation<T> : ISecond<T>
    {
        public T SecondImpl { get; set; }

        public SecondImplementation(T secondImpl)
        {
            SecondImpl = secondImpl;
        }
    }
}
