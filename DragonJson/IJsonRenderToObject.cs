using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace DragonJson
{
    public unsafe interface IJsonRenderToObject
    {
        object CreateObject(JsonReader jsonRender, Type type, char* startChar, int length);
    }

    public unsafe interface IJsonWriterToObject
    {
        List<JsonWriteValue> ReadObject(object data);
    }
}
