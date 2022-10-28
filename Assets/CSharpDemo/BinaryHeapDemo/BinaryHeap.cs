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

        
        /// <summary>
        /// 插入尾部，再跟父节点冒泡排序
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            items.Add(item); //先插入list的末尾

            int i = items.Count - 1;

            bool flag = HType == HeapType.MinHeap;

            while (i > 0)  //一直冒泡到头节点，当前节点的父节点idx为(i - 1) / 2
            {
                if ((items[i].CompareTo(items[(i - 1) / 2]) > 0) ^ flag) //异或，相同取0，相异取1；如果是子>父 异或 最小堆（true） == 0，不发生交换
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

        /// <summary>
        /// 删除头节点
        /// </summary>
        private void DeleteRoot()
        {
            int i = items.Count - 1;

            items[0] = items[i]; //先把队尾部放入头节点
            items.RemoveAt(i);  //移除队尾

            i = 0;

            bool flag = HType == HeapType.MinHeap;

            while (true)
            {
                int leftInd = 2 * i + 1; //左节点
                int rightInd = 2 * i + 2;//右节点
                int largest = i;

                if (leftInd < items.Count)
                {
                    if ((items[leftInd].CompareTo(items[largest]) > 0) ^ flag) //（左 > 父） 异或（是否最小堆）
                        largest = leftInd;
                }

                if (rightInd < items.Count)
                {
                    if ((items[rightInd].CompareTo(items[largest]) > 0) ^ flag)
                        largest = rightInd;
                }

                if (largest != i) //发生交换，父与左或者右的一个
                {
                    T temp = items[largest];
                    items[largest] = items[i];
                    items[i] = temp;
                    i = largest;
                }
                else //未发生交换，说明排序OK
                    break;
            }
        }

        //弹出头节点
        public T PopRoot()
        {
            T result = items[0];

            DeleteRoot();

            return result;
        }

        public T GetRoot()
        {
            T result = items[0];
            return result;
        }
    }

}