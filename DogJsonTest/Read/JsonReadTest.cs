using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using DogJson;
using System.Runtime.InteropServices;
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

            public double VarDouble { get => varDouble; set => varDouble = value; }
            public float VarFloat { get => varFloat; set => varFloat = value; }
            public float VarFloat2 { get => varFloat2; set => varFloat2 = value; }
            public double VarDouble2 { get => varDouble2; set => varDouble2 = value; }
            public int VarInt { get => varInt; set => varInt = value; }
            public int VarIn2t { get => varIn2t; set => varIn2t = value; }
            public long VarLong { get => varLong; set => varLong = value; }
            public long VarLong2 { get => varLong2; set => varLong2 = value; }
        }

        [Test]
        public void ReadNumber()
        {
            CollectionManager.Start();//ReflectionToObject
            JsonReader jsonRender = new JsonReader();

            string testPath = Path.GetDirectoryName(typeof(JsonReader).Assembly.Location) + @"\JsonFile\" + nameof(ReadNumber) + ".json";
            string data = File.ReadAllText(testPath);

            TestReadNumber o = jsonRender.ReadJsonTextCreate<TestReadNumber>(data);

            TestReadNumber test = new TestReadNumber()
            {
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
            Assert.Equal(test.varInt, o.varInt);
            Assert.Equal(test.varIn2t, o.varIn2t);
            Assert.Equal(test.varLong, o.varLong);
            Assert.Equal(test.varLong2, o.varLong2);
        }


        [Test]
        public void ReadPropertyNumber()
        {
            CollectionManager.Start();//ReflectionToObject
            JsonReader jsonRender = new JsonReader();

            string testPath = Path.GetDirectoryName(typeof(JsonReader).Assembly.Location) + @"\JsonFile\" + nameof(ReadPropertyNumber) + ".json";
            string data = File.ReadAllText(testPath);

            TestReadNumber o = jsonRender.ReadJsonTextCreate<TestReadNumber>(data);

            TestReadNumber test = new TestReadNumber()
            {
                varDouble = 1.2,
                varFloat = 1.23f,
                varFloat2 = 1.2E-18f,
                varDouble2 = 4.9E+18f,
                varInt = 566,
                varIn2t = -5435,
                varLong = 18,
                varLong2 = -18
            };

            Assert.Equal(test.VarInt, o.VarInt);
            Assert.Equal(test.VarIn2t, o.VarIn2t);
            Assert.Equal(test.VarLong, o.VarLong);
            Assert.Equal(test.VarLong2, o.VarLong2);
            Assert.AreEqual(test.VarDouble, o.VarDouble, 0.001);
            Assert.AreEqual(test.VarFloat, o.VarFloat, 0.001);
            Assert.AreEqual(test.VarFloat2, o.VarFloat2, 0.001);
            Assert.AreEqual(test.VarDouble2, o.VarDouble2, 0.001);
        }

        [Test]
        public void ReadClass()
        {
            TestReadNumber test1 = new TestReadNumber()
            {
                varDouble = 1.2,
                varFloat = 1.23f,
                varFloat2 = 1.2E-18f,
                varDouble2 = 4.9E+18f,
                varInt = 566,
                varIn2t = -5435,
                varLong = 18,
                varLong2 = -18
            };
            CollectionManager.Start();//ReflectionToObject
            JsonReader jsonRender = new JsonReader();

            string testPath = Path.GetDirectoryName(typeof(JsonReader).Assembly.Location) + @"\JsonFile\" + nameof(ReadPropertyNumber) + ".json";
            string data = File.ReadAllText(testPath);

            TestReadNumber o = jsonRender.ReadJsonTextCreate<TestReadNumber>(data);

            Assert.AreEqualObject(test1, o);
        }


        [Test]
        public unsafe void ReadStruct()
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


        /// <summary>
        /// 值类型 里套 引用类型
        /// </summary>
        [Test]
        public void StructClass()
        {

        }

        /// <summary>
        ///  引用类型 里套 值类型
        /// </summary>
        [Test]
        public void ClassStruct()
        {

        }

        /// <summary>
        ///  容器 里套 引用类型
        /// </summary>
        [Test]
        public void CollectionClass()
        {

        }

        /// <summary>
        ///  引用类型 里套 容器
        /// </summary>
        [Test]
        public void ClassCollection()
        {

        }

        /// <summary>
        ///  值类型 里套 容器
        /// </summary>
        [Test]
        public void StructCollection()
        {

        }

        /// <summary>
        ///  容器  里套 值类型
        /// </summary>
        [Test]
        public void CollectionStruct()
        {

        }

        /// <summary>
        ///  容器  值类型  引用类型
        /// </summary>
        [Test]
        public void CollectionStructClass()
        {

        }
        /// <summary>
        /// 容器  引用类型  值类型
        /// </summary>
        [Test]
        public void CollectionClassStruct()
        {

        }

        /// <summary>
        /// 值类型  容器  引用类型
        /// </summary>
        [Test]
        public void StructCollectionClass()
        {

        }

        /// <summary>
        /// 值类型  引用类型 容器
        /// </summary>
        [Test]
        public void StructClassCollection()
        {

        }

        /// <summary>
        /// 引用类型 值类型  容器
        /// </summary>
        [Test]
        public void ClassStructCollection()
        {

        }

        /// <summary>
        /// 引用类型  容器 值类型
        /// </summary>
        [Test]
        public void ClassCollectionStruct()
        {

        }







    }
}
