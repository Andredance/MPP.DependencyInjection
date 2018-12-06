using DIContainerTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainerTest.Implementations
{
    public class AnotherSecond<T>
    {
        public ISecond<T> SecondParamImpl;

        public AnotherSecond(ISecond<T> second)
        {
            SecondParamImpl = second;
        }
    }
}
