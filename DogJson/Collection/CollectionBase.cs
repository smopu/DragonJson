using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    public unsafe interface IWriterCollectionObject
    {
        IEnumerable<KeyValueStruct> GetValue(object obj);
        JsonWriteType GetWriteType();
    }

    public unsafe interface IWriterCollectionString
    {
        string GetStringValue(object obj);
    }

    public struct KeyValueStruct
    {
        public string key;
        public object value;
        public Type type;
    }


    public class EnumTypeWrap
    {
        public long[] values;
        public string[] names;
        public Dictionary<string, long> nameToValue;
        public Dictionary<long, string> valueToName;
        public TypeCode typeCode;

        public EnumTypeWrap(Type enumType)
        {
            typeCode = Type.GetTypeCode(enumType);
            Array array = Enum.GetValues(enumType);
            nameToValue = new Dictionary<string, long>();
            valueToName = new Dictionary<long, string>();
            values = new long[array.Length];
            names = new string[array.Length];
            for (int i = 0, length = array.Length; i < length; i++)
            {
                object value = array.GetValue(i);
                long id = 0;
                switch (typeCode)
                {
                    case TypeCode.SByte:
                        id = (long)(SByte)value;
                        break;
                    case TypeCode.Byte:
                        id = (long)(Byte)value;
                        break;
                    case TypeCode.Int16:
                        id = (long)(Int16)value;
                        break;
                    case TypeCode.UInt16:
                        id = (long)(UInt16)value;
                        break;
                    case TypeCode.Int32:
                        id = (long)(Int32)value;
                        break;
                    case TypeCode.UInt32:
                        id = (long)(UInt32)value;
                        break;
                    case TypeCode.Int64:
                        id = (long)(Int64)value;
                        break;
                    case TypeCode.UInt64:
                        id = (long)(UInt64)value;
                        break;
                }
                values[i] = id;
                names[i] = value.ToString();
                nameToValue[value.ToString()] = id;
                valueToName[id] = value.ToString();
            }
        }

        public IEnumerable<string> GetValue(object inEnum)
        {
            long data = 0;
            switch (typeCode)
            {
                case TypeCode.SByte:
                    data = (long)(SByte)inEnum;
                    break;
                case TypeCode.Byte:
                    data = (long)(Byte)inEnum;
                    break;
                case TypeCode.Int16:
                    data = (long)(Int16)inEnum;
                    break;
                case TypeCode.UInt16:
                    data = (long)(UInt16)inEnum;
                    break;
                case TypeCode.Int32:
                    data = (long)(Int32)inEnum;
                    break;
                case TypeCode.UInt32:
                    data = (long)(UInt32)inEnum;
                    break;
                case TypeCode.Int64:
                    data = (long)(Int64)inEnum;
                    break;
                case TypeCode.UInt64:
                    data = (long)(UInt64)inEnum;
                    break;
            }

            string name;
            if (valueToName.TryGetValue(data, out name))
            {
                yield return name;
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    var id = values[i];
                    if ((data & id) == id)
                    {
                        data &= ~id;
                        yield return names[i];
                        if (data == 0)
                        {
                            break;
                        }
                    }
                }
            }


        }

        public long GetValue(IEnumerable<string> vs)
        {
            long data = 0;
            foreach (var item in vs)
            {
                data |= nameToValue[item];
            }
            return data;
        }
    }





    public unsafe abstract class CollectionArrayBase<CollectionType, TempCollectionType> : IReadCollectionObject
    {
        protected abstract void Add(TempCollectionType obj, int index, object value, ReadCollectionProxy proxy);

        public void Add(object obj, JsonObject* bridge, object value, ReadCollectionProxy proxy)
        //public void Add(object obj, int index, object value)
        {
            Add((TempCollectionType)obj, bridge->arrayIndex, value, proxy);
        }

        protected abstract void AddValue(TempCollectionType obj, int index, char* str, JsonValue* value, ReadCollectionProxy proxy);

        public void AddValue(object obj, char* str, JsonValue* value, ReadCollectionProxy proxy)
        {
            AddValue((TempCollectionType)obj, value->arrayIndex, str, value, proxy);
        }

        protected abstract TempCollectionType CreateArray(int arrayCount, object parent, Type arrayType, Type parentType);
        public object Create(JsonObject* obj, object parent, Type arrayType, Type parentType)
        {
            return CreateArray(obj->arrayCount, parent, arrayType, parentType);
        }
        protected abstract CollectionType End(TempCollectionType obj);

        public object End(object obj)
        {
            return End((TempCollectionType)obj);
        }
        public Type GetItemType(JsonObject* bridge) 
        {
            return GetItemType(bridge->arrayIndex);
        }
        public abstract Type GetItemType(int index);

        bool isRef = false;
        public CollectionArrayBase()
        {
            if (typeof(CollectionType) == typeof(TempCollectionType))
            {
                isRef = true;
            }
        }
        public virtual bool IsRef()
        {
            return isRef;
        }

    }

    public unsafe class ReadCollectionProxy
    {
        public delegate object GetValue(TypeCode typeCode, char* str, JsonValue* value);
        public GetValue callGetValue;
    }

    public unsafe interface IReadCollectionObject
    {
        void Add(object obj, JsonObject* bridge, object value, ReadCollectionProxy proxy);
        void AddValue(object obj, char* str, JsonValue* value, ReadCollectionProxy proxy);
        object Create(JsonObject* obj, object parent, Type objectType, Type parentType);
        bool IsRef();
        object End(object obj);
        Type GetItemType(JsonObject* bridge);
    }


    public unsafe interface CreateTaget<T>
    {
        T Create();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe class ReadCollectionLink
    {
        public delegate object CallBackGetValue(TypeCode typeCode, char* str, JsonValue* value);

        public struct AddValue_Args
        {
            public char* str;
            public JsonValue* value;
            public object temp;
            public CallBackGetValue callGetValue;
        }

        public struct Add_Args
        {
            public object temp;
            public JsonObject* bridge;
        }

        public struct Create_Args
        {
            public JsonObject* bridge;
            //public void* parent;
            //public object parent;
            //public Type objectType;
            //public Type parentType;
        }

        public struct GetItemType_Args
        {
            public JsonObject* bridge;
        }

        public unsafe delegate void AddValueStruct(void* obj, AddValue_Args arg);
        public unsafe delegate void AddValueClass(object obj, AddValue_Args arg);

        public unsafe delegate void AddObjectStruct(void* obj, object value, Add_Args arg);
        public unsafe delegate void AddObjectClass(object obj, object value, Add_Args arg);


        public unsafe delegate void AddObject(void* obj, void* value, Add_Args arg);
        public unsafe delegate void AddObject1(object obj, void* value, Add_Args arg);
        public unsafe delegate void AddObject2(void* obj, object value, Add_Args arg);



        public unsafe delegate object CreateObject(out object temp, Create_Args arg);
        public unsafe delegate void CreateValue(void* obj, out object temp, Create_Args arg);

        public unsafe delegate void Create2(out object obj, out void* dataStart, out object temp, Create_Args arg);
        public unsafe delegate void CreateByte(out byte* obj, out byte* dataStart, out object temp, Create_Args arg);

        public unsafe delegate void End(void* obj, object temp);
        public unsafe delegate object EndObject(object obj, object temp);

        public unsafe delegate CollectionManager.TypeAllCollection GetItemType(GetItemType_Args arg);

        const int Siez_0 = 0;
        const int Siez_1 = 8 * 1;
        const int Siez_2 = 8 * 2;
        const int Siez_3 = 8 * 3;
        const int Siez_4 = 8 * 4;
        const int Siez_5 = 8 * 5;
        const int Siez_6 = 8 * 6;
        const int Siez_7 = 8 * 7;
        const int Siez_8 = 8 * 8;
        const int Siez_9 = 8 * 9;
        const int Siez_10 = 8 * 10;


        /// <summary>
        /// 添加基本json类型时 容器是class
        /// </summary>
        [FieldOffset(Siez_0)]
        public Delegate addValueStructDelegate;
        [FieldOffset(Siez_0)]
        public AddValueStruct addValueStruct;


        /// <summary>
        /// 添加基本json类型时 容器是class
        /// </summary>
        [FieldOffset(Siez_1)]
        public Delegate addValueClassDelegate;
        [FieldOffset(Siez_1)]
        public AddValueClass addValueClass;


        [FieldOffset(Siez_2)]
        public Delegate addObjectClassDelegate;
        [FieldOffset(Siez_2)]
        public AddObjectClass addObjectClass;


        [FieldOffset(Siez_3)]
        public Delegate addObjectStructDelegate;
        [FieldOffset(Siez_3)]
        public AddObjectStruct addObjectStruct;


        [FieldOffset(Siez_4)]
        public Delegate createObjectDelegate;
        [FieldOffset(Siez_4)]
        public CreateObject createObject;

        [FieldOffset(Siez_5)]
        public Delegate createValueDelegate;
        [FieldOffset(Siez_5)]
        public CreateValue createValue;

        [FieldOffset(Siez_6)]
        public Delegate endDelegate;
        [FieldOffset(Siez_6)]
        public End end;
        [FieldOffset(Siez_6)]
        public EndObject endObject;


        [FieldOffset(Siez_7)]
        public Delegate getItemTypeDelegate;
        [FieldOffset(Siez_7)]
        public GetItemType getItemType;

        [FieldOffset(Siez_8)]
        public bool isLaze;
    }


    public unsafe abstract class CollectionObjectStructBase<T> : CollectionObjectBase<T, Box<T>> where T : struct
    {
        public override bool IsRef()
        {
            return false;
        }
        protected override T End(Box<T> obj)
        {
            return obj.value;
        }
        protected override Box<T> CreateObject(JsonObject* obj, object parent, Type objectType, Type parentType)
        {
            return new Box<T>();
        }
    }

    public unsafe abstract class CollectionObjectBase<CollectionType, TempCollectionType> : IReadCollectionObject
    {
        bool isRef = false;
        public CollectionObjectBase()
        {
            if (typeof(CollectionType) == typeof(TempCollectionType))
            {
                isRef = true;
            }
        }
        public virtual bool IsRef()
        {
            return isRef;
        }
        public void Add(object obj, JsonObject* bridge, object value, ReadCollectionProxy proxy)
        {
            Add((TempCollectionType) obj, bridge->keyStringStart, bridge->keyStringLength, value, proxy);
        }
        public void AddValue(object obj, char* str, JsonValue* value, ReadCollectionProxy proxy)
        {
            AddValue((TempCollectionType)obj, str + value->keyStringStart, value->keyStringLength, str, value, proxy);
        }

        public object Create(JsonObject* obj, object parent, Type objectType, Type parentType)
        {
            return CreateObject(obj, parent, objectType, parentType);
        }

        public object End(object obj)
        {
            return End((TempCollectionType)obj);
        }

        protected virtual void Add(TempCollectionType obj, char* key, int keyLength, object value, ReadCollectionProxy proxy) { }
        protected virtual void AddValue(TempCollectionType obj, char* key, int keyLength, char* str, JsonValue* value, ReadCollectionProxy proxy) { }
        protected abstract TempCollectionType CreateObject(JsonObject* obj, object parent, Type objectType, Type parentType);
        protected abstract CollectionType End(TempCollectionType obj);
        public abstract Type GetItemType(JsonObject* bridge);
    }

}
