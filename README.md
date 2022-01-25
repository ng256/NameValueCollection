# NameValueCollection
Represents a generic implementation of __NameValueCollection__ - a generic collection of associated string keys and given type values that can be accessed either with the key or with the index.  
It is a collection that is similar to a __Dictionary__ but __NameValueCollection__ can have duplicate keys while __Dictionary__ cannot. Elements can be obtained both by index and by key.  
What makes this collection special, is that one key can contain several elements. 





## Introduction

__NameValueCollection<T>__ - a generic collection of associated string keys and given type values that can be accessed either with the key or with the index.
It is a collection that is similar to a __Dictionary<string, string>__ but __NameValueCollection__ can have duplicate keys while Dictionary cannot. Elements can be obtained both by index and by key.
What makes this collection special, is that one key can contain several elements and that null is allowed as a key or as a value. Built into mscorlib realization of __NameValueCollection__ assumes that strings are used as both keys and values. But what if we want to store values of any type. Of course, you can convert to the desired type every time you get a value, but there are two significant limitations here:

- processing overhead for conversion;
- not all types support conversion to and from a string;
- references to the original objects are not preserved.

The need to store objects in a collection in the original type attracted me to write a generic form  __NameValueCollection<T>__ as a modification of __NameValueCollection__.
##  Background

The __NameValueCollection<T>__ collection is based on __NameObjectCollectionBase__ - the base class for a collection of associated String keys and Object values that contains base methods to accessing the values. The interfaces __IDictionary__, __IEnumerable<KeyValuePair<string, T>>__ were implemented in the class as additional usability.
### The class definition

For the first time, define the class and his members that contains keys and values.

```csharp
  public class NameValueCollection<T> :
  NameObjectCollectionBase,
  IDictionary,
  IEnumerable<KeyValuePair<string, T>>
{
    private string[] _keys;       // Cached keys.
    private T[] _values;          // Cached values.

    // Resets the cache.
    protected void InvalidateCachedArrays()
    {
        _values = null;
        _keys = null;
    }

    // Gets all values cache.
    protected ArrayList GetAllValues()
    {
        int count = Count;
        ArrayList arrayList = new ArrayList(count);
        for (int i = 0; i < count; ++i)
        {
            arrayList.AddRange(Get(i));
        }
        arrayList.AsArray<T>();
        return arrayList;
    }

    // Enumerates all entries.
    protected IEnumerable<KeyValuePair<string, T>> GetAllEntries()
    {
        foreach (string key in Keys)
        {
            foreach (T value in Get(key))
            {
                yield return new KeyValuePair<string, T>(key, value);
            }
        }
    }

    // Converts ArrayLit to Array of T elements.
    protected static T[] AsArray<T>(ArrayList list)
    {
        int count = 0;
        if (list == null || (count = list.Count) == 0)
            return (T[])null;
        T[] array = new T[count];
        list.CopyTo(0, array, 0, count);
        return array;
    }
}
```

Private fields will contain the cached data in specified arrays. The __InvalidateCachedArrays__ method will reset the caches and will be called every time the data changes, __GetAllValues__ returns all values regardless of keys they are paired with, _GetAllEntries__ return all key-value array pairs. These methods will be useful to us in the future. __The AsArray<T>__ method just converts __ArrayList__ to __Array__ of elements of type __T__.
### Base methods

The next step, it's time to add in the class methods that can get, set and remove data in collection using base class methods.

The __Add__ and __Set__ methods put received values into the array that paired with the spcified key.

```csharp
// Adds single value to collection.
public void Add(string name, T value)
{
    InvalidateCachedArrays();
    ArrayList arrayList = (ArrayList)BaseGet(name);
    if (arrayList == null)
    {
        arrayList = new ArrayList(1);
        if (value != null) arrayList.Add(value);
        BaseAdd(name, arrayList);
    }
    else
    {
        if (value == null) return;
        arrayList.Add(value);
    }
}

// Adds range of values to collection.
public void Add(NameValueCollection<T> collection)
{
    InvalidateCachedArrays();
    int count = collection.Count;
    for (int i = 0; i < count; i++)
    {
        string key = collection.GetKey(i);
        T[] values = collection.Get(i);
        foreach (var value in values)
        {
            Add(key, value);
        }
    }
}

// Set single value (prevoious values will be removed).
public void Set(string name, T value)
{
    InvalidateCachedArrays();
    BaseSet(name, new ArrayList(1) {value});
}

// Set range of values (prevoious values will be removed).
public void Set(string name, params T[] values)
{
    InvalidateCachedArrays();
    BaseSet(name, new ArrayList(values));
}
```

The __GetKey__ and __Get__ methods return the requested key and array of associated with key values.

```csharp
// Gets all values that paired with specified key.
public T[] Get(string name)
{
    return AsArray<T>((ArrayList)BaseGet(name));
}

// Gets all values at the specified index of collection.
public T[] Get(int index)
{
    return AsArray<T>((ArrayList)BaseGet(index));
}

// Gets string containing the key at the specified index.
public string GetKey(int index)
{
    return BaseGetKey(index);
}
```
  
The __Clear__ and __Remove__ methods delete values from the collection.

```csharp
// Removes values from the specified key.
public void Remove(string name)
{
    InvalidateCachedArrays();
    BaseRemove(name);
}

// Removes all data from the collection.
public void Clear()
{
    if (IsReadOnly)
        throw new NotSupportedException(GetResourceString("CollectionReadOnly"));
    InvalidateCachedArrays();
    BaseClear();
}
```
  
### Properties

Almost done. For ease of use of the collection, it's a good idea to add properties. The __Keys__ and __Values__ properties attempt to return cached data and update it if the cache is invalidated.

```csharp
// All keys that the current collection contains.
public string[] Keys
{
    get
    {
        if (_keys == null)
            _keys = BaseGetAllKeys();
        return _keys;
    }
}
  
// All values that the current collection contains.
public T[] Values
{
    get
    {
        if (_values == null)
            _values = AsArray<T>(GetAllValues());
        return _values;
    }
}

// Values at the specefied index.
public T[] this[int index]
{
    get
    {
        return Get(index);
    }
    set
    {
        BaseSet(index, new ArrayList(value));
    }
}

// Values at the specefied key.
public T[] this[string name]
{
    get
    {
        return Get(name);
    }
    set
    {
        BaseSet(name, new ArrayList(value));
    }
}
```
  
### Collection enumeration

The embeded class __Enumerator__ will be responsible for enumareting all key-value pairs in the collection.

```csharp
private class Enumerator : IEnumerator<KeyValuePair<string, T>>, IDictionaryEnumerator
{
    private IEnumerable<KeyValuePair<string, T>> Entries { get; }
    private readonly IEnumerator<KeyValuePair<string, T>> _enumerator;

    public Enumerator(NameValueCollection<T> collection)
    {
        IEnumerable<KeyValuePair<string, T>> entries = collection.GetAllEntries().AsArray();
        Entries = entries;
        _enumerator = entries.GetEnumerator();
    }

    KeyValuePair<string, T> IEnumerator<KeyValuePair<string, T>>.Current
    {
        get
        {
            return _enumerator.Current;
        }
    }

    object IEnumerator.Current
    {
        get
        {
            IEnumerator enumerator = ((IEnumerator) _enumerator);
            return enumerator.Current;
        }
    }

    object IDictionaryEnumerator.Key
    {
        get
        {
            IEnumerator<KeyValuePair<string, T>> enumerator = 
                  ((IEnumerator<KeyValuePair<string, T>>) this);
            return enumerator.Current.Key;
        }
    }

    object IDictionaryEnumerator.Value
    {
        get
        {
            IEnumerator<KeyValuePair<string, T>> enumerator = 
                 ((IEnumerator<KeyValuePair<string, T>>) this);
            return enumerator.Current.Value;
        }
    }

    DictionaryEntry IDictionaryEnumerator.Entry
    {
        get
        {
            IEnumerator<KeyValuePair<string, T>> enumerator = 
                ((IEnumerator<KeyValuePair<string, T>>) this);
            return new DictionaryEntry(enumerator.Current.Key,
                enumerator.Current.Value);
        }
    }

    public bool MoveNext()
    {
        return _enumerator.MoveNext();
    }

    public void Reset()
    {
        _enumerator.Reset();
    }

    public void Dispose()
    {
    }
}
```
  
The last step is to implement the specified interfaces. Some methods and properties are implemented explicitly, that is, their use requires a prior casting to the appropriate its interface's type.

```csharp
ICollection IDictionary.Values => Values;
public new bool IsReadOnly => base.IsReadOnly;
public bool IsFixedSize => false;
ICollection IDictionary.Keys => Keys;

bool IDictionary.Contains(object key)
{
    return key is string s && Keys.Contains(s);
}

void IDictionary.Add(object key, object value)
{
    Add((string)key, (T)value);
}

IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
{
    return new Enumerator(this);
}

public override IEnumerator GetEnumerator()
{
    return new Enumerator(this);
}

IDictionaryEnumerator IDictionary.GetEnumerator()
{
    return new Enumerator(this);
}

void IDictionary.Remove(object key)
{
    Remove((string)key);
}

object IDictionary.this[object key]
{
    get
    {
        return Get((string)key);
    }
    set
    {
        if (value is IEnumerable<T> collection)
        {
            Set((string)key, (T[])collection.ToArray());
        }
        else
        {
            Set((string)key, (T)value);
        }
    }
}
```
  
## Usage

```csharp
NameValueCollection<int> collection = new NameValueCollection<int>();
collection.Add("a", 123);
collection.Add("a", 456); // 123 and 456 will be inserted into the same key. 
collection.Add("b", 789); // 789 will be inserted into another key.

int[] a = collection.Get("a"); // contains 132 and 456.
int[] b = collection.Get("b"); // contains 789.
```
