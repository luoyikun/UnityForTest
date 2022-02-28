using System;
using System.Collections.Generic;

namespace xasset
{
    [Serializable]
    public class ManifestBundle
    {
        public int id;
        public string name;
        public long size;
        public string hash;
        public int[] deps;
        public bool isRaw;
        public ulong offset { get; set; }
        public List<string> assets { get; set; }
        public string nameWithAppendHash { get; set; }
    }
}