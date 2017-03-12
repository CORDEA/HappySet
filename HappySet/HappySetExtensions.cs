using System;
using System.Collections.Generic;

namespace HappySet
{
    public static class HappySetExtensions
    {
        public static T FirstOrDefault<T>(this HappySet<T> set, Func<T> def = null) where T : IMHappy
        {
            using (IEnumerator<T> e = set.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    return e.Current;
                }
            }
            return def == null ? default(T) : def.Invoke();
        }

        public static T FirstOrDefault<T>(this HappySet<T> set, T def = default(T)) where T : IMHappy
        {
            using (IEnumerator<T> e = set.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    return e.Current;
                }
            }
            return def;
        }
    }
}