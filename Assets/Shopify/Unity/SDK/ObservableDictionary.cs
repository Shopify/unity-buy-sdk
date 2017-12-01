namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Collections;
    using System;

    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        private IDictionary<TKey, TValue> Data;
        private DictionaryChangeHandler OnDictionaryChange;

        public ObservableDictionary(DictionaryChangeHandler onDictionaryChange) {
            Data = new Dictionary<TKey, TValue>();
            OnDictionaryChange = onDictionaryChange;
        }

        public ObservableDictionary(IDictionary<TKey, TValue> data, DictionaryChangeHandler onDictionaryChange) {
            Data = data;
            OnDictionaryChange = onDictionaryChange;
        }

        public TValue this [TKey key] {
            get {
                if (Data.ContainsKey(key)) {
                    return (TValue) Data[key];
                } else {
                    throw new KeyNotFoundException();
                }
            }
            set {
                Data[key] = value;

                OnDictionaryChange();
            }
        }

        public int Count {
            get {
                return Data.Count;
            }
        }

        public bool IsReadOnly {
            get {
                return Data.IsReadOnly;
            }
        }

        public ICollection<TKey> Keys {
            get {
                return Data.Keys;
            }
        }

        public ICollection<TValue> Values {
            get {
                return Data.Values;
            }
        }

        public bool ContainsKey(TKey key) {
            return Data.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<TKey, TValue> entry) {
            return Data.ContainsKey(entry.Key) && EqualityComparer<TValue>.Default.Equals(Data[entry.Key], entry.Value);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return Data.TryGetValue(key, out value);
        }

        public void Add(TKey key, TValue value) {
            Data.Add(key, value);

            OnDictionaryChange();
        }

        public void Add(KeyValuePair<TKey, TValue> entry) {
            Add(entry.Key, entry.Value);
        }

        public bool Remove(TKey key) {
            bool shouldRemove = ContainsKey(key);

            if (shouldRemove) {
                Data.Remove(key);

                OnDictionaryChange();
            }

            return shouldRemove;
        }

        public bool Remove(KeyValuePair<TKey, TValue> entry) {
            return Remove(entry.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index) {
            Data.CopyTo(array, index);
        }

        public void Clear() {
            Data.Clear();

            OnDictionaryChange();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Data.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>) Data).GetEnumerator();
        }
    }
}