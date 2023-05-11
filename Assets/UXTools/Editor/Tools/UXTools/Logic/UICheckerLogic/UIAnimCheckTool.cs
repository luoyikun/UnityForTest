#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace ThunderFireUITool
{
    public class UIAnimCheckTool
    {
        [MenuItem("ThunderFireUXTool/动画资源检查(UIAnimCheck)")]
        private static void BeginAnimCheck()
        {
            string animFolderPath = AssetDatabase.LoadAssetAtPath<UIAtlasCheckRuleSettings>(ThunderFireUIToolConfig.UICheckSettingFullPath)?.animFolderPath;
            if(animFolderPath == null || !Directory.Exists(animFolderPath)) return;
            string[] files = Directory.GetFiles(animFolderPath, "*.anim", SearchOption.AllDirectories);
            foreach(string file in files)
            {
                UIAnimInfo animInfo = new UIAnimInfo();
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(file);
                animInfo.clipName = clip.name;
                animInfo.clipLength = clip.length;
                HashSet<float> frames = new HashSet<float>();
                EditorCurveBinding[] floatCurves = AnimationUtility.GetCurveBindings(clip);
                foreach(var floatCurve in floatCurves)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, floatCurve);
                    foreach(Keyframe frame in curve.keys)
                    {
                        frames.Add(frame.time);
                    }
                }
                EditorCurveBinding[] objRefCurves = AnimationUtility.GetObjectReferenceCurveBindings(clip);
                foreach(var objRefCurve in objRefCurves)
                {
                    ObjectReferenceKeyframe[] curveFrames = AnimationUtility.GetObjectReferenceCurve(clip, objRefCurve);
                    foreach(var frame in curveFrames)
                    {
                        frames.Add(frame.time);
                    }
                }
                animInfo.totalCurvesCount = floatCurves.Length + objRefCurves.Length;
                animInfo.keyframesCount = frames.Count;
                Debug.Log(JsonUtility.ToJson(animInfo));
            }
        }
    }
}
#endif