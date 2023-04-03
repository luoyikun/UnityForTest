
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SingularityGroup.HotReload.DTO;
using JetBrains.Annotations;
using SingularityGroup.HotReload.Burst;
using SingularityGroup.HotReload.HarmonyLib;
using SingularityGroup.HotReload.Newtonsoft.Json;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using SingularityGroup.HotReload.RuntimeDependencies;

[assembly: InternalsVisibleTo("SingularityGroup.HotReload.Editor")]

namespace SingularityGroup.HotReload {
    class CodePatcher {
        public static readonly CodePatcher I = new CodePatcher();
        /// <summary>Tag for use in Debug.Log.</summary>
        public const string TAG = "HotReload";
        
        internal int PatchesApplied { get; private set; }
        string PersistencePath {get;}
        
        readonly List<MethodPatchResponse> pendingPatches;
        readonly Dictionary<MethodBase, IDisposable> patchRecords;
        readonly List<MethodPatchResponse> patchHistory;
        readonly List<SMethod> patchedMethods;
        string[] assemblySearchPaths;
        SymbolResolver symbolResolver;
        readonly string tmpDir;
        
        CodePatcher() {
            pendingPatches = new List<MethodPatchResponse>();
            patchRecords = new Dictionary<MethodBase, IDisposable>();
            patchHistory = new List<MethodPatchResponse>(); 
            patchedMethods = new List<SMethod>();
            if(UnityHelper.IsEditor) {
                tmpDir = PackageConst.LibraryCachePath;
            } else {
                tmpDir = UnityHelper.TemporaryCachePath;
            }
            if(!UnityHelper.IsEditor) {
                PersistencePath = Path.Combine(UnityHelper.PersistentDataPath, "HotReload", "patches.json");
                try {
                    LoadPatches(PersistencePath);
                } catch(Exception ex) {
                    Log.Error("Encountered exception when loading patches from disk:\n{0}", ex);
                }
            }
        }
        
        void LoadPatches(string filePath) {
            PlayerLog("Loading patches from file {0}", filePath);
            var file = new FileInfo(filePath);
            if(file.Exists) {
                var bytes = File.ReadAllText(filePath);
                var patches = JsonConvert.DeserializeObject<List<MethodPatchResponse>>(bytes);
                PlayerLog("Loaded {0} patches from disk", patches.Count.ToString());
                foreach (var patch in patches) {
                    RegisterPatches(patch, persist: false);
                }
            }  
        }

        
        internal IReadOnlyList<SMethod> PatchedMethods => patchedMethods;
        internal IReadOnlyList<MethodPatchResponse> PendingPatches => pendingPatches;
        
        
        internal string[] GetAssemblySearchPaths() {
            EnsureSymbolResolver();
            return assemblySearchPaths;
        }
       
        internal void RegisterPatches(MethodPatchResponse patches, bool persist) {
            PlayerLog("Register patches.\nWarnings: {0} \nMethods:\n{1}", string.Join("\n", patches.failures), string.Join("\n", patches.patches.SelectMany(p => p.modifiedMethods).Select(m => m.displayName)));
            pendingPatches.Add(patches);
            ApplyPatches(persist);
        }
        
        void ApplyPatches(bool persist) {
            PlayerLog("ApplyPatches. {0} patches pending.", pendingPatches.Count);
            EnsureSymbolResolver();

            try {
                int count = 0;
                foreach(var response in pendingPatches) {
                    HandleMethodPatchResponse(response);
                    patchHistory.Add(response);

                    count += response.patches.Length;
                }
                if (count > 0) {
                    Dispatch.OnHotReload().Forget();
                }
            } catch(Exception ex) {
                Log.Warning("Exception occured when handling method patch. Exception:\n{0}", ex);
            } finally {
                pendingPatches.Clear();
                RemoveDuplicateMethods();
            }
            
            if(PersistencePath != null && persist) {
                SaveAppliedPatches(PersistencePath).Forget();
            }

            PatchesApplied++;
        }
        
        internal void ClearPatchedMethods() {
            patchedMethods.Clear();
            PatchesApplied = 0;
        }
        
        void RemoveDuplicateMethods() {
            var seen = new HashSet<SMethod>(SimpleMethodComparer.I);
            patchedMethods.RemoveAll(m => !seen.Add(m));
        }
        
        void TryUndoPatch(MethodBase method, bool requireSuccess) {
            IDisposable state;
            if (patchRecords.TryGetValue(method, out state)) {
                PlayerLog("Undo patch for method {0}", method);
                try {
                    state.Dispose();
                } catch {
                    if(requireSuccess) {
                        throw;
                    }
                }
                    
                patchRecords.Remove(method);
            }
        }


        void HandleMethodPatchResponse(MethodPatchResponse response) {
            EnsureSymbolResolver();
            foreach(var patch in response.patches) {
                try {
                    var asm = Assembly.Load(patch.patchAssembly);
                    var module = asm.GetLoadedModules()[0];
                    foreach(var sMethod in patch.newMethods) {
                        var newMethod = module.ResolveMethod(sMethod.metadataToken);
                        try {
                            UnityEventHelper.EnsureUnityEventMethod(newMethod);
                        } catch(Exception ex) {
                            Log.Warning("Encountered exception in EnsureUnityEventMethod: {0} {1}", ex.GetType().Name, ex.Message);
                        }
                        MethodUtils.DisableVisibilityChecks(newMethod);
                    }
                    
                    symbolResolver.AddAssembly(asm);
                    for (int i = 0; i < patch.modifiedMethods.Length; i++) {
                        PatchMethod(module, patch.modifiedMethods[i], patch.patchMethods[i], patch.unityJobs.Length > 0);
                    }
                    JobHotReloadUtility.HotReloadBurstCompiledJobs(patch, module);
                } catch(Exception ex) {
                    Log.Warning("Failed to apply patch with id: {0}\n{1}", patch.patchId, ex);
                }
            }
        }

        void PatchMethod(Module module, SMethod sOriginalMethod, SMethod sPatchMethod, bool containsBurstJobs) {
            try {
                var patchMethod = module.ResolveMethod(sPatchMethod.metadataToken);
                var start = DateTime.UtcNow;
                var state = TryResolveMethod(sOriginalMethod, patchMethod);

                if (DateTime.UtcNow - start > TimeSpan.FromMilliseconds(500)) {
                    Log.Info("Hot Reload apply took {0}", (DateTime.UtcNow - start).TotalMilliseconds);
                }

                if(state.match == null) {
                    Log.Warning(
                        "Method mismatch: {0}, patch: {1}. This can have multiple reasons:\n"
                        + "1. You are running the Editor multiple times for the same project using symlinks, and are making changes from the symlink project\n"
                        + "2. A bug in Hot Reload. Please send us a reproduce (code before/after), and we'll get it fixed for you\n"
                        , sOriginalMethod.simpleName, patchMethod.Name
                    );

                    return;
                }

                TryUndoPatch(state.match, false);
                PlayerLog("Detour method {0:X8} {1}, offset: {2}", sOriginalMethod.metadataToken, patchMethod.Name, state.offset);
                DetourResult result;
                DetourApi.DetourMethod(state.match, patchMethod, out result);
                if (result.success) {
                    patchRecords.Add(state.match, result.patchRecord);
                    patchedMethods.Add(sOriginalMethod);
                    if (RequestHelper.ServerInfo.isRemote) {
                        Directory.CreateDirectory(tmpDir);
                        File.WriteAllText(Path.Combine(tmpDir, "code-patcher-detour-log"), $"success {patchMethod.Name}");
                    }
                } else {
                    if(result.exception is InvalidProgramException && containsBurstJobs) {
                        //ignore. The method is likely burst compiled and can't be patched
                    } else {
                        HandleMethodPatchFailure(sOriginalMethod, result.exception);
                    }
                }
            } catch(Exception ex) {
                HandleMethodPatchFailure(sOriginalMethod, ex);
            }
        }
        
        struct ResolveMethodState {
            public readonly SMethod originalMethod;
            public readonly int offset;
            public readonly bool tryLowerTokens;
            public readonly bool tryHigherTokens;
            public readonly MethodBase match;
            public ResolveMethodState(SMethod originalMethod, int offset, bool tryLowerTokens, bool tryHigherTokens, MethodBase match) {
                this.originalMethod = originalMethod;
                this.offset = offset;
                this.tryLowerTokens = tryLowerTokens;
                this.tryHigherTokens = tryHigherTokens;
                this.match = match;
            }

            public ResolveMethodState With(bool? tryLowerTokens = null, bool? tryHigherTokens = null, MethodBase match = null, int? offset = null) {
                return new ResolveMethodState(
                    originalMethod, 
                    offset ?? this.offset, 
                    tryLowerTokens ?? this.tryLowerTokens,
                    tryHigherTokens ?? this.tryHigherTokens,
                    match ?? this.match);
            }
        }
        
        struct ResolveMethodResult {
            public readonly MethodBase resolvedMethod;
            public readonly bool tokenOutOfRange;
            public ResolveMethodResult(MethodBase resolvedMethod, bool tokenOutOfRange) {
                this.resolvedMethod = resolvedMethod;
                this.tokenOutOfRange = tokenOutOfRange;
            }
        }
        
        ResolveMethodState TryResolveMethod(SMethod originalMethod, MethodBase patchMethod) {
            var state = new ResolveMethodState(originalMethod, offset: 0, tryLowerTokens: true, tryHigherTokens: true, match: null);
            var result = TryResolveMethodCore(state.originalMethod, patchMethod, 0);
            if(result.resolvedMethod != null) {
                return state.With(match: result.resolvedMethod);
            }
            state = state.With(offset: 1);
            const int tries = 100000;
            while(state.offset <= tries && (state.tryHigherTokens || state.tryLowerTokens)) {
                if(state.tryHigherTokens) {
                    result = TryResolveMethodCore(originalMethod, patchMethod, state.offset);
                    if(result.resolvedMethod != null) {
                        return state.With(match: result.resolvedMethod);
                    } else if(result.tokenOutOfRange) {
                        state = state.With(tryHigherTokens: false);
                    }
                }
                if(state.tryLowerTokens) {
                    result = TryResolveMethodCore(originalMethod, patchMethod, -state.offset);
                    if(result.resolvedMethod != null) {
                        return state.With(match: result.resolvedMethod);
                    } else if(result.tokenOutOfRange) {
                        state = state.With(tryLowerTokens: false);
                    }
                }
                state = state.With(offset: state.offset + 1);
            }
            return state;
        }
        
        
        ResolveMethodResult TryResolveMethodCore(SMethod methodToResolve, MethodBase patchMethod, int offset) {
            bool tokenOutOfRange = false;
            MethodBase resolvedMethod = null;
            try {
                resolvedMethod = TryGetMethodBaseWithRelativeToken(methodToResolve, offset);
                if(!MethodCompatiblity.AreMethodsCompatible(resolvedMethod, patchMethod)) {
                    resolvedMethod = null;
                }
            } catch (SymbolResolvingFailedException ex) when(ex.InnerException is ArgumentOutOfRangeException) {
                tokenOutOfRange = true;
            } catch (ArgumentOutOfRangeException) {
                tokenOutOfRange = true;
            }
            return new ResolveMethodResult(resolvedMethod, tokenOutOfRange);
        }
        
        MethodBase TryGetMethodBaseWithRelativeToken(SMethod sOriginalMethod, int offset) {
            return symbolResolver.Resolve(new SMethod(sOriginalMethod.assemblyName, 
                sOriginalMethod.displayName, 
                sOriginalMethod.metadataToken + offset,
                sOriginalMethod.genericTypeArguments, 
                sOriginalMethod.genericTypeArguments,
                sOriginalMethod.simpleName));
        }
    
        void HandleMethodPatchFailure(SMethod method, Exception exception) {
            Log.Warning($"Failed to apply patch for method {method.displayName} in assembly {method.assemblyName}\n{exception}");
            Log.Exception(exception);
        }

        void EnsureSymbolResolver() {
            if (symbolResolver == null) {
                var searchPaths = new HashSet<string>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var assembliesByName = new Dictionary<string, List<Assembly>>();
                for (var i = 0; i < assemblies.Length; i++) {
                    var name = assemblies[i].GetName().Name;
                    List<Assembly> list;
                    if (!assembliesByName.TryGetValue(name, out list)) {
                        assembliesByName.Add(name, list = new List<Assembly>());
                    }
                    list.Add(assemblies[i]);
                    
                    if(assemblies[i].IsDynamic) continue;
                    
                    var location = assemblies[i].Location;
                    if(File.Exists(location)) {
                        searchPaths.Add(Path.GetDirectoryName(Path.GetFullPath(location)));
                    }
                }
                symbolResolver = new SymbolResolver(assembliesByName);
                assemblySearchPaths = searchPaths.ToArray();
            }
        }
        
        
        async Task SaveAppliedPatches(string filePath) {
            if (filePath == null) {
                throw new ArgumentNullException(nameof(filePath));
            }
            filePath = Path.GetFullPath(filePath);
            var dir = Path.GetDirectoryName(filePath);
            if(string.IsNullOrEmpty(dir)) {
                throw new ArgumentException("Invalid path: " + filePath, nameof(filePath));
            }
            Directory.CreateDirectory(dir);
            var history = patchHistory.ToList();
            
            PlayerLog("Saving {0} applied patches to {1}", history.Count, filePath);

            await Task.Run(() => {
                var json = JsonConvert.SerializeObject(history);
                File.WriteAllText(filePath, json);
            });
        }
        
        
        
        [StringFormatMethod("format")]
        static void PlayerLog(string format, params object[] args) {
#if !UNITY_EDITOR
            HotReload.Log.Info(format, args);
#endif //!UNITY_EDITOR
        }
        
        class SimpleMethodComparer : IEqualityComparer<SMethod> {
            public static readonly SimpleMethodComparer I = new SimpleMethodComparer();
            SimpleMethodComparer() { }
            public bool Equals(SMethod x, SMethod y) => x.metadataToken == y.metadataToken;
            public int GetHashCode(SMethod x) {
                return x.metadataToken;
            }
        }
    }
}