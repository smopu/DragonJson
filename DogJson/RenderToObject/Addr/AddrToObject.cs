//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace DogJson
//{
//    public unsafe class AddrToObject : IJsonRenderToObject
//    {
//        Dictionary<Type, TypeAddrReflectionWrapper> allTypeWrapper = new Dictionary<Type, TypeAddrReflectionWrapper>();
//        public TypeAddrReflectionWrapper GetTypeWrapper(Type type)
//        {
//            TypeAddrReflectionWrapper ob;
//            if (allTypeWrapper.TryGetValue(type, out ob))
//            {
//                return ob;
//            }
//            return allTypeWrapper[type] = new TypeAddrReflectionWrapper(type);
//        }

//        public class CreateObjectItem
//        {
//            public CreateObjectItem(int index)
//            {
//                this.index = index;
//            }
//            public GCHandle gcHandle;
//            public object obj;
//            public void* voidP;
//            public byte* byteP;
//            public object Obj
//            {
//                get { return obj; }
//                set
//                {
//                    obj = value;
//                    voidP = GeneralTool.ObjectToVoid(obj);
//                    byteP = (byte*)voidP + sizeof(IntPtr);
//                    voidP = (void*)(byteP);
//                }
//            }

//            public Type type;
//            public Type sourceType;
//            public TypeAddrFieldAndProperty fieldInfo;
//            public string key;
//            public Array objArray;
//            public TypeCode ArrayItemTypeCode;
//            public Type ArrayItemType;
//            public int ArrayRank;
//            public int[] ArrayRankLengths;
//            public int[] ArrayRankIndex;

//            public JsonObject jsonObject;

//            public List<CreateObjectItem> sub = new List<CreateObjectItem>();
//            public bool isValueType;
//            public int index;
//            public IReadCollectionObject collectionArray;
//            public IReadCollectionObject collectionObject;
//            public TypeAddrReflectionWrapper wrapper;

//        }

//        static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
//        public unsafe object CreateObject(JsonRender jsonRender, Type type, char* vs, int length)
//        {
//            CreateObjectItem[] createObjectItems = new CreateObjectItem[jsonRender.objectQueueIndex];
//            {
//                var rootItem = createObjectItems[0] = new CreateObjectItem(0);
//                rootItem.wrapper = GetTypeWrapper(type);
//                rootItem.type = type;
//                rootItem.Obj = rootItem.wrapper.Create(out rootItem.gcHandle);
//                //rootItem.gcHandle.Free();
//                rootItem.isValueType = type.IsValueType;
//                rootItem.jsonObject = jsonRender.objectQueue[0];
//                //对象数组创建
//                for (int i = 1; i < jsonRender.objectQueueIndex; i++)
//                {
//                    CreateObjectItem myObject = createObjectItems[i] = new CreateObjectItem(i);
//                    JsonObject v = jsonRender.objectQueue[i];
//                    JsonObject parent = jsonRender.objectQueue[v.parentObjectIndex];
//                    CreateObjectItem parentObject = createObjectItems[v.parentObjectIndex];
//                    createObjectItems[i].jsonObject = jsonRender.objectQueue[i];

//                    parentObject.sub.Add(myObject);
//                    if (parent.isObject)
//                    {
//                        IReadCollectionObject collectionParent = parentObject.collectionObject;
//                        if (collectionParent == null)
//                        {
//                            //var fieldInfo = parentObject.wrapper.Find(vs + v.keyStringStart, v.keyStringLength);
//                            string key = new string(v.keyStringStart, 0, v.keyStringLength);
//                            myObject.key = key;
//                            var fieldInfo = parentObject.wrapper.nameOfField[key];

//                            myObject.fieldInfo = fieldInfo;
//                            if (v.isCommandValue)
//                            {
//                                string typeName = new string(vs, v.typeStartIndex, v.typeLength);
//                                var valueType = Type.GetType(typeName);
//                                myObject.type = typeof(Box<>).MakeGenericType(valueType);
//                            }
//                            else
//                            {
//                                if (v.typeLength > 0)
//                                {
//                                    string typeName = new string(vs, v.typeStartIndex, v.typeLength);
//                                    myObject.type = Type.GetType(typeName);
//                                }
//                                else
//                                {
//                                    myObject.type = fieldInfo.fieldType;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            if (v.typeLength > 0)
//                            {
//                                myObject.type = Type.GetType(new string(vs, v.typeStartIndex, v.typeLength));
//                            }
//                            else
//                            {
//                                myObject.type = collectionParent.GetItemType(&v);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        if (v.typeLength > 0)
//                        {
//                            if (v.isCommandValue)
//                            {
//                                string typeName = new string(vs, v.typeStartIndex, v.typeLength);
//                                var valueType = Type.GetType(typeName);
//                                myObject.type = typeof(Box<>).MakeGenericType(valueType);
//                            }
//                            else
//                            {
//                                myObject.type = Type.GetType(new string(vs, v.typeStartIndex, v.typeLength));
//                            }
//                        }
//                        else
//                        {
//                            IReadCollectionObject collectionParent = parentObject.collectionArray;
//                            if (collectionParent == null)
//                            {
//                                myObject.type = parentObject.ArrayItemType;
//                            }
//                            else
//                            {
//                                myObject.type = collectionParent.GetItemType(&v);
//                            }
//                        }
//                    }

//                    myObject.sourceType = myObject.type;
//                    //"$create"
//                    if (v.isConstructor)
//                    {
//                        myObject.type = typeof(ConstructorWrapper);
//                    }

//                    if (v.isObject)
//                    {
//                        IReadCollectionObject collection;
//                        if (CollectionManager.readObjectMap.TryGetValue(myObject.type, out collection))
//                        {
//                            myObject.Obj = collection.Create(&v, parentObject.obj, myObject.sourceType, parentObject.type);
//                            myObject.collectionObject = collection;
//                        }
//                        else
//                        {
//                            Type collectionType;
//                            if (myObject.type.IsGenericType && CollectionManager.readObjectTypeMap.TryGetValue(myObject.type.GetGenericTypeDefinition(), out collectionType))
//                            {
//                                Type type1 = collectionType.MakeGenericType(myObject.type.GetGenericArguments());
//                                CollectionManager.readObjectMap[myObject.type] = collection
//                                    = Activator.CreateInstance(type1) as IReadCollectionObject;

//                                myObject.Obj = collection.Create(&v, parentObject.obj, myObject.type, parentObject.type);
//                                myObject.collectionObject = collection;
//                            }
//                            else
//                            {
//                                myObject.wrapper = GetTypeWrapper(myObject.type);
//                                //if (myObject.isValueType)
//                                //{
//                                //    parentObject
//                                //}
//                                //else
//                                //{
//                                //    myObject.Obj = myObject.wrapper.Create(out myObject.gcHandle);
//                                //}
//                                myObject.Obj = myObject.wrapper.Create(out myObject.gcHandle);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        IReadCollectionObject collection;
//                        if (CollectionManager.readObjectMap.TryGetValue(myObject.type, out collection))
//                        {
//                            myObject.Obj = collection.Create(&v, parentObject.obj, myObject.type, parentObject.type);
//                            myObject.collectionArray = collection;
//                        }
//                        else
//                        {
//                            if (myObject.type.IsGenericType)
//                            {
//                                Type collectionType;
//                                if (CollectionManager.readObjectTypeMap.TryGetValue(myObject.type.GetGenericTypeDefinition(), out collectionType))
//                                {
//                                    Type type1 = collectionType.MakeGenericType(myObject.type.GetGenericArguments());
//                                    CollectionManager.readObjectMap[myObject.type] = collection
//                                        = Activator.CreateInstance(type1) as IReadCollectionObject;

//                                    myObject.Obj = collection.Create(&v, parentObject.obj, myObject.type, parentObject.type);
//                                    myObject.collectionArray = collection;
//                                }
//                                else
//                                {
//                                    if (myObject.type.IsSubclassOf(typeof(MulticastDelegate)))
//                                    {
//                                        collection = CollectionManager.readObjectMap[typeof(MulticastDelegate)];
//                                        myObject.Obj = collection.Create(&v, parentObject.obj, myObject.type, parentObject.type);
//                                        myObject.collectionArray = collection;
//                                    }
//                                    else
//                                    {
//                                        //myObject.obj = Activator.CreateInstance(myObject.type);
//                                        throw new Exception("JSON数组类型容器未注册");
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                if (myObject.type.IsArray)
//                                {
//                                    var rank = myObject.type.GetArrayRank();
//                                    //var lengths = myObject.type.getl();
//                                    //Array array = Array.CreateInstance(elementType, lengths);
//                                    var elementType = myObject.type.GetElementType();
//                                    if (rank == 1)//数组的秩= 1 直接遍历赋值
//                                    {
//                                        myObject.ArrayRank = 1;
//                                        myObject.ArrayItemType = elementType;
//                                        myObject.ArrayItemTypeCode = Type.GetTypeCode(elementType);
//                                        myObject.Obj = myObject.objArray = Array.CreateInstance(elementType, v.arrayCount);
//                                    }
//                                    else
//                                    {
//                                        if (parentObject.type.IsArray && parentObject.ArrayRank > 1)
//                                        {
//                                            myObject.ArrayRank = parentObject.ArrayRank - 1;
//                                            myObject.ArrayRankLengths = parentObject.ArrayRankLengths;
//                                            myObject.ArrayRankIndex = new int[parentObject.ArrayRankIndex.Length];
//                                            parentObject.ArrayRankIndex.CopyTo(myObject.ArrayRankIndex, 0);


//                                            myObject.ArrayRankIndex[parentObject.ArrayRankIndex.Length - parentObject.ArrayRank] = v.arrayIndex;

//                                            myObject.Obj = myObject.objArray = parentObject.objArray;
//                                            if (myObject.ArrayRank == 1)
//                                            {
//                                                myObject.ArrayItemType = elementType;
//                                                myObject.ArrayItemTypeCode = Type.GetTypeCode(elementType);
//                                            }
//                                            else
//                                            {
//                                                myObject.ArrayItemType = myObject.type;
//                                            }
//                                        }
//                                        else
//                                        {
//                                            myObject.ArrayRank = rank;
//                                            myObject.ArrayItemType = myObject.type;
//                                            //myObject.ArrayItemType = elementType;
//                                            myObject.ArrayItemTypeCode = Type.GetTypeCode(elementType);
//                                            myObject.ArrayRankLengths = new int[rank];

//                                            if (rank + i > jsonRender.objectQueueIndex)
//                                            {
//                                                throw new Exception("无法满足秩");
//                                            }

//                                            myObject.ArrayRankLengths[0] = v.arrayCount;
//                                            for (int j = 1; j < rank; j++)
//                                            {
//                                                JsonObject v1 = jsonRender.objectQueue[j + i];
//                                                if (v1.parentObjectIndex == j + i - 1 && !v1.isObject)
//                                                {
//                                                    myObject.ArrayRankLengths[j] = v1.arrayCount;
//                                                }
//                                                else
//                                                {
//                                                    throw new Exception("无法满足秩");
//                                                }
//                                            }
//                                            myObject.Obj = myObject.objArray = Array.CreateInstance(elementType, myObject.ArrayRankLengths);
//                                            myObject.ArrayRankIndex = new int[rank];
//                                        }

//                                    }
//                                }
//                                else
//                                {
//                                    throw new Exception("类型不是数组");
//                                }
//                            }
//                        }
//                    }
//                }


//                object set_value = null;
//                //基本类型赋值
//                for (int i = 0; i < jsonRender.poolIndex; i++)
//                {
//                    var v = jsonRender.pool[i];
//                    CreateObjectItem myObject = createObjectItems[v.objectQueue->objectQueueIndex];
//                    JsonObject* parent = v.objectQueue;
//                    if (parent->isObject)
//                    {
//                        IReadCollectionObject collection = myObject.collectionObject;
//                        if (collection != null)
//                        {
//                            collection.AddValue(myObject.obj, vs, &v);
//                        }
//                        else
//                        {
//                            var key = new string(vs, v.keyStringStart, v.keyStringLength);
//                            //TypeAddrField fieldInfo = myObject.wrapper.Find(vs + v.keyStringStart, v.keyStringLength);
//                            TypeAddrFieldAndProperty fieldInfo = myObject.wrapper.nameOfField[key];
//                            var itemTypeCode = fieldInfo.typeCode;
//                            switch (v.type)
//                            {
//                                case JsonValueType.String:
//                                    switch (itemTypeCode)
//                                    {
//                                        case TypeCode.Char:
//                                            *(char*)(myObject.byteP + fieldInfo.offset) = vs[v.vStringStart];
//                                            break;
//                                        case TypeCode.String:
//                                            GeneralTool.SetObject(myObject.byteP + fieldInfo.offset,
//                                                new string(vs, v.vStringStart, v.vStringLength));
//                                            break;
//                                        case TypeCode.Object:
//                                            GeneralTool.SetObject(myObject.byteP + fieldInfo.offset,
//                                                PathToObject(vs + v.vStringStart, v.vStringLength, createObjectItems[0]));
//                                            break;
//                                        default:
                                            
//                                            if (fieldInfo.isEnum)
//                                            {
//                                                var strEnum = new string(vs, v.vStringStart, v.vStringLength);
//                                                Array Arrays = Enum.GetValues(fieldInfo.fieldType);
//                                                for (int k = 0; k < Arrays.Length; k++)
//                                                {
//                                                    if (Arrays.GetValue(k).ToString().Equals(strEnum))
//                                                    {
//                                                        GeneralTool.Memcpy(myObject.byteP + fieldInfo.offset
//                                                        , ((IntPtr*)GeneralTool.ObjectToVoid(Arrays.GetValue(k)) + 1)
//                                                        , UnsafeOperation.SizeOfStack(fieldInfo.fieldType)
//                                                        );
//                                                    }
//                                                }
//                                            }
//                                            break;
//                                    }
//                                    break;
//                                case JsonValueType.Long:
//                                    switch (itemTypeCode)
//                                    {
//                                        case TypeCode.SByte:
//                                            *(SByte*)(myObject.byteP + fieldInfo.offset) = (SByte)v.valueLong;
//                                            break;
//                                        case TypeCode.Byte:
//                                            *(Byte*)(myObject.byteP + fieldInfo.offset) = (Byte)v.valueLong;
//                                            break;
//                                        case TypeCode.Int16:
//                                            *(Int16*)(myObject.byteP + fieldInfo.offset) = (Int16)v.valueLong;
//                                            break;
//                                        case TypeCode.UInt16:
//                                            *(UInt16*)(myObject.byteP + fieldInfo.offset) = (UInt16)v.valueLong;
//                                            break;
//                                        case TypeCode.Int32:
//                                            *(Int32*)(myObject.byteP + fieldInfo.offset) = (Int32)v.valueLong;
//                                            break;
//                                        case TypeCode.UInt32:
//                                            *(UInt32*)(myObject.byteP + fieldInfo.offset) = (UInt32)v.valueLong;
//                                            break;
//                                        case TypeCode.Int64:
//                                            *(Int64*)(myObject.byteP + fieldInfo.offset) = v.valueLong;
//                                            break;
//                                        case TypeCode.UInt64:
//                                            *(UInt64*)(myObject.byteP + fieldInfo.offset) = (UInt64)v.valueLong;
//                                            break;
//                                        case TypeCode.Single:
//                                            *(Single*)(myObject.byteP + fieldInfo.offset) = (Single)v.valueLong;
//                                            break;
//                                        case TypeCode.Double:
//                                            *(Double*)(myObject.byteP + fieldInfo.offset) = (Double)v.valueLong;
//                                            break;
//                                        case TypeCode.Decimal:
//                                            *(Decimal*)(myObject.byteP + fieldInfo.offset) = (Decimal)v.valueLong;
//                                            break;
//                                    }
//                                    break;
//                                case JsonValueType.Double:
//                                    switch (itemTypeCode)
//                                    {
//                                        case TypeCode.Single:
//                                            *(Single*)(myObject.byteP + fieldInfo.offset) = (Single)v.valueDouble;
//                                            break;
//                                        case TypeCode.Double:
//                                            *(Double*)(myObject.byteP + fieldInfo.offset) = v.valueDouble;
//                                            break;
//                                        case TypeCode.Decimal:
//                                            *(Decimal*)(myObject.byteP + fieldInfo.offset) = (Decimal)v.valueDouble;
//                                            break;
//                                    }
//                                    break;
//                                case JsonValueType.Boolean:
//                                    *(bool*)(myObject.byteP + fieldInfo.offset) = v.valueBool;
//                                    break;
//                                case JsonValueType.Object:
//                                    if (myObject.type == (typeof(Type)))
//                                    {
//                                        GeneralTool.SetObject(myObject.byteP + fieldInfo.offset,
//                                            Type.GetType(new string(vs, v.vStringStart, v.vStringLength))
//                                            );

//                                        //fieldInfo.SetValue(
//                                        //    myObject.Obj, Type.GetType(new string(vs, v.vStringStart, v.vStringLength)
//                                        //    ));
//                                    }
//                                    break;
//                                default:
//                                    break;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        IReadCollectionObject collection = myObject.collectionArray;
//                        if (collection != null)
//                        {
//                            collection.AddValue(myObject.Obj, vs, &v);
//                        }
//                        else
//                        {
//                            Type itemType;
//                            TypeCode itemTypeCode;
//                            if (v.typeLength > 0)
//                            {
//                                string typeName = new string(vs, v.typeStartIndex, v.typeLength);
//                                itemType = Type.GetType(typeName);
//                                itemTypeCode = Type.GetTypeCode(itemType);
//                            }
//                            else
//                            {
//                                itemType = myObject.ArrayItemType;
//                                itemTypeCode = myObject.ArrayItemTypeCode;
//                            }
//                            switch (v.type)
//                            {
//                                case JsonValueType.String:
//                                    switch (itemTypeCode)
//                                    {
//                                        case TypeCode.Char:
//                                            set_value = vs[v.vStringStart];
//                                            break;
//                                        case TypeCode.String:
//                                            set_value = new string(vs, v.vStringStart, v.vStringLength);
//                                            break;
//                                        default:
//                                            if (myObject.ArrayItemType.IsEnum)
//                                            {
//                                                var strEnum = new string(vs, v.vStringStart, v.vStringLength);
//                                                Array Arrays = Enum.GetValues(myObject.ArrayItemType);
//                                                for (int k = 0; k < Arrays.Length; k++)
//                                                {
//                                                    if (Arrays.GetValue(k).ToString().Equals(strEnum))
//                                                    {
//                                                        set_value = Arrays.GetValue(k);
//                                                    }
//                                                }
//                                            }
//                                            else
//                                            {
//                                                set_value = PathToObject(vs + v.vStringStart, v.vStringLength, createObjectItems[0]);
//                                            }
//                                            break;
//                                    }
//                                    break;
//                                case JsonValueType.Long:
//                                    switch (itemTypeCode)
//                                    {
//                                        case TypeCode.SByte:
//                                            set_value = (SByte)v.valueLong;
//                                            break;
//                                        case TypeCode.Byte:
//                                            set_value = (Byte)v.valueLong;
//                                            break;
//                                        case TypeCode.Int16:
//                                            set_value = (Int16)v.valueLong;
//                                            break;
//                                        case TypeCode.UInt16:
//                                            set_value = (UInt16)v.valueLong;
//                                            break;
//                                        case TypeCode.Int32:
//                                            set_value = (Int32)v.valueLong;
//                                            break;
//                                        case TypeCode.UInt32:
//                                            set_value = (UInt32)v.valueLong;
//                                            break;
//                                        case TypeCode.Int64:
//                                            set_value = v.valueLong;
//                                            break;
//                                        case TypeCode.UInt64:
//                                            set_value = (UInt64)v.valueLong;
//                                            break;
//                                        case TypeCode.Single:
//                                            set_value = (Single)v.valueLong;
//                                            break;
//                                        case TypeCode.Double:
//                                            set_value = (Double)v.valueLong;
//                                            break;
//                                        case TypeCode.Decimal:
//                                            set_value = (Decimal)v.valueLong;
//                                            break;
//                                    }
//                                    break;
//                                case JsonValueType.Double:
//                                    switch (itemTypeCode)
//                                    {
//                                        case TypeCode.Single:
//                                            set_value = (Single)v.valueDouble;
//                                            break;
//                                        case TypeCode.Double:
//                                            set_value = (Double)v.valueDouble;
//                                            break;
//                                        case TypeCode.Decimal:
//                                            set_value = (Decimal)v.valueDouble;
//                                            break;
//                                    }
//                                    break;
//                                case JsonValueType.Boolean:
//                                    set_value = v.valueBool;
//                                    break;
//                                case JsonValueType.Object:
//                                    if (myObject.type == (typeof(Type)))
//                                    {
//                                        set_value = Type.GetType(new string(vs, v.vStringStart, v.vStringLength));
//                                    }
//                                    break;
//                            }

//                            if (myObject.ArrayRank == 1)
//                            {
//                                if (myObject.ArrayRankIndex == null)
//                                {
//                                    myObject.objArray.SetValue(set_value, v.arrayIndex);
//                                }
//                                else
//                                {
//                                    myObject.objArray.SetValue(set_value, myObject.ArrayRankIndex);
//                                    var nowRank = myObject.ArrayRankIndex.Length - 1;//到下一个秩，相当于进一位
//                                    Chake:
//                                    ++myObject.ArrayRankIndex[nowRank];
//                                    if (myObject.ArrayRankIndex[nowRank] == myObject.ArrayRankLengths[nowRank])//进一位
//                                    {
//                                        --nowRank;
//                                        if (nowRank >= 0)
//                                        {
//                                            goto Chake;
//                                        }
//                                        else
//                                        {
//                                            //break;
//                                        }
//                                    }
//                                    //进位后，小位数的下标变成0
//                                    for (int k = myObject.ArrayRankIndex.Length - 1; k > nowRank; k--)
//                                    {
//                                        myObject.ArrayRankIndex[k] = 0;
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }


//                //类类型赋值
//                for (int i = createObjectItems.Length - 2; i >= 0; i--)
//                {
//                    CreateObjectItem parentObject = createObjectItems[i];
//                    JsonObject parent = jsonRender.objectQueue[parentObject.index];
//                    foreach (var myObject in createObjectItems[i].sub)
//                    {
//                        JsonObject v = jsonRender.objectQueue[myObject.index];
//                        if (v.isObject)
//                        {
//                            IReadCollectionObject collection = myObject.collectionObject;
//                            if (collection != null)
//                            {
//                                myObject.Obj = collection.End(myObject.Obj);
//                            }
//                        }
//                        else
//                        {
//                            IReadCollectionObject collection = myObject.collectionArray;
//                            if (collection != null)
//                            {
//                                myObject.Obj = collection.End(myObject.Obj);
//                            }
//                        }



//                        if (parent.isObject)
//                        {
//                            IReadCollectionObject parentCollection = parentObject.collectionObject;
//                            if (parentCollection == null)
//                            {
//                                if (myObject.fieldInfo.isValueType)
//                                {
//                                    TypeAddrFieldAndProperty.SetStruct(parentObject.byteP + myObject.fieldInfo.offset,
//                                        GeneralTool.ObjectToVoid(myObject.Obj),
//                                        UnsafeOperation.SizeOf(myObject.type));

//                                    //myObject.fieldInfo.SetValue(parentObject.Obj, myObject.Obj);
//                                }
//                                else
//                                {
//                                    GeneralTool.SetObject(parentObject.byteP + myObject.fieldInfo.offset,
//                                       myObject.Obj);
//                                }
//                                //
//                            }
//                            else
//                            {
//                                parentCollection.Add(parentObject.Obj, &v, myObject.Obj);
//                            }
//                        }
//                        else
//                        {
//                            IReadCollectionObject parentCollection = parentObject.collectionArray;

//                            if (parentCollection == null)
//                            {
//                                if (parentObject.ArrayRank == 1)
//                                {
//                                    if (parentObject.ArrayRankIndex == null)
//                                    {
//                                        parentObject.objArray.SetValue(myObject.Obj, v.arrayIndex);
//                                    }
//                                    else
//                                    {
//                                        //myObject.objArray.SetValue(set_value, myObject.ArrayRankIndex);

//                                        parentObject.ArrayRankIndex[parentObject.ArrayRankIndex.Length - 1] = v.arrayIndex;
//                                        parentObject.objArray.SetValue(myObject.Obj, parentObject.ArrayRankIndex);
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                parentCollection.Add(parentObject.Obj, &v, myObject.Obj);
//                            }


//                        }
//                    }
//                }




//            }


//            for (int i = 0; i < createObjectItems.Length; i++)
//            {
//                if (createObjectItems[i].gcHandle != default(GCHandle))
//                {
//                    createObjectItems[i].gcHandle.Free();
//                }
//            }

//            //Console.ReadKey();
//            return createObjectItems[0].Obj;
//        }



//        private unsafe object PathToObject(char* nowChar, int vStringLength, CreateObjectItem nowObject)
//        {
//            var nowSub = nowObject.sub;
//            int startKey = 0;
//            int index = 0;
//            for (int k = 0; k < vStringLength; k++)
//            {
//                char* now = nowChar + k;
//                if (nowObject.jsonObject.isObject)
//                {
//                    if (*now == '/')
//                    {
//                        var pathKey = new string(nowChar, startKey, k - startKey);
//                        startKey = k + 1;
//                        foreach (var sub in nowSub)
//                        {
//                            if (sub.key == pathKey)
//                            {
//                                nowSub = sub.sub;
//                                nowObject = sub;
//                                goto Continue;
//                            }
//                        }
//                        return null;

//                        Continue:
//                        continue;
//                    }
//                }
//                else
//                {
//                    if (*now == '/')
//                    {
//                        startKey = k + 1;
//                        if (nowSub[index].jsonObject.arrayIndex == index)
//                        {
//                            nowObject = nowSub[index];
//                            nowSub = nowObject.sub;
//                            index = 0;
//                            goto Continue;
//                        }
//                        foreach (var sub in nowSub)
//                        {
//                            if (sub.jsonObject.arrayIndex == index)
//                            {
//                                nowSub = sub.sub;
//                                nowObject = sub;
//                                index = 0;
//                                goto Continue;
//                            }
//                        }
//                        return null;

//                        Continue:
//                        continue;
//                    }
//                    else
//                    {
//                        if (*now < '0' || *now > '9')
//                        {
//                            return null;
//                        }
//                        else
//                        {
//                            index *= 10;
//                            index += (*now - '0');
//                        }
//                    }

//                }
//            }
//            if (startKey < vStringLength)
//            {
//                if (nowObject.jsonObject.isObject)
//                {
//                    var pathKey = new string(nowChar, startKey, vStringLength - startKey);
//                    foreach (var sub in nowSub)
//                    {
//                        if (sub.key == pathKey)
//                        {
//                            return sub.Obj;
//                        }
//                    }
//                    return null;
//                }
//                else
//                {
//                    if (nowSub[index].jsonObject.arrayIndex == index)
//                    {
//                        return nowSub[index].Obj;
//                    }
//                    foreach (var sub in nowSub)
//                    {
//                        if (sub.jsonObject.arrayIndex == index)
//                        {
//                            return sub.Obj;
//                        }
//                    }
//                    return null;
//                }
//            }
//            else
//            {
//                return nowObject.Obj;
//            }
//        }





//    }
//}
