using System;
using System.Collections;
using UnityEngine;

namespace xasset.example
{
    public class DownloadControl : MonoBehaviour
    {
        [SerializeField] private int[] speeds = { 0, 512 * 1024, 1024 * 1024 };
        [SerializeField] private uint[] downloads = { 1, 3, 5, 10 };
        private int downloadIndex;
        private int speedIndex;

        private IEnumerator Start()
        {
            if (!Versions.Initialized)
            {
                yield return Versions.InitializeAsync();
            }
        }

        protected void DrawSpeed()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("速度");
                speedIndex = GUILayout.SelectionGrid(speedIndex, Array.ConvertAll(speeds, s => Utility.FormatBytes(s)),
                    speeds.Length);
                var speed = speeds[speedIndex];
                Download.MaxBandwidth = speed;
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("数量");
                downloadIndex = GUILayout.SelectionGrid(downloadIndex, Array.ConvertAll(downloads, s => s.ToString()),
                    downloads.Length);
                Download.MaxDownloads = downloads[downloadIndex];
            }
        }
    }
}