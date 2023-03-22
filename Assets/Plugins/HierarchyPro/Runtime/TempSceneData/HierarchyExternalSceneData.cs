#if UNITY_EDITOR
//#define EMX_LOG_A


using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.Assertions.Must;


//This class using only for the current editor session and objects will not save to the scene asset. 
//Just that the Unity engine requires that the MonoBehaviour scripts places outside the editor folder, even it using only for editor.

namespace EMX.HierarchyPlugin.Editor
{
    public enum SaverType { ModFreezer = 0, ModDescription = 1, ModManualHighligher = 2, Bookmarks = 3, ModPlayKeeper = 4, SceneHierarchyExands = 5, ModManualIcons = 6, PresetsManager = 7 }

    [Serializable]
    public class SavedObjectData
    {


        public SavedObjectData(int id_in_external_heap) { this.id_in_external_heap = id_in_external_heap; }

        public int saverType;
        public int id_in_external_heap;

        public string indentifierVersion;
        public string identifierTypetargetObjectIdtargetPrefabId;
        public string namesPathBackup;
        public string siblingsPathBackup;
        /* public string globalid;
       public int identifierType;
        public string targetObjectId;
        public string targetPrefabId;*/




        public string GetGuidData()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(1.ToString());
            sb.AppendLine(saverType.ToString());
            sb.AppendLine(id_in_external_heap.ToString());
            sb.AppendLine(indentifierVersion);
            sb.AppendLine(identifierTypetargetObjectIdtargetPrefabId);
            sb.AppendLine(namesPathBackup);
            sb.AppendLine(siblingsPathBackup);
            return sb.ToString();
        }
        public void SetGuidData(string s)
        {
            StringReader sr = new StringReader(s);
            sr.ReadLine(); //version
            saverType = int.Parse(sr.ReadLine());
            id_in_external_heap = int.Parse(sr.ReadLine());

            indentifierVersion = sr.ReadLine();
            identifierTypetargetObjectIdtargetPrefabId = sr.ReadLine();
            namesPathBackup = sr.ReadLine();
            siblingsPathBackup = sr.ReadLine();
        }

        //DATA
        public bool boolValue;
        public string stringValue;
        public HighlighterExternalData[] highLighterData = new HighlighterExternalData[0];
        public IconExternalData[] iconData = new IconExternalData[0];
        public SavedObjectData Clone()
        {
            var res = (SavedObjectData)this.MemberwiseClone();
            if (res.highLighterData != null) res.highLighterData = res.highLighterData.Select(d => d.Clone()).ToArray();
            if (res.iconData != null) res.iconData = res.iconData.Select(d => d.Clone()).ToArray();
            return res;
        }
    }
    [Serializable]
    public class SavedObjectDataByType
    {
        public SavedObjectData[] data = new SavedObjectData[0];

        public SavedObjectDataByType Clone()
        {
            var res = (SavedObjectDataByType)MemberwiseClone();
            if (data != null) res.data = data.Select(d => d.Clone()).ToArray();
            return res;
        }
    }

    [Serializable]
    public class BookMarkCategory_Saved
    {

        public Color bgColor = Color.white;
        public string category_name = null;
        public List<HierExpands_Saved> buttons = new List<HierExpands_Saved>();

        public BookMarkCategory_Saved Clone()
        {
            var res = (BookMarkCategory_Saved)MemberwiseClone();
            if (buttons != null) res.buttons = buttons.Select(d => d.Clone()).ToList();
            return res;
        }
        /*	[SerializeField]
			int __dictionaryKey = 100;
			public int dictionaryKey
			{
				get { return Mathf.Clamp(__dictionaryKey, 100, 1000); }
				set
				{
					if (value < 100 || value > 1000) throw new Exception(value.ToString());
					__dictionaryKey = value;
				}
			}*/
    }
    [Serializable]
    public class ScenesTab_Saved
    {
        public bool pin;
        public string[] path;
        public string[] guid;

        public ScenesTab_Saved Clone()
        {
            var res = (ScenesTab_Saved)MemberwiseClone();
            //if ( guid != null ) res.guid = guid.Select( d => d.Clone() ).ToArray();
            //if ( path != null ) res.path = path.Select( d => d.Clone() ).ToArray();
            return res;
        }
    }

    [Serializable]
    public class HierExpands_Saved
    {
        public string name;
        public int[] ids_in_external_heap;

        public HierExpands_Saved Clone()
        {
            var res = (HierExpands_Saved)MemberwiseClone();
            // if ( ids_in_external_heap != null ) res.ids_in_external_heap = ids_in_external_heap.Select( d => d.Clone() ).ToArray();
            return res;
        }
    }

    public class HierarchyExternalSceneData : ScriptableObject
    {


        [SerializeField]
        public SavedObjectDataByType[] types = new SavedObjectDataByType[0];
        [SerializeField]
        public List<BookMarkCategory_Saved> BookMarks_InternalGlobal = new List<BookMarkCategory_Saved>();
        [SerializeField]
        public List<HierExpands_Saved> HierExpands_InternalGlobal = new List<HierExpands_Saved>();


        public void CopyFrom(HierarchyExternalSceneData copiedScriptableObject)
        {
            types = copiedScriptableObject.types.Select(t => t.Clone()).ToArray();
            BookMarks_InternalGlobal = copiedScriptableObject.BookMarks_InternalGlobal.Select(t => t.Clone()).ToList();
            HierExpands_InternalGlobal = copiedScriptableObject.HierExpands_InternalGlobal.Select(t => t.Clone()).ToList();
        }
        public void ClearData()
        {
            types = new SavedObjectDataByType[0];
            BookMarks_InternalGlobal = new List<BookMarkCategory_Saved>();
            HierExpands_InternalGlobal = new List<HierExpands_Saved>();
        }


        [NonSerialized]
        public static bool SkipSetDirty;


        // Dictionary<SaverType, SavedObjectData[]> _LoadObjects = new Dictionary<SaverType, SavedObjectData[]>
        public static SavedObjectData[] LoadObjects(SaverType type, Scene scene)
        {

            var file = GetHierarchyExternalSceneData(scene);
            /*if (type == SaverType.ModDescription)
			{
				Debug.Log("ASD");
			}*/
            var t = (int)type;
            if (file.types == null) file.types = new SavedObjectDataByType[0];
            if (t >= file.types.Length) Array.Resize(ref file.types, t + 1);
            if (file.types[t] == null) file.types[t] = new SavedObjectDataByType();
            var r = file.types[t];
            if (r.data == null) r.data = new SavedObjectData[0];
            return r.data;

        }
        public static void WriteObjects(SaverType type, Scene scene, SavedObjectData[] objects, bool saveOnSceneSaved = false)
        {
            //Debug.Log("ASD");
            //	if (!EditorSceneManager.EnsureUntitledSceneHasBeenSaved("External data, requires saving the scene before applying any changes")) return;
            var file = GetHierarchyExternalSceneData(scene);
            var t = (int)type;
            if (file.types == null) file.types = new SavedObjectDataByType[0];
            if (t >= file.types.Length) Array.Resize(ref file.types, t + 1);
            if (file.types[t] == null) file.types[t] = new SavedObjectDataByType();
            var r = file.types[t];
            r.data = objects;
            SetDirtyFile(file, saveOnSceneSaved);
            // if ( r.data == null ) r.data = new SavedObjectData[ 0 ];
            // return r.data;
        }

        public static HierarchyExternalSceneData GetHierarchyExternalSceneData(int scene)
        {
            return GetHierarchyExternalSceneData(Folders.GET_SCENE_BY_ID(scene));
        }
        public static HierarchyExternalSceneData GetHierarchyExternalSceneData(Scene scene)
        {


            //var scene_path = GetScenePath(scene);
            var result = GetProjectHash(scene);
            if (!result)
            {
                TryCreateBackupForCache(scene);
                // if (  )
                // {
                // AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
                // }
                //Debug.Log( scene.name );
                result = CreateInstance<HierarchyExternalSceneData>();
                var path = GetStoredDataPathInternal(scene);
                var folder = path.Remove(path.LastIndexOf('/'));
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                AssetDatabase.CreateAsset(result, path);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            }
            return result;
        }
        public static HierarchyExternalSceneData GetHierarchyExternalSceneData_ButNotCreateNew(Scene scene)
        {
            //var scene_path = GetScenePath(scene);
            return GetProjectHash(scene);
        }
        public static HierarchyExternalSceneData GetHierarchyExternalSceneData_ButNotCreateNew(int scene)
        {
            //var scene_path = GetScenePath(Folders.GET_SCENE_BY_ID(scene);
            return GetProjectHash(Folders.GET_SCENE_BY_ID(scene));
        }

        public static void Undo(Scene scene, string v)
        {
            //EMX_TODO - add cache cleaner for undo events
            //	/var file = GetHierarchyExternalSceneData(scene);
            //UnityEditor.Undo.RecordObject(file, v);
        }
        public static void SetDirtyFile(HierarchyExternalSceneData file, bool saveOnSceneSaved = false)
        {
            if (SkipSetDirty) return;
            UnityEditor.EditorUtility.SetDirty(file);
            if (saveOnSceneSaved) SaveAssets(file);
            //Debug.Log("ASD");
        }
        //public static void SetDirtyFile( Scene scene )
        //{
        //	if ( SkipSetDirty ) return;
        //    HierarchyExternalSceneData file = GetHierarchyExternalSceneData(scene);
        //	UnityEditor.EditorUtility.SetDirty( file );
        //	SaveAssets(file);
        //	//	Debug.Log("ASD");
        //}
        public static void SaveAssets(UnityEngine.Object file, bool skip = false)
        {
            if (SkipSetDirty || skip)
            {
                var count = SessionState.GetInt("EM.HP.SaveAssetsCount", 0);
                SessionState.SetInt("EM.HP.AssetIndex-" + count.ToString(), file.GetInstanceID());
                count++;
                SessionState.SetInt("EM.HP.SaveAssetsCount", count);
                return;
            }
            //EMX_TODO SaveAssets
            //if (!Application.isPlaying && !SceneManager.GetActiveScene().isDirty) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            {
                var count = SessionState.GetInt("EM.HP.SaveAssetsCount", 0);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var ass = EditorUtility.InstanceIDToObject(SessionState.GetInt("EM.HP.AssetIndex-" + count.ToString(), -1)) as UnityEngine.Object;
                        if (ass) SaveAssetIfDirty(ass);
                    }
                    SessionState.SetInt("EM.HP.SaveAssetsCount", 0);
                }
            }


            SaveAssetIfDirty(file);
            SaveAssetIfDirtyFin();
            //Debug.Log(AssetDatabase.GetAssetPath(file));
            //AssetDatabase.SaveAssets();
            //Debug.Log("ASD");
        }

        static void SaveAssetIfDirty(UnityEngine.Object file)
        {
#if UNITY_2020_3_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(file);
#else
            
#endif
        }

        static void SaveAssetIfDirtyFin()
        {
#if UNITY_2020_3_OR_NEWER
#else
            AssetDatabase.SaveAssets();
#endif
        }

        public static bool TryCreateBackupForCache(Scene s)
        {
            var sys = Folders.UNITY_SYSTEM_PATH;
            if (!sys.EndsWith("/")) sys += '/';
            var p = GetStoredDataPathInternal(s);
            return TryCreateBackupForCache_ExternalCachePathInput(sys + p);
        }

        public static bool TryCreateBackupForCache_ExternalCachePathInput(string externapPth)
        {

            try
            {
#if EMX_LOG_A
				Debug.Log( "TryCreateBackupForCache_ExternalCachePathInput " + externapPth );
#endif
                var oldName = externapPth;
                bool reload = false;
                if (System.IO.File.Exists(oldName + ".backup"))
                {
                    System.IO.File.Copy(oldName + ".backup", oldName);
                    reload = true;
                }

                if (System.IO.File.Exists(oldName + ".meta" + ".backup"))
                {
                    System.IO.File.Copy(oldName + ".meta" + ".backup", oldName + ".meta");
                    reload = true;
                }

                if (reload)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Cannot create backup for: " + externapPth + "\n\n" + ex.Message + "\n" + ex.StackTrace);
            }


            return false;
        }
        // const string FOLDER = "/Editor/SavedData/ScenesData/";
        //const string SCENED_DATA_FOLDER = "/Editor/_SAVED_DATA/_SCENES/";
        //string SCENED_DATA_FOLDER { get {return Root.p[0].o\ } } "/Editor/_SAVED_DATA/_SCENES/";
        public static HierarchyExternalSceneData GetProjectHash(Scene s)
        //public static HierarchyExternalSceneData GetProjectHash( ref string scene_path )
        {
            // var path = Folders.PluginInternalFolder + SCENED_DATA_FOLDER + scene_path.Remove(scene_path.LastIndexOf('.')) + ".asset";
            var path = GetStoredDataPathInternal(s);
            //var path = d + scene_path.Remove( scene_path.LastIndexOf( '.' ) ) + ".asset";
            return AssetDatabase.LoadAssetAtPath<HierarchyExternalSceneData>(path);
        }

        public const string DATA_SUB_FOLDER = "_SCENES";
        public static string d { get { return Folders.CALC_SCENESDATA_PATH_INTERNAL + "/" + DATA_SUB_FOLDER + "/"; } }
        //string d_internal { get { return Folders.PluginExternalFolder + "/Editor/_SAVED_DATA/.EditorSettings/"; } }



        //static public string GetScenePath( Scene s )
        //{
        //	var p = s.path;
        //
        //	if ( string.IsNullOrEmpty( p ) ) return "untitled.unity";
        //
        //	return p;
        //}
        public static string GetStoredDataPathInternal(Scene s)
        {
            //return Folders.PluginInternalFolder + SCENED_DATA_FOLDER + GetScenePath( s ).Remove( GetScenePath( s ).LastIndexOf( '.' ) ) + ".asset";
            var scene_path = s.path;
            if (string.IsNullOrEmpty(scene_path)) scene_path = "untitled.unity";

            string path;
            var dot_i = scene_path.LastIndexOf('.');
            var s_i = scene_path.LastIndexOf('/');
            if (dot_i <= s_i) path = scene_path + ".asset";
            else path = scene_path.Remove(scene_path.LastIndexOf('.')) + ".asset";

#if EMX_LOG_A
			Debug.Log( "GetStoredDataPathInternal " + d + path );
#endif

            return d + path;
        }





        //public static string GetStoredDataPathExternal( Scene s )
        //{
        //	var path = Folders.UNITY_SYSTEM_PATH;
        //
        //	if ( !path.EndsWith( "/" ) ) path += '/';
        //
        //	return path + GetStoredDataPathInternal( s );
        //}



    }



}

#endif