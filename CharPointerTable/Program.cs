using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
namespace CustomReflection
{
    class Program
    {
        public class Wepon
        {
            public string Name;
        }

        public class People
        {
            public int Age;
            public int Tow { get; set; }

            public byte Sex;

            public string Name;

            public long Time;

            public Wepon wepon;
        }

        //IntPtr ConventToIntPtr (object pVoid)
        //{
        //   var handle = GCHandle.Alloc(pVoid);  //记得要释放  handle.Free();
        //    IntPtr mpVoid = GCHandle.ToIntPtr(handle);
        //}

        //void freeObject()
        //{
        //    handle.Free();
        //}

        //object ConventToobject(IntPtr pVoid)
        //{
        //    Object obj = GCHandle.FromIntPtr(pVoid).Target;
        //    UserTest temp = obj as UserTest;
        //}

        public static Type DynamicCreateType()
        {
            //动态创建程序集
            AssemblyName DemoName = new AssemblyName("DynamicCustomReflection");
            AssemblyBuilder dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(DemoName, AssemblyBuilderAccess.RunAndSave);
            //动态创建模块
            ModuleBuilder mb = dynamicAssembly.DefineDynamicModule(DemoName.Name, DemoName.Name + ".dll");
            //动态创建类MyClass

            TypeBuilder tb = mb.DefineType("GeneralTool", TypeAttributes.Public);

            MethodBuilder mainMethodBuilder2 = tb.DefineMethod("SetObject", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard,
                 null, new Type[] { typeof(void*), typeof(object) }
                );

            ILGenerator ilGenerator = mainMethodBuilder2.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stobj, typeof(object));
            ilGenerator.Emit(OpCodes.Ret);



            MethodBuilder mainMethodBuilder3 = tb.DefineMethod("VoidToObject", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard,
                  typeof(object), new Type[] { typeof(void*) }
                );

            ILGenerator ilGenerator3 = mainMethodBuilder3.GetILGenerator();
            ilGenerator3.Emit(OpCodes.Ldarg_0);
            //ilGenerator3.Emit(OpCodes.Ldobj, typeof(object));
            ilGenerator3.Emit(OpCodes.Ret);


            MethodBuilder mainMethodBuilder4 = tb.DefineMethod("ObjectToVoid", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard,
                  typeof(void*), new Type[] { typeof(object) });
            ILGenerator ilGenerator4 = mainMethodBuilder4.GetILGenerator();

            ilGenerator4.Emit(OpCodes.Ldarg_0);
            ilGenerator4.Emit(OpCodes.Ret);




            MethodBuilder mainMethodBuilder5 = tb.DefineMethod("Memcpy", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard,
                  null, new Type[] { typeof(void*), typeof(void*), typeof(int) }
                );

            ILGenerator ilGenerator5 = mainMethodBuilder5.GetILGenerator();
            ilGenerator5.Emit(OpCodes.Ldarg_0);
            ilGenerator5.Emit(OpCodes.Ldarg_1);
            ilGenerator5.Emit(OpCodes.Ldarg_2);
            ilGenerator5.Emit(OpCodes.Cpblk);
            ilGenerator5.Emit(OpCodes.Ret);
            mainMethodBuilder5.DefineParameter(1, ParameterAttributes.None, "destination");
            mainMethodBuilder5.DefineParameter(2, ParameterAttributes.None, "source");
            mainMethodBuilder5.DefineParameter(3, ParameterAttributes.None, "byteCount");


            TypeBuilder tb2 = mb.DefineType("TypeTool", TypeAttributes.Public);

            //创建类型参数名，将达到这样的效果：public MyClass<TParam1,TParam2>
            GenericTypeParameterBuilder[] gtps = tb2.DefineGenericParameters(new string[] { "T" });
            GenericTypeParameterBuilder tName1 = gtps[0];

            //为泛型添加约束，TName1将会被添加构造器约束和引用类型约束
            //tName1.SetGenericParameterAttributes(GenericParameterAttributes.DefaultConstructorConstraint | GenericParameterAttributes.NotNullableValueTypeConstraint);

            MethodBuilder methodBuilder6 = tb2.DefineMethod(
                "SizeOf",                               //方法名
                MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(int), new Type[] { });

            ILGenerator ilGenerator6 = methodBuilder6.GetILGenerator();
            ilGenerator6.Emit(OpCodes.Sizeof, tName1);
            ilGenerator6.Emit(OpCodes.Ret);
            tb2.CreateType();




            //使用动态类创建类型
            Type classType = tb.CreateType();
            //保存动态创建的程序集 (程序集将保存在程序目录下调试时就在Debug下)
            dynamicAssembly.Save(DemoName.Name + ".dll");
            //创建类
            return classType;
        }

        public struct V3
        {
            public long Name;
            public long Name2;
            public long Name3;
        }
        public struct V2
        {
            public long Name;
            public long Name2;
        }
        public class GG
        {
            public long Name;
            public long Name2;
            public long Name3;
            public long Name4;
            public FFF fff;
        }
        public class FFF
        {
            public long Name5 = 100;
            public override string ToString()
            {
                return Name5.ToString();
            }
        }

        public unsafe static void* ObjectToVoid(long P_0)
        {
            return *(void**)&P_0;
        }

        unsafe static T Create<T>(out GCHandle gCHandle)
        {
            var heapSize = UnsafeUtility.SizeOf(typeof(T));
            var sizeByte_1 = heapSize - sizeof(IntPtr) * 3;
            object objssss = new byte[sizeByte_1];
            gCHandle = GCHandle.Alloc(objssss, GCHandleType.Pinned);
            *(IntPtr*)GeneralTool.ObjectToVoid(objssss) = UnsafeUtility.GetTypeHead(typeof(T));
            return (T)objssss;
        }

        private static unsafe void* Foo99(List<GCHandle> gCHandles)
        {
            void* ptgbns; 
            
            GCHandle gCHandle;
            GG ggsssss = Create<GG>(out gCHandle);
            gCHandles.Add(gCHandle);

            ggsssss.Name = 1132;
            ptgbns = GeneralTool.ObjectToVoid(ggsssss);
            FFF fff = Create<FFF>(out gCHandle);
            gCHandles.Add(gCHandle);

            var ffffield = typeof(GG).GetField("fff");
            int fff_offset = UnsafeUtility.GetFeildOffset(ffffield);

            //ggsssss.fff = new FFF();
            byte* p = (byte*)GeneralTool.ObjectToVoid(ggsssss);
            p += sizeof(IntPtr);
            p += fff_offset;
            *(IntPtr*)p = (IntPtr)GeneralTool.ObjectToVoid(fff);
            return ptgbns;
        }


        unsafe static void Main(string[] args)
        {
            List<GCHandle> gCHandles = new List<GCHandle>();
            void* ptgbns = null;
            {
                ptgbns = Foo99(gCHandles);
            }

            object[] gcccccc = new object[1000];
            for (int i = 0; i < 1000; i++)
            {
                var kasdasds = new GG();
                gcccccc[i] = new GG();
                GC.Collect();
            }
            foreach (var item in gCHandles)
            {
                if (item != default(GCHandle))
                {
                    item.Free();
                }
            }

            gcccccc = null;
            GC.Collect();


            object obj23 = GeneralTool.VoidToObject(ptgbns);
            Console.WriteLine(((GG)obj23).fff);

            Console.ReadKey(false);



            Type type32 = DynamicCreateType();

            
            var gg = new GG();
            gg.Name = 1;
            gg.Name2 = 2;
            gg.Name3 = 3;
            gg.Name4 = 4;
            TypedReference tfdd = __makeref(gg);
            
            var handle22 = GCHandle.Alloc(gg);  //记得要释放  handle.Free();
            IntPtr mpVoid = GCHandle.ToIntPtr(handle22);

            long* cccc = (long*)mpVoid;
            List<long> list3 = new List<long>();
            for (int i = 0; i < 10; i++)
            {
                list3.Add(cccc[i]);
            }

            Object obj = GCHandle.FromIntPtr(mpVoid).Target;
            GG temp = obj as GG;
            void* ccvoid2 = UnsafeUtility.ObjectToVoid(obj);

            long* cc = (long*)UnsafeUtility.GetObjectAddr(tfdd);
            void* ccvoid = UnsafeUtility.ObjectToVoid(gg);
            object vvvv = UnsafeUtility.VoidToObject(ccvoid);
            int sizes = UnsafeUtility.SizeOf(typeof(GG));
            int sizesV3 = UnsafeUtility.SizeOf(typeof(V3));
            int sizesV32 = sizeof(V3);



            void* ccvoidP = &ccvoid;
            //object ojbk = type32.GetMethod("SetObject").Invoke(null, new object[] { ccvoid });
            //GeneralTool generalTool = new GeneralTool("c");
            //object vvvv2 = GeneralTool.GetObject(ccvoid);
            //Console.WriteLine(vvvv2);

            //void* vvvv2 = GeneralTool.ObjectToVoid(gg);

            //void* ccvoid23 = (void*)vvvv2;
            IntPtr vv2 = typeof(People).GetField("Age").FieldHandle.Value + 4 + sizeof(IntPtr);
            short os12 = *(short*)(vv2);
            byte os121 = *(byte*)(vv2 + 2);
            int over2 = os12 + (os121 << 16);



            V3 v3 = new V3();
            v3.Name = 1;
            v3.Name2 = 2;
            v3.Name3 = 3;

            object v333 = v3;
            TypedReference tfdd2 = __makeref(v333);

            long* cc222 = (long*)UnsafeUtility.GetObjectAddr(tfdd2);
            List<long> list = new List<long>();
            for (int i = 0; i < 6; i++)
            {
                list.Add(cc222[i]);
            }

            var ddd = new long[5];
            long* dddddr = (long*)UnsafeUtility.GetObjectAddr(ddd);
            *dddddr = (long)typeof(V3).TypeHandle.Value;


            int size = *((int*)typeof(GG).TypeHandle.Value + 1);

            long dfff = (long)typeof(V3).TypeHandle.Value;
            long dfffV2 = (long)typeof(V2).TypeHandle.Value;
            
            long dfff2 = *(long*)typeof(GG).TypeHandle.Value;
            var cc2 = (int*)dfff;
            List<int> list2 = new List<int>();
            for (int i = 0; i < 7; i++)
            {
                list2.Add(cc2[i]);
            }

            Type typ = typeof(People);

            var nf = typ.GetField("Age");

            var handle = nf.FieldHandle;

            int os1 = UnsafeUtility.GetFeildOffset(handle.Value) + sizeof(IntPtr);

            nf = typ.GetField("Sex");

            var handle2 = nf.FieldHandle;

            int os2 = UnsafeUtility.GetFeildOffset(handle2.Value) + sizeof(IntPtr);

            nf = typ.GetField("wepon");

            var handle3 = nf.FieldHandle;

            int os3 = UnsafeUtility.GetFeildOffset(handle3.Value) + sizeof(IntPtr);

            nf = typ.GetField("Name");

            var handle4 = nf.FieldHandle;

            int os4 = UnsafeUtility.GetFeildOffset(handle4.Value) + sizeof(IntPtr);

            People people = new People();

            people.Age = 16;

            people.Name = "Hello";

            people.Tow = 6;

            people.Sex = 4;

            people.Time = -1;

            Wepon wepon = new Wepon();

            wepon.Name = "机枪";

            TypedReference tf = __makeref(people);

            IntPtr c = UnsafeUtility.GetObjectAddr(tf);

            *(int*)(c + os1) = 200;//设置年龄

            *(byte*)(c + os2) = 3;//设置性别

            UnsafeUtility.SetObject(c + os3, wepon);//设置武器类

            UnsafeUtility.SetObject(c + os4, "打的不错哦!呵呵.");//设置名字

            Console.WriteLine(people.Age);

            Console.WriteLine(people.Sex);

            Console.WriteLine(people.wepon.Name);

            Console.WriteLine(people.Name);

            Console.ReadKey(false);

        }

    }

}
