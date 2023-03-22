using System.Collections.Generic;

namespace EMX.HierarchyPlugin.Editor
{
	class LibraryFolderManager
    {



        static Dictionary<string,bool> _libraryFields = new Dictionary<string, bool>();

        internal static string ReadLibraryFile( string fileName )
        {
            return Root.p[ 0 ].par_e.GET( fileName, "", ".Temp" );
            //if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library")) Directory.CreateDirectory(Folders.UNITY_SYSTEM_PATH + "Library");
            //if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "")) Directory.CreateDirectory(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "");
            //if (!File.Exists(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "/" + fileName)) return "";
            //return File.ReadAllText(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "/" + fileName);
        }

        internal static void WriteLibraryFile( string fileName, ref System.Text.StringBuilder content )
        {
            Root.p[ 0 ].par_e.SET( fileName, content.ToString(), ".Temp" );
            //if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library")) Directory.CreateDirectory(Folders.UNITY_SYSTEM_PATH + "Library");
            //if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "")) Directory.CreateDirectory(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "");
            //File.WriteAllText(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "/" + fileName, content.ToString());
        }

        //internal static void RemoveLibraryFile(string fileName)
        //{
        //	if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library")) return;
        //	if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "")) return;
        //	if (!File.Exists(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "/" + fileName)) return;
        //	File.Delete(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "/" + fileName);
        //}
    }
}
