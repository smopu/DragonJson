using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    public unsafe class TypeAddrReflectionWarp
    {
        public unsafe TypeAddrReflectionWarp(Type type)
        {
            isValueType = type.IsValueType;
            Dictionary<string, TypeAddrField> nameOfField = new Dictionary<string, TypeAddrField>();
            FieldInfo[] TypeAddrFieldsNow = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var item in TypeAddrFieldsNow)
            {
                if (nameOfField.ContainsKey(item.Name))
                {
                    nameOfField[item.Name] = new TypeAddrField(type.GetField(item.Name,
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                }
                else
                {
                    nameOfField[item.Name] = new TypeAddrField(item);
                }
            }

            var loopType = type;
            while (loopType.BaseType != typeof(object))
            {
                foreach (var item in loopType.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (item.Attributes == FieldAttributes.Private)
                    {
                        if (!nameOfField.ContainsKey(item.Name))
                        {
                            nameOfField[item.Name] = new TypeAddrField(item);
                        }
                    }
                }
                loopType = loopType.BaseType;
            }

            this.allTypeField = new TypeAddrField[nameOfField.Count];
            Dictionary<string, int> strs = new Dictionary<string, int>();
            int indexNow = 0;
            foreach (var item in nameOfField)
            {
                this.allTypeField[indexNow] = item.Value;
                strs[item.Key] = indexNow;
                indexNow++;
            }
            this.nameOfField = nameOfField;
            this.stringTable = new StringTable(strs);


            this.type = type;
            this.heapSize = UnsafeOperation.SizeOf(type);


            this.sizeByte_1 = this.heapSize - UnsafeOperation.PTR_COUNT * 3;//this.heapSize / PTR_COUNT - 1;
            long dfffV2 = (long)type.TypeHandle.Value;

            this.typeHead = UnsafeOperation.GetTypeHead(type);

        }


        public Dictionary<string, TypeAddrField> nameOfField;
        StringTable stringTable;
        TypeAddrField[] allTypeField;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TypeAddrField Find(char* d, int length) 
        {
            int id =  stringTable.Find(d, length);
            if (id < 0)
            {
                return null; 
            }
            return allTypeField[id];
        }

        /// <summary>
        ///  class struct
        /// </summary>
        public bool isValueType = false;
        public Type type;
        public int heapSize;
        private int sizeByte_1;
        public IntPtr typeHead;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(out GCHandle gcHandle)
        {
            object obj = new byte[sizeByte_1];
            //gcHandle = new GCHandle();
            //if (isValueType)
            //{
            //    ptr = UnsafeUtility.GetValueAddr(obj);
            //}
            //else
            //{
            //}
            gcHandle = GCHandle.Alloc(obj, GCHandleType.Pinned);
            *(IntPtr*)GeneralTool.ObjectToVoid(obj) = typeHead;
            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create()
        {
            object obj = new byte[sizeByte_1];
            //IntPtr ptr;
            //ptr = UnsafeUtility.GetObjectAddr(obj);
            //*(IntPtr*)ptr = typeHead;
            *(IntPtr*)GeneralTool.ObjectToVoid(obj) = typeHead;

            return obj;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(object source, char* name, int length, object value)
        {
            int index = stringTable.Find(name, length);
            if (index >= 0)
            {
                allTypeField[index].SetValue(source, value);
            }
        }



    }

    public unsafe class TypeAddrField
    {
        /// <summary>
        ///  class struct
        /// </summary>
        public bool isValueType = false;
        public TypeAddrField(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
            this.fieldType = fieldInfo.FieldType;
            offset = UnsafeOperation.GetFeildOffset(fieldInfo);
            typeCode = Type.GetTypeCode(fieldType);
            isValueType = fieldType.IsValueType;
            isArray = fieldType.IsArray;
            isEnum = fieldType.IsEnum;
            if (isValueType)
            {
                stackSize = UnsafeOperation.SizeOf(fieldType);
            }
            else
            {
                stackSize = UnsafeOperation.PTR_COUNT;
            }
            typeHead = UnsafeOperation.GetTypeHead(fieldType);
        }

        public bool isArray = false;
        public bool isEnum = false;

        public FieldInfo fieldInfo;
        public IntPtr typeHead;
        public Type fieldType;
        public int offset;
        public int stackSize;
        public TypeCode typeCode;
        //public TypeAddrReflectionWarp warp;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetStruct(void* destination, void* value, int size)
        {
            value = ((IntPtr*)value) + 1;
            size -= UnsafeOperation.PTR_COUNT * 2;
            byte* byteV = (byte*)value;
            byte* byteD = (byte*)destination;
            for (int i = 0; i < size; i++)
            {
                *byteD = *byteV;
                ++byteD;
                ++byteV;
            }
            //UnsafeUtility.MemCpy(destination, value, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetValue(object source, object value)
        {
            TypedReference tf = __makeref(source);
            void* v = GeneralTool.ObjectToVoid(source);

            void* field = (byte*)v + UnsafeOperation.PTR_COUNT + this.offset;

            if (this.isValueType)
            {
                switch (this.typeCode)
                {
                    case TypeCode.Boolean:
                        *(bool*)field = (bool)value;
                        break;
                    case TypeCode.Byte:
                        *(Byte*)field = (Byte)value;
                        break;
                    case TypeCode.Char:
                        *(Char*)field = (Char)value;
                        break;
                    case TypeCode.DateTime:
                        *(DateTime*)field = (DateTime)value;
                        break;
                    case TypeCode.DBNull:
                        GeneralTool.SetObject(field, value);
                        break;
                    case TypeCode.Decimal:
                        *(Decimal*)field = (Decimal)value;
                        break;
                    case TypeCode.Double:
                        *(Double*)field = (Double)value;
                        break;
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Int16:
                        *(Int16*)field = (Int16)value;
                        break;
                    case TypeCode.Int32:
                        *(Int32*)field = (Int32)value;
                        break;
                    case TypeCode.Int64:
                        *(Int64*)field = (Int64)value;
                        break;
                    case TypeCode.Object:
                        tf = __makeref(value);
                        SetStruct(field, (void*)(*(IntPtr*)*((IntPtr*)&tf + 1)), this.stackSize);

                        break;
                    case TypeCode.SByte:
                        *(SByte*)field = (SByte)value;
                        break;
                    case TypeCode.Single:
                        *(Single*)field = (Single)value;
                        break;
                    case TypeCode.String:
                        GeneralTool.SetObject(field, value);
                        break;
                    case TypeCode.UInt16:
                        *(UInt16*)field = (UInt16)value;
                        break;
                    case TypeCode.UInt32:
                        *(UInt32*)field = (UInt32)value;
                        break;
                    case TypeCode.UInt64:
                        *(UInt64*)field = (UInt64)value;
                        break;
                    default:
                        tf = __makeref(value);
                        SetStruct(field, (void*)(*(IntPtr*)*((IntPtr*)&tf + 1)), this.stackSize);
                        break;
                }
            }
            else
            {
                GeneralTool.SetObject(field, value);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void StaticSetValue(void* field, object value)
        {
            Type type = value.GetType();
            if (type.IsValueType)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        *(bool*)field = (bool)value;
                        break;
                    case TypeCode.Byte:
                        *(Byte*)field = (Byte)value;
                        break;
                    case TypeCode.Char:
                        *(Char*)field = (Char)value;
                        break;
                    case TypeCode.DateTime:
                        *(DateTime*)field = (DateTime)value;
                        break;
                    case TypeCode.DBNull:
                        GeneralTool.SetObject(field, value);
                        break;
                    case TypeCode.Decimal:
                        *(Decimal*)field = (Decimal)value;
                        break;
                    case TypeCode.Double:
                        *(Double*)field = (Double)value;
                        break;
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Int16:
                        *(Int16*)field = (Int16)value;
                        break;
                    case TypeCode.Int32:
                        *(Int32*)field = (Int32)value;
                        break;
                    case TypeCode.Int64:
                        *(Int64*)field = (Int64)value;
                        break;
                    case TypeCode.Object:
                        SetStruct(field, GeneralTool.ObjectToVoid(value), UnsafeOperation.SizeOf(type));
                        break;
                    case TypeCode.SByte:
                        *(SByte*)field = (SByte)value;
                        break;
                    case TypeCode.Single:
                        *(Single*)field = (Single)value;
                        break;
                    case TypeCode.String:
                        GeneralTool.SetObject(field, value);
                        break;
                    case TypeCode.UInt16:
                        *(UInt16*)field = (UInt16)value;
                        break;
                    case TypeCode.UInt32:
                        *(UInt32*)field = (UInt32)value;
                        break;
                    case TypeCode.UInt64:
                        *(UInt64*)field = (UInt64)value;
                        break;
                    default:
                        SetStruct(field, GeneralTool.ObjectToVoid(value), UnsafeOperation.SizeOf(type));
                        break;
                }
            }
            else
            {
                GeneralTool.SetObject(field, value);
            }
        }



    }




}
