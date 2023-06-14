using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonJson
{
    public class JsonWriter
    {
        public JsonWriter(IJsonWriterToObject jsonWriterToObject)
        {
            this.jsonWriterToObject = jsonWriterToObject;
            if (CollectionManager.IsStart == false)
            {
                CollectionManager.Start();
            }
        }

        IJsonWriterToObject jsonWriterToObject;

        const int backCount = 2;
        public string Writer(object data)
        {
            bool isNext = false;
            List<JsonWriteValue> writers = jsonWriterToObject.ReadObject(data);

            StringBuilder sb = new StringBuilder();
            Stack<JsonWriteValue> objStack = new Stack<JsonWriteValue>();
            JsonWriteValue parent = writers[0];
            switch (parent.jsonType)
            {
                case JsonWriteType.Object:
                    sb.AppendLine("{");
                    break;
                case JsonWriteType.Array:
                    sb.AppendLine("[");
                    break;
            }

            objStack.Push(parent);

            for (int i = 1; i < writers.Count; i++)
            {
                JsonWriteValue item = parent.back;
                parent = item;
                isNext = false;
                if (!isNext)
                {
                    sb.Append(' ', objStack.Count * backCount);
                }
                else
                {
                    sb.Append(' ');
                }
                if (item.key != null)
                {
                    if (isNext)
                    {
                        sb.AppendLine();
                        sb.Append(' ', objStack.Count * backCount);
                    }
                    sb.Append("\"" + item.key + "\": ");
                }
                else
                {

                }


                switch (item.jsonType)
                {
                    case JsonWriteType.String:
                        sb.Append(item.value);
                    Loop:
                        if (item.isLast)
                        {
                            JsonWriteValue parentStack;
                            sb.AppendLine();
                            if (item.back == null)
                            {
                                while (objStack.Count > 0)//item.back != null &&  
                                {
                                    parentStack = objStack.Pop();
                                    sb.Append(' ', objStack.Count * backCount);

                                    if (parentStack.isLast)
                                    {
                                        if (parentStack.jsonType == JsonWriteType.Object)
                                        {
                                            sb.AppendLine("}");
                                        }
                                        else
                                        {
                                            sb.AppendLine("]");
                                        }
                                    }
                                    else
                                    {
                                        if (parentStack.jsonType == JsonWriteType.Object)
                                        {
                                            sb.AppendLine("},");
                                        }
                                        else
                                        {
                                            sb.AppendLine("],");
                                        }
                                    }
                                }
                                break;
                            }
                            else
                            {

                                while (objStack.Peek().parent != item.back.parent)//item.back != null &&  
                                {
                                    parentStack = objStack.Pop();
                                    sb.Append(' ', objStack.Count * backCount);

                                    if (parentStack.isLast)
                                    {
                                        if (parentStack.jsonType == JsonWriteType.Object)
                                        {
                                            sb.AppendLine("}");
                                        }
                                        else
                                        {
                                            sb.AppendLine("]");
                                        }
                                    }
                                    else
                                    {
                                        if (parentStack.jsonType == JsonWriteType.Object)
                                        {
                                            sb.AppendLine("},");
                                        }
                                        else
                                        {
                                            sb.AppendLine("],");
                                        }
                                    }
                                }
                                parentStack = objStack.Pop();
                                sb.Append(' ', objStack.Count * backCount);

                                if (parentStack.isLast)
                                {
                                    if (parentStack.jsonType == JsonWriteType.Object)
                                    {
                                        sb.AppendLine("}");
                                    }
                                    else
                                    {
                                        sb.AppendLine("]");
                                    }
                                }
                                else
                                {
                                    if (parentStack.jsonType == JsonWriteType.Object)
                                    {
                                        sb.AppendLine("},");
                                    }
                                    else
                                    {
                                        sb.AppendLine("],");
                                    }
                                }
                            }

                            isNext = false;
                        }
                        else
                        {
                            sb.AppendLine(",");
                            //sb.Append(","); 
                            //isNext = true;
                        }
                        break;
                    case JsonWriteType.Object:
                        if (item.back != null && item.back.parent != item)
                        {
                            if (item.isLast)
                            {
                                sb.AppendLine("{ }");
                                while (objStack.Peek() != item.back.parent)
                                {
                                    JsonWriteValue value = objStack.Pop();
                                    sb.Append(' ', objStack.Count * backCount);
                                    switch (value.jsonType)
                                    {
                                        case JsonWriteType.Object:
                                            if (value.isLast)
                                            {
                                                sb.AppendLine("}");
                                            }
                                            else
                                            {
                                                sb.AppendLine("},");
                                            }
                                            break;
                                        case JsonWriteType.Array:
                                            if (value.isLast)
                                            {
                                                sb.AppendLine("]");
                                            }
                                            else
                                            {
                                                sb.AppendLine("],");
                                            }
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                sb.AppendLine("{ },");
                            }
                        }
                        else
                        {
                            sb.AppendLine("{");
                            isNext = false;
                            objStack.Push(item);
                        }
                        break;
                    case JsonWriteType.Array:
                        if (item.back != null && item.back.parent != item)
                        {
                            if (item.isLast)
                            // if (item.parent == item.back.parent)
                            {
                                sb.AppendLine("[ ]");
                                while (objStack.Peek() != item.back.parent)
                                {
                                    JsonWriteValue value = objStack.Pop();
                                    sb.Append(' ', objStack.Count * backCount);
                                    switch (value.jsonType)
                                    {
                                        case JsonWriteType.Object:
                                            if (value.isLast)
                                            {
                                                sb.AppendLine("}");
                                            }
                                            else
                                            {
                                                sb.AppendLine("},");
                                            }
                                            break;
                                        case JsonWriteType.Array:
                                            if (value.isLast)
                                            {
                                                sb.AppendLine("]");
                                            }
                                            else
                                            {
                                                sb.AppendLine("],");
                                            }
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                sb.AppendLine("[ ],");
                            }
                        }
                        else
                        {
                            isNext = false;
                            sb.AppendLine("[");
                            objStack.Push(item);
                        }
                        break;
                    case JsonWriteType.None:
                        sb.Append("null");
                        goto Loop;
                    default:
                        break;
                }
            }

            while (objStack.Count > 0)
            {
                JsonWriteValue value = objStack.Pop();
                sb.Append(' ', objStack.Count * backCount);
                switch (value.jsonType)
                {
                    case JsonWriteType.Object:
                        sb.AppendLine("}");
                        break;
                    case JsonWriteType.Array:
                        sb.AppendLine("]");
                        break;
                }
            }
            return sb.ToString();
        }



    }



}
