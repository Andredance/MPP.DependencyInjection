using DIContainerTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainerTest.Implementations
{
    public class AnotherFirst
    {
        public IFirst FirstImpl { get; set; }

        public AnotherFirst(IFirst first)
        {
            FirstImpl = first;
        }
    }
}
