using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackBasedCalculator
{
    //Author: Kurt Carpenter
    //Date: 1/26/2015
    //Version: 1.0
    //Class which implements a basic dictionary using an array as the backing structure
    sealed class ArrayDictionary<T,E>
    {
        private T[] keys;
        private E[] values;
        private int size;

        public ArrayDictionary() : this(10)
        {
        }

        public ArrayDictionary(int capacity)
        {
            keys = new T[capacity];
            values = new E[capacity];
        }

        public void Add(T key, E value)
        {
            if (null == value)
                throw new ArgumentNullException();
            if (size >= keys.Length - 1)
            {
                T[] tempKeys = new T[keys.Length * 2];
                E[] tempValues = new E[values.Length * 2];
                for (int i = 0; i < keys.Length; i++)
                {
                    tempKeys[i] = keys[i];
                    tempValues[i] = values[i];
                }
                keys = tempKeys;
                values = tempValues;
            }
            keys[size] = key;
            values[size] = value;
            size++;
        }

        public bool Remove(T key)
        {
            if (key == null)
                throw new ArgumentNullException();
            for (int i = 0; i < size; i++)
            {
                if (keys[i].Equals(key))
                {
                    for (int j = i + 1; j < size; j++)
                    {
                        keys[j - 1] = keys[j];
                    }
                    break;
                }
            }
            return false;
        }

        public bool ContainsKey(T key)
        {
            if (key == null)
                throw new ArgumentNullException();
            foreach (T item in keys)
            {
                if (item == null)
                    return false;
                if (item.Equals(key))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsValue(E value)
        {
            if (value == null)
                throw new ArgumentNullException();
            foreach (E item in values)
            {
                if (item.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public void Clear()
        {
            size = 0;
            keys = new T[keys.Length];
            values = new E[keys.Length];
        }

        public int Size
        {
            get { return size; }
        }

        public E this[T key]
        {
            get
            {
                for(int i = 0; i < keys.Length; i++)
                {
                    if (key.Equals(keys[i]))
                        return values[i];
                }
                throw new KeyNotFoundException();
            }
            set
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (key.Equals(keys[i]))
                        values[i] = value;
                }
                throw new KeyNotFoundException();
            }
        }
    }
}
