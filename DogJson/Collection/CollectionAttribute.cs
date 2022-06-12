using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
     [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CollectionAttribute : Attribute
    {
        public Type type;
        public bool isArrays;
        public CollectionAttribute(Type type, bool isArrays)
        {
            this.type = type;
            this.isArrays = isArrays;
        }
    }
}
