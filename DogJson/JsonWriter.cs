using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    public class ACE
    {
        public bool b;
        public double num;
        public int kk;
        public string str;
    }


    public unsafe class JsonWriteValue
    {
        public JsonWriteValue()
        {

        }

        public JsonWriteValue(int index, JsonWriteValue parent)
        {
            this.index = index;
            this.parent = parent;
        }


        public JsonWriteType type;

        public string key;

        public string value;

        public bool isLast;
        public object data;
        public JsonWriteValue back;
        public JsonWriteValue parent;
        public int index;
    }



    public enum JsonWriteType : byte
    {
        None = 0,
        String = 1,
        Object = 5,
        Array = 6,
    }


    public unsafe class JsonWriter
    {
        
        Dictionary<Type, TypeAddrReflectionWrapper> allTypeWarp = new Dictionary<Type, TypeAddrReflectionWrapper>();
        public TypeAddrReflectionWrapper GetTypeWarp(Type type)
        {
            TypeAddrReflectionWrapper ob;
            if (allTypeWarp.TryGetValue(type, out ob))
            {
                return ob;
            }
            return allTypeWarp[type] = new TypeAddrReflectionWrapper(type);
        }

        //public List<JsonWriteValue> Wirter(object data)
        //{
        //    Type type = data.GetType();
        //    List<JsonWriteValue> writers = new List<JsonWriteValue>();
        //    writers.Add(jsonWriteValue);
        //    Wirter(data, writers);
        //    return writers;
        //}

        /// <summary>
        /// 多维数组按照 Json 格式输出
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        static void ArrayToJsonString(
            List<JsonWriteValue> writers,
            Array array, 
            JsonWriteValue parent, 
            JsonWriteValue last,
            List<JsonWriteValue> nows
            )
        {
            var elementType = array.GetType().GetElementType();
            var elementTypeCode = Type.GetTypeCode(elementType);
            bool elementTypeIsValue = elementType.IsValueType;

            StringBuilder sb = new StringBuilder();
            int rank = array.Rank;
            int[] lengths = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                lengths[i] = array.GetLength(i);
            }

            //List<JsonWriteValue> nows = new List<JsonWriteValue>();
            //List<JsonWriteValue> parents = new List<JsonWriteValue>();
            int length = lengths[0];
            int parentLength = 1;

            //JsonWriteValue root = parent;
            JsonWriteValue previous = parent;
            previous.back = last;
            JsonWriteValue nowlast = previous.back;
            int parentIndex = writers.Count;
            for (int k = 0; k < length; k++)
            {
                JsonWriteValue now = new JsonWriteValue(writers.Count, parent);
                now.type = JsonWriteType.Array;
                now.key = null;
                previous.back = now;

                writers.Add(now);
                previous = now;
            }
            previous.back = nowlast;
            previous.isLast = true;

            for (int nowRank = 1; nowRank < rank - 1; nowRank++)
            {
                length = lengths[nowRank];
                parentLength *= lengths[nowRank - 1];
                int parentIndex2 = writers.Count;
                for (int j = 0; j < parentLength; j++)
                {
                    previous = parent = writers[parentIndex + j];
                    nowlast = previous.back;

                    for (int i = 0; i < length; i++)
                    {
                        JsonWriteValue now = new JsonWriteValue(writers.Count, parent);
                        now.type = JsonWriteType.Array;
                        now.key = null;
                        previous.back = now;

                        writers.Add(now);
                        previous = now;
                    }
                    previous.back = nowlast;
                    previous.isLast = true;
                }
                parentIndex = parentIndex2;

            }


            length = lengths[rank - 1];
            parentLength *= lengths[rank - 2];

            byte* startOffcet = (byte*)GeneralTool.ObjectToVoid(array) +
              UnsafeOperation.PTR_COUNT * 2 + rank * 2 * 4;
            int arrayIndex = 0;
            int arrayItemSize = UnsafeOperation.SizeOfStack(elementType);

            //return;
            for (int j = 0; j < parentLength; j++)
            {
                previous = parent = writers[parentIndex + j];
                nowlast = previous.back;

                for (int i = 0; i < length; i++)
                {
                    byte* value = startOffcet + arrayItemSize * arrayIndex;
                    //var value = array.GetValue(indices);
                    JsonWriteValue now = new JsonWriteValue(writers.Count, parent);
                    now.key = null;
                    previous.back = now;
                    writers.Add(now);
                    previous = now;

                    switch (elementTypeCode)
                    {
                        case TypeCode.Empty:
                            break;
                        case TypeCode.Object:
                            if (elementTypeIsValue)
                            {
                                //now.data = GeneralTool.VoidToObject(value);
                            }
                            else
                            {
                                now.data = GeneralTool.VoidToObject(*(IntPtr**)value);
                            }
                            if (now.data == null)
                            {
                                now.type = JsonWriteType.None;
                            }
                            else
                            {
                                if (elementType.IsArray)
                                {
                                    now.type = JsonWriteType.Array;
                                }
                                else
                                {
                                    now.type = JsonWriteType.Object;
                                }
                            }
                            nows.Add(now);


                            break;
                        case TypeCode.DBNull:
                            break;
                        case TypeCode.Boolean:
                            now.value = *(bool*)(value) ? "true" : "false";
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.Char:
                            now.value = "\"" + *(char*)(value) + "\"";
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.SByte:
                            now.value = (*(sbyte*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.Byte:
                            now.value = (*(Byte*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.Int16:
                            now.value = (*(Int16*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.UInt16:
                            now.value = (*(UInt16*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.Int32:
                            now.value = (*(Int32*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.UInt32:
                            now.value = (*(UInt32*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.Int64:
                            now.value = (*(Int64*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.UInt64:
                            now.value = (*(UInt64*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.Single:
                            now.value = (*(Single*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.Double:
                            now.value = (*(Double*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.Decimal:
                            now.value = (*(Decimal*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.DateTime:
                            now.value = (*(DateTime*)(value)).ToString();
                            now.type = JsonWriteType.String;
                            break;
                        case TypeCode.String:
                            now.value = "\"" + (*(DateTime*)(value)) + "\"";
                            now.type = JsonWriteType.String;
                            break;
                        default:
                            break;
                    }

                    ++arrayIndex;
                }
                previous.back = nowlast;
                previous.isLast = true;

            }






        }




        public List<JsonWriteValue> Wirter(object data)
        {
            JsonWriteValue jsonWriteValue = new JsonWriteValue();
            jsonWriteValue.key = "";
            jsonWriteValue.type = JsonWriteType.Object;
            jsonWriteValue.isLast = true;
            jsonWriteValue.data = data;


            List<JsonWriteValue> writers = new List<JsonWriteValue>();
            writers.Add(jsonWriteValue);
            //byte* byteP = (byte*)GeneralTool.ObjectToVoid(data) + UnsafeOperation.PTR_COUNT;


            //int startIndex = writers.Count - 1;
            //foreach (var item in warp.nameOfField)
            //{
            //    //object value = GeneralTool.VoidToObject(byteP + item.Value.offset);

            //}


            List<JsonWriteValue> nows = new List<JsonWriteValue>();
            List<JsonWriteValue> parents = new List<JsonWriteValue>();
            parents.Add(jsonWriteValue);

            while (parents.Count > 0)
            {
                foreach (var parent in parents)
                {
                    if (parent.type == JsonWriteType.Array)
                    {
                        JsonWriteValue previous = parent;
                        JsonWriteValue last = parent.back;

                        var array = parent.data as Array;
                        if (array.Rank == 1)
                        {
                            var elementType = parent.data.GetType().GetElementType();


                            var elementTypeCode = Type.GetTypeCode(elementType);
                            for (int i = 0; i < array.Length; i++)
                            {
                                object value = array.GetValue(i);
                                previous = ArrayItem(writers, nows, parent, previous, elementType, elementTypeCode, value);
                            }

                            previous.back = last;
                            previous.isLast = true;

                        }
                        else
                        {
                            ArrayToJsonString(writers, array, parent, last, nows);
                        }



                    }
                    else
                    {
                        TypeAddrReflectionWrapper warp = GetTypeWarp(parent.data.GetType());
                        int count = warp.nameOfField.Count;

                        JsonWriteValue previous = parent;
                        JsonWriteValue last = parent.back;

                        foreach (var item in warp.nameOfField)
                        {
                            if (item.Value.isProperty)
                            {
                                continue;
                            }
                            object value = item.Value.fieldInfo.GetValue(parent.data);
                            if (value != null)
                            {
                                JsonWriteValue now = new JsonWriteValue(writers.Count, parent);
                                now.data = value;
                                now.key = item.Key;
                                previous.back = now;
                                writers.Add(now);
                                previous = now;
                                switch (item.Value.typeCode)
                                {
                                    case TypeCode.Empty:
                                        break;
                                    case TypeCode.Object:
                                        if (item.Value.isArray)
                                        {
                                            now.type = JsonWriteType.Array;
                                            nows.Add(now);
                                        }
                                        else
                                        {
                                            now.type = JsonWriteType.Object;
                                            nows.Add(now);
                                        }
                                        break;
                                    case TypeCode.DBNull:
                                        break;
                                    case TypeCode.Boolean:
                                        now.value = ((bool)value) ? "true" : "false";
                                        now.type = JsonWriteType.String;
                                        break;
                                    case TypeCode.Char:
                                        now.value = "\"" + (char)value + "\"";
                                        now.type = JsonWriteType.String;
                                        break;
                                    case TypeCode.SByte:
                                    case TypeCode.Byte:
                                    case TypeCode.Int16:
                                    case TypeCode.UInt16:
                                    case TypeCode.Int32:
                                    case TypeCode.UInt32:
                                    case TypeCode.Int64:
                                    case TypeCode.UInt64:
                                    case TypeCode.Single:
                                    case TypeCode.Double:
                                    case TypeCode.Decimal:
                                        now.value = value.ToString();
                                        now.type = JsonWriteType.String;
                                        break;
                                    case TypeCode.DateTime:
                                        now.value = value.ToString();
                                        now.type = JsonWriteType.String;
                                        break;
                                    case TypeCode.String:
                                        now.value = "\"" + (string)value + "\"";
                                        now.type = JsonWriteType.String;
                                        break;
                                    default:
                                        break;
                                }

                            }
                        }

                        previous.back = last;
                        previous.isLast = true;


                    }
                }


                parents = nows;

                nows = new List<JsonWriteValue>();
            }
            return writers;



            //IEnumerator<KeyValuePair<string, TypeAddrField>> enumerator = warp.nameOfField.GetEnumerator();
            //if (enumerator.MoveNext())
            //{
            //    do
            //    {
            //        int startIndex = writers.Count - 1;
            //        var item = enumerator.Current;
            //        if (!enumerator.MoveNext())
            //        {
            //            object value = item.Value.fieldInfo.GetValue(data);
            //            if (value != null)
            //            {
            //                WriteItem(value, item.Key, item.Value.typeCode, item.Value.fieldType, writers, true, startIndex);
            //            }
            //            //这里是最后一个
            //            break;
            //        }
            //        else
            //        {
            //            object value = item.Value.fieldInfo.GetValue(data);
            //            if (value != null)
            //            {
            //                WriteItem(value, item.Key, item.Value.typeCode, item.Value.fieldType, writers, false, 0);
            //            }
            //        }
            //    } while (true);
            //}




            //JsonWriteValue last = writers[writers.Count - 1];
            //last.isLast = true;
            //last.backIndex = startIndex;
            //writers[writers.Count - 1] = last;
        }

        private static JsonWriteValue ArrayItem(List<JsonWriteValue> writers, List<JsonWriteValue> nows, JsonWriteValue parent, JsonWriteValue previous, Type elementType, TypeCode elementTypeCode, object value)
        {
            JsonWriteValue now = new JsonWriteValue(writers.Count, parent);
            now.key = null;
            writers.Add(now);
            previous.back = now;
            previous = now;


            if (value == null)
            {
                now.type = JsonWriteType.None;
                now.data = null;
            }
            else
            {
                now.data = value;
                switch (elementTypeCode)
                {
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Object:
                        if (elementType.IsArray)
                        {
                            now.type = JsonWriteType.Array;
                            nows.Add(now);
                        }
                        else
                        {
                            now.type = JsonWriteType.Object;
                            nows.Add(now);
                        }

                        break;
                    case TypeCode.DBNull:
                        break;
                    case TypeCode.Boolean:
                        now.value = ((bool)value) ? "true" : "false";
                        now.type = JsonWriteType.String;
                        break;
                    case TypeCode.Char:
                        now.value = "\"" + (char)value + "\"";
                        now.type = JsonWriteType.String;
                        break;
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        now.value = value.ToString();
                        now.type = JsonWriteType.String;
                        break;
                    case TypeCode.DateTime:
                        now.value = value.ToString();
                        now.type = JsonWriteType.String;
                        break;
                    case TypeCode.String:
                        now.value = "\"" + (string)value + "\"";
                        now.type = JsonWriteType.String;
                        break;
                    default:
                        break;
                }
            }

            return previous;
        }


        //private void WriteItem(object value, string key, TypeCode typeCode, Type type, List<JsonWriteValue> writers, bool isLast, int backIndex)
        //{
        //    JsonWriteValue jsonWriteValue = new JsonWriteValue();
        //    jsonWriteValue.key = key;

        //    if (isLast)
        //    {
        //        jsonWriteValue.isLast = true;
        //        jsonWriteValue.backIndex = backIndex;
        //    }
        //    else
        //    {
        //        jsonWriteValue.isLast = false;
        //        jsonWriteValue.backIndex = 0;
        //    }
        //    switch (typeCode)
        //    {
        //        case TypeCode.Empty:
        //            break;
        //        case TypeCode.Object:
        //            if (type.IsArray)
        //            {
        //                jsonWriteValue.type = JsonWriteType.Array;
        //                writers.Add(jsonWriteValue);
        //                int startIndex = writers.Count - 1;
        //                Array array = value as Array;

        //                var elementType = array.GetType().GetElementType();

        //                for (int i = 0; i < array.Length - 1; i++)
        //                {
        //                    WriteItem(array.GetValue(i), null,
        //                        Type.GetTypeCode(elementType), elementType,
        //                        writers, false, 0);
        //                }

        //                WriteItem(array.GetValue(array.Length - 1), null,
        //                    Type.GetTypeCode(elementType), elementType,
        //                    writers, true, startIndex);

        //                return;
        //            }
        //            else
        //            {
        //                jsonWriteValue.type = JsonWriteType.Object;
        //                writers.Add(jsonWriteValue);
        //                Wirter(value, writers);
        //                return;
        //            }

        //        case TypeCode.DBNull:
        //            break;
        //        case TypeCode.Boolean:
        //            jsonWriteValue.value = ((bool)value) ? "true" : "false";
        //            jsonWriteValue.type = JsonWriteType.String;
        //            break;
        //        case TypeCode.Char:
        //            jsonWriteValue.value = "\"" + (char)value + "\"";
        //            jsonWriteValue.type = JsonWriteType.String;
        //            break;
        //        case TypeCode.SByte:
        //        case TypeCode.Byte:
        //        case TypeCode.Int16:
        //        case TypeCode.UInt16:
        //        case TypeCode.Int32:
        //        case TypeCode.UInt32:
        //        case TypeCode.Int64:
        //        case TypeCode.UInt64:
        //        case TypeCode.Single:
        //        case TypeCode.Double:
        //        case TypeCode.Decimal:
        //            jsonWriteValue.value = value.ToString();
        //            jsonWriteValue.type = JsonWriteType.String;
        //            break;
        //        case TypeCode.DateTime:
        //            jsonWriteValue.value = value.ToString();
        //            jsonWriteValue.type = JsonWriteType.String;
        //            break;
        //        case TypeCode.String:
        //            jsonWriteValue.value = "\"" + (string)value + "\"";
        //            jsonWriteValue.type = JsonWriteType.String;
        //            break;
        //        default:
        //            break;
        //    }
        //    writers.Add(jsonWriteValue);
        //}



    }
}
