using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using DogJson;
//using Assert = DogJson.Assert;
namespace DogJsonTest
{
    [TestFixture]
    public class JsonReadTest
    {
        [Test]
        public void ReadString()
        {
        }

        class TestReadNumber
        {
            public double varDouble;
            public float varFloat;
            public float varFloat2;
            public double varDouble2;
            public int varInt;
            public int varIn2t;
            public long varLong;
            public long varLong2;
        }
        
        [Test]
        public void ReadNumber()
        {
            //double
            //float
            //int
            //long
            JsonManager.Start(new ReflectionToObject());
            JsonRender jsonRender = new JsonRender();

            string testPath = Path.GetDirectoryName(typeof(JsonRender).Assembly.Location) + @"\JsonFile\" + nameof(ReadNumber) + ".json";
            string data = File.ReadAllText(testPath);


            TestReadNumber o = jsonRender.ReadJsonTextCreateObject<TestReadNumber>( data);

            TestReadNumber test = new TestReadNumber() {

                varDouble = 1.2,
                varFloat = 1.23f,
                varFloat2 = 1.2E-18f,
                varDouble2 = 4.9E+18f,
                varInt = 566,
                varIn2t = -5435,
                varLong = 18,
                varLong2 = -18

            };
            Assert.AreEqual(test.varDouble, o.varDouble, 0.001);
            Assert.AreEqual(test.varFloat, o.varFloat, 0.001);
            Assert.AreEqual(test.varFloat2, o.varFloat2, 0.001);
            Assert.AreEqual(test.varDouble2, o.varDouble2, 0.001);
            NUnit.Framework.Assert.AreEqual(test.varInt, o.varInt);
            NUnit.Framework.Assert.AreEqual(test.varIn2t, o.varIn2t);
            NUnit.Framework.Assert.AreEqual(test.varLong, o.varLong);
            NUnit.Framework.Assert.AreEqual(test.varLong2, o.varLong2);
        }

        [Test]
        public void ReadClass()
        {

        }
        [Test]
        public void ReadStruct()
        {

        }

        [Test]
        public void ReadArray()
        {

        }
        
        /// <summary>
        /// 多维数组
        /// </summary>
        [Test]
        public void ReadMultidimensionalArray()
        {

        }

        /// <summary>
        /// 协变参数
        /// </summary>
        [Test]
        public void ReadCovariantParameter()
        {

        }
        /// <summary>
        /// 构造参数
        /// </summary>
        [Test]
        public void ReadConstruction()
        {

        }

        /// <summary>
        /// 循环引用
        /// </summary>
        [Test]
        public void ReadReference()
        {

        }

        /// <summary>
        /// 指针
        /// </summary>
        [Test]
        public void ReadPointer()
        {

        }

        /// <summary>
        /// 别名
        /// </summary>
        [Test]
        public void ReadAlias()
        {

        }

    }
}
