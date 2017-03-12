using System.Collections.Generic;

namespace HappySet
{
    public class Commit<T> where T : IMHappy
    {
        public string Message { get; }

        public HashSet<T> HashSet { get; }

        public Commit(string message, HashSet<T> hashSet)
        {
            Message = message;
            HashSet = hashSet;
        }
    }
}