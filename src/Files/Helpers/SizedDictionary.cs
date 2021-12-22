using System;
using System.Collections.Generic;
using System.Linq;

namespace Files.Helpers
{
    public sealed class SizedDictionary<Key, Value> : Dictionary<Key, Value>
    {
        private readonly int maxSize;

        private Queue<Key> keys = new();

        public SizedDictionary(int maxSize) : base(maxSize) => this.maxSize = maxSize;

        new public void Add(Key key, Value value)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            if (keys.Count == maxSize)
            {
                base.Remove(keys.Dequeue());
            }

            base.Add(key, value);
            keys.Enqueue(key);
        }

        new public bool Remove(Key key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            if (!keys.Contains(key))
            {
                return false;
            }

            keys = new Queue<Key>(keys.Where(k => !k.Equals(key)));
            return base.Remove(key);
        }
    }
}
