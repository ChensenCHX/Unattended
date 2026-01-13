using System;
using System.Collections.Generic;

namespace Utils
{
    public static class RandomsAndShuffle
    {
        public static Random Rng { get; } = new Random();
        public static List<T> Shuffle<T>(this List<T> list, Random rng=null)
        {
            rng ??= Rng;
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list;
        }
        
    }
}