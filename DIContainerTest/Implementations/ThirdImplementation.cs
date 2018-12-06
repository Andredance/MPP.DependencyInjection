using DIContainerTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainerTest.Implementations
{
    class ThirdImplementation<T1,T2> : IThird<T1,T2>
    {
        public T1 ThirdImpl1 { get; set; }
        public T2 ThirdImpl2 { get; set; }

        public ThirdImplementation(T1 third1, T2 third2)
        {
            ThirdImpl1 = third1;
            ThirdImpl2 = third2;
        }
    }
}
