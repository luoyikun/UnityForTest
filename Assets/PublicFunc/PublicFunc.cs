using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PublicFunc 
{
    public static string GetIP()
    {
        string ip = "";
        var strHostName = System.Net.Dns.GetHostName();


        var ipEntry = System.Net.Dns.GetHostEntry(strHostName);


        var addr = ipEntry.AddressList;



        return addr[1].ToString();

    }

    static public void SaveJsonString(string JsonString, string path)    //保存Json格式字符串
    {//写入Json数据
        if (File.Exists(path) == true)
        {
            File.Delete(path);
        }

        string onlyPath = GetOnlyPath(path);
        if (!Directory.Exists(onlyPath))
        {
            Directory.CreateDirectory(onlyPath);
        }

        FileInfo file = new FileInfo(path);
        StreamWriter writer = file.CreateText();
        writer.Write(JsonString);
        writer.Close();
        writer.Dispose();
    }

    static public string GetJsonString(string path)     //从文件里面读取json数据
    {//读取Json数据
        StreamReader reader = new StreamReader(path);
        string jsonData = reader.ReadToEnd();
        reader.Close();
        reader.Dispose();
        return jsonData;
    }

    public static string GetOnlyPath(string path)
    {
        string[] bufPath = path.Split('/');
        string name = bufPath[bufPath.Length - 1];
        string onlyPath = path.Replace(name, "");
        //string abPath = info.m_prefabName.Replace("/" + abName, "");
        //string[] bufAbName = abName.Split('.');
        return onlyPath;
    }

}
