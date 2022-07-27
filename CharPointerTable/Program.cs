using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
namespace CustomReflection
{
    class Program
    {

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

    }

}
