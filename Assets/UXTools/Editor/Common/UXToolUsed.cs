using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ThunderFireUITool
{
    // [InitializeOnLoad]
    public class UXToolUsed
    {
        // static UXToolUsed()
        // {
        //     var used = EditorPrefs.GetBool("UXToolUsed", true);
        //     if (!used) return;
        //     EditorPrefs.SetBool("UXToolUsed", false);
        //     EditorApplication.update += InitFunction;
        // }

        [Serializable]
        public class PostData
        {
            public string content = "";
        }

        public static void InitUXToolUsed()
        {
            Debug.Log("Thanks for using our tool!");
            Upload();

            // EditorApplication.update -= InitFunction;
        }

        private static void Upload()
        {
            var postData = new PostData();
            var data = JsonUtility.ToJson(postData);

            UnityWebRequest www =
                UnityWebRequest.Post("https://uxtool.netease.com/uxtool/api/collect", data);

            www.SendWebRequest();
        }
    }
}