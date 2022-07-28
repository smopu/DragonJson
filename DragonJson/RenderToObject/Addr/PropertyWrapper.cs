using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DragonJson
{
    public delegate TResult RefFunc<T, out TResult>(ref T arg);
    public delegate void RefAction<T1, T2>(ref T1 arg1, T2 arg2);
    public unsafe delegate void ActionVoidPtr<T2>(void* arg1, T2 arg2);
    public unsafe delegate void ActionVoidPtrVoidPtr(void* arg1, void* arg2);


    [StructLayout(LayoutKind.Explicit)]
    public unsafe class PropertyDelegateItem2
    {
        [FieldOffset(0)]
        public object _set;
        [FieldOffset(0)]
        public Delegate _setDelegate;
        [FieldOffset(0)]
        public Action<object> setObject;
        [FieldOffset(0)]
        public Action<bool> setBoolean;
        [FieldOffset(0)]
        public Action<char> setChar;
        [FieldOffset(0)]
        public Action<sbyte> setSByte;
        [FieldOffset(0)]
        public Action<byte> setByte;
        [FieldOffset(0)]
        public Action<short> setInt16;
        [FieldOffset(0)]
        public Action<ushort> setUInt16;
        [FieldOffset(0)]
        public Action<int> setInt32;
        [FieldOffset(0)]
        public Action<uint> setUInt32;
        [FieldOffset(0)]
        public Action<long> setInt64;
        [FieldOffset(0)]
        public Action<ulong> setUInt64;
        [FieldOffset(0)]
        public Action<float> setSingle;
        [FieldOffset(0)]
        public Action<double> setDouble;
        [FieldOffset(0)]
        public Action<decimal> setDecimal;
        [FieldOffset(0)]
        public Action<DateTime> setDateTime;
        [FieldOffset(0)]
        public Action<string> setString;

        [FieldOffset(8)]
        public IntPtr* setTarget;
        [FieldOffset(8)]
        public void** setTargetPtr;

        public object Set
        {
            get
            {
                return _set;
            }
            set
            {
                _set = value;
                var logp = (IntPtr*)GeneralTool.ObjectToVoid(value);
                ++logp;
                setTarget = logp;
            }
        }

        public void* SetDelegateIntPtr
        {
            set
            {
                _set = GeneralTool.VoidToObject(value);
                var logp = (IntPtr*)value;
                ++logp;
                setTarget = logp;
            }
        }

        [FieldOffset(16)]
        public object _get;
        [FieldOffset(16)]
        public Delegate _getDelegate;
        [FieldOffset(16)]
        public Func<object> getObject;
        [FieldOffset(16)]
        public Func<bool> getBoolean;
        [FieldOffset(16)]
        public Func<char> getChar;
        [FieldOffset(16)]
        public Func<sbyte> getSByte;
        [FieldOffset(16)]
        public Func<byte> getByte;
        [FieldOffset(16)]
        public Func<short> getInt16;
        [FieldOffset(16)]
        public Func<ushort> getUInt16;
        [FieldOffset(16)]
        public Func<int> getInt32;
        [FieldOffset(16)]
        public Func<uint> getUInt32;
        [FieldOffset(16)]
        public Func<long> getInt64;
        [FieldOffset(16)]
        public Func<ulong> getUInt64;
        [FieldOffset(16)]
        public Func<float> getSingle;
        [FieldOffset(16)]
        public Func<double> getDouble;
        [FieldOffset(16)]
        public Func<decimal> getDecimal;
        [FieldOffset(16)]
        public Func<DateTime> getDateTime;
        [FieldOffset(16)]
        public Func<string> getString;

        [FieldOffset(24)]
        public IntPtr* getTarget;
        [FieldOffset(24)]
        public void** getTargetPtr;

        [FieldOffset(32)]
        public int debug = 0;
        public void* GetDelegateIntPtr
        {
            set
            {
                _get = GeneralTool.VoidToObject(value);
                var logp = (IntPtr*)value;
                ++logp;
                getTarget = logp;
            }
        }

        //[FieldOffset(16)]
        //public Delegate get2;
        //[FieldOffset(16)]
        //public Action<object, object> SetObject2;
        //[FieldOffset(16)]
        //public Action<object, double> get_double2;
        //[FieldOffset(16)]
        //public Action<object, float> get_float2;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe class PropertyDelegateItem
    {
        [FieldOffset(0)]
        public object _set;
        [FieldOffset(0)]
        public Delegate _setDelegate;
        [FieldOffset(0)]
        public ActionVoidPtr<object> setObject;
        [FieldOffset(0)]
        public ActionVoidPtrVoidPtr setVoidPtr;

        [FieldOffset(0)]
        public ActionVoidPtr<bool> setBoolean;
        [FieldOffset(0)]
        public ActionVoidPtr<char> setChar;
        [FieldOffset(0)]
        public ActionVoidPtr<sbyte> setSByte;
        [FieldOffset(0)]
        public ActionVoidPtr<byte> setByte;
        [FieldOffset(0)]
        public ActionVoidPtr<short> setInt16;
        [FieldOffset(0)]
        public ActionVoidPtr<ushort> setUInt16;
        [FieldOffset(0)]
        public ActionVoidPtr<int> setInt32;
        [FieldOffset(0)]
        public ActionVoidPtr<uint> setUInt32;
        [FieldOffset(0)]
        public ActionVoidPtr<long> setInt64;
        [FieldOffset(0)]
        public ActionVoidPtr<ulong> setUInt64;
        [FieldOffset(0)]
        public ActionVoidPtr<float> setSingle;
        [FieldOffset(0)]
        public ActionVoidPtr<double> setDouble;
        [FieldOffset(0)]
        public ActionVoidPtr<decimal> setDecimal;
        [FieldOffset(0)]
        public ActionVoidPtr<DateTime> setDateTime;
        [FieldOffset(0)]
        public ActionVoidPtr<string> setString;

        [FieldOffset(16)]
        public object _get;
        [FieldOffset(16)]
        public Delegate _getDelegate;
        [FieldOffset(16)]
        public Func<object> getObject;
        [FieldOffset(16)]
        public Func<bool> getBoolean;
        [FieldOffset(16)]
        public Func<char> getChar;
        [FieldOffset(16)]
        public Func<sbyte> getSByte;
        [FieldOffset(16)]
        public Func<byte> getByte;
        [FieldOffset(16)]
        public Func<short> getInt16;
        [FieldOffset(16)]
        public Func<ushort> getUInt16;
        [FieldOffset(16)]
        public Func<int> getInt32;
        [FieldOffset(16)]
        public Func<uint> getUInt32;
        [FieldOffset(16)]
        public Func<long> getInt64;
        [FieldOffset(16)]
        public Func<ulong> getUInt64;
        [FieldOffset(16)]
        public Func<float> getSingle;
        [FieldOffset(16)]
        public Func<double> getDouble;
        [FieldOffset(16)]
        public Func<decimal> getDecimal;
        [FieldOffset(16)]
        public Func<DateTime> getDateTime;
        [FieldOffset(16)]
        public Func<string> getString;

        [FieldOffset(24)]
        public IntPtr* getTarget;
        [FieldOffset(24)]
        public void** getTargetPtr;

        [FieldOffset(32)]
        public int debug = 0;
        public void* GetDelegateIntPtr
        {
            set
            {
                _get = GeneralTool.VoidToObject(value);
                var logp = (IntPtr*)value;
                ++logp;
                getTarget = logp;
            }
        }

        //[FieldOffset(16)]
        //public Delegate get2;
        //[FieldOffset(16)]
        //public Action<object, object> SetObject2;
        //[FieldOffset(16)]
        //public Action<object, double> get_double2;
        //[FieldOffset(16)]
        //public Action<object, float> get_float2;
    }

    public interface IPropertyWrapper
    {
        void Set(object target, object value);
        object Get(object target);      
        void SetGet(Delegate d);
        void SetSet(Delegate d);
    }


    public class PropertyWrapper
    {
        public static IPropertyWrapper Create(Type type, PropertyInfo propertyInfo)
        {
            Type propertyType = propertyInfo.PropertyType;
            if (type.IsValueType)
            {
                var setValue = Delegate.CreateDelegate(typeof(RefAction<,>).MakeGenericType(type, propertyType),
                   propertyInfo.GetSetMethod());
                var getValue = Delegate.CreateDelegate(typeof(RefFunc<,>).MakeGenericType(type, propertyType),
                   propertyInfo.GetGetMethod());
                IPropertyWrapper propertyWrapper = (IPropertyWrapper)Activator.CreateInstance(typeof(PropertyStructWrapper<,>).MakeGenericType(type, propertyType));
                propertyWrapper.SetGet(getValue);
                propertyWrapper.SetSet(setValue);
                return propertyWrapper;
            }
            else
            {
                var setValue = Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(type, propertyType),
                   propertyInfo.GetSetMethod());
                var getValue = Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(type, propertyType),
                   propertyInfo.GetGetMethod());
                IPropertyWrapper propertyWrapper = (IPropertyWrapper)Activator.CreateInstance(typeof(PropertyClassWrapper<,>).MakeGenericType(type, propertyType));
                propertyWrapper.SetGet(getValue);
                propertyWrapper.SetSet(setValue);
                return propertyWrapper;
            }
        }

        public static unsafe PropertyDelegateItem2 CreateStructTargetSetDelegate(PropertyInfo propertyInfo)
        {
            var setValue = propertyInfo.GetSetMethod().
                CreateDelegate(typeof(Action<>).MakeGenericType(propertyInfo.PropertyType), null);

            IPropertyWrapperTarget propertyWrapper = (IPropertyWrapperTarget)Activator.CreateInstance(
                typeof(PropertyWrapperTarget<>).MakeGenericType(propertyInfo.PropertyType));
            propertyWrapper.set = setValue;
            Action<object> setValueCall = propertyWrapper.Set;

            PropertyDelegateItem2 propertyDelegateItem = new PropertyDelegateItem2();
            propertyDelegateItem._setDelegate = setValueCall;
            var logp = (IntPtr*)GeneralTool.ObjectToVoid(setValue);
            ++logp;
            propertyDelegateItem.setTarget = logp;

            return propertyDelegateItem;
        }



        public static unsafe IPropertyWrapperTarget CreateStructIPropertyWrapperTarget(PropertyInfo propertyInfo)
        {
            var setValue = propertyInfo.GetSetMethod().
                CreateDelegate(typeof(Action<>).MakeGenericType(propertyInfo.PropertyType), null);

            IPropertyWrapperTarget propertyWrapper = (IPropertyWrapperTarget)Activator.CreateInstance(
                typeof(PropertyWrapperTarget<>).MakeGenericType(propertyInfo.PropertyType));
            propertyWrapper.set = setValue;

            return propertyWrapper;
        }

        public static unsafe Delegate CreateStructIPropertyWrapperTarget(Type type, PropertyInfo propertyInfo, out Delegate sourceDelegate)
        {
            if (type.IsValueType)
            {
                var setValue = Delegate.CreateDelegate(typeof(RefAction<,>).MakeGenericType(type, propertyInfo.PropertyType),
               propertyInfo.GetSetMethod());

                IGetSetStruct propertyWrapper = (IGetSetStruct)Activator.CreateInstance(
                    typeof(RefPropertyWrapper<,>).MakeGenericType(type, propertyInfo.PropertyType));
                propertyWrapper.set = setValue;
                sourceDelegate = setValue;
                return propertyWrapper.GetDelegate();
            }
            else
            {
                var setValue = Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(type, propertyInfo.PropertyType),
                propertyInfo.GetSetMethod());

                IGetSetStruct propertyWrapper = (IGetSetStruct)Activator.CreateInstance(
                    typeof(PropertyWrapper<,>).MakeGenericType(type, propertyInfo.PropertyType));
                propertyWrapper.set = setValue;
                sourceDelegate = setValue;
                return propertyWrapper.GetDelegate();
            }
        }


        public static Delegate CreateGetTargetDelegate(PropertyInfo propertyInfo)
        {
            var getValue = propertyInfo.GetGetMethod().
                CreateDelegate(typeof(Func<>).MakeGenericType(propertyInfo.PropertyType), null);
            return getValue;
        }

        public static Delegate CreateSetTargetDelegate(PropertyInfo propertyInfo)
        {
            var setValue = propertyInfo.SetMethod.
                CreateDelegate(typeof(Action<>).MakeGenericType(propertyInfo.PropertyType), null);
            return setValue;
        }
        public static Delegate CreateSetTargetDelegate2(Type type, PropertyInfo propertyInfo)
        {
            if (type.IsValueType)
            {
                var setValue = Delegate.CreateDelegate(typeof(RefAction<,>).MakeGenericType(type, propertyInfo.PropertyType),
                   propertyInfo.GetSetMethod());
                return setValue;
            }
            else
            {
                var setValue = Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(type, propertyInfo.PropertyType),
                   propertyInfo.GetSetMethod());
                return setValue;
            }
        }

    }

    public class PropertyClassWrapper<Target, Value> : IPropertyWrapper
    {
        Action<Target, Value> set;
        Func<Target, Value> get;
        public void SetGet(Delegate d)
        {
            get = (Func<Target, Value>)d;
        }

        public void SetSet(Delegate d)
        {
            set = (Action<Target, Value>)d;
        }

        public object Get(object target)
        {
            return get((Target)target);
        }

        public void Set(object target, object value)
        {
            set((Target)target, (Value)value);
        }
    }


    public interface IPropertyWrapperTarget
    {
        Delegate set { get; set; }
        void Set(object value);
    }

    public class PropertyWrapperTarget<Value> : IPropertyWrapperTarget
    {
        Action<Value> _set;
        public Delegate set
        {
          get { return _set; }
          set { _set = (Action<Value>)value; }
        }


        public void Set(object value)
        {
            _set((Value)value);
        }
    }

    public interface IGetSetStruct
    {
        Delegate set { get; set; }
        Delegate GetDelegate();
    }
    public class PropertyWrapper<Target, Value> : IGetSetStruct
    {
        public Action<Target, Value> _set;
        public Delegate set
        {
          get { return _set; }
          set { _set = (Action<Target, Value>)value; }
        }

        public void SetStruct(Target t, object value)
        {
            _set(t, (Value)value);
        }
        public Delegate GetDelegate()
        {
            Action<Target, object> v = SetStruct;
            return v;
        }
    }

    public class RefPropertyWrapper<Target, Value> : IGetSetStruct
    {
        public RefAction<Target, Value> _set;
        public Delegate set
        {
          get { return _set; }
          set { _set = (RefAction<Target, Value>)value; }
        }

        public void SetStruct(ref Target t, object value)
        {
            _set(ref t, (Value)value);
        }
        public Delegate GetDelegate()
        {
            RefAction<Target, object> v = SetStruct;
            return v;
        }
    }

    public class PropertyStructWrapper<Target, Value> : IPropertyWrapper
    {
        RefAction<Target, Value> set;
        RefFunc<Target, Value> get;
        public void SetGet(Delegate d)
        {
            get = (RefFunc<Target, Value>)d;
        }

        public void SetSet(Delegate d)
        {
            set = (RefAction<Target, Value>)d;
        }

        public object Get(object target)
        {
            var d = (Box<Target>)target;
            return get(ref d.value);
        }

        public void Set(object target, object value)
        {
            var d = (Box<Target>)target;
            set(ref d.value, (Value)value);
        }
    }


}
