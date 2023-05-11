#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.U2D;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TF_TableList;

namespace ThunderFireUITool
{
    public enum ShowType
    {
        Atlas,
        Image
    }
    public class UIResCheckTool : OdinEditorWindow
    {
        private static ShowType showType = ShowType.Atlas;

        [ShowIf("showType", ShowType.Atlas), ReadOnly]
        [ShowInInspector]
        public static List<AtlasData> atlasDatas = new List<AtlasData>();

        [ShowIf("showType", ShowType.Image)]
        [ShowInInspector]
        [TF_TableList(AlwaysExpanded = true)]
        public static List<SingleImageData> imageDatas = new List<SingleImageData>();

        [System.Serializable]
        public class AtlasData
        {
            [LabelText("Atlas"), ReadOnly]
            public UnityEngine.Object altas = null;
            [HideInInspector]
            public AtlasResolutionItem resolution;
            [LabelText("Atlas分辨率"), ReadOnly]
            public string resolutionString;

            [LabelText("Atlas填充率"), ReadOnly]
            [ProgressBar(0, 1)]
            public float fillRate;

            [HideInInspector]
            public WasteMemoryItem wasteMemory = new WasteMemoryItem();
            [LabelText("浪费内存"), ReadOnly]
            public string wasteMemoryString;

            [LabelText("超过最大尺寸"), ReadOnly]
            public bool isOverMaxSize = false;

            [TF_TableList(AlwaysExpanded = true)]
            [LabelText("详情")]
            public List<AtlasImageData> Images = new List<AtlasImageData>();
        }

        [System.Serializable]
        public class AtlasImageData
        {
            [TF_TableListColumnName("图片"), ReadOnly]
            public Texture Texture = null;
            [TF_TableListColumnName("图片尺寸"), ReadOnly]
            public AtlasResolutionItem resolution;
            [HideInInspector]
            public bool inRule;
            [TF_TableListColumnName("图片尺寸是否合规"), ReadOnly]
            [GUIColor("GetRuleColor")]
            public string inRuleString;

            private Color GetRuleColor()
            {
                if (inRule) return UIResCheckTool.ruleColor;
                else return UIResCheckTool.unRuleColor;
            }

            public AtlasImageData()
            {

            }

            public AtlasImageData(Texture tex, AtlasResolutionItem item, bool rule)
            {
                Texture = tex;
                resolution = item;
                inRule = rule;
                inRuleString = inRule ? ruleString : unRuleString;
            }
        }
        [System.Serializable]
        public class SingleImageData
        {
            [TF_TableListColumnName("图片"), ReadOnly]
            public Texture texture = null;
            [TF_TableListColumnName("图片尺寸"), ReadOnly]
            public AtlasResolutionItem resolution;
            [HideInInspector]
            public bool inRule;
            [TF_TableListColumnName("图片尺寸是否合规"), ReadOnly]
            [GUIColor("GetRuleColor")]
            public string inRuleString;

            private Color GetRuleColor()
            {
                if (inRule) return UIResCheckTool.ruleColor;
                else return UIResCheckTool.unRuleColor;
            }
        }

        private void Clear()
        {
            atlasDatas.Clear();
            imageDatas.Clear();
        }

        [MenuItem("ThunderFireUXTool/静态资源检查(UIResCheck)")]
        public static UIResCheckTool CheckAtlasButton()
        {
            UIResCheckTool window = (UIResCheckTool)EditorWindow.GetWindow(typeof(UIResCheckTool));
            window.titleContent = new GUIContent("Prefab 检查结果");
            window.minSize = new Vector2(400f, 800f);
            window.Show();
            return window;
        }

        //private static string greenColorFlag = "<color=#BEEB9C>";
        //private static string redColorFlag = "<color=#E05B5B>";
        //private static string ColorFlagEnd = "</color>";
        private static string atlasCountString = "Atlas数量:   ";
        private static string imageCountString = "Image数量:   ";
        private static GUIStyle countStyle;
        private static string ruleString = "合规";
        private static string unRuleString = "不合规";

        private static Color ruleColor = new Color();
        private static Color unRuleColor = new Color();

        protected override void OnEnable()
        {
            ColorUtility.TryParseHtmlString("#BEEB9C", out ruleColor);
            ColorUtility.TryParseHtmlString("#E05B5B", out unRuleColor);
        }

        protected override void OnGUI()
        {

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("图集检查", GUILayout.Height(50)))
            {
                CheckAtlas();
            }
            if (GUILayout.Button("图片检查", GUILayout.Height(50)))
            {
                CheckImage();
            }
            EditorGUILayout.EndHorizontal();

            if (showType == ShowType.Atlas)
            {
                EditorGUILayout.LabelField(atlasCountString + atlasDatas.Count);
            }
            else
            {
                EditorGUILayout.LabelField(imageCountString + imageDatas.Count);
            }

            base.OnGUI();
        }

        private static void CheckAtlas()
        {
            showType = ShowType.Atlas;
            atlasDatas.Clear();

            var checkAtlasSettings = AssetDatabase.LoadAssetAtPath<UIAtlasCheckRuleSettings>(ThunderFireUIToolConfig.UICheckSettingFullPath);
            string atlasFolderPath = checkAtlasSettings.atlasFolderPath;

            string[] guids = AssetDatabase.FindAssets("t:spriteatlas", new string[] { atlasFolderPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);

                if (atlas != null)
                {
                    SpriteAltasFillingInfo AltasFillingInfo = GetAltasFillingInfo(atlas);
                    var atlasData = new AtlasData()
                    {
                        altas = atlas,
                        resolution = AltasFillingInfo.atlasResolution,
                        resolutionString = AltasFillingInfo.atlasResolution.toString,
                        fillRate = AltasFillingInfo.realFillingRate,
                        wasteMemory = AltasFillingInfo.wasteMemory,
                        wasteMemoryString = AltasFillingInfo.wasteMemory.toString,
                        isOverMaxSize = AltasFillingInfo.realFillingRate > 1.0 ? true : false,
                        Images = new List<AtlasImageData>()
                    };

                    Sprite[] sprites = new Sprite[atlas.spriteCount];
                    atlas.GetSprites(sprites);
                    foreach (Sprite sp in sprites)
                    {
                        bool inrule = GetAtlasImageIsInRule(sp.texture);
                        var sln = new AtlasResolutionItem();
                        sln.Add(sp.texture.width, sp.texture.height);
                        AtlasImageData data = new AtlasImageData()
                        {
                            Texture = sp.texture,
                            resolution = sln,
                            inRule = inrule,
                            inRuleString = inrule ? ruleString : unRuleString
                        };
                        atlasData.Images.Add(data);
                    }

                    atlasDatas.Add(atlasData);
                }
            }
        }

        private static void CheckImage()
        {
            showType = ShowType.Image;
            imageDatas.Clear();

            var checkAtlasSettings = AssetDatabase.LoadAssetAtPath<UIAtlasCheckRuleSettings>(ThunderFireUIToolConfig.UICheckSettingFullPath);
            string imageFolderPath = checkAtlasSettings.imageFolderPath;

            string[] guids = AssetDatabase.FindAssets("t:sprite", new string[] { imageFolderPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sp != null)
                {
                    bool inrule = GetImageIsInRule(sp.texture);
                    var sln = new AtlasResolutionItem();
                    sln.Add(sp.texture.width, sp.texture.height);
                    var imgData = new SingleImageData()
                    {
                        texture = sp.texture,
                        resolution = sln,
                        inRule = inrule,
                        inRuleString = inrule ? ruleString : unRuleString
                    };
                    imageDatas.Add(imgData);
                }
            }
        }

        public static SpriteAltasFillingInfo GetAltasFillingInfo(SpriteAtlas atlas)
        {
            var sprites = new Sprite[atlas.spriteCount];
            atlas.GetSprites(sprites);
            var getPreviewTextureMI = typeof(SpriteAtlasExtensions).GetMethod("GetPreviewTextures", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            Texture2D[] atlasTextures = (Texture2D[])getPreviewTextureMI.Invoke(null, new System.Object[] { atlas });
            int altasArea = 0;
            float realArea = 0;
            SpriteAltasFillingInfo spriteAtlasFillingInfo = new SpriteAltasFillingInfo();
            foreach (var texture in atlasTextures)
            {
                altasArea += texture.width * texture.height;
                spriteAtlasFillingInfo.atlasResolution.Add(texture.width, texture.height);
            }

            foreach (var sprite in sprites)
            {
                Texture2D texture = sprite.texture;
                string texturePath = AssetDatabase.GetAssetPath(texture.GetInstanceID());

                Texture2D textureForRead = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false, false);
                ImageConversion.LoadImage(textureForRead, File.ReadAllBytes(texturePath));
                var boundingBox = GetTexture2dBoundingBox(textureForRead);
                realArea += GetBoundingBoxArea(boundingBox.top, boundingBox.bottom, boundingBox.left, boundingBox.right);
            }

            spriteAtlasFillingInfo.realFillingRate = realArea / altasArea;
            spriteAtlasFillingInfo.altas = atlas;
            spriteAtlasFillingInfo.wasteMemory.SetVal((altasArea - realArea) / 1024);

            return spriteAtlasFillingInfo;
        }
        private static (int top, int bottom, int left, int right) GetTexture2dBoundingBox(Texture2D texture)
        {
            int top = texture.height - 1, bottom = 0, left = 0, right = texture.width - 1;
            Color[] pixels = texture.GetPixels();
            //x表示行,y表示列
            //pixels[x * texture.width + y].a

            //图片下边界
            {
                //非均匀对边界进行采样
                int mid = texture.width / 2;
                int p = 0, q = texture.height - 1;
                while (p < q && Mathf.Approximately(pixels[p * texture.width + mid].a, 0))
                {
                    p++;
                }
                q = p;
                int k = mid - 1;
                int f = 1;
                while (k >= 0)
                {
                    p = 0;
                    while (p < q && Mathf.Approximately(pixels[p * texture.width + k].a, 0))
                    {
                        p++;
                    }
                    q = p;
                    if (q == 0)
                        break;
                    k = mid - (1 << f);
                    f++;
                }
                k = mid + 1;
                f = 1;
                while (k < texture.width)
                {
                    p = 0;
                    while (p < q && Mathf.Approximately(pixels[p * texture.width + k].a, 0))
                    {
                        p++;
                    }
                    q = p;
                    if (q == 0)
                        break;
                    k = mid + (1 << f);
                    f++;
                }
                p = 0;
                while (p < q)
                {
                    mid = (p + q) >> 1;
                    float aSum = 0;
                    for (int i = 0; i < texture.width; i++)
                    {
                        aSum += pixels[mid * texture.width + i].a;
                    }
                    if (Mathf.Approximately(aSum, 0))
                    {
                        p = mid + 1;
                    }
                    else
                    {
                        q = mid;
                    }
                }
                bottom = p;
            }
            //图片上边界
            {
                //非均匀对边界进行采样
                int mid = texture.width / 2;
                int p = texture.height - 1, q = 0;
                while (p > q && Mathf.Approximately(pixels[p * texture.width + mid].a, 0))
                {
                    p--;
                }
                q = p;
                int k = mid - 1;
                int f = 1;
                while (k >= 0)
                {
                    p = texture.height - 1;
                    while (p > q && Mathf.Approximately(pixels[p * texture.width + k].a, 0))
                    {
                        p--;
                    }
                    q = p;
                    if (q == texture.height - 1)
                        break;
                    k = mid - (1 << f);
                    f++;
                }
                k = mid + 1;
                f = 1;
                while (k < texture.width)
                {
                    p = texture.height - 1;
                    while (p > q && Mathf.Approximately(pixels[p * texture.width + k].a, 0))
                    {
                        p--;
                    }
                    q = p;
                    if (q == texture.height - 1)
                        break;
                    k = mid + (1 << f);
                    f++;
                }
                p = texture.height - 1;
                while (p > q)
                {
                    mid = ((p + q) >> 1) + ((p ^ q) & 1);
                    float aSum = 0;
                    for (int i = 0; i < texture.width; i++)
                    {
                        aSum += pixels[mid * texture.width + i].a;
                    }
                    if (Mathf.Approximately(aSum, 0))
                    {
                        p = mid - 1;
                    }
                    else
                    {
                        q = mid;
                    }
                }
                top = p;
            }
            //图片左边界
            {
                //非均匀对边界进行采样
                int mid = texture.height / 2;
                int p = 0, q = texture.width - 1;
                while (p < q && Mathf.Approximately(pixels[mid * texture.width + p].a, 0))
                {
                    p++;
                }
                q = p;
                int k = mid - 1;
                int f = 1;
                while (k >= 0)
                {
                    p = 0;
                    while (p < q && Mathf.Approximately(pixels[k * texture.width + p].a, 0))
                    {
                        p++;
                    }
                    q = p;
                    if (q == 0)
                        break;
                    k = mid - (1 << f);
                    f++;
                }
                k = mid + 1;
                f = 1;
                while (k < texture.height)
                {
                    p = 0;
                    while (p < q && Mathf.Approximately(pixels[k * texture.width + p].a, 0))
                    {
                        p++;
                    }
                    q = p;
                    if (q == 0)
                        break;
                    k = mid + (1 << f);
                    f++;
                }
                p = 0;
                while (p < q)
                {
                    mid = (p + q) >> 1;
                    float aSum = 0;
                    for (int i = bottom; i <= top; i++)
                    {
                        aSum += pixels[i * texture.width + mid].a;
                    }
                    if (Mathf.Approximately(aSum, 0))
                    {
                        p = mid + 1;
                    }
                    else
                    {
                        q = mid;
                    }
                }
                left = p;
            }
            //图片右边界
            {
                //非均匀对边界进行采样
                int mid = texture.height / 2;
                int p = texture.width - 1, q = 0;
                while (p > q && Mathf.Approximately(pixels[mid * texture.width + p].a, 0))
                {
                    p--;
                }
                q = p;
                int k = mid - 1;
                int f = 1;
                while (k >= 0)
                {
                    p = texture.width - 1;
                    while (p > q && Mathf.Approximately(pixels[k * texture.width + p].a, 0))
                    {
                        p--;
                    }
                    q = p;
                    if (q == texture.width - 1)
                        break;
                    k = mid - (1 << f);
                    f++;
                }
                k = mid + 1;
                f = 1;
                while (k < texture.height)
                {
                    p = texture.width - 1;
                    while (p > q && Mathf.Approximately(pixels[k * texture.width + p].a, 0))
                    {
                        p--;
                    }
                    q = p;
                    if (q == texture.width - 1)
                        break;
                    k = mid + (1 << f);
                    f++;
                }
                p = texture.width - 1;
                while (p > q)
                {
                    mid = ((p + q) >> 1) + ((p ^ q) & 1);
                    float aSum = 0;
                    for (int i = bottom; i <= top; i++)
                    {
                        aSum += pixels[i * texture.width + mid].a;
                    }
                    if (Mathf.Approximately(aSum, 0))
                    {
                        p = mid - 1;
                    }
                    else
                    {
                        q = mid;
                    }
                }
                right = p;
            }
            return (top, bottom, left, right);
        }
        private static int GetBoundingBoxArea(int top, int bottom, int left, int right)
        {
            return (top - bottom + 1) * (right - left + 1);
        }

        #region CheckRule
        public static bool GetImageIsInRule(Texture2D tex)
        {
            bool r = Is4Multiple(tex);
            return r;
        }
        public static bool GetAtlasImageIsInRule(Texture2D tex)
        {
            bool r1 = IsUnderMaxSize(tex);
            return r1;
        }
        private static bool Is4Multiple(Texture2D tex)
        {
            return (tex.width & 3) == 0 && (tex.height & 3) == 0;
        }
        private static bool Is2Power(Texture2D tex)
        {
            if (tex.width <= 1 || tex.height <= 1) return false;
            return (tex.width & tex.width - 1) == 0 && (tex.height & tex.height - 1) == 0;
        }
        private static bool IsUnderMaxSize(Texture2D tex)
        {
            return tex.width <= 256 && tex.height <= 256;
        }
        #endregion
    }
}
#endif