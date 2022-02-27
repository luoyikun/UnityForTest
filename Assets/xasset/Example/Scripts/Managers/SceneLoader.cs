using UnityEngine;

namespace xasset.example
{
    public class SceneLoader : MonoBehaviour
    {
        public ExampleScene scene;
        public float delay;

        public bool loadOnAwake = true;
        public bool showProgress;

        private Scene loading;

        private void Start()
        {
            if (loadOnAwake)
            {
                LoadScene();
            }
        }

        public void LoadScene()
        {
            if (delay > 0)
            {
                Invoke("Loading", 3);
                return;
            }

            Loading();
        }

        private void Loading()
        {
            if (loading != null)
            {
                return;
            }

            loading = Scene.LoadAsync(scene.ToString());
            if (showProgress)
            {
                PreloadManager.Instance.ShowProgress(loading);
            }
        }
    }
}