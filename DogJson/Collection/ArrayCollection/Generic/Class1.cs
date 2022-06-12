//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DogJson
//{

//    //[Collection(typeof(Array))]
//    public unsafe class CollectionRankArray : CollectionArrayBase
//    {
//        TypeCode typeCode;
//        public CollectionRankArray()
//        {
//            typeCode = Type.GetTypeCode(typeof(T));
//        }

//        public override void Add(object obj, int index, object value)
//        {
//            List<T> array = (List<T>)obj;
//            //array.Insert(index, (T)value);
//            array.Add((T)value);
//        }

//        public override object CreateArray(int arrayCount, Type arrayType, Type parentType)
//        {
//            return new List<T>(arrayCount);
//        }

//        public override Type ItemType(int index)
//        {
//            return typeof(T);
//        }
//    }



//    public unsafe class RankArray
//    {
        
//    }
        
//    public unsafe class CollectionLastRankArray<T> : CollectionArrayBase
//    {
//        TypeCode typeCode;
//        public CollectionLastRankArray()
//        {
//            typeCode = Type.GetTypeCode(typeof(T));
//        }

//        public override void Add(object obj, int index, object value)
//        {
//            List<T> array = (List<T>)obj;
//            //array.Insert(index, (T)value);
//            array.Add((T)value);
//        }

//        public override void AddValue(object obj, int index, char* str, JsonValue value)
//        {
//            List<T> array = (List<T>)obj;
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
//            array.Add((T)set_value);
//            //array.Insert(index , (T)set_value);
//        }

//        public override object CreateArray(int arrayCount, Type arrayType, Type parentType)
//        {
//            return new List<T>(arrayCount);
//        }

//        public override Type ItemType(int index)
//        {
//            return typeof(T);
//        }
//    }

//}
