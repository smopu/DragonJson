using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson.Collection.ArrayCollection
{
    public class EnumWrap<T>
    {
        public EnumWrap(T inEnum)
        {
            this.inEnum = inEnum;
        }
        public T inEnum;
    }

    [ReadCollection(typeof(EnumWrap<>), true)]
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
                    break;
                case TypeCode.Byte:
            read.addValueStructDelegate = (AddValueStruct_<byte>)AddValueStruct;
                    break;
                case TypeCode.Int16:
            read.addValueStructDelegate = (AddValueStruct_<short>)AddValueStruct;
                    break;
                case TypeCode.UInt16:
            read.addValueStructDelegate = (AddValueStruct_<ushort>)AddValueStruct;
                    break;
                case TypeCode.Int32:
            read.addValueStructDelegate = (AddValueStruct_<int>)AddValueStruct;
                    break;
                case TypeCode.UInt32:
            read.addValueStructDelegate = (AddValueStruct_<uint>)AddValueStruct;
                    break;
                case TypeCode.Int64:
            read.addValueStructDelegate = (AddValueStruct_<long>)AddValueStruct;
                    break;
                case TypeCode.UInt64:
            read.addValueStructDelegate = (AddValueStruct_<ulong>)AddValueStruct;
                    break;
            }

            read.createStructDelegate = (CeateValue_)CeateValue;
            read.createObject = CreateObject;

            //read.getItemType = GetItemType;
            return read;
        }


        void AddObjectClass(Box<T> obj, object value, ReadCollectionLink.Add_Args arg)
        {
            string keystring = new string(arg.bridge->keyStringStart, 0, arg.bridge->keyStringLength);
        }

        delegate void AddObjecStruct_(ref T obj, object value, ReadCollectionLink.Add_Args arg);
        void AddObjecStruct(ref T obj, object value, ReadCollectionLink.Add_Args arg)
        {
            string keystring = new string(arg.bridge->keyStringStart, 0, arg.bridge->keyStringLength);
        }


        void AddValueClass(Box<T> obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->valueStringStart, arg.value->valueStringLength);
            long vc = enumTypeWrap.nameToValue[keystring];

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

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            return new Box<T>();
        }


        CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            return null;
        }

    }



}
