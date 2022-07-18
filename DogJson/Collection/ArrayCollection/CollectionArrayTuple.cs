//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DogJson
//{

//    [Collection(typeof(Tuple<>), true)]
//    public unsafe class CollectionArrayTuple<T> : CollectionArrayBase
//    {
//        class MyTuple 
//        {
//            public object value;
//        }

//        TypeCode typeCode;
//        public CollectionArrayTuple()
//        {
//            typeCode = Type.GetTypeCode(typeof(T));
//        }

//        public override void Add(object obj, int index, object value)
//        {
//            MyTuple array = (MyTuple)obj;
//            array.value = value;
//        }

//        public override void AddValue(object obj, int index, char* str, JsonValue value)
//        {
//            MyTuple array = (MyTuple)obj;
//            object set_value = default(T);
//            switch (value.type)
//            {
//                case JsonValueType.String:
//                    switch (typeCode)
//                    {
//                        case TypeCode.Char:
//                            set_value = str[value.vStringStart];
//                            break;
//                        case TypeCode.String:
//                            set_value = new string(str, value.vStringStart, value.vStringLength);
//                            break;
//                    }
//                    break;
//                case JsonValueType.Long:
//                    switch (typeCode)
//                    {
//                        case TypeCode.SByte:
//                            set_value = (SByte)value.valueLong;
//                            break;
//                        case TypeCode.Byte:
//                            set_value = (Byte)value.valueLong;
//                            break;
//                        case TypeCode.Int16:
//                            set_value = (Int16)value.valueLong;
//                            break;
//                        case TypeCode.UInt16:
//                            set_value = (UInt16)value.valueLong;
//                            break;
//                        case TypeCode.Int32:
//                            set_value = (Int32)value.valueLong;
//                            break;
//                        case TypeCode.UInt32:
//                            set_value = (UInt32)value.valueLong;
//                            break;
//                        case TypeCode.Int64:
//                            set_value = value.valueLong;
//                            break;
//                        case TypeCode.UInt64:
//                            set_value = (UInt64)value.valueLong;
//                            break;
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueLong;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueLong;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueLong;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Double:
//                    switch (typeCode)
//                    {
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueDouble;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueDouble;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueDouble;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Boolean:
//                    set_value = value.valueBool;
//                    break;
//            }
//            array.value = set_value;
//        }

//        public override object CreateArray(int arrayCount, Type arrayType, Type parentType)
//        {
//            return new MyTuple();
//        }

//        public override Type GetItemType(int index)
//        {
//            return typeof(T);
//        }

//        public override object End(object obj)
//        {
//            MyTuple array = (MyTuple)obj;
//            return new Tuple<T>((T)array.value);
//        }

//    }


//    [Collection(typeof(Tuple<,>), true)]
//    public unsafe class CollectionArrayTuple<T1,T2> : CollectionArrayBase
//    {
//        TypeCode[] typeCode = new TypeCode[2];
//        public CollectionArrayTuple()
//        {
//            typeCode[0] = Type.GetTypeCode(typeof(T1));
//            typeCode[1] = Type.GetTypeCode(typeof(T2));
//        }

//        public override void Add(object obj, int index, object value)
//        {
//            object[] array = (object[])obj;
//            array[index] = value;
//        }

//        public override void AddValue(object obj, int index, char* str, JsonValue value)
//        {
//            object[] array = (object[])obj;
//            object set_value = null;

//            switch (value.type)
//            {
//                case JsonValueType.String:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Char:
//                            set_value = str[value.vStringStart];
//                            break;
//                        case TypeCode.String:
//                            set_value = new string(str, value.vStringStart, value.vStringLength);
//                            break;
//                    }
//                    break;
//                case JsonValueType.Long:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.SByte:
//                            set_value = (SByte)value.valueLong;
//                            break;
//                        case TypeCode.Byte:
//                            set_value = (Byte)value.valueLong;
//                            break;
//                        case TypeCode.Int16:
//                            set_value = (Int16)value.valueLong;
//                            break;
//                        case TypeCode.UInt16:
//                            set_value = (UInt16)value.valueLong;
//                            break;
//                        case TypeCode.Int32:
//                            set_value = (Int32)value.valueLong;
//                            break;
//                        case TypeCode.UInt32:
//                            set_value = (UInt32)value.valueLong;
//                            break;
//                        case TypeCode.Int64:
//                            set_value = value.valueLong;
//                            break;
//                        case TypeCode.UInt64:
//                            set_value = (UInt64)value.valueLong;
//                            break;
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueLong;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueLong;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueLong;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Double:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueDouble;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueDouble;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueDouble;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Boolean:
//                    set_value = value.valueBool;
//                    break;
//            }
//            array[index] = set_value;

//        }
       
        
//        public override object CreateArray(int arrayCount, Type arrayType, Type parentType)
//        {
//            return new object[2];
//        }

//        public override Type GetItemType(int index)
//        {
//            switch (index)
//            {
//                case 0:
//                    return typeof(T1);
//                default:
//                    return typeof(T2);
//            }

//        }

//        public override object End(object obj)
//        {
//            object[] array = (object[])obj;

//            return new Tuple<T1, T2>((T1)array[0], (T2)array[1]);
//        }

//    }


//    [Collection(typeof(Tuple<,,>), true)]
//    public unsafe class CollectionArrayTuple<T1, T2, T3> : CollectionArrayBase
//    {
//        TypeCode[] typeCode = new TypeCode[3];
//        public CollectionArrayTuple()
//        {
//            typeCode[0] = Type.GetTypeCode(typeof(T1));
//            typeCode[1] = Type.GetTypeCode(typeof(T2));
//            typeCode[2] = Type.GetTypeCode(typeof(T3));
//        }

//        public override void Add(object obj, int index, object value)
//        {
//            object[] array = (object[])obj;
//            array[index] = value;
//        }

//        public override void AddValue(object obj, int index, char* str, JsonValue value)
//        {
//            object[] array = (object[])obj;
//            object set_value = null;
         
//            switch (value.type)
//            {
//                case JsonValueType.String:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Char:
//                            set_value = str[value.vStringStart];
//                            break;
//                        case TypeCode.String:
//                            set_value = new string(str, value.vStringStart, value.vStringLength);
//                            break;
//                    }
//                    break;
//                case JsonValueType.Long:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.SByte:
//                            set_value = (SByte)value.valueLong;
//                            break;
//                        case TypeCode.Byte:
//                            set_value = (Byte)value.valueLong;
//                            break;
//                        case TypeCode.Int16:
//                            set_value = (Int16)value.valueLong;
//                            break;
//                        case TypeCode.UInt16:
//                            set_value = (UInt16)value.valueLong;
//                            break;
//                        case TypeCode.Int32:
//                            set_value = (Int32)value.valueLong;
//                            break;
//                        case TypeCode.UInt32:
//                            set_value = (UInt32)value.valueLong;
//                            break;
//                        case TypeCode.Int64:
//                            set_value = value.valueLong;
//                            break;
//                        case TypeCode.UInt64:
//                            set_value = (UInt64)value.valueLong;
//                            break;
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueLong;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueLong;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueLong;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Double:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueDouble;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueDouble;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueDouble;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Boolean:
//                    set_value = value.valueBool;
//                    break;
//            }
//            array[index] = set_value;
            
//        }

//        public override object CreateArray(int arrayCount, Type arrayType, Type parentType)
//        {
//            return new object[3];
//        }

//        public override Type GetItemType(int index)
//        {
//            switch (index)
//            {
//                case 0:
//                    return typeof(T1);
//                case 1:
//                    return typeof(T2);
//                default:
//                    return typeof(T3);
//            }

//        }

//        public override object End(object obj)
//        {
//            object[] array = (object[])obj;

//            return new Tuple<T1, T2, T3>((T1)array[0], (T2)array[1], (T3)array[2]);
//        }

//    }


//    [Collection(typeof(Tuple<,,,>), true)]
//    public unsafe class CollectionArrayTuple<T1, T2, T3, T4> : CollectionArrayBase
//    {
//        TypeCode[] typeCode = new TypeCode[4];
//        public CollectionArrayTuple()
//        {
//            typeCode[0] = Type.GetTypeCode(typeof(T1));
//            typeCode[1] = Type.GetTypeCode(typeof(T2));
//            typeCode[2] = Type.GetTypeCode(typeof(T3));
//            typeCode[3] = Type.GetTypeCode(typeof(T4));
//        }

//        public override void Add(object obj, int index, object value)
//        {
//            object[] array = (object[])obj;
//            array[index] = value;
//        }

//        public override void AddValue(object obj, int index, char* str, JsonValue value)
//        {
//            object[] array = (object[])obj;
//            object set_value = null;

//            switch (value.type)
//            {
//                case JsonValueType.String:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Char:
//                            set_value = str[value.vStringStart];
//                            break;
//                        case TypeCode.String:
//                            set_value = new string(str, value.vStringStart, value.vStringLength);
//                            break;
//                    }
//                    break;
//                case JsonValueType.Long:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.SByte:
//                            set_value = (SByte)value.valueLong;
//                            break;
//                        case TypeCode.Byte:
//                            set_value = (Byte)value.valueLong;
//                            break;
//                        case TypeCode.Int16:
//                            set_value = (Int16)value.valueLong;
//                            break;
//                        case TypeCode.UInt16:
//                            set_value = (UInt16)value.valueLong;
//                            break;
//                        case TypeCode.Int32:
//                            set_value = (Int32)value.valueLong;
//                            break;
//                        case TypeCode.UInt32:
//                            set_value = (UInt32)value.valueLong;
//                            break;
//                        case TypeCode.Int64:
//                            set_value = value.valueLong;
//                            break;
//                        case TypeCode.UInt64:
//                            set_value = (UInt64)value.valueLong;
//                            break;
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueLong;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueLong;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueLong;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Double:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueDouble;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueDouble;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueDouble;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Boolean:
//                    set_value = value.valueBool;
//                    break;
//            }
//            array[index] = set_value;
            
//        }

//        public override object CreateArray(int arrayCount, Type arrayType, Type parentType)
//        {
//            return new object[4];
//        }

//        public override Type GetItemType(int index)
//        {
//            switch (index)
//            {
//                case 0:
//                    return typeof(T1);
//                case 1:
//                    return typeof(T2);
//                case 2:
//                    return typeof(T3);
//                default:
//                    return typeof(T4);
//            }

//        }

//        public override object End(object obj)
//        {
//            object[] array = (object[])obj;

//            return new Tuple<T1, T2, T3, T4>((T1)array[0], (T2)array[1], (T3)array[2], (T4)array[3]);
//        }

//    }



//    [Collection(typeof(Tuple<,,,,>), true)]
//    public unsafe class CollectionArrayTuple<T1, T2, T3, T4, T5> : CollectionArrayBase
//    {
//        TypeCode[] typeCode = new TypeCode[5];
//        public CollectionArrayTuple()
//        {
//            typeCode[0] = Type.GetTypeCode(typeof(T1));
//            typeCode[1] = Type.GetTypeCode(typeof(T2));
//            typeCode[2] = Type.GetTypeCode(typeof(T3));
//            typeCode[3] = Type.GetTypeCode(typeof(T4));
//            typeCode[4] = Type.GetTypeCode(typeof(T5));
//        }

//        public override void Add(object obj, int index, object value)
//        {
//            object[] array = (object[])obj;
//            array[index] = value;
//        }

//        public override void AddValue(object obj, int index, char* str, JsonValue value)
//        {
//            object[] array = (object[])obj;
//            object set_value = null;

//            switch (value.type)
//            {
//                case JsonValueType.String:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Char:
//                            set_value = str[value.vStringStart];
//                            break;
//                        case TypeCode.String:
//                            set_value = new string(str, value.vStringStart, value.vStringLength);
//                            break;
//                    }
//                    break;
//                case JsonValueType.Long:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.SByte:
//                            set_value = (SByte)value.valueLong;
//                            break;
//                        case TypeCode.Byte:
//                            set_value = (Byte)value.valueLong;
//                            break;
//                        case TypeCode.Int16:
//                            set_value = (Int16)value.valueLong;
//                            break;
//                        case TypeCode.UInt16:
//                            set_value = (UInt16)value.valueLong;
//                            break;
//                        case TypeCode.Int32:
//                            set_value = (Int32)value.valueLong;
//                            break;
//                        case TypeCode.UInt32:
//                            set_value = (UInt32)value.valueLong;
//                            break;
//                        case TypeCode.Int64:
//                            set_value = value.valueLong;
//                            break;
//                        case TypeCode.UInt64:
//                            set_value = (UInt64)value.valueLong;
//                            break;
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueLong;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueLong;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueLong;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Double:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueDouble;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueDouble;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueDouble;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Boolean:
//                    set_value = value.valueBool;
//                    break;
//            }
//            array[index] = set_value;
            
//        }

//        public override object CreateArray(int arrayCount, Type arrayType, Type parentType)
//        {
//            return new object[5];
//        }

//        public override Type GetItemType(int index)
//        {
//            switch (index)
//            {
//                case 0:
//                    return typeof(T1);
//                case 1:
//                    return typeof(T2);
//                case 2:
//                    return typeof(T3);
//                case 3:
//                    return typeof(T4);
//                default:
//                    return typeof(T5);
//            }

//        }

//        public override object End(object obj)
//        {
//            object[] array = (object[])obj;

//            return new Tuple<T1, T2, T3, T4, T5>((T1)array[0], (T2)array[1], (T3)array[2], (T4)array[3], (T5)array[4]);
//        }

//    }


//    [Collection(typeof(Tuple<,,,,,>), true)]
//    public unsafe class CollectionArrayTuple<T1, T2, T3, T4, T5, T6> : CollectionArrayBase
//    {
//        TypeCode[] typeCode = new TypeCode[6];
//        public CollectionArrayTuple()
//        {
//            typeCode[0] = Type.GetTypeCode(typeof(T1));
//            typeCode[1] = Type.GetTypeCode(typeof(T2));
//            typeCode[2] = Type.GetTypeCode(typeof(T3));
//            typeCode[3] = Type.GetTypeCode(typeof(T4));
//            typeCode[4] = Type.GetTypeCode(typeof(T5));
//            typeCode[5] = Type.GetTypeCode(typeof(T6));
//        }

//        public override void Add(object obj, int index, object value)
//        {
//            object[] array = (object[])obj;
//            array[index] = value;
//        }

//        public override void AddValue(object obj, int index, char* str, JsonValue value)
//        {
//            object[] array = (object[])obj;
//            object set_value = null;

//            switch (value.type)
//            {
//                case JsonValueType.String:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Char:
//                            set_value = str[value.vStringStart];
//                            break;
//                        case TypeCode.String:
//                            set_value = new string(str, value.vStringStart, value.vStringLength);
//                            break;
//                    }
//                    break;
//                case JsonValueType.Long:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.SByte:
//                            set_value = (SByte)value.valueLong;
//                            break;
//                        case TypeCode.Byte:
//                            set_value = (Byte)value.valueLong;
//                            break;
//                        case TypeCode.Int16:
//                            set_value = (Int16)value.valueLong;
//                            break;
//                        case TypeCode.UInt16:
//                            set_value = (UInt16)value.valueLong;
//                            break;
//                        case TypeCode.Int32:
//                            set_value = (Int32)value.valueLong;
//                            break;
//                        case TypeCode.UInt32:
//                            set_value = (UInt32)value.valueLong;
//                            break;
//                        case TypeCode.Int64:
//                            set_value = value.valueLong;
//                            break;
//                        case TypeCode.UInt64:
//                            set_value = (UInt64)value.valueLong;
//                            break;
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueLong;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueLong;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueLong;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Double:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueDouble;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueDouble;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueDouble;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Boolean:
//                    set_value = value.valueBool;
//                    break;
//            }
//            array[index] = set_value;
            
//        }

//        public override object CreateArray(int arrayCount, Type arrayType, Type parentType)
//        {
//            return new object[6];
//        }

//        public override Type GetItemType(int index)
//        {
//            switch (index)
//            {
//                case 0:
//                    return typeof(T1);
//                case 1:
//                    return typeof(T2);
//                case 2:
//                    return typeof(T3);
//                case 3:
//                    return typeof(T4);
//                case 4:
//                    return typeof(T5);
//                default:
//                    return typeof(T6);
//            }

//        }

//        public override object End(object obj)
//        {
//            object[] array = (object[])obj;

//            return new Tuple<T1, T2, T3, T4, T5, T6>((T1)array[0], (T2)array[1], (T3)array[2], (T4)array[3], (T5)array[4], (T6)array[5]);
//        }

//    }


//    [Collection(typeof(Tuple<,,,,,,>), true)]
//    public unsafe class CollectionArrayTuple<T1, T2, T3, T4, T5, T6, T7> : CollectionArrayBase
//    {
//        TypeCode[] typeCode = new TypeCode[7];
//        public CollectionArrayTuple()
//        {
//            typeCode[0] = Type.GetTypeCode(typeof(T1));
//            typeCode[1] = Type.GetTypeCode(typeof(T2));
//            typeCode[2] = Type.GetTypeCode(typeof(T3));
//            typeCode[3] = Type.GetTypeCode(typeof(T4));
//            typeCode[4] = Type.GetTypeCode(typeof(T5));
//            typeCode[5] = Type.GetTypeCode(typeof(T6));
//            typeCode[6] = Type.GetTypeCode(typeof(T7));
//        }

//        public override void Add(object obj, int index, object value)
//        {
//            object[] array = (object[])obj;
//            array[index] = value;
//        }

//        public override void AddValue(object obj, int index, char* str, JsonValue value)
//        {
//            object[] array = (object[])obj;
//            object set_value = null;

//            switch (value.type)
//            {
//                case JsonValueType.String:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Char:
//                            set_value = str[value.vStringStart];
//                            break;
//                        case TypeCode.String:
//                            set_value = new string(str, value.vStringStart, value.vStringLength);
//                            break;
//                    }
//                    break;
//                case JsonValueType.Long:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.SByte:
//                            set_value = (SByte)value.valueLong;
//                            break;
//                        case TypeCode.Byte:
//                            set_value = (Byte)value.valueLong;
//                            break;
//                        case TypeCode.Int16:
//                            set_value = (Int16)value.valueLong;
//                            break;
//                        case TypeCode.UInt16:
//                            set_value = (UInt16)value.valueLong;
//                            break;
//                        case TypeCode.Int32:
//                            set_value = (Int32)value.valueLong;
//                            break;
//                        case TypeCode.UInt32:
//                            set_value = (UInt32)value.valueLong;
//                            break;
//                        case TypeCode.Int64:
//                            set_value = value.valueLong;
//                            break;
//                        case TypeCode.UInt64:
//                            set_value = (UInt64)value.valueLong;
//                            break;
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueLong;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueLong;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueLong;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Double:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueDouble;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueDouble;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueDouble;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Boolean:
//                    set_value = value.valueBool;
//                    break;
//            }
//            array[index] = set_value;

//        }

//        public override object CreateArray(int arrayCount, Type arrayType, Type parentType)
//        {
//            return new object[7];
//        }

//        public override Type GetItemType(int index)
//        {
//            switch (index)
//            {
//                case 0:
//                    return typeof(T1);
//                case 1:
//                    return typeof(T2);
//                case 2:
//                    return typeof(T3);
//                case 3:
//                    return typeof(T4);
//                case 4:
//                    return typeof(T5);
//                case 5:
//                    return typeof(T6);
//                default:
//                    return typeof(T7);
//            }

//        }

//        public override object End(object obj)
//        {
//            object[] array = (object[])obj;

//            return new Tuple<T1, T2, T3, T4, T5, T6, T7>((T1)array[0], (T2)array[1], (T3)array[2], (T4)array[3], (T5)array[4], (T6)array[5], (T7)array[6]);
//        }

//    }


//    [Collection(typeof(Tuple<,,,,,,,>), true)]
//    public unsafe class CollectionArrayTuple<T1, T2, T3, T4, T5, T6, T7, TRest> : CollectionArrayBase
//    {
//        TypeCode[] typeCode = new TypeCode[8];
//        public CollectionArrayTuple()
//        {
//            typeCode[0] = Type.GetTypeCode(typeof(T1));
//            typeCode[1] = Type.GetTypeCode(typeof(T2));
//            typeCode[2] = Type.GetTypeCode(typeof(T3));
//            typeCode[3] = Type.GetTypeCode(typeof(T4));
//            typeCode[4] = Type.GetTypeCode(typeof(T5));
//            typeCode[5] = Type.GetTypeCode(typeof(T6));
//            typeCode[6] = Type.GetTypeCode(typeof(T7));
//            typeCode[7] = Type.GetTypeCode(typeof(TRest));
//        }

//        public override void Add(object obj, int index, object value)
//        {
//            object[] array = (object[])obj;
//            array[index] = value;
//        }

//        public override void AddValue(object obj, int index, char* str, JsonValue value)
//        {
//            object[] array = (object[])obj;
//            object set_value = null;

//            switch (value.type)
//            {
//                case JsonValueType.String:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Char:
//                            set_value = str[value.vStringStart];
//                            break;
//                        case TypeCode.String:
//                            set_value = new string(str, value.vStringStart, value.vStringLength);
//                            break;
//                    }
//                    break;
//                case JsonValueType.Long:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.SByte:
//                            set_value = (SByte)value.valueLong;
//                            break;
//                        case TypeCode.Byte:
//                            set_value = (Byte)value.valueLong;
//                            break;
//                        case TypeCode.Int16:
//                            set_value = (Int16)value.valueLong;
//                            break;
//                        case TypeCode.UInt16:
//                            set_value = (UInt16)value.valueLong;
//                            break;
//                        case TypeCode.Int32:
//                            set_value = (Int32)value.valueLong;
//                            break;
//                        case TypeCode.UInt32:
//                            set_value = (UInt32)value.valueLong;
//                            break;
//                        case TypeCode.Int64:
//                            set_value = value.valueLong;
//                            break;
//                        case TypeCode.UInt64:
//                            set_value = (UInt64)value.valueLong;
//                            break;
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueLong;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueLong;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueLong;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Double:
//                    switch (typeCode[index])
//                    {
//                        case TypeCode.Single:
//                            set_value = (Single)value.valueDouble;
//                            break;
//                        case TypeCode.Double:
//                            set_value = (Double)value.valueDouble;
//                            break;
//                        case TypeCode.Decimal:
//                            set_value = (Decimal)value.valueDouble;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Boolean:
//                    set_value = value.valueBool;
//                    break;
//            }
//            array[index] = set_value;
//        }

//        public override object CreateArray(int arrayCount, Type arrayType, Type parentType)
//        {
//            return new object[8];
//        }

//        public override Type GetItemType(int index)
//        {
//            switch (index)
//            {
//                case 0:
//                    return typeof(T1);
//                case 1:
//                    return typeof(T2);
//                case 2:
//                    return typeof(T3);
//                case 3:
//                    return typeof(T4);
//                case 4:
//                    return typeof(T5);
//                case 5:
//                    return typeof(T6);
//                case 6:
//                    return typeof(T7);
//                default:
//                    return typeof(TRest); 
//            }

//        }

//        public override object End(object obj)
//        {
//            object[] array = (object[])obj;

//            return new Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>((T1)array[0], (T2)array[1], (T3)array[2], (T4)array[3], (T5)array[4], (T6)array[5], (T7)array[6], (TRest)array[7]);
//        }

//    }


//}
