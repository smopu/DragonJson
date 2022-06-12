using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace DogJson
{
    public unsafe interface IJsonRenderToObject
    {
        object CreateObject(JsonRender jsonRender, Type type, char* startChar, int length);
    }

}
