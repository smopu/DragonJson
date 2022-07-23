using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DogJson;
using NUnit.Framework;

namespace DogJsonTest.Read.Addr
{
    [TestFixture]
    internal class StringTableTest
    {
        [Test]
        public unsafe void RunTest001()
        {
            string[] strss = new string[] { 
                "testDelegate",
                "testDelegate2",
                "testDelegate3",
                "arrayLinkedList",
                "arrayArray1",
                "arrayArray2",
                "arrayArray3",
                "arrayArray4",
                "arrayArray5",
                "listB",
                "listE",
                "b",
                "kk",
                "str",
                "gcc",
                "gD",
                "gDs",
                "v3",
                "testOB",
                "dcc",
                "dcc3",
                "dcc2",
                "ddd",
                "objects",
                "tClass001",
                "testClassDD",
                "testClassDD2",
                "testClassDD3",
                "testClassDD4",
                "testEnum",
                "testEnums",
                "arrayint2",
                "Iclass0Z",
                "p3",
                "arrayRekn",
                "arrayStack",
                "fd",
                "arraydouble",
                "arraystring",
                "arraybool",
                "dictionary3",
                "tstructA",
                "Num",
                "BB",
                "testEnum2"
            };

            Dictionary<string, int> strs = new Dictionary<string, int>();
            for (int i = 0; i < strss.Length; i++)
            {
                strs.Add(strss[i], i);
            }

            var varStringTable = new StringTable(strs);

            for (int i = 0; i < strss.Length; i++)
            {
                string str = strss[i] + "\"";
                fixed (char* c = str)
                {
                    int id = varStringTable.Find(c, str.Length - 1);
                    Assert.Equal(id, i);
                }

            }

        }

        [Test]
        public unsafe void RunTest002()
        {
            string[] strss = new string[] 
            {
                "a23",// 1 2
                "b23",// 2 3
                "c23",// 3 4
                //21 18
                "d24",// 4 2
                "e24",// 5 3
                "f24",// 6 4
            };

            Dictionary<string, int> strs = new Dictionary<string, int>();
            for (int i = 0; i < strss.Length; i++)
            {
                strs.Add(strss[i], i);
            }

            var varStringTable = new StringTable(strs);

            for (int i = 0; i < strss.Length; i++)
            {
                string str = strss[i] + "\"";
                fixed (char* c = str)
                {
                    int id = varStringTable.Find(c, str.Length - 1);
                    Assert.Equal(id, i);
                }

            }

        }


    }


}
