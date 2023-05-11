using System;
using System.Collections.Generic;
using ThunderFireUnityEx;

namespace UnityEngine.UI
{
    public class UXText : Text, ILocalizationText
    {
        [SerializeField]
        private LocalizationHelper.TextLocalizationType m_localizationType = LocalizationHelper.TextLocalizationType.RuntimeUse;
        public LocalizationHelper.TextLocalizationType localizationType { get { return m_localizationType; } set { m_localizationType = value; } }

        // 记录一下未提取的RuntimeUse类型的原文,方便切换语言时显示前缀
        private string originText;
        [SerializeField]
        private bool m_ignoreLocalization = true;
        public bool ignoreLocalization { get { return m_ignoreLocalization; } set { m_ignoreLocalization = value; } }
        private UXTextLocaleProcesser localeProcesser;


        public bool UseTashkeel = false;
        public bool UseHinduNumber = false;
        public bool EnableArabicFix = false;
        public bool useParagraphShrinkFit = true;

        /// <summary>
        /// 需要被渲染的文字,已经经过Localization,还没经过替换省略号
        /// </summary>
        private string RenderedText { get; set; } = null;

        public bool ellipsisOnRight = true;
        private const string ELLIPSIS = "…";
        /// <summary>
        /// 是否有超框文字
        /// </summary>
        private bool HasEllipsis { get; set; } = false;
        /// <summary>
        /// 本地化用到的ID
        /// </summary>
        [SerializeField]
        private string m_localizationID = "";
        public string localizationID { get { return m_localizationID; } }
        [SerializeField]
        private string m_previewID = "";
        public string previewID
        {
            get { return m_previewID; }
            set {
                if(m_previewID != value)
                {
                    m_previewID = value;
                    ChangeLanguage(LocalizationHelper.GetLanguge());
                }
            }
        }
        private static readonly string need_replace = "UNFILLED TEXT";

        public System.Action<UXText> OnTextWidthChanged;
        public override string text
        {
            get
            {
                return base.text;
            }
            set
            {
#if UNITY_EDITOR
                // 先移除报错，因为有各种需要Layout， ContentSizeFilter的情况，还是要把本地化后的文本，赋值给text
                // 报错出来 Runtime类型就不应该跑
                // if (GlobalDebugRecord.enableLXTextRuntimeDetection && Application.isPlaying && this.localizationType == TextLocalizationType.RuntimeUse)
                // {
                // 	Debug.LogError($"[{Leihuo.LXTransformUtils.GetChildNodeFullPath(null, this.gameObject.transform)}] LXText类型是Runtime，不应该运行时改 {this.gameObject}");
                // }
#endif
                base.text = value;
            }
        }

        readonly UIVertex[] m_TempVerts = new UIVertex[4];

        private LXRolling rolling;
        protected override void Awake()
        {
            base.Awake();
            rolling = GetComponentInParent<LXRolling>();
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            #region UX Custom
            // Localization/BestFit排版/超框处理 
            CustomRefreshRenderedText();
            cachedTextGenerator.PopulateWithErrors(RenderedText, customGenerationSettings, gameObject);
            #endregion

            #region Text
            // Text.OnPopulateMesh()的原实现，有变动需要同步过来
            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            int vertCount = verts.Count;

            // We have no verts to process just return (case 1037923)
            if (vertCount <= 0)
            {
                toFill.Clear();
                return;
            }

            Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
            toFill.Clear();

            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }

            m_DisableFontTextureRebuiltCallback = false;
            #endregion
        }

        public TextGenerationSettings customGenerationSettings;
        public TextGenerationSettings CustomGenerationSettings { get => customGenerationSettings; }


        public void CustomModifyLocalizationSettings()
        {

        }
        public void CustomRefreshRenderedText()
        {
            UnityEngine.Profiling.Profiler.BeginSample("LXText.CustomRefreshRenderedText");

            // 所有PopulateWithErrors都使用如下调用来完成，已确保之前的修改都应用上了
            // cachedTextGenerator.PopulateWithErrors(RenderedText, customGenerationSettings, this.gameObject);

            PrepareCustomTextRender();

            DoShrinkFit();
            // 如果文本超框 末尾显示省略号
            ReplaceOutOfAreaTextWithEllipsis();
            UnityEngine.Profiling.Profiler.EndSample();
        }
        private void PrepareCustomTextRender()
        {
            UnityEngine.Profiling.Profiler.BeginSample("LXText.PrepareCustomTextRender");

            // 初始化localeProcesser 各个语种实现各自的processer
            if (localeProcesser == null || localeProcesser.LocalizationType != UXGUIConfig.CurLocalizationType)
            {
                localeProcesser = LocaleProcesserFactory.Create(this);
            }

            //修改Localization 时Text参数
            localeProcesser.ModifyLocaleTextSettings();

            // 初始化customGenerationSettings
            customGenerationSettings = GetGenerationSettings(rectTransform.rect.size);

            // 初始化RenderedText
            string needRenderderTxt = null;
            if (Application.isPlaying)
            {
                //现在运行时本地化切换语言后，会更新this.text，所以不需要再读一遍本地化文本了
                needRenderderTxt = this.text;
            }
            else
            {
#if UNITY_EDITOR
                needRenderderTxt = localeProcesser.GenLocaleRenderedString(text);
#endif
            }
            this.RenderedText = needRenderderTxt;

            DisplayTextPreferredWidth = GetDisplayTextWidth(RenderedText);
            UnityEngine.Profiling.Profiler.EndSample();
        }

        /// <summary>
        /// process for best fit
        /// </summary>
        private void DoShrinkFit()
        {
            UnityEngine.Profiling.Profiler.BeginSample("LXText.DoShrinkFit");
            if (this.UseParagraphShrinkFit())
            {
                customGenerationSettings.resizeTextForBestFit = false;

                int minSize = resizeTextMinSize;
                int txtLen = this.RenderedText.Length;
                for (int i = resizeTextMaxSize; i >= minSize; --i)
                {
                    customGenerationSettings.fontSize = i;
                    cachedTextGenerator.PopulateWithErrors(this.RenderedText, customGenerationSettings, this.gameObject);
                    if (cachedTextGenerator.characterCountVisible >= txtLen) break;
                }
            }
            else
            {
                cachedTextGenerator.PopulateWithErrors(this.RenderedText, customGenerationSettings, this.gameObject);
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }
        public bool UseParagraphShrinkFit()
        {
            return this.resizeTextForBestFit
                && this.useParagraphShrinkFit
                && this.horizontalOverflow == HorizontalWrapMode.Wrap;
        }

        /// <summary>
        /// process for ...
        /// </summary>
        private void ReplaceOutOfAreaTextWithEllipsis()
        {
            var needRenderderTxt = this.RenderedText;
            var characterCountVisible = cachedTextGenerator.characterCountVisible;

            UnityEngine.Profiling.Profiler.BeginSample("LXText.ReplaceOutOfAreaTextWithEllipsis");
            //滚动文字就不用省略号
            HasEllipsis = rolling == null && needRenderderTxt.Length > characterCountVisible && characterCountVisible > 0;
            if (HasEllipsis)
            {
                if (Application.isPlaying)
                {
                    Debug.Log($"有lxtext组件文字超框,path:{this.transform.PathFromRoot()}, content:{needRenderderTxt.Substring(0, Mathf.Min(needRenderderTxt.Length, 10))}");
                }
                if (this.ellipsisOnRight)
                {
                    RenderedText = needRenderderTxt.Substring(0, characterCountVisible - 1) + ELLIPSIS;
                }
                else
                {
                    RenderedText = ELLIPSIS + needRenderderTxt.Substring(needRenderderTxt.Length - characterCountVisible - 1);
                }
                cachedTextGenerator.PopulateWithErrors(RenderedText, customGenerationSettings, this.gameObject);
                characterCountVisible = cachedTextGenerator.characterCountVisible;
                while (RenderedText.Length > 1 && RenderedText.Length > characterCountVisible)
                {
                    var needLen = RenderedText.Length - 2;
                    if (this.ellipsisOnRight)
                        RenderedText = needRenderderTxt.Substring(0, needLen) + ELLIPSIS;
                    else
                        RenderedText = ELLIPSIS + needRenderderTxt.Substring(needRenderderTxt.Length - needLen);
                    cachedTextGenerator.PopulateWithErrors(RenderedText, customGenerationSettings, this.gameObject);
                    characterCountVisible = cachedTextGenerator.characterCountVisible;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        float displayTextPreferredWidth = -1;
        public float DisplayTextPreferredWidth
        {
            get
            {
                if (displayTextPreferredWidth < 0) return preferredWidth;
                return displayTextPreferredWidth;
            }
            private set
            {
                displayTextPreferredWidth = value;
                if (OnTextWidthChanged != null)
                    OnTextWidthChanged(this);
            }
        }
        float GetDisplayTextWidth(string text)
        {
            return cachedTextGeneratorForLayout.GetPreferredWidth(text, this.customGenerationSettings) / pixelsPerUnit;
        }

        private static LocalizationTextRow[] lines;
        private static bool loaded = false;
        private int origin_len;

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;
            origin_len = text.Length;
            if (!loaded)
            {
                loaded = true;
                lines = LocalizationHelper.ReadFromJSON();
            }
            ChangeLanguage(LocalizationHelper.GetLanguge());
        }

        public void ChangeLanguage(int language)
        {
            if (language == -2 && !ignoreLocalization)
            {
                text = "";
                for (int i = 0; i < origin_len; i++)
                {
                    text += '□';
                }
                return;
            }
            string id = localizationType == LocalizationHelper.TextLocalizationType.RuntimeUse ? localizationID : m_previewID;
            if(language == -3 && !ignoreLocalization)
            {
                text = id;
                return;
            }
            if (language >= 0 && id != "" && !ignoreLocalization)
            {
                text = "";
                if (lines != null)
                {
                    foreach (LocalizationTextRow line in lines)
                    {
                        if (line.key == id)
                        {
                            text = line.translates[language];
                        }
                    }
                }
                if (text == "")
                {
                    text = need_replace;
                }
            }
        }
    }
}
