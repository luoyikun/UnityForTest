using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace xasset.example
{
    [RequireComponent(typeof(Updater))]
    [DisallowMultipleComponent]
    public class Startup : MonoBehaviour
    {
        [Tooltip("是否开启日志")] public bool loggable;

        [Tooltip("文件校验模式，对于带hash的文件名，其实使用 size 校验就可以了")] [SerializeField]
        private VerifyMode verifyMode = VerifyMode.Size;

        [Tooltip("初始化完成后回调")] public UnityEvent completed;

        private IEnumerator Start()
        {
            DontDestroyOnLoad(gameObject);
            Versions.VerifyMode = verifyMode;
            var customLoader = GetComponent<CustomLoader>();
            if (customLoader != null)
            {
                customLoader.Initialize();
            }

            var operation = Versions.InitializeAsync();
            yield return operation;
            Logger.I("Initialize: {0}", operation.status);
            Logger.I("API Version: {0}", Versions.APIVersion);
            Logger.I("Manifests Version: {0}", Versions.ManifestVersion);
            Logger.I("SimulationMode: {0}", Versions.SimulationMode);
            Logger.I("OfflineMode: {0}", Versions.OfflineMode);
            var go = gameObject;
            var preloadManager = go.AddComponent<PreloadManager>();
            var assetManager = AssetManager.Get(go, true);
            var instantiate = InstantiateObject.InstantiateAsync("LoadingScreen");
            yield return instantiate;
            DontDestroyOnLoad(instantiate.result);
            instantiate.result.SetActive(true);
            yield return assetManager.Preload("MessageBox", typeof(GameObject));
            var loadingScreen = instantiate.result.GetComponent<LoadingScreen>();
            preloadManager.progressBar = loadingScreen;
            preloadManager.SetVisible(false);
            completed?.Invoke();
        }

        private void Update()
        {
            Logger.Loggable = loggable;
        }
    }
}