using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace EMX.HierarchyPlugin.Editor.Mods
{





    internal class AutoSaveMod
    {


        static PluginInstance p { get { return Root.p[0]; } }


        static float AS_LAST_SAVE_TIME_IN_SEC {
            get { return SessionState.GetFloat("EMX_AS_LAST_SAVE_TIME_IN_SEC", -1); }
            set { SessionState.SetFloat("EMX_AS_LAST_SAVE_TIME_IN_SEC", value); }
        }
        static float AS_PLAY_LAUNCH_TIME {
            get { return SessionState.GetFloat("EMX_AS_PLAY_LAUNCH_TIME", -1); }
            set { SessionState.SetFloat("EMX_AS_PLAY_LAUNCH_TIME", value); }
        }

        static float lastSave {
            get { return AS_LAST_SAVE_TIME_IN_SEC; }
            set {
                if (AS_LAST_SAVE_TIME_IN_SEC != (value))
                    AS_LAST_SAVE_TIME_IN_SEC = (value);
            }
        }
        static float EDITOR_TIMER {
            get { return (float)(EditorApplication.timeSinceStartup % 1000000); }
        }





        internal static void Subscribe(EditorSubscriber sbs)
        {
            sbs.OnUpdate += UpdateCS;
        }

        public static string GET_PATTERN(string v, DateTime dt)
        {
            return GET_PATTERN(v, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }
        public static string GET_PATTERN(string file, int YYYY, int MM, int DD, int hh, int mm, int ss)
        {
            var p = Root.p[0].par_e.AS_FILES_PATTERN;

            p = p.Replace("SCENE", file);

            var y = YYYY.ToString();
            p = p.Replace("YYYY", YYYY.ToString("D4"));
            p = p.Replace("YYY", (((YYYY / 1000f) % 1) * 1000).ToString("000"));
            p = p.Replace("YY", (((YYYY / 100f) % 1) * 100).ToString("00"));
            p = p.Replace("Y", (((YYYY / 10f) % 1) * 10).ToString("0"));

            p = p.Replace("MM", MM.ToString("D2"));
            p = p.Replace("DD", DD.ToString("D2"));

            p = p.Replace("hh", hh.ToString("D2"));
            p = p.Replace("mm", mm.ToString("D2"));
            p = p.Replace("ss", ss.ToString("D2"));

            return p;

        }

        static string _dataPath = null;
        internal static string dataPath {
            get {
                return _dataPath ?? (_dataPath = System.IO.Directory.GetCurrentDirectory().Replace('\\', '/').TrimEnd('/') + "/Assets");
            }
        }


        static string autoSaveFileName(string source)
        {
            // get {
            if (!System.IO.Directory.Exists(dataPath + "/" + p.par_e.AS_LOCATION))
            {
                System.IO.Directory.CreateDirectory(dataPath + "/" + p.par_e.AS_LOCATION);
                AssetDatabase.Refresh();
            }
            //if (!AssetDatabase.IsValidFolder("Assets/" + AutoSaveFolder)) AssetDatabase.CreateFolder("Assets", AutoSaveFolder);

            var files = System.IO.Directory.GetFiles(dataPath + "/" + p.par_e.AS_LOCATION).Select(f => f.Replace('\\', '/')).Where(f =>
                f.EndsWith(".unity", StringComparison.OrdinalIgnoreCase) && f.Substring(f.LastIndexOf('/') + 1).StartsWith("AutoSave", StringComparison.OrdinalIgnoreCase)).ToArray();

            var as_loc = dataPath + "/" + p.par_e.AS_LOCATION + "/";
            var out_loc = "Assets/" + p.par_e.AS_LOCATION + "/";

            if (p.par_e.AS_FILES_STYLE == 0)
            {

                //var D = Mathf.Max( 2, p.par_e.AS_FILES_COUNT.ToString().Length);
                var D = p.par_e.AS_FILES_COUNT.ToString().Length;
                var D_string = Enumerable.Repeat(0, D).Select(s => s.ToString()).Aggregate((a, b) => a + b);

                if (files.Length == 0) return out_loc + "AutoSave_" + D_string + ".unity";


                DateTime? ta_max = null;
                string f_max = null;
                var _times = files.Select(f => {
                    var fa = f.Remove(f.LastIndexOf('.'));
                    DateTime ta = System.IO.File.GetLastWriteTime(f);
                    if (!ta_max.HasValue || ta_max < ta)
                    {
                        ta_max = ta;
                        f_max = f;
                    }
                    return new { f = fa, t = ta };
                }
                ).ToArray();

                Func<string, string> tryGet = (file) => {
                    var _ = file.LastIndexOf('_');
                    if (_ == -1 || _ == file.Length - 1) return null;
                    int count;
                    if (int.TryParse(file.Substring(_ + 1), out count)) //.TrimStart( '0' )
                    {
                        count = (count + 1) % p.par_e.AS_FILES_COUNT;
                        return out_loc + "AutoSave_" + count.ToString("D" + D.ToString()) + ".unity";
                    }
                    return null;
                };

                // attempt 1
                if (f_max != null)
                {
                    f_max = f_max.Remove(f_max.LastIndexOf('.'));
                    var res1 = tryGet(f_max);
                    if (res1 != null) return res1;
                }


                // attempt 2
                var times = files.Select(f => new { f = f.Remove(f.LastIndexOf('.')), t = System.IO.File.GetLastWriteTime(f) }).OrderBy(w => w.t).ToList();
                for (int ind = times.Count - 1; ind >= 0; ind--)
                {
                    var res = tryGet(times[ind].f);
                    if (res != null) return res;
                }
                // files = files.Select( n => n.Remove( n.LastIndexOf( '.' ) ) ).ToArray();

                return out_loc + "AutoSave_" + D_string + ".unity";
            }
            else
            {

                var dt = DateTime.Now;
                //Debug.Log( dt.Year+ " " + dt.Month+ " " + dt.Day+ " " + dt.Hour+ " " + dt.Minute+ " " + dt.Second );
                var res = "AutoSave" + AutoSaveMod.GET_PATTERN(source, dt);
                var shs = "";
                var hihi = 0;
                while (File.Exists(as_loc + res + shs + ".unity"))
                {
                    hihi++;
                    shs = " (" + hihi.ToString() + ")";
                }
                var outNewFileName = out_loc + res + shs + ".unity";
                var asNewFileName = as_loc + res + shs + ".unity";

                if (files.Length >= p.par_e.AS_FILES_COUNT)
                {
                    //var times = files.Select(f=>new {f = f.Remove( f.LastIndexOf( '.' ) ),t = System.IO.File.GetLastWriteTime(f) } ).ToArray();
                    //.OrderBy(w=>w.t).ToList();
                    DateTime? ta_max = null;
                    string f_max = null;
                    var _times = files.Select(f => {
                        // var fa = f.Remove( f.LastIndexOf( '.' ) );
                        var fa = f;
                        DateTime ta = System.IO.File.GetLastWriteTime(f);
                        if (!ta_max.HasValue || ta_max > ta)
                        {
                            ta_max = ta;
                            f_max = f;
                        }
                        return new { f = fa, t = ta };
                    }).ToArray();
                    if (f_max == null) throw new Exception("f_max == null");

                    var f_max_local = f_max.Replace('\\', '/').TrimEnd('/');

                    //    if ( !f_max_local.StartsWith( dataPath, StringComparison.OrdinalIgnoreCase ) ) throw new Exception( "f_max_local" );
                    if (!f_max_local.StartsWith(dataPath, StringComparison.OrdinalIgnoreCase)) throw new Exception("f_max_local | " + dataPath + " | " + f_max_local);
                    f_max_local = "Assets" + f_max_local.Substring(dataPath.Length);


                    // if ( !string.IsNullOrEmpty( AssetDatabase.MoveAsset( f_max_local, outNewFileName ) ) )
                    //if (AssetDatabase.DeleteAsset)
                    //
                    //
                    //if ( File.Exists( outNewFileName ) ) File.Delete( outNewFileName );
                    //File.Move( f_max_local, outNewFileName );
                    //if ( File.Exists( f_max_local + ".meta" ) )
                    //{
                    //    if ( File.Exists( outNewFileName + ".meta" ) ) File.Delete( outNewFileName + ".meta" );
                    //    File.Move( f_max_local + ".meta", outNewFileName + ".meta" );
                    //}
                    if (!string.IsNullOrEmpty(AssetDatabase.MoveAsset(f_max_local, outNewFileName)))
                    {
                        if (!AssetDatabase.DeleteAsset(f_max_local))
                        {
                            File.Delete(f_max);
                            if (File.Exists(f_max + ".meta")) File.Delete(f_max + ".meta");
                        }
                    }
                    else
                    {
                        AssetDatabase.ImportAsset(outNewFileName, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                    }
                }

                return outNewFileName;




            }



            // }
        }



        static float speeder = 0;

        public static void UpdateCS()
        {
            if (Application.isPlaying)
            {
                if (AS_PLAY_LAUNCH_TIME == -1) AS_PLAY_LAUNCH_TIME = EDITOR_TIMER;
                return;
            }

            if (AS_PLAY_LAUNCH_TIME != -1)
            {
                lastSave += (float)(EDITOR_TIMER - AS_PLAY_LAUNCH_TIME);
                AS_PLAY_LAUNCH_TIME = -1;
            }

            if (Mathf.Abs(speeder - EDITOR_TIMER) < 4) return;
            speeder = EDITOR_TIMER;

            if (Mathf.Abs(lastSave - (float)EDITOR_TIMER) >= p.par_e.AS_SAVE_INTERVAL_IN_SEC * 2)
            {
                lastSave = (float)EDITOR_TIMER;
                // resetSet();
            }

            //Debug.Log(lastSave + " : " +  (float)EDITOR_TIMER + "  : " +  p.par_e.AS_SAVE_INTERVAL_IN_SEC);
            if (Mathf.Abs(lastSave - (float)EDITOR_TIMER) >= p.par_e.AS_SAVE_INTERVAL_IN_SEC)
            {
                SaveScene();
            }
            //debug();
        }

        /*	static void debug()
            {
                var EDITOR_TIMER = 760f;
                var lastSave = 680f;
                var SSS = 35f;
                float dif = EDITOR_TIMER - lastSave - SSS;
                if (dif > 0)
                {
                    int interator = 0;
                    while (dif > 0)
                    {
                        lastSave = EDITOR_TIMER - dif;
                        dif = EDITOR_TIMER - lastSave - SSS;
                        interator++;
                        if (interator > 15)
                        {
                            lastSave = EDITOR_TIMER;
                            break;
                        }
                    }
                }
                Debug.Log(lastSave);
            }
            */
        public static string GET_SCENE_NAME()
        {
            var s = EditorSceneManager.GetActiveScene();
            if (!s.IsValid()) return "";
            var scenename = s.name;
            if (string.IsNullOrEmpty(scenename)) scenename = "untitled";
            return scenename + ".unity";
        }
        public static void SaveScene()
        {

            var s = EditorSceneManager.GetActiveScene();
            if (!s.IsValid()) return;
            var scenename = GET_SCENE_NAME();
            var fn = autoSaveFileName(scenename);

            //var relativeSavePath = "Assets/" + p.par_e.AS_LOCATION + "/";
            var interval = p.par_e.AS_SAVE_INTERVAL_IN_SEC;
            if (EditorSceneManager.SaveScene(s, fn, true))
            {
                //AssetDatabase.ImportAsset( fn, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport );
            }
            else
            {
                interval = 10;
            }
            var dif = (float)EDITOR_TIMER - lastSave - interval;
            if (dif > 0)
            {
                int interator = 0;
                while (dif > 0)
                {
                    lastSave = (float)EDITOR_TIMER - dif;
                    dif = (float)EDITOR_TIMER - lastSave - interval;
                    interator++;
                    if (interator > 15)
                    {
                        lastSave = (float)EDITOR_TIMER;
                        break;
                    }
                }
            }
            else lastSave = (float)EDITOR_TIMER;

            if (p.par_e.AS_LOG)
            {
                Debug.Log("Auto-Save Current Scene: " + fn
                    + '\n' +
                    lastSave + " : " + dif + " : " + interval + " : " + EDITOR_TIMER);
                if (interval == 10) Debug.LogWarning("Error save, second attempt");
            }
        }




    }
}
