using System;

namespace xasset
{
    [Serializable]
    public class BuildVersion
    {
        public string name;
        public string file;
        public long size;
        public string hash;
    }
}