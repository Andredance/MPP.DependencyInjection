using DIContainerTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainerTest.Implementations
{
    class SecondImplementation2<T> : ISecond<T>
    {
        public T SecondImpl { get; set; }

        public SecondImplementation2(T secondImpl)
        {
            SecondImpl = secondImpl;
        }
    }
}
