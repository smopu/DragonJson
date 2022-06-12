using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace DogJson
{
    public class JsonReflection
    {

        static Dictionary<Type, TypeWarp> allGenericType = new Dictionary<Type, TypeWarp>();
        static Dictionary<Type, TypeReflectionWarp> allTypeStructWarp = new Dictionary<Type, TypeReflectionWarp>();

        /// <summary>
        /// 设置整数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="typeCode"></param>
        /// <param name="field"></param>
        public unsafe static void SetvalueInt(long value, TypeCode typeCode, FieldInfo field, object data)
        {
            //int* field = root + offset;
            switch (typeCode)
            {
                case TypeCode.Char:
                    field.SetValue(data, (char)value);
                    break;
                case TypeCode.SByte:
                    field.SetValue(data, (sbyte)value);
                    break;
                case TypeCode.Byte:
                    field.SetValue(data, (byte)value);
                    break;
                case TypeCode.Int16:
                    field.SetValue(data, (short)value);
                    break;
                case TypeCode.UInt16:
                    field.SetValue(data, (ushort)value);
                    break;
                case TypeCode.Int32:
                    field.SetValue(data, (int)value);
                    break;
                case TypeCode.UInt32:
                    field.SetValue(data, (uint)value);
                    break;
                case TypeCode.Int64:
                    field.SetValue(data, value);
                    break;
                case TypeCode.UInt64:
                    field.SetValue(data, (ulong)value);
                    break;
                case TypeCode.Decimal:
                    field.SetValue(data, (decimal)value);
                    break;
            }
        }

        /// <summary>
        /// 设置浮点数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="typeCode"></param>
        /// <param name="field"></param> 
        public unsafe static void SetvalueFloat(double value, TypeCode typeCode, FieldInfo field, object data)
        {
            //int* field = root + offset;
            switch (typeCode)
            {
                case TypeCode.Single:
                    field.SetValue(data, (float)value);
                    break;
                case TypeCode.Double:
                    field.SetValue(data, value);
                    break;
                case TypeCode.Decimal:
                    field.SetValue(data, (decimal)value);
                    break;
            }
        }

        static unsafe TypeReflectionWarp AddTypeStructWarp(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            TypeReflectionWarp typeStructWarp = new TypeReflectionWarp();
            foreach (var field in fieldInfos)
            {
                //IntPtr ix = Marshal.OffsetOf(type, field.Name);
                TypeField typeStructField = new TypeField();
                typeStructField.field = field;
                //typeStructField.offset = (int)ix;
                //if (typeStructField.fieldType.IsPrimitive)
                {
                    TypeCode typeCode = Type.GetTypeCode(field.FieldType);
                    typeStructField.typeCode = typeCode;
                    switch (typeCode)
                    {
                        case TypeCode.Boolean:
                            typeStructField.jsonType = JsonValueType.Boolean;
                            break;
                        case TypeCode.SByte:
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Decimal:
                            typeStructField.jsonType = JsonValueType.Long;
                            break;
                        case TypeCode.Single:
                        case TypeCode.Double:
                            typeStructField.jsonType = JsonValueType.Double;
                            break;
                        case TypeCode.DateTime:
                            break;
                        case TypeCode.String:
                        case TypeCode.Char:
                            typeStructField.jsonType = JsonValueType.String;
                            break;
                        default:
                            break;
                    }
                }

                typeStructWarp.nameOfField[field.Name] = typeStructField;
            }
            allTypeStructWarp[type] = typeStructWarp;
            return typeStructWarp;
        }
        public static unsafe TypeReflectionWarp GetTypeStructWarp(Type type)
        {
            TypeReflectionWarp typeStructWarp;
            if (!allTypeStructWarp.TryGetValue(type, out typeStructWarp))
            {
                typeStructWarp = AddTypeStructWarp(type);
                allTypeStructWarp[type] = typeStructWarp;
            }
            return typeStructWarp;
        }

        public static unsafe TypeReflectionWarp GetArrayTypeStructWarp(Type type)
        {
            TypeReflectionWarp typeStructWarp;
            if (!allTypeStructWarp.TryGetValue(type, out typeStructWarp))
            {
                typeStructWarp = new TypeReflectionWarp();
                if (type.IsArray)
                {
                    typeStructWarp.itemType = type.GetElementType();
                    typeStructWarp.itemTypeCode = Type.GetTypeCode(typeStructWarp.itemType);

                }
                else if (type.IsGenericType)
                {
                    typeStructWarp.itemType = type.GetGenericArguments()[0];
                    typeStructWarp.itemTypeCode = Type.GetTypeCode(typeStructWarp.itemType);
                }
            }
            return typeStructWarp;
        }



    }
    public class TypeWarp
    {
        public bool isObject;
        //public ReadArrayObject readArray;
        public Type target;
        public Type readArrayType;

    }

    public class TypeReflectionWarp
    {
        public TypeCode itemTypeCode;
        public Type itemType;

        public Dictionary<string, TypeField> nameOfField = new Dictionary<string, TypeField>();
        public unsafe void SetTypeStructWarpLong(object obj, string key, long v)
        {
            TypeField typeStructField;
            if (nameOfField.TryGetValue(key, out typeStructField))
            {
                JsonReflection.SetvalueInt(v, typeStructField.typeCode, typeStructField.field, obj);
            }
        }

        public unsafe void SetTypeStructWarpDouble(object obj, string key, double v)
        {
            TypeField typeStructField;
            if (nameOfField.TryGetValue(key, out typeStructField))
            {
                JsonReflection.SetvalueFloat(v, typeStructField.typeCode, typeStructField.field, obj);
            }
        }

        public unsafe void SetTypeStructWarpString(object obj, string key, string v)
        {
            TypeField typeStructField;
            if (nameOfField.TryGetValue(key, out typeStructField))
            {
                typeStructField.field.SetValue(obj, v);
            }
        }

        public unsafe void SetTypeStructWarpBool(object obj, string key, bool v)
        {
            TypeField typeStructField;
            if (nameOfField.TryGetValue(key, out typeStructField))
            {
                typeStructField.field.SetValue(obj, v);
            }
        }

        public unsafe void AddTypeStructWarpLong(IList obj, long value)
        {
            switch (itemTypeCode)
            {
                case TypeCode.Char:
                    obj.Add((char)value);
                    break;
                case TypeCode.SByte:
                    obj.Add((sbyte)value);
                    break;
                case TypeCode.Byte:
                    obj.Add((byte)value);
                    break;
                case TypeCode.Int16:
                    obj.Add((short)value);
                    break;
                case TypeCode.UInt16:
                    obj.Add((ushort)value);
                    break;
                case TypeCode.Int32:
                    obj.Add((int)value);
                    break;
                case TypeCode.UInt32:
                    obj.Add((uint)value);
                    break;
                case TypeCode.Int64:
                    obj.Add(value);
                    break;
                case TypeCode.UInt64:
                    obj.Add((ulong)value);
                    break;
                case TypeCode.Decimal:
                    obj.Add((decimal)value);
                    break;
            }
        }

        public unsafe void AddTypeStructWarpDouble(IList obj, double value)
        {
            switch (itemTypeCode)
            {
                case TypeCode.Single:
                    obj.Add((float)value);
                    break;
                case TypeCode.Double:
                    obj.Add(value);
                    break;
                case TypeCode.Decimal:
                    obj.Add((decimal)value);
                    break;
            }
        }



    }


    public class TypeField
    {
        ///// <summary>
        /////  class struct
        ///// </summary>
        //public bool isClass = false;

        /// <summary>
        ///  object array
        /// </summary>
        public bool isArray = false;

        public FieldInfo field;

        //public Type fieldType;
        //public int offset;
        public JsonValueType jsonType = JsonValueType.None;
        public TypeCode typeCode;
    }
}
