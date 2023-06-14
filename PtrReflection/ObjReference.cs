using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PtrReflection
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ObjReference
    {
        [FieldOffset(0)]
        public object obj;
        public ObjReference(object obj)
        {
            this.obj = obj;
        }
    }
    public unsafe struct ObjReferenceStruct
    {
        public IntPtr obj;
        public ObjReferenceStruct(IntPtr obj)
        {
            this.obj = obj;
        }
    }
}
