using UnityEngine;

namespace xasset
{
    //本地包
    internal class LocalBundle : Bundle
    {
        private AssetBundleCreateRequest _request;

        protected override void OnLoad()
        {
            Debug.Log("异步加载：" + pathOrURL);
            _request = LoadAssetBundleAsync(pathOrURL);
            m_enA2S = EnAsync2Sync.LoadingAsync;
            
        }

        public override void LoadImmediate()
        {
            if (isDone)
            {
                return;
            }
            if (m_enA2S == EnAsync2Sync.LoadingAsync)
            {
                m_enA2S = EnAsync2Sync.LoadingAsync2Sync;
            }
            OnLoaded(_request.assetBundle);//设置了加载状态，会停止OnUpdate
            Debug.Log("同步加载：" + pathOrURL);
            _request = null;
        }

        protected override void OnUpdate()
        {
            if (status != LoadableStatus.Loading)
            {
                return;
            }

            progress = _request.progress;
            if (_request.isDone)
            {
                OnLoaded(_request.assetBundle);
            }
        }
    }
}