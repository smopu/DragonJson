using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DogJson.Collection.ArrayCollection;

namespace DogJson
{
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

        public JsonWriteType jsonType;

        public string key;
        public int arrayIndex;

        public string value;

        public bool isLast;
        public bool isSetType = false;
        public object data;
        public IWriterCollectionObject collection;
        
        public Type type;
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


    public unsafe class WriterReflection : IJsonWriterToObject
    {
        public enum WriterType : byte
        {
            All,
            No,
            Public,
        }


        public WriterType classFieldWriter = WriterType.Public;
        public WriterType classPropertyWriter = WriterType.Public;

        public WriterType structFieldWriter = WriterType.All;
        public WriterType structPropertyWriter = WriterType.No;



        static Dictionary<Type, TypeAddrReflectionWrapper> allTypeWarp = new Dictionary<Type, TypeAddrReflectionWrapper>();
        public static  TypeAddrReflectionWrapper GetTypeWarp(Type type)
        {
            TypeAddrReflectionWrapper ob;
            if (allTypeWarp.TryGetValue(type, out ob))
            {
                return ob;
            }
            lock (allTypeWarp)
            {
                return allTypeWarp[type] = new TypeAddrReflectionWrapper(type);
            }
        }
            
        public List<JsonWriteValue> ReadObject(object data)
        {
            isRoot = true;
            allPath.Clear();
            JsonWriteValue jsonWriteValue = new JsonWriteValue();
            jsonWriteValue.key = "";
            jsonWriteValue.jsonType = JsonWriteType.Object;
            jsonWriteValue.isLast = true;
            jsonWriteValue.data = data;
            //allPath[data] = new ObjectPath() { value = jsonWriteValue };

            List<JsonWriteValue> writers = new List<JsonWriteValue>();
            writers.Add(jsonWriteValue);
            List<JsonWriteValue> nows = new List<JsonWriteValue>();
            List<JsonWriteValue> parents = new List<JsonWriteValue>();
            parents.Add(jsonWriteValue);
            while (parents.Count > 0)
            {
                //int parentIndex = 0;
                foreach (var parent in parents)
                {
                    //++parentIndex;
                    if (parent.data == null)
                    {
                        continue;
                    }
                    var type = parent.data.GetType(); //IWriterObject writeObject = JsonManager.GetIWriterArray(type);
                    JsonWriteValue previous = parent;
                    JsonWriteValue last = parent.back;

                    bool isPath = false;

                    //IWriterCollectionString writeString = CollectionManager.GetWriterCollectionString(type);
                    //if (writeString != null)
                    //{
                    //    JsonWriteValue now = previous;
                    //    now.type = JsonWriteType.String;
                    //    now.value = "\"" + writeString.GetStringValue(parent.data) + "\"";
                    //    continue;
                    //}


                    //if (!isRoot && parent.data == data)
                    //{
                    //    parent.type = JsonWriteType.String;
                    //    parent.value = "\"$\"";
                    //    isPath = true;
                    //    continue;
                    //}

                    //if ( !parent.data.GetType().IsValueType)
                    //{
                    //    ObjectPath path;
                    //    if (allPath.TryGetValue(parent.data, out path))
                    //    {
                    //        if (path.path == null)
                    //        {
                    //            path.path = "\"" + GetRootPath(path.value) + "\"";
                    //            //allPath[parent.data] = path;
                    //        }
                    //        parent.type = JsonWriteType.String;
                    //        parent.value = path.path;
                    //        isPath = true;
                    //        continue;
                    //    }
                    //    else
                    //    {
                    //        allPath[parent.data] = new ObjectPath() { value = parent };
                    //    }
                    //}



                    var array = parent.data as Array;
                    if (array != null)
                    {
                        parent.jsonType = JsonWriteType.Array;
                        if (array.Rank == 1)
                        {
                            var elementType = parent.data.GetType().GetElementType();
                            var elementTypeCode = Type.GetTypeCode(elementType);

                            for (int i = 0; i < array.Length; i++)
                            {
                                object value = array.GetValue(i);
                                previous = ArrayItem(i, writers, nows, parent,
                                    previous, last, elementType, elementTypeCode, value);
                            }
                            previous.back = last;
                            previous.isLast = true;
                        }
                        else
                        {
                            ArrayToJsonString(writers, array, parent, last, nows);
                        }
                        continue;
                    }

                    //IWriterCollectionObject writeObject = CollectionManager.GetWriterCollection(type);

                    var typeCode = Type.GetTypeCode(type);
                    IWriterCollectionObject writeObject = null;
                    if (typeCode == TypeCode.Object && type.IsArray == false)
                    {
                        writeObject = CollectionManager.GetWriterCollection(type);
                    }
                    if (writeObject != null)
                    {
                        switch (writeObject.GetWriteType())
                        {
                            case JsonWriteType.None:
                                break;
                            case JsonWriteType.String:
                                {
                                    //JsonWriteValue now = new JsonWriteValue(writers.Count, parent);
                                    JsonWriteValue now = previous;
                                    now.jsonType = JsonWriteType.String;
                                    foreach (var item in writeObject.GetValue(parent.data))
                                    {
                                        now.value = "\"" + item.key + "\"";
                                    }
                                }
                                break;
                            case JsonWriteType.Object:
                                {
                                    if (parent.isSetType)
                                    {
                                        JsonWriteValue typeWrite = new JsonWriteValue(writers.Count, parent);
                                        typeWrite.key = "$type";
                                        typeWrite.value = "\"" + parent.type.Assembly.GetName().Name + "," + parent.type.ToString() + "\"";
                                        previous.back = typeWrite;
                                        typeWrite.jsonType = JsonWriteType.String;
                                        writers.Add(typeWrite);
                                        previous = typeWrite;
                                    }
                                    parent.jsonType = JsonWriteType.Object;
                                    //对象 容器
                                    foreach (var item in writeObject.GetValue(parent.data))
                                    {
                                        previous = ObjectItem(writers, nows, parent, previous, last, item.key, item.type, item.value);
                                    }
                                    previous.back = last;
                                    previous.isLast = true;
                                }
                                break;
                            case JsonWriteType.Array:
                                {
                                    parent.jsonType = JsonWriteType.Array;
                                    //数组 容器
                                    int i = 0;
                                    foreach (var item in writeObject.GetValue(parent.data))
                                    {
                                        var elementType = item.type;
                                        var elementTypeCode = Type.GetTypeCode(elementType);

                                        previous = ArrayItem(i, writers, nows, parent,
                                            previous, last, elementType, elementTypeCode, item.value);
                                        i++;
                                    }
                                    //previous.type = JsonWriteType.Array;
                                    previous.back = last;
                                    previous.isLast = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        parent.jsonType = JsonWriteType.Object;
                        TypeAddrReflectionWrapper warp = GetTypeWarp(parent.data.GetType());
                        int count = warp.nameOfField.Count;
                        if (warp.isValueType)
                        {
                            if (parent.isSetType)
                            {
                                JsonWriteValue typeWrite = new JsonWriteValue(writers.Count, parent);
                                typeWrite.key = "$type";
                                typeWrite.value = "\"" + parent.type.Assembly.GetName().Name + "," + parent.type.ToString() + "\"";
                                previous.back = typeWrite;
                                typeWrite.jsonType = JsonWriteType.String;
                                writers.Add(typeWrite);
                                previous = typeWrite;
                            }
                            foreach (var item in warp.nameOfField)
                            {
                                if (item.Value.isProperty)
                                {
                                    switch (structPropertyWriter)
                                    {
                                        case WriterType.All:
                                            {
                                                object value = item.Value.propertyInfo.GetValue(parent.data);
                                                // object value = item.Value.GetValue(parent.data);
                                                previous = ObjectItem(writers, nows, parent, previous, last, item.Key, item.Value.fieldOrPropertyType, value);
                                            }
                                            break;
                                        case WriterType.No:
                                            break;
                                        case WriterType.Public:
                                            if (item.Value.isPublic)
                                            {
                                                object value = item.Value.propertyInfo.GetValue(parent.data);
                                                // object value = item.Value.GetValue(parent.data);
                                                previous = ObjectItem(writers, nows, parent, previous, last, item.Key, item.Value.fieldOrPropertyType, value);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (structFieldWriter)
                                    {
                                        case WriterType.All:
                                            {
                                                object value = item.Value.fieldInfo.GetValue(parent.data);
                                                // object value = item.Value.GetValue(parent.data);
                                                previous = ObjectItem(writers, nows, parent, previous, last, item.Key, item.Value.fieldOrPropertyType, value);
                                            }
                                            break;
                                        case WriterType.No:
                                            break;
                                        case WriterType.Public:
                                            if (item.Value.isPublic)
                                            {
                                                object value = item.Value.fieldInfo.GetValue(parent.data);
                                                // object value = item.Value.GetValue(parent.data);
                                                previous = ObjectItem(writers, nows, parent, previous, last, item.Key, item.Value.fieldOrPropertyType, value);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (parent.isSetType)
                            {
                                JsonWriteValue typeWrite = new JsonWriteValue(writers.Count, parent);
                                typeWrite.key = "$type";
                                typeWrite.value = "\"" + parent.type.Assembly.GetName().Name + "," + parent.type.ToString() + "\"";
                                previous.back = typeWrite;
                                typeWrite.jsonType = JsonWriteType.String;
                                writers.Add(typeWrite);
                                previous = typeWrite;
                            }
                            foreach (var item in warp.nameOfField)
                            {
                                if (item.Value.isProperty)
                                {
                                    switch (classPropertyWriter)
                                    {
                                        case WriterType.All:
                                            {
                                                object value = item.Value.propertyInfo.GetValue(parent.data);
                                                // object value = item.Value.GetValue(parent.data);
                                                previous = ObjectItem(writers, nows, parent, previous, last, item.Key, item.Value.fieldOrPropertyType, value);
                                            }
                                            break;
                                        case WriterType.No:
                                            break;
                                        case WriterType.Public:
                                            if (item.Value.isPublic)
                                            {
                                                object value = item.Value.propertyInfo.GetValue(parent.data);
                                                // object value = item.Value.GetValue(parent.data);
                                                previous = ObjectItem(writers, nows, parent, previous, last, item.Key, item.Value.fieldOrPropertyType, value);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (classFieldWriter)
                                    {
                                        case WriterType.All:
                                            {
                                                object value = item.Value.fieldInfo.GetValue(parent.data);
                                                // object value = item.Value.GetValue(parent.data);
                                                previous = ObjectItem(writers, nows, parent, previous, last, item.Key, item.Value.fieldOrPropertyType, value);
                                            }
                                            break;
                                        case WriterType.No:
                                            break;
                                        case WriterType.Public:
                                            if (item.Value.isPublic)
                                            {
                                                object value = item.Value.fieldInfo.GetValue(parent.data);
                                                // object value = item.Value.GetValue(parent.data);
                                                previous = ObjectItem(writers, nows, parent, previous, last, item.Key, item.Value.fieldOrPropertyType, value);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        previous.back = last;
                        previous.isLast = true;
                    }
                }


                isRoot = false;
                parents = nows;
                nows = new List<JsonWriteValue>();
            }
            return writers;
        }


        /// <summary>
        /// 多维数组
        /// </summary>
        /// <param name="writers"></param>
        /// <param name="array"></param>
        /// <param name="parent"></param>
        /// <param name="last"></param>
        /// <param name="nows"></param>
        void ArrayToJsonString(
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
                now.jsonType = JsonWriteType.Array;
                now.arrayIndex = k;
                now.key = null;
                now.arrayIndex = k;
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
                        now.jsonType = JsonWriteType.Array;
                        now.arrayIndex = i;
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

            if (elementTypeCode == TypeCode.Object)
            {
                if (ArrayWrapper.IsObjectArrayAddOffcet && !elementTypeIsValue)
                {
                    startOffcet += ArrayWrapper.objectArray1StartOffcetAdd;
                }
            }

            int arrayIndex = 0;
            int arrayItemSize = UnsafeOperation.SizeOfStack(elementType);

            TypeAddrReflectionWrapper elementWrapper = null;
            if (elementTypeIsValue)
            {
                elementWrapper = GetTypeWarp(elementType);
            }
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
                    now.arrayIndex = i;
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
                                IntPtr* intPtr = elementWrapper.CreateGetIntPtr(out now.data);
                                ++intPtr;
                                GeneralTool.Memcpy(intPtr, value, elementWrapper.heapSize - UnsafeOperation.PTR_COUNT * 2);
                                //GC.Collect();
                            }
                            else
                            {
                                now.data = GeneralTool.VoidToObject(*(IntPtr**)value);
                            }
                            if (now.data == null)
                            {
                                now.jsonType = JsonWriteType.None;
                            }
                            else
                            {
                                if (elementType.IsArray)
                                {
                                    now.jsonType = JsonWriteType.Array;
                                }
                                else
                                {
                                    now.jsonType = JsonWriteType.Object;
                                }
                            }
                            nows.Add(now);


                            break;
                        case TypeCode.DBNull:
                            break;
                        case TypeCode.Boolean:
                            now.value = *(bool*)(value) ? "true" : "false";
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.Char:
                            now.value = "\"" + *(char*)(value) + "\"";
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.SByte:
                            now.value = (*(sbyte*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.Byte:
                            now.value = (*(Byte*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.Int16:
                            now.value = (*(Int16*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.UInt16:
                            now.value = (*(UInt16*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.Int32:
                            now.value = (*(Int32*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.UInt32:
                            now.value = (*(UInt32*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.Int64:
                            now.value = (*(Int64*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.UInt64:
                            now.value = (*(UInt64*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.Single:
                            now.value = (*(Single*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.Double:
                            now.value = (*(Double*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.Decimal:
                            now.value = (*(Decimal*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.DateTime:
                            now.value = (*(DateTime*)(value)).ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.String:
                            now.value = "\"" + (*(DateTime*)(value)) + "\"";
                            now.jsonType = JsonWriteType.String;
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

        class ObjectPath {
            public JsonWriteValue value;
            public string path;
        }

        Dictionary<object, ObjectPath> allPath = new Dictionary<object, ObjectPath>();
        StringBuilder path = new StringBuilder();

        public string GetRootPath(JsonWriteValue v)
        {
            path.Clear();
            while (v.parent != null)
            {
                if (v.parent.jsonType == JsonWriteType.Object)
                {
                    path.Insert(0, "/" + v.key);
                }
                else
                {
                    path.Insert(0, "/" + v.arrayIndex);
                }
                v = v.parent;
            }
            path.Insert(0, "$");
            return path.ToString();
        }

        bool isRoot = true;


        private JsonWriteValue ObjectItem(List<JsonWriteValue> writers, List<JsonWriteValue> nows, JsonWriteValue parent,
            JsonWriteValue previous, JsonWriteValue last, string key, Type fieldType, object value)
        {
            if (value == null)
            {
                JsonWriteValue now = new JsonWriteValue(writers.Count, parent);
                now.jsonType = JsonWriteType.None;
                now.data = null;
            }
            else
            {
                JsonWriteValue now;
                TypeCode typeCode;
                bool isArray = false;

                var valueType = value.GetType();

                IWriterCollectionString writeString = CollectionManager.GetWriterCollectionString(fieldType);
                if (writeString != null)
                {
                    now = new JsonWriteValue(writers.Count, parent);
                    now.data = value;
                    now.key = key;
                    previous.back = now;
                    now.jsonType = JsonWriteType.String;
                    now.value = "\"" + writeString.GetStringValue(value) + "\"";
                    writers.Add(now);
                    previous = now;
                    return previous;
                }

                bool isPath = false;
                if (!isRoot && value == writers[0].data)
                {
                    now = new JsonWriteValue(writers.Count, parent);
                    now.data = value;
                    now.key = key;
                    previous.back = now;
                    now.jsonType = JsonWriteType.String;
                    now.value = "\"$\"";
                    writers.Add(now);
                    previous = now;
                    return previous;
                }

                if (!valueType.IsValueType)
                {
                    ObjectPath path;
                    if (allPath.TryGetValue(value, out path))
                    {
                        if (path.path == null)
                        {
                            path.path = "\"" + GetRootPath(path.value) + "\"";
                        }
                        now = new JsonWriteValue(writers.Count, parent);
                        now.data = value;
                        now.key = key;
                        previous.back = now;
                        now.jsonType = JsonWriteType.String;
                        now.value = path.path;
                        writers.Add(now);
                        previous = now;
                        return previous;
                    }
                    else
                    {
                        allPath[parent.data] = new ObjectPath() { value = parent };
                    }
                }
                typeCode = Type.GetTypeCode(fieldType);

                if (fieldType.IsEnum)
                {
                    now = new JsonWriteValue(writers.Count, parent);
                    now.data = new EnumWrap(value);
                    now.key = key;
                    previous.back = now;
                    writers.Add(now);
                    previous = now;
                    now.jsonType = JsonWriteType.Array;
                    nows.Add(now);
                    return previous;



                    now = new JsonWriteValue(writers.Count, parent);
                    now.data = value;
                    now.key = key;
                    previous.back = now;
                    writers.Add(now);
                    previous = now;
                    now.jsonType = JsonWriteType.String;


                    Array Arrays = Enum.GetValues(fieldType);
                    switch (typeCode)
                    {
                        case TypeCode.SByte:
                            now.value = "\"" + Arrays.GetValue((SByte)now.data) + "\"";
                            break;
                        case TypeCode.Byte:
                            now.value = "\"" + Arrays.GetValue((Byte)now.data) + "\"";
                            break;
                        case TypeCode.Int16:
                            now.value = "\"" + Arrays.GetValue((Int16)now.data) + "\"";
                            break;
                        case TypeCode.UInt16:
                            now.value = "\"" + Arrays.GetValue((UInt16)now.data) + "\"";
                            break;
                        case TypeCode.Int32:
                            now.value = "\"" + Arrays.GetValue((Int32)now.data) + "\"";
                            break;
                        case TypeCode.UInt32:
                            now.value = "\"" + Arrays.GetValue((UInt32)now.data) + "\"";
                            break;
                        case TypeCode.Int64:
                            now.value = "\"" + Arrays.GetValue((Int64)now.data) + "\"";
                            break;
                        case TypeCode.UInt64:
                            now.value = "\"" + Arrays.GetValue((long)(UInt64)now.data) + "\"";
                            break;
                        default:
                            now.value = "\"" + now.data + "\"";
                            break;
                    }
                }
                else
                {
                    if (valueType != fieldType
                       && fieldType != typeof(Type)
                       && !valueType.IsSubclassOf(typeof(Type))
                        )
                    {
                        typeCode = Type.GetTypeCode(valueType);
                        isArray = valueType.IsArray;

                        var writeObject = CollectionManager.GetWriterCollection(valueType);

                        if (typeCode == TypeCode.Object && !isArray &&
                            (writeObject == null ||
                            writeObject.GetWriteType() == JsonWriteType.Object)
                            )
                        {
                            //JsonWriteValue newParent = new JsonWriteValue(writers.Count, parent);
                            //newParent.key = key;
                            //previous.back = newParent;
                            //newParent.jsonType = JsonWriteType.Object;
                            //writers.Add(newParent);
                            //previous = newParent;
                            //newParent.back = last;

                            //JsonWriteValue typeWrite = new JsonWriteValue(writers.Count, newParent);
                            //typeWrite.key = "$type";
                            //typeWrite.value = "\"" + valueType.Assembly.GetName().Name + "," + valueType.ToString() + "\"";
                            //previous.back = typeWrite;
                            //typeWrite.jsonType = JsonWriteType.String;
                            //writers.Add(typeWrite);
                            //previous = typeWrite;

                            now = new JsonWriteValue(writers.Count, parent);
                            now.data = value; 
                            now.isSetType = true; 
                            now.key = key;
                            now.collection = writeObject;
                            now.type = valueType;
                            previous.back = now;
                            writers.Add(now);
                            previous = now;

                            now.jsonType = JsonWriteType.Object;
                            nows.Add(now);
                            return previous;

                        }
                        else
                        {
                            now = new JsonWriteValue(writers.Count, parent);
                            now.data = Activator.CreateInstance(typeof(Box<>).MakeGenericType(valueType));
                            IBox box = (IBox)now.data;
                            box.SetObject(value);

                            now.key = key;
                            now.type = valueType;
                            previous.back = now;
                            writers.Add(now);
                            previous = now;
                            now.jsonType = JsonWriteType.Object;
                            nows.Add(now);
                            return previous;

                            //JsonWriteValue newParent = new JsonWriteValue(writers.Count, parent);
                            //newParent.key = key;
                            //previous.back = newParent;
                            //newParent.jsonType = JsonWriteType.Object;
                            //writers.Add(newParent);
                            //previous = newParent;
                            //newParent.back = last;

                            //JsonWriteValue typeWrite = new JsonWriteValue(writers.Count, newParent);
                            //typeWrite.key = "$type";
                            //typeWrite.value = "\"" + valueType.Assembly.GetName().Name + "," + valueType.ToString() + "\"";
                            //previous.back = typeWrite;
                            //typeWrite.jsonType = JsonWriteType.String;
                            //writers.Add(typeWrite);
                            //previous = typeWrite;

                            //now = new JsonWriteValue(writers.Count, newParent);
                            //now.data = value;
                            //now.key = "$value";
                            //previous.back = now;
                            //writers.Add(now);
                            //previous = now;
                            //now.isLast = true;
                        }

                    }
                    else
                    {
                        typeCode = Type.GetTypeCode(fieldType);
                        isArray = fieldType.IsArray;
                        now = new JsonWriteValue(writers.Count, parent);
                        now.data = value;
                        now.key = key;
                        previous.back = now;
                        writers.Add(now);
                        previous = now;
                    }


                    switch (typeCode)
                    {
                        case TypeCode.Empty:
                            break;
                        case TypeCode.Object:
                            if (isArray)
                            {
                                now.jsonType = JsonWriteType.Array;
                                nows.Add(now);
                            }
                            else
                            {
                                now.jsonType = JsonWriteType.Object;
                                nows.Add(now);
                            }
                            break;
                        case TypeCode.DBNull:
                            break;
                        case TypeCode.Boolean:
                            now.value = ((bool)value) ? "true" : "false";
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.Char:
                            now.value = "\"" + (char)value + "\"";
                            now.jsonType = JsonWriteType.String;
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
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.DateTime:
                            now.value = value.ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.String:
                            now.value = "\"" + (string)value + "\"";
                            now.jsonType = JsonWriteType.String;
                            break;
                        default:
                            break;
                    }
                }
            }
            return previous;
        }


        private JsonWriteValue ArrayItem(int i, List<JsonWriteValue> writers, List<JsonWriteValue> nows, JsonWriteValue parent, JsonWriteValue previous,
            JsonWriteValue last, Type fieldType, TypeCode elementTypeCode, object value)
        {
            JsonWriteValue now = new JsonWriteValue(writers.Count, parent);

            if (value == null)
            {
                now.jsonType = JsonWriteType.None;
                now.data = null;
                if (fieldType.IsEnum)
                {
                    Array Arrays = Enum.GetValues(fieldType);
                    if (Arrays.Length > 0)
                    {
                        now.value = "\"" + Arrays.GetValue(0).ToString() + "\"";
                        now.jsonType = JsonWriteType.String;
                        now.key = null;
                        now.arrayIndex = i;
                        writers.Add(now);
                        previous.back = now;
                        previous = now;
                    }
                }
            }
            else
            {
                var valueType = value.GetType();


                IWriterCollectionString writeString = CollectionManager.GetWriterCollectionString(fieldType);
                if (writeString != null)
                {
                    now = new JsonWriteValue(writers.Count, parent);
                    now.data = value;
                    now.arrayIndex = i;
                    previous.back = now;
                    now.jsonType = JsonWriteType.String;
                    now.value = "\"" + writeString.GetStringValue(value) + "\"";
                    writers.Add(now);
                    previous = now;
                    return previous;
                }

                bool isPath = false;
                if (!isRoot && value == writers[0].data)
                {
                    isPath = true;
                    now = new JsonWriteValue(writers.Count, parent);
                    now.data = value;
                    now.arrayIndex = i;
                    previous.back = now;
                    now.jsonType = JsonWriteType.String;
                    now.value = "\"$\"";
                    writers.Add(now);
                    previous = now;
                    isPath = true;
                    return previous;
                }

                if (!valueType.IsValueType)
                {
                    ObjectPath path;
                    if (allPath.TryGetValue(value, out path))
                    {
                        if (path.path == null)
                        {
                            path.path = "\"" + GetRootPath(path.value) + "\"";
                        }
                        now = new JsonWriteValue(writers.Count, parent);
                        now.data = value;
                        now.arrayIndex = i;
                        previous.back = now;
                        now.jsonType = JsonWriteType.String;
                        now.value = path.path;
                        writers.Add(now);
                        previous = now;
                        return previous;
                    }
                    else
                    {
                        allPath[parent.data] = new ObjectPath() { value = parent };
                    }
                }

                if (fieldType.IsEnum)
                {
                    now.key = null;
                    now.arrayIndex = i;
                    writers.Add(now);
                    previous.back = now;
                    previous = now;

                    Enum _enum = (Enum)value;
                    now.value = "\"" + _enum.ToString() + "\"";
                    now.jsonType = JsonWriteType.String;
                }
                else
                {

                    if (valueType != fieldType
                       && fieldType != typeof(Type)
                       && !valueType.IsSubclassOf(typeof(Type))
                        )
                    {
                        var typeCode = Type.GetTypeCode(valueType);
                        var isArray = valueType.IsArray;
                        var writeObject = CollectionManager.GetWriterCollection(valueType);

                        if (typeCode == TypeCode.Object && !isArray &&
                            (writeObject == null ||
                            writeObject.GetWriteType() == JsonWriteType.Object)
                            )
                        {
                            now = new JsonWriteValue(writers.Count, parent);
                            now.data = value;
                            now.isSetType = true;
                            now.arrayIndex = i;
                            now.collection = writeObject;
                            now.type = valueType;
                            previous.back = now;
                            writers.Add(now);
                            previous = now;

                            now.jsonType = JsonWriteType.Object;
                            nows.Add(now);
                            return previous;

                        }
                        else
                        {
                            now = new JsonWriteValue(writers.Count, parent);
                            now.data = Activator.CreateInstance(typeof(Box<>).MakeGenericType(valueType));
                            IBox box = (IBox)now.data;
                            box.SetObject(value);

                            now.arrayIndex = i;
                            now.type = valueType;
                            previous.back = now;
                            writers.Add(now);
                            previous = now;
                            now.jsonType = JsonWriteType.Object;
                            nows.Add(now);
                            return previous;
                        }
                    }
                    else
                    {
                        now.key = null;
                        now.arrayIndex = i;
                        writers.Add(now);
                        previous.back = now;
                        previous = now;
                    }


                    now.data = value;
                    switch (elementTypeCode)
                    {
                        case TypeCode.Empty:
                            break;
                        case TypeCode.Object:
                            if (fieldType.IsArray)
                            {
                                now.jsonType = JsonWriteType.Array;
                                nows.Add(now);
                            }
                            else
                            {
                                now.jsonType = JsonWriteType.Object;
                                nows.Add(now);
                            }

                            break;
                        case TypeCode.DBNull:
                            break;
                        case TypeCode.Boolean:
                            now.value = ((bool)value) ? "true" : "false";
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.Char:
                            now.value = "\"" + (char)value + "\"";
                            now.jsonType = JsonWriteType.String;
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
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.DateTime:
                            now.value = value.ToString();
                            now.jsonType = JsonWriteType.String;
                            break;
                        case TypeCode.String:
                            now.value = "\"" + (string)value + "\"";
                            now.jsonType = JsonWriteType.String;
                            break;
                        default:
                            break;
                    }
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
