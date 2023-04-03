#pragma warning disable CS0618 // obsolete warnings (stay warning-free also in newer unity versions) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SingularityGroup.HotReload {

    static class Dispatch {
        // DispatchOnHotReload is called every time a patch is applied (1x per batch of filechanges)
        // Currently, we don't support [InvokeOnHotReload] on patched methods
        public static async Task OnHotReload() {
            var methods = await Task.Run(() => GetOrFillMethodsCacheThreaded());

            foreach (var m in methods) {
                if (m.IsStatic) {
                    InvokeStaticMethod(m);
                } else {
                    foreach (var go in GameObject.FindObjectsOfType(m.DeclaringType)) {
                        InvokeInstanceMethod(m, go);
                    }
                }
            }
        }

        private static List<MethodInfo> methodsCache;

        private static List<MethodInfo> GetOrFillMethodsCacheThreaded() {
            if (methodsCache != null) {
                return methodsCache;
            }

#if UNITY_2019_1_OR_NEWER && UNITY_EDITOR
            var methodCollection = UnityEditor.TypeCache.GetMethodsWithAttribute(typeof(InvokeOnHotReload));
            var methods = new List<MethodInfo>();
            foreach (var m in methodCollection) {
                methods.Add(m);
            }
#else
            var methods = GetMethodsReflection();
#endif

            methodsCache = methods;
            return methods;
        }

        private static List<MethodInfo> GetMethodsReflection() {
            var methods = new List<MethodInfo>();

            try {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
                    if (asm.FullName == "System" || asm.FullName.StartsWith("System.", StringComparison.Ordinal)) {
                        continue; // big performance optimization
                    }

                    try {
                        foreach (var type in asm.GetTypes()) {
                            try {
                                foreach (var m in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
                                    try {
                                        if (Attribute.IsDefined(m, typeof(InvokeOnHotReload))) {
                                            methods.Add(m);
                                        }
                                    } catch (BadImageFormatException) {
                                        // silently ignore (can happen, is very annoying if it spams)
                                        /*
                                            BadImageFormatException: VAR 3 (TOutput) cannot be expanded in this context with 3 instantiations
                                            System.Reflection.MonoMethod.GetBaseMethod () (at <c8d0d7b9135640958bff528a1e374758>:0)
                                            System.MonoCustomAttrs.GetBase (System.Reflection.ICustomAttributeProvider obj) (at <c8d0d7b9135640958bff528a1e374758>:0)
                                            System.MonoCustomAttrs.IsDefined (System.Reflection.ICustomAttributeProvider obj, System.Type attributeType, System.Boolean inherit) (at <c8d0d7b9135640958bff528a1e374758>:0)
                                        */
                                    } catch (TypeLoadException) {
                                        // silently ignore (can happen, is very annoying if it spams)
                                    } catch (Exception e) {
                                        ThreadUtility.LogException(new AggregateException(type.Name + "." + m.Name, e));
                                    }
                                }
                            } catch (BadImageFormatException) {
                                // silently ignore (can happen, is very annoying if it spams)
                            } catch (TypeLoadException) {
                                // silently ignore (can happen, is very annoying if it spams)
                            } catch (Exception e) {
                                ThreadUtility.LogException(new AggregateException(type.Name, e));
                            }
                        }
                    } catch (BadImageFormatException) {
                        // silently ignore (can happen, is very annoying if it spams)
                    } catch (TypeLoadException) {
                        // silently ignore (can happen, is very annoying if it spams)
                    } catch (Exception e) {
                        ThreadUtility.LogException(new AggregateException(asm.FullName, e));
                    }
                }
            } catch (Exception e) {
                ThreadUtility.LogException(e);
            }
            return methods;
        }

        private static void InvokeStaticMethod(MethodInfo m) {
            try {
                m.Invoke(null, new object[] { });
            } catch (Exception e) {
                if (m.GetParameters().Length != 0) {
                    Log.Exception(new AggregateException($"[InvokeOnHotReload] {m.DeclaringType?.Name} {m.Name} failed. Make sure it has 0 parameters", e));
                } else {
                    Log.Exception(new AggregateException($"[InvokeOnHotReload] {m.DeclaringType?.Name} {m.Name} failed", e));
                }
            }
        }

        private static void InvokeInstanceMethod(MethodInfo m, Object go) {
            try {
                m.Invoke(go, new object[] { });
            } catch (Exception e) {
                if (m.GetParameters().Length != 0) {
                    Log.Exception(new AggregateException($"[InvokeOnHotReload] {m.DeclaringType?.Name} {m.Name} failed. Make sure it has 0 parameters", e));
                } else {
                    Log.Exception(new AggregateException($"[InvokeOnHotReload] {m.DeclaringType?.Name} {m.Name} failed", e));
                }
            }
        }
    }
}
