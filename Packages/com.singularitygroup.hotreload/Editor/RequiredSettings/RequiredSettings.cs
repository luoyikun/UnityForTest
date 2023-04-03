using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    struct ShowResult {
        public bool shown;
        public bool requiresSaveAssets;
    }
    
    static class RequiredSettings {
        static IRequiredSettingPresenter[] _lazyPresenters;
        public static IRequiredSettingPresenter[] Presenters => _lazyPresenters ?? (_lazyPresenters = Init());
            
        static IRequiredSettingPresenter[] Init() {
            var presenters = new List<IRequiredSettingPresenter>();
            
            Add(presenters, new RequiredSettingData(
                checker: new AutoRefreshSettingChecker(),
                cacheKey: HotReloadPrefs.AutoRefreshSettingCacheKey,
                installPromptRenderData: new PromptRenderData {
                    title = "Disable Auto Refresh",
                    message = "For the best Hot Reload experience, it is recommended to disable Unity's Auto Refresh setting",
                    ok = "Disable Auto Refresh",
                    cancel  = "Not now",
                },
                helpBoxRenderData: new HelpBoxRenderData {
                    description = "The Unity Auto Refresh setting is enabled. Hot Reload works best with Auto Refresh disabled.",
                    buttonText = "Disable Unity Auto Refresh",
                    messageType = MessageType.Warning,
                }
            ));
            
            const string suggested = "Recompile After Finished Playing";
            Add(presenters, new RequiredSettingData(
                checker: new ScriptCompilationSettingChecker(),
                cacheKey: HotReloadPrefs.ScriptCompilationSettingCacheKey,
                installPromptRenderData: new PromptRenderData {
                    title = suggested,
                    message = $"For the best Hot Reload experience, it is recommended to set Unity's 'Script Changes While Playing' to {suggested}",
                    ok = "Apply Suggestion",
                    cancel  = "Not now",
                },
                helpBoxRenderData: new HelpBoxRenderData {
                    description = $"Hot Reload works best when the Editor setting 'Script Changes While Playing' is set to '{suggested}'",
                    buttonText = "Apply Suggestion",
                    messageType = MessageType.Info,
                }
            ));
             Add(presenters, new InstallServerSettingsPresenter(new RequiredSettingData(
                checker: new InstallServerSettingChecker(),
                cacheKey: HotReloadPrefs.DontShowPromptForDownloadKey,
                installPromptRenderData: null,
                helpBoxRenderData: new HelpBoxRenderData {
                    description = ServerDownloader.InstallDescription,
                    buttonText = "Apply Suggestion",
                    messageType = MessageType.Error,
                }
            )));
            return presenters.ToArray();
        }
        
        static void Add(List<IRequiredSettingPresenter> list, RequiredSettingData data) {
            list.Add(new DefaulRequiredSettingPresenter(data));
        }
        
        static void Add(List<IRequiredSettingPresenter> list, IRequiredSettingPresenter presenter) {
            list.Add(presenter);
        }
    }
    
    class RequiredSettingData {
        public readonly IRequiredSettingChecker checker;
        public readonly PromptRenderData installPromptRenderData;
        public readonly HelpBoxRenderData helpBoxRenderData;
        public readonly string cacheKey;
        public RequiredSettingData(IRequiredSettingChecker checker, PromptRenderData installPromptRenderData, HelpBoxRenderData helpBoxRenderData, string cacheKey) {
            this.checker = checker;
            this.installPromptRenderData = installPromptRenderData;
            this.helpBoxRenderData = helpBoxRenderData;
            this.cacheKey = cacheKey;
        }
    }
    
    class HelpBoxRenderData {
        public string description, buttonText;
        public MessageType messageType;
    }
    
    class PromptRenderData {
        public string title, message, ok, cancel;
    }
}