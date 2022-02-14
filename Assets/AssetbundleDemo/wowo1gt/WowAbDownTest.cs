using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreadTest;

public class WowAbDownTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Loom.Current.StarUp();
        DownloadUnit doUnit = new DownloadUnit();
        doUnit.name = "123.rar";
        doUnit.downUrl = "http://192.168.31.54:10089/123.rar";
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
}
