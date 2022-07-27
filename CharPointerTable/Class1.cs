using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomReflection
{
    public class Class1
    {
        public class TClass001
        {
            public object[] objects;
            public double TestDuble;
            public TClass002 tClass002;
            public TClass002[] tClass002s;
        }
        public class TClass002
        {
            public double size;
            public string testString;
            public TClass003 tClass003;
            public TClass003[] tClass003s;
        }
        public class TClass003
        {
            public string testString;
        }
        public class A
        {
            public bool b;
            public double num;
            public int kk;
            public string str;
        }
        public struct V3
        {
            public long Name;
            public long Name2;
            public long Name3;
        }
        public unsafe static void Main2(string[] args)
        {
            //DynamicCreateType();
            TypeAddrReflectionWrapper wrapper = new TypeAddrReflectionWrapper(typeof(V3));
            GCHandle gcHandle;
            object obj = wrapper.Create();

            object objV3 = new V3();

            var a1 = UnsafeUtility.GetTypeHead(typeof(V3));
            var a2 = UnsafeUtility.GetTypeHead(typeof(System.ValueType));
            var cc222 = (IntPtr*)UnsafeUtility.GetObjectAddr(objV3);

            var ddd = new long[5];
            long* dddddr = (long*)UnsafeUtility.GetObjectAddr(ddd);
            *dddddr = (long)typeof(V3).TypeHandle.Value;



            var stackIntPtr = Marshal.AllocHGlobal(12 * Marshal.SizeOf(typeof(int)));
            int* pool = (int*)stackIntPtr.ToPointer();
            for (int i = 0; i < 12; i++)
            {
                pool[i] = i;
            }
            //Marshal.ReAllocHGlobal(stackIntPtr, IntPtr)



            Console.Read();
        }




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
                  typeof(void*), new Type[] { typeof(object) }
                );

            ILGenerator ilGenerator4 = mainMethodBuilder4.GetILGenerator();

            ilGenerator4.Emit(OpCodes.Ldarg_0);
            ilGenerator4.Emit(OpCodes.Ret);


            //使用动态类创建类型
            Type classType = tb.CreateType();
            //保存动态创建的程序集 (程序集将保存在程序目录下调试时就在Debug下)
            dynamicAssembly.Save(DemoName.Name + ".dll");
            //创建类
            return classType;
        }
    }




}
