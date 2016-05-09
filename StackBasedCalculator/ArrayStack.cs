using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackBasedCalculator
{
    //Author: Kurt Carpenter
    //Date: 1/26/2015
    //Version: 1.0
    //Class which implements a basic stack using an array as the backing structure
    sealed class ArrayStack<T>
    {
        private T[] stack;
        private int size;

        public ArrayStack() : this(10)
        {
        }

        public ArrayStack(int capacity)
        {
            stack = new T[capacity];
        }

        public void Push(T item)
        {
            if (null == item)
            {
                Console.WriteLine("Attempted to add null to stack");
            }
            if (size >= stack.Length - 1)
            {
                T[] temp = new T[stack.Length * 2];
                for (int i = 0; i < stack.Length; i++)
                {
                    temp[i] = stack[i];
                }
                stack = temp;
            }
            stack[size] = item;
            size++;
        }

        public T Pop()
        {
            if (size == 0)
            {
                              
            }
            size--;
            T ret = stack[size];
            stack[size] = default(T);            
            return ret;
        }

        public T Peek()
        {
            T ret = Pop();
            Push(ret);
            return ret;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public void Clear()
        {
            size = 0;
            stack = new T[stack.Length];
        }

        public int Size
        {
            get { return size; }
        }

        public override string ToString()
        {
            StringBuilder stringy = new StringBuilder();
            for (int i = size; i >= 0; i--)
            {
                stringy.Append(stack[i]);
                stringy.Append(" ");
            }
            return stringy.ToString();
        }
    }
}
