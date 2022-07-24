using DogJson.RenderToObject;
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
        public AddrToObject2()
        {
            proxy = new ReadCollectionProxy();
            proxy.callGetValue = GetValue;
            for (int i = 0; i < createObjectItems.Length; i++)
            {
                createObjectItems[i] = new CreateObjectItem();
            }

            setValuesLength = 1024;
            setValuesIntPtr = Marshal.AllocHGlobal(setValuesLength * sizeof(int));
            setValues = (int*)setValuesIntPtr.ToPointer();

            setValuesOrderLength = 1024;
            setValuesOrderIntPtr = Marshal.AllocHGlobal(setValuesOrderLength * sizeof(int));
            setValuesOrder = (int*)setValuesOrderIntPtr.ToPointer();


            this.maxRank = 10;
            arrayRankIntPtr = Marshal.AllocHGlobal(100);
            arrayLengths = (int*)arrayRankIntPtr.ToPointer();
        }

        unsafe void ResizeSetValues()
        {
            setValuesLength *= 2;
            setValuesIntPtr = Marshal.ReAllocHGlobal(setValuesIntPtr, new IntPtr(setValuesLength * sizeof(int)));
            setValues = (int*)setValuesIntPtr.ToPointer();
        }

        unsafe void ResizeSetValuesOrder()
        {
            setValuesOrderLength = setValuesLength;
            setValuesOrderIntPtr = Marshal.ReAllocHGlobal(setValuesOrderIntPtr, new IntPtr(setValuesLength * sizeof(int)));
            setValuesOrder = (int*)setValuesOrderIntPtr.ToPointer();
        }

        ~AddrToObject2()
        {
            Marshal.FreeHGlobal(setValuesIntPtr);
            Marshal.FreeHGlobal(setValuesOrderIntPtr);
            Marshal.FreeHGlobal(arrayRankIntPtr);
        }

        IntPtr setValuesIntPtr;
        int* setValues;
        int setValuesLength = 1024;


        IntPtr setValuesOrderIntPtr;
        int* setValuesOrder;
        int setValuesOrderLength = 1024;
        //int[] setValues = new int[1024];
        //int[] setValuesOrder = new int[1024];


        IntPtr arrayRankIntPtr;
        int* arrayLengths;
        //int rank = 1;
        //public int arraySize = 1;
        //void SetSize(int length)
        //{
        //    arrayLengths[rank] = length;
        //    ++rank;
        //    arraySize *= length;
        //    if (maxRank < rank)
        //    {
        //        maxRank = rank;
        //    }
        //}
        int maxRank = 10;

        unsafe void ResizeSetArrayRank()
        {
            arrayRankIntPtr = Marshal.ReAllocHGlobal(arrayRankIntPtr, new IntPtr(maxRank * sizeof(int)));
            arrayLengths = (int*)arrayRankIntPtr.ToPointer();
        }


        static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        class CreateObjectItem
        {
            public Type type;
            public Type collectionType;
            public bool isValueType;

            public CollectionManager.TypeCollectionBranch readBranch = CollectionManager.TypeCollectionBranch.Wrapper;
            public ReadCollectionLink readCollection;
            public TypeAddrReflectionWrapper wrapper;
            public IArrayWrap arrayWrap;

            public PropertyDelegateItem2 propertyDelegateItem;
            public bool isProperty;
            
            public int offset;
            public GCHandle gcHandle;
            public byte* bytePtr;
            public byte* objPtr;
            public object obj;
            public object temp;
            public bool isSet;
            public bool collectionNoRef;
            

            public TypeCode arrayElementTypeCode;
            public Type arrayElementType;
            public int arrayRank;
            public int arrayItemTypeSize;
            public int arrayNowItemSize;

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

        int setValuesIndex = 0;
        //ArrayWrapper arrayWrapper = new ArrayWrapper();

        //SetValue[] setValues = new SetValue[1024];
        CreateObjectItem[] createObjectItems = new CreateObjectItem[1024];


        JsonRender jsonRender;
        public static int indexDbug = 0;
        public unsafe object CreateObject(JsonRender jsonRender,  char* vs, int length)
        {
            this.jsonRender = jsonRender;
            setValuesIndex = 0;
            var rootItem = createObjectItems[0];
            int itemCount = jsonRender.objectQueueIndex;
            {
                JsonObject* rootJsonObject = jsonRender.objectQueue;
                string rootTypeName = new string(vs, rootJsonObject->typeStartIndex, rootJsonObject->typeLength);
                CollectionManager.TypeAllCollection rootCollection;
                if (rootJsonObject->isCommandValue)
                {
                    if (rootJsonObject->isObject)
                    {
                        rootCollection = CollectionManager.GetBoxTypeCollection(rootTypeName);
                    }
                    else
                    {
                        rootCollection = CollectionManager.GetTypeCollection(rootTypeName);
                    }
                }
                else
                {
                    string typeName = new string(vs, rootJsonObject->typeStartIndex, rootJsonObject->typeLength);
                    rootCollection = CollectionManager.GetTypeCollection(rootTypeName);
                }
                Type type = rootCollection.type;
                rootItem.isValueType = type.IsValueType;
                rootItem.type = type;

                //"#create"
                if (rootJsonObject->isConstructor)
                {
                    rootCollection = rootCollection.GetConstructor(out rootItem.collectionType);
                }
                rootItem.readBranch = rootCollection.readBranch;

                switch (rootCollection.readBranch)
                {
                    case CollectionManager.TypeCollectionBranch.Wrapper:
                        rootItem.wrapper = rootCollection.wrapper;
                        rootItem.obj = rootItem.wrapper.Create(out rootItem.gcHandle, out rootItem.bytePtr, out rootItem.objPtr);
                        break;
                    case CollectionManager.TypeCollectionBranch.ReadCollection:
                        rootItem.readCollection = rootCollection.readCollection;
                        ReadCollectionLink.Create_Args arg = new ReadCollectionLink.Create_Args();
                        arg.objectType = rootItem.type;
                        arg.bridge = rootJsonObject;
                        rootItem.obj = rootItem.readCollection.createObject(out rootItem.temp, arg);
                        break;
                    case CollectionManager.TypeCollectionBranch.Array:
                        rootItem.arrayWrap = rootCollection.arrayWrap;

                        var rank = rootItem.arrayWrap.rank;
                        if (rank == 1)//数组的秩= 1 直接遍历赋值
                        {
                            rootItem.arrayRank = 1;
                            rootItem.arrayElementType = rootItem.arrayWrap.elementType;
                            rootItem.arrayElementTypeCode = rootItem.arrayWrap.elementTypeCode;

                            rootItem.obj = rootItem.arrayWrap.CreateArray(rootJsonObject->arrayCount, arrayLengths,
                                out rootItem.objPtr, out rootItem.bytePtr, out rootItem.gcHandle, out rootItem.arrayNowItemSize);

                            //myObject.objPtr = (byte*)GeneralTool.ObjectToVoid(myObject.obj);
                            rootItem.arrayItemTypeSize = rootItem.arrayNowItemSize;
                        }
                        else
                        {
                            rootItem.arrayRank = rank;
                            rootItem.arrayElementType = rootItem.arrayWrap.elementType;
                            rootItem.arrayElementTypeCode = rootItem.arrayWrap.elementTypeCode;

                            if (rank > jsonRender.objectQueueIndex)
                            {
                                throw new Exception("无法满足秩");
                            }

                            if (maxRank < rank)
                            {
                                maxRank = rank;
                                ResizeSetArrayRank();
                            }

                            int rankIndex = 0;
                            arrayLengths[rankIndex] = rootJsonObject->arrayCount;
                            ++rankIndex;
                            int arraySize = rootJsonObject->arrayCount;

                            for (int j = 1; j < rank; j++)
                            {
                                JsonObject* v1 = jsonRender.objectQueue + (j);
                                if (v1->parentObjectIndex == j - 1 && !v1->isObject)
                                {
                                    arrayLengths[rankIndex] = v1->arrayCount;
                                    ++rankIndex;
                                    arraySize *= v1->arrayCount;
                                    //arrayWrapper.SetSize(v1->arrayCount);
                                }
                                else
                                {
                                    throw new Exception("无法满足秩");
                                }
                            }

                            rootItem.arrayNowItemSize = arraySize / rootJsonObject->arrayCount;

                            rootItem.obj = rootItem.arrayWrap.CreateArray(arraySize, arrayLengths,
                                out rootItem.objPtr, out rootItem.bytePtr, out rootItem.gcHandle, out rootItem.arrayItemTypeSize);

                            rootItem.arrayNowItemSize *= rootItem.arrayItemTypeSize;

                        }
                        break;
                }


                //if (rootJsonObject->isCommandValue)
                //{
                //    rootCollection = rootCollection.GetBox();
                //    rootItem.type = rootCollection.boxType;
                //    rootItem.collectionObject = rootCollection.read;
                //    ReadCollectionLink.Create_Args arg = new ReadCollectionLink.Create_Args();
                //    arg.objectType = type;
                //    arg.bridge = rootJsonObject;
                //    rootItem.obj = rootItem.collectionObject.createObject(out rootItem.temp, arg);
                //}
                //else
                //{
                //    switch (rootCollection.typeCollectionEnum)
                //    {
                //        case CollectionManager.TypeCollectionEnum.Wrapper:
                //            rootItem.wrapper = rootCollection.wrapper;
                //            rootItem.obj = rootItem.wrapper.Create(out rootItem.gcHandle, out rootItem.bytePtr, out rootItem.objPtr);
                //            break;
                //        case CollectionManager.TypeCollectionEnum.Read:
                //            rootItem.collectionObject = rootCollection.read;
                //            ReadCollectionLink.Create_Args arg = new ReadCollectionLink.Create_Args();
                //            arg.objectType = rootItem.type;
                //            arg.bridge = rootJsonObject;
                //            rootItem.obj = rootItem.collectionObject.createObject(out rootItem.temp, arg);
                //            break;
                //        case CollectionManager.TypeCollectionEnum.Array:
                //            break;
                //    }
                //}


                //rootItem.Obj = rootItem.wrapper.Create(out rootItem.gcHandle);//out rootItem.gcHandle

                //对象数组创建
                for (int i = 1; i < jsonRender.objectQueueIndex; i++)
                {
                    CreateObjectItem myObject = createObjectItems[i];
                    JsonObject* v = jsonRender.objectQueue + i;
                    JsonObject* parent = jsonRender.objectQueue + v->parentObjectIndex;
                    CreateObjectItem parentObject = createObjectItems[v->parentObjectIndex];
                    ReadCollectionLink parentCollection = parentObject.readCollection;
                    myObject.obj = null;

                    TypeAddrFieldAndProperty fieldInfo = null;
                    myObject.isProperty = false;
                    myObject.collectionNoRef = false;
                    //string key;
                   
                    CollectionManager.TypeAllCollection typeAllCollection = null;

                    if (v->typeLength > 0)
                    {
                        if (v->isCommandValue)
                        {
                            string typeName = new string(vs, v->typeStartIndex, v->typeLength);
                            if (v->isObject)
                            {
                                typeAllCollection = CollectionManager.GetBoxTypeCollection(typeName);
                            }
                            else
                            {
                                typeAllCollection = CollectionManager.GetTypeCollection(typeName);
                            }
                        }
                        else
                        {
                            string typeName = new string(vs, v->typeStartIndex, v->typeLength);
                            typeAllCollection = CollectionManager.GetTypeCollection(typeName);
                        }

                        switch (parentObject.readBranch)
                        {
                            case CollectionManager.TypeCollectionBranch.Wrapper:
                                fieldInfo = parentObject.wrapper.Find(v->keyStringStart, v->keyStringLength);
                                if (fieldInfo.isProperty)
                                {
                                    myObject.isProperty = true;
                                    myObject.propertyDelegateItem = fieldInfo.propertyDelegateItem;
                                }
                                else
                                {
                                    myObject.offset = fieldInfo.offset;
                                }
                                break;
                            case CollectionManager.TypeCollectionBranch.ReadCollection:
                                break;
                            case CollectionManager.TypeCollectionBranch.Array:
                                myObject.offset = parentObject.arrayNowItemSize * v->arrayIndex;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        #region 没有 typeLength
                        switch (parentObject.readBranch)
                        {
                            case CollectionManager.TypeCollectionBranch.Wrapper:
                                {
                                    fieldInfo = parentObject.wrapper.Find(v->keyStringStart, v->keyStringLength);
                                    //var key = new string(v->keyStringStart, 0, v->keyStringLength);
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
                                    typeAllCollection = fieldInfo.GetTypeAllCollection();
                                }
                                break;
                            case CollectionManager.TypeCollectionBranch.ReadCollection:
                                {
                                    typeAllCollection = parentCollection.getItemType(new ReadCollectionLink.GetItemType_Args() { bridge = v });
                                }
                                break;
                            case CollectionManager.TypeCollectionBranch.Array:
                                {
                                    if (parentObject.arrayWrap.rank == 1)//数组的秩= 1 直接遍历赋值
                                    {
                                        typeAllCollection = parentObject.arrayWrap.GetTypeAllCollection();
                                        myObject.offset = parentObject.arrayNowItemSize * v->arrayIndex;
                                    }
                                    else
                                    {
                                        if (parentObject.arrayRank > 1)
                                        {
                                            myObject.arrayRank = parentObject.arrayRank - 1;

                                            myObject.arrayItemTypeSize = parentObject.arrayItemTypeSize;
                                            myObject.arrayNowItemSize = parentObject.arrayNowItemSize / v->arrayCount;
                                            myObject.offset = parentObject.arrayNowItemSize * v->arrayIndex;
                                            myObject.bytePtr = parentObject.bytePtr + myObject.offset;


                                            myObject.obj = parentObject.obj;

                                            myObject.arrayWrap = parentObject.arrayWrap;
                                            myObject.readBranch = CollectionManager.TypeCollectionBranch.Array;
                                            myObject.type = parentObject.type;
                                            myObject.isValueType = false;
                                            
                                            if (myObject.arrayRank == 1)
                                            {
                                                myObject.arrayElementType = myObject.arrayWrap.elementType;
                                                myObject.arrayElementTypeCode = myObject.arrayWrap.elementTypeCode;
                                            }
                                            else
                                            {
                                                myObject.arrayElementType = parentObject.arrayElementType;
                                            }
                                            continue;
                                        }
                                        else
                                        {
                                            typeAllCollection = parentObject.arrayWrap.GetTypeAllCollection();
                                            myObject.offset = parentObject.arrayNowItemSize * v->arrayIndex;
                                        }
                                    }


                                }
                                break;
                            default:
                                break;
                        }
                        #endregion

                    }

                    myObject.type = typeAllCollection.type;
                    myObject.isValueType = typeAllCollection.IsValueType;

                    //"#create"
                    if (v->isConstructor)
                    {
                        typeAllCollection = typeAllCollection.GetConstructor(out myObject.collectionType);
                        //myObject.readCollection = typeAllCollection.readCollection;
                    }
                    myObject.readBranch = typeAllCollection.readBranch;

                    //switch (typeAllCollection.readBranch)
                    //{
                    //    case CollectionManager.TypeCollectionBranch.Wrapper:
                    //        myObject.wrapper = typeAllCollection.wrapper;
                    //        break;
                    //    case CollectionManager.TypeCollectionBranch.ReadCollection:
                    //        myObject.readCollection = typeAllCollection.readCollection;
                    //        break;
                    //    case CollectionManager.TypeCollectionBranch.Array:
                    //        myObject.arrayWrap = typeAllCollection.arrayWrap;
                    //        break;
                    //}
                    switch (typeAllCollection.readBranch)
                    {
                        case CollectionManager.TypeCollectionBranch.Wrapper:
                            if (!v->isObject)
                            {
                                throw new Exception("数组容器未注册");
                            }
                            myObject.wrapper = typeAllCollection.wrapper;
                            //父对象是容器就延迟赋值
                            if (parentCollection != null)
                            {
                                myObject.obj = myObject.wrapper.Create(out myObject.gcHandle, out myObject.bytePtr, out myObject.objPtr);
                                setValues[setValuesIndex++] = i; if (setValuesIndex == setValuesLength) { ResizeSetValues(); }
                            }
                            else
                            {
                                //对象是属性延迟赋值
                                if (myObject.isProperty)
                                {
                                    myObject.obj = myObject.wrapper.Create(out myObject.gcHandle, out myObject.bytePtr, out myObject.objPtr);
                                    setValues[setValuesIndex++] = i; if (setValuesIndex == setValuesLength) { ResizeSetValues(); }
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
                            break;
                        case CollectionManager.TypeCollectionBranch.ReadCollection:
                            ReadCollectionLink collection = myObject.readCollection = typeAllCollection.readCollection;
                            ReadCollectionLink.Create_Args arg = new ReadCollectionLink.Create_Args();
                            arg.objectType = myObject.type;
                            arg.bridge = v;
                            //arg.parent = parentObject.objPtr;

                            //父对象是容器就延迟赋值
                            if (parentCollection != null)
                            {
                                myObject.obj = collection.createObject(out myObject.temp, arg);
                                setValues[setValuesIndex++] = i; if (setValuesIndex == setValuesLength) { ResizeSetValues(); }
                            }
                            else
                            {
                                //对象是属性延迟赋值
                                if (myObject.isProperty)
                                {
                                    myObject.obj = collection.createObject(out myObject.temp, arg);
                                    setValues[setValuesIndex++] = i; if (setValuesIndex == setValuesLength) { ResizeSetValues(); }
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
                                            setValues[setValuesIndex++] = i; if (setValuesIndex == setValuesLength) { ResizeSetValues(); }
                                        }
                                    }
                                    else
                                    {
                                        myObject.obj = collection.createObject(out myObject.temp, arg);
                                        if (collection.isLaze)
                                        {
                                            setValues[setValuesIndex++] = i; if (setValuesIndex == setValuesLength) { ResizeSetValues(); }
                                        }
                                        else
                                        {
                                            GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, myObject.obj);
                                        }
                                    }
                                }
                            }
                            break;
                        case CollectionManager.TypeCollectionBranch.Array:
                            myObject.arrayWrap = typeAllCollection.arrayWrap;

                            var rank = myObject.arrayWrap.rank;
                            if (rank == 1)//数组的秩= 1 直接遍历赋值
                            {
                                myObject.arrayRank = 1;
                                myObject.arrayElementType = myObject.arrayWrap.elementType;
                                myObject.arrayElementTypeCode = myObject.arrayWrap.elementTypeCode;

                                myObject.obj = myObject.arrayWrap.CreateArray(v->arrayCount, arrayLengths,
                                    out myObject.objPtr, out myObject.bytePtr, out myObject.gcHandle, out myObject.arrayNowItemSize);

                                //myObject.objPtr = (byte*)GeneralTool.ObjectToVoid(myObject.obj);
                                myObject.arrayItemTypeSize = myObject.arrayNowItemSize;
                            }
                            else
                            {
                                myObject.arrayRank = rank;
                                myObject.arrayElementType = myObject.arrayWrap.elementType;
                                myObject.arrayElementTypeCode = myObject.arrayWrap.elementTypeCode;

                                if (rank + i > jsonRender.objectQueueIndex)
                                {
                                    throw new Exception("无法满足秩");
                                }

                                if (maxRank < rank)
                                {
                                    maxRank = rank;
                                    ResizeSetArrayRank();
                                }

                                int rankIndex = 0;
                                arrayLengths[rankIndex] = v->arrayCount;
                                ++rankIndex;
                                int arraySize = v->arrayCount;

                                for (int j = 1; j < rank; j++)
                                {
                                    JsonObject* v1 = jsonRender.objectQueue + (j + i);
                                    if (v1->parentObjectIndex == j + i - 1 && !v1->isObject)
                                    {
                                        arrayLengths[rankIndex] = v1->arrayCount;
                                        ++rankIndex;
                                        arraySize *= v1->arrayCount;
                                        //arrayWrapper.SetSize(v1->arrayCount);
                                    }
                                    else
                                    {
                                        throw new Exception("无法满足秩");
                                    }
                                }

                                myObject.arrayNowItemSize = arraySize / v->arrayCount;

                                myObject.obj = myObject.arrayWrap.CreateArray(arraySize, arrayLengths,
                                    out myObject.objPtr, out myObject.bytePtr, out myObject.gcHandle, out myObject.arrayItemTypeSize);

                                myObject.arrayNowItemSize *= myObject.arrayItemTypeSize;

                            }

                            //父对象是容器就延迟赋值
                            if (parentCollection != null)
                            {
                                setValues[setValuesIndex++] = i; if (setValuesIndex == setValuesLength) { ResizeSetValues(); }
                            }
                            else
                            {
                                //对象是属性延迟赋值
                                if (myObject.isProperty)
                                {
                                    setValues[setValuesIndex++] = i; if (setValuesIndex == setValuesLength) { ResizeSetValues(); }
                                }
                                else
                                {
                                    GeneralTool.SetObject(parentObject.bytePtr + myObject.offset, myObject.obj);
                                }
                            }
                            break;
                    }

                }




                //goto Dubg;

                //基本类型赋值  
                for (int i = 0; i < jsonRender.poolIndex; i++)
                {
                    var v = jsonRender.pool[i];
                    CreateObjectItem myObject = createObjectItems[v.objectQueueIndex];
                    JsonObject* parent = jsonRender.objectQueue + v.objectQueueIndex;
                    if (parent->isObject)
                    {
                        ReadCollectionLink collection = myObject.readCollection;
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
                        ReadCollectionLink collection = myObject.readCollection;
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
                                itemType = myObject.arrayElementType;
                                itemTypeCode = myObject.arrayElementTypeCode;
                            }
                            byte* pByte = myObject.bytePtr + myObject.arrayItemTypeSize * v.arrayIndex;

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
                                                Array Arrays = Enum.GetValues(myObject.arrayElementType);
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
                    if (setValuesOrderLength != setValuesLength) { ResizeSetValuesOrder(); }
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
                        ReadCollectionLink collection = myObject.readCollection;
                        ReadCollectionLink parentCollection = parentObject.readCollection;
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
                                TypeCode typeCode = Type.GetTypeCode(myObject.type);
                                switch (typeCode)
                                {
                                    case TypeCode.Boolean:
                                        myObject.propertyDelegateItem.setBoolean(parentObject.objPtr, (bool)myObject.obj);
                                        break;
                                    case TypeCode.Char:
                                        myObject.propertyDelegateItem.setChar(parentObject.objPtr, (Char)myObject.obj);
                                        break;
                                    case TypeCode.SByte:
                                        myObject.propertyDelegateItem.setSByte(parentObject.objPtr, (SByte)myObject.obj);
                                        break;
                                    case TypeCode.Byte:
                                        myObject.propertyDelegateItem.setByte(parentObject.objPtr, (Byte)myObject.obj);
                                        break;
                                    case TypeCode.Int16:
                                        myObject.propertyDelegateItem.setInt16(parentObject.objPtr, (Int16)myObject.obj);
                                        break;
                                    case TypeCode.UInt16:
                                        myObject.propertyDelegateItem.setUInt16(parentObject.objPtr, (UInt16)myObject.obj);
                                        break;
                                    case TypeCode.Int32:
                                        myObject.propertyDelegateItem.setInt32(parentObject.objPtr, (Int32)myObject.obj);
                                        break;
                                    case TypeCode.UInt32:
                                        myObject.propertyDelegateItem.setUInt32(parentObject.objPtr, (UInt32)myObject.obj);
                                        break;
                                    case TypeCode.Int64:
                                        myObject.propertyDelegateItem.setInt64(parentObject.objPtr, (Int64)myObject.obj);
                                        break;
                                    case TypeCode.UInt64:
                                        myObject.propertyDelegateItem.setUInt64(parentObject.objPtr, (UInt64)myObject.obj);
                                        break;
                                    case TypeCode.Single:
                                        myObject.propertyDelegateItem.setSingle(parentObject.objPtr, (Single)myObject.obj);
                                        break;
                                    case TypeCode.Double:
                                        myObject.propertyDelegateItem.setDouble(parentObject.objPtr, (Double)myObject.obj);
                                        break;
                                    case TypeCode.Decimal:
                                        myObject.propertyDelegateItem.setDecimal(parentObject.objPtr, (Decimal)myObject.obj);
                                        break;
                                    case TypeCode.DateTime:
                                        myObject.propertyDelegateItem.setDateTime(parentObject.objPtr, (DateTime)myObject.obj);
                                        break;
                                    case TypeCode.Object:
                                    case TypeCode.String:
                                        if (myObject.obj == null)
                                        {
                                            myObject.propertyDelegateItem.setVoidPtr(parentObject.objPtr, myObject.objPtr);
                                        }
                                        else
                                        {
                                            myObject.propertyDelegateItem.setObject(parentObject.objPtr, myObject.obj);
                                        }
                                        break;
                                    default:
                                        break;
                                }
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
            JsonObject* parent = jsonRender.objectQueue + value->objectQueueIndex;
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
                             myObject = createObjectItems[value->objectQueueIndex];
                            if (myObject.type == typeof(Type))
                            //if (fieldInfo.fieldType.IsSubclassOf(typeof(Type)))
                            {
                                return UnsafeOperation.GetType(new string(str, value->valueStringStart, value->valueStringLength));
                            }
                            break;
                        default:
                             myObject = createObjectItems[value->objectQueueIndex];
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
