using System.Collections.Generic;

namespace SECCS.Tests.Classes
{
    public class TestClassConcrete
    {
        [ConcreteType(typeof(List<int>))]
        public IList<int> List { get; set; }
    }
}
