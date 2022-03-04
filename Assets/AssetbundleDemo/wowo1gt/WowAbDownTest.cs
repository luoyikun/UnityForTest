using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreadTest;

public class WowAbDownTest : MonoBehaviour
{
    static int m_port = 10089;

    public static string DownUrl
    {
        get {
            return "http://" + PublicFunc.GetIP() + ":" + m_port.ToString() + "/";
        }
    }

    public static Dictionary<string, long> m_dicFileSize = new Dictionary<string, long>();
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return StartCoroutine(GetFileSize());
        Loom.Current.StarUp();
        DownloadUnit doUnit = new DownloadUnit();
        doUnit.name = "123.rar";
        string downUrl = "http://" + PublicFunc.GetIP() + ":" + m_port +  "/123.rar";
        Debug.Log(downUrl);
        doUnit.downUrl = downUrl;
        doUnit.savePath = "E:/AbDownTest/123.rar";
        doUnit.progressFun = DonwloadProgressCallBack;
        DownloadMgr.Instance.DownloadAsync(doUnit);
    }

    void DonwloadProgressCallBack(DownloadUnit downUnit, int curSize, int allSize)
    {
        Debug.Log(curSize + "/" + allSize);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetFileSize()
    {
        WWW www = new WWW(DownUrl + "FileList.txt");
        yield return www;
        if (www.error != null)
        {
            Debug.LogError("得不到文件:" + www.error.ToString());

            yield break;
        }


        string[] files = www.text.Split('\n');

        for (int i = 0; i < files.Length; i++)
        {
            var file = files[i];
            if (string.IsNullOrEmpty(file))
                continue;

            string[] keyValue = file.Split('|');
            m_dicFileSize[keyValue[0]] = long.Parse(keyValue[1]);
        }

        Debug.Log("文件列表count：" + m_dicFileSize.Count);
    }
}
