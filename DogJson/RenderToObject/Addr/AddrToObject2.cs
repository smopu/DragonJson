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
        Dictionary<Type, TypeAddrReflectionWarp> allTypeWarp = new Dictionary<Type, TypeAddrReflectionWarp>();
        public TypeAddrReflectionWarp GetTypeWarp(Type type)
        {
            TypeAddrReflectionWarp ob;
            if (allTypeWarp.TryGetValue(type, out ob))
            {
                return ob;
            }
            return allTypeWarp[type] = new TypeAddrReflectionWarp(type);
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

            public TypeAddrReflectionWarp warp;
            public int offset;
            public GCHandle gcHandle;
            public byte* byteP;
            public object obj;

            public TypeCode ArrayItemTypeCode;
            public Type ArrayItemType; 
            public int ArrayRank; 
            public int ArrayItemTypeSize;
            public int ArrayNowItemSize;


            public object Obj
            {
               // get { return GeneralTool.VoidToObject(byteP - TypeAddrReflectionWarp.PTR_COUNT); }
                get { return obj; }
                set
                {
                    obj = value;
                    byteP = (byte*)GeneralTool.ObjectToVoid(value) + UnsafeOperation.PTR_COUNT;
                }
            }
        }

        struct SetValue {
            public JsonObject* objValue;
            public void* byteP;
            public CreateObjectItem myObject; 
            public ICollectionObjectBase parentCollection; 
            public CreateObjectItem parentObject;
        }

        int setValuesIndex = 0;
        ArrayWarp arrayWarp = new ArrayWarp();
        SetValue[] setValues = new SetValue[1024];
        CreateObjectItem[] createObjectItems = new CreateObjectItem[1024];



        public static int indexDbug = 0;
        public unsafe object CreateObject(JsonRender jsonRender, Type type, char* vs, int length)
        {
            setValuesIndex = 0;
            var rootItem = createObjectItems[0];
            int itemCount = jsonRender.objectQueueIndex;
            {
                rootItem.warp = GetTypeWarp(type);
                rootItem.type = type;
                rootItem.Obj = rootItem.warp.Create(out rootItem.gcHandle);//out rootItem.gcHandle
                rootItem.isValueType = type.IsValueType;

                //对象数组创建
                for (int i = 1; i < jsonRender.objectQueueIndex; i++)
                {
                    CreateObjectItem myObject = createObjectItems[i];
                    JsonObject* v = jsonRender.objectQueue + i;
                    JsonObject* parent = jsonRender.objectQueue + v->parentObjectIndex;
                    CreateObjectItem parentObject = createObjectItems[v->parentObjectIndex];
                    ICollectionObjectBase parentCollection = parentObject.collectionObject;
                    TypeAddrField fieldInfo = null;
                    if (parent->isObject)
                    {
                        if (parentCollection == null)
                        {
                            fieldInfo = parentObject.warp.Find(v->keyStringStart, v->keyStringLength);
                            //string key = new string(vs, v->keyStringStart, v->keyStringLength);
                            //var fieldInfo = parentObject.warp.nameOfField[key];
                            myObject.offset = fieldInfo.offset;
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
                        myObject.type = typeof(ConstructorWarp);
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
                                myObject.warp = GetTypeWarp(myObject.type);
                                if (myObject.isValueType)
                                {
                                    if (parentCollection == null)
                                    {
                                        myObject.byteP = (byte*)(parentObject.byteP + myObject.offset);// +TypeAddrReflectionWarp.PTR_COUNT;
                                    }
                                    else
                                    {
                                        myObject.Obj = myObject.warp.Create(out myObject.gcHandle);
                                        setValues[setValuesIndex].parentCollection = parentCollection;
                                        setValues[setValuesIndex].objValue = v;
                                        setValues[setValuesIndex].myObject = myObject;
                                        setValues[setValuesIndex].parentObject = parentObject;
                                        ++setValuesIndex;
                                    }
                                }
                                else
                                {
                                   myObject.Obj = myObject.warp.Create(out myObject.gcHandle);

                                    if (parentCollection == null)
                                    {
                                        GeneralTool.SetObject(parentObject.byteP + myObject.offset, myObject.obj);
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
                                GeneralTool.SetObject(parentObject.byteP + myObject.offset, myObject.obj);
                            }
                            else
                            {
                                parentCollection.Add(parentObject.obj, v, myObject.obj);
                            }
                        }
                        else
                        {
                            setValues[setValuesIndex].parentCollection = parentCollection;
                            setValues[setValuesIndex].objValue = v;
                            setValues[setValuesIndex].myObject = myObject;
                            if (parentCollection == null)
                            {
                                setValues[setValuesIndex].byteP = parentObject.byteP + myObject.offset;
                            }
                            else
                            {
                                setValues[setValuesIndex].parentObject = parentObject;
                                //parentCollection.Add(parentObject.obj, vs + v->keyStringStart, v->keyStringLength, myObject.obj);
                            }
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
                                        myObject.obj = arrayWarp.CreateArrayOne(elementType, v->arrayCount, out myObject.byteP, out myObject.gcHandle, out myObject.ArrayNowItemSize);
                                        myObject.ArrayItemTypeSize = myObject.ArrayNowItemSize;
                                    }
                                    else
                                    {
                                        if (parentObject.type.IsArray && parentObject.ArrayRank > 1)
                                        {
                                            myObject.ArrayRank = parentObject.ArrayRank - 1;
                                            myObject.byteP = parentObject.byteP + myObject.offset;

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

                                            arrayWarp.SetSize(v->arrayCount);

                                            for (int j = 1; j < rank; j++)
                                            {
                                                JsonObject* v1 = jsonRender.objectQueue + (j + i);
                                                if (v1->parentObjectIndex == j + i - 1 && !v1->isObject)
                                                {
                                                    arrayWarp.SetSize(v1->arrayCount);
                                                }
                                                else
                                                {
                                                    throw new Exception("无法满足秩");
                                                }
                                            }

                                            myObject.ArrayNowItemSize = arrayWarp.arraySize / v->arrayCount;

                                            myObject.obj = arrayWarp.CreateArray(elementType, out myObject.byteP, out myObject.gcHandle, out myObject.ArrayItemTypeSize);

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
                                    GeneralTool.SetObject(parentObject.byteP + myObject.offset, myObject.obj);
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
                                GeneralTool.SetObject(parentObject.byteP + myObject.offset, myObject.obj);
                            }
                            else
                            {
                                parentCollection.Add(parentObject.obj, v, myObject.obj);
                            }
                        }
                        else
                        {
                            setValues[setValuesIndex].parentCollection = parentCollection;
                            setValues[setValuesIndex].objValue = v;
                            setValues[setValuesIndex].myObject = myObject;
                            if (parentCollection == null)
                            {
                                setValues[setValuesIndex].byteP = parentObject.byteP + myObject.offset;
                            }
                            else
                            {
                                setValues[setValuesIndex].parentObject = parentObject;
                                //parentCollection.Add(parentObject.obj, vs + v->keyStringStart, v->keyStringLength, myObject.obj);
                            }
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
                            TypeAddrField fieldInfo = myObject.warp.Find(vs + v.keyStringStart, v.keyStringLength);
                            var itemTypeCode = fieldInfo.typeCode;
                            switch (v.type) 
                            {
                                case JsonValueType.String:
                                    switch (itemTypeCode)
                                    {
                                        case TypeCode.Char:
                                            *(char*)(myObject.byteP + fieldInfo.offset) = vs[v.vStringStart];
                                            break;
                                        case TypeCode.String:
                                            var str = new string(vs, v.vStringStart, v.vStringLength);
                                            //gCHandles.Add(GCHandle.Alloc(str, GCHandleType.Pinned));
                                            GeneralTool.SetObject(myObject.byteP + fieldInfo.offset, str);
                                            break;
                                        case TypeCode.Object:
                                            JsonObject* obj = jsonRender.objectQueue;
                                            if (PathToObject(vs + v.vStringStart, v.vStringLength, jsonRender, ref obj))
                                            {
                                                GeneralTool.SetObject(myObject.byteP + fieldInfo.offset,
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
                                                        GeneralTool.Memcpy(myObject.byteP + fieldInfo.offset
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
                                            *(SByte*)(myObject.byteP + fieldInfo.offset) = (SByte)v.valueLong;
                                            break;
                                        case TypeCode.Byte:
                                            *(Byte*)(myObject.byteP + fieldInfo.offset) = (Byte)v.valueLong;
                                            break;
                                        case TypeCode.Int16:
                                            *(Int16*)(myObject.byteP + fieldInfo.offset) = (Int16)v.valueLong;
                                            break;
                                        case TypeCode.UInt16:
                                            *(UInt16*)(myObject.byteP + fieldInfo.offset) = (UInt16)v.valueLong;
                                            break;
                                        case TypeCode.Int32:
                                            *(Int32*)(myObject.byteP + fieldInfo.offset) = (Int32)v.valueLong;
                                            break;
                                        case TypeCode.UInt32:
                                            *(UInt32*)(myObject.byteP + fieldInfo.offset) = (UInt32)v.valueLong;
                                            break;
                                        case TypeCode.Int64:
                                            *(Int64*)(myObject.byteP + fieldInfo.offset) = v.valueLong;
                                            break;
                                        case TypeCode.UInt64:
                                            *(UInt64*)(myObject.byteP + fieldInfo.offset) = (UInt64)v.valueLong;
                                            break;
                                        case TypeCode.Single:
                                            *(Single*)(myObject.byteP + fieldInfo.offset) = (Single)v.valueLong;
                                            break;
                                        case TypeCode.Double:
                                            *(Double*)(myObject.byteP + fieldInfo.offset) = (Double)v.valueLong;
                                            break;
                                        case TypeCode.Decimal:
                                            *(Decimal*)(myObject.byteP + fieldInfo.offset) = (Decimal)v.valueLong;
                                            break;
                                    }
                                    break;
                                case JsonValueType.Double:
                                    switch (itemTypeCode)
                                    {
                                        case TypeCode.Single:
                                            *(Single*)(myObject.byteP + fieldInfo.offset) = (Single)v.valueDouble;
                                            break;
                                        case TypeCode.Double:
                                            *(Double*)(myObject.byteP + fieldInfo.offset) = v.valueDouble;
                                            break;
                                        case TypeCode.Decimal:
                                            *(Decimal*)(myObject.byteP + fieldInfo.offset) = (Decimal)v.valueDouble;
                                            break;
                                    }
                                    break;
                                case JsonValueType.Boolean:
                                    *(bool*)(myObject.byteP + fieldInfo.offset) = v.valueBool;
                                    break;
                                case JsonValueType.Object:
                                    if (myObject.type == (typeof(Type)))
                                    {
                                        GeneralTool.SetObject(myObject.byteP + fieldInfo.offset,
                                            UnsafeOperation.GetType(new string(vs, v.vStringStart, v.vStringLength))
                                            );
                                    }
                                    break;
                                default:
                                    break;
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
                            byte* pByte = myObject.byteP + myObject.ArrayItemTypeSize * v.arrayIndex;

                          

                            switch (v.type)
                            {
                                case JsonValueType.String:
                                    switch (itemTypeCode)
                                    {
                                        case TypeCode.Char:
                                            *(char*)(pByte) = vs[v.vStringStart];
                                            break;
                                        case TypeCode.String:
                                            var str = new string(vs, v.vStringStart, v.vStringLength);
                                            GeneralTool.SetObject(pByte, str);
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

                //goto Dubg;
                //类类型赋值
                for (int i = setValuesIndex - 1; i >= 0; i--)
                {
                    SetValue setValue = setValues[i];
                    CreateObjectItem myObject = setValue.myObject;
                    JsonObject* parent = jsonRender.objectQueue + setValue.objValue->parentObjectIndex;

                    object over;
                    if (myObject.collectionObject == null)
                    {
                        over = myObject.obj;
                    }
                    else
                    {
                        over = myObject.collectionObject.End(myObject.obj);
                    }

                    if (setValues[i].parentCollection == null)
                    {
                        if (myObject.isValueType)
                        {
                            GeneralTool.Memcpy(setValue.byteP
                            , ((IntPtr*)GeneralTool.ObjectToVoid(over) + 1)
                            , UnsafeOperation.SizeOfStack(myObject.type)
                            );
                        }
                        else
                        {
                            GeneralTool.SetObject(setValue.byteP, over);
                        }
                    }
                    else
                    {
                        setValue.parentCollection.Add(setValue.parentObject.obj, setValue.objValue, over);
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
