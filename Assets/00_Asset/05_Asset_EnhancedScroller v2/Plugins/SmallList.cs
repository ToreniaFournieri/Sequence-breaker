using UnityEngine;

namespace _00_Asset._05_Asset_EnhancedScroller_v2.Plugins
{
    /// <summary>
    /// This is a super light implementation of an array that 
    /// behaves like a list, automatically allocating new memory
    /// when needed, but not releasing it to garbage collection.
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    public class SmallList<T>
    {
        /// <summary>
        /// internal storage of list data
        /// </summary>
        public T[] Data;

        /// <summary>
        /// The number of elements in the list
        /// </summary>
        public int Count = 0;

        /// <summary>
        /// Indexed access to the list items
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i]
        {
            get { return Data[i]; }
            set { Data[i] = value; }
        }

        /// <summary>
        /// Resizes the array when more memory is needed.
        /// </summary>
        private void ResizeArray()
        {
            T[] newData;

            if (Data != null)
                newData = new T[Mathf.Max(Data.Length << 1, 64)];
            else
                newData = new T[64];

            if (Data != null && Count > 0)
                Data.CopyTo(newData, 0);

            Data = newData;
        }

        /// <summary>
        /// Instead of releasing the memory to garbage collection, 
        /// the list size is set back to zero
        /// </summary>
        public void Clear()
        {
            Count = 0;
        }

        /// <summary>
        /// Returns the first element of the list
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            if (Data == null || Count == 0) return default(T);
            return Data[0];
        }

        /// <summary>
        /// Returns the last element of the list
        /// </summary>
        /// <returns></returns>
        public T Last()
        {
            if (Data == null || Count == 0) return default(T);
            return Data[Count - 1];
        }

        /// <summary>
        /// Adds a new element to the array, creating more
        /// memory if necessary
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (Data == null || Count == Data.Length)
                ResizeArray();

            Data[Count] = item;
            Count++;
        }

        /// <summary>
        /// Adds a new element to the start of the array, creating more
        /// memory if necessary
        /// </summary>
        /// <param name="item"></param>
        public void AddStart(T item)
        {
            Insert(item, 0);
        }

        /// <summary>
        /// Inserts a new element to the array at the index specified, creating more
        /// memory if necessary
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item, int index)
        {
            if (Data == null || Count == Data.Length)
                ResizeArray();

            for (var i = Count; i > index; i--)
            {
                Data[i] = Data[i - 1];
            }

            Data[index] = item;
            Count++;
        }

        /// <summary>
        /// Removes an item from the start of the data
        /// </summary>
        /// <returns></returns>
        public T RemoveStart()
        {
            return RemoveAt(0);
        }

        /// <summary>
        /// Removes an item from the index of the data
        /// </summary>
        /// <returns></returns>
        public T RemoveAt(int index)
        {
            if (Data != null && Count != 0)
            {
                T val = Data[index];

                for (var i = index; i < Count - 1; i++)
                {
                    Data[i] = Data[i + 1];
                }

                Count--;
                Data[Count] = default(T);
                return val;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Removes an item from the data
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T Remove(T item)
        {
            if (Data != null && Count != 0)
            {
                for (var i = 0; i < Count; i++)
                {
                    if (Data[i].Equals(item))
                    {
                        return RemoveAt(i);
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// Removes an item from the end of the data
        /// </summary>
        /// <returns></returns>
        public T RemoveEnd()
        {
            if (Data != null && Count != 0)
            {
                Count--;
                T val = Data[Count];
                Data[Count] = default(T);

                return val;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Determines if the data contains the item
        /// </summary>
        /// <param name="item">The item to compare</param>
        /// <returns>True if the item exists in teh data</returns>
        public bool Contains(T item)
        {
            if (Data == null)
                return false;

            for (var i = 0; i < Count; i++)
            {
                if (Data[i].Equals(item))
                    return true;
            }

            return false;
        }
    }
}