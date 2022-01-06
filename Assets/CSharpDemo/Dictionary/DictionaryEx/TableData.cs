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
            mChineseTableName = strChineseTableName;//上线后屏蔽
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
            return default(T); //此关键字对于引用类型会返回空，对于数值类型会返回零。对于结构，此关键字将返回初始化为零或空的每个结构成员
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
