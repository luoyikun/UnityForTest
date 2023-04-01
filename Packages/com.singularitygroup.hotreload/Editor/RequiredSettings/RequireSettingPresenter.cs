using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    interface IRequiredSettingPresenter {
        ShowResult ShowWarningPromptIfRequired();
        ShowResult ShowHelpBoxIfRequired();
        ShowResult Apply();
        bool CanShowHelpBox();
        bool CanShowWarningPrompt();
        bool CanAutoApply();
        void DebugReset();
    }
    
    class DefaulRequiredSettingPresenter : RequiredSettingPresenter {
        public DefaulRequiredSettingPresenter(RequiredSettingData data) : base(data) { }

        public override ShowResult ShowWarningPromptIfRequired() {
            return ShowWarningPromptIfRequiredCommon();
        }

        public override ShowResult ShowHelpBoxIfRequired() {
            return ShowHelpBoxIfRequiredCommon();
        }

        public override ShowResult Apply() {
            return ApplyCommon();
        }

        public override bool CanShowHelpBox() {
            return CanShowHelpBoxCommon();
        }

        public override bool CanShowWarningPrompt() {
            return CanShowWarningPromptCommon();
        }

        public override bool CanAutoApply() {
            return true;
        }
    }
    
    class InstallServerSettingsPresenter : RequiredSettingPresenter {
        public InstallServerSettingsPresenter(RequiredSettingData data) : base(data) { }

        public override ShowResult ShowWarningPromptIfRequired() {
            if(CanShowWarningPrompt()) {
                EditorCodePatcher.serverDownloader.PromptForDownload();
                return new ShowResult { shown = true };
            }
            return new ShowResult { shown =  false };
        }

        public override ShowResult ShowHelpBoxIfRequired() {
            if (!CanShowHelpBoxCommon()) {
                return default(ShowResult);
            }
            var rd = data.helpBoxRenderData;
            EditorGUILayout.HelpBox(rd.description, rd.messageType);
                
            EditorGUILayout.BeginHorizontal();
            var result = new ShowResult { shown = true };
            if (GUILayout.Button(rd.buttonText)) {
                result = Apply();
            }
            if (GUILayout.Button(new GUIContent("More info here", EditorGUIUtility.IconContent("console.infoicon").image), GUILayout.MaxHeight(19), GUILayout.ExpandWidth(false))) {
                Application.OpenURL(Constants.AdditionalContentURL);
            }
            EditorGUILayout.EndHorizontal();
            return result;
        }

        public override ShowResult Apply() {
            return ApplyCommon();
        }

        public override bool CanShowHelpBox() {
            return data.helpBoxRenderData != null && !data.checker.IsApplied();
        }

        public override bool CanShowWarningPrompt() {
            if (!EditorWindowHelper.IsHumanControllingUs()) {
                return false;
            }
            return !data.checker.IsApplied();
        }

        public override bool CanAutoApply() {
            return true;
        }
    }
    
    abstract class RequiredSettingPresenter : IRequiredSettingPresenter {
        public readonly RequiredSettingData data;
        protected RequiredSettingPresenter(RequiredSettingData data) {
            this.data = data;
        }
        
        static GUILayoutOption[] _nonExpandable;
        public static GUILayoutOption[] NonExpandableLayout => _nonExpandable ?? (_nonExpandable = new [] {GUILayout.ExpandWidth(false)});

        protected ShowResult ShowWarningPromptIfRequiredCommon() {
            if (CanShowWarningPromptCommon()) {
                var rd = data.installPromptRenderData;
                var requiresSaveAssets = false;
                if(EditorUtility.DisplayDialog(rd.title, rd.message, rd.ok, rd.cancel)) {
                    data.checker.Apply();
                    requiresSaveAssets = data.checker.ApplyRequiresSaveAssets;
                }
                return new ShowResult {
                    shown = true,
                    requiresSaveAssets = requiresSaveAssets
                };
            }
            return default(ShowResult);
        }
        
        protected ShowResult ShowHelpBoxIfRequiredCommon() {
            if (CanShowHelpBoxCommon()) {
                var rd = data.helpBoxRenderData;
                EditorGUILayout.HelpBox(rd.description, rd.messageType);
                
                var allowHide = rd.messageType != MessageType.Error;

                if (allowHide) {
                    EditorGUILayout.BeginHorizontal();
                }
                var result = new ShowResult { shown = true };
                if (GUILayout.Button(rd.buttonText)) {
                    result = Apply();
                }
                if (allowHide) {
                    if (GUILayout.Button("Hide", NonExpandableLayout)) {
                        EditorPrefs.SetBool(data.cacheKey, false);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
                return result;
            }
            return default(ShowResult);
        }
        
        protected ShowResult ApplyCommon() {
            data.checker.Apply();
            return new ShowResult {
                shown = true,
                requiresSaveAssets = data.checker.ApplyRequiresSaveAssets,
            };
        }
        
        protected bool CanShowHelpBoxCommon() {
            return data.helpBoxRenderData != null && !data.checker.IsApplied() && (EditorPrefs.GetBool(data.cacheKey, true) || data.helpBoxRenderData.messageType == MessageType.Error);
        }
        
        protected bool CanShowWarningPromptCommon() {
            if (!EditorWindowHelper.IsHumanControllingUs()) {
                return false;
            }

            return data.installPromptRenderData != null && !data.checker.IsApplied() && EditorPrefs.GetBool(data.cacheKey, true);
        }

        public abstract ShowResult ShowWarningPromptIfRequired();
        public abstract ShowResult ShowHelpBoxIfRequired();
        public abstract ShowResult Apply();
        public abstract bool CanShowHelpBox();
        public abstract bool CanShowWarningPrompt();
        public abstract bool CanAutoApply();
        public void DebugReset() {
            data.checker.DebugReset();
        }
    }
}
