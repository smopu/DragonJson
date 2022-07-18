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
            proxy = new ReadCollectionProxy();
            proxy.callGetValue = GetValue;
            for (int i = 0; i < createObjectItems.Length; i++)
            {
                createObjectItems[i] = new CreateObjectItem();
            }
        }


        static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        class CreateObjectItem
        {
            public Type type;
            //public Type sourceType;
            public bool isValueType;
            public ReadCollectionLink collectionObject;
            public ReadCollectionLink parentCollection;
            
            public PropertyDelegateItem2 propertyDelegateItem;
            public bool isProperty;
            
            public TypeAddrReflectionWrapper wrapper;
            public int offset;
            public GCHandle gcHandle;
            public byte* bytePtr;
            public byte* objPtr;
            public object obj;
            public object temp;
            public bool isSet;
            public bool collectionNoRef;
            

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
                    
                    IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoid(obj);

                    objPtr = (byte*)p;

                    //IntPtr head = *p;
                    //*p = ArrayWrapper.ByteArrayHead;
                    //gcHandle = GCHandle.Alloc(obj, GCHandleType.Pinned);
                    //*p = head;

                    bytePtr = objPtr + UnsafeOperation.PTR_COUNT;
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


        JsonRender jsonRender;
        public static int indexDbug = 0;
        public unsafe object CreateObject(JsonRender jsonRender, Type type, char* vs, int length)
        {
            this.jsonRender = jsonRender;
            setValuesIndex = 0;
            var rootItem = createObjectItems[0];
            int itemCount = jsonRender.objectQueueIndex;
            {
                //if (!allTypeWrapper.TryGetValue(type, out rootItem.wrapper))
                //{
                //    rootItem.wrapper = allTypeWrapper[type] = new TypeAddrReflectionWrapper(type);
                //}
                rootItem.wrapper = CollectionManager.GetTypeCollection(type).wrapper;

                //rootItem.wrapper = GetTypeWrapper(type);
                rootItem.type = type;

                rootItem.obj = rootItem.wrapper.Create(out rootItem.gcHandle, out rootItem.bytePtr, out rootItem.objPtr);
                //rootItem.Obj = rootItem.wrapper.Create(out rootItem.gcHandle);//out rootItem.gcHandle

                rootItem.isValueType = type.IsValueType;

                //对象数组创建
                for (int i = 1; i < jsonRender.objectQueueIndex; i++)
                {
                    int kkk = 21;
                    if (i == 21)
                    {
                        kkk = 9;
                    }
                    CreateObjectItem myObject = createObjectItems[i];
                    JsonObject* v = jsonRender.objectQueue + i;
                    JsonObject* parent = jsonRender.objectQueue + v->parentObjectIndex;
                    CreateObjectItem parentObject = createObjectItems[v->parentObjectIndex];
                    ReadCollectionLink parentCollection = myObject.parentCollection = parentObject.collectionObject;
                    myObject.obj = null;


                    TypeAddrFieldAndProperty fieldInfo = null;
                    myObject.isProperty = false;
                    myObject.collectionNoRef = false;
                    //string key;
                    if (parent->isObject)
                    {
                        //typeLength  parentCollection.GetItemType   fieldInfo.fieldType;

                        //优先级 typeLength >  parentCollection.GetItemType > fieldInfo.fieldType;

                        if (v->typeLength > 0)
                        {
                            if (v->isCommandValue)
                            {
                                if (v->isObject)
                                {
                                    string typeName = new string(vs, v->typeStartIndex, v->typeLength);
                                    myObject.type = UnsafeOperation.GetType(typeName);
                                    myObject.type = typeof(Box<>).MakeGenericType(myObject.type);
                                    //throw new Exception("To DO");
                                    var typeAllCollection = CollectionManager.GetTypeCollection(myObject.type);
                                    myObject.type = typeAllCollection.type;
                                    switch (typeAllCollection.typeCollectionEnum)
                                    {
                                        case CollectionManager.TypeCollectionEnum.Wrapper:
                                            myObject.wrapper = typeAllCollection.wrapper;
                                            break;
                                        case CollectionManager.TypeCollectionEnum.Read:
                                            myObject.collectionObject = typeAllCollection.read;
                                            break;
                                    }
                                    myObject.isValueType = typeAllCollection.IsValueType;
                                }
                                else
                                {
                                    string typeName = new string(vs, v->typeStartIndex, v->typeLength);
                                    var typeAllCollection = CollectionManager.GetTypeCollection(typeName);
                                    myObject.type = typeAllCollection.type;
                                    switch (typeAllCollection.typeCollectionEnum)
                                    {
                                        case CollectionManager.TypeCollectionEnum.Wrapper:
                                            myObject.wrapper = typeAllCollection.wrapper;
                                            break;
                                        case CollectionManager.TypeCollectionEnum.Read:
                                            myObject.collectionObject = typeAllCollection.read;
                                            break;
                                    }
                                    myObject.isValueType = typeAllCollection.IsValueType;
                                }
                            }
                            else
                            {
                                string typeName = new string(vs, v->typeStartIndex, v->typeLength);
                                var typeAllCollection = CollectionManager.GetTypeCollection(typeName);
                                myObject.type = typeAllCollection.type;
                                switch (typeAllCollection.typeCollectionEnum)
                                {
                                    case CollectionManager.TypeCollectionEnum.Wrapper:
                                        myObject.wrapper = typeAllCollection.wrapper;
                                        break;
                                    case CollectionManager.TypeCollectionEnum.Read:
                                        myObject.collectionObject = typeAllCollection.read;
                                        break;
                                }
                                myObject.isValueType = typeAllCollection.IsValueType;
                            }
                            if (parentCollection == null)
                            {
                                fieldInfo = parentObject.wrapper.Find(v->keyStringStart, v->keyStringLength);
                                var key = new string(v->keyStringStart, 0, v->keyStringLength);
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
                            }
                        }
                        else
                        {
                            if (parentCollection != null)
                            {
                                var typeAllCollection = parentCollection.getItemType(new ReadCollectionLink.GetItemType_Args() { bridge = v });
                                switch (typeAllCollection.typeCollectionEnum)
                                {
                                    case CollectionManager.TypeCollectionEnum.Wrapper:
                                        myObject.wrapper = typeAllCollection.wrapper;
                                        break;
                                    case CollectionManager.TypeCollectionEnum.Read:
                                        myObject.collectionObject = typeAllCollection.read;
                                        break;
                                }
                                myObject.isValueType = typeAllCollection.IsValueType;
                                myObject.type = typeAllCollection.type;
                            }
                            else
                            {
                                fieldInfo = parentObject.wrapper.Find(v->keyStringStart, v->keyStringLength);
                                var key = new string(v->keyStringStart, 0, v->keyStringLength);
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

                                myObject.type = fieldInfo.fieldOrPropertyType;
                                myObject.collectionObject = fieldInfo.read;
                                if (myObject.collectionObject == null)
                                {
                                    if (fieldInfo.wrapper == null)
                                    {
                                        var typeAllCollection = CollectionManager.GetTypeCollection(myObject.type);
                                        switch (typeAllCollection.typeCollectionEnum)
                                        {
                                            case CollectionManager.TypeCollectionEnum.Wrapper:
                                                fieldInfo.wrapper = myObject.wrapper = typeAllCollection.wrapper;
                                                break;
                                            case CollectionManager.TypeCollectionEnum.Read:
                                                myObject.collectionObject = typeAllCollection.read;
                                                break;
                                        }
                                        myObject.isValueType = typeAllCollection.IsValueType;
                                    }
                                    else
                                    {
                                        myObject.wrapper = fieldInfo.wrapper;
                                        myObject.isValueType = fieldInfo.isValueType;
                                    }
                                }
                                else
                                {
                                    myObject.isValueType = fieldInfo.isValueType;
                                }
                            }
                        }
                    }
                    else
                    {
                        //typeLength  parentCollection.GetItemType   fieldInfo.fieldType;
                        if (v->typeLength > 0)
                        {
                            if (v->isCommandValue)
                            {
                                if (v->isObject)
                                {
                                    string typeName = new string(vs, v->typeStartIndex, v->typeLength);
                                    myObject.type = UnsafeOperation.GetType(typeName);
                                    myObject.type = typeof(Box<>).MakeGenericType(myObject.type);
                                    //throw new Exception("To DO");
                                    var typeAllCollection = CollectionManager.GetTypeCollection(myObject.type);
                                    myObject.type = typeAllCollection.type;
                                    switch (typeAllCollection.typeCollectionEnum)
                                    {
                                        case CollectionManager.TypeCollectionEnum.Wrapper:
                                            myObject.wrapper = typeAllCollection.wrapper;
                                            break;
                                        case CollectionManager.TypeCollectionEnum.Read:
                                            myObject.collectionObject = typeAllCollection.read;
                                            break;
                                    }
                                    myObject.isValueType = typeAllCollection.IsValueType;
                                }
                                else
                                {
                                    string typeName = new string(vs, v->typeStartIndex, v->typeLength);
                                    var typeAllCollection = CollectionManager.GetTypeCollection(typeName);
                                    myObject.type = typeAllCollection.type;
                                    switch (typeAllCollection.typeCollectionEnum)
                                    {
                                        case CollectionManager.TypeCollectionEnum.Wrapper:
                                            myObject.wrapper = typeAllCollection.wrapper;
                                            break;
                                        case CollectionManager.TypeCollectionEnum.Read:
                                            myObject.collectionObject = typeAllCollection.read;
                                            break;
                                    }
                                    myObject.isValueType = typeAllCollection.IsValueType;
                                }
                                //myObject.type = typeof(Box<>).MakeGenericType(typeAllCollection.type);
                                //throw new Exception("To DO");
                            }
                            else
                            {
                                string typeName = new string(vs, v->typeStartIndex, v->typeLength);
                                var typeAllCollection = CollectionManager.GetTypeCollection(typeName);

                                myObject.type = typeAllCollection.type;
                                switch (typeAllCollection.typeCollectionEnum)
                                {
                                    case CollectionManager.TypeCollectionEnum.Wrapper:
                                        myObject.wrapper = typeAllCollection.wrapper;
                                        break;
                                    case CollectionManager.TypeCollectionEnum.Read:
                                        myObject.collectionObject = typeAllCollection.read;
                                        break;
                                }
                                myObject.isValueType = typeAllCollection.IsValueType;
                            }

                            if (parentCollection == null)
                            {
                                myObject.offset = parentObject.ArrayNowItemSize * v->arrayIndex;
                            }
                        }
                        else
                        {
                            CollectionManager.TypeAllCollection typeAllCollection;
                            if (parentCollection != null)
                            {
                                typeAllCollection = parentCollection.getItemType(new ReadCollectionLink.GetItemType_Args() { bridge = v });
                                myObject.type = typeAllCollection.type;
                            }
                            else
                            {
                                myObject.offset = parentObject.ArrayNowItemSize * v->arrayIndex;
                                myObject.type = parentObject.ArrayItemType;
                                typeAllCollection = CollectionManager.GetTypeCollection(myObject.type);
                            }
                            switch (typeAllCollection.typeCollectionEnum)
                            {
                                case CollectionManager.TypeCollectionEnum.Wrapper:
                                    myObject.wrapper = typeAllCollection.wrapper;
                                    break;
                                case CollectionManager.TypeCollectionEnum.Read:
                                    myObject.collectionObject = typeAllCollection.read;
                                    break;
                            }
                            myObject.isValueType = typeAllCollection.IsValueType;
                        }
                    }


                    //"$create"
                    if (v->isConstructor)
                    {
                        myObject.type = typeof(ConstructorWrapper<>).MakeGenericType(myObject.type);
                        var typeAllCollection = CollectionManager.GetTypeCollection(myObject.type);
                        myObject.collectionObject = typeAllCollection.read;
                    }



                    if (v->isObject)
                    {
                        ReadCollectionLink collection = myObject.collectionObject;

                        if (collection == null)
                        {
                            //父对象是容器就延迟赋值
                            if (parentCollection != null)
                            {
                                myObject.obj = myObject.wrapper.Create(out myObject.gcHandle, out myObject.bytePtr, out myObject.objPtr);
                                setValues[setValuesIndex++] = i;
                            }
                            else
                            {
                                //对象是属性延迟赋值
                                if (myObject.isProperty)
                                {
                                    myObject.obj = myObject.wrapper.Create(out myObject.gcHandle, out myObject.bytePtr, out myObject.objPtr);
                                    setValues[setValuesIndex++] = i;
                                }
                                else
                                {
                                    //对象是值类型字段就取指针
                                    if (myObject.isValueType)
                                    {
                                        myObject.objPtr = myObject.bytePtr = parentObject.bytePtr + myObject.offset;
                                    }
                                    else
                                    {
                                        myObject.obj = myObject.wrapper.Create(out myObject.gcHandle, out myObject.bytePtr, out myObject.objPtr);
                                        GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, myObject.obj);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //collection != null

                            ReadCollectionLink.Create_Args arg = new ReadCollectionLink.Create_Args();
                            //arg.objectType = myObject.sourceType;
                            arg.bridge = v;
                            //arg.parent = parentObject.objPtr;


                            //父对象是容器就延迟赋值
                            if (parentCollection != null)
                            {
                                myObject.obj = collection.createObject(out myObject.temp, arg);
                                setValues[setValuesIndex++] = i;
                            }
                            else
                            {
                                //对象是属性延迟赋值
                                if (myObject.isProperty)
                                {
                                    myObject.obj = collection.createObject(out myObject.temp, arg);
                                    setValues[setValuesIndex++] = i;
                                }
                                else
                                {
                                    //对象是值类型字段就取指针
                                    if (myObject.isValueType)
                                    {
                                        myObject.objPtr = myObject.bytePtr = parentObject.bytePtr + myObject.offset;
                                        collection.createStruct(myObject.objPtr, out myObject.temp, arg);
                                        if (collection.isLaze)
                                        {
                                            setValues[setValuesIndex++] = i;
                                        }
                                    }
                                    else
                                    {
                                        myObject.obj = collection.createObject(out myObject.temp, arg);
                                        if (collection.isLaze)
                                        {
                                            setValues[setValuesIndex++] = i;
                                        }
                                        else
                                        {
                                            GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, myObject.obj);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //array
                    else
                    {
                        ReadCollectionLink collection = CollectionManager.GetReadCollectionLink(myObject.type);
                        if (collection == null)
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
                                    myObject.obj = arrayWrapper.CreateArrayOne(elementType, v->arrayCount, out myObject.objPtr, out myObject.bytePtr, out myObject.gcHandle, out myObject.ArrayNowItemSize);
                                    myObject.objPtr = (byte*)GeneralTool.ObjectToVoid(myObject.obj);
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

                                        myObject.obj = arrayWrapper.CreateArray(elementType, out myObject.objPtr, out myObject.bytePtr, out myObject.gcHandle, out myObject.ArrayItemTypeSize);

                                        myObject.ArrayNowItemSize *= myObject.ArrayItemTypeSize;

                                    }
                                }

                                //父对象是容器就延迟赋值
                                if (parentCollection != null)
                                {
                                    setValues[setValuesIndex++] = i;
                                }
                                else
                                {
                                    //对象是属性延迟赋值
                                    if (myObject.isProperty)
                                    {
                                        setValues[setValuesIndex++] = i;
                                    }
                                    else
                                    {
                                        GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, myObject.obj);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("数组容器未注册");
                            }
                        }
                        else
                        {
                            //collection != null
                            myObject.collectionObject = collection;

                            ReadCollectionLink.Create_Args arg = new ReadCollectionLink.Create_Args();
                            //arg.objectType = myObject.sourceType;
                            arg.bridge = v;
                            //arg.parent = parentObject.objPtr;

                            //父对象是容器就延迟赋值
                            if (parentCollection != null)
                            {
                                myObject.obj = collection.createObject(out myObject.temp, arg);
                                setValues[setValuesIndex++] = i;
                            }
                            else
                            {
                                //对象是属性延迟赋值
                                if (myObject.isProperty)
                                {
                                    myObject.obj = collection.createObject(out myObject.temp, arg);
                                    setValues[setValuesIndex++] = i;
                                }
                                else
                                {
                                    //对象是值类型字段就取指针
                                    if (myObject.isValueType)
                                    {
                                        myObject.objPtr = myObject.bytePtr = parentObject.bytePtr + myObject.offset;

                                        if (collection.isLaze)
                                        {
                                            setValues[setValuesIndex++] = i;
                                        }
                                        else
                                        {
                                            collection.createStruct(myObject.objPtr, out myObject.temp, arg);
                                        }
                                    }
                                    else
                                    {
                                        myObject.obj = collection.createObject(out myObject.temp, arg);

                                        if (collection.isLaze)
                                        {
                                            setValues[setValuesIndex++] = i;
                                        }
                                        else
                                        {
                                            GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, myObject.obj);
                                        }
                                    }
                                }
                            }


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
                        ReadCollectionLink collection = myObject.collectionObject;
                        if (collection != null)
                        {
                            ReadCollectionLink.AddValue_Args addValue_Args = new ReadCollectionLink.AddValue_Args();
                            addValue_Args.callGetValue = GetValue;
                            addValue_Args.str = vs;
                            addValue_Args.temp = myObject.temp;
                            addValue_Args.value = jsonRender.pool + i;

                            if (myObject.obj == null)
                            {
                                collection.addValueStruct(myObject.objPtr, addValue_Args);
                            }
                            else
                            {
                                collection.addValueClass(myObject.obj, addValue_Args);
                            }
                            //parentCollection.add(parentObject.bytePtr, myObject.bytePtr, add_Args);
                        }
                        else
                        {
                            //var debug = new string(vs, v.keyStringStart, v.keyStringLength);
                            TypeAddrFieldAndProperty fieldInfo = myObject.wrapper.Find(vs + v.keyStringStart, v.keyStringLength);
                            var itemTypeCode = fieldInfo.typeCode;
                            if (fieldInfo.isProperty)
                            {
                                CreateObjectItem parentObject = createObjectItems[parent->objectQueueIndex];
                                byte* bytePtr;
                                
                                //if (parentObject.isValueType)
                                //{
                                //    bytePtr = myObject.bytePtr;
                                //}
                                //else
                                //{
                                //    bytePtr = myObject.dataStartPtr;
                                //}
                                bytePtr = myObject.objPtr;

                                //* fieldInfo.propertyDelegateItem.setTargetPtr = bytePtrStart;
                                switch (v.type)
                                {
                                    case JsonValueType.String:
                                        switch (itemTypeCode)
                                        {
                                            case TypeCode.Char:
                                                fieldInfo.propertyDelegateItem.setChar(bytePtr, vs[v.valueStringStart]);
                                                break;
                                            case TypeCode.String:
                                                fieldInfo.propertyDelegateItem.setString(bytePtr, jsonRender.EscapeString(vs + v.valueStringStart, v.valueStringLength));
                                                break;
                                            case TypeCode.Object:
                                                JsonObject* obj = jsonRender.objectQueue;
                                                if (PathToObject(vs + v.valueStringStart, v.valueStringLength, parent, jsonRender, ref obj))
                                                {
                                                    var taget = createObjectItems[obj->objectQueueIndex];
                                                    if (taget.collectionNoRef)
                                                    {
                                                        fieldInfo.propertyDelegateItem.setObject(bytePtr, taget.obj);
                                                    }
                                                    else
                                                    {
                                                        fieldInfo.propertyDelegateItem.setObject(bytePtr, taget.obj);
                                                    }
                                                    break;
                                                }
                                                //var k2 = new string(vs, v.vStringStart, v.vStringLength);

                                                if (fieldInfo.fieldOrPropertyType == typeof(Type))
                                                //if (fieldInfo.fieldType.IsSubclassOf(typeof(Type)))
                                                { 
                                                    GeneralTool.SetObject(bytePtr + fieldInfo.offset,
                                                        UnsafeOperation.GetType(new string(vs, v.valueStringStart, v.valueStringLength))
                                                        );
                                                }
                                                break;
                                            default:

                                                if (fieldInfo.isEnum)
                                                {
                                                    var strEnum = new string(vs, v.valueStringStart, v.valueStringLength);
                                                    Array Arrays = Enum.GetValues(fieldInfo.fieldOrPropertyType);
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
                                        fieldInfo.propertyDelegateItem.setBoolean(bytePtr, v.valueBool);
                                        break;
                                    case JsonValueType.Object:
                                        if (myObject.type == (typeof(Type)))
                                        {
                                            fieldInfo.propertyDelegateItem.setObject(bytePtr,
                                                UnsafeOperation.GetType(new string(vs, v.valueStringStart, v.valueStringLength))
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
                                                *(char*)(myObject.bytePtr + fieldInfo.offset) = vs[v.valueStringStart];
                                                break;
                                            case TypeCode.String:
                                                GeneralTool.SetObject(myObject.bytePtr + fieldInfo.offset, jsonRender.EscapeString(vs + v.valueStringStart, v.valueStringLength));
                                                break;
                                            case TypeCode.Object:
                                                JsonObject* obj = jsonRender.objectQueue;
                                                if (PathToObject(vs + v.valueStringStart, v.valueStringLength, parent, jsonRender, ref obj))
                                                {
                                                    GeneralTool.SetObject(myObject.bytePtr + fieldInfo.offset,
                                                      createObjectItems[obj->objectQueueIndex].obj);
                                                    break;
                                                }
                                                if (fieldInfo.fieldOrPropertyType == typeof(Type))
                                                //if (fieldInfo.fieldType.IsSubclassOf(typeof(Type)))
                                                {
                                                    GeneralTool.SetObject(myObject.bytePtr + fieldInfo.offset,
                                                        UnsafeOperation.GetType(new string(vs, v.valueStringStart, v.valueStringLength))
                                                        );
                                                }
                                                break;
                                            default:

                                                if (fieldInfo.isEnum)
                                                {
                                                    var strEnum = new string(vs, v.valueStringStart, v.valueStringLength);
                                                    Array Arrays = Enum.GetValues(fieldInfo.fieldOrPropertyType);
                                                    for (int k = 0; k < Arrays.Length; k++)
                                                    {
                                                        if (Arrays.GetValue(k).ToString().Equals(strEnum))
                                                        {
                                                            GeneralTool.Memcpy(myObject.bytePtr + fieldInfo.offset
                                                                , ((IntPtr*)GeneralTool.ObjectToVoid(Arrays.GetValue(k)) + 1)
                                                                , UnsafeOperation.SizeOfStack(fieldInfo.fieldOrPropertyType)
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
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        ReadCollectionLink collection = myObject.collectionObject;
                        if (collection != null)
                        {
                            ReadCollectionLink.AddValue_Args addValue_Args = new ReadCollectionLink.AddValue_Args();
                            addValue_Args.callGetValue = GetValue;
                            addValue_Args.str = vs;
                            addValue_Args.temp = myObject.temp;
                            addValue_Args.value = jsonRender.pool + i;

                            if (myObject.obj == null)
                            {
                                collection.addValueStruct(myObject.objPtr, addValue_Args);
                            }
                            else
                            {
                                collection.addValueClass(myObject.obj, addValue_Args);
                            }
                            //collection.addValue(myObject.objPtr, addValue_Args);
                            //collection.AddValue(myObject.obj, vs, jsonRender.pool + i, proxy);
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
                                            *(char*)(pByte) = vs[v.valueStringStart];
                                            break;
                                        case TypeCode.String:
                                            GeneralTool.SetObject(pByte, jsonRender.EscapeString(vs + v.valueStringStart, v.valueStringLength));
                                            break;
                                        case TypeCode.Object:
                                            JsonObject* obj = jsonRender.objectQueue;
                                            if (PathToObject(vs + v.valueStringStart, v.valueStringLength, parent, jsonRender, ref obj))
                                            {
                                                GeneralTool.SetObject(pByte,
                                                  createObjectItems[obj->objectQueueIndex].obj);
                                                    break;
                                            }
                                            if (itemType == typeof(Type))
                                            //if (fieldInfo.fieldType.IsSubclassOf(typeof(Type)))
                                            {
                                                GeneralTool.SetObject(pByte,
                                                    UnsafeOperation.GetType(new string(vs, v.valueStringStart, v.valueStringLength))
                                                    );
                                            }
                                            //set_value = PathToObject(vs + v.vStringStart, v.vStringLength, createObjectItems[0]);
                                            break;
                                        default:
                                            if (itemType.IsEnum)
                                            {
                                                var strEnum = new string(vs, v.valueStringStart, v.valueStringLength);
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
                                                break;
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
                                            UnsafeOperation.GetType(new string(vs, v.valueStringStart, v.valueStringLength)));
                                    }
                                    break;
                            }
                        }
                    }
                }



                ///*
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
                   {
                       //int i = j;
                       int i = setValuesOrder[j];
                       //SetValue setValue = setValues[i];
                       JsonObject* objValue = jsonRender.objectQueue + setValues[i];

                       //CreateObjectItem myObject = setValue.myObject;
                       CreateObjectItem myObject = createObjectItems[objValue->objectQueueIndex];
                       JsonObject* parent = jsonRender.objectQueue + objValue->parentObjectIndex;
                       CreateObjectItem parentObject = createObjectItems[objValue->parentObjectIndex];
                        ReadCollectionLink collection = myObject.collectionObject;
                        ReadCollectionLink parentCollection = parentObject.collectionObject;
                        if (myObject.temp != null || collection != null && collection.isLaze)
                        {
                            if (myObject.obj == null)
                            {
                                collection.end(myObject.objPtr, myObject.temp);
                            }
                            else
                            {
                                myObject.obj = collection.endObject(myObject.obj, myObject.temp);
                            }
                            //if (myObject.isSet)
                            //{
                            //    continue;
                            //}
                        }


                        //父对象是容器
                        if (parentCollection != null)
                        {
                            ReadCollectionLink.Add_Args arg = new ReadCollectionLink.Add_Args();
                            arg.bridge = objValue;
                            arg.temp = parentObject.temp;
                            if (parentObject.obj == null)
                            {
                                parentCollection.addObjectStruct(parentObject.bytePtr, myObject.obj, arg);
                            }
                            else
                            {
                                parentCollection.addObjectClass(parentObject.obj, myObject.obj, arg);
                            }
                        }
                        else
                        {
                            //对象是属性
                            if (myObject.isProperty)
                            {
                                myObject.propertyDelegateItem.setObject(parentObject.objPtr, myObject.obj);
                            }
                            else
                            {
                                ReadCollectionLink.Create_Args arg = new ReadCollectionLink.Create_Args();
                                //arg.objectType = myObject.sourceType;
                                arg.bridge = objValue;
                                //arg.parent = parentObject.objPtr;


                                if (collection == null)
                                {
                                    //对象是值类型字段就取指针
                                    if (myObject.isValueType)
                                    {
                                        collection.createStruct(myObject.objPtr, out myObject.temp, arg);
                                    }
                                    else
                                    {
                                        collection.createStruct(myObject.objPtr, out myObject.temp, arg);
                                    }
                                }
                                else {
                                    //对象是值类型字段就取指针
                                    if (myObject.isValueType)
                                    {
                                    }
                                    else
                                    {
                                        GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, myObject.obj);
                                    }
                                }
                            }
                        }


                       // if (myObject.isProperty)
                       // {
                       //     myObject.propertyDelegateItem.setObject(parentObject.objPtr, myObject.obj);
                       // }
                       // else
                       //{
                       //    if (parentObject.collectionObject == null)
                       //    {
                       //         throw new Exception("Error");
                       //    }
                       //    else
                       //    {
                       //         ReadCollectionLink.Add_Args arg = new ReadCollectionLink.Add_Args();
                       //         arg.bridge = objValue;
                       //         arg.temp = myObject.temp;

                       //         //* (IntPtr*)(parentObject.bytePtr + myObject.offset) = (IntPtr)myObject.objPtr;
                       //         parentObject.collectionObject.addObject3(parentObject.obj, myObject.obj, arg);
                       //        //void Add(void* obj, void* value, Add_Args arg);
                       //    }
                       //}



                    }
                }


               // */

            }


            //Dubg:
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


        public ReadCollectionProxy proxy = new ReadCollectionProxy();
        object GetValue(TypeCode typeCode, char* str, JsonValue* value)
        {
            CreateObjectItem myObject;
            JsonObject* parent = value->objectQueue;
            switch (value->type)
            {
                case JsonValueType.String:
                    switch (typeCode)
                    {
                        case TypeCode.Char:
                            return str[value->valueStringStart];

                        case TypeCode.String:
                            return new string(str, value->valueStringStart, value->valueStringLength);

                        case TypeCode.Object:
                            JsonObject* obj = jsonRender.objectQueue;
                            if (PathToObject(str + value->valueStringStart, value->valueStringLength, parent, jsonRender, ref obj))
                            {
                                return createObjectItems[obj->objectQueueIndex].obj;
                            }
                             myObject = createObjectItems[value->objectQueue->objectQueueIndex];
                            if (myObject.type == typeof(Type))
                            //if (fieldInfo.fieldType.IsSubclassOf(typeof(Type)))
                            {
                                return UnsafeOperation.GetType(new string(str, value->valueStringStart, value->valueStringLength));
                            }
                            break;
                        default:
                             myObject = createObjectItems[value->objectQueue->objectQueueIndex];
                            if (myObject.type.IsEnum)
                            {
                                var strEnum = new string(str, value->valueStringStart, value->valueStringLength);
                                Array Arrays = Enum.GetValues(myObject.type);
                                for (int k = 0; k < Arrays.Length; k++)
                                {
                                    if (Arrays.GetValue(k).ToString().Equals(strEnum))
                                    {
                                        return Arrays.GetValue(k);
                                    }
                                }
                            }
                            break;
                    }
                    break;
                case JsonValueType.Long:
                    switch (typeCode)
                    {
                        case TypeCode.SByte:
                            return (SByte)value->valueLong;
                        case TypeCode.Byte:
                            return (Byte)value->valueLong;
                        case TypeCode.Int16:
                            return (Int16)value->valueLong;
                        case TypeCode.UInt16:
                            return (UInt16)value->valueLong;
                        case TypeCode.Int32:
                            return (Int32)value->valueLong;
                        case TypeCode.UInt32:
                            return (UInt32)value->valueLong;
                        case TypeCode.Int64:
                            return value->valueLong;
                        case TypeCode.UInt64:
                            return (UInt64)value->valueLong;
                        case TypeCode.Single:
                            return (Single)value->valueLong;
                        case TypeCode.Double:
                            return (Double)value->valueLong;
                        case TypeCode.Decimal:
                            return (Decimal)value->valueLong;
                    }
                    break;
                case JsonValueType.Double:
                    switch (typeCode)
                    {
                        case TypeCode.Single:
                            return (Single)value->valueDouble;
                        case TypeCode.Double:
                            return (Double)value->valueDouble;
                        case TypeCode.Decimal:
                            return (Decimal)value->valueDouble;
                    }
                    break;
                case JsonValueType.Boolean:
                    return value->valueBool;
            }
            return null;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe bool PathToObject(char* path, int pathLength, JsonObject* nowParent, JsonRender render, ref JsonObject* obj)
        {
            if (*path == '$')
            {
                if (pathLength == 1)
                {
                    return true;
                }
                ++path;
                if (*path == '/')
                {
                    ++path;
                    pathLength -= 2;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

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
                    if (obj->objectQueueIndex + 1 >= render.objectQueueIndex)
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
                if (obj->objectNext >= render.objectQueueIndex)
                {
                    return false;
                }
                var next = render.objectQueue + obj->objectNext;
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
                    obj = render.objectQueue + obj->objectNext;
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
