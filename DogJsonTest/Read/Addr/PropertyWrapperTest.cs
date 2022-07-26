//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using DogJson;
//using NUnit.Framework;

//namespace DogJsonTest.Read.Addr
//{
//    [TestFixture]
//    public class JsonReadArrPropertyWrapperTestayCollectionTest
//    {
//        public struct A2
//        {
//            public int Num
//            {
//                get { return num; }
//                set { num = value; }
//            }
//            private int num;
//            public ItemStruct Struct
//            {
//                get { return vStruct; }
//                set { vStruct = value; }
//            }
//            private ItemStruct vStruct;
//            //public ItemClass Class
//            //{
//            //    get { return vClass; }
//            //    set { vClass = value; }
//            //}
//            //private ItemClass vClass;
//        }


//        public struct ItemStruct
//        {
//            public int Num
//            {
//                get { return num; }
//                set { num = value; }
//            }
//            private int num;
//            public override bool Equals(object obj)
//            {
//                if (obj is ItemStruct)
//                    return ((ItemStruct)obj).num == this.num;
//                return false;
//            }
//            public override int GetHashCode()
//            {
//                return num;
//            }
//        }

//        public class ItemClass
//        {
//            public int Num
//            {
//                get { return num; }
//                set { num = value; }
//            }
//            private int num;
//        }



//        public class A3
//        {
//            public int Num
//            {
//                get { return num; }
//                set { num = value; }
//            }
//            private int num;

//            public ItemStruct Struct
//            {
//                get { return vStruct; }
//                set { vStruct = value; }
//            }
//            private ItemStruct vStruct;
//            public ItemClass Class
//            {
//                get { return vClass; }
//                set { vClass = value; }
//            }
//            private ItemClass vClass;
//        }


//        [Test]
//        public unsafe void Class_Value()
//        {
//            //A3 a1 = new A3();
//            //a1.Num = 999;
//            //object obj = a1;

//            //PropertyInfo propertyInfo = obj.GetType().GetProperty("Num");

//            //Delegate setValue = PropertyWrapper.CreateSetTargetDelegate(propertyInfo);

//            //PropertyDelegateItem propertyDelegateItem = new PropertyDelegateItem();
//            //propertyDelegateItem.Set = setValue;

//            //var a1_p = GeneralTool.ObjectToVoid(obj);
//            //*propertyDelegateItem.setTarget = (IntPtr)a1_p;
//            //propertyDelegateItem.setInt32(777);

//            //Assert.Equal(((A3)obj).Num, 777);
//        }


//        [Test]
//        public unsafe void Class_Class()
//        {
//            A3 a1 = new A3();

//            ItemClass v = new ItemClass();
//            v.Num = 999;

//            //a1.Class = v;
//            object obj = a1;
//            object value = v;


//            PropertyInfo propertyInfo = obj.GetType().GetProperty(nameof(a1.Class));

//            //Delegate setValue = PropertyWrapper.CreateSetTargetDelegate(propertyInfo);

//            PropertyDelegateItem2 propertyDelegateItem = new PropertyDelegateItem2();
//            //propertyDelegateItem.Set = setValue;

//            var a1_p = GeneralTool.ObjectToVoid(obj);
//            //*propertyDelegateItem.setTarget = (IntPtr)a1_p;
//            //propertyDelegateItem.setObject(value);

//            propertyDelegateItem._set = PropertyWrapper.CreateSetTargetDelegate2(obj.GetType(), propertyInfo);
//            propertyDelegateItem.setObject(a1_p, value);

//            Assert.Equal(((A3)obj).Class, value);
//        }

//        [Test]
//        public unsafe void Class_Struct()
//        {
//            A3 a1 = new A3();

//            ItemStruct v = new ItemStruct();
//            v.Num = 999;

//            //a1.Struct = v;
//            object obj = a1;
//            object value = v;

//            PropertyInfo propertyInfo = obj.GetType().GetProperty(nameof(a1.Struct));

//            PropertyDelegateItem2 propertyDelegateItem = new PropertyDelegateItem2();
//            //PropertyDelegateItem propertyDelegateItem = PropertyWrapper.CreateStructTargetSetDelegate(propertyInfo);

//            var a1_p = GeneralTool.ObjectToVoid(obj);
//            //*propertyDelegateItem.setTarget = (IntPtr)a1_p;
//            //propertyDelegateItem.setObject(value);

//            propertyDelegateItem._set = PropertyWrapper.CreateSetTargetDelegate2(obj.GetType(), propertyInfo);
//            propertyDelegateItem.setObject(a1_p, value);

//            Assert.Equal(((A3)obj).Struct, value);
//        }

//        [Test]
//        public unsafe void Struct_Value()
//        {
//            //A2 a1 = new A2();
//            //a1.Num = 999;

//            //object obj = a1;

//            //PropertyInfo propertyInfo = obj.GetType().GetProperty("Num");

//            //Delegate setValue = PropertyWrapper.CreateSetTargetDelegate(propertyInfo);

//            //PropertyDelegateItem propertyDelegateItem = new PropertyDelegateItem();
//            //propertyDelegateItem.Set = setValue;

//            //var a1_p = GeneralTool.ObjectToVoid(obj);
//            //*propertyDelegateItem.setTarget = (IntPtr)a1_p;
//            //propertyDelegateItem.setInt32(777);

//            //Assert.Equal(((A2)obj).Num, 777);
//        }

//        [Test]
//        public unsafe void Struct_Class()
//        {
//            //A2 a1 = new A2();

//            //ItemClass v = new ItemClass();
//            //v.Num = 999;

//            ////a1.Class = v;
//            //object obj = a1;
//            //object value = v;

//            //PropertyInfo propertyInfo = obj.GetType().GetProperty(nameof(a1.Class));

//            //Delegate setValue = PropertyWrapper.CreateSetTargetDelegate(propertyInfo);

//            //PropertyDelegateItem propertyDelegateItem = new PropertyDelegateItem();
//            //propertyDelegateItem.Set = setValue;

//            //var a1_p = GeneralTool.ObjectToVoid(obj);
//            //*propertyDelegateItem.setTarget = (IntPtr)a1_p;
//            //propertyDelegateItem.setObject(value);

//            //Assert.Equal(((A2)obj).Class, value);
//        }

//        [Test]
//        public unsafe void Struct_Struct()
//        {
//            A2 a1 = new A2();

//            ItemStruct v = new ItemStruct();
//            v.Num = 999;

//            //a1.Struct = v;
//            object obj = a1;
//            object value = v;

//            PropertyInfo propertyInfo = obj.GetType().GetProperty(nameof(a1.Struct));

//            Delegate set = PropertyWrapper.CreateStructIPropertyWrapperTarget(obj.GetType(),
//                propertyInfo, out Delegate sourceDelegate);

//            PropertyDelegateItem2 item2 = new PropertyDelegateItem2();
//            item2._set = set;

//            var a1_p = GeneralTool.ObjectToVoid(a1);
//            item2.setObject(&a1, value);

//            Assert.Equal(a1.Struct, value);
//        }



//        [Test]
//        public unsafe void Class_Struct2()
//        {
//            //A3 a1 = new A3();

//            //ItemStruct v = new ItemStruct();
//            //v.Num = 999;

//            ////a1.Struct = v;
//            //object obj = a1;
//            //object value = v;

//            //PropertyInfo propertyInfo = obj.GetType().GetProperty(nameof(a1.Struct));

//            //IPropertyWrapperTarget wrapperTarget = PropertyWrapper.CreateStructIPropertyWrapperTarget(propertyInfo);
//            //MulticastDelegateValue multicastDelegateValue = new MulticastDelegateValue();
//            //var p = &multicastDelegateValue;

//            //GeneralTool.Memcpy(p,
//            //    GeneralTool.ObjectToVoid(wrapperTarget.set), 
//            //    sizeof(MulticastDelegateValue));

//            //PropertyDelegateItem propertyDelegateItem = new PropertyDelegateItem();
//            ////propertyDelegateItem._set = GeneralTool.VoidToObject(p);
//            ////var logp = (IntPtr*)p;
//            ////++logp;
//            //propertyDelegateItem.setTarget = (IntPtr*)&(multicastDelegateValue._target);

//            //wrapperTarget.set = (Delegate)GeneralTool.VoidToObject(p);

//            //Action<object> setValueCall = wrapperTarget.Set;
//            //propertyDelegateItem._setDelegate = setValueCall;


//            ////propertyDelegateItem.target = multicastDelegateValue

//            //var a1_p = GeneralTool.ObjectToVoid(obj);
//            //*propertyDelegateItem.setTarget = (IntPtr)a1_p;
//            ////  multicastDelegateValue._target = a1_p;

//            ////item.Set(value);
//            //propertyDelegateItem.setObject(value);

//            //Assert.Equal(((A3)obj).Struct, value);
//        }


//        [Test]
//        public unsafe void Class_Value2()
//        {
//            //A3 a1 = new A3();
//            //a1.Num = 999;
//            //object obj = a1;

//            //PropertyInfo propertyInfo = obj.GetType().GetProperty("Num");

//            //Delegate setValue = PropertyWrapper.CreateSetTargetDelegate(propertyInfo);

//            //PropertyDelegateItem propertyDelegateItem = new PropertyDelegateItem();
//            ////propertyDelegateItem.Get = setValue;

//            //MulticastDelegateValue multicastDelegateValue = new MulticastDelegateValue();

//            //GeneralTool.Memcpy(&multicastDelegateValue,
//            //    GeneralTool.ObjectToVoid(setValue),
//            //    sizeof(MulticastDelegateValue));
//            //propertyDelegateItem.SetDelegateIntPtr = &multicastDelegateValue;



//            //var a1_p = GeneralTool.ObjectToVoid(obj);
//            ////multicastDelegateValue._target = a1_p;
//            //*propertyDelegateItem.setTargetPtr = a1_p;

//            //propertyDelegateItem.setInt32(777);

//            //Assert.Equal(((A3)obj).Num, 777);
//        }





//    }
//}
