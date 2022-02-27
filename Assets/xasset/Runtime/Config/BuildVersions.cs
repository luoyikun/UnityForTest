using System.Collections.Generic;
using UnityEngine;

namespace xasset
{
    public class BuildVersions : ScriptableObject
    {
        public long timestamp;
        public bool offlineMode;

        public List<string> streamingAssets = new List<string>();

        public List<BuildVersion> data = new List<BuildVersion>();

        private readonly Dictionary<string, BuildVersion> nameWithVersion = new Dictionary<string, BuildVersion>();

        public static BuildVersions Load(string path)
        {
            var asset = Utility.LoadScriptableObjectWithJson<BuildVersions>(path);
            asset.Load();
            return asset;
        }

        private void Load()
        {
            nameWithVersion.Clear();
            foreach (var version in data)
            {
                nameWithVersion[version.name] = version;
            }
        }

        public void Set(string build, string file, long size, long time, string hash)
        {
            if (!nameWithVersion.TryGetValue(build, out var value))
            {
                value = new BuildVersion { name = build, file = file, size = size, hash = hash };
                nameWithVersion.Add(build, value);
                data.Add(value);
            }
            else
            {
                value.file = file;
                value.size = size;
                value.hash = hash;
            }

            timestamp = time;
        }


        public BuildVersion Get(string build)
        {
            return nameWithVersion.TryGetValue(build, out var value) ? value : null;
        }
    }
}