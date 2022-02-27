using System;
using UnityEngine;

namespace xasset
{
    public class BundledAsset : Asset
    {
        private Dependencies _dependencies;
        private AssetBundleRequest _request;

        internal static BundledAsset Create(string path, Type type)
        {
            return new BundledAsset
            {
                pathOrURL = path,
                type = type
            };
        }

        protected override void OnLoad()
        {
            _dependencies = Dependencies.Load(pathOrURL);
            status = LoadableStatus.DependentLoading;
        }

        protected override void OnUnload()
        {
            if (_dependencies != null)
            {
                _dependencies.Release();
                if (_dependencies.unused)
                {
                    if (type != typeof(GameObject) && !(asset is GameObject))
                    {
                        if (isSubAssets)
                        {
                            foreach (var subAsset in subAssets)
                            {
                                Resources.UnloadAsset(subAsset);
                            }
                        }
                        else
                        {
                            Resources.UnloadAsset(asset);
                        }

                        _updateUnloadUnusedAssets = true;
                    }
                }

                _dependencies = null;
            }

            asset = null;
            _request = null;
            base.OnUnload();
        }

        public override void LoadImmediate()
        {
            if (isDone)
            {
                return;
            }

            if (_dependencies == null)
            {
                Finish("dependencies == null");
                return;
            }

            if (!_dependencies.isDone)
            {
                _dependencies.LoadImmediate();
            }

            if (_dependencies.assetBundle == null)
            {
                Finish("dependencies.assetBundle == null");
                return;
            }

            if (isSubAssets)
            {
                subAssets = _dependencies.assetBundle.LoadAssetWithSubAssets(pathOrURL, type);//从捆绑包中加载名为 name 的资源和子资源,把自己的依赖资源也加载了
                Finish();
            }
            else
            {
                OnLoaded(_dependencies.assetBundle.LoadAsset(pathOrURL, type));
            }
        }

        protected override void OnUpdate()
        {
            switch (status)
            {
                case LoadableStatus.Loading:
                    UpdateLoading();
                    break;
                case LoadableStatus.DependentLoading:
                    UpdateDependencies();
                    break;
            }
        }

        private void UpdateLoading()
        {
            if (_request == null)
            {
                Finish("request == null");
                return;
            }

            progress = 0.5f + _request.progress * 0.5f;
            if (!_request.isDone)
            {
                return;
            }

            if (isSubAssets)
            {
                subAssets = _request.allAssets;
                Finish(subAssets == null ? "subAssets == null" : null);
            }
            else
            {
                OnLoaded(_request.asset);
            }
        }

        private void UpdateDependencies()
        {
            if (_dependencies == null)
            {
                Finish("dependencies == null");
                return;
            }

            progress = 0.5f * _dependencies.progress;
            if (!_dependencies.isDone)
            {
                return;
            }

            var assetBundle = _dependencies.assetBundle;
            if (assetBundle == null)
            {
                Finish("assetBundle == null");
                return;
            }

            _request = isSubAssets
                ? assetBundle.LoadAssetWithSubAssetsAsync(pathOrURL, type)
                : assetBundle.LoadAssetAsync(pathOrURL, type);
            status = LoadableStatus.Loading;
        }
    }
}