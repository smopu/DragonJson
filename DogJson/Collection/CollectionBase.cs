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
        //protected object GetValue(TypeCode typeCode, char* str, JsonValue* value)
        //{
        //    switch (value->type)
        //    {
        //        case JsonValueType.String:
        //            switch (typeCode)
        //            {
        //                case TypeCode.Char:
        //                    return str[value->vStringStart];
        //                case TypeCode.String:
        //                    return new string(str, value->vStringStart, value->vStringLength);
        //            }
        //            break;
        //        case JsonValueType.Long:
        //            switch (typeCode)
        //            {
        //                case TypeCode.SByte:
        //                    return (SByte)value->valueLong;
        //                case TypeCode.Byte:
        //                    return (Byte)value->valueLong;
        //                case TypeCode.Int16:
        //                    return (Int16)value->valueLong;
        //                case TypeCode.UInt16:
        //                    return (UInt16)value->valueLong;
        //                case TypeCode.Int32:
        //                    return (Int32)value->valueLong;
        //                case TypeCode.UInt32:
        //                    return (UInt32)value->valueLong;
        //                case TypeCode.Int64:
        //                    return value->valueLong;
        //                case TypeCode.UInt64:
        //                    return (UInt64)value->valueLong;
        //                case TypeCode.Single:
        //                    return (Single)value->valueLong;
        //                case TypeCode.Double:
        //                    return (Double)value->valueLong;
        //                case TypeCode.Decimal:
        //                    return (Decimal)value->valueLong;
        //            }
        //            break;
        //        case JsonValueType.Double:
        //            switch (typeCode)
        //            {
        //                case TypeCode.Single:
        //                    return (Single)value->valueDouble;
        //                case TypeCode.Double:
        //                    return (Double)value->valueDouble;
        //                case TypeCode.Decimal:
        //                    return (Decimal)value->valueDouble;
        //            }
        //            break;
        //        case JsonValueType.Boolean:
        //            return value->valueBool;
        //    }
        //    return null;
        //}
      
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

    //public unsafe interface ICollectionObjectBase
    //{
    //    void Add(object obj, int index, object value);
    //    void AddValue(object obj, int index, char* str, JsonValue* value);
    //    object Create(int arrayCount, Type arrayType, Type parentType);
    //    object End(object obj);
    //    Type GetItemType(int index);
    //}

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
            public CallBackGetValue callGetValue;
        }

        public struct Add_Args
        {
            public JsonObject* bridge;
        }

        public struct Create_Args
        {
            public JsonObject* bridge;
            public void* parent;
            public void* structPtr;
           
            //public object parent;
            public Type objectType;
            //public Type parentType;
        }

        public struct GetItemType_Args
        {
            public JsonObject* bridge;
        }

        public unsafe delegate void AddValue(void* obj, AddValue_Args arg);
        public unsafe delegate void Add(void* obj, void* value, Add_Args arg);
        public unsafe delegate void Add1(object obj, void* value, Add_Args arg);
        public unsafe delegate void Add2(void* obj, object value, Add_Args arg);
        public unsafe delegate void Add3(object obj, object value, Add_Args arg);

        public unsafe delegate void Create(out object obj, out void* dataStart, out object temp, Create_Args arg);
        public unsafe delegate void Create2(out void* obj, out void* dataStart, out object temp, Create_Args arg);
        public unsafe delegate void CreateByte(out byte* obj, out byte* dataStart, out object temp, Create_Args arg);

        public unsafe delegate void End(object temp);
        public unsafe delegate Type GetItemType(GetItemType_Args arg);

        [FieldOffset(0)]
        public Delegate addValueDelegate;
        [FieldOffset(0)]
        public AddValue addValue;

        [FieldOffset(8)]
        public Delegate addDelegate;
        [FieldOffset(8)]
        public Add add;
        [FieldOffset(8)]
        public Add1 add1;
        [FieldOffset(8)]
        public Add2 add2;
        [FieldOffset(8)]
        public Add3 add3;

        [FieldOffset(16)]
        public Delegate createDelegate;
        [FieldOffset(16)]
        public Create create;
        [FieldOffset(16)]
        public CreateByte createByte;

        [FieldOffset(24)]
        public bool isRef;

        [FieldOffset(32)]
        public Delegate endDelegate;
        [FieldOffset(32)]
        public End end;

        [FieldOffset(40)]
        public Delegate getItemTypeDelegate;
        [FieldOffset(40)]
        public GetItemType getItemType;
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

        //protected object GetValue(TypeCode typeCode, char* str, JsonValue* value)
        //{
        //    switch (value->type)
        //    {
        //        case JsonValueType.String:
        //            switch (typeCode)
        //            {
        //                case TypeCode.Char:
        //                    return str[value->vStringStart];
        //                case TypeCode.String:
        //                    return new string(str, value->vStringStart, value->vStringLength);
        //            }
        //            break;
        //        case JsonValueType.Long:
        //            switch (typeCode)
        //            {
        //                case TypeCode.SByte:
        //                    return (SByte)value->valueLong;
        //                case TypeCode.Byte:
        //                    return (Byte)value->valueLong;
        //                case TypeCode.Int16:
        //                    return (Int16)value->valueLong;
        //                case TypeCode.UInt16:
        //                    return (UInt16)value->valueLong;
        //                case TypeCode.Int32:
        //                    return (Int32)value->valueLong;
        //                case TypeCode.UInt32:
        //                    return (UInt32)value->valueLong;
        //                case TypeCode.Int64:
        //                    return value->valueLong;
        //                case TypeCode.UInt64:
        //                    return (UInt64)value->valueLong;
        //                case TypeCode.Single:
        //                    return (Single)value->valueLong;
        //                case TypeCode.Double:
        //                    return (Double)value->valueLong;
        //                case TypeCode.Decimal:
        //                    return (Decimal)value->valueLong;
        //            }
        //            break;
        //        case JsonValueType.Double:
        //            switch (typeCode)
        //            {
        //                case TypeCode.Single:
        //                    return (Single)value->valueDouble;
        //                case TypeCode.Double:
        //                    return (Double)value->valueDouble;
        //                case TypeCode.Decimal:
        //                    return (Decimal)value->valueDouble;
        //            }
        //            break;
        //        case JsonValueType.Boolean:
        //            return value->valueBool;
        //    }
        //    return null;
        //}

        protected virtual void Add(TempCollectionType obj, char* key, int keyLength, object value, ReadCollectionProxy proxy) { }
        protected virtual void AddValue(TempCollectionType obj, char* key, int keyLength, char* str, JsonValue* value, ReadCollectionProxy proxy) { }
        protected abstract TempCollectionType CreateObject(JsonObject* obj, object parent, Type objectType, Type parentType);
        protected abstract CollectionType End(TempCollectionType obj);
        public abstract Type GetItemType(JsonObject* bridge);
    }

    //public unsafe abstract class CollectionObjectBase<CollectionType> : IReadCollectionObject
    //{
    //    public virtual bool IsRef()
    //    {
    //        return true;
    //    }
    //    public void Add(object obj, JsonObject* bridge, object value)
    //    {
    //    }

    //    public void AddValue(object obj, char* str, JsonValue* value)
    //    {
    //    }

    //    public object Create(JsonObject* obj, object parent, Type objectType, Type parentType)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public virtual object End(object obj)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public virtual Type GetItemType(JsonObject* bridge)
    //    {
    //        throw new NotImplementedException();
    //    }

    //}



}
