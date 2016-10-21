using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpecFlow.FsCheck
{
    public abstract class ParamDictionaryBase<TValue> : IReadOnlyList<TValue>
    {
        private readonly List<KeyValuePair<string, TValue>> paramList;

        public IEnumerator<TValue> GetEnumerator()
        {
            return paramList.Select(keyValuePair => keyValuePair.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => paramList.Count;
        public TValue this[int index] => paramList[index].Value;
        public TValue this[string key] => paramList.Single(p => p.Key == key).Value;
        public IEnumerable<KeyValuePair<string, TValue>> KeyValuePairs => paramList;

        protected ParamDictionaryBase()
        {
            paramList = new List<KeyValuePair<string, TValue>>(3);
        }

        protected ParamDictionaryBase(IEnumerable<KeyValuePair<string, TValue>> items)
        {
            paramList = new List<KeyValuePair<string, TValue>>(items);
        }

        public void Clear()
        {
            paramList.Clear();
        }

        protected void Add(string key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException($"Key '{key}' already exists.", nameof(key));

            paramList.Add(new KeyValuePair<string, TValue>(key, value));
        }

        public bool ContainsKey(string key)
        {
            return paramList.Any(p => p.Key.Equals(key));
        }
    }
}