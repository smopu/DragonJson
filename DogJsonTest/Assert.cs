using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJsonTest
{
    public class Assert
    {
        public static void AreEqual(double expected, double actual, double delta) 
        {
            double d = Math.Abs(expected - actual);
            if (expected * delta < d)
            {
                throw new Exception("");
            }
        }

        public static void AreEqual(float expected, float actual, double delta)
        {
            double d = Math.Abs(expected - actual);
            if (expected * delta < d)
            {
                throw new Exception("");
            }
        }

        public static void AreEqual(int expected, int actual)
        {
            if (expected != actual)
            {
                throw new Exception("");
            }
        }

        public static void AreEqual(object expected, object actual)
        {
            
        }




    }
}
