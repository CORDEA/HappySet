using System;
using System.Collections.Generic;
using System.Linq;

namespace HappySet
{
    public class HappySet<T> : HashSet<T> where T : IMHappy
    {
        public int MaxHistory { get; }

        public int CommitCount => Commits.Count;

        private Dictionary<string, Commit<T>> Commits { get; } = new Dictionary<string, Commit<T>>();

        private List<string> Histories { get; } = new List<string>();

        private long EpochTime
        {
            get
            {
                var span = DateTime.UtcNow - new DateTime(1970, 1, 1);
                return (long) span.TotalMilliseconds;
            }
        }

        public HappySet(int maxHistory = 5)
        {
            MaxHistory = maxHistory;
        }

        public HappySet(IEnumerable<T> collection, int maxHistory = 5)
        {
            foreach (var col in collection)
            {
                Add(col);
            }
            MaxHistory = maxHistory;
        }

        public string Commit(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException();
            }
            var newObj = new HashSet<T>(this.Select(item => (T) item.Clone()));
            var commit = new Commit<T>(message, newObj);
            var key = $"{commit.HashSet.GetHashCode()}-{EpochTime}";

            Histories.Add(key);
            Commits.Add(key, commit);
            if (Commits.Count > MaxHistory)
            {
                var old = Histories.Last();
                Commits.Remove(old);
                Histories.Remove(old);
            }
            return key;
        }

        public void Reset(string id)
        {
            Checkout(id);
            foreach (var history in Histories.ToList())
            {
                Commits.Remove(history);
                Histories.Remove(history);
                if (history.Equals(id))
                {
                    break;
                }
            }
        }

        public List<Log> Logs()
        {
            return Commits
                .Select(item => new Log(item.Key, item.Value.Message))
                .ToList();
        }

        public Log Log(string id)
        {
            if (!Commits.ContainsKey(id))
            {
                throw new KeyNotFoundException();
            }
            var commit = Commits[id];
            return new Log(id, commit.Message);
        }

        public void Checkout(string id)
        {
            if (!Commits.ContainsKey(id))
            {
                throw new KeyNotFoundException();
            }
            Clear();
            foreach (var commit in Commits[id].HashSet)
            {
                Add(commit);
            }
        }

        public HashSet<T> CherryPick(string id)
        {
            if (!Commits.ContainsKey(id))
            {
                throw new KeyNotFoundException();
            }
            return Commits[id].HashSet;
        }

        public void ClearHistories()
        {
            Commits.Clear();
            Histories.Clear();
        }
    }
}