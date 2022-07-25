using DogJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DogJsonTest
{

    public class TestJsonClassA
    {
        public Action<int, string> testDelegate;
        public Action<int, string> testDelegate2;
        public Action<int, string> testDelegate3;
        public void Fool(int a, string b)
        {
        }
        public void Foo2(int a, string b)
        {
            
        }
        ///*
        private double num;
        public double Num
        {
            get { return num; }
            set { num = value; }
        }

        private B bb;
        public B BB
        {
            get { return bb; }
            set { bb = value; }
        }

        public LinkedList<long> arrayLinkedList;
        public int[,,] arrayArray1;
        public int[,][] arrayArray2;
        public int[] arrayArray3;
        public int[][] arrayArray4;
        public int[,,] arrayArray5;

        public List<B> listB;
        public List<C> listC;
        public List<E> listE;

        public bool b;
        public int kk;
        public string str;
        public C gcc;
        public E gD;
        public E[] gDs;
        public Vector3 v3;
        public TestOB testOB;

        public TclassDCC dcc;
        public TclassDCC3 dcc3;
        public object dcc2;

        public object ddd;
        public object[] objects;
        public TClass001 tClass001;
        public TClass003 testClassDD;
        public TClass003 testClassDD2;
        public TClass003 testClassDD3;
        public TClass003 testClassDD4;
        public TestEnum testEnum;
        public TestEnum testEnum2 { get; set; }
        public TestEnum[] testEnums;

        public IList<int> arrayint2;
        public TclassA Iclass0Z;
        public P_box_3 p3;
        public B[,,] arrayRekn;
        public Stack<int> arrayStack;
        public B[] fd;
        public HashSet<double> arraydouble;
        public Queue<string> arraystring;

        public List<bool> arraybool;

        public Dictionary<int, string> dictionary1;
        public Dictionary<int, B> dictionary2;
        public Dictionary<Vector3, B> dictionary3;
        //*/
        public TstructA tstructA;
    }

    public class TclassA
    {
        public int b;
        public void Fool(int a, string b)
        {
        }
        public void Foo2(int a, string b)
        {
        }
    }

    public class TclassC : TclassA
    {
        public double value;
        B _bbb;
        public B bbb { get => _bbb; set => _bbb = value; }
    }


    public struct TstructA
    {
        public double value;
        TstructB _b;
        public TstructB b { get => _b; set => _b = value; }
    }

    public struct TstructB
    {
        public string kk;
        TstructC _c;
        public TstructC c { get => _c; set => _c = value; }
    }


    public struct TstructC
    {
        private int id;

        public int Id { get => id; set => id = value; }
    }

    public enum TestEnum
    {
        Test001 = 1,
        Test002 = 2,
        Test003 = 4,
        Test004 = 8,
        Test005 = 16,
        Test006 = 32,
        Test007 = 64,
        Test008 = 128,
    }

    public class TClass001
    {
        public object[] objects;
        private double testDuble;
        public double TestDuble
        {
            get { return testDuble; }
            set { testDuble = value; }
        }
        public TClass002 tClass002;
        private TClass002[] _tClass002s;
        public TClass002[] tClass002s { get => _tClass002s; set => _tClass002s = value; }
    }
    public class TClass002
    {
        public double size;
        public string testString;
        private TClass003 _tClass003;
        public TClass003 tClass003 { get => _tClass003; set => _tClass003 = value; }
        //public TClass003 tClass003;
        public TClass003[] tClass003s;

    }
    public class TClass003
    {
        public string testString;
    }

    public class TclassDCC
    {
        public TclassDCC(string str, IList<int> array, double num)
        {
            this.str = str;
            this.array = array;
            this.num = num;
        }
        public string str;
        public IList<int> array;
        public double num;
    }

    public struct TclassDCC3
    {
        public TclassDCC3(string str, IList<int> array, double num)
        {
            this.str = str;
            this.array = array;
            this.num = num;
        }
        public string str;
        public IList<int> array;
        public double num;
    }


    [CollectionWriteAttribute(typeof(TclassDCC3))]
    public unsafe class CreateWriterTclassDCC3 : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType(object obj) { return JsonWriteType.Object; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            var collection = (TclassDCC3)obj;

            if (collection.str != null)
            {
                yield return new KeyValueStruct()
                {
                    key = "#create",
                    value = new object[] { collection.str, collection.array, collection.num },
                    type = typeof(object[]),
                };
            }
        }
    }

    [CollectionWriteAttribute(typeof(TclassDCC))]
    public unsafe class CreateWriterTclassDCC : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType(object obj) { return JsonWriteType.Object; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            var collection = (TclassDCC)obj;

            if (collection.str != null)
            {
                yield return new KeyValueStruct()
                {
                    key = "#create",
                    value = new object[] { collection.str, collection.array, collection.num },
                    type = typeof(object[]),
                };
            }
        }
    }

    [ReadCollection(typeof(TestOB), true)]
    public unsafe class CollectionTestOB : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        public CollectionTestOB()
        {
        }

        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = false;


            read.addObjectClassDelegate = (Action<Box<TestOB>, object, ReadCollectionLink.Add_Args>)AddObjectClass;
            read.addObjectStructDelegate = (AddObjecStruct_)AddObjecStruct;


            read.addValueClassDelegate = (Action<Box<TestOB>, ReadCollectionLink.AddValue_Args>)AddValueClass;
            read.addValueStructDelegate = (AddValueStruct_)AddValueStruct;

            read.createStructDelegate = (CeateValue_)CeateValue;
            read.createObject = CreateObject;
            
            read.getItemType = GetItemType;
            return read;
        }
        void AddObjectClass(Box<TestOB> obj, object value, ReadCollectionLink.Add_Args arg)
        {
            string keystring = new string(arg.bridge->keyStringStart, 0, arg.bridge->keyStringLength);
            switch (keystring)
            {
                case "p3_0":
                case "_p3_0":
                    obj.value.p3_0 = (P_box_3)value;
                    break;
                case "p3_2":
                    obj.value.p3_2 = (P_box_3)value;
                    break;
            }
        }

        delegate void AddObjecStruct_(ref TestOB obj, object value, ReadCollectionLink.Add_Args arg);
        void AddObjecStruct(ref TestOB obj, object value, ReadCollectionLink.Add_Args arg)
        {
            string keystring = new string(arg.bridge->keyStringStart, 0, arg.bridge->keyStringLength);
            switch (keystring)
            {
                case "p3_0":
                case "_p3_0":
                    obj.p3_0 = (P_box_3)value;
                    break;
                case "p3_2":
                    obj.p3_2 = (P_box_3)value;
                    break;
            }
        }

        void AddValueClass(Box<TestOB> obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->keyStringStart, arg.value->keyStringLength);
            switch (keystring)
            {
                case "numI":
                    if (arg.value->type == JsonValueType.Long)
                    {
                        obj.value.numI = (int)arg.value->valueLong;
                    }
                    else
                    {
                        obj.value.numI = (int)arg.value->valueDouble;
                    }
                    break;
                case "num":
                    if (arg.value->type == JsonValueType.Long)
                    {
                        obj.value.num = arg.value->valueLong;
                    }
                    else
                    {
                        obj.value.num = arg.value->valueDouble;
                    }
                    break;
            }
        }

        delegate void AddValueStruct_(ref TestOB obj, ReadCollectionLink.AddValue_Args arg);
        void AddValueStruct(ref TestOB obj, ReadCollectionLink.AddValue_Args arg)
        {
            string keystring = new string(arg.str, arg.value->keyStringStart, arg.value->keyStringLength);
            switch (keystring)
            {
                case "numI":
                    if (arg.value->type == JsonValueType.Long)
                    {
                        obj.numI = (int)arg.value->valueLong;
                    }
                    else
                    {
                        obj.numI = (int)arg.value->valueDouble;
                    }
                    break;
                case "num":
                    if (arg.value->type == JsonValueType.Long)
                    {
                        obj.num = arg.value->valueLong;
                    }
                    else
                    {
                        obj.num = arg.value->valueDouble;
                    }
                    break;
            }
        }

        delegate void CeateValue_(ref TestOB obj, out object temp, ReadCollectionLink.Create_Args arg);
        void CeateValue(ref TestOB obj,out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            obj =  new TestOB();
        }

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            return new TestOB();
        }


        CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            if (collection == null)
            {
                collection = CollectionManager.GetTypeCollection(typeof(P_box_3));
            }
            string keystring = new string(arg.bridge->keyStringStart, 0, arg.bridge->keyStringLength);
            switch (keystring)
            {
                case "p3_0":
                case "p3_2":
                case "_p3_0":
                    return collection;
            }
            return null;
        }

    }


    //[CollectionRead(typeof(TestOB), false)]
    //public unsafe class CollectionTestOB : CollectionObjectStructBase<TestOB>
    //{
    //    public override unsafe Type GetItemType(JsonObject* bridge)
    //    {
    //        string keystring = new string(bridge->keyStringStart, 0, bridge->keyStringLength);
    //        switch (keystring)
    //        {
    //            case "p3_0":
    //                return typeof(P_box_3);
    //            case "p3_2":
    //                return typeof(P_box_3);
    //            case "numI":
    //                return typeof(int);
    //            case "num":
    //                return typeof(double);
    //        }
    //        return typeof(float);
    //    }

    //    protected override unsafe void Add(Box<TestOB> obj, char* key, int keyLength, object value, ReadCollectionProxy proxy)
    //    {
    //        string keystring = new string(key, 0, keyLength);
    //        switch (keystring)
    //        {
    //            case "p3_0":
    //                obj.value.p3_0 = (P_box_3)value;
    //                break;
    //            case "p3_2":
    //                obj.value.p3_2 = (P_box_3)value;
    //                break;
    //        }
    //    }

    //    protected override unsafe void AddValue(Box<TestOB> obj, char* key, int keyLength, char* str, JsonValue* value, ReadCollectionProxy proxy)
    //    {
    //        string keystring = new string(key, 0, keyLength);
    //        switch (keystring)
    //        {
    //            case "numI":
    //                if (value->type == JsonValueType.Long)
    //                {
    //                    obj.value.numI = (int)value->valueLong;
    //                }
    //                else
    //                {
    //                    obj.value.numI = (int)value->valueDouble;
    //                }
    //                break;
    //            case "num":
    //                if (value->type == JsonValueType.Long)
    //                {
    //                    obj.value.num = value->valueLong;
    //                }
    //                else
    //                {
    //                    obj.value.num = value->valueDouble;
    //                }
    //                break;
    //        }
    //    }
    //}


    public struct TestOB2
    {
        public P_box_3 p3_0;
        public P_box_3 p3_2;
        public int numI;
        public double num;
    }
    public struct TestOB
    {
        private P_box_3 _p3_0;
        public P_box_3 p3_2;
        public int numI;
        public double num;

        public P_box_3 p3_0 { get => _p3_0; set => _p3_0 = value; }
    }



    public struct P_box_3
    {
        public float x;
        public float y;
        public float z;
    }
    public struct P_3
    {
        public float x;
        public float y;
        public float z;
    }
    public struct C
    {
        public double k;
        public B bbb;
    }
    public struct B
    {
        public bool b;
        public double num;
        public string str;
    }

    public class TClassA
    {
        public static void Fool(int a, string b)
        {

        }
        public static void Foo2(int a, string b)
        {

        }

    }
    public struct Vector3
    {
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public float x;
        public float y;
        public float z;
        public override bool Equals(object obj)
        {
            if (obj is Vector3)
            {
                Vector3 v = (Vector3)obj;
                return v.x == x && v.y == y && v.z == z;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return (x + y + z).GetHashCode();
        }
    }
    [StructLayout(LayoutKind.Explicit)]
    struct D
    {
        [FieldOffset(0)]
        public bool b;

        [FieldOffset(1)]
        public double k;

        [FieldOffset(16)]
        public string str;

        [FieldOffset(40)]
        public E e;
    }

    public class E
    {
        public bool b;
        public double k;
        public string str;
    }

    [ReadCollection(typeof(DogJsonTest.Vector3), true)]
    public unsafe class CollectionArrayV3 : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        public ReadCollectionLink Create()
        {
            //collection = CollectionManager.GetTypeCollection(typeof(float));
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = false;

            read.addValueStructDelegate = (AddValue_)AddValue;
            read.addValueClassDelegate = (AddValueObject_)AddValueObject;

            read.createObject = CreateObject;
            read.createStructDelegate = (CreateValue_)CreateValue;
            //read.getItemType = GetItemType;
            return read;
        }


        delegate void AddValue_(ref Vector3 v3, ReadCollectionLink.AddValue_Args arg);
        void AddValue(ref Vector3 v3, ReadCollectionLink.AddValue_Args arg)
        {
            switch (arg.value->type)
            {
                case JsonValueType.Long:
                    switch (arg.value->arrayIndex)
                    {
                        case 0:
                            v3.x = (float)arg.value->valueLong;
                            break;
                        case 1:
                            v3.y = (float)arg.value->valueLong;
                            break;
                        case 2:
                            v3.z = (float)arg.value->valueLong;
                            break;
                    }
                    break;
                case JsonValueType.Double:
                    switch (arg.value->arrayIndex)
                    {
                        case 0:
                            v3.x = (float)arg.value->valueDouble;
                            break;
                        case 1:
                            v3.y = (float)arg.value->valueDouble;
                            break;
                        case 2:
                            v3.z = (float)arg.value->valueDouble;
                            break;
                    }
                    break;
            }
        }

        delegate void AddValueObject_(Box<Vector3> v3, ReadCollectionLink.AddValue_Args arg);
        void AddValueObject(Box<Vector3> v3, ReadCollectionLink.AddValue_Args arg)
        {
            switch (arg.value->type)
            {
                case JsonValueType.Long:
                    switch (arg.value->arrayIndex)
                    {
                        case 0:
                            v3.value.x = (float)arg.value->valueLong;
                            break;
                        case 1:
                            v3.value.y = (float)arg.value->valueLong;
                            break;
                        case 2:
                            v3.value.z = (float)arg.value->valueLong;
                            break;
                    }
                    break;
                case JsonValueType.Double:
                    switch (arg.value->arrayIndex)
                    {
                        case 0:
                            v3.value.x = (float)arg.value->valueDouble;
                            break;
                        case 1:
                            v3.value.y = (float)arg.value->valueDouble;
                            break;
                        case 2:
                            v3.value.z = (float)arg.value->valueDouble;
                            break;
                    }
                    break;
            }
        }

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            return new Vector3();
        }

        delegate void CreateValue_(ref Vector3 v3, out object temp, ReadCollectionLink.Create_Args arg);

        void CreateValue(ref Vector3 v3, out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            v3 = new Vector3();
        }
    }



    [CollectionWrite(typeof(DogJsonTest.Vector3), true)]
    public unsafe class WriteCollectionV3 : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType(object obj) { return JsonWriteType.Array; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            Vector3 collection = (Vector3)obj;

            yield return new KeyValueStruct()
            {
                value = collection.x,
                type = typeof(float),
            };
            yield return new KeyValueStruct()
            {
                value = collection.y,
                type = typeof(float),
            };
            yield return new KeyValueStruct()
            {
                value = collection.z,
                type = typeof(float),
            };
        }
    }


}


