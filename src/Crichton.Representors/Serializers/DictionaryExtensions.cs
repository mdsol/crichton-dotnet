using System.Collections.Generic;
using System.Linq;

namespace Crichton.Representors.Serializers
{
    // adapted from http://stackoverflow.com/a/2679857
    public static class DictionaryExtensions
    {
        public static T MergeLeft<T, TK, TV>(this T left, params IDictionary<TK, TV>[] right)
            where T : IDictionary<TK, TV>, new()
        {
            var newMap = new T();
            foreach (var p in (new List<IDictionary<TK, TV>> { left }).Concat(right).SelectMany(src => src))
            {
                newMap[p.Key] = p.Value;
            }
            return newMap;
        }

    }
}