using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using static System.Extensions;

namespace System.Collections.Specialized
{
    /// <summary>
    /// Represents a generic collection of associated <see cref="string" /> keys
    /// and <typeparamref name="T"/> values that can be accessed either with the key or with the index.
    /// </summary>
    /// <typeparam name="T">
    /// Value type.
    /// </typeparam>
    [Serializable]
    [DebuggerDisplay("Count = {Count.ToString()}")]
    public class NameValueCollection<T> : NameObjectCollectionBase
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private string[] _keys;
        private T[] _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection{T}" /> class that is empty,
        /// has the default initial capacity and uses the default case-insensitive hash code provider
        /// and the default case-insensitive comparer.
        /// </summary>
        public NameValueCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection{T}" /> class that is empty,
        /// has the default initial capacity and uses the specified hash code provider and the specified comparer.
        /// </summary>
        /// <param name="hashProvider">
        /// The <see cref="T:System.Collections.IHashCodeProvider" />
        /// that will supply the hash codes for all keys in the <see cref="NameValueCollection{T}" />.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> to use to determine whether two keys are equal.
        /// </param>
        [Obsolete("Please use NameValueCollection(IEqualityComparer) instead.")]
        public NameValueCollection(IHashCodeProvider hashProvider, IComparer comparer)
          : base(hashProvider, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection{T}" /> class that is empty,
        /// has the specified initial capacity and uses the default case-insensitive hash code provider
        /// and the default case-insensitive comparer.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of entries that the <see cref="NameValueCollection{T}" /> can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public NameValueCollection(int capacity)
          : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection{T}" /> class that is empty,
        /// has the default initial capacity, and uses the specified <see cref="IEqualityComparer" /> object.
        /// </summary>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer" /> object to use
        /// to determine whether two keys are equal and to generate hash codes for the keys in the collection.
        /// </param>
        public NameValueCollection(IEqualityComparer equalityComparer)
          : base(equalityComparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection{T}" /> class that is empty,
        /// has the specified initial capacity, and uses the specified <see cref="IEqualityComparer" /> object.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of entries that the <see cref="NameValueCollection{T}" /> object can contain.
        /// </param>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer" /> object to use to determine whether two keys are equal
        /// and to generate hash codes for the keys in the collection.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public NameValueCollection(int capacity, IEqualityComparer equalityComparer)
          : base(capacity, equalityComparer)
        {
        }

        /// <summary>
        /// Copies the entries from the specified <see cref="NameValueCollection{T}" /> to a new <see cref="NameValueCollection{T}" />
        /// with the specified initial capacity or the same initial capacity as the number of entries copied, whichever is greater,
        /// and using the default case-insensitive hash code provider and the default case-insensitive comparer.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of entries that the <see cref="NameValueCollection{T}" /> can contain.
        /// </param>
        /// <param name="col">
        /// The <see cref="NameValueCollection{T}" /> to copy to the new <see cref="NameValueCollection{T}" /> instance.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="col" /> is <see langword="null" />.
        /// </exception>
        public NameValueCollection(int capacity, NameValueCollection<T> col)
          : base(capacity)
        {
            if (col == null)
                throw new ArgumentNullException(nameof(col));
            Add(col);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection{T}" /> class that is empty,
        /// has the specified initial capacity and uses the specified hash code provider and the specified comparer.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of entries that the <see cref="NameValueCollection{T}" /> can contain.
        /// </param>
        /// <param name="hashProvider">
        /// The <see cref="IHashCodeProvider" /> that will supply the hash codes
        /// for all keys in the <see cref="NameValueCollection{T}" />.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> to use to determine whether two keys are equal.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.</exception>
        [Obsolete("Please use NameValueCollection<T>(Int32, IEqualityComparer) instead.")]
        public NameValueCollection(int capacity, IHashCodeProvider hashProvider, IComparer comparer)
          : base(capacity, hashProvider, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection{T}" /> class that is serializable
        /// and uses the specified <see cref="SerializationInfo" /> and <see cref="StreamingContext" />.
        /// </summary>
        /// <param name="info">
        /// A <see cref="SerializationInfo" /> object that contains the information
        /// required to serialize the new <see cref="NameValueCollection{T}" /> instance.
        /// </param>
        /// <param name="context">A <see cref="StreamingContext" /> object that contains the source
        /// and destination of the serialized stream associated with the new <see cref="NameValueCollection{T}" /> instance.
        /// </param>
        protected NameValueCollection(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        /// <summary>
        /// Resets the cached arrays of the collection to <see langword="null" />.
        /// </summary>
        protected void InvalidateCachedArrays()
        {
            _values = (T[])null;
            _keys = (string[])null;
        }

        /// <summary>
        /// Copies the entries in the specified <see cref="NameValueCollection{T}" />
        /// to the current <see cref="NameValueCollection{T}" />.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="NameValueCollection{T}" /> to copy
        /// to the current <see cref="NameValueCollection{T}" />.
        /// </param>
        /// <exception cref="T:System.NotSupportedException">
        /// The collection is read-only.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="collection" /> is <see langword="null" />.
        /// </exception>
        public void Add(NameValueCollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            InvalidateCachedArrays();
            int count = collection.Count;
            for (int index1 = 0; index1 < count; ++index1)
            {
                string key = collection.GetKey(index1);
                T[] values = collection.GetValues(index1);
                if (values != null)
                {
                    for (int index2 = 0; index2 < values.Length; ++index2)
                        Add(key, values[index2]);
                }
                else
                    Add(key,default);
            }
        }

        /// <summary>
        /// Invalidates the cached arrays and removes all entries from the <see cref="NameValueCollection{T}" />.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The collection is read-only.
        /// </exception>
        public virtual void Clear()
        {
            if (IsReadOnly)
                throw new NotSupportedException(GetResourceString("CollectionReadOnly"));
            InvalidateCachedArrays();
            BaseClear();
        }

        /// <summary>
        /// Copies the entire <see cref="NameValueCollection{T}" />
        /// to a compatible one-dimensional <see cref="Array" />,
        /// starting at the specified index of the target array.</summary>
        /// <param name="dest">The one-dimensional <see cref="Array" />
        /// that is the destination of the elements copied from <see cref="NameValueCollection{T}" />.
        /// The <see cref="Array" /> must have zero-based indexing.</param>
        /// <param name="index">
        /// The zero-based index in <paramref name="dest" /> at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dest" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="dest" /> is multidimensional.
        /// -or-
        /// The number of elements in the source <see cref="NameValueCollection{T}" />
        /// is greater than the available space from <paramref name="index" />
        /// to the end of the destination <paramref name="dest" />.
        /// </exception>
        /// <exception cref="T:System.InvalidCastException">
        /// The type of the source <see cref="NameValueCollection{T}" />
        /// cannot be cast automatically to the type of the destination <paramref name="dest" />.
        /// </exception>
        public void CopyTo(T[] dest, int index)
        {
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));
            if (dest.Rank != 1)
                throw new ArgumentException(GetResourceString("Arg_MultiRank"));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), 
                    GetResourceString("IndexOutOfRange", (object)index.ToString((IFormatProvider)CultureInfo.CurrentCulture)));
            if (_values == null)
            {
                _values = GetAllValues().AsArray<T>();
            }
            int count = _values.Length;
            if (dest.Length - index < count)
                throw new ArgumentException(GetResourceString("Arg_InsufficientSpace"));
            for (int i = 0; i < count; i++)
                dest.SetValue(_values[i], i + index);
        }

        /// <summary>
        /// Returns an array that contains all the values in the <see cref="NameValueCollection{T}" /> instance.
        /// </summary>
        /// <returns>
        /// An array that contains all the values in the <see cref="NameValueCollection{T}" /> instance.
        /// </returns>
        protected ArrayList GetAllValues()
        {
            object[] values =  base.BaseGetAllValues();
            int count = Count;
            ArrayList arrayList = new ArrayList(count);
            for (int i = 0; i < count; ++i)
            {
                arrayList.AddRange(GetValues(i));
            }

            arrayList.AsArray<T>();
            return arrayList;
        }


        /// <summary>Gets a value indicating whether the <see cref="NameValueCollection{T}" />
        /// contains keys that are not <see langword="null" />.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="NameValueCollection{T}" />
        /// contains keys that are not <see langword="null" />; otherwise, <see langword="false" />.
        /// </returns>
        public bool HasKeys()
        {
            return BaseHasKeys();
        }

        /// <summary>Adds an entry with the specified name and value to the <see cref="NameValueCollection{T}" />.</summary>
        /// <param name="name">The <see cref="string" /> key of the entry to add. The key can be <see langword="null" />.</param>
        /// <param name="value">The value of the entry to add. The value can be <see langword="null" />.</param>
        /// <exception cref="NotSupportedException">The collection is read-only. </exception>
        public virtual void Add(string name, T value)
        {
            if (IsReadOnly)
                throw new NotSupportedException(GetResourceString("CollectionReadOnly"));
            InvalidateCachedArrays();
            ArrayList arrayList1 = (ArrayList)BaseGet(name);
            if (arrayList1 == null)
            {
                ArrayList arrayList2 = new ArrayList(1);
                if (value != null)
                    arrayList2.Add((object)value);
                BaseAdd(name, (object)arrayList2);
            }
            else
            {
                if (value == null)
                    return;
                arrayList1.Add((object)value);
            }
        }

        /// <summary>
        /// Gets the values associated with the specified key from the <see cref="NameValueCollection{T}" />.
        /// </summary>
        /// <param name="name">
        /// The <see cref="string"/> key of the entry that contains the values to get. The key can be <see langword="null" />.
        /// </param>
        /// <returns>
        /// An array that contains the values associated with the specified key from the <see cref="NameValueCollection{T}" />,
        /// if found; otherwise, <see langword="null" />.
        /// </returns>
        public virtual T[] GetValues(string name)
        {
            return ((ArrayList) BaseGet(name)).AsArray<T>();
        }

        /// <summary>Sets the value of an entry in the <see cref="NameValueCollection{T}" />.
        /// </summary>
        /// <param name="name">The <see cref="string" /> key of the entry to add the new value to.
        /// The key can be <see langword="null" />.</param>
        /// <param name="value">
        /// The <see cref="object" /> that represents the new value to add to the specified entry.
        /// The value can be <see langword="null" />.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The collection is read-only.
        /// </exception>
        public virtual void Set(string name, T value)
        {
            if (IsReadOnly)
                throw new NotSupportedException(GetResourceString("CollectionReadOnly"));
            InvalidateCachedArrays();
            BaseSet(name, (object) new ArrayList(1)
            {
                (object) value
            });
        }

        /// <summary>
        /// Removes the entries with the specified key from the <see cref="NameValueCollection{T}" /> instance.
        /// </summary>
        /// <param name="name">
        /// The <see cref="string" /> key of the entry to remove. The key can be <see langword="null" />.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The collection is read-only.
        /// </exception>
        public virtual void Remove(string name)
        {
            InvalidateCachedArrays();
            BaseRemove(name);
        }

        /// <summary>
        /// Gets the values at the specified index of the <see cref="NameValueCollection{T}" />.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the entry that contains the values to get from the collection.
        /// </param>
        /// <returns>
        /// An array that contains the values at the specified index of the <see cref="NameValueCollection{T}" />,
        /// if found; otherwise, <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the valid range of indexes for the collection.
        /// </exception>
        public virtual T[] GetValues(int index)
        {
            return ((ArrayList) BaseGet(index)).AsArray<T>();
        }

        /// <summary>
        /// Gets the key at the specified index of the <see cref="NameValueCollection{T}" />.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the key to get from the collection.
        /// </param>
        /// <returns>
        /// A <see cref="string" /> that contains the key at the specified index of the <see cref="NameValueCollection{T}" />,
        /// if found; otherwise, <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the valid range of indexes for the collection.
        /// </exception>
        public virtual string GetKey(int index)
        {
            return BaseGetKey(index);
        }

        /// <summary>
        /// Gets the entry at the specified index of the <see cref="NameValueCollection{T}" />.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the entry to locate in the collection.
        /// </param>
        /// <returns>
        /// A <see cref="string" /> that contains the comma-separated list of values at the specified index of the collection.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the valid range of indexes for the collection.
        /// </exception>
        public T[] this[int index]
        {
            get
            {
                return GetValues(index);
            }
        }

        /// <summary>
        /// Gets all the keys in the <see cref="NameValueCollection{T}" />.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> array that contains all the keys of the <see cref="NameValueCollection{T}" />.
        /// </returns>
        public virtual string[] AllKeys
        {
            get
            {
                if (_keys == null)
                    _keys = BaseGetAllKeys();
                return _keys;
            }
        }

        /// <summary>
        /// Gets all the values in the <see cref="NameValueCollection{T}" />.
        /// </summary>
        /// <returns>
        /// An array that contains all the values of the <see cref="NameValueCollection{T}" />.
        /// </returns>
        public virtual string[] Values
        {
            get
            {
                if (_values == null)
                    _values = GetAllValues().AsArray<T>();
                return _keys;
            }
        }

        /// <summary>
        /// Gets collection of <see cref="DictionaryEntry"/> entries that contains
        /// all keys and values pairs in the <see cref="NameValueCollection{T}" />.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="DictionaryEntry"/> that contains
        /// all the entries of the <see cref="NameValueCollection{T}" />.
        /// </returns>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        protected IEnumerable<DictionaryEntry> Entries
        {
            get
            {
                foreach (string key in AllKeys)
                {
                    foreach (T value in GetValues(key))
                    {
                        yield return new DictionaryEntry(key, value);
                    }
                }
            }
        }
    }
}

namespace System
{
    internal static class Extensions
    {
        private static ResourceSet _mscorlib = null;

        internal static string GetResourceString(string name) // gets mscorlib internal error message.
        {
            if (_mscorlib == null)
            {
                var a = Assembly.GetAssembly(typeof(object));
                var n = a.GetName().Name;
                var m = new ResourceManager(n, a);
                _mscorlib = m.GetResourceSet(CultureInfo.CurrentUICulture, false, true);
            }

            return _mscorlib.GetString(name);
        }

        internal static string GetResourceString(string name, params object[] args)
        {
            return string.Format(GetResourceString(name) ?? throw new ArgumentNullException(nameof(name)), args);
        }

        internal static T[] AsArray<T>(this ArrayList list)
        {
            int count = 0;
            if (list == null || (count = list.Count) == 0)
                return (T[])null;
            T[] array = new T[count];
            list.CopyTo(0, array, 0, count);
            return array;
        }
    }
}
