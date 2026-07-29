// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/08/2013 by Ivan Lyagin

#if INCLUDE_FILE
using System;

namespace Aspose.Collections
{
    /// <summary>
    /// Represents a variable size last-in-first-out (LIFO) collection of instances of the same arbitrary type.
    /// </summary>
    /// <dev>
    /// This class was created using ROTOR sources. It was reworked considering AW specifics and Java autoportability.
    /// </dev>
    public class Stack<T>
    {
        /// <summary>
        /// Initializes a new instance of the Stack<T> class that is empty and has the default initial capacity.
        /// </summary>
        public Stack()
        {
            mItems = gEmptyArray;
        }

        /// <summary>
        /// Initializes a new instance of the Stack<T> class that is empty and has the specified initial capacity 
        /// or the default initial capacity, whichever is greater.
        /// </summary>
        public Stack(int capacity)
        {
            ArgumentUtil.CheckNonNegative(capacity, "capacity");
            mItems = new T[capacity];
        }

        /// <summary>
        /// Initializes a new instance of the Stack<T> class that contains elements copied from the specified array 
        /// and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        public Stack(T[] array)
        {
            ArgumentUtil.CheckNotNull(array, "array");
            mSize = array.Length;
            mItems = new T[mSize];
            array.CopyTo(mItems, 0);
        }

        /// <summary>
        /// Initializes a new instance of the Stack<T> class that contains elements copied from the specified Stack<T> 
        /// and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        public Stack(Stack<T> stack)
        {
            ArgumentUtil.CheckNotNull(stack, "stack");
            mSize = stack.mSize;
            mItems = new T[mSize];
            Array.Copy(stack.mItems, 0, mItems, 0, mSize);
        }

        /// <summary>
        /// Checks whether the stack is not empty. Throws if it is.
        /// </summary>
        private void CheckNotEmpty()
        {
            if (mSize == 0)
                throw new InvalidOperationException("The stack is empty.");
        }

        /// <summary>
        /// Removes all objects from the Stack<T>.
        /// </summary>
        public void Clear()
        {
            // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
            Array.Clear(mItems, 0, mSize);
            mSize = 0;
        }

        /// <summary>
        /// Determines whether an element is in the Stack<T>.
        /// </summary>
        public bool Contains(T item)
        {
            if (mSize == 0)
                return false;

            return (Array.LastIndexOf(mItems, item, mSize - 1, mSize) >= 0);
        }

        /// <summary>
        /// Copies the Stack<T> to an existing one-dimensional array, starting at the specified array index.
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ArgumentUtil.CheckNotNull(array, "array");
            ArgumentUtil.CheckNonNegative(arrayIndex, "arrayIndex");
            if (arrayIndex + mSize > array.Length)
                throw new ArgumentException("The given array index specifies an invalid offset length.");

            for (int i = 0; i < mSize; i++)
                array[i + arrayIndex] = mItems[mSize - i - 1];
        }

        /// <summary>
        /// Returns the object at the top of the Stack<T> without removing it.
        /// </summary>
        public T Peek()
        {
            CheckNotEmpty();
            return mItems[mSize - 1];
        }

        /// <summary>
        /// Removes and returns the object at the top of the Stack<T>.
        /// </summary>
        public T Pop()
        {
            CheckNotEmpty();
            T item = mItems[--mSize];
            mItems[mSize] = gDefaultValue;
            return item;
        }

        /// <summary>
        /// Inserts an object at the top of the Stack<T>.
        /// </summary>
        public void Push(T item)
        {
            if (mSize == mItems.Length)
            {
                int newCapacity = (mItems.Length == 0) ? DefaultCapacity : (mItems.Length * 2);
                T[] newItems = new T[newCapacity];
                Array.Copy(mItems, 0, newItems, 0, mSize);
                mItems = newItems;
            }

            mItems[mSize++] = item;
        }

        /// <summary>
        /// Copies the Stack<T> to a new array.
        /// </summary>
        public T[] ToArray()
        {
            T[] array = new T[mSize];
            CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the Stack<T>, if that number is less 
        /// than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess()
        {
            int threshold = (int)(((double)mItems.Length) * 0.9);
            if (mSize < threshold)
            {
                T[] newItems = new T[mSize];
                Array.Copy(mItems, 0, newItems, 0, mSize);
                mItems = newItems;
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(Stack<T>))
                return false;

            return Equals((Stack<T>)obj);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        private bool Equals(Stack<T> rhs)
        {
            if (ReferenceEquals(null, rhs))
                return false;
            if (ReferenceEquals(this, rhs))
                return true;

            if (Count != rhs.Count)
                return false;

            for (int i = 0; i < Count; i++)
            {
                if (mItems[i] != rhs.mItems[i])
                    return false;
            }

            return true;
        }
        
        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <remarks>
        /// Note, this object is not immutable, so you should take your own care about it is not changed
        /// after this method is invoked, for example when it is used in a hashtable.
        /// </remarks>
        public override int GetHashCode()
        {
            int hashCode = Count;

            for (int i = 0; i < Count; i++)
                hashCode = unchecked(hashCode * 397 + mItems[i].GetHashCode());

            return hashCode;
        }


#if DEBUG
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        //public override string ToString()
        //{
        //    return ArrayUtil.DumpArray(mItems, 0, Math.Min(20, Count));
        //}
#endif

        /// <summary>
        /// Gets the number of elements contained in the Stack<T>.
        /// </summary>
        public int Count
        {
            get { return mSize; }
        }

        private T[] mItems;
        private int mSize;

        private static readonly T[] gEmptyArray = new T[0];

        // The default item value. The field is not readonly to satisfy Java final.
        private static T gDefaultValue;

        private const int DefaultCapacity = 4;
    }
}
#endif
