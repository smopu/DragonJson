//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;

//namespace DogJson
//{
//    public class ReflectionToObject : IJsonRenderToObject
//    {
//        public class CreateObjectItem
//        {
//            public CreateObjectItem(int index)
//            {
//                this.index = index;
//            }
//            public object obj;
//            public Type type;
//            public Type sourceType;
//            public FieldInfo fieldInfo;
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
//        }
//        static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

//        public unsafe object CreateObject(JsonRender jsonRender, Type type, char* vs, int length)
//        {
//            CreateObjectItem[] createObjectItems = new CreateObjectItem[jsonRender.objectQueueIndex];
//            {
//                createObjectItems[0] = new CreateObjectItem(0);
//                createObjectItems[0].type = type;
//                createObjectItems[0].obj = FormatterServices.GetUninitializedObject(type);
//                createObjectItems[0].isValueType = type.IsValueType;
//                createObjectItems[0].jsonObject = jsonRender.objectQueue[0];

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
//                            string key = new string(v.keyStringStart,0, v.keyStringLength);
//                            var fieldInfo = parentObject.type.GetField(key, bindingFlags);

//                            myObject.key = key;
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
//                                    myObject.type = fieldInfo.FieldType;
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
//                    //"#create"
//                    if (v.isConstructor)
//                    {
//                        myObject.type = typeof(ConstructorWrapper);
//                    }

//                    if (v.isObject)
//                    {
//                        IReadCollectionObject collection;
//                        if (CollectionManager.readObjectMap.TryGetValue(myObject.type, out collection))
//                        {
//                            myObject.obj = collection.Create(&v, parentObject.obj, myObject.sourceType, parentObject.type);
//                            myObject.collectionObject = collection;
//                        }
//                        else
//                        {
//                            if (myObject.type.IsGenericType && (collection = CollectionManager.GetReadObjectCollection(myObject.type)) != null)
//                            {
//                                myObject.obj = collection.Create(&v, parentObject.obj, myObject.sourceType, parentObject.type);
//                                myObject.collectionObject = collection;
//                            }
//                            else
//                            {
//                                myObject.obj = FormatterServices.GetUninitializedObject(myObject.type);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        IReadCollectionObject collection;
//                        if (CollectionManager.readObjectMap.TryGetValue(myObject.type, out collection))
//                        {
//                            myObject.obj = collection.Create(&v, parentObject.obj, myObject.type, parentObject.type);
//                            myObject.collectionArray = collection;
//                        }
//                        else
//                        {
//                            if (myObject.type.IsGenericType)
//                            {
//                                if (myObject.type.IsGenericType && (collection = CollectionManager.GetReadArrayCollection(myObject.type)) != null)
//                                {
//                                    myObject.obj = collection.Create(&v, parentObject.obj, myObject.type, parentObject.type);
//                                    myObject.collectionArray = collection;
//                                }
//                                else
//                                {
//                                    if (myObject.type.IsSubclassOf(typeof(MulticastDelegate)))
//                                    {
//                                        collection = CollectionManager.readObjectMap[typeof(MulticastDelegate)];
//                                        myObject.obj = collection.Create(&v, parentObject.obj, myObject.type, parentObject.type);
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
//                                        myObject.obj = myObject.objArray = Array.CreateInstance(elementType, v.arrayCount);
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

//                                            myObject.obj = myObject.objArray = parentObject.objArray;
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
//                                            myObject.obj = myObject.objArray = Array.CreateInstance(elementType, myObject.ArrayRankLengths);
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
//                            string key = new string(vs, v.keyStringStart, v.keyStringLength);
//                            FieldInfo fieldInfo = myObject.type.GetField(key, bindingFlags);
//                            var itemTypeCode = Type.GetTypeCode(fieldInfo.FieldType);
//                            switch (v.type)
//                            {
//                                case JsonValueType.String:
//                                    switch (itemTypeCode)
//                                    {
//                                        case TypeCode.Char:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, vs[v.vStringStart]);
//                                            break;
//                                        case TypeCode.String:
//                                            fieldInfo.SetValue(
//                                                myObject.obj,
//                                                new string(vs, v.vStringStart, v.vStringLength));
//                                            break;
//                                        case TypeCode.Object:

//                                            fieldInfo.SetValue(
//                                                myObject.obj, PathToObject(vs + v.vStringStart, v.vStringLength, createObjectItems[0]));

//                                            break;
//                                        default:

//                                            if (fieldInfo.FieldType.IsEnum)
//                                            {
//                                                var strEnum = new string(vs, v.vStringStart, v.vStringLength);
//                                                Array Arrays = Enum.GetValues(fieldInfo.FieldType);
//                                                for (int k = 0; k < Arrays.Length; k++)
//                                                {
//                                                    if (Arrays.GetValue(k).ToString().Equals(strEnum))
//                                                    {
//                                                        fieldInfo.SetValue(
//                                                            myObject.obj, Arrays.GetValue(k));
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
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (SByte)v.valueLong);
//                                            break;
//                                        case TypeCode.Byte:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (Byte)v.valueLong);
//                                            break;
//                                        case TypeCode.Int16:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (Int16)v.valueLong);
//                                            break;
//                                        case TypeCode.UInt16:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (UInt16)v.valueLong);
//                                            break;
//                                        case TypeCode.Int32:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (Int32)v.valueLong);
//                                            break;
//                                        case TypeCode.UInt32:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (UInt32)v.valueLong);
//                                            break;
//                                        case TypeCode.Int64:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, v.valueLong);
//                                            break;
//                                        case TypeCode.UInt64:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (UInt64)v.valueLong);
//                                            break;
//                                        case TypeCode.Single:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (Single)v.valueLong);
//                                            break;
//                                        case TypeCode.Double:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (Double)v.valueLong);
//                                            break;
//                                        case TypeCode.Decimal:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (Decimal)v.valueLong);
//                                            break;
//                                    }
//                                    break;
//                                case JsonValueType.Double:
//                                    switch (itemTypeCode)
//                                    {
//                                        case TypeCode.Single:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (Single)v.valueDouble);
//                                            break;
//                                        case TypeCode.Double:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, v.valueDouble);
//                                            break;
//                                        case TypeCode.Decimal:
//                                            fieldInfo.SetValue(
//                                                myObject.obj, (Decimal)v.valueDouble);
//                                            break;
//                                    }
//                                    break;
//                                case JsonValueType.Boolean:
//                                    fieldInfo.SetValue(
//                                        myObject.obj, v.valueBool);
//                                    break;
//                                case JsonValueType.Object:
//                                    if (myObject.type == (typeof(Type)))
//                                    {
//                                        fieldInfo.SetValue(
//                                            myObject.obj, Type.GetType(new string(vs, v.vStringStart, v.vStringLength)));
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
//                            collection.AddValue(myObject.obj, vs, &v);
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
//                                Chake:
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
//                                myObject.obj = collection.End(myObject.obj);
//                            }
//                        }
//                        else
//                        {
//                            IReadCollectionObject collection = myObject.collectionArray;
//                            if (collection != null)
//                            {
//                                myObject.obj = collection.End(myObject.obj);
//                            }
//                        }



//                        if (parent.isObject)
//                        {
//                            IReadCollectionObject parentCollection = parentObject.collectionObject;
//                            if (parentCollection == null)
//                            {
//                                myObject.fieldInfo.SetValue(parentObject.obj, myObject.obj);
//                            }
//                            else
//                            {
//                                parentCollection.Add(parentObject.obj, &v, myObject.obj);
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
//                                        parentObject.objArray.SetValue(myObject.obj, v.arrayIndex);
//                                    }
//                                    else
//                                    {
//                                        //myObject.objArray.SetValue(set_value, myObject.ArrayRankIndex);

//                                        parentObject.ArrayRankIndex[parentObject.ArrayRankIndex.Length - 1] = v.arrayIndex;
//                                        parentObject.objArray.SetValue(myObject.obj, parentObject.ArrayRankIndex);
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                parentCollection.Add(parentObject.obj, &v, myObject.obj);
//                            }


//                        }
//                    }
//                }




//            }




//            //Console.ReadKey();
//            return createObjectItems[0].obj;
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

//                    Continue:
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

//                    Continue:
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
//                            return sub.obj;
//                        }
//                    }
//                    return null;
//                }
//                else
//                {
//                    if (nowSub[index].jsonObject.arrayIndex == index)
//                    {
//                        return nowSub[index].obj;
//                    }
//                    foreach (var sub in nowSub)
//                    {
//                        if (sub.jsonObject.arrayIndex == index)
//                        {
//                            return sub.obj;
//                        }
//                    }
//                    return null;
//                }
//            }
//            else
//            {
//                return nowObject.obj;
//            }
//        }








//    }
//}
