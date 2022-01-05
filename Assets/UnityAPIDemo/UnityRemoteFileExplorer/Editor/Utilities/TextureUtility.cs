using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental;

namespace RemoteFileExplorer.Editor
{
    public class TextureUtility
    {
        public const string IconPath = "Packages/com.iwin.remotefileexplorer/Resources/Icons/";
        private static Dictionary<string, Texture2D> m_TextureCache = new Dictionary<string, Texture2D>();

        private static Texture2D m_DefaultTexture;

        public static Texture2D GetTexture(string key)
        {
            if(!EditorGUIUtility.isProSkin)
            {
                key = "d_" + key; 
            }
            return Internal_GetTexture(key);
        }

        private static Texture2D Internal_GetTexture(string key)
        {
            
            if (m_TextureCache.ContainsKey(key))
            {
                return m_TextureCache[key];
            }
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath + key + ".png");
            if (icon == null)
            {
                string prefix = "";
                if(key.StartsWith("d_"))
                    prefix = "d_";
                if(key.Contains("active"))
                {
                    key = "default active";
                }
                else if(key.Contains("inactive"))
                {
                    key = "default inactive";
                }
                else
                {
                    key = "default";
                }
                key = prefix + key;
                return Internal_GetTexture(key);
            }
            else
            {
                m_TextureCache.Add(key, icon);
            }
            return icon;
        }

        public static void SaveTextureToFile(string key)
        {
            Texture2D texture = GetBuiltinTexture(key);
            // blend
            texture = CloneToReadableTexture(texture);
            var bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(IconPath + key + ".png", bytes);
        }

        public static Texture2D GetBuiltinTexture(string key)
        {
            Texture2D icon = null;
            switch (key)
            {
                case "project":
                    icon = EditorGUIUtility.IconContent("Project").image as Texture2D;
                    break;
                case "folder":
                    icon = EditorGUIUtility.IconContent(EditorResources.folderIconName).image as Texture2D;
                    break;
                case "cs":
                    icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
                    break;
                case "shader":
                    icon = EditorGUIUtility.IconContent("shader Icon").image as Texture2D;
                    break;
                default:
                    icon = EditorReflection.InvokeStaticMethod<EditorGUIUtility>("FindTextureByType", typeof(TextAsset)) as Texture2D;
                    break;
            }
            return icon;
        }

        public static Texture2D GetActiveTexture(Texture texture)
        {

            Texture2D t = TextureUtility.CloneToReadableTexture(texture);
            Color color;
            ColorUtility.TryParseHtmlString("#91c9f7", out color);
            color.a = 0.2f;
            var target = t;
            Texture2D texture2D = new Texture2D(target.width, target.height);
            for (int y = 0; y < texture2D.height; y++)
            {
                for (int x = 0; x < texture2D.width; x++)
                {
                    Color c = target.GetPixel(x, y);
                    if (c.a != 0)
                    {
                        texture2D.SetPixel(x, y, (c * (1 - color.a)) + (color * color.a));
                    }
                    else
                    {
                        texture2D.SetPixel(x, y, c);
                    }
                }
            }
            texture2D.Apply();
            return texture2D;
        }

        public static Texture2D GetActiveTexture2(Texture2D target, Color color)
        {
            Texture2D texture2D = new Texture2D(target.width, target.height);
            for (int y = 0; y < texture2D.height; y++)
            {
                for (int x = 0; x < texture2D.width; x++)
                {
                    color.a = target.GetPixel(x, y).a;
                    texture2D.SetPixel(x, y, color);
                }
            }
            texture2D.Apply();
            return texture2D;
        }

        public static Texture2D CloneToReadableTexture(Texture texture)
        {
            Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
            Graphics.Blit(texture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;
        }
    }
}