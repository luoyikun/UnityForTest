using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace xasset.editor
{
    public class BuildTask
    {
        public readonly List<ManifestBundle> bundles = new List<ManifestBundle>();
        public readonly List<string> changes = new List<string>();
        public readonly string outputPath;
        public readonly List<BuildTaskJob> jobs = new List<BuildTaskJob>();
        public readonly Stopwatch stopwatch = new Stopwatch();
        public int buildVersion;
        public bool forceRebuild;

        public BuildTask(int version = -1) : this("Manifest")
        {
            buildVersion = version;
            jobs.Add(new BuildBundles(this, BuildAssetBundleOptions.ChunkBasedCompression));
            jobs.Add(new CreateManifest(this));
        }

        public void Run()
        {
            stopwatch.Start();
            foreach (var job in jobs)
            {
                try
                {
                    job.Run();
                }
                catch (Exception e)
                {
                    job.error = e.Message;
                    Debug.LogException(e);
                }

                if (string.IsNullOrEmpty(job.error)) continue;
                break;
            }

            stopwatch.Stop();
            var elapsed = stopwatch.ElapsedMilliseconds / 1000f;
            Debug.LogFormat("Run BuildTask for {0} with {1}s", name, elapsed);
        }

        public BuildTask(string build)
        {
            name = build;
            outputPath = Settings.PlatformBuildPath;
        }

        public string name { get; }


        public string GetBuildPath(string filename)
        {
            return $"{outputPath}/{filename}";
        }

        public void SaveManifest(Manifest manifest)
        {
            var timestamp = DateTime.Now.ToFileTime();
            manifest.name = name.ToLower();
            var filename = $"{manifest.name}.json";
            File.WriteAllText(GetBuildPath(filename), JsonUtility.ToJson(manifest));
            var path = GetBuildPath(filename);
            var hash = Utility.ComputeHash(path);
            var file = $"{manifest.name}_v{manifest.version}_{hash}.json";
            File.Move(GetBuildPath(filename), GetBuildPath(file));
            changes.Add(file);
            // save version
            SaveVersion(file, timestamp, hash);
        }

        private void SaveVersion(string file, long timestamp, string hash)
        {
            var buildVersions = BuildVersions.Load(GetBuildPath(Versions.Filename));
            var info = new FileInfo(GetBuildPath(file));
            buildVersions.Set(name, file, info.Length, timestamp, hash);
            File.WriteAllText(GetBuildPath(Versions.Filename), JsonUtility.ToJson(buildVersions));
        }
    }
}