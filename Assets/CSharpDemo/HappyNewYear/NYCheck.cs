﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Crosstales.Common.EditorTask
{
    /// <summary>Checks if a 'Happy new year'-message must be displayed.</summary>
    [InitializeOnLoad] //InitializeOnLoad会保证该函数在打开unity编辑器时就开始执行
    public static class NYCheck
    {
        private const string KEY_NYCHECK_DATE = "CT_CFG_NYCHECK_DATE";

        #region Constructor

        static NYCheck()
        {
            string lastYear = EditorPrefs.GetString(KEY_NYCHECK_DATE);

            string year = System.DateTime.Now.ToString("yyyy");
            //string year = "9999"; //only for test

            string month = System.DateTime.Now.ToString("MM");
            //string month = "01"; //only for test

            if (!year.Equals(lastYear) && month.Equals("01"))
            {
                //Debug.LogWarning(Util.BaseHelper.CreateString("-", 400));
                Debug.LogWarning($"<color=yellow>¸.•°*”˜˜”*°•.¸ ★</color>  <b><color=darkblue>crosstales LLC</color></b> wishes you a <b>happy</b> and <b>successful <color=orange>{year}</color></b>!  <color=yellow>★ ¸.•*¨`*•.</color><color=cyan>♫</color><color=red>❤</color><color=lime>♫</color><color=red>❤</color><color=magenta>♫</color><color=red>❤</color>");
                //Debug.LogWarning(Util.BaseHelper.CreateString("-", 400));

                if (!year.Equals("9999"))
                    EditorPrefs.SetString(KEY_NYCHECK_DATE, year);
            }
        }

        #endregion
    }
}
#endif
// © 2017-2021 crosstales LLC (https://www.crosstales.com)
