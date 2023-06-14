using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PtrReflection.Test
{
    public struct Vector3
    {
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public float x;
        public float y;
        public float z;
        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }

        public unsafe override int GetHashCode()
        {
            float num = x * y * z;
            if (num == 0f)
            {
                return 0;
            }
            return *(int*)(&num);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3)
            {
                Vector3 v = (Vector3)obj;
                return v.x == this.x && v.y == this.y && v.z == this.z;
            }
            else
            {
                return false;
            }
        }
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

    }

    public class MyClass
    {
        public int one;
        public string str;
        public Vector3 point;

        public int One { get; set; }
        public string Str { get; set; }
        public Vector3 Point { get; set; }


        public int[,,] oness;
        public string[,] strss;
        public Vector3[,] pointss;

        public int[] ones;
        public string[] strs;
        public Vector3[] points;


        public int point3;
        public int dd = 11;
    }


    public struct MyClassStruct2
    {
        public int one;
        //public string str;
        //public Vector3 point;

        //public int One { get; set; }
        //public string Str { get; set; }
        //public Vector3 Point { get; set; }


        //public int[,,] oness;
        //public string[,] strss;
        //public Vector3[,] pointss;

        //public int[] ones;
        //public string[] strs;
        //public Vector3[] points;


        //public int point3;
    }

    public struct MyClassStruct
    {
        public int one;
        public string str;
        public Vector3 point;

        public int One { get; set; }
        public string _Str;
        public string Str {
            get { 
                return _Str;
            }
            set {
                _Str = value;
            }
        }
        public Vector3 Point { get; set; }


        public int[,,] oness;
        public string[,] strss;
        public Vector3[,] pointss;

        public int[] ones;
        public string[] strs;
        public Vector3[] points;
    }
}
