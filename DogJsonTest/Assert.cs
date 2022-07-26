using DogJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DogJsonTest
{
    public class Assert
    {
        const double AreEqual_delta = 0.0001;
        public static void AreEqual(double expected, double actual, double delta = AreEqual_delta) 
        {
            double d = Math.Abs(expected - actual);
            if (Math.Abs(expected * delta) < d)
            {
                throw new Exception(d + ", " + expected * delta);
                throw new Exception(expected + ", " + actual);
            }
        }

        public static void AreEqual(float expected, float actual, float delta = (float)AreEqual_delta)
        {
            double d = Math.Abs(expected - actual);
            if (Math.Abs(expected * delta) < d)
            {
                throw new Exception("");
            }
        }

        public static void AreEqual(decimal expected, decimal actual, decimal delta = (decimal)AreEqual_delta)
        {
            decimal d = Math.Abs(expected - actual);
            if (Math.Abs(expected * delta) < d)
            {
                throw new Exception("");
            }
        }

        public static void Equal(object objA, object objB)
        {
            if (objA != null && objA != null)
            {
                if (objA == objB)
                {
                    return;
                }
                if (objA.Equals(objB))
                {
                    return;
                }
            }
            throw new Exception("不相等");
        }




        public static void Equal(int expected, int actual)
        {
            if (expected != actual)
            {
                throw new Exception("不相等");
            }
        }
        public static void Equal(uint expected, int actual)
        {
            if (expected != actual)
            {
                throw new Exception("不相等");
            }
        }

        public static void Equal(ulong expected, ulong actual)
        {
            if (expected != actual)
            {
                throw new Exception("不相等");
            }
        }
        public static void Equal(long expected, long actual)
        {
            if (expected != actual)
            {
                throw new Exception("不相等");
            }
        }
        public static void Equal(short expected, short actual)
        {
            if (expected != actual)
            {
                throw new Exception("不相等");
            }
        }
        public static void Equal(ushort expected, ushort actual)
        {
            if (expected != actual)
            {
                throw new Exception("不相等");
            }
        }



        static Dictionary<Type, Func<object, object, bool>> chakeAllObject = new Dictionary<Type, Func<object, object, bool>>()
        {
            { typeof(Dictionary<,>), (a, b)=>{return EqualIDictionary((IDictionary)a,(IDictionary)b); } },
            { typeof(Stack<>), (a, b)=>{return EqualIEnumerable((IEnumerable)a,(IEnumerable)b); } },
            { typeof(Stack), (a, b)=>{return EqualIEnumerable((IEnumerable)a,(IEnumerable)b); } },
            { typeof(List<>), (a, b)=>{return EqualIEnumerable((IEnumerable)a,(IEnumerable)b); } },
            { typeof(Queue<>), (a, b)=>{return EqualIEnumerable((IEnumerable)a,(IEnumerable)b); } },
            { typeof(LinkedList<>), (a, b)=>{return EqualIEnumerable((IEnumerable)a,(IEnumerable)b); } },
            { typeof(HashSet<>), (a, b)=>{return EqualIEnumerable((IEnumerable)a,(IEnumerable)b); } },
        };


        abstract class AssertGeneric
        {
            public abstract bool EqualHashSet(object expected, object actual);
        }
        class AssertGeneric<T> : AssertGeneric
        {
            public override bool EqualHashSet(object expected, object actual)
            {
                return EqualHashSet((HashSet<T>)expected, (HashSet<T>)actual);
            }
            public static bool EqualHashSet(HashSet<T> expected, HashSet<T> actual)
            {
                EqualNull(expected, actual);
                foreach (var item in expected)
                {
                    if (!actual.Contains(item))
                    {
                        throw new Exception("不相等");
                    }
                }
                return true;
            }
        }

        public static bool EqualIDictionary(IDictionary expected, IDictionary actual)
        {
            HashSet<Type> set = new HashSet<Type>();
            //if (!IsEqualNull(expected, actual))
            //{
            //    return false;
            //}
            EqualNull(expected, actual);
            foreach (var key in actual.Keys)
            {
                if (!expected.Contains(key))
                {
                    throw new Exception("不相等");
                }
                else
                {
                    _AreEqualObject(expected[key], actual[key]);
                }
            }
            return true;
        }

        public static bool EqualIEnumerable(IEnumerable expected, IEnumerable actual)
        {
            EqualNull(expected, actual);
            IEnumerator a = expected.GetEnumerator();
            IEnumerator b = actual.GetEnumerator();
            while (true)
            {
                bool aisNext = a.MoveNext();
                if (aisNext != b.MoveNext())
                {
                    throw new Exception("长度不相等");
                }
                if (!aisNext) { break; }

                _AreEqualObject(a.Current, b.Current);
            }
            return true;
        }




        public static bool EqualNull(object expected, object actual)
        {
            if (expected == null)
            {
                if (actual == null)
                {
                    return true;
                }
                throw new Exception("expected == null " + actual);
            }
            else if (actual == null)
            {
                throw new Exception("actual == null " + expected);
            }
            return false;
        }

        public static bool IsEqualNull(object expected, object actual)
        {
            if (expected == null)
            {
                if (actual == null)
                {
                    return true;
                }
                return false;
            }
            else if (actual == null)
            {
                return false;
            }
            return true;
        }


        public static void EqualArrayOne(Array array1, Array array2)
        {
            int length = array1.Length;
            if (length != array2.Length)
            {
                throw new Exception("Unequal length");
            }
            for (int i = 0; i < length; i++)
            {
                object value1 = array1.GetValue(i);
                object value2 = array2.GetValue(i);
                if (value1 != value2)
                {
                    if (!_AreEqualObject(value1, value2))
                    {
                        throw new Exception("Unequal Value");
                    }
                }
            }

        }


        public static void EqualMulticastDelegate(MulticastDelegate expected, MulticastDelegate actual)
        {
            EqualNull(expected, actual);
            var list1 = expected.GetInvocationList();
            var list2 = actual.GetInvocationList();
            if (list1.Length != list2.Length)
            {
                throw new Exception("Unequal length");
            }
            for (int i = 0; i < list1.Length; i++)
            {
                if (!list1[i].Method.Equals(list2[i].Method))
                {
                    throw new Exception("Unequal Value");
                }
                //if (!list1[i].Equals(list2[i]))
                //{
                //    throw new Exception("Unequal Value");
                //}
            }
        }


        public static void EqualArray(object expected, object actual)
        {
            EqualNull(expected, actual);

            Array array1 = expected as Array;
            Array array2 = actual as Array;

            if (array1.Rank != array2.Rank)
            {
                throw new Exception("Unequal Rank");
            }
            int rank = array1.Rank;
            if (rank == 1)
            {
                EqualArrayOne(array1, array2);
                return;
            }

            int[] lengths = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                lengths[i] = array1.GetLength(i);
                if (array2.GetLength(i) != lengths[i])
                {
                    throw new Exception("Unequal lengths");
                }
            }
            int nowRank = 0;
            int[] indices = new int[rank];

            do
            {
                for (int i = 0; i < lengths[0]; i++)
                {
                    indices[0] = i;

                    object value1 = array1.GetValue(indices);
                    object value2 = array1.GetValue(indices);
                    if (value1 != value2)
                    {
                        if (!_AreEqualObject(value1, value2))
                        {
                            throw new Exception("Unequal Value");
                        }
                    }


                }
                nowRank = 1;
            Chake:
                ++indices[nowRank];
                if (indices[nowRank] == lengths[nowRank])
                {
                    ++nowRank;
                    if (nowRank < rank)
                    {
                        goto Chake;
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 0; i < nowRank; i++)
                {
                    indices[i] = 0;
                }
            } while (true);
        }


        static Dictionary<Tuple<object, object>, Tuple<object, object>> allChake = new Dictionary<Tuple<object, object>, Tuple<object, object>>();
        static FieldInfo fieldInfo;
        public unsafe static bool AreEqualObject(object expected, object actual)
        {

            return _AreEqualObject(expected, actual);
        }

        public unsafe static bool _AreEqualObject(object expected, object actual)
        {
            if (EqualNull(expected, actual))
            {
                return true;
            }

            Tuple<object, object> tuple = new Tuple<object, object>(expected, actual);
            Tuple<object, object> tuple2;
            if (allChake.TryGetValue(tuple, out tuple2))
            {
                if (
                    GeneralTool.ObjectToVoid(tuple2.Item1)
                    ==
                    GeneralTool.ObjectToVoid(tuple.Item1)
                    &&
                    GeneralTool.ObjectToVoid(tuple2.Item2)
                    ==
                    GeneralTool.ObjectToVoid(tuple.Item2)
                    )
                {
                    return true;
                }
            }
            allChake[tuple] = tuple;


            Type type = expected.GetType();
            if (type == actual.GetType())
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Object:
                        if (expected == actual)
                        {
                            return true;
                        }
                        if (expected.Equals(actual))
                        {
                            return true;
                        }

                        Func<object, object, bool> func;
                        if (!chakeAllObject.TryGetValue(type, out func))
                        {
                            if (!(type.IsGenericType && chakeAllObject.TryGetValue(type.GetGenericTypeDefinition(), out func)))
                            {
                                //if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>))
                                //{
                                //    AssertGeneric ob = System.Activator.CreateInstance(typeof(AssertGeneric<>).MakeGenericType(type.GetGenericArguments()[0])) as AssertGeneric;

                                //    if (ob.EqualHashSet(expected, actual))
                                //    {
                                //        return;
                                //    }
                                //}
                                if (type.BaseType == typeof(MulticastDelegate))
                                {
                                    EqualMulticastDelegate((MulticastDelegate)expected, (MulticastDelegate)actual);
                                    return true;
                                }
                                if (type.IsArray)
                                {
                                    EqualArray(expected, actual);
                                    return true;
                                }
                            }
                        }

                        if (func != null)
                        {
                            if (func(expected, actual))
                            {
                                return true;
                            }
                            throw new Exception("不相等");
                            return false;
                        }

                        foreach (var item in TypeAddrReflectionWrapper.GetAllFieldInfo(type))
                        {
                            object obj1 = item.Value.GetValue( actual);
                            if (obj1 != null)
                            {
                                fieldInfo = item.Value;
                                _AreEqualObject(obj1, item.Value.GetValue(expected));
                            }
                        }

                        foreach (var item in TypeAddrReflectionWrapper.GetAllPropertyInfo(type))
                        {
                            if (item.Value.GetMethod != null)
                            {
                                object obj1 = item.Value.GetValue(actual);
                                if (obj1 != null)
                                {
                                    _AreEqualObject(obj1, item.Value.GetValue(expected));
                                }
                            }
                        }
                        return true;

                        break;
                    case TypeCode.DBNull:
                        break;
                    case TypeCode.Single:
                        AreEqual((float)expected, (float)actual);
                        return true;
                    case TypeCode.Double:
                        AreEqual((double)expected, (double)actual);
                        return true;
                    case TypeCode.Decimal:
                        AreEqual((decimal)expected, (decimal)actual);
                        return true;
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.DateTime:
                    case TypeCode.String:
                        if (expected == actual)
                        {
                            return true;
                        }
                        if (expected.Equals(actual))
                        {
                            return true;
                        }
                        break;
                    default:
                        break;
                }
            }

            void* p_expected = GeneralTool.ObjectToVoid(expected);
            void* p_actual = GeneralTool.ObjectToVoid(actual);

            string vvv = "不相等" + (*(byte*)p_expected) + " , " + (*(byte*)p_actual);
            throw new Exception("不相等" + expected + " , " + actual);
            return false;
        }




    }
}
