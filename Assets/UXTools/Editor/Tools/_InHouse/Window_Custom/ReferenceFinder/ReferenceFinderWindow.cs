#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using System.Text;
namespace ThunderFireUITool
{
    public class ReferenceInfo : EditorWindow
    {
        //依赖模式的key
        const string isDependPrefKey = "ReferenceFinderData_IsDepend";

        public static ReferenceFinderData data = new ReferenceFinderData();
        private static bool initializedData = false;

        private bool isDepend = false;
        public bool needUpdateState = true;

        public bool needUpdateAssetTree = false;
        private bool initializedGUIStyle = false;
        //工具栏按钮样式
        private GUIStyle toolbarButtonGUIStyle;
        //工具栏样式
        private GUIStyle toolbarGUIStyle;
        //选中资源列表
        public List<string> selectedAssetGuid = new List<string>();

        public AssetTreeView m_AssetTreeView;

        [SerializeField]
        private TreeViewState m_TreeViewState;


        private string docUrl = "https://confluence.leihuo.netease.com/pages/viewpage.action?pageId=21214849";

        //查找资源引用信息
        [MenuItem("Assets/==查找资源的引用== (Find Reference)", false, -801)]
        static void FindRef()
        {
            InitDataIfNeeded();
            OpenWindow();
            ReferenceInfo window = GetWindow<ReferenceInfo>();
            window.UpdateSelectedAssets();
        }

        //打开窗口
        [MenuItem("ThunderFireUXTool/资源管理 (ResourceTools)/查找引用 (Find Reference)", false, 51)]
        static void OpenWindow()
        {
            ReferenceInfo window = GetWindow<ReferenceInfo>();
            window.wantsMouseMove = false;
            window.titleContent = new GUIContent("查找引用");
            window.Show();
            window.Focus();
            SortHelper.Init();
        }

        //初始化数据
        static void InitDataIfNeeded()
        {
            if (!initializedData)
            {
                //初始化数据
                if (!data.ReadFromCache())
                {
                    data.CollectDependenciesInfo();
                }
                initializedData = true;
            }
        }

        //初始化GUIStyle
        void InitGUIStyleIfNeeded()
        {
            if (!initializedGUIStyle)
            {
                toolbarButtonGUIStyle = new GUIStyle("ToolbarButton");
                toolbarGUIStyle = new GUIStyle("Toolbar");
                initializedGUIStyle = true;
            }
        }

        //更新选中资源列表
        private void UpdateSelectedAssets()
        {
            artInfo = new Dictionary<string, ListInfo>();
            selectedAssetGuid.Clear();
            foreach (var obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                //如果是文件夹
                if (Directory.Exists(path))
                {
                    string[] folder = new string[] { path };
                    //将文件夹下所有资源作为选择资源
                    string[] guids = AssetDatabase.FindAssets(null, folder);
                    foreach (var guid in guids)
                    {
                        if (!selectedAssetGuid.Contains(guid) &&
                                !Directory.Exists(AssetDatabase.GUIDToAssetPath(guid)))
                        {
                            selectedAssetGuid.Add(guid);
                        }
                    }
                }
                //如果是文件资源
                else
                {
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    selectedAssetGuid.Add(guid);
                }
            }
            needUpdateAssetTree = true;
        }

        private void UpdateDragAssets()
        {
            if (EditorWindow.mouseOverWindow)
            {
                var tempObj = DragAreaGetObject.GetOjbects();
                if (tempObj != null)
                {
                    InitDataIfNeeded();
                    selectedAssetGuid.Clear();
                    foreach (var obj in tempObj)
                    {
                        string path = AssetDatabase.GetAssetPath(obj);
                        //如果是文件夹
                        if (Directory.Exists(path))
                        {
                            string[] folder = new string[] { path };
                            //将文件夹下所有资源作为选择资源
                            string[] guids = AssetDatabase.FindAssets(null, folder);
                            foreach (var guid in guids)
                            {
                                if (!selectedAssetGuid.Contains(guid) &&
                                        !Directory.Exists(AssetDatabase.GUIDToAssetPath(guid)))
                                {
                                    selectedAssetGuid.Add(guid);
                                }
                            }
                        }
                        //如果是文件资源
                        else
                        {
                            string guid = AssetDatabase.AssetPathToGUID(path);
                            selectedAssetGuid.Add(guid);
                        }
                    }
                    needUpdateAssetTree = true;
                }
            }
        }

        //通过选中资源列表更新TreeView
        private void UpdateAssetTree()
        {
            if (needUpdateAssetTree && selectedAssetGuid.Count != 0)
            {
                var root = SelectedAssetGuidToRootItem(selectedAssetGuid);
                if (m_AssetTreeView == null)
                {
                    //初始化TreeView
                    if (m_TreeViewState == null)
                        m_TreeViewState = new TreeViewState();
                    var headerState = AssetTreeView.CreateDefaultMultiColumnHeaderState(position.width, isDepend);
                    //var multiColumnHeader = new MultiColumnHeader(headerState);
                    var multiColumnHeader = new ClickColumn(headerState);
                    m_AssetTreeView = new AssetTreeView(m_TreeViewState, multiColumnHeader);
                }
                else
                {
                    var headerState = AssetTreeView.CreateDefaultMultiColumnHeaderState(position.width, isDepend);
                    var multiColumnHeader = new ClickColumn(headerState);
                    m_AssetTreeView.multiColumnHeader = multiColumnHeader;
                }

                m_AssetTreeView.assetRoot = root;
                //m_AssetTreeView.CollapseAll();
                m_AssetTreeView.Reload();
                needUpdateAssetTree = false;

                int totalPrefab = 0;
                int totalMat = 0;
                string prefabName = "";
                string matName = "";
                StringBuilder sb = new StringBuilder();
                if (artInfo.Count > 0)
                {
                    foreach (KeyValuePair<string, ListInfo> kv in artInfo)
                    {
                        if (kv.Value.type == "prefab")
                        {
                            totalPrefab += kv.Value.count;
                            prefabName += kv.Value.name + "<--->";
                        }
                        if (kv.Value.type == "mat")
                        {
                            totalMat += kv.Value.count;
                            matName += kv.Value.name + "<--->";
                        }
                        string tempInfo = $"name  <color=green>[{kv.Key}]</color>, type: <color=orange>[{kv.Value.type}]</color>, count: <color=red>[{kv.Value.count}]</color>";//"name:  " + kv.Key + "  type:  " + <color=orange>[{Time.frameCount}]</color>kv.Value.type + "  count:  " + kv.Value.count + "\r\n";
                        sb.AppendLine(tempInfo);
                    }
                }
                sb.Insert(0, $"Prefab总数  <color=red>[{totalPrefab}]</color>  Prefab详情  <color=green>[{prefabName}]</color> \r\n");
                sb.Insert(0, $"Mat总数  <color=red>[{totalMat}]</color>  Mat详情  <color=green>[{matName}]</color>  \r\n");
                Debug.Log(sb.ToString());
            }
        }

        private void OnEnable()
        {
            isDepend = PlayerPrefs.GetInt(isDependPrefKey, 0) == 1;
        }

        private void OnGUI()
        {
            UpdateDragAssets();
            InitGUIStyleIfNeeded();
            DrawOptionBar();
            UpdateAssetTree();
            if (m_AssetTreeView != null)
            {
                //绘制Treeview
                m_AssetTreeView.OnGUI(new Rect(0, toolbarGUIStyle.fixedHeight, position.width, position.height - toolbarGUIStyle.fixedHeight));
            }
        }

        //绘制上条
        public void DrawOptionBar()
        {
            EditorGUILayout.BeginHorizontal(toolbarGUIStyle);
            //刷新数据
            if (GUILayout.Button("点击更新本地库", toolbarButtonGUIStyle))
            {
                data.CollectDependenciesInfo();
                needUpdateAssetTree = true;
                EditorGUIUtility.ExitGUI();
            }
            //修改模式
            bool PreIsDepend = isDepend;
            isDepend = GUILayout.Toggle(isDepend, isDepend ? "依赖模式" : "引用模式", toolbarButtonGUIStyle, GUILayout.Width(100));
            if (PreIsDepend != isDepend)
            {
                OnModelSelect();
            }
            GUILayout.FlexibleSpace();
            //文档
            if (GUILayout.Button("使用说明", toolbarButtonGUIStyle))
            {
                Application.OpenURL(docUrl);
            }
            //扩展
            if (GUILayout.Button("展开", toolbarButtonGUIStyle))
            {
                if (m_AssetTreeView != null) m_AssetTreeView.ExpandAll();
            }
            //折叠
            if (GUILayout.Button("折叠", toolbarButtonGUIStyle))
            {
                if (m_AssetTreeView != null) m_AssetTreeView.CollapseAll();
            }

            EditorGUILayout.EndHorizontal();
        }
        private string GetSelPath()
        {
            if (m_AssetTreeView == null)
                return string.Empty;
            if (m_AssetTreeView.assetRoot != null && m_AssetTreeView.assetRoot.children.Count > 0)
            {
                AssetViewItem item = m_AssetTreeView.assetRoot.children[0] as AssetViewItem;
                string path = item.data.path;
                string aa = "Assets";
                int pos = path.IndexOf(aa);
                return path.Substring(0, pos + aa.Length + 1);
            }
            return string.Empty;
        }
        private void OnModelSelect()
        {
            needUpdateAssetTree = true;
            PlayerPrefs.SetInt(isDependPrefKey, isDepend ? 1 : 0);
            UpdateAssetTree();
        }


        //生成root相关
        private HashSet<string> updatedAssetSet = new HashSet<string>();
        private HashSet<string> ParentAssetIsAdd = new HashSet<string>();
        private HashSet<string> BrotherAssetIsAdd = new HashSet<string>();
        //通过选择资源列表生成TreeView的根节点
        private AssetViewItem SelectedAssetGuidToRootItem(List<string> selectedAssetGuid)
        {
            updatedAssetSet.Clear();
            ParentAssetIsAdd.Clear();
            BrotherAssetIsAdd.Clear();
            int elementCount = 0;
            var root = new AssetViewItem { id = elementCount, depth = -1, displayName = "Root", data = null };
            int depth = 0;
            foreach (var childGuid in selectedAssetGuid)
            {
                AssetViewItem rs = null;
                rs = CreateTree(childGuid, ref elementCount, depth);
                root.AddChild(rs);
            }

            updatedAssetSet.Clear();
            return root;
        }

        Dictionary<string, ListInfo> artInfo = new Dictionary<string, ListInfo>();  //记录输出给美术用
                                                                                    //通过每个节点的数据生成子节点
        private AssetViewItem CreateTree(string guid, ref int elementCount, int _depth)
        {
            if (ParentAssetIsAdd.Contains(guid))
            {
                return null;
            }
            if (needUpdateState && !updatedAssetSet.Contains(guid))
            {
                data.UpdateAssetState(guid);
                updatedAssetSet.Add(guid);
            }

            ++elementCount;
            var referenceData = data.assetDict[guid];
            var root = new AssetViewItem { id = elementCount, displayName = referenceData.name, data = referenceData, depth = _depth };
            var childGuids = isDepend ? referenceData.dependencies : referenceData.references;
            ParentAssetIsAdd.Add(guid);
            foreach (var childGuid in childGuids)
            {
                if (BrotherAssetIsAdd.Contains(childGuid))
                {
                    continue;
                }
                //Debug.Log(root.displayName + "---->>" + AssetDatabase.GUIDToAssetPath(childGuid));
                ListInfo listInfo = new ListInfo();
                if (AssetDatabase.GUIDToAssetPath(childGuid).EndsWith(".mat") && _depth < 2)
                {
                    listInfo.type = "mat";
                    listInfo.count = 1;
                    listInfo.name = System.IO.Path.GetFileName(AssetDatabase.GUIDToAssetPath(childGuid));
                    if (artInfo.ContainsKey(root.displayName))
                    {
                        artInfo[root.displayName].count += 1;
                        artInfo[root.displayName].name += "<<==>>" + listInfo.name;
                    }
                    else
                    {
                        artInfo.Add(root.displayName, listInfo);
                    }
                }
                if (AssetDatabase.GUIDToAssetPath(childGuid).EndsWith(".prefab") && !AssetDatabase.GUIDToAssetPath(childGuid).Contains("_gen_render") && _depth < 2)
                {
                    listInfo.type = "prefab";
                    listInfo.count = 1;
                    listInfo.name = System.IO.Path.GetFileName(AssetDatabase.GUIDToAssetPath(childGuid));
                    if (artInfo.ContainsKey(root.displayName))
                    {
                        artInfo[root.displayName].count += 1;
                        artInfo[root.displayName].name += "<<==>>" + listInfo.name;
                    }
                    else
                    {
                        artInfo.Add(root.displayName, listInfo);
                    }
                }

                BrotherAssetIsAdd.Add(childGuid);
                AssetViewItem rs = null;
                rs = CreateTree(childGuid, ref elementCount, _depth + 1);
                if (rs != null)
                {
                    root.AddChild(rs);
                }
            }
            foreach (var childGuid in childGuids)
            {
                if (BrotherAssetIsAdd.Contains(childGuid))
                {
                    BrotherAssetIsAdd.Remove(childGuid);
                }
            }

            ParentAssetIsAdd.Remove(guid);
            return root;
        }
    }

    public class ListInfo
    {
        public string type;
        public int count;
        public string name;
    }

    public sealed class DragAreaGetObject
    {
        public static UnityEngine.Object[] GetOjbects(string meg = null)
        {
            Event aEvent;
            aEvent = Event.current;
            GUI.contentColor = Color.white;
            if (aEvent.type == EventType.DragUpdated || aEvent.type == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                bool needReturn = false;
                if (aEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    needReturn = true;
                }
                Event.current.Use();
                if (needReturn)
                {
                    return DragAndDrop.objectReferences;
                }
            }
            return null;
        }
    }



    public sealed class ClickColumn : MultiColumnHeader
    {
        public delegate void SortInColumn();

        public static Dictionary<int, SortInColumn> SortWithIndex = new Dictionary<int, SortInColumn>()
    {
        {0, SortByName },
        {1, SortByPath },
    };

        public ClickColumn(MultiColumnHeaderState state) : base(state)
        {
            canSort = true;
        }


        protected override void ColumnHeaderClicked(MultiColumnHeaderState.Column column, int columnIndex)
        {
            base.ColumnHeaderClicked(column, columnIndex);
            if (SortWithIndex.ContainsKey(columnIndex))
            {
                SortWithIndex[columnIndex].Invoke();
                ReferenceInfo curWindow = EditorWindow.GetWindow<ReferenceInfo>();
                curWindow.m_AssetTreeView.SortExpandItem();
            }
        }

        public static void SortByName()
        {
            SortHelper.SortByName();
        }

        public static void SortByPath()
        {
            SortHelper.SortByPath();
        }


    }





    public enum SortType
    {
        None,
        AscByName,
        DescByName,
        AscByPath,
        DescByPath
    }

    public class SortConfig
    {
        public static Dictionary<SortType, SortType> SortTypeChangeByNameHandler = new Dictionary<SortType, SortType>()
    {
        {SortType.None, SortType.AscByName },
        {SortType.AscByName, SortType.DescByName },
        {SortType.DescByName, SortType.AscByName },
    };

        public static Dictionary<SortType, SortType> SortTypeChangeByPathHandler = new Dictionary<SortType, SortType>()
    {
        {SortType.None, SortType.AscByPath },
        {SortType.AscByPath, SortType.DescByPath },
        {SortType.DescByPath, SortType.AscByPath },
    };

        public static Dictionary<SortType, short> SortTypeGroup = new Dictionary<SortType, short>()
    {
        {SortType.None, 0 },
        {SortType.AscByPath, 1 },
        {SortType.DescByPath, 1 },
        {SortType.AscByName, 2 },
        {SortType.DescByName, 2 },
    };


        public static short TypeByNameGroup = 2;
        public static short TypeByPathGroup = 1;
    }

    public class SortHelper
    {
        public static HashSet<string> sortedGuid = new HashSet<string>();

        public static Dictionary<string, SortType> sortedAsset = new Dictionary<string, SortType>();

        public static SortType curSortType = SortType.None;

        public static SortType PathType = SortType.None;
        public static SortType NameType = SortType.None;

        public delegate int SortCompare(string lString, string rString);
        public static Dictionary<SortType, SortCompare> CompareFunction = new Dictionary<SortType, SortCompare>()
    {
        {SortType.AscByPath,  CompaerWithPath},
        {SortType.DescByPath,  CompaerWithPathDesc},
        {SortType.AscByName,  CompaerWithName},
        {SortType.DescByName,  CompaerWithNameDesc},
    };

        public static void Init()
        {
            sortedGuid.Clear();
            sortedAsset.Clear();
        }

        public static void ChangeSortType(short sortGroup, Dictionary<SortType, SortType> handler, ref SortType recoverType)
        {
            if (SortConfig.SortTypeGroup[curSortType] == sortGroup)
            {
                curSortType = handler[curSortType];
            }
            else
            {
                curSortType = recoverType;
                if (curSortType == SortType.None)
                {
                    curSortType = handler[curSortType];
                }
            }
            recoverType = curSortType;
        }

        public static void SortByName()
        {
            ChangeSortType(SortConfig.TypeByNameGroup, SortConfig.SortTypeChangeByNameHandler, ref NameType);
        }

        public static void SortByPath()
        {
            ChangeSortType(SortConfig.TypeByPathGroup, SortConfig.SortTypeChangeByPathHandler, ref PathType);
        }

        public static void SortChild(ReferenceFinderData.AssetDescription data)
        {
            //Debug.Log(curSortType);
            if (data == null)
            {
                return;
            }
            if (sortedAsset.ContainsKey(data.path))
            {
                if (sortedAsset[data.path] == curSortType)
                {
                    return;
                }
                else
                {
                    SortType oldSortType = sortedAsset[data.path];
                    if (SortConfig.SortTypeGroup[oldSortType] == SortConfig.SortTypeGroup[curSortType])
                    {
                        FastSort(data.dependencies);
                        FastSort(data.references);
                    }
                    else
                    {
                        NormalSort(data.dependencies);
                        NormalSort(data.references);
                    }
                    sortedAsset[data.path] = curSortType;
                }
            }
            else
            {
                NormalSort(data.dependencies);
                NormalSort(data.references);
                sortedAsset.Add(data.path, curSortType);
            }
        }

        public static void NormalSort(List<string> strList)
        {
            SortCompare curCompare = CompareFunction[curSortType];
            strList.Sort((l, r) =>
            {
                return curCompare(l, r);
            });
        }

        public static void FastSort(List<string> strList)
        {
            int i = 0;
            int j = strList.Count - 1;
            while (i < j)
            {
                string tmp = strList[i];
                strList[i] = strList[j];
                strList[j] = tmp;
                i++;
                j--;
            }
        }

        public static int CompaerWithName(string lString, string rString)
        {
            var asset = ReferenceInfo.data.assetDict;
            return asset[lString].name.CompareTo(asset[rString].name);
        }

        public static int CompaerWithNameDesc(string lString, string rString)
        {
            return 0 - CompaerWithName(lString, rString);
        }

        public static int CompaerWithPath(string lString, string rString)
        {
            var asset = ReferenceInfo.data.assetDict;
            return asset[lString].path.CompareTo(asset[rString].path);
        }

        public static int CompaerWithPathDesc(string lString, string rString)
        {
            return 0 - CompaerWithPath(lString, rString);
        }
    }
}
#endif
