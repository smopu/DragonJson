using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson.Collection.ArrayCollection
{

    [ReadCollection(typeof(EnumWrapper<>), true)]
    public unsafe class EnumWrapCollection<T> : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        TypeCode typeCode;
        EnumTypeWrap enumTypeWrap;
        public EnumWrapCollection()
        {
            enumTypeWrap = CollectionManager.GetEnumTypeWrap(typeof(T));
            typeCode = Type.GetTypeCode(typeof(T));
        }
        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = false;

            //read.addObjectClassDelegate = (Action<Box<T>, object, ReadCollectionLink.Add_Args>)AddObjectClass;
            //read.addObjectStructDelegate = (AddObjecStruct_)AddObjecStruct;

            //read.addValueClassDelegate = (Action<Box<T>, ReadCollectionLink.AddValue_Args>)AddValueClass;
            switch (typeCode)
            {
                case TypeCode.SByte:
            read.addValueStructDelegate = (AddValueStruct_<sbyte>)AddValueStruct; 
            read.addValueClassDelegate = (AddValueClass_<sbyte>)AddValueClass; 
                    break;
                case TypeCode.Byte:
            read.addValueStructDelegate = (AddValueStruct_<byte>)AddValueStruct;
            read.addValueClassDelegate = (AddValueClass_<byte>)AddValueClass; 
                    break;
                case TypeCode.Int16:
            read.addValueStructDelegate = (AddValueStruct_<short>)AddValueStruct;
            read.addValueClassDelegate = (AddValueClass_<short>)AddValueClass; 
                    break;
                case TypeCode.UInt16:
            read.addValueStructDelegate = (AddValueStruct_<ushort>)AddValueStruct;
            read.addValueClassDelegate = (AddValueClass_<ushort>)AddValueClass; 
                    break;
                case TypeCode.Int32:
            read.addValueStructDelegate = (AddValueStruct_<int>)AddValueStruct;
            read.addValueClassDelegate = (AddValueClass_<int>)AddValueClass; 
                    break;
                case TypeCode.UInt32:
            read.addValueStructDelegate = (AddValueStruct_<uint>)AddValueStruct;
            read.addValueClassDelegate = (AddValueClass_<uint>)AddValueClass; 
                    break;
                case TypeCode.Int64:
            read.addValueStructDelegate = (AddValueStruct_<long>)AddValueStruct;
            read.addValueClassDelegate = (AddValueClass_<long>)AddValueClass; 
                    break;
                case TypeCode.UInt64:
            read.addValueStructDelegate = (AddValueStruct_<ulong>)AddValueStruct;
            read.addValueClassDelegate = (AddValueClass_<ulong>)AddValueClass; 
                    break;
            }



            read.createStructDelegate = (CeateValue_)CeateValue;
            read.createClassDelegate = (CreateObject_)CreateObject;

            read.getItemType = GetItemType;
            return read;
        }

        delegate void AddValueClass_<TypeInt>(Box<TypeInt> obj, ReadCollectionLink.AddValue_Args arg);

        void AddValueClass(Box<sbyte> obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj.value |= (sbyte)vc;
        }
        void AddValueClass(Box<byte> obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj.value |= (byte)vc;
        }
        void AddValueClass(Box<short> obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj.value |= (short)vc;
        }
        void AddValueClass(Box<ushort> obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj.value |= (ushort)vc;
        }
        void AddValueClass(Box<int> obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj.value |= (int)vc;
        }
        void AddValueClass(Box<uint> obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj.value |= (uint)vc;
        }
        void AddValueClass(Box<long> obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj.value |= (long)vc;
        }
        void AddValueClass(Box<ulong> obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj.value |= (ulong)vc;
        }


        delegate void AddValueStruct_<TypeInt>(ref TypeInt obj, ReadCollectionLink.AddValue_Args arg);
        void AddValueStruct(ref sbyte obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj |= (sbyte)vc;
        }
        void AddValueStruct(ref byte obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj |= (byte)vc;
        }
        void AddValueStruct(ref short obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj |= (short)vc;
        }
        void AddValueStruct(ref ushort obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj |= (ushort)vc;
        }
        void AddValueStruct(ref int obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj |= (int)vc;
        }
        void AddValueStruct(ref uint obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj |= (uint)vc;
        }
        void AddValueStruct(ref long obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj |= (long)vc;
        }
        void AddValueStruct(ref ulong obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];
            obj |= (ulong)vc;
        }


        delegate void CeateValue_(ref T obj, out object temp, ReadCollectionLink.Create_Args arg);
        void CeateValue(ref T obj, out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            obj = default(T);
        }

        delegate object CreateObject_(out IBox temp, ReadCollectionLink.Create_Args arg);
        object CreateObject(out IBox temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            switch (typeCode)
            {
                case TypeCode.SByte:
                    temp = new Box<SByte>();
                    break;
                case TypeCode.Byte:
                    temp = new Box<Byte>();
                    break;
                case TypeCode.Int16:
                    temp = new Box<Int16>();
                    break;
                case TypeCode.UInt16:
                    temp = new Box<UInt16>();
                    break;
                case TypeCode.Int32:
                    temp = new Box<Int32>();
                    break;
                case TypeCode.UInt32:
                    temp = new Box<UInt32>();
                    break;
                case TypeCode.Int64:
                    temp = new Box<Int64>();
                    break;
                case TypeCode.UInt64:
                    temp = new Box<UInt64>();
                    break;
            }
            object obj = temp.GetObject();
            temp = null;
            return obj;
        }

        CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            if (collection == null)
            {
                collection = CollectionManager.GetTypeCollection(typeof(Box<T>));
            }
            return collection;
        }

    }



}
