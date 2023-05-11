using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class ManagementLevelWindow : EditorWindow
    {
        private static ManagementLevelWindow _mWindow;
        private static bool _isNew = true;
        private static ManagementLevel _level;
        private static List<ManagementLevel> _managementLevels = new List<ManagementLevel>();
        private static List<ManagementChannel> _managementChannels = new List<ManagementChannel>();
        private static string _inputText = "";
        private static int _range;

        static ManagementLevelWindow()
        {
            EditorApplication.playModeStateChanged += (obj) =>
            {
                if (HasOpenInstances<ManagementLevelWindow>())
                    _mWindow = GetWindow<ManagementLevelWindow>();
                if (EditorApplication.isPaused)
                {
                    return;
                }
                if (!(EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode))
                {
                    return;
                }
                if(_mWindow != null)
                    _mWindow.CloseWindow();
            };
        }
        
        [UnityEditor.Callbacks.DidReloadScripts(0)]
        private static void OnScriptReload()
        {
            if (HasOpenInstances<ManagementLevelWindow>())
                _mWindow = GetWindow<ManagementLevelWindow>();
        }

        public static ManagementLevelWindow GetInstance()
        {
            return _mWindow;
        }
        
        private void CloseWindow()
        {
            if (_mWindow != null)
            {
                _mWindow.Close();
            }
        }
        
        public static void OpenWindow(ManagementLevel level)
        {
            int width = 300;
            int height = 100;
            InitWindowData();
            _isNew = false;
            _level = level;
            _inputText = level.Index.ToString();
            _mWindow = GetWindow<ManagementLevelWindow>();
            _mWindow.minSize = new Vector2(width, height);
            _mWindow.position = new Rect((Screen.currentResolution.width - width) / 2,
                (Screen.currentResolution.height - height) / 2, width, height);
            _mWindow.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_修改层级);
        }
        
        public static void OpenWindow()
        {
            int width = 300;
            int height = 100;
            _isNew = true;
            InitWindowData();
            _level = new ManagementLevel();
            _mWindow = GetWindow<ManagementLevelWindow>();
            _mWindow.minSize = new Vector2(width, height);
            _mWindow.position = new Rect((Screen.currentResolution.width - width) / 2,
                (Screen.currentResolution.height - height) / 2, width, height);
            _mWindow.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新增层级);
        }
        
        private static string _okText;
        private static string _cancelText;
        private static void InitWindowData()
        {
            var hierarchyManagementSetting = SettingAssetsUtils.GetAssets<HierarchyManagementSetting>();
            if (HierarchyManagementSetting.isDemo)
                hierarchyManagementSetting = AssetDatabase.LoadAssetAtPath<HierarchyManagementSetting>(
                    ThunderFireUIToolConfig.AssetsRootPath + "Samples/HierarchyManage/HierarchyManagementSettingDemo.asset");
            _managementChannels = hierarchyManagementSetting.managementChannelList;
            _managementLevels = hierarchyManagementSetting.managementLevelList;
            _range = hierarchyManagementSetting.range;
            _okText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定);
            _cancelText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消);
        }
        
        private void OnEnable()
        {
            VisualElement root = rootVisualElement;

            var div = UXBuilder.Div(root, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    width = 300, height = 100, alignItems = Align.Center,
                    paddingBottom = 10, paddingLeft = 10, paddingRight = 10, paddingTop = 20,
                }
            });
            var row = UXBuilder.Row(div, new UXBuilderRowStruct()
            {
                align = Align.Center,
                style = new UXStyle() { marginLeft = 10 }
            });

            UXBuilder.Text(row, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_层级Index值),
                style = new UXStyle()
                    { marginRight = 5, unityTextAlign = TextAnchor.MiddleRight, width = Length.Percent(30) }
            });
            
            var input = UXBuilder.Input(row, new UXBuilderInputStruct()
            {
                style = new UXStyle()
                    {width = Length.Percent(60)},
                onChange = s =>
                {
                    _inputText = s;
                }
            });
            if (!_isNew)
            {
                input.value = _level.Index.ToString();
            }
            else input.value = "";
            
            row = UXBuilder.Row(div, new UXBuilderRowStruct()
            {
                justify = Justify.Center,
                align = Align.Center,
                style = new UXStyle()
                {
                    marginTop = 25,
                }
            });
            UXBuilder.Button(row, new UXBuilderButtonStruct()
            {
                type = ButtonType.Primary,
                text = _okText,
                OnClick = Submit,
                style = new UXStyle(){ width = Length.Percent(30), height = 25, fontSize = 14 }
            });
            UXBuilder.Button(row, new UXBuilderButtonStruct()
            {
                text = _cancelText,
                OnClick = CloseWindow,
                style = new UXStyle(){ width = Length.Percent(30), height = 25, fontSize = 14, marginLeft = 20}
            });
        }

        private void Submit()
        {
            int num = 0;
            if (!int.TryParse(_inputText, out num))
            {
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请正确输入数字), _okText, _cancelText);
                return;
            }
            if (_isNew)
            {
                if (_managementLevels.FindAll(s => s.Index == num).Count != 0)
                {
                    if (!EditorUtility.DisplayDialog("messageBox", 
                            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_已存在层级Tip), _okText, _cancelText)) 
                        return;
                }
                var level = _managementLevels.FindLast(s => s.Index <= num);
                if (level == null)
                {
                    level = _managementLevels[0];
                    HierarchyManagementEvent.AddLevel(level, num);
                }
                else if (level.Index == num)
                {
                    HierarchyManagementEvent.AddLevel(level, num);
                }
                else
                {
                    HierarchyManagementEvent.AddLevel(level, num);
                }
            }
            else
            {
                var ind = _managementLevels.IndexOf(_level);
                if (ind == 0)
                {
                    if (num >= _managementLevels[1].Index)
                    {
                        EditorUtility.DisplayDialog("messageBox",
                            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_只能在前后节点范围中进行更改Tip), _okText, _cancelText);
                        return;
                    }
                }
                else if (ind == _managementLevels.Count - 1)
                {
                    if (num <= _managementLevels[ind - 1].Index)
                    {
                        EditorUtility.DisplayDialog("messageBox",
                            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_只能在前后节点范围中进行更改Tip), _okText, _cancelText);
                        return;
                    }
                }
                else
                {
                    if (num <= _managementLevels[ind - 1].Index || num >= _managementLevels[ind + 1].Index)
                    {
                        EditorUtility.DisplayDialog("messageBox",
                            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_只能在前后节点范围中进行更改Tip), _okText, _cancelText);
                        return;
                    }
                }
                _level.Index = num;
                var newChannelID = num / _range;
                if (newChannelID >= _managementChannels.Count)
                {
                    for (int i = _managementChannels.Count; i <= newChannelID; i++)
                    {
                        _managementChannels.Add(new ManagementChannel() { ID = i, Name = "Channel " + i });
                    }
                }

                if (newChannelID != _level.ChannelID)
                {
                    _managementChannels[_level.ChannelID].LevelIDList.Remove(_level.ID);
                    if(newChannelID > _level.ChannelID)
                        _managementChannels[newChannelID].LevelIDList.Insert(0, _level.ID);
                    else _managementChannels[newChannelID].LevelIDList.Add(_level.ID);
                }
            }
            HierarchyManagementWindow.GetInstance().RefreshPaint();
            CloseWindow();

        }
    }
}