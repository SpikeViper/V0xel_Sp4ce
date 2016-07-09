 // ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/*============================================================
**
** Class:  ResizeableArray
** 
** <OWNER>[....]</OWNER>
**
** Purpose: Implements a generic, dynamically sized ResizeableArray as an 
**          array.
**
** 
===========================================================*/
namespace System.Collections.Generic
{

    using System;
    using System.Runtime;
    using System.Runtime.Versioning;
    using System.Diagnostics;
    using System.Collections.ObjectModel;
    using System.Security.Permissions;
    using UnityEngine;

    // Implements a variable-size ResizeableArray that uses an array of objects to store the
    // elements. A ResizeableArray has a capacity, which is the allocated length
    // of the internal array. As elements are added to a ResizeableArray, the capacity
    // of the ResizeableArray is automatically increased as required by reallocating the
    // internal array.
    // 
    [Serializable]
    public class ResizeableArray<T>
    {
        private const int _defaultCapacity = 0;

        public T[] _items;
        public T[] newItems;
        public int c;
        private int _size;
        private int _version;

        [NonSerialized]
        private System.Object _syncRoot;

        static readonly T[] _emptyArray = new T[0];

        // Constructs a ResizeableArray. The ResizeableArray is initially empty and has a capacity
        // of zero. Upon adding the first element to the ResizeableArray the capacity is
        // increased to 16, and then increased in multiples of two as required. LIES I CHANGED THIS HAHAHAHHAHAHAHHA
        public ResizeableArray()
        {
            _items = _emptyArray;
        }

        // Constructs a ResizeableArray with a given initial capacity. The ResizeableArray is
        // initially empty, but will have room for the given number of elements
        // before any reallocations are required.
        // 
        public ResizeableArray(int capacity)
        {
            if (capacity == 0)
                _items = _emptyArray;
            else
                _items = new T[capacity];
        }

        // Constructs a ResizeableArray, copying the contents of the given collection. The
        // size and capacity of the new ResizeableArray will both be equal to the size of the
        // given collection.
        // 
        public ResizeableArray(IEnumerable<T> collection)
        {

            ICollection<T> c = collection as ICollection<T>;
            if (c != null)
            {
                int count = c.Count;
                if (count == 0)
                {
                    _items = _emptyArray;
                }
                else
                {
                    _items = new T[count];
                    c.CopyTo(_items, 0);
                    _size = count;
                }
            }
            else
            {
                _size = 0;
                _items = _emptyArray;
                // This enumerable could be empty.  Let Add allocate a new array, if needed.
                // Note it will also go to _defaultCapacity first, not 1, then 2, etc.

                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Add(en.Current);
                    }
                }
            }
        }

        // Gets and sets the capacity of this ResizeableArray.  The capacity is the size of
        // the internal array used to hold items.  When set, the internal 
        // array of the ResizeableArray is reallocated to the given capacity.
        // 
        public int Capacity
        {
            get
            {
                return _items.Length;
            }
            set
            {

                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        newItems = new T[value];
                        if (_size > 0)
                        {

                            for (int i = 0; i < _size; i++)
                            {
                                
                                newItems[i] = _items[i];
    
                            }

                        }
                        _items = newItems;
                    }
                    else
                    {
                        _items = _emptyArray;
                    }
                }
            }
        }

        // Read-only property describing how many elements are in the ResizeableArray.
        public int Count
        {
            get
            {
                return _size;
            }
        }

        bool IsFixedSize
        {
            get { return false; }
        }

        bool IsReadOnly
        {
            get { return false; }
        }

        // Is this ResizeableArray synchronized (thread-safe)?
        bool IsSynchronized
        {
            get { return false; }
        }

        // Synchronization root for this object.
        System.Object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<System.Object>(ref _syncRoot, new System.Object(), null);
                }
                return _syncRoot;
            }
        }
        // Sets or Gets the element at the given index.
        // 
        public T this[int index]
        {
            get
            {
                return _items[index];
            }

            set
            {
                _items[index] = value;
                _version++;
            }
        }

        private static bool IsCompatibleObject(object value)
        {
            // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>. 
            return ((value is T) || (value == null && default(T) == null));
        }


        // Adds the given object to the end of this ResizeableArray. The size of the ResizeableArray is
        // increased by one. If required, the capacity of the ResizeableArray is doubled
        // before adding the new element.
        //
        public void Add(T item)
        {
            if (_size == _items.Length) EnsureCapacity(_size + 1);
            _items[_size++] = item;
            _version++;
        }

        int Add(System.Object item)
        {


            try
            {
                Add((T)item);
            }
            catch (InvalidCastException)
            {

            }

            return Count - 1;
        }


        // Adds the elements of the given collection to the end of this ResizeableArray. If
        // required, the capacity of the ResizeableArray is increased to twice the previous
        // capacity or the new size, whichever is larger.
        //
        public void AddRange(IEnumerable<T> collection)
        {

            InsertRange(_size, collection);
        }


        // Searches a section of the ResizeableArray for a given element using a binary search
        // algorithm. Elements of the ResizeableArray are compared to the search value using
        // the given IComparer interface. If comparer is null, elements of
        // the ResizeableArray are compared to the search value using the IComparable
        // interface, which in that case must be implemented by all elements of the
        // ResizeableArray and the given search value. This method assumes that the given
        // section of the ResizeableArray is already sorted; if this is not the case, the
        // result will be incorrect.
        //
        // The method returns the index of the given value in the ResizeableArray. If the
        // ResizeableArray does not contain the given value, the method returns a negative
        // integer. The bitwise complement operator (~) can be applied to a
        // negative result to produce the index of the first element (if any) that
        // is larger than the given search value. This is also the index at which
        // the search value should be inserted into the ResizeableArray in order for the ResizeableArray
        // to remain sorted.
        // 
        // The method uses the Array.BinarySearch method to perform the
        // search.
        // 
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return Array.BinarySearch<T>(_items, index, count, item, comparer);
        }

        public int BinarySearch(T item)
        {
            return BinarySearch(0, Count, item, null);
        }

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return BinarySearch(0, Count, item, comparer);
        }


        // Clears the contents of ResizeableArray.
        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
                _size = 0;
            }
            _version++;
        }

        // Contains returns true if the specified element is in the ResizeableArray.
        // It does a linear, O(n) search.  Equality is determined by calling
        // item.Equals().
        //
        public bool Contains(T item)
        {
            if ((System.Object)item == null)
            {
                for (int i = 0; i < _size; i++)
                    if ((System.Object)_items[i] == null)
                        return true;
                return false;
            }
            else
            {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                for (int i = 0; i < _size; i++)
                {
                    if (c.Equals(_items[i], item)) return true;
                }
                return false;
            }
        }

        bool Contains(System.Object item)
        {
            if (IsCompatibleObject(item))
            {
                return Contains((T)item);
            }
            return false;
        }

        // Copies this ResizeableArray into array, which must be of a 
        // compatible array type.  
        //
        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        // Copies this ResizeableArray into array, which must be of a 
        // compatible array type.  
        //
        void CopyTo(Array array, int arrayIndex)
        {

            try
            {
                // Array.Copy will check for NULL.
                Array.Copy(_items, 0, array, arrayIndex, _size);
            }
            catch (ArrayTypeMismatchException)
            {

            }
        }

        // Copies a section of this ResizeableArray to the given array at the given index.
        // 
        // The method uses the Array.Copy method to copy the elements.
        // 
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            // Delegate rest of error checking to Array.Copy.
            Array.Copy(_items, index, array, arrayIndex, count);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            // Delegate rest of error checking to Array.Copy.
            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        // Ensures that the capacity of this ResizeableArray is at least the given minimum
        // value. If the currect capacity of the ResizeableArray is less than min, the
        // capacity is increased to twice the current capacity or to min,
        // whichever is larger.
        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int newCapacity = _items.Length == 0 ? _defaultCapacity : _items.Length * 2;
                // Allow the ResizeableArray to grow to maximum possible capacity (~2G elements) before encountering overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                if (newCapacity < min) newCapacity = min;
                Capacity = newCapacity;
            }
        }

        public bool Exists(Predicate<T> match)
        {
            return FindIndex(match) != -1;
        }

        public T Find(Predicate<T> match)
        {

            for (int i = 0; i < _size; i++)
            {
                if (match(_items[i]))
                {
                    return _items[i];
                }
            }
            return default(T);
        }

        public ResizeableArray<T> FindAll(Predicate<T> match)
        {

            ResizeableArray<T> ResizeableArray = new ResizeableArray<T>();
            for (int i = 0; i < _size; i++)
            {
                if (match(_items[i]))
                {
                    ResizeableArray.Add(_items[i]);
                }
            }
            return ResizeableArray;
        }

        public int FindIndex(Predicate<T> match)
        {
            return FindIndex(0, _size, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return FindIndex(startIndex, _size - startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {

            int endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (match(_items[i])) return i;
            }
            return -1;
        }

        public T FindLast(Predicate<T> match)
        {

            for (int i = _size - 1; i >= 0; i--)
            {
                if (match(_items[i]))
                {
                    return _items[i];
                }
            }
            return default(T);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return FindLastIndex(_size - 1, _size, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return FindLastIndex(startIndex, startIndex + 1, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {

            int endIndex = startIndex - count;
            for (int i = startIndex; i > endIndex; i--)
            {
                if (match(_items[i]))
                {
                    return i;
                }
            }
            return -1;
        }


        // Returns an enumerator for this ResizeableArray with the given
        // permission for removal of elements. If modifications made to the ResizeableArray 
        // while an enumeration is in progress, the MoveNext and 
        // GetObject methods of the enumerator will throw an exception.
        //
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }


        // Returns the index of the first occurrence of a given value in a range of
        // this ResizeableArray. The ResizeableArray is searched forwards from beginning to end.
        // The elements of the ResizeableArray are compared to the given value using the
        // Object.Equals method.
        // 
        // This method uses the Array.IndexOf method to perform the
        // search.
        // 
        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _size);
        }

        int IndexOf(System.Object item)
        {
            if (IsCompatibleObject(item))
            {
                return IndexOf((T)item);
            }
            return -1;
        }

        // Returns the index of the first occurrence of a given value in a range of
        // this ResizeableArray. The ResizeableArray is searched forwards, starting at index
        // index and ending at count number of elements. The
        // elements of the ResizeableArray are compared to the given value using the
        // Object.Equals method.
        // 
        // This method uses the Array.IndexOf method to perform the
        // search.
        // 
        public int IndexOf(T item, int index)
        {
            return Array.IndexOf(_items, item, index, _size - index);
        }

        // Returns the index of the first occurrence of a given value in a range of
        // this ResizeableArray. The ResizeableArray is searched forwards, starting at index
        // index and upto count number of elements. The
        // elements of the ResizeableArray are compared to the given value using the
        // Object.Equals method.
        // 
        // This method uses the Array.IndexOf method to perform the
        // search.
        // 
        public int IndexOf(T item, int index, int count)
        {
            return Array.IndexOf(_items, item, index, count);
        }

        // Inserts an element into this ResizeableArray at a given index. The size of the ResizeableArray
        // is increased by one. If required, the capacity of the ResizeableArray is doubled
        // before inserting the new element.
        // 
        public void Insert(int index, T item)
        {
            if (_size == _items.Length) EnsureCapacity(_size + 1);
            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }
            _items[index] = item;
            _size++;
            _version++;
        }

        void Insert(int index, System.Object item)
        {

            try
            {
                Insert(index, (T)item);
            }
            catch (InvalidCastException)
            {

            }
        }

        // Inserts the elements of the given collection at a given index. If
        // required, the capacity of the ResizeableArray is increased to twice the previous
        // capacity or the new size, whichever is larger.  Ranges may be added
        // to the end of the ResizeableArray by setting index to the ResizeableArray's size.
        //
        public void InsertRange(int index, IEnumerable<T> collection)
        {

            ICollection<T> c = collection as ICollection<T>;
            if (c != null)
            {    // if collection is ICollection<T>
                int count = c.Count;
                if (count > 0)
                {
                    EnsureCapacity(_size + count);
                    if (index < _size)
                    {
                        Array.Copy(_items, index, _items, index + count, _size - index);
                    }

                    // If we're inserting a ResizeableArray into itself, we want to be able to deal with that.
                    if (this == c)
                    {
                        // Copy first part of _items to insert location
                        Array.Copy(_items, 0, _items, index, index);
                        // Copy last part of _items back to inserted location
                        Array.Copy(_items, index + count, _items, index * 2, _size - index);
                    }
                    else
                    {
                        T[] itemsToInsert = new T[count];
                        c.CopyTo(itemsToInsert, 0);
                        itemsToInsert.CopyTo(_items, index);
                    }
                    _size += count;
                }
            }
            else
            {
                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Insert(index++, en.Current);
                    }
                }
            }
            _version++;
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this ResizeableArray. The ResizeableArray is searched backwards, starting at the end 
        // and ending at the first element in the ResizeableArray. The elements of the ResizeableArray 
        // are compared to the given value using the Object.Equals method.
        // 
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        // 
        public int LastIndexOf(T item)
        {

            if (_size == 0)
            {  // Special case for empty ResizeableArray
                return -1;
            }
            else
            {
                return LastIndexOf(item, _size - 1, _size);
            }
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this ResizeableArray. The ResizeableArray is searched backwards, starting at index
        // index and ending at the first element in the ResizeableArray. The 
        // elements of the ResizeableArray are compared to the given value using the 
        // Object.Equals method.
        // 
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        // 
        public int LastIndexOf(T item, int index)
        {
            return LastIndexOf(item, index, index + 1);
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this ResizeableArray. The ResizeableArray is searched backwards, starting at index
        // index and upto count elements. The elements of
        // the ResizeableArray are compared to the given value using the Object.Equals
        // method.
        // 
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        // 
        public int LastIndexOf(T item, int index, int count)
        {

            if (_size == 0)
            {  // Special case for empty ResizeableArray
                return -1;
            }

            return Array.LastIndexOf(_items, item, index, count);
        }

        // Removes the element at the given index. The size of the ResizeableArray is
        // decreased by one.
        // 
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        void Remove(System.Object item)
        {
            if (IsCompatibleObject(item))
            {
                Remove((T)item);
            }
        }

        // This method removes all items which matches the predicate.
        // The complexity is O(n).   
        public int RemoveAll(Predicate<T> match)
        {

            int freeIndex = 0;   // the first free slot in items array

            // Find the first item which needs to be removed.
            while (freeIndex < _size && !match(_items[freeIndex])) freeIndex++;
            if (freeIndex >= _size) return 0;

            int current = freeIndex + 1;
            while (current < _size)
            {
                // Find the first item which needs to be kept.
                while (current < _size && match(_items[current])) current++;

                if (current < _size)
                {
                    // copy item to the free slot.
                    _items[freeIndex++] = _items[current++];
                }
            }

            Array.Clear(_items, freeIndex, _size - freeIndex);
            int result = _size - freeIndex;
            _size = freeIndex;
            _version++;
            return result;
        }

        // Removes the element at the given index. The size of the ResizeableArray is
        // decreased by one.
        // 
        public void RemoveAt(int index)
        {
            _size--;
            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
            _items[_size] = default(T);
            _version++;
        }

        // Removes a range of elements from this ResizeableArray.
        // 
        public void RemoveRange(int index, int count)
        {
            if (count > 0)
            {
                int i = _size;
                _size -= count;
                if (index < _size)
                {
                    Array.Copy(_items, index + count, _items, index, _size - index);
                }
                Array.Clear(_items, _size, count);
                _version++;
            }
        }

        // Reverses the elements in this ResizeableArray.
        public void Reverse()
        {
            Reverse(0, Count);
        }

        // Reverses the elements in a range of this ResizeableArray. Following a call to this
        // method, an element in the range given by index and count
        // which was previously located at index i will now be located at
        // index index + (index + count - i - 1).
        // 
        // This method uses the Array.Reverse method to reverse the
        // elements.
        // 
        public void Reverse(int index, int count)
        {
            Array.Reverse(_items, index, count);
            _version++;
        }

        // Sorts the elements in this ResizeableArray.  Uses the default comparer and 
        // Array.Sort.
        public void Sort()
        {
            Sort(0, Count, null);
        }

        // Sorts the elements in this ResizeableArray.  Uses Array.Sort with the
        // provided comparer.
        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        // Sorts the elements in a section of this ResizeableArray. The sort compares the
        // elements to each other using the given IComparer interface. If
        // comparer is null, the elements are compared to each other using
        // the IComparable interface, which in that case must be implemented by all
        // elements of the ResizeableArray.
        // 
        // This method uses the Array.Sort method to sort the elements.
        // 
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            Array.Sort<T>(_items, index, count, comparer);
            _version++;
        }

        // ToArray returns a new Object array containing the contents of the ResizeableArray.
        // This requires copying the ResizeableArray, which is an O(n) operation.
        public T[] ToArray()
        {
            T[] array = new T[_size];
            Array.Copy(_items, 0, array, 0, _size);
            return array;
        }

        // Sets the capacity of this ResizeableArray to the size of the ResizeableArray. This method can
        // be used to minimize a ResizeableArray's memory overhead once it is known that no
        // new elements will be added to the ResizeableArray. To completely clear a ResizeableArray and
        // release all memory referenced by the ResizeableArray, execute the following
        // statements:
        // 
        // ResizeableArray.Clear();
        // ResizeableArray.TrimExcess();
        // 
        public void TrimExcess()
        {
            int threshold = (int)(((double)_items.Length));
            if (_size < threshold)
            {
                Capacity = _size;
            }
        }

        public bool TrueForAll(Predicate<T> match)
        {

            for (int i = 0; i < _size; i++)
            {
                if (!match(_items[i]))
                {
                    return false;
                }
            }
            return true;
        }


        [Serializable()]
        internal class SynchronizedResizeableArray
        {
            private ResizeableArray<T> _ResizeableArray;
            private System.Object _root;

            internal SynchronizedResizeableArray(ResizeableArray<T> ResizeableArray)
            {
                _ResizeableArray = ResizeableArray;
                _root = ((System.Collections.ICollection)ResizeableArray).SyncRoot;
            }

            public int Count
            {
                get
                {
                    lock (_root)
                    {
                        return _ResizeableArray.Count;
                    }
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return ((ICollection<T>)_ResizeableArray).IsReadOnly;
                }
            }

            public void Add(T item)
            {
                lock (_root)
                {
                    _ResizeableArray.Add(item);
                }
            }

            public void Clear()
            {
                lock (_root)
                {
                    _ResizeableArray.Clear();
                }
            }

            public bool Contains(T item)
            {
                lock (_root)
                {
                    return _ResizeableArray.Contains(item);
                }
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                lock (_root)
                {
                    _ResizeableArray.CopyTo(array, arrayIndex);
                }
            }

            public bool Remove(T item)
            {
                lock (_root)
                {
                    return _ResizeableArray.Remove(item);
                }
            }

            public T this[int index]
            {
                get
                {
                    lock (_root)
                    {
                        return _ResizeableArray[index];
                    }
                }
                set
                {
                    lock (_root)
                    {
                        _ResizeableArray[index] = value;
                    }
                }
            }

            public int IndexOf(T item)
            {
                lock (_root)
                {
                    return _ResizeableArray.IndexOf(item);
                }
            }

            public void Insert(int index, T item)
            {
                lock (_root)
                {
                    _ResizeableArray.Insert(index, item);
                }
            }

            public void RemoveAt(int index)
            {
                lock (_root)
                {
                    _ResizeableArray.RemoveAt(index);
                }
            }
        }

        [Serializable]
        public struct Enumerator : IEnumerator<T>, System.Collections.IEnumerator
        {
            private ResizeableArray<T> ResizeableArray;
            private int index;
            private int version;
            private T current;

            internal Enumerator(ResizeableArray<T> ResizeableArray)
            {
                this.ResizeableArray = ResizeableArray;
                index = 0;
                version = ResizeableArray._version;
                current = default(T);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {

                ResizeableArray<T> localResizeableArray = ResizeableArray;

                if (version == localResizeableArray._version && ((uint)index < (uint)localResizeableArray._size))
                {
                    current = localResizeableArray._items[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {

                index = ResizeableArray._size + 1;
                current = default(T);
                return false;
            }

            public T Current
            {
                get
                {
                    return current;
                }
            }

            System.Object System.Collections.IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {

                index = 0;
                current = default(T);
            }

        }
    }
}

