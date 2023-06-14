using PtrReflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PtrReflection
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


        static Dictionary<Type, TypeAddrReflectionWrapper> allTypeWarp = new Dictionary<Type, TypeAddrReflectionWrapper>();
        public static TypeAddrReflectionWrapper GetTypeWarp(Type type)
        {
            TypeAddrReflectionWrapper ob;
            if (allTypeWarp.TryGetValue(type, out ob))
            {
                return ob;
            }
            lock (allTypeWarp)
            {
                return allTypeWarp[type] = new TypeAddrReflectionWrapper(type);
            }
        }


        ~TypeAddrReflectionWrapper()
        {
            //Marshal.FreeHGlobal(delegateValueIntPtr);
        }
        public static bool IsFundamental(Type type)
        {
            return type.IsPrimitive || type.IsEnum || type.Equals(typeof(string)) || type.Equals(typeof(DateTime));
        }

        //MulticastDelegateValue* delegateValue;
        //IntPtr delegateValueIntPtr;

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
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public), isValueType);
                }
                else
                {
                    nameOfField[item.Name] = new TypeAddrFieldAndProperty(item, isValueType);
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
                            nameOfField[item.Name] = new TypeAddrFieldAndProperty(item, isValueType);
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
            //        //GeneralTool.MemCpy(p, GeneralTool.ObjectToVoidPtr(item.propertyDelegateItem._get),
            //        //    sizeof(MulticastDelegateValue));
            //        //item.propertyDelegateItem.GetDelegateIntPtr = p;
            //        //++propertyIndex;
            //    }
            //    if (item.isPropertySet)
            //    {
            //        if (item.propertyWrapper == null)
            //        {
            //            //var p = delegateValue + propertyIndex;
            //            //GeneralTool.MemCpy(p, GeneralTool.ObjectToVoidPtr(item.propertyDelegateItem._set),
            //            //    sizeof(MulticastDelegateValue));
            //            //item.propertyDelegateItem._set = p;
            //            //++propertyIndex;
            //        }
            //        else
            //        {
            //            var p = delegateValue + propertyIndex;
            //            //Action<object> setValueCall = item.propertyWrapper.Set;
            //            //GeneralTool.MemCpy(p, GeneralTool.ObjectToVoidPtr(setValueCall),
            //            //    sizeof(MulticastDelegateValue));

            //            //item.propertyDelegateItem.setTarget = (IntPtr*)&(p->_target);

            //            //item.propertyWrapper.set = (Delegate)GeneralTool.VoidPtrToObject(p);

            //            //item.propertyDelegateItem._setDelegate = setValueCall;
            //            ++propertyIndex;

            //            IPropertyWrapperTarget wrapperTarget = item.propertyWrapper;

            //            GeneralTool.MemCpy(p,
            //                GeneralTool.ObjectToVoidPtr(wrapperTarget.set),
            //                sizeof(MulticastDelegateValue));

            //            PropertyDelegateItem propertyDelegateItem = item.propertyDelegateItem;
            //            propertyDelegateItem.setTargetPtr = &(p->_target);

            //            wrapperTarget.set = (Delegate)GeneralTool.VoidPtrToObject(p);

            //            Action<object> setValueCall = wrapperTarget.Set;
            //            propertyDelegateItem._setDelegate = setValueCall;

            //            propertyDelegateItem.debug = 8;
            //            //var a1_p = GeneralTool.ObjectToVoidPtr(obj);
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
            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(obj);
            *ptr = typeHead;
            *(ptr + 1) = new IntPtr(0);
            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(out GCHandle gcHandle, out byte* bytePtr, out byte* objPtr)
        {
            object obj = new byte[sizeByte_1];
            gcHandle = GCHandle.Alloc(obj, GCHandleType.Pinned);
            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(obj);
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
            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(obj);
            *ptr = typeHead;
            *(ptr + 1) = new IntPtr(0);
            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr* CreateGetIntPtr(out object obj)
        {
            obj = new byte[sizeByte_1];
            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(obj);
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
        //public TypeAddrReflectionWrapper wrapper;
        //public ReadCollectionLink read;
        //public IArrayWrap arrayWrap;
        //public CollectionManager.TypeCollectionEnum readEnum;

        //CollectionManager.TypeAllCollection typeAllCollection;
        //public CollectionManager.TypeAllCollection GetTypeAllCollection()
        //{
        //    //if (fieldOrPropertyType != typeof(object))
        //    if (typeAllCollection == null)
        //    {
        //        typeAllCollection = CollectionManager.GetTypeCollection(fieldOrPropertyType);
        //        //read = CollectionManager.GetReadCollectionLink(fieldOrPropertyType);
        //    }
        //    return typeAllCollection;
        //}

        /// <summary>
        ///  class struct
        /// </summary>
        public bool isValueType = false;
        public bool isParntTypeValueType = false;

        public TypeAddrFieldAndProperty(FieldInfo fieldInfo, bool isParntTypeValueType)
        {
            this.isPublic = fieldInfo.IsPublic;
            this.isProperty = false;
            this.fieldInfo = fieldInfo;
            this.fieldOrPropertyType = fieldInfo.FieldType;
            this.offsetTypeHead = UnsafeOperation.GetFieldOffset(fieldInfo);
            this.offsetObject = this.offsetTypeHead + UnsafeOperation.PTR_COUNT;
            this.isParntTypeValueType = isParntTypeValueType;
            if (isParntTypeValueType)
            {
                this.offset = this.offsetTypeHead;
            }
            else
            {
                this.offset = this.offsetObject;
            }
            this.offsetObject = this.offsetTypeHead + UnsafeOperation.PTR_COUNT;
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
            //if (isEnum)
            //{
            //    fieldOrPropertyType = typeof(EnumWrapper<>).MakeGenericType(fieldOrPropertyType);
            //}
            //StartReadCollectionLink();
        }


        public TypeAddrFieldAndProperty(Type parntType,  PropertyInfo propertyInfo)
        {
            this.isPublic = true;
            this.isProperty = true;
            this.propertyInfo = propertyInfo;
            this.fieldOrPropertyType = propertyInfo.PropertyType;
            this.typeCode = Type.GetTypeCode(fieldOrPropertyType);
            this.isValueType = fieldOrPropertyType.IsValueType;
            this.isParntTypeValueType = parntType.IsValueType;
            this.isArray = fieldOrPropertyType.IsArray;
            this.isEnum = fieldOrPropertyType.IsEnum;

            this.isPropertySet = propertyInfo.SetMethod != null;
            this.isPropertyGet = propertyInfo.GetMethod != null;
            this.propertyDelegateItem = new PropertyDelegateItem();

            if (this.isPropertyGet)
            {
                if (isValueType && !TypeAddrReflectionWrapper.IsFundamental(this.fieldOrPropertyType))
                {
                    Delegate sourceDelegate;
                    Delegate get = PropertyWrapper.CreateStructGet(parntType,
                        propertyInfo, out sourceDelegate);
                    this.propertyDelegateItem._get = get;
                }
                else
                {
                    this.propertyDelegateItem._get = PropertyWrapper.CreateClassGet(parntType, propertyInfo);
                }
            }
            if (this.isPropertySet) 
            {
                if (isValueType && !TypeAddrReflectionWrapper.IsFundamental(this.fieldOrPropertyType))
                {
                    Delegate sourceDelegate;
                    Delegate set = PropertyWrapper.CreateStructIPropertyWrapperTarget(parntType,
                        propertyInfo, out sourceDelegate);
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
            //if (isEnum)
            //{
            //    fieldOrPropertyType = typeof(EnumWrapper<>).MakeGenericType(fieldOrPropertyType);
            //}
            //StartReadCollectionLink();
        }


        public bool isArray = false;
        public bool isEnum = false;
        public bool isProperty = false;
        public bool isPublic = false;
        
        public FieldInfo fieldInfo;
        public IntPtr typeHead;
        public Type fieldOrPropertyType;

        /// <summary>
        /// 值类型和引用类型都是从对象地址开始的偏移量
        /// </summary>
        public int offsetTypeHead;

        /// <summary>
        /// 值类型是直接的偏移量，引用类型是从TypeHead的偏移量
        /// </summary>
        public int offset;

        /// <summary>
        /// 只考虑引用类型从对象地址开始的偏移量
        /// </summary>
        public int offsetObject;
        public int stackSize;
        public int heapSize;
        public TypeCode typeCode;


        public bool isPropertySet = true;
        public bool isPropertyGet = true;
        public PropertyInfo propertyInfo;
        public PropertyDelegateItem propertyDelegateItem;
        public IPropertyWrapperTarget propertyWrapper;
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetStruct(void* destination, void* value, int size)
        {
            value = ((IntPtr*)value) + 1;
            size -= UnsafeOperation.PTR_COUNT * 2;
            byte* byteV = (byte*)value;
            byte* byteD = (byte*)destination;
            //for (int i = 0; i < size; i++)
            //{
            //    *byteD = *byteV;
            //    ++byteD;
            //    ++byteV;
            //}
            GeneralTool.MemCpy(destination, value, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetValue(object source, object value)
        {
            TypedReference tf = __makeref(source);
            void* v = GeneralTool.ObjectToVoidPtr(source);

            if (isProperty)
            {
                if (this.isValueType)
                {
                    switch (this.typeCode)
                    {
                        case TypeCode.Boolean:
                            propertyDelegateItem.setBoolean(v, (bool)value);
                            break;
                        case TypeCode.Byte:
                            propertyDelegateItem.setByte(v, (Byte)value);
                            break;
                        case TypeCode.Char:
                            propertyDelegateItem.setChar(v, (Char)value);
                            break;
                        case TypeCode.DateTime:
                            propertyDelegateItem.setDateTime(v, (DateTime)value);
                            break;
                        case TypeCode.Decimal:
                            propertyDelegateItem.setDecimal(v, (Decimal)value);
                            break;
                        case TypeCode.Double:
                            propertyDelegateItem.setDouble(v, (Double)value);
                            break;
                        case TypeCode.Empty:
                            break;
                        case TypeCode.Int16:
                            propertyDelegateItem.setInt16(v, (Int16)value);
                            break;
                        case TypeCode.Int32:
                            propertyDelegateItem.setInt32(v, (Int32)value);
                            break;
                        case TypeCode.Int64:
                            propertyDelegateItem.setInt64(v, (Int64)value);
                            break;
                        case TypeCode.SByte:
                            propertyDelegateItem.setSByte(v, (SByte)value);
                            break;
                        case TypeCode.Single:
                            propertyDelegateItem.setSingle(v, (Single)value);
                            break;
                        case TypeCode.UInt16:
                            propertyDelegateItem.setUInt16(v, (UInt16)value);
                            break;
                        case TypeCode.UInt32:
                            propertyDelegateItem.setUInt32(v, (UInt32)value);
                            break;
                        case TypeCode.UInt64:
                            propertyDelegateItem.setUInt64(v, (UInt64)value);
                            break;
                        //case TypeCode.String:
                        case TypeCode.Object:
                            propertyDelegateItem.setObject(v, value);
                            break;
                    }
                }
                else
                {
                    propertyDelegateItem.setObject(v, value);
                }
            }
            else
            {
                void* field = (byte*)v + UnsafeOperation.PTR_COUNT + this.offsetTypeHead;

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
                        case TypeCode.Object:
                        default:
                            //tf = __makeref(value);
                            //SetStruct(field, (void*)(*(IntPtr*)*((IntPtr*)&tf + 1)), this.stackSize);
                            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(value);
                            GeneralTool.MemCpy(field, ptr + 1, stackSize);
                            break;
                            //tf = __makeref(value);
                            //SetStruct(field, (void*)(*(IntPtr*)*((IntPtr*)&tf + 1)), this.stackSize);
                            //break;
                    }
                }
                else
                {
                    GeneralTool.SetObject(field, value);
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe object GetValue(object obj)
        {
            void* source = GeneralTool.ObjectToVoidPtr(obj);

            if (isProperty)
            {
                if (this.isValueType)
                {
                    switch (this.typeCode)
                    {
                        case TypeCode.Boolean:
                            return propertyDelegateItem.getBoolean(source);
                        case TypeCode.Byte:
                            return propertyDelegateItem.getByte(source);
                        case TypeCode.Char:
                            return propertyDelegateItem.getChar(source);
                        case TypeCode.DateTime:
                            return propertyDelegateItem.getDateTime(source);
                        case TypeCode.Decimal:
                            return propertyDelegateItem.getDecimal(source);
                        case TypeCode.Double:
                            return propertyDelegateItem.getDouble(source);
                        case TypeCode.Int16:
                            return propertyDelegateItem.getInt16(source);
                        case TypeCode.Int32:
                            return propertyDelegateItem.getInt32(source);
                        case TypeCode.Int64:
                            return propertyDelegateItem.getInt64(source);
                        case TypeCode.SByte:
                            return propertyDelegateItem.getSByte(source);
                        case TypeCode.Single:
                            return propertyDelegateItem.getSingle(source);
                        case TypeCode.UInt16:
                            return propertyDelegateItem.getUInt16(source);
                        case TypeCode.UInt32:
                            return propertyDelegateItem.getUInt32(source);
                        case TypeCode.UInt64:
                            return propertyDelegateItem.getUInt64(source);
                        case TypeCode.Object:
                        default:
                            return propertyDelegateItem.getObject(source);
                    }
                }
                else
                {
                    return propertyDelegateItem.getObject(source);
                }
            }
            else
            {
                if (this.isValueType)
                {
                    switch (this.typeCode)
                    {
                        case TypeCode.Boolean:
                            return *(bool*)((byte*)source + this.offsetObject);
                        case TypeCode.Byte:
                            return *(Byte*)((byte*)source + this.offsetObject);
                        case TypeCode.Char:
                            return *(Char*)((byte*)source + this.offsetObject);
                        case TypeCode.DateTime:
                            return *(DateTime*)((byte*)source + this.offsetObject);
                        //case TypeCode.DBNull:
                        //    return GeneralTool.VoidPtrToObject(field);
                        //case TypeCode.String:
                        //    return GeneralTool.VoidPtrToObject(field);
                        case TypeCode.Decimal:
                            return *(Decimal*)((byte*)source + this.offsetObject);
                        case TypeCode.Double:
                            return *(Double*)((byte*)source + this.offsetObject);
                        //case TypeCode.Empty:
                        case TypeCode.Int16:
                            return *(Int16*)((byte*)source + this.offsetObject);
                        case TypeCode.Int32:
                            return *(Int32*)((byte*)source + this.offsetObject);
                        case TypeCode.Int64:
                            return *(Int64*)((byte*)source + this.offsetObject);
                        case TypeCode.SByte:
                            return *(Int64*)((byte*)source + this.offsetObject);
                        case TypeCode.Single:
                            return *(Single*)((byte*)source + this.offsetObject);
                        case TypeCode.UInt16:
                            return *(UInt16*)((byte*)source + this.offsetObject);
                        case TypeCode.UInt32:
                            return *(UInt32*)((byte*)source + this.offsetObject);
                        case TypeCode.UInt64:
                            return *(UInt64*)((byte*)source + this.offsetObject);
                        case TypeCode.Object:
                        default:
                            object outObj = new byte[this.heapSize - 3 * UnsafeOperation.PTR_COUNT];
                            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(outObj);
                            *ptr = typeHead;
                            ptr += 1;
                            GeneralTool.MemCpy(ptr, ((byte*)source + this.offsetObject), this.stackSize);
                            return outObj;
                    }
                }
                else
                {
                    return GeneralTool.VoidPtrToObject(*(IntPtr**)((byte*)source + this.offsetObject));
                }
                //gcHandle.Free();
            }

        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void ClassSetValue(void** source, object value)
        {
            if (isProperty)
            {
                ClassSetPropertyValue(source, value);
            }
            else
            {
                ClassSetFieldValue(source, value);
            }

        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void ClassSetFieldValue(void** source, object value)
        {
            void* field = (byte*)*source + this.offset;
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
                    case TypeCode.SByte:
                        *(SByte*)field = (SByte)value;
                        break;
                    case TypeCode.Single:
                        *(Single*)field = (Single)value;
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
                    case TypeCode.String:
                    case TypeCode.Object:
                        IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(value);
                        GeneralTool.MemCpy(field, ptr + 1, stackSize);
                        break;
                }
            }
            else
            {
                GeneralTool.SetObject(field, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void ClassSetPropertyValue(void** source, object value)
        {
            if (this.isValueType)
            {
                switch (this.typeCode)
                {
                    case TypeCode.Boolean:
                        propertyDelegateItem.setBoolean(*source, (bool)value);
                        break;
                    case TypeCode.Byte:
                        propertyDelegateItem.setByte(*source, (Byte)value);
                        break;
                    case TypeCode.Char:
                        propertyDelegateItem.setChar(*source, (Char)value);
                        break;
                    case TypeCode.DateTime:
                        propertyDelegateItem.setDateTime(*source, (DateTime)value);
                        break;
                    case TypeCode.Decimal:
                        propertyDelegateItem.setDecimal(*source, (Decimal)value);
                        break;
                    case TypeCode.Double:
                        propertyDelegateItem.setDouble(*source, (Double)value);
                        break;
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Int16:
                        propertyDelegateItem.setInt16(*source, (Int16)value);
                        break;
                    case TypeCode.Int32:
                        propertyDelegateItem.setInt32(*source, (Int32)value);
                        break;
                    case TypeCode.Int64:
                        propertyDelegateItem.setInt64(*source, (Int64)value);
                        break;
                    case TypeCode.SByte:
                        propertyDelegateItem.setSByte(*source, (SByte)value);
                        break;
                    case TypeCode.Single:
                        propertyDelegateItem.setSingle(*source, (Single)value);
                        break;
                    case TypeCode.UInt16:
                        propertyDelegateItem.setUInt16(*source, (UInt16)value);
                        break;
                    case TypeCode.UInt32:
                        propertyDelegateItem.setUInt32(*source, (UInt32)value);
                        break;
                    case TypeCode.UInt64:
                        propertyDelegateItem.setUInt64(*source, (UInt64)value);
                        break;
                    //case TypeCode.String:
                    case TypeCode.Object:
                        propertyDelegateItem.setObject(*source, value);
                        break;
                }
            }
            else
            {
                propertyDelegateItem.setObject(*source, value);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe object ClassGetValue(void** source)
        {
            if (isProperty)
            {
                return ClassGetPropertyValue(source);
            }
            else
            {
                return ClassGetFieldValue(source);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe object ClassGetFieldValue(void** source)
        {//void* field = (byte*)*source + this.offsetPtr;
            if (this.isValueType)
            {
                switch (this.typeCode)
                {
                    case TypeCode.Boolean:
                        return *(bool*)((byte*)*source + this.offsetObject);
                    case TypeCode.Byte:
                        return *(Byte*)((byte*)*source + this.offsetObject);
                    case TypeCode.Char:
                        return *(Char*)((byte*)*source + this.offsetObject);
                    case TypeCode.DateTime:
                        return *(DateTime*)((byte*)*source + this.offsetObject);
                    //case TypeCode.DBNull:
                    //    return GeneralTool.VoidPtrToObject(field);
                    //case TypeCode.String:
                    //    return GeneralTool.VoidPtrToObject(field);
                    case TypeCode.Decimal:
                        return *(Decimal*)((byte*)*source + this.offsetObject);
                    case TypeCode.Double:
                        return *(Double*)((byte*)*source + this.offsetObject);
                    //case TypeCode.Empty:
                    case TypeCode.Int16:
                        return *(Int16*)((byte*)*source + this.offsetObject);
                    case TypeCode.Int32:
                        return *(Int32*)((byte*)*source + this.offsetObject);
                    case TypeCode.Int64:
                        return *(Int64*)((byte*)*source + this.offsetObject);
                    case TypeCode.SByte:
                        return *(Int64*)((byte*)*source + this.offsetObject);
                    case TypeCode.Single:
                        return *(Single*)((byte*)*source + this.offsetObject);
                    case TypeCode.UInt16:
                        return *(UInt16*)((byte*)*source + this.offsetObject);
                    case TypeCode.UInt32:
                        return *(UInt32*)((byte*)*source + this.offsetObject);
                    case TypeCode.UInt64:
                        return *(UInt64*)((byte*)*source + this.offsetObject);
                    case TypeCode.Object:
                    default:
                        object obj = new byte[this.heapSize - 3 * UnsafeOperation.PTR_COUNT];
                        IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(obj);
                        *ptr = typeHead;
                        ptr += 1;
                        GeneralTool.MemCpy(ptr, ((byte*)*source + this.offsetObject), this.stackSize);
                        return obj;
                }
            }
            else
            {
                return GeneralTool.VoidPtrToObject(*(IntPtr**)((byte*)*source + this.offsetObject));
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe object ClassGetPropertyValue(void** source)
        {
            if (this.isValueType)
            {
                switch (this.typeCode)
                {
                    case TypeCode.Boolean:
                        return propertyDelegateItem.getBoolean(*source);
                    case TypeCode.Byte:
                        return propertyDelegateItem.getByte(*source);
                    case TypeCode.Char:
                        return propertyDelegateItem.getChar(*source);
                    case TypeCode.DateTime:
                        return propertyDelegateItem.getDateTime(*source);
                    case TypeCode.Decimal:
                        return propertyDelegateItem.getDecimal(*source);
                    case TypeCode.Double:
                        return propertyDelegateItem.getDouble(*source);
                    case TypeCode.Int16:
                        return propertyDelegateItem.getInt16(*source);
                    case TypeCode.Int32:
                        return propertyDelegateItem.getInt32(*source);
                    case TypeCode.Int64:
                        return propertyDelegateItem.getInt64(*source);
                    case TypeCode.SByte:
                        return propertyDelegateItem.getSByte(*source);
                    case TypeCode.Single:
                        return propertyDelegateItem.getSingle(*source);
                    case TypeCode.UInt16:
                        return propertyDelegateItem.getUInt16(*source);
                    case TypeCode.UInt32:
                        return propertyDelegateItem.getUInt32(*source);
                    case TypeCode.UInt64:
                        return propertyDelegateItem.getUInt64(*source);
                    case TypeCode.Object:
                    default:
                        return propertyDelegateItem.getObject(*source);
                }
            }
            else
            {
                return propertyDelegateItem.getObject(*source);
            }
        }


        public unsafe T ClassReadStruct<T>(void** source) where T : struct
        {
            if (isProperty)
            {
                return (T)propertyDelegateItem.getObject((byte*)*source + this.offset);
            }
            else
            {

                T v = new T();
                GeneralTool.MemCpy(GeneralTool.AsPointer<T>(ref v), (byte*)*source + this.offset, stackSize);
                return v;
            }
        }

        public unsafe void ClassWriteStruct<T>(void** source, T v) where T : struct
        {
            if (isProperty)
            {
                propertyDelegateItem.setObject(*source, v);
            }
            else
            {

                GeneralTool.MemCpy((byte*)*source + this.offset, GeneralTool.AsPointer(ref v), stackSize);
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void StructSetValue(void* source, object value)
        {
            if (isProperty)
            {
                StructSetPropertyValue(source, value);
            }
            else
            {
                StructSetFieldValue(source, value);
            }

        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void StructSetFieldValue(void* source, object value)
        {
            void* field = (byte*)source + this.offset;
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
                    case TypeCode.SByte:
                        *(SByte*)field = (SByte)value;
                        break;
                    case TypeCode.Single:
                        *(Single*)field = (Single)value;
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
                    case TypeCode.String:
                    case TypeCode.Object:
                        IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(value);
                        GeneralTool.MemCpy(field, ptr + 1, stackSize);
                        break;
                }
            }
            else
            {
                GeneralTool.SetObject(field, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void StructSetPropertyValue(void* source, object value)
        {
            if (this.isValueType)
            {
                switch (this.typeCode)
                {
                    case TypeCode.Boolean:
                        propertyDelegateItem.setBoolean(source, (bool)value);
                        break;
                    case TypeCode.Byte:
                        propertyDelegateItem.setByte(source, (Byte)value);
                        break;
                    case TypeCode.Char:
                        propertyDelegateItem.setChar(source, (Char)value);
                        break;
                    case TypeCode.DateTime:
                        propertyDelegateItem.setDateTime(source, (DateTime)value);
                        break;
                    case TypeCode.Decimal:
                        propertyDelegateItem.setDecimal(source, (Decimal)value);
                        break;
                    case TypeCode.Double:
                        propertyDelegateItem.setDouble(source, (Double)value);
                        break;
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Int16:
                        propertyDelegateItem.setInt16(source, (Int16)value);
                        break;
                    case TypeCode.Int32:
                        propertyDelegateItem.setInt32(source, (Int32)value);
                        break;
                    case TypeCode.Int64:
                        propertyDelegateItem.setInt64(source, (Int64)value);
                        break;
                    case TypeCode.SByte:
                        propertyDelegateItem.setSByte(source, (SByte)value);
                        break;
                    case TypeCode.Single:
                        propertyDelegateItem.setSingle(source, (Single)value);
                        break;
                    case TypeCode.UInt16:
                        propertyDelegateItem.setUInt16(source, (UInt16)value);
                        break;
                    case TypeCode.UInt32:
                        propertyDelegateItem.setUInt32(source, (UInt32)value);
                        break;
                    case TypeCode.UInt64:
                        propertyDelegateItem.setUInt64(source, (UInt64)value);
                        break;
                    //case TypeCode.String:
                    case TypeCode.Object:
                        propertyDelegateItem.setObject(source, value);
                        break;
                }
            }
            else
            {
                propertyDelegateItem.setObject(source, value);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe object StructGetValue(void* source)
        {
            if (isProperty)
            {
                return StructGetPropertyValue(source);
            }
            else
            {
                return StructGetFieldValue(source);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe object StructGetFieldValue(void* source)
        {//void* field = (byte*)*source + this.offsetPtr;
            if (this.isValueType)
            {
                switch (this.typeCode)
                {
                    case TypeCode.Boolean:
                        return *(bool*)((byte*)source + this.offset);
                    case TypeCode.Byte:
                        return *(Byte*)((byte*)source + this.offset);
                    case TypeCode.Char:
                        return *(Char*)((byte*)source + this.offset);
                    case TypeCode.DateTime:
                        return *(DateTime*)((byte*)source + this.offset);
                    //case TypeCode.DBNull:
                    //    return GeneralTool.VoidPtrToObject(field);
                    //case TypeCode.String:
                    //    return GeneralTool.VoidPtrToObject(field);
                    case TypeCode.Decimal:
                        return *(Decimal*)((byte*)source + this.offset);
                    case TypeCode.Double:
                        return *(Double*)((byte*)source + this.offset);
                    //case TypeCode.Empty:
                    case TypeCode.Int16:
                        return *(Int16*)((byte*)source + this.offset);
                    case TypeCode.Int32:
                        return *(Int32*)((byte*)source + this.offset);
                    case TypeCode.Int64:
                        return *(Int64*)((byte*)source + this.offset);
                    case TypeCode.SByte:
                        return *(Int64*)((byte*)source + this.offset);
                    case TypeCode.Single:
                        return *(Single*)((byte*)source + this.offset);
                    case TypeCode.UInt16:
                        return *(UInt16*)((byte*)source + this.offset);
                    case TypeCode.UInt32:
                        return *(UInt32*)((byte*)source + this.offset);
                    case TypeCode.UInt64:
                        return *(UInt64*)((byte*)source + this.offset);
                    case TypeCode.Object:
                    default:
                        object obj = new byte[this.heapSize - 3 * UnsafeOperation.PTR_COUNT];
                        IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(obj);
                        *ptr = typeHead;
                        ptr += 1;
                        GeneralTool.MemCpy(ptr, ((byte*)source + this.offset), this.stackSize);
                        return obj;
                }
            }
            else
            {
                return GeneralTool.VoidPtrToObject(*(void**)((byte*)source + this.offset));
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe object StructGetPropertyValue(void* source)
        {
            if (this.isValueType)
            {
                switch (this.typeCode)
                {
                    case TypeCode.Boolean:
                        return propertyDelegateItem.getBoolean(source);
                    case TypeCode.Byte:
                        return propertyDelegateItem.getByte(source);
                    case TypeCode.Char:
                        return propertyDelegateItem.getChar(source);
                    case TypeCode.DateTime:
                        return propertyDelegateItem.getDateTime(source);
                    case TypeCode.Decimal:
                        return propertyDelegateItem.getDecimal(source);
                    case TypeCode.Double:
                        return propertyDelegateItem.getDouble(source);
                    case TypeCode.Int16:
                        return propertyDelegateItem.getInt16(source);
                    case TypeCode.Int32:
                        return propertyDelegateItem.getInt32(source);
                    case TypeCode.Int64:
                        return propertyDelegateItem.getInt64(source);
                    case TypeCode.SByte:
                        return propertyDelegateItem.getSByte(source);
                    case TypeCode.Single:
                        return propertyDelegateItem.getSingle(source);
                    case TypeCode.UInt16:
                        return propertyDelegateItem.getUInt16(source);
                    case TypeCode.UInt32:
                        return propertyDelegateItem.getUInt32(source);
                    case TypeCode.UInt64:
                        return propertyDelegateItem.getUInt64(source);
                    case TypeCode.Object:
                    default:
                        return propertyDelegateItem.getObject(source);
                }
            }
            else
            {
                return propertyDelegateItem.getObject(source);
            }
        }

        public unsafe T StructReadStruct<T>(void* source) where T : struct
        {
            if (isProperty)
            {
                return (T)propertyDelegateItem.getObject((byte*)source + this.offset);
            }
            else
            {

                T v = new T();
                GeneralTool.MemCpy(GeneralTool.AsPointer<T>(ref v), (byte*)source + this.offset, stackSize);
                return v;
            }
        }

        public unsafe void StructWriteStruct<T>(void* source, T v) where T : struct
        {
            if (isProperty)
            {
                propertyDelegateItem.setObject(source, v);
            }
            else
            {

                GeneralTool.MemCpy((byte*)source + this.offset, GeneralTool.AsPointer(ref v), stackSize);
            }
        }
    }




}
