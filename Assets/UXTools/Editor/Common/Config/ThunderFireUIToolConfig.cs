
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace ThunderFireUITool
{
    //UXTools中的路径和常量
    public partial class ThunderFireUIToolConfig
    {
        public static readonly string UXCommonPath = $"{AssetsRootPath}UX-GUI-Editor-Common/";
        public static readonly string UXToolsPath = $"{AssetsRootPath}UX-GUI-Editor-Tools/";
        public static readonly string UXGUIPath = $"{AssetsRootPath}UX-GUI/";

        #region Editor Res
        public static readonly string IconPath = UXToolsPath + "Assets/Editor/Res/Icon/";
        public static readonly string UIBuilderPath = UXToolsPath + "Assets/Editor/Window_uibuilder/";
        public static readonly string ScenePath = UXToolsPath + "Assets/Editor/Scene/";
        #endregion

        #region Project Setting —Default 
        public static readonly string DefaultPrefabPath = "Assets/";//默认Prefab路径
        public static readonly int DefaultMaxFileName = 100;
        #endregion

        #region Project Setting  —Prefab
        public static readonly string PrefabSettingsPath = UXToolsPath + "Assets/Editor/Settings/Prefab/";
        //组件库-组件类型数据
        public static readonly string PrefabLabelsPath = PrefabSettingsPath + "PrefabLabels.asset";
        //组件库-被认定为组件的Prefab信息
        public static readonly string PrefabSettledPath = PrefabSettingsPath + "PrefabSettled.asset";
        //组件库-默认创建Prefab路径
        public static readonly string PrefabDefaultSettingPath = PrefabSettingsPath + "PrefabDefault.asset";
        //层级管理工具用于绘制
        public static readonly string HierarchyManagementSettingPath =
            PrefabSettingsPath + "HierarchyManagementSetting.asset";
        //层级管理工具用于打包
        public static readonly string HierarchyManagementOutSettingPath =
            PrefabSettingsPath + "HierarchyManagementOutSetting.asset";
        public static readonly string PrefabRepoDefaultType = "All";
        #endregion

        #region User Data
        public static readonly string UserDataPath = UXToolsPath + "UserDatas/Editor/";
        //辅助线-辅助线数据
        public static readonly string LocationLinesDataPath = UserDataPath + "LocationLinesData.asset";
        //组件库-最近使用的Prefab数据
        public static readonly string PrefabRecentOpenedPath = UserDataPath + "PrefabRecentOpened.asset";
        public static readonly string PrefabTabsPath = UserDataPath + "PrefabTabsData.asset";
        public static readonly string QuickBackgroundDataPath = UserDataPath + "QuickBackgroundData.asset";
        public static readonly string SwitchSettingPath = UserDataPath + "SwitchSetting.asset";
        #endregion

        #region Res Check Setting
        public static readonly string SettingsPath = UXToolsPath + "Assets/Editor/Settings/";
        public static readonly string UICheckSettingFullPath = SettingsPath + "UIAtlasCheckSettings.asset";
        public static readonly string UICheckUserDataFullPath = UserDataPath + "UIAtlasCheckUserData.asset";
        public static readonly string UICheckLegacyComponentFullPath = SettingsPath + "UILegacyComponent.asset";
        #endregion

        #region MenuItem Name
        public const string Menu_OpenEditor = "ThunderFireUXTool/工具栏 (Toolbar)";

        #endregion

        #region EditorPref Name
        //用于存储需要在Play状态前后保持，但是又没有重要到需要持久化的Editor数据
        //其实就是持久化数据的简便做法
        public const string PreviewPrefabPath = "PreviewPrefabPath";
        public const string PreviewOriginScene = "PreviewOriginScene";
        #endregion
    }
}
