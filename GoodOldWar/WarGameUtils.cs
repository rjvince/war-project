using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodOldWar
{
    public static class WarGameUtils
    {
        private static Random rand = new Random();

        public static void Shuffle<T>(this IList<T> list, int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int swap = rand.Next(n + 1);
                    T temp = list[swap];
                    list[swap] = list[n];
                    list[n] = temp;
                }
            }
        }

        public static T Top<T>(this IList<T> list)
        {
            T topItem = list[0];
            list.RemoveAt(0);
            return topItem;
        }
    }
}
