using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DogJson
{
    public static class DataBaseExtend
    {
        private static List<T> RandomSort<T>(this List<T> list)
        {
            var random = new System.Random();
            var newList = new List<T>();
            foreach (var item in list)
            {
                newList.Insert(random.Next(newList.Count), item);
            }
            return newList;
        }

        public static List<TSource> ToLists<TSource>(this IEnumerable<TSource> source)
        {
            return source.ToList();
        }
        public static object StringToEnum(this string v, Type enumType)
        {
            Array Arrays = Enum.GetValues(enumType);
            for (int i = 0; i < Arrays.Length; i++)
            {
                if (Arrays.GetValue(i).ToString().Equals(v))
                {
                    return Arrays.GetValue(i);
                }
            }
            return Arrays.GetValue(0);
        }
        public static T StringToEnum<T>(this string v)
        {
            Array Arrays = Enum.GetValues(typeof(T));
            for (int i = 0; i < Arrays.Length; i++)
            {
                if (Arrays.GetValue(i).ToString().Equals(v))
                {
                    return (T)Arrays.GetValue(i);
                }
            }
            return (T)Arrays.GetValue(0);
        }

        public static bool StringToBool(this string v)
        {
            if (v.Equals("True") || v.Equals("1") || v.Equals("true") || v.Equals("yes") || v.Equals("Yes"))
                return true;
            return false;
        }


        //public static Player StringToPlayer(this string v)
        //{
        //    string[] ss = v.Split('|');
        //    ulong sceneId = ulong.Parse(ss[0]);
        //    int playerId = int.Parse(ss[1]);
        //    return SceneYgo.GetScene(sceneId).allPlayer[playerId];
        //}

        //public static SceneYgo StringToScene(this string v)
        //{
        //    ulong sceneId = ulong.Parse(v);
        //    return SceneYgo.GetScene(sceneId);
        //}

        //public static string PlayerToString(this Player player)
        //{
        //    return player.scene.id + "|" + player.id;
        //}

        //public static string ItemToString(this Item item)
        //{
        //    return item.scene.id + "|" + item.instanceId;
        //}

        //public static Item StringToItem(this string v)
        //{
        //    string[] ss = v.Split('|');
        //    ulong sceneId = ulong.Parse(ss[0]);
        //    ulong itemId = ulong.Parse(ss[1]);
        //    return SceneYgo.GetScene(sceneId).allItem[itemId];
        //}

        //public static string DecimalIntToString(this DecimalInt item)
        //{
        //    return item.v.ToString();
        //}

        //public static DecimalInt StringToDecimalInt(this string s)
        //{
        //    long v = long.Parse(s);
        //    DecimalInt item = new DecimalInt();
        //    item.v = v;
        //    return item;
        //}

    }
}
