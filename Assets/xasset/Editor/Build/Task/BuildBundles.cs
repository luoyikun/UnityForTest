using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace xasset.editor
{
    public class BuildBundles : BuildTaskJob
    {
        private readonly BuildAssetBundleOptions _options;
        public readonly List<ManifestBundle> bundles = new List<ManifestBundle>();

        public BuildBundles(BuildTask task, BuildAssetBundleOptions options) : base(task)
        {
            _options = options;
        }

        private ABuildPipeline BuildPipeline => customBuildPipeline != null
            ? customBuildPipeline.Invoke(this)
            : new BuiltinBuildPipeline();

        public static Func<BuildBundles, ABuildPipeline> customBuildPipeline { get; set; } = null;

        public override void Run()
        {
            CreateBundles();
            if (bundles.Count > 0)
            {
                if (!BuildAssetBundles())
                {
                    return;
                }
            }

            _task.bundles.AddRange(bundles);
        }

        protected AssetBundleBuild[] GetBuilds()
        {
            return bundles.ConvertAll(bundle =>
                new AssetBundleBuild
                {
                    assetNames = bundle.assets.ToArray(),
                    assetBundleName = bundle.name
                }).ToArray();
        }

        private bool BuildAssetBundles()
        {
            var manifest = BuildPipeline.BuildAssetBundles(_task.outputPath, GetBuilds(), _options,
                EditorUserBuildSettings.activeBuildTarget);
            if (manifest == null)
            {
                TreatError($"Failed to build AssetBundles with {_task.name}.");
                return false;
            }

            var nameWithBundles = GetBundles();
            return BuildWithoutEncryption(nameWithBundles, manifest);
        }

        private bool BuildWithoutEncryption(IReadOnlyDictionary<string, ManifestBundle> nameWithBundles,
            IAssetBundleManifest manifest)
        {
            var assetBundles = manifest.GetAllAssetBundles();
            foreach (var assetBundle in assetBundles)
            {
                if (nameWithBundles.TryGetValue(assetBundle, out var bundle))
                {
                    var path = GetBuildPath(assetBundle);
                    var hash = Utility.ComputeHash(path);
                    var nameWithAppendHash =
                        $"{Path.GetFileNameWithoutExtension(path)}_{hash}{Settings.BundleExtension}";
                    bundle.hash = hash;
                    bundle.deps = Array.ConvertAll(manifest.GetAllDependencies(assetBundle),
                        input => nameWithBundles[input].id);
                    bundle.nameWithAppendHash = nameWithAppendHash;
                    var dir = Path.GetDirectoryName(path);
                    var newPath = $"{dir}/{nameWithAppendHash}";
                    var info = new FileInfo(path);
                    if (info.Exists)
                    {
                        bundle.size = info.Length;
                    }
                    else
                    {
                        TreatError($"File not found: {info}");
                        return false;
                    }

                    if (!File.Exists(newPath))
                    {
                        info.CopyTo(newPath, true);
                    }
                }
                else
                {
                    TreatError($"Bundle not found: {assetBundle}");
                    return false;
                }
            }

            return true;
        }

        private Dictionary<string, ManifestBundle> GetBundles()
        {
            var nameWithBundles = new Dictionary<string, ManifestBundle>();
            for (var i = 0; i < bundles.Count; i++)
            {
                var bundle = bundles[i];
                bundle.id = i;
                nameWithBundles[bundle.name] = bundle;
            }

            return nameWithBundles;
        }

        private void CreateBundles()
        {
            foreach (var assetBundleName in AssetDatabase.GetAllAssetBundleNames())
            {
                bundles.Add(new ManifestBundle
                {
                    name = assetBundleName,
                    assets = new List<string>(AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName))
                });
            }
        }
    }
}