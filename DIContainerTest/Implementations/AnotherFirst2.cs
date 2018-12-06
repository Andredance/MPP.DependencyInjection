using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIContainerTest.Interfaces;

namespace DIContainerTest.Implementations
{
    public class AnotherFirst2 : AnotherFirst
    {
        public IFirst first { get; set; }

        public AnotherFirst2(IFirst first) : base(first)
        {
        }
    }
}
