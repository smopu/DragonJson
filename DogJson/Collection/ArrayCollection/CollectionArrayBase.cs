using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    public unsafe abstract class CollectionArrayBase<CollectionType, TempCollectionType> : ICollectionObjectBase
    {
        protected abstract void Add(TempCollectionType obj, int index, object value);

        public void Add(object obj, JsonObject* bridge, object value)
        //public void Add(object obj, int index, object value)
        {
            Add((TempCollectionType)obj, bridge->arrayIndex, value);
        }

        protected abstract void AddValue(TempCollectionType obj, int index, char* str, JsonValue* value);

        public void AddValue(object obj, char* str, JsonValue* value)
        {
            AddValue((TempCollectionType)obj, value->arrayIndex, str, value);
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
        protected object GetValue(TypeCode typeCode, char* str, JsonValue* value)
        {
            switch (value->type)
            {
                case JsonValueType.String:
                    switch (typeCode)
                    {
                        case TypeCode.Char:
                            return str[value->vStringStart];
                        case TypeCode.String:
                            return new string(str, value->vStringStart, value->vStringLength);
                    }
                    break;
                case JsonValueType.Long:
                    switch (typeCode)
                    {
                        case TypeCode.SByte:
                            return (SByte)value->valueLong;
                        case TypeCode.Byte:
                            return (Byte)value->valueLong;
                        case TypeCode.Int16:
                            return (Int16)value->valueLong;
                        case TypeCode.UInt16:
                            return (UInt16)value->valueLong;
                        case TypeCode.Int32:
                            return (Int32)value->valueLong;
                        case TypeCode.UInt32:
                            return (UInt32)value->valueLong;
                        case TypeCode.Int64:
                            return value->valueLong;
                        case TypeCode.UInt64:
                            return (UInt64)value->valueLong;
                        case TypeCode.Single:
                            return (Single)value->valueLong;
                        case TypeCode.Double:
                            return (Double)value->valueLong;
                        case TypeCode.Decimal:
                            return (Decimal)value->valueLong;
                    }
                    break;
                case JsonValueType.Double:
                    switch (typeCode)
                    {
                        case TypeCode.Single:
                            return (Single)value->valueDouble;
                        case TypeCode.Double:
                            return (Double)value->valueDouble;
                        case TypeCode.Decimal:
                            return (Decimal)value->valueDouble;
                    }
                    break;
                case JsonValueType.Boolean:
                    return value->valueBool;
            }
            return null;
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

    //public unsafe interface ICollectionObjectBase
    //{
    //    void Add(object obj, int index, object value);
    //    void AddValue(object obj, int index, char* str, JsonValue* value);
    //    object Create(int arrayCount, Type arrayType, Type parentType);
    //    object End(object obj);
    //    Type GetItemType(int index);
    //}

    public unsafe interface ICollectionObjectBase
    {
        void Add(object obj, JsonObject* bridge, object value);
        void AddValue(object obj, char* str, JsonValue* value);
        object Create(JsonObject* obj, object parent, Type objectType, Type parentType);
        bool IsRef();
        object End(object obj);
        Type GetItemType(JsonObject* bridge);
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

    public unsafe abstract class CollectionObjectBase<CollectionType, TempCollectionType> : ICollectionObjectBase
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
        public void Add(object obj, JsonObject* bridge, object value)
        {
            Add((TempCollectionType) obj, bridge->keyStringStart, bridge->keyStringLength, value);
        }
        public void AddValue(object obj, char* str, JsonValue* value)
        {
            AddValue((TempCollectionType)obj, str + value->keyStringStart, value->keyStringLength, str, value);
        }

        public object Create(JsonObject* obj, object parent, Type objectType, Type parentType)
        {
            return CreateObject(obj, parent, objectType, parentType);
        }

        public object End(object obj)
        {
            return End((TempCollectionType)obj);
        }

        protected object GetValue(TypeCode typeCode, char* str, JsonValue* value)
        {
            switch (value->type)
            {
                case JsonValueType.String:
                    switch (typeCode)
                    {
                        case TypeCode.Char:
                            return str[value->vStringStart];
                        case TypeCode.String:
                            return new string(str, value->vStringStart, value->vStringLength);
                    }
                    break;
                case JsonValueType.Long:
                    switch (typeCode)
                    {
                        case TypeCode.SByte:
                            return (SByte)value->valueLong;
                        case TypeCode.Byte:
                            return (Byte)value->valueLong;
                        case TypeCode.Int16:
                            return (Int16)value->valueLong;
                        case TypeCode.UInt16:
                            return (UInt16)value->valueLong;
                        case TypeCode.Int32:
                            return (Int32)value->valueLong;
                        case TypeCode.UInt32:
                            return (UInt32)value->valueLong;
                        case TypeCode.Int64:
                            return value->valueLong;
                        case TypeCode.UInt64:
                            return (UInt64)value->valueLong;
                        case TypeCode.Single:
                            return (Single)value->valueLong;
                        case TypeCode.Double:
                            return (Double)value->valueLong;
                        case TypeCode.Decimal:
                            return (Decimal)value->valueLong;
                    }
                    break;
                case JsonValueType.Double:
                    switch (typeCode)
                    {
                        case TypeCode.Single:
                            return (Single)value->valueDouble;
                        case TypeCode.Double:
                            return (Double)value->valueDouble;
                        case TypeCode.Decimal:
                            return (Decimal)value->valueDouble;
                    }
                    break;
                case JsonValueType.Boolean:
                    return value->valueBool;
            }
            return null;
        }

        protected virtual void Add(TempCollectionType obj, char* key, int keyLength, object value) { }
        protected virtual void AddValue(TempCollectionType obj, char* key, int keyLength, char* str, JsonValue* value) { }
        protected abstract TempCollectionType CreateObject(JsonObject* obj, object parent, Type objectType, Type parentType);
        protected abstract CollectionType End(TempCollectionType obj);
        public abstract Type GetItemType(JsonObject* bridge);
    }

    public unsafe abstract class CollectionObjectBase<CollectionType> : ICollectionObjectBase
    {
        public virtual bool IsRef()
        {
            return true;
        }
        public void Add(object obj, JsonObject* bridge, object value)
        {
        }

        public void AddValue(object obj, char* str, JsonValue* value)
        {
        }

        public object Create(JsonObject* obj, object parent, Type objectType, Type parentType)
        {
            throw new NotImplementedException();
        }

        public virtual object End(object obj)
        {
            throw new NotImplementedException();
        }

        public virtual Type GetItemType( JsonObject* bridge)
        {
            throw new NotImplementedException();
        }

    }



}
