using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DragonJson
{

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MulticastDelegateValue
    {
        public IntPtr typePointer;

        public void* _target;

        public void* _methodBase;

        public IntPtr _methodPtr;

        public IntPtr _methodPtrAux;

        public IntPtr _invocationList;

        //object
        public void* _invocationCount;

        private IntPtr __alignment;
    }


}
