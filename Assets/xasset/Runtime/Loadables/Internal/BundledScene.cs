﻿using UnityEngine.SceneManagement;

namespace xasset
{
    public class BundledScene : Scene
    {
        private Dependencies _dependencies;

        protected override void OnUpdate()
        {
            switch (status)
            {
                case LoadableStatus.DependentLoading:
                    UpdateDependencies();
                    break;
                case LoadableStatus.Loading:
                    UpdateLoading();
                    break;
            }
        }

        private void UpdateDependencies()
        {
            if (_dependencies == null)
            {
                Finish("dependencies == null");
                return;
            }

            progress = _dependencies.progress * 0.5f;
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

            LoadSceneAsync(SceneManager.LoadSceneAsync(sceneName, loadSceneMode));
            status = LoadableStatus.Loading;
        }

        protected override void OnUnload()
        {
            if (_dependencies != null)
            {
                _dependencies.Release();
                _dependencies = null;
            }

            base.OnUnload();
        }


        protected override void OnLoad()
        {
            PrepareToLoad();
            _dependencies = Dependencies.Load(pathOrURL);
            if (mustCompleteOnNextFrame)
            {
                _dependencies.LoadImmediate();
                SceneManager.LoadScene(sceneName, loadSceneMode);
                Finish();
            }
            else
            {
                status = LoadableStatus.DependentLoading;
            }
        }
    }
}