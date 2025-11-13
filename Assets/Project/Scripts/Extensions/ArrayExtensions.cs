using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class ArrayExtensions
    {
        public static T GetRandom<T>(this IEnumerable<T> array)
        {
            var newArray = array.ToArray();
            var randomIndex = Random.Range(0, newArray.Length);
            return newArray[randomIndex];
        }
    }
}