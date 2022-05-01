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
        /// ����β�����ٸ����ڵ�ð������
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            items.Add(item); //�Ȳ���list��ĩβ

            int i = items.Count - 1;

            bool flag = HType == HeapType.MinHeap;

            while (i > 0)  //һֱð�ݵ�ͷ�ڵ㣬��ǰ�ڵ�ĸ��ڵ�idxΪ(i - 1) / 2
            {
                if ((items[i].CompareTo(items[(i - 1) / 2]) > 0) ^ flag) //�����ͬȡ0������ȡ1���������>�� ��� ��С�ѣ�true�� == 0������������
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
        /// ɾ��ͷ�ڵ�
        /// </summary>
        private void DeleteRoot()
        {
            int i = items.Count - 1;

            items[0] = items[i]; //�ȰѶ�β������ͷ�ڵ�
            items.RemoveAt(i);  //�Ƴ���β

            i = 0;

            bool flag = HType == HeapType.MinHeap;

            while (true)
            {
                int leftInd = 2 * i + 1; //��ڵ�
                int rightInd = 2 * i + 2;//�ҽڵ�
                int largest = i;

                if (leftInd < items.Count)
                {
                    if ((items[leftInd].CompareTo(items[largest]) > 0) ^ flag) //���� > ���� ����Ƿ���С�ѣ�
                        largest = leftInd;
                }

                if (rightInd < items.Count)
                {
                    if ((items[rightInd].CompareTo(items[largest]) > 0) ^ flag)
                        largest = rightInd;
                }

                if (largest != i) //��������������������ҵ�һ��
                {
                    T temp = items[largest];
                    items[largest] = items[i];
                    items[i] = temp;
                    i = largest;
                }
                else //δ����������˵������OK
                    break;
            }
        }

        //����ͷ�ڵ�
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