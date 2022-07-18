using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
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

        const int backCount = 4;
        public string Writer(object data)
        {
            bool isNext = false;
            List<JsonWriteValue> writers = jsonWriterToObject.ReadObject(data);

            JsonWriteValue root = new JsonWriteValue();
            root.jsonType = JsonWriteType.Object;
            root.isLast = true;

            JsonWriteValue item = new JsonWriteValue();

            StringBuilder sb = new StringBuilder();
            Stack<JsonWriteValue> objStack = new Stack<JsonWriteValue>();
            sb.AppendLine("{");

            JsonWriteValue parent = writers[0];
            objStack.Push(parent);

            for (int i = 1; i < writers.Count; i++)
            {
                item = parent.back;
                parent = item;

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
                            sb.Append(",");
                            isNext = true;
                        }
                        break;
                    case JsonWriteType.Object:
                        isNext = false;
                        sb.AppendLine("{");
                        isNext = false;
                        objStack.Push(item);
                        break;
                    case JsonWriteType.Array:
                        isNext = false;
                        sb.AppendLine("[");
                        objStack.Push(item);
                        break;
                    case JsonWriteType.None:
                        sb.Append("null");
                        goto Loop;
                    default:
                        break;
                }
            }

            //sb.AppendLine("}");
            return sb.ToString();
        }



    }



}
