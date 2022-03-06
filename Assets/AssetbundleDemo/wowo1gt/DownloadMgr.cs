using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using UnityEngine;

public delegate void DonwloadErrorCallBack(DownloadUnit downUnit, string msg);
public delegate void DonwloadProgressCallBack(DownloadUnit downUnit, int curSize, int allSize);
public delegate void DonwloadCompleteCallBack(DownloadUnit downUnit);

public class DownloadUnit
{
    public string name; //���ص��ļ�����Ϊ��ʶ��
    public string downUrl; //Զ�̵�ַ
    public string savePath; //���ص�ַ
    public int size; //�ļ����ȣ��Ǳ���
    public string md5; //��ҪУ���md5���Ǳ��롣������������Ϻ�Ĭ�ϳɹ�
    public bool isDelete; //���������������ص��ļ�

    public DonwloadErrorCallBack errorFun;
    public DonwloadProgressCallBack progressFun;
    public DonwloadCompleteCallBack completeFun;
}

public class DownloadMgr
{

    //�õ��ļ���С
    public static long GetDownSizeByPath(string url)
    {
        long length = 0;
        string path = url.Replace(WowAbDownTest.DownUrl, "");

        if (WowAbDownTest.m_dicFileSize.ContainsKey(path))
        {
            length = (int)WowAbDownTest.m_dicFileSize[path];
        }
        return length;
    }


    private static DownloadMgr _Instance = null;

    public static DownloadMgr Instance
    {
        get
        {
            if (_Instance == null) _Instance = new DownloadMgr();
            return _Instance;
        }
    }

    private static object _lock = new object();
    private const int MAX_THREAD_COUNT = 20;

    private Queue<DownloadFileMac> _readyList; //׼���õ����ݣ�������У��Ƚ��ȳ�
    private Dictionary<Thread, DownloadFileMac> _runningList; //���������̣߳����أ�<20,�����µ�
    private List<DownloadUnit> _completeList;  //�Ѿ���ɵ��߳�
    private List<DownloadFileMac> _errorList;  //���������쳣���߳�

    private DownloadMgr()
    {
        _readyList = new Queue<DownloadFileMac>();
        _runningList = new Dictionary<Thread, DownloadFileMac>();
        _completeList = new List<DownloadUnit>();
        _errorList = new List<DownloadFileMac>();

        //https����������
        System.Net.ServicePointManager.DefaultConnectionLimit = 100;
        ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
    }

    private bool RemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }


    public void DownloadAsync(DownloadUnit info)
    {
        if (info == null) return;

        var fileMac = new DownloadFileMac(info);

        lock (_lock)
        {
            _readyList.Enqueue(fileMac);
        }

        if (_runningList.Count < MAX_THREAD_COUNT)
        {
            var thread = new Thread(ThreadLoop);
            lock (_lock)
            {
                _runningList.Add(thread, null);
            }
            thread.Start();
        }

    }

    //ͬ��������ûص�����
    public bool DownloadSync(DownloadUnit info)
    {
        if (info == null) return false;

        var mac = new DownloadFileMac(info);
        try
        {//ͬ�����س�������
            mac.Run();
            if (mac._state == DownloadMacState.Complete) return true;
            mac.Run();
            if (mac._state == DownloadMacState.Complete) return true;
            mac.Run();
            if (mac._state == DownloadMacState.Complete) return true;
        }
        catch (Exception ex)
        {
            Debug.Log("Error DownloadSync " + mac._state + " " + mac._downUnit.name + " " + ex.Message + " " + ex.StackTrace);
        }

        return false;
    }

    public void DeleteDownload(DownloadUnit info)
    {
        lock (_lock)
        {
            info.isDelete = true;
        }
    }

    //������������
    public void ClearAllDownloads()
    {
        lock (_lock)
        {
            foreach (var mac in _readyList)
            {
                if (mac != null) mac._downUnit.isDelete = true;
            }

            foreach (var item in _runningList)
            {
                if (item.Value != null) item.Value._downUnit.isDelete = true;
            }

            foreach (var unit in _completeList)
            {
                if (unit != null) unit.isDelete = true;
            }
        }
    }

    private void ThreadLoop()
    {
        while (true)
        {
            DownloadFileMac mac = null;
            lock (_lock)
            {
                if (_readyList.Count > 0)
                {
                    mac = _readyList.Dequeue();
                    _runningList[Thread.CurrentThread] = mac;

                    if (mac != null && mac._downUnit.isDelete)
                    {//�Ѿ����٣�����ȡ���У�ֱ��ɾ��
                        _runningList[Thread.CurrentThread] = null;
                        continue;
                    }
                }
            }

            //�Ѿ�û����Ҫ���ص���
            if (mac == null) break;

            mac.Run();

            if (mac._state == DownloadMacState.Complete)
            {
                lock (_lock)
                {
                    _completeList.Add(mac._downUnit);
                    _runningList[Thread.CurrentThread] = null;
                }
            }
            else if (mac._state == DownloadMacState.Error)
            {
                lock (_lock)
                {
                    _readyList.Enqueue(mac);
                    //��ֹʧ��Ƶ���ص���ֻ���ض������ص�
                    if (mac.IsNeedErrorCall())
                        _errorList.Add(mac);
                }
                break;
            }
            else
            {
                ThreadDebugLog.Log("Error DownloadMacState " + mac._state + " " + mac._downUnit.name);
                break;
            }
        }

    }



    private void UpdateComplete()
    {//�ص����
        if (_completeList.Count == 0) return;

        DownloadUnit[] completeArr = null;
        lock (_lock)
        {
            completeArr = _completeList.ToArray();
            _completeList.Clear();
        }

        foreach (var info in completeArr)
        {
            if (info.isDelete) continue; //�Ѿ����٣������лص�
            info.isDelete = true;

            if (info.progressFun != null)
            {
                info.progressFun(info, info.size, info.size);
            }

            if (info.completeFun != null)
            {
                try
                {
                    info.completeFun(info);
                }
                catch (Exception ex)
                {
                    Debug.LogError("UpdateComplete " + ex.Message);
                }
            }
        }

    }

    private void UpdateError()
    {//�ص�error
        if (_errorList.Count == 0) return;

        DownloadFileMac[] errorArr = null;
        lock (_lock)
        {
            errorArr = _errorList.ToArray();
            _errorList.Clear();
        }

        foreach (var mac in errorArr)
        {
            var info = mac._downUnit;
            if (info.isDelete) continue; //�Ѿ����٣������лص�

            if (info.errorFun != null)
            {
                info.errorFun(info, mac._error);
                mac._error = "";
            }
        }
    }

    private void UpdateProgress()
    {
        //�ص�����
        if (_runningList.Count == 0) return;

        List<DownloadFileMac> runArr = new List<DownloadFileMac>();
        lock (_lock)
        {
            foreach (var mac in _runningList.Values)
            {
                if (mac != null) runArr.Add(mac);
            }
        }

        foreach (var mac in runArr)
        {
            var info = mac._downUnit;
            if (info.isDelete) continue; //�Ѿ����٣������лص�

            if (info.progressFun != null)
            {
                info.progressFun(info, mac._curSize, mac._allSize);
            }
        }
    }

    private void UpdateThread()
    {
        if (_readyList.Count == 0 && _runningList.Count == 0) return;

        lock (_lock)
        {//�رտ������߳�
            List<Thread> threadArr = new List<Thread>();
            foreach (var item in _runningList)
            {
                if (item.Key.IsAlive) continue;

                if (item.Value != null) _readyList.Enqueue(item.Value);

                threadArr.Add(item.Key);
            }

            foreach (var thread in threadArr)
            {
                _runningList.Remove(thread);
                thread.Abort();
            }
        }

        //���û�����磬���������̣߳����̻߳�����ر�
        if (!CheckNetworkActive()) return;

        if (_runningList.Count >= MAX_THREAD_COUNT) return;
        if (_readyList.Count > 0)
        {
            var thread = new Thread(ThreadLoop);
            lock (_lock)
            {
                _runningList.Add(thread, null);
            }
            thread.Start();
        }

    }

    public bool CheckNetworkActive()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {//û������

            return false;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {//234G����

            return true;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {//wifi����
            return true;
        }

        return false;
    }

    public void Update()
    {
        UpdateComplete();
        UpdateProgress();
        UpdateError();
        UpdateThread();
    }
}

