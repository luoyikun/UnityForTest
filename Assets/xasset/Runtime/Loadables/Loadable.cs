using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace xasset
{
    //�ܼ�����С����
    public class Loadable
    {
        public static readonly List<Loadable> Loading = new List<Loadable>();//��̬���������ڼ��ص�ab
        public static readonly List<Loadable> Unused = new List<Loadable>();

        protected static bool _updateUnloadUnusedAssets;

        private readonly Reference _reference = new Reference();
        public LoadableStatus status { get; protected set; } = LoadableStatus.Wait;
        public string pathOrURL { get;
            protected set; }
        public string error { get; internal set; }

        public bool isDone => status == LoadableStatus.SuccessToLoad || status == LoadableStatus.Unloaded ||
                              status == LoadableStatus.FailedToLoad || status == LoadableStatus.Async2SyncEnd;

        public float progress { get; protected set; }

        protected void Finish(string errorCode = null)
        {
            error = errorCode;
            if (status == LoadableStatus.LoadingAsync)
            {
                status = LoadableStatus.Async2SyncEnd;
            }
            else
            {
                status = string.IsNullOrEmpty(errorCode) ? LoadableStatus.SuccessToLoad : LoadableStatus.FailedToLoad;
            }
            progress = 1;
        }

        public static void UpdateAll()
        {
            //��β���������Ǹ���
            for (var index = 0; index < Loading.Count; index++)
            {
                var item = Loading[index];
                if (Updater.busy) return;

                item.Update();
                if (!item.isDone) continue;

                Loading.RemoveAt(index);
                index--;
                item.Complete();
            }

            if (Scene.IsLoadingOrUnloading()) return;

            for (int index = 0, max = Unused.Count; index < max; index++)
            {
                var item = Unused[index];
                if (Updater.busy) break;

                if (!item.isDone) continue;

                Unused.RemoveAt(index);
                index--;
                max--;
                if (!item._reference.unused) continue;

                item.Unload();
            }

            if (Unused.Count > 0) return;

            if (_updateUnloadUnusedAssets)
            {
                Resources.UnloadUnusedAssets();
                _updateUnloadUnusedAssets = false;
            }
        }

        private void Update()
        {
            OnUpdate();
        }

        private void Complete()
        {
            if (status == LoadableStatus.FailedToLoad)
            {
                Logger.E("Unable to load {0} {1} with error: {2}", GetType().Name, pathOrURL, error);
                Release();
            }

            //�첽תͬ����ȡ���첽�Ļص�
            //if (status != LoadableStatus.Async2SyncEnd)
            {
                OnComplete();
            }
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnLoad()
        {
        }

        protected virtual void OnUnload()
        {
        }

        protected virtual void OnComplete()
        {
        }

        public virtual void LoadImmediate()
        {
            throw new InvalidOperationException();
        }

        protected void Load()
        {
            if (status != LoadableStatus.Wait && _reference.unused) Unused.Remove(this);

            _reference.Retain(); //����+1
            Loading.Add(this);
            if (status != LoadableStatus.Wait) return;
            Logger.I("Load {0} {1}.", GetType().Name, pathOrURL);
            status = LoadableStatus.Loading;
            progress = 0;
            OnLoad();
        }

        private void Unload()
        {
            if (status == LoadableStatus.Unloaded) return;
            Logger.I("Unload {0} {1}.", GetType().Name, pathOrURL, error);
            OnUnload();
            status = LoadableStatus.Unloaded;
        }

        public void Release()
        {
            if (_reference.count <= 0)
            {
                Logger.W("Release {0} {1}.", GetType().Name, Path.GetFileName(pathOrURL));
                return;
            }

            _reference.Release();
            if (!_reference.unused) return;

            Unused.Add(this);
            OnUnused();
        }

        protected virtual void OnUnused()
        {
        }

        public static void ClearAll()
        {
            Asset.Cache.Clear();
            Bundle.Cache.Clear();
            Dependencies.Cache.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
        }
    }

    public enum LoadableStatus
    {
        Wait,
        Loading,
        LoadingAsync,//��ǰ���첽����
        DependentLoading,
        SuccessToLoad,
        FailedToLoad,
        Unloaded,
        Async2SyncEnd//�첽ת��ͬ��
    }
}