using System.IO;
using UnityEngine;

public class ResourceManager
{
    /// <summary>
    /// 资源加载
    /// </summary>
    /// <param name="path">资源路径，可传相对于Assets的路径，可带扩展名</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns>资源</returns>
    public static T Load<T>(string path) where T : Object
    {
        if(path == null) return null;
        int index = path.IndexOf("/Resources/");
        return Resources.Load<T>(Path.ChangeExtension(index == -1 ? path : path.Substring(index + 11), null));
    }
}
