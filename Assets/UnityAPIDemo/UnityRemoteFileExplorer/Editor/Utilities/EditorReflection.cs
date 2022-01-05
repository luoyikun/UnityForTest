using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.Collections.Generic;

namespace RemoteFileExplorer.Editor
{
    public class EditorReflection
    {
        public static object InvokeStaticMethod<T>(string methodName, params object[] param)
        {
            return InvokeStaticMethod(typeof(T), methodName, param);
        }

        public static object InvokeStaticMethod(System.Type type, string methodName, params object[] param)
        {
            return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, param);
        }

        public static List<MethodInfo> GetMethods<T>(System.Predicate<MethodInfo> validateFunc) where T : System.Attribute
        {
            var extractedMethods = TypeCache.GetMethodsWithAttribute<T>();
            List<MethodInfo> methods = new List<MethodInfo>();
            foreach (var method in extractedMethods)
            {
                if (validateFunc == null || validateFunc(method))
                {
                    methods.Add(method);
                }
            }
            return methods;
        }

        public static List<MethodInfo> GetBeforeUploadMethods()
        {
            List<MethodInfo> methods = GetMethods<BeforeUploadAttribute>(method =>
            {
                if (!(method.IsPublic && method.IsStatic))
                    return false;
                var parameters = method.GetParameters();
                if (!(parameters.Length == 2 && parameters[0].ParameterType == typeof(string))
                    && parameters[1].ParameterType == typeof(string))
                {
                    return false;
                }
                return true;
            });
            methods.Sort(new ProcessMethodComparer());
            return methods;
        }

        public static void CallBeforeUploadMethods(string path, string dest)
        {
            var methods = EditorReflection.GetBeforeUploadMethods();
            foreach(var method in methods)
            {
                var attribute = method.GetCustomAttribute<BeforeUploadAttribute>();
                if(attribute.Validate(path, dest))
                {
                    Log.Debug(string.Format(Constants.BeforeUploadCallTip, attribute.description));
                    method.Invoke(null, new object[] { path, dest });
                }
            }
        }
    }

    public class ProcessMethodComparer : IComparer<MethodInfo>
    {
        public int Compare(MethodInfo m1, MethodInfo m2)
        {
            var attribute1 = m1.GetCustomAttribute<BeforeUploadAttribute>();
            var attribute2 = m2.GetCustomAttribute<BeforeUploadAttribute>();
            return attribute1.priority.CompareTo(attribute2.priority);
        }
    }
}