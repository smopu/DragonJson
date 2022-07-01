using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DogJsonTest.Read;
using DogJson;
using System.Diagnostics;

namespace DogJsonTest
{
    public class Program
    {
        public static unsafe void Main(string[] args)
        {

            JsonManager.Start(new AddrToObject2());//ReflectionToObject
            JsonRender jsonRender = new JsonRender();

            string testPath = Path.GetDirectoryName(typeof(JsonRender).Assembly.Location) + @"\JsonFile\" + nameof(JsonReadTestClassA.ReadClassTestJsonClassA) + ".json";
            string str = File.ReadAllText(testPath, Encoding.Unicode);

            JsonReadTestClassA.TestJsonClassA o = jsonRender.ReadJsonTextCreateObject<JsonReadTestClassA.TestJsonClassA>(str);

            //Assert.AreEqualObject(o, test1);
            Stopwatch oTime = new Stopwatch();
            int testConst = 10000;

            oTime.Reset(); oTime.Start();
            for (int __1 = 0; __1 < testConst; __1++)
            {
                jsonRender.ReadJsonText(str);
            }
            oTime.Stop();
            double time001 = oTime.Elapsed.TotalMilliseconds;
            Console.WriteLine("1：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);
            o = jsonRender.ReadJsonTextCreateObject<JsonReadTestClassA.TestJsonClassA>(str);

            //string strd = File.ReadAllText("TextFile1.json");

            Stopwatch oTime2 = new Stopwatch();
            oTime2.Reset(); oTime2.Start();
            for (int __1 = 0; __1 < testConst; __1++)
            {
                var ot = jsonRender.ReadJsonTextCreateObject<JsonReadTestClassA.TestJsonClassA>(str);
                //AddrToObject2.indexDbug++;
            }


            //GC.Collect();
            oTime2.Stop();
            Console.WriteLine("C：{0} 毫秒", (oTime2.Elapsed - oTime.Elapsed).TotalMilliseconds);
            Console.WriteLine("2：{0} 毫秒", (oTime2.Elapsed).TotalMilliseconds);

            double time002 = (oTime2.Elapsed - oTime.Elapsed).TotalMilliseconds;
            Console.WriteLine(time002 / time001);
            Console.ReadKey();
        }
    }
}
