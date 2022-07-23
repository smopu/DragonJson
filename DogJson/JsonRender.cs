using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DogJson
{
    public enum JsonValueType : byte
    {
        None = 0,
        String = 1,
        Long = 2,
        Double = 3,
        Boolean = 4,
        Object = 5,
        Array = 6,
    }
    public unsafe class JsonRender
    {
        static double[] pow309 = new double[309];
        static double[] pow309negative = new double[309];
        private static bool initializedPow = false;

        private static object mutex = new object();
        private static bool initialized = false;
        private static JsonRender _ins = null;
        IJsonRenderToObject jsonRenderToObject;
        public static JsonRender Instance
        {
            get
            {
                if (!initialized)
                {
                    lock (mutex)
                    {
                        if (_ins == null)
                        {
                            _ins = new JsonRender();
                            initialized = true;
                        }
                    }
                }
                return _ins;
            }
        }

        ~JsonRender() 
        {
            Marshal.FreeHGlobal(stackIntPtr);
            Marshal.FreeHGlobal(objectQueueIntPtr);
            Marshal.FreeHGlobal(poolIntPtr);
            Marshal.FreeHGlobal(stringQueueIntPtr);
        }

        public unsafe JsonRender(
            IJsonRenderToObject jsonRenderToObject = null,
            int jsonStackLength = 1024, int poolLength = 65535)
        {
            if (CollectionManager.IsStart == false)
            {
                CollectionManager.Start();
            }
            if (jsonRenderToObject == null)
            {
                this.jsonRenderToObject = new AddrToObject2();
            }
            else
            {
                this.jsonRenderToObject = jsonRenderToObject;
            }
            if (!initializedPow)
            {
                lock (pow309)
                {
                    lock (pow309negative)
                    {
                        for (int i = 0; i < 309; i++)
                        {
                            pow309[i] = Math.Pow(10, i);
                            pow309negative[i] = Math.Pow(10, -i);
                        }
                        initializedPow = true;
                    }
                }
            }

            this.stackLength = 2;//jsonStackLength
            this.poolLength = 12;//poolLength
            this.objectQueueLength = 12;


            //stackIntPtr = Marshal.AllocHGlobal(jsonStackLength * sizeof(typeof(JsonObject*)));
            stackIntPtr = Marshal.AllocHGlobal(jsonStackLength * sizeof(int));
            objectQueueIntPtr = Marshal.AllocHGlobal(objectQueueLength * sizeof(JsonObject));
            poolIntPtr = Marshal.AllocHGlobal(poolLength * sizeof(JsonValue));
            stringQueueIntPtr = Marshal.AllocHGlobal(stringQueueLength * sizeof(char));

            
            pool = (JsonValue*)poolIntPtr.ToPointer();
            objectQueue = (JsonObject*)objectQueueIntPtr.ToPointer();

            //byte* startItemOffcet;
            //jsonObjects = (JsonObject[])arrayWrapper.CreateArrayOne(typeof(JsonObject), objectQueueLength + 10, out byte* objPtr, out startItemOffcet, out GCHandle gCHandle, out int itemSize);
            //objectQueue = (JsonObject*)startItemOffcet;


            //stack = (JsonObject**)stackIntPtr.ToPointer();
            //stack = new int[jsonStackLength];
            stack = (int*)stackIntPtr.ToPointer();
            stringQueue = (char*)stringQueueIntPtr.ToPointer();


            fixed (char* vs = "true")
            {
                truelong = *(long*)vs;
            }
            fixed (char* vs = "fals")
            {
                falslong = *(long*)vs;
            }
            fixed (char* vs = "null")
            {
                nulllong = *(long*)vs;
            }
            fixed (char* vs = "alse")
            {
                alselong = *(long*)vs;
            }
            fixed (char* vs = "type")
            {
                typelong = *(long*)vs;
            }
            fixed (char* vs = "valu")
            {
                valulong = *(long*)vs;
            }
            fixed (char* vs = "crea")
            {
                crealong = *(long*)vs;
            }
            fixed (char* vs = "te")
            {
                telong = *(int*)vs;
            }
        }

        char* stringQueue;
        int stringQueueLength = 100;
        IntPtr stringQueueIntPtr;


        //JsonObject** stack;
        //int[] stack;
        int* stack;
        IntPtr stackIntPtr;
        int stackLength = 1024;
        int stackIndex = 0;


        public JsonValue* pool;
        IntPtr poolIntPtr;
        int poolLength = 65536;
        public int poolIndex = 0;

        ArrayWrapper arrayWrapper = new ArrayWrapper();

        public JsonObject[] jsonObjects;
        public JsonObject* objectQueue;
        int objectQueueLength = 65536;
        IntPtr objectQueueIntPtr;
        public int objectQueueIndex = 0;

        unsafe void ResizeObjectQueue()
        {
            /*
            objectQueueLength *= 2;
            byte* startItemOffcet;
            //JsonObject[] var_jsonObjects = (JsonObject[])arrayWrapper.CreateArrayOne(typeof(JsonObject), objectQueueLength + 10, out byte* objPtr, out startItemOffcet, out GCHandle gCHandle, out int itemSize);
            //JsonObject* var_jobjectQueue = (JsonObject*)startItemOffcet;
            JsonObject* var_jobjectQueue = (JsonObject*)(Marshal.AllocHGlobal((objectQueueLength * 2 + 10) * sizeof(JsonObject)).ToPointer());

            //int size = sizeof(typeof(JsonObject));
            //int size2 = sizeof(JsonObject);

            GeneralTool.Memcpy(var_jobjectQueue, objectQueue, (objectQueueIndex) * sizeof(JsonObject));
            //Array.Copy(jsonObjects, var_jsonObjects, objectQueueIndex);
            //jsonObjects = var_jsonObjects;
            objectQueue = var_jobjectQueue;

            stackNow = objectQueue + stack[stackIndex];
            objectNow = objectQueue + (objectQueueIndex - 1);

            return;
            // */
            //objectQueueLength = 65536;

            objectQueueLength *= 2;
            objectQueueIntPtr = Marshal.ReAllocHGlobal(objectQueueIntPtr, new IntPtr((objectQueueLength + 10) * sizeof(JsonObject)));
            //IntPtr copy = Marshal.AllocHGlobal((objectQueueLength + 10) * sizeof(JsonObject));
            //JsonObject* objectQueueCopy = (JsonObject*)copy.ToPointer();

            //GeneralTool.Memcpy(objectQueueCopy, objectQueue, objectQueueIndex * sizeof(JsonObject));
            objectQueue = (JsonObject*)objectQueueIntPtr.ToPointer();

            // Marshal.FreeHGlobal(objectQueueIntPtr);
            //objectQueueIntPtr = copy;

            stackNow = objectQueue + stack[stackIndex];
            objectNow = objectQueue + (objectQueueIndex - 1);
        }


        unsafe void ResizeStack()
        {
            stackLength *= 2;
            //Array.Resize(ref stack, stackLength);
            stackIntPtr = Marshal.ReAllocHGlobal(stackIntPtr, new IntPtr(stackLength * sizeof(int)));
            //Marshal.FreeHGlobal(stackIntPtr);
            stack = (int*)stackIntPtr.ToPointer();
        }

        unsafe void ResizePool()
        {
            poolLength *= 2;
            poolIntPtr = Marshal.ReAllocHGlobal(poolIntPtr, new IntPtr(poolLength * sizeof(JsonValue)));
            //Marshal.FreeHGlobal(poolIntPtr);
            pool = (JsonValue*)poolIntPtr.ToPointer(); 
            
            json_value = pool + (poolIndex - 1);
        }


        long truelong;
        long falslong;
        long nulllong;
        long alselong;
        long typelong;
        long valulong;
        long crealong;
        int telong;


        public class CreateObjectItem
        {
            public CreateObjectItem(int index)
            {
                this.index = index;
            }
            public object obj;
            public Type type;
            public Type sourceType;
            public FieldInfo fieldInfo;
            public string key;
            public Array objArray;
            public TypeCode ArrayItemTypeCode;
            public Type ArrayItemType;
            public int ArrayRank;
            public int[] ArrayRankLengths;
            public int[] ArrayRankIndex;

            public JsonObject jsonObject;

            public List<CreateObjectItem> sub = new List<CreateObjectItem>();
            public bool isValueType;
            public int index;
            public IReadCollectionObject collectionArray;
            public IReadCollectionObject collectionObject;
        }


        int keyStringStart = 0;
        int keyStringLength = 0;
        int vStringStart = 0;
        int vStringLength = 0;



        unsafe string Debug(char* vs, int length, int index, string txt)
        {
            int line = 1;
            {
                char* now = vs;
                for (int i = 0; i < index; i++)
                {
                    if (*now == '\n')
                    {
                        ++line;
                    }
                    ++now;
                }
            }
            txt += " 第" + line + "行";
            return txt;
        }

        JsonObject* objectNow = null;
        JsonObject* stackNow = null;
        JsonValue* json_value = null;
        JsonValue* poolNow = null;
        public unsafe void ReadJsonText(char* startChar, int length)
        {
            {
                char* now = startChar;

                stackIndex = 0;
                poolIndex = 0;
                keyStringLength = 0;
                keyStringStart = 0;
                objectQueueIndex = 0;

                double v_double = 0;
                double v_decimal = 1;
                long v_long = 0;
                stackNow = null;
                objectNow = null;
                bool fu = false;
                int i = 0;
                //先读初始状态 这里 先只支持{开头的数据
                for (; i < length; ++i, ++now)
                {
                    switch (*now)
                    {
                        case '{':
                            objectNow = objectQueue + objectQueueIndex;
                            objectNow->objectQueueIndex = objectQueueIndex;
                            objectNow->parentObjectIndex = -1;
                            objectNow->keyStringStart = null;
                            objectNow->keyStringLength = 1;
                            objectNow->isObject = true;

                            objectNow->_startCommand = 0;
                            objectNow->_startType = 0;
                            objectNow->objectNext = -1;
                            

                            stack[stackIndex] = objectQueueIndex;
                            stackNow = objectQueue + stack[stackIndex];
                            ++objectQueueIndex; 
                            goto Run;
                        case ' ':
                        case '\t':
                        case '\r':
                        case '\n':
                            break;

                        case '"':
                        case ':':
                        case '}':
                        case '[':
                        case ']':
                        case ',':
                        default:
                            throw new Exception(Debug(startChar, length, i, "错误开头 " + *now));
                    }
                }
            Run:

                do
                {
                Loop:
                    if (stackIndex == -1)
                    {
                        break;
                    }
                    if (stackNow->isObject)
                    {
                        goto State_Object;
                    }
                    else
                    {
                        goto State_Array;
                    }
                State_Object:
                    for (++i, ++now; i < length; ++i, ++now)
                    {
                        switch (*now)                                                             
                        {
                            case '"':
                                keyStringStart = i + 1;
                                //再找" 
                                for (++i, ++now; i < length; ++i, ++now)
                                {
                                    if (*now == '"')
                                    {
                                        keyStringLength = i - keyStringStart;
                                        for (++i, ++now; i < length; ++i, ++now)
                                        {
                                            if (*now == ':')
                                            {
                                                if (*(startChar + keyStringStart) == '#')
                                                {
                                                    long nameLong = *(long*)(startChar + keyStringStart + 1);
                                                    switch (keyStringLength)
                                                    {
                                                        case 5:
                                                            if (nameLong == typelong)
                                                            {
                                                                for (++i, ++now; i < length; ++i, ++now)
                                                                {
                                                                    switch (*now)
                                                                    {
                                                                        case '"':
                                                                            int typeStartIndex = i + 1;

                                                                            for (++i, ++now; i < length; ++i, ++now)
                                                                            {
                                                                                if (*now == '"')
                                                                                {
                                                                                    int typeLength = i - typeStartIndex;
                                                                                    stackNow ->typeStartIndex = typeStartIndex;
                                                                                    stackNow->typeLength = typeLength;
                                                                                    goto Run3;
                                                                                }
                                                                            }

                                                                            break;
                                                                        case ' ':
                                                                        case '\t':
                                                                        case '\r':
                                                                        case '\n':
                                                                            break;
                                                                        default:
                                                                            throw new Exception(Debug(startChar, length, i, "#type 后面必须是字符串 " + *now));
                                                                    }
                                                                }
                                                            }
                                                            break;

                                                        case 6:
                                                            if (nameLong == valulong && *(startChar + keyStringStart + 5) == 'e')
                                                            {
                                                                stackNow->isCommandValue = true;
                                                                goto Run1;
                                                            }
                                                            break;

                                                        case 7:
                                                            if (nameLong == crealong
                                                             && *(int*)(startChar + keyStringStart + 5) == telong
                                                             )
                                                            {
                                                                stackNow->isConstructor = true;
                                                                goto Run1;
                                                            }
                                                            break;
                                                        default:
                                                            throw new Exception(Debug(startChar, length, i, "# 未知指令" + *now));
                                                    }
                                                            throw new Exception(Debug(startChar, length, i, "# 未知指令" + *now));
                                                Run3:
                                                    // 下必 ,} 
                                                    for (++i, ++now; i < length; ++i, ++now)
                                                    {
                                                        switch (*now)
                                                        {
                                                            case ',':
                                                                goto State_Object;
                                                            case '}':
                                                                //string入队
                                                                //出栈 
                                                                --stackIndex; stackNow = objectQueue + stack[stackIndex];
                                                                if (stackNow->isObject)
                                                                {
                                                                    goto State_Object;
                                                                }
                                                                else
                                                                {
                                                                    (objectQueue + stack[stackIndex + 1])->arrayIndex = stackNow->arrayCount;
                                                                    ++stackNow->arrayCount;
                                                                    goto State_Array;
                                                                }
                                                            //goto Loop;
                                                            case ' ':
                                                            case '\t':
                                                            case '\r':
                                                            case '\n':
                                                                break;
                                                            default:
                                                                throw new Exception("key:value后面只能是,或者}");
                                                        }
                                                    }
                                                    throw new Exception("key:value后面必须要跟,或者} 未正常 结尾");
                                                }


                                                goto Run1;
                                            }
                                        }
                                        throw new Exception("key后面需要:");
                                    }
                                }
                                throw new Exception("字符串未结束 ");
                            case '}':

                                //objectNext 赋值
                                stackNow->objectNext = objectQueueIndex;
                                //出栈 
                                --stackIndex; 
                                if (stackIndex == -1)
                                {
                                    goto BACK;
                                }
                                stackNow = objectQueue + stack[stackIndex];
                                if (stackNow->isObject)
                                {
                                    goto State_Object;
                                }
                                else
                                {
                                    (objectQueue + stack[stackIndex + 1])->arrayIndex = stackNow->arrayCount;
                                    ++stackNow->arrayCount;
                                    goto State_Array;
                                }

                            case ' ':
                            case '\t':
                            case '\r':
                            case '\n':
                            case ',':
                                break;
                            default:
                                throw new Exception(Debug(startChar, length, i, "对象 下面只能有keyvalue结构 " + *now));
                        }
                    }

                    //下一个必出" 或}
                    for (++i, ++now; i < length; ++i, ++now)
                    {
                        switch (*now)
                        {
                            case '"':
                                keyStringStart = i + 1;
                                //再找" 
                                for (++i, ++now; i < length; ++i, ++now)
                                {
                                    if (*now == '"')
                                    {
                                        keyStringLength = i - keyStringStart;
                                        for (++i, ++now; i < length; ++i, ++now)
                                        {
                                            if (*now == ':')
                                            {
                                                goto Run1;
                                            }
                                        }
                                        throw new Exception(Debug(startChar, length, i, "key后面需要: " + *now));
                                    }
                                }
                                throw new Exception(Debug(startChar, length, i, "字符串未结束 " + *now));

                            case '}':
                                //objectNext 赋值
                                stackNow->objectNext = objectQueueIndex;
                                //出栈 

                                --stackIndex; 
                                if (stackIndex == -1)
                                {
                                    goto BACK;
                                }
                                stackNow = objectQueue + stack[stackIndex];
                                if (stackNow->isObject)
                                {
                                    goto State_Object;
                                }
                                else
                                {
                                    (objectQueue + stack[stackIndex + 1])->arrayIndex = stackNow->arrayCount;
                                    ++stackNow->arrayCount;
                                    goto State_Array;
                                }

                            case ' ':
                            case '\t':
                            case '\r':
                            case '\n':
                            case ',':
                                break;
                            default:
                                throw new Exception(Debug(startChar, length, i, "对象 下面只能有keyvalue结构 " + *now));
                        }
                    }
                Run1:
                    //key:后面只能是 布尔、数值、对象 、数组 再找下 " { [ 其他  不能是 } : , ]  

                    for (++i, ++now; i < length; ++i, ++now)
                    {
                        switch (*now)
                        {
                            case '"':
                                vStringStart = i + 1;

                                for (++i, ++now; i < length; ++i, ++now)
                                {
                                    if (*now == '"')
                                    {
                                        vStringLength = i - vStringStart;
                                        goto Run2;
                                    }
                                }
                                throw new Exception(Debug(startChar, length, i, "字符串未结束 " + *now));
                            Run2:
                                // 下必 ,} 
                                for (++i, ++now; i < length; ++i, ++now)
                                {
                                    switch (*now)
                                    {
                                        case ',':
                                            //string入队
                                            //json_value = &pool+(poolIndex++);

                                            poolNow = pool + poolIndex;
                                            poolNow->type = JsonValueType.String;
                                            poolNow->keyStringLength = keyStringLength;
                                            poolNow->keyStringStart = keyStringStart;
                                            poolNow->valueStringStart = vStringStart;
                                            poolNow->valueStringLength = vStringLength;
                                            poolNow->objectQueueIndex = stackNow->objectQueueIndex;
                                            ++poolIndex;
                                            if (poolIndex == poolLength)
                                            {
                                                ResizePool();
                                            }

                                            goto State_Object;
                                        case '}':
                                            //string入队
                                            poolNow = pool + poolIndex;
                                            poolNow->type = JsonValueType.String;
                                            poolNow->keyStringLength = keyStringLength;
                                            poolNow->keyStringStart = keyStringStart;
                                            poolNow->valueStringStart = vStringStart;
                                            poolNow->valueStringLength = vStringLength;
                                            poolNow->objectQueueIndex = stackNow->objectQueueIndex;
                                            ++poolIndex;
                                            if (poolIndex == poolLength)
                                            {
                                                ResizePool();
                                            }

                                            //objectNext 赋值
                                            stackNow->objectNext = objectQueueIndex;

                                            //出栈 
                                            --stackIndex; 
                                            if (stackIndex == -1)
                                            {
                                                goto BACK;
                                            }
                                            stackNow = objectQueue + stack[stackIndex];
                                            if (stackNow->isObject)
                                            {
                                                goto State_Object;
                                            }
                                            else
                                            {
                                                (objectQueue + stack[stackIndex + 1])->arrayIndex = stackNow->arrayCount;
                                                ++stackNow->arrayCount;
                                                goto State_Array;
                                            }
                                        //goto Loop;
                                        case ' ':
                                        case '\t':
                                        case '\r':
                                        case '\n':
                                            break;
                                        default:
                                        throw new Exception(Debug(startChar, length, i, "key:value后面只能是,或者} " + *now));
                                    }
                                }
                                throw new Exception(Debug(startChar, length, i, "key:value后面必须要跟,或者} 未正常 结尾 " + *now));

                            case '{':
                                //新建object赋予string名字 object入栈
                                objectNow = objectQueue + objectQueueIndex;
                                objectNow->objectQueueIndex = objectQueueIndex;
                                objectNow->parentObjectIndex = stackNow->objectQueueIndex;
                                objectNow->keyStringStart = startChar + keyStringStart;
                                objectNow->keyStringLength = keyStringLength;
                                objectNow->isObject = true;
                                objectNow->_startCommand = 0;
                                objectNow->_startType = 0;
                                objectNow->objectNext = -1;

                                if (++stackIndex == stackLength)
                                {
                                    ResizeStack();
                                }
                                stack[stackIndex] = objectQueueIndex;
                                //stack[++stackIndex] = objectNow;
                                stackNow = objectQueue + stack[stackIndex];
                                ++objectQueueIndex;
                                if (objectQueueIndex == objectQueueLength)
                                {
                                    ResizeObjectQueue();
                                }
                                goto State_Object;

                            case '[':
                                //新建array赋予string名字 array入栈
                                if (stackNow->isCommandValue)
                                {
                                    stackNow->isObject = false;
                                    stackNow->arrayCount = 0;
                                    goto State_Array;
                                }
                                else
                                {
                                    objectNow = objectQueue + objectQueueIndex;
                                    objectNow->objectQueueIndex = objectQueueIndex;
                                    objectNow->parentObjectIndex = stackNow->objectQueueIndex;
                                    objectNow->keyStringStart = startChar + keyStringStart;
                                    objectNow->keyStringLength = keyStringLength;
                                    objectNow->isObject = false;
                                    objectNow->_startCommand = 0;
                                    objectNow->_startType = 0;
                                    objectNow->objectNext = -1;

                                    if (++stackIndex == stackLength)
                                    {
                                        ResizeStack();
                                    }
                                    stack[stackIndex] = objectQueueIndex;
                                    //stack[++stackIndex] = objectNow;
                                    stackNow = objectQueue + stack[stackIndex];
                                    ++objectQueueIndex;

                                    if (objectQueueIndex == objectQueueLength)
                                    {
                                        ResizeObjectQueue();
                                    }
                                    goto State_Array;
                                }

                            case 't':
                                if (i < length + 4)
                                {
                                    long nameLong = *(long*)(now);
                                    if (nameLong == truelong)
                                    {
                                        json_value = pool + poolIndex; if (++poolIndex == poolLength) { ResizePool(); } 
                                        json_value->_startType = 0; 
                                        json_value->type = JsonValueType.Boolean;
                                        json_value->valueBool = true; 
                                         now += 4;
                                        i += 4;
                                        goto Value;
                                    }
                                }
                                throw new Exception(Debug(startChar, length, i, "key:后面的value解析错误 " + *now));
                            case 'n':
                                if (i < length + 4)
                                {
                                    long nameLong = *(long*)(now);
                                    if (nameLong == nulllong)
                                    {
                                        json_value = pool + poolIndex; if (++poolIndex == poolLength) { ResizePool(); };
                                        json_value->_startType = 0;
                                        json_value->type = JsonValueType.None;
                                        now += 4;
                                        i += 4;
                                        goto Value;
                                    }
                                }
                                throw new Exception(Debug(startChar, length, i, "key:后面的value解析错误 " + *now));
                            case 'f':
                                if (i < length + 4)
                                {
                                    ++now;
                                    ++i;
                                    long nameLong = *(long*)(now);
                                    if (nameLong == alselong)
                                    {
                                        json_value = pool + poolIndex; if (++poolIndex == poolLength) { ResizePool(); };
                                        json_value->_startType = 0;
                                        json_value->type = JsonValueType.Boolean;
                                        json_value->valueBool = false;
                                        now += 4;
                                        i += 4;
                                        goto Value;
                                    }
                                }
                                throw new Exception(Debug(startChar, length, i, "key:后面的value解析错误 " + *now));
                            default:
                                fu = false;
                                if (*now < '0' || *now > '9')
                                {
                                    if (*now == '-')
                                    {
                                        fu = true;
                                        //负号后面可以加空格
                                        for (++i, ++now; i < length; ++i, ++now)
                                        {
                                            if (*now != ' ')
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else if (*now == '+')
                                    {
                                        ++i; ++now;
                                    }
                                    else
                                    {
                                        throw new Exception(Debug(startChar, length, i, "key:value后面必须要跟,或者} " + *now));
                                    }
                                }
                                json_value = pool + poolIndex; if (++poolIndex == poolLength) { ResizePool(); };
                                json_value->_startType = 0;
                                v_long = (*now - '0');
                                for (++i, ++now; i < length; ++i, ++now)
                                {
                                    if (*now < '0' || *now > '9')
                                    {
                                        if (*now == '.')
                                        {
                                            goto Dot;
                                        }
                                        else if (*now == 'E' || *now == 'e')
                                        {
                                            --now;
                                            --i;
                                            goto Dot;
                                        }
                                        else
                                        {
                                            json_value->type = JsonValueType.Long;
                                            json_value->valueLong = fu ? -v_long : v_long;
                                            goto Value;
                                        }
                                    }
                                    else
                                    {
                                        v_long *= 10;
                                        v_long += (*now - '0');
                                    }
                                }
                                //long
                                throw new Exception(Debug(startChar, length, i, "key:value后面必须要跟,或者} " + *now));

                            Dot:
                                v_double = v_long;
                                //点
                                v_decimal = 0.1;
                                for (++i, ++now; i < length; ++i, ++now)
                                {
                                    if (*now < '0' || *now > '9')
                                    {
                                        if (*now == 'E' || *now == 'e')
                                        {
                                            ++now; ++i;
                                            if (*now == '-')
                                            {
                                                v_long = 0;

                                                for (++i, ++now; i < length; ++i, ++now)
                                                {
                                                    if (*now < '0' || *now > '9')
                                                    {
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        v_long *= 10;
                                                        v_long += (*now - '0');
                                                    }
                                                }
                                                v_double *= pow309negative[v_long];
                                                json_value->type = JsonValueType.Double;
                                                json_value->valueDouble = fu ? -v_double : v_double;
                                                goto Value;
                                            }
                                            else if (*now == '+')
                                            {
                                                v_long = 0;

                                                for (++i, ++now; i < length; ++i, ++now)
                                                {
                                                    if (*now < '0' || *now > '9')
                                                    {
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        v_long *= 10;
                                                        v_long += (*now - '0');
                                                    }
                                                }
                                                v_double *= pow309[v_long];
                                                json_value->type = JsonValueType.Double;
                                                json_value->valueDouble = fu ? -v_double : v_double;
                                                goto Value;
                                            }

                                        }
                                        break;
                                    }
                                    else
                                    {
                                        v_double += (*now - '0') * v_decimal;
                                        v_decimal *= 0.1;
                                    }
                                }
                                json_value->type = JsonValueType.Double;
                                json_value->valueDouble = fu ? -v_double : v_double;
                            //goto Value;
                            //throw new Exception("key:后面的value解析错误");

                            Value:

                                for (; i < length; ++i, ++now)
                                {
                                    switch (*now)
                                    {
                                        case ' ':
                                        case '\t':
                                        case '\r':
                                        case '\n':
                                            break;

                                        case ',':
                                            json_value->keyStringLength = keyStringLength;
                                            json_value->keyStringStart = keyStringStart;
                                            json_value->objectQueueIndex = stackNow->objectQueueIndex;


                                            goto State_Object;
                                        case '}':

                                            json_value->keyStringLength = keyStringLength;
                                            json_value->keyStringStart = keyStringStart;
                                            json_value->objectQueueIndex = stackNow->objectQueueIndex;

                                            //objectNext 赋值
                                            stackNow->objectNext = objectQueueIndex;
                                            //出栈 
                                            --stackIndex;
                                            if (stackIndex == -1)
                                            {
                                                goto BACK;
                                            }
                                            stackNow = objectQueue + stack[stackIndex];
                                            if (stackNow->isObject)
                                            {
                                                goto State_Object;
                                            }
                                            else
                                            {
                                                (objectQueue + stack[stackIndex + 1])->arrayIndex = stackNow->arrayCount;
                                                ++stackNow->arrayCount;
                                                goto State_Array;
                                            }
                                        //goto Loop;
                                        default:
                                            throw new Exception("key:value后面必须要跟,或者} ,");
                                    }
                                }
                                throw new Exception("key:value后面必须要跟,或者} ,未正常结尾");

                            case ' ':
                            case '\t':
                            case '\r':
                            case '\n':
                                break;

                            case ':':
                            case '}':
                            case ',':
                            case ']':
                                throw new Exception("key:后面不能是" + *now);
                        }
                    }
                //}
                ////else
                //{

                State_Array:
                    //" ]
                    for (++i, ++now; i < length; ++i, ++now)
                    {
                        switch (*now)
                        {
                            case '"':
                                json_value = pool + poolIndex; if (++poolIndex == poolLength) { ResizePool(); };
                                json_value->_startType = 0;
                                vStringStart = i + 1;
                                //再找" 
                                for (++i, ++now; i < length; ++i, ++now)
                                {
                                    if (*now == '"')
                                    {
                                        vStringLength = i - vStringStart;
                                        for (++i, ++now; i < length; ++i, ++now)
                                        {
                                            //必出 , ]
                                            if (*now == ',')
                                            {
                                                json_value->type = JsonValueType.String;
                                                json_value->valueStringStart = vStringStart;
                                                json_value->valueStringLength = vStringLength;
                                                json_value->objectQueueIndex = stackNow->objectQueueIndex;
                                                json_value->arrayIndex = stackNow->arrayCount;


                                                ++stackNow->arrayCount;
                                                goto State_Array;
                                            }
                                            else if (*now == ']')
                                            {
                                                json_value->type = JsonValueType.String;
                                                json_value->valueStringStart = vStringStart;
                                                json_value->valueStringLength = vStringLength;
                                                json_value->objectQueueIndex = stackNow->objectQueueIndex;
                                                json_value->arrayIndex = stackNow->arrayCount;

                                                //objectNext 赋值
                                                stackNow->objectNext = objectQueueIndex;
                                                ++stackNow->arrayCount;
                                                //出栈
                                                --stackIndex; stackNow = objectQueue + stack[stackIndex];
                                                goto Loop;
                                            }
                                        }
                                        throw new Exception("key后面需要:");
                                    }
                                }
                                throw new Exception("字符串未结束 ");

                            case '{':
                                //数组元素没有Key
                                // { 新建 object 入栈
                                objectNow = objectQueue + objectQueueIndex;
                                objectNow->objectQueueIndex = objectQueueIndex;
                                objectNow->parentObjectIndex = stackNow->objectQueueIndex;
                                objectNow->isObject = true;
                                objectNow->arrayIndex = 0;
                                objectNow->_startCommand = 0;
                                objectNow->_startType = 0;
                                objectNow->objectNext = -1;

                                if (++stackIndex == stackLength)
                                {
                                    ResizeStack();
                                }
                                stack[stackIndex] = objectQueueIndex;
                                //stack[++stackIndex] = objectNow; 
                                stackNow = objectQueue + stack[stackIndex];
                                ++objectQueueIndex;

                                if (objectQueueIndex == objectQueueLength)
                                {
                                    ResizeObjectQueue();
                                }

                                goto State_Object;
                            //goto Loop;

                            case '[':
                                // [新建 array 入栈
                                objectNow = objectQueue + objectQueueIndex;
                                objectNow->_startCommand = 0;
                                objectNow->_startType = 0;
                                objectNow->objectNext = -1;

                                objectNow->objectQueueIndex = objectQueueIndex;
                                objectNow->parentObjectIndex = stackNow->objectQueueIndex;
                                objectNow->isObject = false;
                                objectNow->arrayCount = 0;
                                objectNow->arrayIndex = stackNow->arrayCount;

                                //json_value->arrayIndex = stackNow->arrayCount;
                                ++stackNow->arrayCount;

                                if (++stackIndex == stackLength)
                                {
                                    ResizeStack();
                                }
                                stack[stackIndex] = objectQueueIndex;
                                //stack[++stackIndex] = objectNow; 
                                stackNow = objectQueue + stack[stackIndex];
                                ++objectQueueIndex;
                                if (objectQueueIndex == objectQueueLength)
                                {
                                    ResizeObjectQueue();
                                }

                                goto State_Array;
                            case 't':
                                if (i < length + 4)
                                {
                                    long nameLong = *(long*)(now);
                                    if (nameLong == truelong)
                                    {
                                        json_value = pool + poolIndex; if (++poolIndex == poolLength) { ResizePool(); };
                                        json_value->type = JsonValueType.Boolean;
                                        json_value->valueBool = true;
                                        json_value->_startType = 0;
                                        now += 4;
                                        i += 4;
                                        goto Value2;
                                    }
                                }
                                throw new Exception("key:后面的value解析错误");
                            case 'n':
                                if (i < length + 4)
                                {
                                    long nameLong = *(long*)(now);
                                    if (nameLong == nulllong)
                                    {
                                        json_value = pool + poolIndex; if (++poolIndex == poolLength) { ResizePool(); };
                                        json_value->type = JsonValueType.None;
                                        json_value->_startType = 0;
                                        now += 4;
                                        i += 4;
                                        goto Value2;
                                    }
                                }
                                throw new Exception("key:后面的value解析错误");
                            case 'f':
                                if (i < length + 4)
                                {
                                    ++now;
                                    ++i;
                                    long nameLong = *(long*)(now);
                                    if (nameLong == alselong)
                                    {
                                        json_value = pool + poolIndex; if (++poolIndex == poolLength) { ResizePool(); };
                                        json_value->type = JsonValueType.Boolean;
                                        json_value->valueBool = false;
                                        json_value->_startType = 0;
                                        now += 4;
                                        i += 4;
                                        goto Value2;
                                    }
                                }
                                throw new Exception("key:后面的value解析错误");
                            default:
                                fu = false;
                                if (*now < '0' || *now > '9')
                                {
                                    if (*now == '-')
                                    {
                                        fu = true;
                                        //负号后面可以加空格
                                        for (++i, ++now; i < length; ++i, ++now)
                                        {
                                            if (*now != ' ')
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else if (*now == '+')
                                    {
                                        ++i; ++now;
                                    }
                                    else
                                    {
                                        throw new Exception("key:value后面必须要跟,或者}" + *now);
                                    }
                                }
                                json_value = pool + poolIndex; if (++poolIndex == poolLength) { ResizePool(); };
                                json_value->_startType = 0;
                                v_long = (*now - '0');
                                for (++i, ++now; i < length; ++i, ++now)
                                {
                                    if (*now < '0' || *now > '9')
                                    {
                                        if (*now == '.')
                                        {
                                            goto Dot2;
                                        }
                                        else if (*now == 'E' || *now == 'e')
                                        {
                                            --now;
                                            --i;
                                            goto Dot2;
                                        }
                                        else
                                        {
                                            json_value->type = JsonValueType.Long;
                                            json_value->valueLong = fu ? -v_long : v_long;
                                            goto Value2;
                                        }
                                    }
                                    else
                                    {
                                        v_long *= 10;
                                        v_long += (*now - '0');
                                    }
                                }
                                //long
                                throw new Exception("key:value后面必须要跟,或者}" + *now);

                            Dot2:
                                v_double = v_long;
                                //点
                                v_decimal = 0.1;
                                for (++i, ++now; i < length; ++i, ++now)
                                {
                                    if (*now < '0' || *now > '9')
                                    {
                                        if (*now == 'E' || *now == 'e')
                                        {
                                            ++now; ++i;
                                            if (*now == '-')
                                            {
                                                v_long = 0;

                                                for (++i, ++now; i < length; ++i, ++now)
                                                {
                                                    if (*now < '0' || *now > '9')
                                                    {
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        v_long *= 10;
                                                        v_long += (*now - '0');
                                                    }
                                                }
                                                v_double *= pow309negative[v_long];
                                                json_value->type = JsonValueType.Double;
                                                json_value->valueDouble = fu ? -v_double : v_double;
                                                goto Value2;
                                            }
                                            else if (*now == '+')
                                            {
                                                v_long = 0;

                                                for (++i, ++now; i < length; ++i, ++now)
                                                {
                                                    if (*now < '0' || *now > '9')
                                                    {
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        v_long *= 10;
                                                        v_long += (*now - '0');
                                                    }
                                                }
                                                v_double *= pow309[v_long];
                                                json_value->type = JsonValueType.Double;
                                                json_value->valueDouble = fu ? -v_double : v_double;
                                                goto Value2;
                                            }

                                        }
                                        break;
                                    }
                                    else
                                    {
                                        v_double += (*now - '0') * v_decimal;
                                        v_decimal *= 0.1;
                                    }
                                }
                                json_value->type = JsonValueType.Double;
                                json_value->valueDouble = fu ? -v_double : v_double;
                            //goto Value;
                            //throw new Exception("key:后面的value解析错误");

                            Value2:
                                json_value->objectQueueIndex = stackNow->objectQueueIndex;

                                for (; i < length; ++i, ++now)
                                {
                                    switch (*now)
                                    {
                                        case ' ':
                                        case '\t':
                                        case '\r':
                                        case '\n':
                                            break;

                                        case ',':
                                            json_value->arrayIndex = stackNow->arrayCount;
                                            ++stackNow->arrayCount;

                                            goto State_Array;
                                        case ']':

                                            if (stackNow->isCommandValue)
                                            {
                                                //下面必是 }
                                                for (++i, ++now; i < length; ++i, ++now)
                                                {
                                                    switch (*now)
                                                    {
                                                        case ' ':
                                                        case '\t':
                                                        case '\r':
                                                        case '\n':
                                                            break;

                                                        case '}':
                                                            //出栈 
                                                            json_value->arrayIndex = stackNow->arrayCount;
                                                            ++stackNow->arrayCount;
                                                            //objectNext 赋值
                                                            stackNow->objectNext = objectQueueIndex;
                                                            //出栈 
                                                            --stackIndex;
                                                            if (stackIndex == -1)
                                                            {
                                                                goto BACK;
                                                            }
                                                            stackNow = objectQueue + stack[stackIndex];
                                                            if (stackNow->isObject)
                                                            {
                                                                goto State_Object;
                                                            }
                                                            else
                                                            {
                                                                (objectQueue + stack[stackIndex + 1])->arrayIndex = stackNow->arrayCount;
                                                                ++stackNow->arrayCount;
                                                                goto State_Array;
                                                            }

                                                        case '"':
                                                        case '{':
                                                        case ':':
                                                        case '[':
                                                        case ']':
                                                        case ',':
                                                        default:
                                                            throw new Exception(Debug(startChar, length, i, "#value[] 后面必须是} 现在是:" + *now));
                                                    }
                                                }
                                                throw new Exception(Debug(startChar, length, i, "#value[] 后面必须是}"));

                                            }
                                            else
                                            {
                                                //出栈 
                                                json_value->arrayIndex = stackNow->arrayCount;
                                                ++stackNow->arrayCount;

                                                //objectNext 赋值
                                                stackNow->objectNext = objectQueueIndex;

                                                --stackIndex; stackNow = objectQueue + stack[stackIndex];
                                                goto Loop;
                                            }
                                        default:
                                            throw new Exception(Debug(startChar, length, i, "array后面必须要跟,或者] , " + *now));
                                    }
                                }
                                throw new Exception("array后面必须要跟,或者] ,未正常结尾");

                            case ' ':
                            case '\t':
                            case '\r':
                            case '\n':
                            case ',':
                                break;
                            case ']':
                                if (stackNow->isCommandValue)
                                {
                                    //下面必是 }
                                    for (++i, ++now; i < length; ++i, ++now)
                                    {
                                        switch (*now)
                                        {
                                            case ' ':
                                            case '\t':
                                            case '\r':
                                            case '\n':
                                                break;

                                            case '}':
                                                //出栈 
                                                //(*jsonStackNow)->objectIndex = poolIndex;
                                                //objectNext 赋值
                                                stackNow->objectNext = objectQueueIndex;
                                                //出栈 
                                                --stackIndex;
                                                if (stackIndex == -1)
                                                {
                                                    goto BACK;
                                                }
                                                stackNow = objectQueue + stack[stackIndex];
                                                if (stackNow->isObject)
                                                {
                                                    goto State_Object;
                                                }
                                                else
                                                {
                                                    (objectQueue + stack[stackIndex + 1])->arrayIndex = stackNow->arrayCount;
                                                    ++stackNow->arrayCount;
                                                    goto State_Array;
                                                }
                                            case '"':
                                            case '{':
                                            case ':':
                                            case '[':
                                            case ']':
                                            case ',':
                                            default:
                                                throw new Exception(Debug(startChar, length, i, "#value[] 后面必须是} 现在是:" + *now));
                                        }
                                    }
                                    throw new Exception(Debug(startChar, length, i, "#value[] 后面必须是}"));

                                }
                                else
                                {
                                    //(*jsonStackNow)->objectIndex = poolIndex;
                                    //objectNext 赋值
                                    stackNow->objectNext = objectQueueIndex;
                                    //出栈 
                                    --stackIndex; stackNow = objectQueue + stack[stackIndex];
                                    goto Loop;
                                }
                            case ':':
                            case '}':
                                throw new Exception("key:后面不能是" + *now);
                        }
                    }

                }
                while (stackIndex >= 0);
            }
        BACK:

            return;
        }

        public unsafe T ReadJsonTextCreateObject<T>(string str)
        {
            int length = str.Length;
            fixed (char* startChar = str)
            {
                ReadJsonText(startChar, length);
                return (T)jsonRenderToObject.CreateObject(this, startChar, length);
            }
        }

        public unsafe object ReadJsonTextCreate(string str)
        {
            int length = str.Length;
            fixed (char* startChar = str)
            {
                ReadJsonText(startChar, length);
                return jsonRenderToObject.CreateObject(this, startChar, length);
            }
        }

        public unsafe void ReadJsonText(string str)
        {
            int length = str.Length;
            fixed (char* startChar = str)
            {
                ReadJsonText(startChar, length);
            }
        }

        public unsafe string EscapeString(char* start, int length)
        {
            if (stringQueueLength < length)
            {
                stringQueueLength = length;
                Marshal.FreeHGlobal(stringQueueIntPtr);
                stringQueueIntPtr = Marshal.AllocHGlobal(stringQueueLength + 2);
                stringQueue = (char*)stringQueueIntPtr.ToPointer();
            }
            int index = 0, i = 0, max = length - 1;
            for (; i < max; ++i)
            {
                if (start[i] == '\\')
                {
                    ++i;
                    switch (start[i])
                    {
                        case 'u':
                            ++i;
                            if (max - i < 3)
                            {
                                i += 3;
                                break;
                            }
                            int u0 = start[i];
                            int u1 = start[i + 1];
                            int u2 = start[i + 2];
                            int u3 = start[i + 3];
                            i += 3;
                            if ('0' > u0 || u0 > '9')
                            {
                                switch (u0)
                                {
                                    case 'a':
                                    case 'A':
                                        u0 = 10;
                                        break;
                                    case 'b':
                                    case 'B':
                                        u0 = 11;
                                        break;
                                    case 'c':
                                    case 'C':
                                        u0 = 12;
                                        break;
                                    case 'd':
                                    case 'D':
                                        u0 = 13;
                                        break;
                                    case 'e':
                                    case 'E':
                                        u0 = 14;
                                        break;
                                    case 'f':
                                    case 'F':
                                        u0 = 15;
                                        break;
                                    default:
                                        u0 = '0';
                                        break;
                                }
                            }
                            else
                            {
                                u0 -= '0';
                            }
                            if ('0' > u1 || u1 > '9')
                            {
                                switch (u1)
                                {
                                    case 'a':
                                    case 'A':
                                        u1 = 10;
                                        break;
                                    case 'b':
                                    case 'B':
                                        u1 = 11;
                                        break;
                                    case 'c':
                                    case 'C':
                                        u1 = 12;
                                        break;
                                    case 'd':
                                    case 'D':
                                        u1 = 13;
                                        break;
                                    case 'e':
                                    case 'E':
                                        u1 = 14;
                                        break;
                                    case 'f':
                                    case 'F':
                                        u1 = 15;
                                        break;
                                    default:
                                        u1 = '0';
                                        break;
                                }
                            }
                            else
                            {
                                u1 -= '0';
                            }
                            if ('0' > u2 || u2 > '9')
                            {
                                switch (u2)
                                {
                                    case 'a':
                                    case 'A':
                                        u2 = 10;
                                        break;
                                    case 'b':
                                    case 'B':
                                        u2 = 11;
                                        break;
                                    case 'c':
                                    case 'C':
                                        u2 = 12;
                                        break;
                                    case 'd':
                                    case 'D':
                                        u2 = 13;
                                        break;
                                    case 'e':
                                    case 'E':
                                        u2 = 14;
                                        break;
                                    case 'f':
                                    case 'F':
                                        u2 = 15;
                                        break;
                                    default:
                                        u2 = '0';
                                        break;
                                }
                            }
                            else
                            {
                                u2 -= '0';
                            }
                            if ('0' > u3 || u3 > '9')
                            {
                                switch (u3)
                                {
                                    case 'a':
                                    case 'A':
                                        u3 = 10;
                                        break;
                                    case 'b':
                                    case 'B':
                                        u3 = 11;
                                        break;
                                    case 'c':
                                    case 'C':
                                        u3 = 12;
                                        break;
                                    case 'd':
                                    case 'D':
                                        u3 = 13;
                                        break;
                                    case 'e':
                                    case 'E':
                                        u3 = 14;
                                        break;
                                    case 'f':
                                    case 'F':
                                        u3 = 15;
                                        break;
                                    default:
                                        u3 = '0';
                                        break;
                                }
                            }
                            else
                            {
                                u3 -= '0';
                            }

                            int over = (u0 << 12) | (u1 << 8) | (u2 << 4) | u3;

                            stringQueue[index++] = (char)over;

                            break;
                        case 't':
                            stringQueue[index] = '\t';
                            ++index;
                            break;
                        case 'v':
                            stringQueue[index] = '\v';
                            ++index;
                            break;
                        case 'n':
                            stringQueue[index] = '\n';
                            ++index;
                            break;
                        case 'r':
                            stringQueue[index] = '\r';
                            ++index;
                            break;
                        case '\\':
                            stringQueue[index] = '\\';
                            ++index;
                            break;
                        case '\"':
                            stringQueue[index] = '\"';
                            ++index;
                            break;
                        default:
                            Console.WriteLine(start[i]);
                            break;
                    }
                }
                else
                {
                    stringQueue[index] = start[i];
                    ++index;
                }
            }

            if (i == max)
            {
                stringQueue[index] = start[i];
                ++index;
            }
            return new string(stringQueue, 0, index);
        }


    }
}
