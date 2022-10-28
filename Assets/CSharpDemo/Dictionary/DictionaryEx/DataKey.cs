using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DictionaryEx
{
    public class DataKey1
    {
        public int mKey1;

        public DataKey1(int key)
        {
            mKey1 = key;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            DataKey1 key = obj as DataKey1;
            if (key == null) return false;

            return this.mKey1 == key.mKey1;
        }

        public override int GetHashCode()
        {
            return mKey1.GetHashCode();
        }
    }

    public class DataKey2 : DataKey1
    {
        public int mKey2;

        public DataKey2(int key1, int key2)
            : base(key1)
        {
            mKey2 = key2;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            DataKey2 key = obj as DataKey2;
            if (key == null) return false;

            return this.mKey1 == key.mKey1 && this.mKey2 == key.mKey2;
        }

        public override int GetHashCode()
        {

            return base.GetHashCode() ^ mKey2.GetHashCode();
        }
    }

    public class DataKey3 : DataKey2
    {
        public int mKey3;

        public DataKey3(int key1, int key2, int key3)
            : base(key1, key2)
        {
            mKey3 = key3;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            DataKey3 key = obj as DataKey3;
            if (key == null) return false;

            return this.mKey3 == key.mKey3 && this.mKey2 == key.mKey2 && this.mKey1 == key.mKey1;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ mKey3.GetHashCode();
        }
    }

    public class DataKey4 : DataKey3
    {
        public int mKey4;

        public DataKey4(int key1, int key2, int key3, int key4)
            : base(key1, key2, key3)
        {
            mKey4 = key4;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            DataKey4 key = obj as DataKey4;
            if (key == null) return false;

            return this.mKey4 == key.mKey4 && this.mKey3 == key.mKey3 && this.mKey2 == key.mKey2 && this.mKey1 == key.mKey1;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ mKey4.GetHashCode();
        }
    }
}
