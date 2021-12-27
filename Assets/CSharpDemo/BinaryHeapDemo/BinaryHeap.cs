using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    public enum HeapType
    {
        MinHeap,
        MaxHeap
    }

    public class BinaryHeap<T> where T : IComparable<T>
    {
        public List<T> items;

        public HeapType HType { get; private set; }

        public T Root
        {
            get { return items[0]; }
        }

        public BinaryHeap(HeapType type)
        {
            items = new List<T>();
            this.HType = type;
        }

        public bool Contains(T data)
        {
            return items.Contains(data);
        }

        

        public void Push(T item)
        {
            items.Add(item);

            int i = items.Count - 1;

            bool flag = HType == HeapType.MinHeap;

            while (i > 0)
            {
                if ((items[i].CompareTo(items[(i - 1) / 2]) > 0) ^ flag)
                {
                    T temp = items[i];
                    items[i] = items[(i - 1) / 2];
                    items[(i - 1) / 2] = temp;
                    i = (i - 1) / 2;
                }
                else
                    break;
            }
        }

        private void DeleteRoot()
        {
            int i = items.Count - 1;

            items[0] = items[i];
            items.RemoveAt(i);

            i = 0;

            bool flag = HType == HeapType.MinHeap;

            while (true)
            {
                int leftInd = 2 * i + 1;
                int rightInd = 2 * i + 2;
                int largest = i;

                if (leftInd < items.Count)
                {
                    if ((items[leftInd].CompareTo(items[largest]) > 0) ^ flag)
                        largest = leftInd;
                }

                if (rightInd < items.Count)
                {
                    if ((items[rightInd].CompareTo(items[largest]) > 0) ^ flag)
                        largest = rightInd;
                }

                if (largest != i)
                {
                    T temp = items[largest];
                    items[largest] = items[i];
                    items[i] = temp;
                    i = largest;
                }
                else
                    break;
            }
        }

        public T PopRoot()
        {
            T result = items[0];

            DeleteRoot();

            return result;
        }
    }

}