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
    public unsafe class TypeAddrReflectionWrapper
    {
        public IntPtr byteArrayHead = default(IntPtr);
        public static Dictionary<string, FieldInfo> GetAllFieldInfo(Type type) 
        {
            Dictionary<string, FieldInfo> nameOfField = new Dictionary<string, FieldInfo>();
            FieldInfo[] typeAddrFieldsNow = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var item in typeAddrFieldsNow)
            {
                if (nameOfField.ContainsKey(item.Name))
                {
                    nameOfField[item.Name] = type.GetField(item.Name,
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }
                else
                {
                    nameOfField[item.Name] = item;
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
                            nameOfField[item.Name] = item;
                        }
                    }
                }
                loopType = loopType.BaseType;
            }
            return nameOfField;
        }


        public static Dictionary<string, PropertyInfo> GetAllPropertyInfo(Type type)
        {
            Dictionary<string, PropertyInfo> nameOfField = new Dictionary<string, PropertyInfo>();
            PropertyInfo[] typeAddrFieldsNow = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var item in typeAddrFieldsNow)
            {
                if (nameOfField.ContainsKey(item.Name))
                {
                    nameOfField[item.Name] = type.GetProperty(item.Name,
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }
                else
                {
                    nameOfField[item.Name] = item;
                }
            }
            var loopType = type;
            while (loopType.BaseType != typeof(object))
            {
                foreach (var item in loopType.BaseType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (!nameOfField.ContainsKey(item.Name))
                    {
                        nameOfField[item.Name] = item;
                    }
                }
                loopType = loopType.BaseType;
            }
            return nameOfField;
        }



        ~TypeAddrReflectionWrapper() {
            Marshal.FreeHGlobal(delegateValueIntPtr);
        }
        public static bool IsFundamental(Type type)
        {
            return type.IsPrimitive || type.IsEnum || type.Equals(typeof(string)) || type.Equals(typeof(DateTime));
        }

        MulticastDelegateValue* delegateValue;
        IntPtr delegateValueIntPtr;

        public unsafe TypeAddrReflectionWrapper(Type type)
        {
            isValueType = type.IsValueType;
            Dictionary<string, TypeAddrFieldAndProperty> nameOfField = new Dictionary<string, TypeAddrFieldAndProperty>();
            FieldInfo[] typeAddrFieldsNow = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var item in typeAddrFieldsNow)
            {
                if (nameOfField.ContainsKey(item.Name))
                {
                    nameOfField[item.Name] = new TypeAddrFieldAndProperty(type.GetField(item.Name,
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                }
                else
                {
                    nameOfField[item.Name] = new TypeAddrFieldAndProperty(item);
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
                            nameOfField[item.Name] = new TypeAddrFieldAndProperty(item);
                        }
                    }
                }
                loopType = loopType.BaseType;
            }

            //获得所有属性 get set
            //如果属性是值类型且不是基本数据类型，且不是DateTime
            //额外处理
            //计算属性数量 构造非托管内存
            //讲属性方法设置到非托管内存
            //int propertySize = 0;
            PropertyInfo[] propertyInfosNow = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Dictionary<string, TypeAddrFieldAndProperty> nameOfProperty = new Dictionary<string, TypeAddrFieldAndProperty>();
            foreach (var item in propertyInfosNow)
            {
                if (nameOfField.ContainsKey(item.Name))
                {
                    //nameOfProperty[item.Name] = new TypeAddrFieldAndProperty(type.GetField(item.Name,
                    //    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                }
                else
                {
                    if (item.Name != "Item")
                    {
                        var property = new TypeAddrFieldAndProperty(type, item);
                        //if (property.isPropertyGet)
                        //{
                        //    ++propertySize;
                        //}
                        //if (property.isPropertySet)
                        //{
                        //    ++propertySize;
                        //}
                        nameOfField[item.Name] = property;
                        nameOfProperty[item.Name] = property;
                    }
                }
            }


            //delegateValueIntPtr = Marshal.AllocHGlobal(propertySize * sizeof(MulticastDelegateValue));
            //delegateValue = (MulticastDelegateValue*)delegateValueIntPtr.ToPointer();
            //int propertyIndex = 0;
            //foreach (TypeAddrFieldAndProperty item in nameOfProperty.Values)
            //{
            //    if (item.isPropertyGet)
            //    {
            //        //var p = delegateValue + propertyIndex;
            //        //GeneralTool.Memcpy(p, GeneralTool.ObjectToVoid(item.propertyDelegateItem._get),
            //        //    sizeof(MulticastDelegateValue));
            //        //item.propertyDelegateItem.GetDelegateIntPtr = p;
            //        //++propertyIndex;
            //    }
            //    if (item.isPropertySet)
            //    {
            //        if (item.propertyWrapper == null)
            //        {
            //            //var p = delegateValue + propertyIndex;
            //            //GeneralTool.Memcpy(p, GeneralTool.ObjectToVoid(item.propertyDelegateItem._set),
            //            //    sizeof(MulticastDelegateValue));
            //            //item.propertyDelegateItem._set = p;
            //            //++propertyIndex;
            //        }
            //        else
            //        {
            //            var p = delegateValue + propertyIndex;
            //            //Action<object> setValueCall = item.propertyWrapper.Set;
            //            //GeneralTool.Memcpy(p, GeneralTool.ObjectToVoid(setValueCall),
            //            //    sizeof(MulticastDelegateValue));

            //            //item.propertyDelegateItem.setTarget = (IntPtr*)&(p->_target);

            //            //item.propertyWrapper.set = (Delegate)GeneralTool.VoidToObject(p);

            //            //item.propertyDelegateItem._setDelegate = setValueCall;
            //            ++propertyIndex;

            //            IPropertyWrapperTarget wrapperTarget = item.propertyWrapper;

            //            GeneralTool.Memcpy(p,
            //                GeneralTool.ObjectToVoid(wrapperTarget.set),
            //                sizeof(MulticastDelegateValue));

            //            PropertyDelegateItem propertyDelegateItem = item.propertyDelegateItem;
            //            propertyDelegateItem.setTargetPtr = &(p->_target);

            //            wrapperTarget.set = (Delegate)GeneralTool.VoidToObject(p);

            //            Action<object> setValueCall = wrapperTarget.Set;
            //            propertyDelegateItem._setDelegate = setValueCall;

            //            propertyDelegateItem.debug = 8;
            //            //var a1_p = GeneralTool.ObjectToVoid(obj);
            //            //multicastDelegateValue._target = a1_p;



            //        }







            //    }
            //}





            this.allTypeField = new TypeAddrFieldAndProperty[nameOfField.Count];
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


        public Dictionary<string, TypeAddrFieldAndProperty> nameOfField;
        StringTable stringTable;
        TypeAddrFieldAndProperty[] allTypeField;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TypeAddrFieldAndProperty Find(char* d, int length) 
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
            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoid(obj);
            *ptr = typeHead;
            *(ptr + 1) = new IntPtr(0);
            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(out GCHandle gcHandle, out byte* bytePtr, out byte* objPtr)
        {
            object obj = new byte[sizeByte_1];
            gcHandle = GCHandle.Alloc(obj, GCHandleType.Pinned);
            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoid(obj);
            *ptr = typeHead;
            objPtr = (byte*)ptr;
            ++ptr;
            *ptr = new IntPtr(0);
            bytePtr = (byte*)ptr;
            return obj;
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create()
        {
            object obj = new byte[sizeByte_1];
            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoid(obj);
            *ptr = typeHead;
            *(ptr + 1) = new IntPtr(0);
            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr* CreateGetIntPtr(out object obj)
        {
            obj = new byte[sizeByte_1];
            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoid(obj);
            *ptr = typeHead;
            return ptr;
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


    public unsafe class TypeAddrFieldAndProperty
    {
        public TypeAddrReflectionWrapper wrapper;
        public ReadCollectionLink read;
        //
        public void StartReadCollectionLink() 
        {
            if (fieldOrPropertyType != typeof(object))
            {
                read = CollectionManager.GetReadCollectionLink(fieldOrPropertyType);
            }
        }

        /// <summary>
        ///  class struct
        /// </summary>
        public bool isValueType = false;

        public TypeAddrFieldAndProperty(FieldInfo fieldInfo)
        {
            this.isPublic = fieldInfo.IsPublic;
            this.isProperty = false;
            this.fieldInfo = fieldInfo;
            this.fieldOrPropertyType = fieldInfo.FieldType;
            offset = UnsafeOperation.GetFeildOffset(fieldInfo);
            typeCode = Type.GetTypeCode(fieldOrPropertyType);
            isValueType = fieldOrPropertyType.IsValueType;
            isArray = fieldOrPropertyType.IsArray;
            isEnum = fieldOrPropertyType.IsEnum;
            if (isValueType)
            {
                stackSize = UnsafeOperation.SizeOfStack(fieldOrPropertyType);
            }
            else
            {
                stackSize = UnsafeOperation.PTR_COUNT;
            }
            heapSize = UnsafeOperation.SizeOf(fieldOrPropertyType);

            typeHead = UnsafeOperation.GetTypeHead(fieldOrPropertyType);
            StartReadCollectionLink();
        }


        public TypeAddrFieldAndProperty(Type parntType,  PropertyInfo propertyInfo)
        {
            this.isPublic = true;
            this.isProperty = true;
            this.propertyInfo = propertyInfo;
            this.fieldOrPropertyType = propertyInfo.PropertyType;
            this.typeCode = Type.GetTypeCode(fieldOrPropertyType);
            this.isValueType = fieldOrPropertyType.IsValueType;
            this.isArray = fieldOrPropertyType.IsArray;
            this.isEnum = fieldOrPropertyType.IsEnum;

            this.isPropertySet = propertyInfo.SetMethod != null;
            this.isPropertyGet = propertyInfo.GetMethod != null;
            this.propertyDelegateItem = new PropertyDelegateItem2();
            if (this.isPropertyGet)
            {
                this.propertyDelegateItem._get = PropertyWrapper.CreateGetTargetDelegate(propertyInfo);
            }
            if (this.isPropertySet) 
            {
                if (isValueType && !TypeAddrReflectionWrapper.IsFundamental(this.fieldOrPropertyType))
                {
                    Delegate sourceDelegate;
                    Delegate set = PropertyWrapper.CreateStructIPropertyWrapperTarget(parntType,
                        propertyInfo, out sourceDelegate);
                    this.propertyDelegateItem = new PropertyDelegateItem2();
                    this.propertyDelegateItem._set = set;

                    //this.propertyWrapper = PropertyWrapper.CreateStructIPropertyWrapperTarget(propertyInfo);
                }
                else
                {
                    //this.propertyDelegateItem._set = PropertyWrapper.CreateSetTargetDelegate(propertyInfo);
                    this.propertyDelegateItem._set = PropertyWrapper.CreateSetTargetDelegate2(parntType, propertyInfo);
                }
            }

            typeHead = UnsafeOperation.GetTypeHead(fieldOrPropertyType);
            StartReadCollectionLink();
        }


        public bool isArray = false;
        public bool isEnum = false;
        public bool isProperty = false;
        public bool isPublic = false;
        
        public FieldInfo fieldInfo;
        public IntPtr typeHead;
        public Type fieldOrPropertyType;
        public int offset;
        public int stackSize;
        public int heapSize;
        public TypeCode typeCode;


        public bool isPropertySet = true;
        public bool isPropertyGet = true;
        public PropertyInfo propertyInfo;
        public PropertyDelegateItem2 propertyDelegateItem;
        public IPropertyWrapperTarget propertyWrapper;
        

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
        public unsafe object GetValue(object source)
        {
            void* v = GeneralTool.ObjectToVoid(source);

            void* field = (byte*)v + UnsafeOperation.PTR_COUNT + this.offset;

            if (this.isValueType)
            {
                switch (this.typeCode)
                {
                    case TypeCode.Boolean:
                        return *(bool*)field;
                    case TypeCode.Byte:
                        return *(Byte*)field;
                    case TypeCode.Char:
                        return *(Char*)field;
                    case TypeCode.DateTime:
                        return *(DateTime*)field;
                    //case TypeCode.DBNull:
                    //    return GeneralTool.VoidToObject(field);
                    //case TypeCode.String:
                    //    return GeneralTool.VoidToObject(field);
                    case TypeCode.Decimal:
                        return *(Decimal*)field;
                    case TypeCode.Double:
                        return *(Double*)field;
                    case TypeCode.Empty:
                    case TypeCode.Int16:
                        return *(Int16*)field;
                    case TypeCode.Int32:
                        return *(Int32*)field;
                    case TypeCode.Int64:
                        return *(Int64*)field;
                    case TypeCode.SByte:
                        return *(Int64*)field;
                    case TypeCode.Single:
                        return *(Single*)field;
                    case TypeCode.UInt16:
                        return *(UInt16*)field;
                    case TypeCode.UInt32:
                        return *(UInt32*)field;
                    case TypeCode.UInt64:
                        return *(UInt64*)field;
                    case TypeCode.Object:
                    default:
                        //GC.Collect();

                        object obj = new byte[this.heapSize - 1 * UnsafeOperation.PTR_COUNT];
                        IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoid(obj);
                        //ptr = UnsafeUtility.GetObjectAddr(obj);
                        //*(IntPtr*)ptr = typeHead;
                        *ptr = typeHead;
                        ++ptr;
                        GeneralTool.Memcpy(ptr, field, this.stackSize);
                        //GC.Collect();
                        return obj;
                }
            }
            else
            {
                return GeneralTool.VoidToObject(*(IntPtr**)field);
            }
        }

    }




}
