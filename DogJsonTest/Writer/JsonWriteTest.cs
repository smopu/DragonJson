using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using DogJson;
using System.Runtime.InteropServices;
//using Assert = DogJson.Assert;
namespace DogJsonTest
{
    [TestFixture]
    public class JsonWriteTest
    {
        public class ACE3
        {
            public int kk;
            public string str;
        }

        public class ACE1
        {
            public ACE3[] aCE3s;
            public bool b;
            public double num;
            public string str;
            public ACE3 aCE3;
            public int[] ints;
            public int[,] ints2;
        }

        [Test]
        public void TestWrite()
        {
            JsonWriter jsonWriter = new JsonWriter();
            ACE1 inputData = new ACE1();
            inputData.aCE3 = new ACE3
            {
                kk = 44,
                str = "E4",
            };
            inputData.ints = new int[] {
                1, 2, 3, 9
            };

            inputData.aCE3s = new ACE3[] {
                new ACE3{
                    kk = 4,
                    str = "eewqr"
                },
                new ACE3{
                    kk = 2,
                    str = "DE4"
                },
                new ACE3{
                    kk = 1,
                    str = "F3"
                }
            };

            inputData.ints2 = new int[,]
            {
                {1, 2, 3, 4},
                {11, 12, 13, 14},
                {101, 102, 103, 104},
            };


            inputData.b = false;
            inputData.num = 4.5;
            inputData.str = "CCCCD";

            List<JsonWriteValue> writers = new List<JsonWriteValue>();
            writers = jsonWriter.Wirter(inputData);


            JsonWriteValue root = new JsonWriteValue();
            root.type = JsonWriteType.Object;
            root.isLast = true;

            JsonWriteValue item = new JsonWriteValue();

            StringBuilder sb = new StringBuilder();
            Stack<JsonWriteValue> objStack = new Stack<JsonWriteValue>();
            sb.AppendLine("{");

            JsonWriteValue parent = writers[0];
            objStack.Push(parent);


            //for (int i = 1; i < writers.Count; i++)
            //{
            //    item = parent.back;
            //    parent = item;

            //    if (item.key != null)
            //    {
            //        sb.Append("\"" + item.key + "\": ");
            //    }
            //    sb.AppendLine(item.value);

            //}

            //Console.WriteLine(sb.ToString());
            //Console.ReadKey();

            for (int i = 1; i < writers.Count; i++)
            {
                sb.Append('\t', objStack.Count);
                item = parent.back;
                parent = item;

                if (item.key != null)
                {
                    sb.Append("\"" + item.key + "\": ");
                }

                switch (item.type)
                {
                    case JsonWriteType.String:
                        sb.Append(item.value);
                        if (item.isLast)
                        {
                            JsonWriteValue parentStack;
                            sb.AppendLine();
                            if (item.back == null)
                            {
                                while (objStack.Count > 0)//item.back != null &&  
                                {
                                    parentStack = objStack.Pop();
                                    sb.Append('\t', objStack.Count);

                                    if (parentStack.isLast)
                                    {
                                        if (parentStack.type == JsonWriteType.Object)
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
                                        if (parentStack.type == JsonWriteType.Object)
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
                                    sb.Append('\t', objStack.Count);

                                    if (parentStack.isLast)
                                    {
                                        if (parentStack.type == JsonWriteType.Object)
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
                                        if (parentStack.type == JsonWriteType.Object)
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
                                sb.Append('\t', objStack.Count);

                                if (parentStack.isLast)
                                {
                                    if (parentStack.type == JsonWriteType.Object)
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
                                    if (parentStack.type == JsonWriteType.Object)
                                    {
                                        sb.AppendLine("},");
                                    }
                                    else
                                    {
                                        sb.AppendLine("],");
                                    }
                                }
                            }


                        }
                        else
                        {
                            sb.AppendLine(",");
                        }
                        break;
                    case JsonWriteType.Object:
                        sb.AppendLine("{");
                        objStack.Push(item);
                        break;
                    case JsonWriteType.Array:
                        sb.AppendLine("[");
                        objStack.Push(item);
                        break;
                    default:
                        break;
                }
            }

            //sb.AppendLine("}");

            JsonRender jsonRender = new JsonRender();
            var outData = jsonRender.ReadJsonTextCreateObject<ACE1>(sb.ToString());

            Console.WriteLine(sb.ToString());
            Assert.AreEqualObject(outData, inputData);
            //Console.ReadKey();
        }


    }
}
