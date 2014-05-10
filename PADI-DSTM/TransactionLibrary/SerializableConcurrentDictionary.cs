using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

public class SerializableConcurrentDictionary<TKey, TValue> : MarshalByRefObject, IDictionary<TKey, TValue>
{
    private ConcurrentDictionary<TKey, TValue> dictionary = new ConcurrentDictionary<TKey, TValue>();

    public void Add(TKey key, TValue value)
    {

    }

    public bool ContainsKey(TKey key)
    {
        return true;
    }

    public ICollection<TKey> Keys
    {
        get { return null; }
    }

    public bool Remove(TKey key)
    {
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        value = this.dictionary[key];
        return true;
    }

    public ICollection<TValue> Values
    {
        get { return null; }
    }

    public TValue this[TKey key]
    {
        get
        {
            return this.dictionary[key];
        }
        set
        {
            this.dictionary[key] = value;
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {

    }

    public void Clear()
    {

    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {

    }

    public int Count
    {
        get { return 1; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return true;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}