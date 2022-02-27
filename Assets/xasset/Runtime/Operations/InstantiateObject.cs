using System.Collections.Generic;
using UnityEngine;

namespace xasset
{
    public sealed class InstantiateObject : Operation
    {
        private static readonly List<InstantiateObject> AllObjects = new List<InstantiateObject>();
        private Asset _asset;
        private string _path { get; set; }
        public GameObject result { get; private set; }

        public override void Start()
        {
            base.Start();
            _asset = Asset.LoadAsync(_path, typeof(GameObject));
            AllObjects.Add(this);
        }

        public static InstantiateObject InstantiateAsync(string assetPath)
        {
            var operation = new InstantiateObject
            {
                _path = assetPath
            };
            operation.Start();
            return operation;
        }

        protected override void Update()
        {
            if (status != OperationStatus.Processing)
            {
                return;
            }

            if (_asset == null)
            {
                Finish("asset == null");
                return;
            }

            progress = _asset.progress;
            if (!_asset.isDone)
            {
                return;
            }

            if (_asset.status == LoadableStatus.FailedToLoad)
            {
                Finish(_asset.error);
                return;
            }

            if (_asset.asset == null)
            {
                Finish("asset == null");
                return;
            }

            result = Object.Instantiate(_asset.asset as GameObject);
            Finish();
        }


        public void Destroy()
        {
            if (!isDone)
            {
                Finish("User Cancelled");
                return;
            }

            if (status == OperationStatus.Success)
            {
                if (result != null)
                {
                    Object.DestroyImmediate(result);
                    result = null;
                }
            }

            if (_asset == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_asset.error))
            {
                _asset.Release();
            }

            _asset = null;
        }

        public static void UpdateObjects()
        {
            for (var index = 0; index < AllObjects.Count; index++)
            {
                var item = AllObjects[index];
                if (Updater.busy)
                {
                    return;
                }

                if (!item.isDone || item.result != null)
                {
                    continue;
                }

                AllObjects.RemoveAt(index);
                index--;
                item.Destroy();
            }
        }

        public static void ClearObjects()
        {
            AllObjects.Clear();
        }
    }
}