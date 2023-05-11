using System;
using ThunderFireUnityEx;
using TMPro;

namespace UnityEngine.UI
{
    public class UXTextMeshPro : TextMeshProUGUI, ILocalizationText
    {
        [SerializeField]
        private LocalizationHelper.TextLocalizationType m_localizationType = LocalizationHelper.TextLocalizationType.RuntimeUse;
        public LocalizationHelper.TextLocalizationType localizationType { get { return m_localizationType; } set { m_localizationType = value; } }
        [SerializeField]
        private bool m_ignoreLocalization = true;
        public bool ignoreLocalization { get { return m_ignoreLocalization; } set { m_ignoreLocalization = value; } }
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
