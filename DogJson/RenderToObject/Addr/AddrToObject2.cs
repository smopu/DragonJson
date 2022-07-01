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
    public unsafe class AddrToObject2 : IJsonRenderToObject
    {
        Dictionary<Type, TypeAddrReflectionWrapper> allTypeWrapper = new Dictionary<Type, TypeAddrReflectionWrapper>();
        public TypeAddrReflectionWrapper GetTypeWrapper(Type type)
        {
            TypeAddrReflectionWrapper ob;
            if (allTypeWrapper.TryGetValue(type, out ob))
            {
                return ob;
            }
            return allTypeWrapper[type] = new TypeAddrReflectionWrapper(type);
        }

        public AddrToObject2()
        {
            for (int i = 0; i < createObjectItems.Length; i++)
            {
                createObjectItems[i] = new CreateObjectItem();
            }
        }


        static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        class CreateObjectItem
        {
            public Type type;
            public Type sourceType;
            public bool isValueType;
            public ICollectionObjectBase collectionObject;

            public PropertyDelegateItem2 propertyDelegateItem;
            public bool isProperty;
            
            public TypeAddrReflectionWrapper wrapper;
            public int offset;
            public GCHandle gcHandle;
            public byte* bytePtr;
            public byte* bytePtrStart;
            public object obj;

            public TypeCode ArrayItemTypeCode;
            public Type ArrayItemType;
            public int ArrayRank;
            public int ArrayItemTypeSize;
            public int ArrayNowItemSize;


            public object Obj
            {
                // get { return GeneralTool.VoidToObject(byteP - TypeAddrReflectionWrapper.PTR_COUNT); }
                get { return obj; }
                set
                {
                    obj = value;
                    bytePtrStart = (byte*)GeneralTool.ObjectToVoid(value);
                    bytePtr = bytePtrStart + UnsafeOperation.PTR_COUNT;
                }
            }
        }

        struct SetValue
        {
            public JsonObject* objValue;
            public PropertyDelegateItem propertyDelegateItem;
        }

        int setValuesIndex = 0;
        ArrayWrapper arrayWrapper = new ArrayWrapper();

        //SetValue[] setValues = new SetValue[1024];
        int[] setValues = new int[1024];

        int[] setValuesOrder = new int[1024];
        CreateObjectItem[] createObjectItems = new CreateObjectItem[1024];



        public static int indexDbug = 0;
        public unsafe object CreateObject(JsonRender jsonRender, Type type, char* vs, int length)
        {
            setValuesIndex = 0;
            var rootItem = createObjectItems[0];
            int itemCount = jsonRender.objectQueueIndex;
            {
                rootItem.wrapper = GetTypeWrapper(type);
                rootItem.type = type;
                rootItem.Obj = rootItem.wrapper.Create(out rootItem.gcHandle);//out rootItem.gcHandle
                rootItem.isValueType = type.IsValueType;

                //对象数组创建
                for (int i = 1; i < jsonRender.objectQueueIndex; i++)
                {
                    //if (i == 6)
                    //{
                    //    int ccc = 0;
                    //}

                    CreateObjectItem myObject = createObjectItems[i];
                    JsonObject* v = jsonRender.objectQueue + i;
                    JsonObject* parent = jsonRender.objectQueue + v->parentObjectIndex;
                    CreateObjectItem parentObject = createObjectItems[v->parentObjectIndex];
                    ICollectionObjectBase parentCollection = parentObject.collectionObject;
                    TypeAddrFieldAndProperty fieldInfo = null;
                    myObject.isProperty = false;
                    //string key;
                    if (parent->isObject)
                    {
                        if (parentCollection == null)
                        {
                            fieldInfo = parentObject.wrapper.Find(v->keyStringStart, v->keyStringLength);
                            //key = new string(v->keyStringStart, 0, v->keyStringLength);
                            //var fieldInfo = parentObject.wrapper.nameOfField[key];
                            if (fieldInfo.isProperty)
                            {
                                myObject.isProperty = true;
                                myObject.propertyDelegateItem = fieldInfo.propertyDelegateItem;
                            }
                            else
                            {
                                myObject.offset = fieldInfo.offset;
                            }

                            myObject.type = fieldInfo.fieldType;
                        }

                    }
                    else
                    {
                        if (parentCollection == null)
                        {
                            myObject.offset = parentObject.ArrayNowItemSize * v->arrayIndex;
                            myObject.type = parentObject.ArrayItemType;
                        }
                    }

                    //typeLength  parentCollection.GetItemType   fieldInfo.fieldType;
                    if (v->typeLength > 0)
                    {
                        string typeName = new string(vs, v->typeStartIndex, v->typeLength);
                        var valueType = UnsafeOperation.GetType(typeName);

                        if (v->isCommandValue)
                        {
                            myObject.type = typeof(Box<>).MakeGenericType(valueType);
                        }
                        else
                        {
                            myObject.type = valueType;
                        }
                    }
                    else
                    {
                        if (parentCollection != null)
                        {
                            myObject.type = parentCollection.GetItemType(v);
                        }
                    }

                    myObject.isValueType = myObject.type.IsValueType;
                    myObject.sourceType = myObject.type;
                    //"#create"
                    if (v->isConstructor)
                    {
                        myObject.type = typeof(ConstructorWrapper);
                    }

                    if (v->isObject)
                    {
                        ICollectionObjectBase collection;
                        if (!JsonManager.formatterAllObjectMap.TryGetValue(myObject.type, out collection))
                        {
                            Type collectionType;
                            if (myObject.type.IsGenericType && JsonManager.formatterObjectTypeMap.TryGetValue(myObject.type.GetGenericTypeDefinition(), out collectionType))
                            {
                                Type type1 = collectionType.MakeGenericType(myObject.type.GetGenericArguments());
                                JsonManager.formatterAllObjectMap[myObject.type] = collection
                                    = Activator.CreateInstance(type1) as ICollectionObjectBase;
                            }
                            else
                            {
                                myObject.collectionObject = null;
                                myObject.wrapper = GetTypeWrapper(myObject.type);
                                //collection == null
                                if (myObject.isValueType)
                                {
                                    if (parentCollection == null)
                                    {
                                        if (myObject.isProperty)
                                        {
                                            //ToDo Add struct

                                            // myObject.obj = myObject.wrapper.Create(out myObject.gcHandle, out myObject.byteP);
                                            myObject.Obj = myObject.wrapper.Create(out myObject.gcHandle);
                                             

                                            setValues[setValuesIndex] = i;

                                            ++setValuesIndex;
                                        }
                                        else
                                        {
                                            myObject.bytePtr = (byte*)(parentObject.bytePtr + myObject.offset);
                                        }
                                    }
                                    else
                                    {
                                        myObject.Obj = myObject.wrapper.Create(out myObject.gcHandle);
                                        //setValues[setValuesIndex].parentCollection = parentCollection;
                                        setValues[setValuesIndex] = i;
                                        //setValues[setValuesIndex].myObject = myObject;

                                        ++setValuesIndex;


                                    }
                                }
                                else
                                {
                                    myObject.Obj = myObject.wrapper.Create(out myObject.gcHandle);

                                    if (parentCollection == null)
                                    {
                                        if (myObject.isProperty)
                                        {
                                            if (parentObject.isValueType)
                                            {
                                                myObject.propertyDelegateItem.setObject(parentObject.bytePtr, myObject.obj);
                                            }
                                            else
                                            {
                                                myObject.propertyDelegateItem.setObject(parentObject.bytePtrStart, myObject.obj);
                                            }
                                        }
                                        else
                                        {
                                            GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, myObject.obj);
                                        }
                                    }
                                    else
                                    {
                                        parentCollection.Add(parentObject.obj, v, myObject.obj);
                                    }
                                }
                                continue;
                            }
                        }
                        //collection != null
                        myObject.collectionObject = collection;
                        myObject.obj = collection.Create(v, parentObject.obj, myObject.sourceType, parentObject.type);
                        if (collection.IsRef())
                        {
                            if (parentCollection == null)
                            {
                                if (myObject.isProperty)
                                {
                                    if (parentObject.isValueType)
                                    {
                                        myObject.propertyDelegateItem.setObject(parentObject.bytePtr, myObject.obj);
                                    }
                                    else
                                    {
                                        myObject.propertyDelegateItem.setObject(parentObject.bytePtrStart, myObject.obj);
                                    }
                                    myObject.propertyDelegateItem.setObject(parentObject.bytePtr, myObject.obj);
                                    //*myObject.propertyDelegateItem.setTargetPtr = parentObject.bytePtrStart;
                                    //myObject.propertyDelegateItem.setObject(myObject.obj);
                                }
                                else
                                {
                                    GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, myObject.obj);
                                }
                            }
                            else
                            {
                                parentCollection.Add(parentObject.obj, v, myObject.obj);
                            }
                        }
                        else
                        {
                            //setValues[setValuesIndex].parentCollection = parentCollection;
                            setValues[setValuesIndex] = i;
                            //setValues[setValuesIndex].myObject = myObject;
                            //if (parentCollection == null)
                            //{
                            //    //if (myObject.isProperty)
                            //    //{
                            //    //    //setValues[setValuesIndex].propertyDelegateItem = fieldInfo.propertyDelegateItem;
                            //    //}
                            //    //else
                            //    //{
                            //    //    //setValues[setValuesIndex].byteP = parentObject.byteP + myObject.offset;
                            //    //}
                            //}
                            //else
                            //{
                            //    //setValues[setValuesIndex].parentObject = parentObject;
                            //    //parentCollection.Add(parentObject.obj, vs + v->keyStringStart, v->keyStringLength, myObject.obj);
                            //}
                            ++setValuesIndex;
                        }

                    }
                    //array
                    else
                    {
                        ICollectionObjectBase collection;
                        if (!JsonManager.formatterAllObjectMap.TryGetValue(myObject.type, out collection))
                        {
                            if (myObject.type.IsGenericType)
                            {
                                Type collectionType;
                                if (JsonManager.formatterObjectTypeMap.TryGetValue(myObject.type.GetGenericTypeDefinition(), out collectionType))
                                {
                                    Type type1 = collectionType.MakeGenericType(myObject.type.GetGenericArguments());
                                    JsonManager.formatterAllObjectMap[myObject.type] = collection
                                        = Activator.CreateInstance(type1) as ICollectionObjectBase;
                                }
                                else
                                {
                                    if (myObject.type.IsSubclassOf(typeof(MulticastDelegate)))
                                    {
                                        collection = JsonManager.formatterAllObjectMap[typeof(MulticastDelegate)];
                                    }
                                    else
                                    {
                                        throw new Exception("JSON数组类型容器未注册");
                                    }
                                }
                            }
                            else
                            {
                                myObject.collectionObject = null;
                                if (myObject.type.IsArray)
                                {
                                    var rank = myObject.type.GetArrayRank();
                                    if (rank == 1)//数组的秩= 1 直接遍历赋值
                                    {
                                        var elementType = myObject.type.GetElementType();
                                        myObject.ArrayRank = 1;
                                        myObject.ArrayItemType = elementType;
                                        myObject.ArrayItemTypeCode = Type.GetTypeCode(elementType);
                                        myObject.obj = arrayWrapper.CreateArrayOne(elementType, v->arrayCount, out myObject.bytePtr, out myObject.gcHandle, out myObject.ArrayNowItemSize);
                                        myObject.ArrayItemTypeSize = myObject.ArrayNowItemSize;
                                    }
                                    else
                                    {
                                        if (parentObject.type.IsArray && parentObject.ArrayRank > 1)
                                        {
                                            myObject.ArrayRank = parentObject.ArrayRank - 1;
                                            myObject.bytePtr = parentObject.bytePtr + myObject.offset;

                                            myObject.ArrayItemTypeSize = parentObject.ArrayItemTypeSize;
                                            myObject.ArrayNowItemSize = parentObject.ArrayNowItemSize /
                                                v->arrayCount;

                                            myObject.obj = parentObject.obj;


                                            if (myObject.ArrayRank == 1)
                                            {
                                                var elementType = myObject.type.GetElementType();
                                                myObject.ArrayItemType = elementType;
                                                myObject.ArrayItemTypeCode = Type.GetTypeCode(elementType);
                                            }
                                            else
                                            {
                                                myObject.ArrayItemType = myObject.type;
                                            }
                                            continue;
                                        }
                                        else
                                        {
                                            myObject.ArrayRank = rank;
                                            myObject.ArrayItemType = myObject.type;
                                            var elementType = myObject.type.GetElementType();
                                            //myObject.ArrayItemType = elementType;
                                            myObject.ArrayItemTypeCode = Type.GetTypeCode(elementType);


                                            if (rank + i > jsonRender.objectQueueIndex)
                                            {
                                                throw new Exception("无法满足秩");
                                            }

                                            arrayWrapper.SetSize(v->arrayCount);

                                            for (int j = 1; j < rank; j++)
                                            {
                                                JsonObject* v1 = jsonRender.objectQueue + (j + i);
                                                if (v1->parentObjectIndex == j + i - 1 && !v1->isObject)
                                                {
                                                    arrayWrapper.SetSize(v1->arrayCount);
                                                }
                                                else
                                                {
                                                    throw new Exception("无法满足秩");
                                                }
                                            }

                                            myObject.ArrayNowItemSize = arrayWrapper.arraySize / v->arrayCount;

                                            myObject.obj = arrayWrapper.CreateArray(elementType, out myObject.bytePtr, out myObject.gcHandle, out myObject.ArrayItemTypeSize);

                                            myObject.ArrayNowItemSize *= myObject.ArrayItemTypeSize;

                                        }

                                    }
                                }
                                else
                                {
                                    throw new Exception("类型不是数组");
                                }
                                if (parentCollection == null)
                                {
                                    if (myObject.isProperty)
                                    {
                                        if (parentObject.isValueType)
                                        {
                                            myObject.propertyDelegateItem.setObject(parentObject.bytePtr, myObject.obj);
                                        }
                                        else
                                        {
                                            myObject.propertyDelegateItem.setObject(parentObject.bytePtrStart, myObject.obj);
                                        }
                                        //myObject.propertyDelegateItem.setObject(parentObject.bytePtr, myObject.obj);
                                        //*myObject.propertyDelegateItem.setTargetPtr = parentObject.bytePtrStart;
                                        //myObject.propertyDelegateItem.setObject(myObject.obj);
                                    }
                                    else
                                    {
                                        GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, myObject.obj);
                                    }
                                }
                                else
                                {
                                    parentCollection.Add(parentObject.obj, v, myObject.obj);
                                }
                                continue;
                            }
                        }
                        //collection != null
                        myObject.obj = collection.Create(v, parentObject.obj, myObject.sourceType, parentObject.type);
                        myObject.collectionObject = collection;
                        if (collection.IsRef())
                        {
                            if (parentCollection == null)
                            {
                                GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, myObject.obj);
                            }
                            else
                            {
                                parentCollection.Add(parentObject.obj, v, myObject.obj);
                            }
                        }
                        else
                        {
                            //setValues[setValuesIndex].parentCollection = parentCollection;
                            setValues[setValuesIndex] = i;
                            //setValues[setValuesIndex].myObject = myObject;
                            //if (parentCollection == null)
                            //{
                            //    //setValues[setValuesIndex].byteP = parentObject.byteP + myObject.offset;
                            //}
                            //else
                            //{
                            //    //setValues[setValuesIndex].parentObject = parentObject;
                            //    //parentCollection.Add(parentObject.obj, vs + v->keyStringStart, v->keyStringLength, myObject.obj);
                            //}
                            ++setValuesIndex;
                        }
                    }

                }

                //goto Dubg;

                //基本类型赋值  
                for (int i = 0; i < jsonRender.poolIndex; i++)
                {
                    var v = jsonRender.pool[i];
                    CreateObjectItem myObject = createObjectItems[v.objectQueue->objectQueueIndex];
                    JsonObject* parent = v.objectQueue;
                    if (parent->isObject)
                    {
                        ICollectionObjectBase collection = myObject.collectionObject;
                        if (collection != null)
                        {
                            collection.AddValue(myObject.obj, vs, jsonRender.pool + i);
                        }
                        else
                        {
                            //var debug = new string(vs + v.keyStringStart, 0, v.keyStringLength);
                            TypeAddrFieldAndProperty fieldInfo = myObject.wrapper.Find(vs + v.keyStringStart, v.keyStringLength);
                            var itemTypeCode = fieldInfo.typeCode;
                            if (fieldInfo.isProperty)
                            {
                                CreateObjectItem parentObject = createObjectItems[parent->objectQueueIndex];
                                byte* bytePtr;
                                
                                if (parentObject.isValueType)
                                {
                                    bytePtr = myObject.bytePtr;
                                }
                                else
                                {
                                    bytePtr = myObject.bytePtrStart;
                                }

                                //* fieldInfo.propertyDelegateItem.setTargetPtr = bytePtrStart;
                                switch (v.type)
                                {
                                    case JsonValueType.String:
                                        switch (itemTypeCode)
                                        {
                                            case TypeCode.Char:
                                                fieldInfo.propertyDelegateItem.setChar(bytePtr, vs[v.vStringStart]);
                                                break;
                                            case TypeCode.String:
                                                fieldInfo.propertyDelegateItem.setString(bytePtr, jsonRender.EscapeString(vs + v.vStringStart, v.vStringLength));
                                                break;
                                            case TypeCode.Object:
                                                JsonObject* obj = jsonRender.objectQueue;
                                                if (PathToObject(vs + v.vStringStart, v.vStringLength, jsonRender, ref obj))
                                                {
                                                    fieldInfo.propertyDelegateItem.setObject(bytePtr, createObjectItems[obj->objectQueueIndex].obj);
                                                }
                                                break;
                                            default:

                                                if (fieldInfo.isEnum)
                                                {
                                                    var strEnum = new string(vs, v.vStringStart, v.vStringLength);
                                                    Array Arrays = Enum.GetValues(fieldInfo.fieldType);
                                                    for (int k = 0; k < Arrays.Length; k++)
                                                    {
                                                        if (Arrays.GetValue(k).ToString().Equals(strEnum))
                                                        {
                                                            switch (itemTypeCode)
                                                            {
                                                                case TypeCode.SByte:
                                                                    fieldInfo.propertyDelegateItem.setSByte(bytePtr, (sbyte)Arrays.GetValue(k));
                                                                    break;
                                                                case TypeCode.Byte:
                                                                    fieldInfo.propertyDelegateItem.setByte(bytePtr, (byte)Arrays.GetValue(k));
                                                                    break;
                                                                case TypeCode.Int16:
                                                                    fieldInfo.propertyDelegateItem.setInt16(bytePtr, (short)Arrays.GetValue(k));
                                                                    break;
                                                                case TypeCode.UInt16:
                                                                    fieldInfo.propertyDelegateItem.setUInt16(bytePtr, (ushort)Arrays.GetValue(k));
                                                                    break;
                                                                case TypeCode.Int32:
                                                                    fieldInfo.propertyDelegateItem.setInt32(bytePtr, (int)Arrays.GetValue(k));
                                                                    break;
                                                                case TypeCode.UInt32:
                                                                    fieldInfo.propertyDelegateItem.setUInt32(bytePtr, (uint)Arrays.GetValue(k));
                                                                    break;
                                                                case TypeCode.Int64:
                                                                    fieldInfo.propertyDelegateItem.setInt64(bytePtr, (long)Arrays.GetValue(k));
                                                                    break;
                                                                case TypeCode.UInt64:
                                                                    fieldInfo.propertyDelegateItem.setUInt64(bytePtr, (ulong)Arrays.GetValue(k));
                                                                    break;
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                        break;
                                    case JsonValueType.Long:
                                        switch (itemTypeCode)
                                        {
                                            case TypeCode.SByte:
                                                fieldInfo.propertyDelegateItem.setSByte(bytePtr, (SByte)v.valueLong);
                                                break;
                                            case TypeCode.Byte:
                                                fieldInfo.propertyDelegateItem.setByte(bytePtr, (Byte)v.valueLong);
                                                break;
                                            case TypeCode.Int16:
                                                fieldInfo.propertyDelegateItem.setInt16(bytePtr, (Int16)v.valueLong);
                                                break;
                                            case TypeCode.UInt16:
                                                fieldInfo.propertyDelegateItem.setUInt16(bytePtr, (UInt16)v.valueLong);
                                                break;
                                            case TypeCode.Int32:
                                                fieldInfo.propertyDelegateItem.setInt32(bytePtr, (Int32)v.valueLong);
                                                break;
                                            case TypeCode.UInt32:
                                                fieldInfo.propertyDelegateItem.setUInt32(bytePtr, (UInt32)v.valueLong);
                                                break;
                                            case TypeCode.Int64:
                                                fieldInfo.propertyDelegateItem.setInt64(bytePtr, v.valueLong);
                                                break;
                                            case TypeCode.UInt64:
                                                fieldInfo.propertyDelegateItem.setUInt64(bytePtr, (UInt64)v.valueLong);
                                                break;
                                            case TypeCode.Single:
                                                fieldInfo.propertyDelegateItem.setSingle(bytePtr, (Single)v.valueLong);
                                                break;
                                            case TypeCode.Double:
                                                fieldInfo.propertyDelegateItem.setDouble(bytePtr, (Double)v.valueLong);
                                                break;
                                            case TypeCode.Decimal:
                                                fieldInfo.propertyDelegateItem.setDecimal(bytePtr, (Decimal)v.valueLong);
                                                break;
                                        }
                                        break;
                                    case JsonValueType.Double:
                                        switch (itemTypeCode)
                                        {
                                            case TypeCode.Single:
                                                fieldInfo.propertyDelegateItem.setSingle(bytePtr, (Single)v.valueDouble);
                                                break;
                                            case TypeCode.Double:
                                                fieldInfo.propertyDelegateItem.setDouble(bytePtr, v.valueDouble);
                                                break;
                                            case TypeCode.Decimal:
                                                fieldInfo.propertyDelegateItem.setDecimal(bytePtr, (Decimal)v.valueDouble);
                                                break;
                                        }
                                        break;
                                    case JsonValueType.Boolean:
                                        *(bool*)(bytePtr + fieldInfo.offset) = v.valueBool;
                                        break;
                                    case JsonValueType.Object:
                                        if (myObject.type == (typeof(Type)))
                                        {
                                            fieldInfo.propertyDelegateItem.setObject(bytePtr,
                                                UnsafeOperation.GetType(new string(vs, v.vStringStart, v.vStringLength))
                                                );
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                switch (v.type)
                                {
                                    case JsonValueType.String:
                                        switch (itemTypeCode)
                                        {
                                            case TypeCode.Char:
                                                *(char*)(myObject.bytePtr + fieldInfo.offset) = vs[v.vStringStart];
                                                break;
                                            case TypeCode.String:
                                                GeneralTool.SetObject(myObject.bytePtr + fieldInfo.offset, jsonRender.EscapeString(vs + v.vStringStart, v.vStringLength));
                                                break;
                                            case TypeCode.Object:
                                                JsonObject* obj = jsonRender.objectQueue;
                                                if (PathToObject(vs + v.vStringStart, v.vStringLength, jsonRender, ref obj))
                                                {
                                                    GeneralTool.SetObject(myObject.bytePtr + fieldInfo.offset,
                                                      createObjectItems[obj->objectQueueIndex].obj);
                                                }
                                                break;
                                            default:

                                                if (fieldInfo.isEnum)
                                                {
                                                    var strEnum = new string(vs, v.vStringStart, v.vStringLength);
                                                    Array Arrays = Enum.GetValues(fieldInfo.fieldType);
                                                    for (int k = 0; k < Arrays.Length; k++)
                                                    {
                                                        if (Arrays.GetValue(k).ToString().Equals(strEnum))
                                                        {
                                                            GeneralTool.Memcpy(myObject.bytePtr + fieldInfo.offset
                                                                , ((IntPtr*)GeneralTool.ObjectToVoid(Arrays.GetValue(k)) + 1)
                                                                , UnsafeOperation.SizeOfStack(fieldInfo.fieldType)
                                                                );
                                                            break;
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                        break;
                                    case JsonValueType.Long:
                                        switch (itemTypeCode)
                                        {
                                            case TypeCode.SByte:
                                                *(SByte*)(myObject.bytePtr + fieldInfo.offset) = (SByte)v.valueLong;
                                                break;
                                            case TypeCode.Byte:
                                                *(Byte*)(myObject.bytePtr + fieldInfo.offset) = (Byte)v.valueLong;
                                                break;
                                            case TypeCode.Int16:
                                                *(Int16*)(myObject.bytePtr + fieldInfo.offset) = (Int16)v.valueLong;
                                                break;
                                            case TypeCode.UInt16:
                                                *(UInt16*)(myObject.bytePtr + fieldInfo.offset) = (UInt16)v.valueLong;
                                                break;
                                            case TypeCode.Int32:
                                                *(Int32*)(myObject.bytePtr + fieldInfo.offset) = (Int32)v.valueLong;
                                                break;
                                            case TypeCode.UInt32:
                                                *(UInt32*)(myObject.bytePtr + fieldInfo.offset) = (UInt32)v.valueLong;
                                                break;
                                            case TypeCode.Int64:
                                                *(Int64*)(myObject.bytePtr + fieldInfo.offset) = v.valueLong;
                                                break;
                                            case TypeCode.UInt64:
                                                *(UInt64*)(myObject.bytePtr + fieldInfo.offset) = (UInt64)v.valueLong;
                                                break;
                                            case TypeCode.Single:
                                                *(Single*)(myObject.bytePtr + fieldInfo.offset) = (Single)v.valueLong;
                                                break;
                                            case TypeCode.Double:
                                                *(Double*)(myObject.bytePtr + fieldInfo.offset) = (Double)v.valueLong;
                                                break;
                                            case TypeCode.Decimal:
                                                *(Decimal*)(myObject.bytePtr + fieldInfo.offset) = (Decimal)v.valueLong;
                                                break;
                                        }
                                        break;
                                    case JsonValueType.Double:
                                        switch (itemTypeCode)
                                        {
                                            case TypeCode.Single:
                                                *(Single*)(myObject.bytePtr + fieldInfo.offset) = (Single)v.valueDouble;
                                                break;
                                            case TypeCode.Double:
                                                *(Double*)(myObject.bytePtr + fieldInfo.offset) = v.valueDouble;
                                                break;
                                            case TypeCode.Decimal:
                                                *(Decimal*)(myObject.bytePtr + fieldInfo.offset) = (Decimal)v.valueDouble;
                                                break;
                                        }
                                        break;
                                    case JsonValueType.Boolean:
                                        *(bool*)(myObject.bytePtr + fieldInfo.offset) = v.valueBool;
                                        break;
                                    case JsonValueType.Object:
                                        if (myObject.type == (typeof(Type)))
                                        {
                                            GeneralTool.SetObject(myObject.bytePtr + fieldInfo.offset,
                                                UnsafeOperation.GetType(new string(vs, v.vStringStart, v.vStringLength))
                                                );
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        ICollectionObjectBase collection = myObject.collectionObject;
                        if (collection != null)
                        {
                            collection.AddValue(myObject.obj, vs, jsonRender.pool + i);
                        }
                        else
                        {
                            Type itemType;
                            TypeCode itemTypeCode;
                            if (v.typeLength > 0)
                            {
                                string typeName = new string(vs, v.typeStartIndex, v.typeLength);
                                itemType = UnsafeOperation.GetType(typeName);
                                itemTypeCode = Type.GetTypeCode(itemType);
                            }
                            else
                            {
                                itemType = myObject.ArrayItemType;
                                itemTypeCode = myObject.ArrayItemTypeCode;
                            }
                            byte* pByte = myObject.bytePtr + myObject.ArrayItemTypeSize * v.arrayIndex;



                            switch (v.type)
                            {
                                case JsonValueType.String:
                                    switch (itemTypeCode)
                                    {
                                        case TypeCode.Char:
                                            *(char*)(pByte) = vs[v.vStringStart];
                                            break;
                                        case TypeCode.String:
                                            GeneralTool.SetObject(pByte, jsonRender.EscapeString(vs + v.vStringStart, v.vStringLength));
                                            break;
                                        case TypeCode.Object:
                                            JsonObject* obj = jsonRender.objectQueue;
                                            if (PathToObject(vs + v.vStringStart, v.vStringLength, jsonRender, ref obj))
                                            {
                                                GeneralTool.SetObject(pByte,
                                                  createObjectItems[obj->objectQueueIndex].obj);
                                            }
                                            //set_value = PathToObject(vs + v.vStringStart, v.vStringLength, createObjectItems[0]);
                                            break;
                                        default:
                                            if (itemType.IsEnum)
                                            {
                                                var strEnum = new string(vs, v.vStringStart, v.vStringLength);
                                                Array Arrays = Enum.GetValues(myObject.ArrayItemType);
                                                for (int k = 0; k < Arrays.Length; k++)
                                                {
                                                    if (Arrays.GetValue(k).ToString().Equals(strEnum))
                                                    {
                                                        GeneralTool.Memcpy(pByte
                                                            , ((IntPtr*)GeneralTool.ObjectToVoid(Arrays.GetValue(k)) + 1)
                                                            , UnsafeOperation.SizeOfStack(itemType)
                                                            );
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                case JsonValueType.Long:
                                    switch (itemTypeCode)
                                    {
                                        case TypeCode.SByte:
                                            *(SByte*)(pByte) = (SByte)v.valueLong;
                                            break;
                                        case TypeCode.Byte:
                                            *(Byte*)(pByte) = (Byte)v.valueLong;
                                            break;
                                        case TypeCode.Int16:
                                            *(Int16*)(pByte) = (Int16)v.valueLong;
                                            break;
                                        case TypeCode.UInt16:
                                            *(UInt16*)(pByte) = (UInt16)v.valueLong;
                                            break;
                                        case TypeCode.Int32:
                                            *(Int32*)(pByte) = (Int32)v.valueLong;
                                            break;
                                        case TypeCode.UInt32:
                                            *(UInt32*)(pByte) = (UInt32)v.valueLong;
                                            break;
                                        case TypeCode.Int64:
                                            *(Int64*)(pByte) = v.valueLong;
                                            break;
                                        case TypeCode.UInt64:
                                            *(UInt64*)(pByte) = (UInt64)v.valueLong;
                                            break;
                                        case TypeCode.Single:
                                            *(Single*)(pByte) = (Single)v.valueLong;
                                            break;
                                        case TypeCode.Double:
                                            *(Double*)(pByte) = (Double)v.valueLong;
                                            break;
                                        case TypeCode.Decimal:
                                            *(Decimal*)(pByte) = (Decimal)v.valueLong;
                                            break;
                                    }
                                    break;
                                case JsonValueType.Double:
                                    switch (itemTypeCode)
                                    {
                                        case TypeCode.Single:
                                            *(Single*)(pByte) = (Single)v.valueDouble;
                                            break;
                                        case TypeCode.Double:
                                            *(Double*)(pByte) = v.valueDouble;
                                            break;
                                        case TypeCode.Decimal:
                                            *(Decimal*)(pByte) = (Decimal)v.valueDouble;
                                            break;
                                    }
                                    break;
                                case JsonValueType.Boolean:
                                    *(Boolean*)(pByte) = v.valueBool;
                                    break;
                                case JsonValueType.Object:
                                    if (myObject.type == (typeof(Type)))
                                    {
                                        GeneralTool.SetObject(pByte,
                                            UnsafeOperation.GetType(new string(vs, v.vStringStart, v.vStringLength)));
                                    }
                                    break;
                            }
                        }
                    }
                }

                if (setValuesIndex > 0)
                {
                    int indexLeft = 0;
                    int indexRight = setValuesIndex - 1;

                    setValuesOrder[indexRight] = 0;

                    for (int i = 1; i < setValuesIndex; i++)
                    {
                    Loop:
                        //右边没对象，则进入
                        if (indexRight == setValuesIndex)
                        {
                            --indexRight;
                            setValuesOrder[indexRight] = i;
                            continue;
                        }
                        //比较右边如果右边的对象是当前的父对象，则进入右边队伍，否则把右边的对象转移到左边队伍，然后继续比较
                        JsonObject* now = jsonRender.objectQueue + setValues[i];
                        JsonObject* next = jsonRender.objectQueue + setValues[setValuesOrder[indexRight]];
                        if (now->parentObjectIndex >= next->objectQueueIndex)
                        {
                            --indexRight;
                            setValuesOrder[indexRight] = i;
                            continue;
                        }
                        setValuesOrder[indexLeft] = setValuesOrder[indexRight];
                        ++indexLeft;
                        ++indexRight;
                        goto Loop;
                    }



                    //goto Dubg;
                    //类类型赋值
                    for (int j = 0; j < setValuesIndex; j++)
                    //for (int i = setValuesIndex - 1; i >= 0; i--)
                    {
                        //int i = j;
                        int i = setValuesOrder[j];
                        //SetValue setValue = setValues[i];
                        JsonObject* objValue = jsonRender.objectQueue + setValues[i];


                        //CreateObjectItem myObject = setValue.myObject;
                        CreateObjectItem myObject = createObjectItems[objValue->objectQueueIndex];
                        JsonObject* parent = jsonRender.objectQueue + objValue->parentObjectIndex;
                        var parentObject = createObjectItems[objValue->parentObjectIndex];
                        if (myObject.isProperty)
                        {
                            if (parentObject.isValueType)
                            {
                                myObject.propertyDelegateItem.setObject(parentObject.bytePtr, myObject.obj);
                            }
                            else
                            {
                                myObject.propertyDelegateItem.setObject(parentObject.bytePtrStart, myObject.obj);
                            }
                            //try
                            //{
                            //    //*myObject.propertyDelegateItem.setTargetPtr = parentObject.bytePtrStart;
                            //    //myObject.propertyDelegateItem.setObject(myObject.obj);
                            //}
                            //catch (Exception)
                            //{

                            //    throw;
                            //}
                        }
                        else
                        {
                            object over;
                            if (myObject.collectionObject == null)
                            {
                                over = myObject.obj;
                            }
                            else
                            {
                                over = myObject.collectionObject.End(myObject.obj);
                            }

                            if (parentObject.collectionObject == null)
                            {
                                if (myObject.isValueType)
                                {
                                    //var byteP = setValue.parentObject.byteP + myObject.offset;
                                    GeneralTool.Memcpy(parentObject.bytePtr + myObject.offset
                                    , ((IntPtr*)GeneralTool.ObjectToVoid(over) + 1)
                                    , UnsafeOperation.SizeOfStack(myObject.type)
                                    );
                                }
                                else
                                {
                                    GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, over);
                                }
                            }
                            else
                            {
                                parentObject.collectionObject.Add(parentObject.obj, objValue, over);
                            }
                        }

                    }
                }

            }


        Dubg:
            for (int i = 0; i < itemCount; i++)
            {
                if (createObjectItems[i].gcHandle != default(GCHandle))
                {
                    createObjectItems[i].gcHandle.Free();
                    createObjectItems[i].gcHandle = default(GCHandle);
                }
            }
            //GC.KeepAlive(createObjectItems);
            //for (int i = 0; i < itemCount; i++)
            //{
            //    if (createObjectItems[i].obj != null)
            //    {
            //        GC.KeepAlive(createObjectItems[i].obj);
            //        //GC.KeepAlive(createObjectItems[i]);
            //    }
            //}
            //GC.KeepAlive(createObjectItems);
            return rootItem.obj;
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe bool PathToObject(char* path, int pathLength, JsonRender jsonRender, ref JsonObject* obj)
        {
            JsonObject* parent = obj;
            //JsonObject* now = jsonRender.objectQueue + obj->objectQueueIndex + 1;
            ++obj;
        Start:
            if (parent->isObject)
            {
                int keySize = pathLength;
                for (int i = 0; i < pathLength; i++)
                {
                    if (*(path + i) == '/')
                    {
                        keySize = i;
                        break;
                    }
                }

            ObjectStart:
                if (keySize != obj->keyStringLength)
                {
                    goto False;
                }

                char* now = obj->keyStringStart;

                for (int i = 0; i < keySize; i++)
                {
                    if (*(path + i) == *(now + i))
                    {
                    }
                    else
                    {
                        goto False;
                    }
                }

                //true
                pathLength -= keySize + 1;
                if (pathLength <= 0)
                {
                    return true;
                }
                path += keySize;
                if (*path == '/')
                {
                    if (obj->objectQueueIndex + 1 >= jsonRender.objectQueueIndex)
                    {
                        return false;
                    }
                    parent = obj;
                    ++obj;
                    ++path;
                    goto Start;
                }
            False:
                //寻找下一个
                if (obj->objectNext >= jsonRender.objectQueueIndex)
                {
                    return false;
                }
                var next = jsonRender.objectQueue + obj->objectNext;
                //如果下一个的父对象不是和之前的一样
                //next->keyStringStart == null || 
                if (next->parentObjectIndex != obj->parentObjectIndex)
                {
                    return false;
                }
                obj = next;
                goto ObjectStart;
            }
            else
            {
                int pathIndex = 0;
                int pathSize = 0;
                for (int i = 0; i < pathLength; i++)
                {
                    if (*(path + i) == '/')
                    {
                        break;
                    }
                    if (*path < '0' || *path > '9')
                    {
                        return false;
                    }
                    else
                    {
                        ++pathSize;
                        pathIndex *= 10;
                        pathIndex += (*path - '0');
                    }
                }

                if (pathIndex >= parent->arrayCount)
                {
                    return false;
                }
                for (int i = 0; i < pathIndex; i++)
                {
                    obj = jsonRender.objectQueue + obj->objectNext;
                }
                path += pathSize + 1;
                pathLength -= pathSize + 1;
                if (pathLength <= 0)
                {
                    return true;
                }
                parent = obj;
                ++obj;
                goto Start;
            }

        }




    }
}
