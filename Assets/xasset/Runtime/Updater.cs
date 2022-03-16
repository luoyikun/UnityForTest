using UnityEngine;

namespace xasset
{
    [DisallowMultipleComponent]
    public sealed class Updater : MonoBehaviour
    {
        private static float _realtimeSinceUpdateStartup;
        [SerializeField] private float _maxUpdateTimeSlice = 0.01f;
        public static float maxUpdateTimeSlice { get; set; }
        public static bool busy => Time.realtimeSinceStartup - _realtimeSinceUpdateStartup >= maxUpdateTimeSlice;

        private void Start()
        {
            maxUpdateTimeSlice = _maxUpdateTimeSlice;
        }

        private void Update()
        {
            _realtimeSinceUpdateStartup = Time.realtimeSinceStartup;
            Loadable.UpdateAll();
            Operation.UpdateAll();
            AsyncUpdate.UpdateAll();
        }

        private void OnDestroy()
        {
            Loadable.ClearAll();
            Operation.ClearAll();
            AsyncUpdate.ClearAll();
        }


        //[RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            var updater = FindObjectOfType<Updater>();
            if (updater != null)
            {
                return;
            }

            updater = new GameObject("Updater").AddComponent<Updater>();
            DontDestroyOnLoad(updater);
        }
    }
}