using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using DragonJson;
using System.Runtime.InteropServices;
//using Assert = DogJson.Assert;
namespace DragonJsonTest
{
    [TestFixture]
    public class JsonWriteTest
    {
        public class ACE3
        {
            public int kk;
            public string str;
        }

        public class ACE1
        {
            public ACE3[] aCE3s;
            public bool b;
            public double num;
            public string str;
            public ACE3 aCE3;
            public int[] ints;
            public int[,] ints2;
        }

        [Test]
        public void TestWrite()
        {
            ACE1 inputData = new ACE1();
            inputData.aCE3 = new ACE3
            {
                kk = 44,
                str = "E4",
            };
            inputData.ints = new int[] {
                1, 2, 3, 9
            };

            inputData.aCE3s = new ACE3[] {
                new ACE3{
                    kk = 4,
                    str = "eewqr"
                },
                new ACE3{
                    kk = 2,
                    str = "DE4"
                },
                new ACE3{
                    kk = 1,
                    str = "F3"
                }
            };

            inputData.ints2 = new int[,]
            {
                {1, 2, 3, 4},
                {11, 12, 13, 14},
                {101, 102, 103, 104},
            };


            inputData.b = false;
            inputData.num = 4.5;
            inputData.str = "CCCCD";

            JsonWriter jsonWriter = new JsonWriter(new WriterReflection());
            string dataStr = jsonWriter.Writer(inputData);

            Console.WriteLine(dataStr);

            JsonReader jsonRender = new JsonReader();
            var outData = jsonRender.ReadJsonTextCreate<ACE1>(dataStr);

            Assert.AreEqualObject(outData, inputData);
            //Console.ReadKey();
        }


    }
}
