using System;
using System.Collections.Generic;

namespace Files.Helpers
{
    public interface ISizedDictionary<Key, Value> : IDictionary<Key, Value>
    {
        int Size { get; set; }
    }

    public class SizedDictionary<Key, Value> : Dictionary<Key, Value>, ISizedDictionary<Key, Value>
    {
        private readonly List<Key> keys = new();

        private int size = 0;
        public int Size
        {
            get => size;
            set
            {
                if (size != value)
                {
                    size = value;

                    while (keys.Count > size)
                    {
                        RemoveOlderKey();
                    }
                }
            }
        }

        public SizedDictionary(int size) : base(size) => this.size = size;

        new public void Add(Key key, Value value)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            while (keys.Count >= size)
            {
                RemoveOlderKey();
            }

            base.Add(key, value);
            keys.Add(key);
        }

        new public bool Remove(Key key)
        {
            if (key is null || !keys.Contains(key))
            {
                return false;
            }

            keys.Remove(key);
            return base.Remove(key);
        }

        private void RemoveOlderKey()
        {
            Key olderKey = keys[0];
            keys.RemoveAt(0);
            base.Remove(olderKey);
        }
    }
}
