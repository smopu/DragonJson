//using DogJson;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace DogJsonTest
//{

//    public unsafe class TestJsonClassA
//    {
//        public Action<int, string> testDelegate2;
//        ///*
//        private double num;
//        public double Num
//        {
//            get { return num; }
//            set { num = value; }
//        }

//        private B bb;
//        public B BB
//        {
//            get { return bb; }
//            set { bb = value; }
//        }


//        public LinkedList<long> arrayLinkedList;
//        public int[,,] arrayArray1;
//        public int[,][] arrayArray2;
//        public int[] arrayArray3;
//        public int[][] arrayArray4;
//        public int[,,] arrayArray5;

//        public List<B> listB;
//        public List<C> listC;
//        public List<E> listE;

//        public bool b;
//        public int kk;
//        public string str;
//        public C gcc;
//        public E gD;
//        public V3 v3;

//        public TestOB testOB;

//        public TclassDCC dcc;

//        public object ddd;
//        public object[] objects;
//        public TClass001 tClass001;
//        public TClass003 testClassDD;
//        public TClass003 testClassDD2;
//        public TClass003 testClassDD3;
//        public TClass003 testClassDD4;
//        public TestEnum testEnum;
//        public TestEnum[] testEnums;

//        public IList<int> arrayint2;
//        public TclassA Iclass0Z;
//        public P_box_3 p3;
//        public B[,,] arrayRekn;
//        public Stack<int> arrayStack;
//        public B[] fd;
//        public HashSet<double> arraydouble;
//        public Queue<string> arraystring;
//        public List<bool> arraybool;
//        public Dictionary<int, string> dictionary1;
//        public Dictionary<int, B> dictionary2;
//        public Dictionary<V3, B> dictionary3;
//        //*/
//        public TstructA tstructA;
//    }

//    public class TclassA
//    {
//        public int b;
//    }

//    public class TclassC : TclassA
//    {
//        public double value;
//        B _bbb;
//        public B bbb { get => _bbb; set => _bbb = value; }
//    }


//    public struct TstructA
//    {
//        public double value;
//        TstructB _b;
//        public TstructB b { get => _b; set => _b = value; }
//    }

//    public struct TstructB
//    {
//        public string kk;
//        TstructC _c;
//        public TstructC c { get => _c; set => _c = value; }
//    }


//    public struct TstructC
//    {
//        private int id;

//        public int Id { get => id; set => id = value; }
//    }


//    public enum TestEnum
//    {
//        Test001,
//        Test002,
//        Test003,
//        Test004,
//    }
//    public class TClass001
//    {
//        public object[] objects;
//        private double testDuble;
//        public double TestDuble
//        {
//            get { return testDuble; }
//            set { testDuble = value; }
//        }
//        public TClass002 tClass002;
//        private TClass002[] _tClass002s;
//        public TClass002[] tClass002s { get => _tClass002s; set => _tClass002s = value; }
//    }
//    public class TClass002
//    {
//        public double size;
//        public string testString;
//        private TClass003 _tClass003;
//        public TClass003[] tClass003s;

//        public TClass003 tClass003 { get => _tClass003; set => _tClass003 = value; }
//    }
//    public class TClass003
//    {
//        public string testString;
//    }
//    public class TclassDCC
//    {
//        public TclassDCC(string str, IList<int> array, double num)
//        {
//            this.str = str;
//            this.array = array;
//            this.num = num;
//        }
//        public string str;
//        public IList<int> array;
//        public double num;
//    }

//    public struct TestOB2
//    {
//        public P_box_3 p3_0;
//        public P_box_3 p3_2;
//        public int numI;
//        public double num;
//    }
//    public struct TestOB
//    {
//        private P_box_3 _p3_0;
//        public P_box_3 p3_2;
//        public int numI;
//        public double num;

//        public P_box_3 p3_0 { get => _p3_0; set => _p3_0 = value; }
//    }



//    public struct P_box_3
//    {
//        public float x;
//        public float y;
//        public float z;
//    }
//    public struct P_3
//    {
//        public float x;
//        public float y;
//        public float z;
//    }
//    public struct C
//    {
//        public double k;
//        public B bbb;
//    }
//    public struct B
//    {
//        public bool b;
//        public double num;
//        public string str;
//    }

//    public class TClassA
//    {
//        public static void Fool(int a, string b)
//        {

//        }
//        public static void Foo2(int a, string b)
//        {

//        }

//    }
//    public struct V3
//    {
//        public V3(float x, float y, float z)
//        {
//            this.x = x;
//            this.y = y;
//            this.z = z;
//        }
//        public float x;
//        public float y;
//        public float z;
//        public override bool Equals(object obj)
//        {
//            if (obj is V3)
//            {
//                V3 v = (V3)obj;
//                return v.x == x && v.y == y && v.z == z;
//            }
//            return false;
//        }
//        public override int GetHashCode()
//        {
//            return (x + y + z).GetHashCode();
//        }
//    }
//    [StructLayout(LayoutKind.Explicit)]
//    struct D
//    {
//        [FieldOffset(0)]
//        public bool b;

//        [FieldOffset(1)]
//        public double k;

//        [FieldOffset(16)]
//        public string str;

//        [FieldOffset(40)]
//        public E e;
//    }

//    public class E
//    {
//        public bool b;
//        public double k;
//        public string str;
//    }

//    [Collection(typeof(V3), true)]
//    public unsafe class CollectionArrayV3 : CollectionArrayBase<V3, CollectionArrayV3.V3_>
//    {
//        public class V3_
//        {
//            public V3 v3;
//        }
//        protected override void AddValue(V3_ obj, int index, char* str, JsonValue* value)
//        {
//            switch (value->type)
//            {
//                case JsonValueType.Long:
//                    switch (index)
//                    {
//                        case 0:
//                            obj.v3.x = (float)value->valueLong;
//                            break;
//                        case 1:
//                            obj.v3.y = (float)value->valueLong;
//                            break;
//                        case 2:
//                            obj.v3.z = (float)value->valueLong;
//                            break;
//                    }
//                    break;
//                case JsonValueType.Double:
//                    switch (index)
//                    {
//                        case 0:
//                            obj.v3.x = (float)value->valueDouble;
//                            break;
//                        case 1:
//                            obj.v3.y = (float)value->valueDouble;
//                            break;
//                        case 2:
//                            obj.v3.z = (float)value->valueDouble;
//                            break;
//                    }
//                    break;
//            }
//        }
//        protected override V3_ CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
//        {
//            return new V3_();
//        }
//        protected override V3 End(V3_ obj)
//        {
//            return obj.v3;
//        }
//        public override Type GetItemType(int index)
//        {
//            return typeof(float);
//        }

//        protected override void Add(V3_ obj, int index, object value)
//        {
//            throw new NotImplementedException();
//        }
//    }
//    //   */

//    [Collection(typeof(TestOB), false)]
//    public unsafe class CollectionTestOB : CollectionObjectStructBase<TestOB>
//    {
//        public override unsafe Type GetItemType(JsonObject* bridge)
//        {
//            string keystring = new string(bridge->keyStringStart, 0, bridge->keyStringLength);
//            switch (keystring)
//            {
//                case "p3_0":
//                    return typeof(P_box_3);
//                case "p3_2":
//                    return typeof(P_box_3);
//                case "numI":
//                    return typeof(int);
//                case "num":
//                    return typeof(double);
//            }
//            return typeof(float);
//        }

//        protected override unsafe void Add(Box<TestOB> obj, char* key, int keyLength, object value)
//        {
//            string keystring = new string(key, 0, keyLength);
//            switch (keystring)
//            {
//                case "p3_0":
//                    obj.value.p3_0 = (P_box_3)value;
//                    break;
//                case "p3_2":
//                    obj.value.p3_2 = (P_box_3)value;
//                    break;
//            }
//        }

//        protected override unsafe void AddValue(Box<TestOB> obj, char* key, int keyLength, char* str, JsonValue* value)
//        {
//            string keystring = new string(key, 0, keyLength);
//            switch (keystring)
//            {
//                case "numI":
//                    if (value->type == JsonValueType.Long)
//                    {
//                        obj.value.numI = (int)value->valueLong;
//                    }
//                    else
//                    {
//                        obj.value.numI = (int)value->valueDouble;
//                    }
//                    break;
//                case "num":
//                    if (value->type == JsonValueType.Long)
//                    {
//                        obj.value.num = value->valueLong;
//                    }
//                    else
//                    {
//                        obj.value.num = value->valueDouble;
//                    }
//                    break;
//            }
//        }
//    }


//}
