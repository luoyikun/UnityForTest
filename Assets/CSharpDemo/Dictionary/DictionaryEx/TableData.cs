using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DictionaryEx
{
    public class TableData<K, T> //: CountString
        where K : DataKey1
    {
        protected const int mcHeadSize = 140;
        protected string mTableName;
        protected string mChineseTableName = "";
        protected int mTimeStamp;
        protected int mCurSeq;
        protected Dictionary<K, T> mTableData = new Dictionary<K, T>(131);

        protected List<T> mTableDataSequence = new List<T>(128);		

        public TableData(string tableName, string strChineseTableName)
        {
            mTableName = tableName;
            mChineseTableName = strChineseTableName;//���ߺ�����
        }

        public TableData()
        {
            
        }

        public string name
        {
            get
            {
                return mTableName;
            }
        }

        public int timeStamp
        {
            set
            {
                mTimeStamp = value;
            }
            get
            {
                return mTimeStamp;
            }
        }

        public int curSeq
        {
            set
            {
                mCurSeq = value;
            }
            get
            {
                return mCurSeq;
            }
        }

        // 
        public List<T> GetTableSequenceData()
        {
            return mTableDataSequence;
        }

        public Dictionary<K, T> GetTableKeyValueData()
        {
            return mTableData;
        }

        public Int32 Count()
        {
            return mTableData.Count;
        }

        public T Find(K key)
        {
            T ret;

            if (mTableData.TryGetValue(key, out ret)) return ret;
            return default(T); //�˹ؼ��ֶ����������ͻ᷵�ؿգ�������ֵ���ͻ᷵���㡣���ڽṹ���˹ؼ��ֽ����س�ʼ��Ϊ���յ�ÿ���ṹ��Ա
        }

        public void Add(K key,T data)
        {
            mTableDataSequence.Add(data);
            mTableData.Add(key, data);
        }

        public void Remove(K key)
        {
            T data;
            if (mTableData.TryGetValue(key, out data))
            {
                mTableDataSequence.Remove(data);
                mTableData.Remove(key);
            }
        }
    }
}
