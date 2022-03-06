using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// �˽ű�ֻΪ��Ϥ���̣���������Ϊ������
/// </summary>
public class AssetBundleLoadMgr
{
    public delegate void AssetBundleLoadCallBack(AssetBundle ab);

    private class AssetBundleObject
    {
        public string _hashName;

        public int _refCount;
        public List<AssetBundleLoadCallBack> _callFunList = new List<AssetBundleLoadCallBack>();

        public AssetBundleCreateRequest _request;
        public AssetBundle _ab;

        public int _dependLoadingCount;
        public List<AssetBundleObject> _depends = new List<AssetBundleObject>();
    }

    private static AssetBundleLoadMgr _Instance = null;

    public static AssetBundleLoadMgr I
    {
        get
        {
            if (_Instance == null) _Instance = new AssetBundleLoadMgr();
            return _Instance;
        }
    }

    private const int MAX_LOADING_COUNT = 10; //ͬʱ���ص��������

    private List<AssetBundleObject> tempLoadeds = new List<AssetBundleObject>(); //������ʱ�洢������������������

    private Dictionary<string, string[]> _dependsDataList;

    private Dictionary<string, AssetBundleObject> _readyABList; //Ԥ�����ص��б�
    private Dictionary<string, AssetBundleObject> _loadingABList; //���ڼ��ص��б�
    private Dictionary<string, AssetBundleObject> _loadedABList; //������ɵ��б�
    private Dictionary<string, AssetBundleObject> _unloadABList; //׼��ж�ص��б�

    private AssetBundleLoadMgr()
    {
        _dependsDataList = new Dictionary<string, string[]>();

        _readyABList = new Dictionary<string, AssetBundleObject>();
        _loadingABList = new Dictionary<string, AssetBundleObject>();
        _loadedABList = new Dictionary<string, AssetBundleObject>();
        _unloadABList = new Dictionary<string, AssetBundleObject>();

    }

    public void LoadMainfest()
    {
        //string path = FileVersionMgr.I.GetFilePathByExist("Assets");
        string path = "";
        if (string.IsNullOrEmpty(path)) return;

        _dependsDataList.Clear();
        AssetBundle ab = AssetBundle.LoadFromFile(path);

        if (ab == null)
        {
            string errormsg = string.Format("LoadMainfest ab NULL error !");
            Debug.LogError(errormsg);
            return;
        }

        AssetBundleManifest mainfest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        if (mainfest == null)
        {
            string errormsg = string.Format("LoadMainfest NULL error !");
            Debug.LogError(errormsg);
            return;
        }

        foreach (string assetName in mainfest.GetAllAssetBundles())
        {
            string hashName = assetName.Replace(".ab", "");
            string[] dps = mainfest.GetAllDependencies(assetName);
            for (int i = 0; i < dps.Length; i++)
                dps[i] = dps[i].Replace(".ab", "");
            _dependsDataList.Add(hashName, dps);
        }

        ab.Unload(true);
        ab = null;

        Debug.Log("AssetBundleLoadMgr dependsCount=" + _dependsDataList.Count);
    }

    private string GetHashName(string _assetName)
    {//���߿����Լ�����hash��ʽ�����ڴ���Ҫ��Ļ�������hash��uint����uint64����ʡ�ڴ�
        return _assetName.ToLower();
    }

    private string GetFileName(string _hashName)
    {//���߿����Լ�ʵ���Լ��Ķ�Ӧ��ϵ
        return _hashName + ".ab";
    }

    // ��ȡһ����Դ��·��
    private string GetAssetBundlePath(string _hashName)
    {//���߿����Լ�ʵ�ֵĶ�Ӧ��ϵ�����������ж����Ժ��ļ��汾�Ĵ���
        //string lngHashName = GetHashName(LocalizationMgr.I.GetAssetPrefix() + _hashName);
        //if (_dependsDataList.ContainsKey(lngHashName))
        //    _hashName = lngHashName;

        //return FileVersionMgr.I.GetFilePath(GetFileName(_hashName));
        return _hashName;
    }

    public bool IsABExist(string _assetName)
    {
        string hashName = GetHashName(_assetName);
        return _dependsDataList.ContainsKey(hashName);
    }

    //ͬ������
    public AssetBundle LoadSync(string _assetName)
    {
        string hashName = GetHashName(_assetName);
        var abObj = LoadAssetBundleSync(hashName);
        return abObj._ab;
    }

    //�첽���أ��Ѿ�����ֱ�ӻص�����ÿ�μ������ü���+1
    public void LoadAsync(string _assetName, AssetBundleLoadCallBack callFun)
    {
        string hashName = GetHashName(_assetName);
        LoadAssetBundleAsync(hashName, callFun);
    }
    //ж�أ��첽����ÿ��ж�����ü���-1
    public void Unload(string _assetName)
    {
        string hashName = GetHashName(_assetName);
        UnloadAssetBundleAsync(hashName);
    }

    private AssetBundleObject LoadAssetBundleSync(string _hashName)
    {
        AssetBundleObject abObj = null;
        if (_loadedABList.ContainsKey(_hashName)) //�Ѿ�����
        {
            abObj = _loadedABList[_hashName];
            abObj._refCount++;

            foreach (var dpObj in abObj._depends)
            {
                LoadAssetBundleSync(dpObj._hashName); //�ݹ�������������ü���
            }

            return abObj;
        }
        else if (_loadingABList.ContainsKey(_hashName)) //�ڼ�����,�첽��ͬ��
        {
            abObj = _loadingABList[_hashName];
            abObj._refCount++;

            foreach (var dpObj in abObj._depends)
            {
                LoadAssetBundleSync(dpObj._hashName); //�ݹ������������
            }

            DoLoadedCallFun(abObj, false); //ǿ����ɣ��ص�

            return abObj;
        }
        else if (_readyABList.ContainsKey(_hashName)) //��׼��������
        {
            abObj = _readyABList[_hashName];
            abObj._refCount++;

            foreach (var dpObj in abObj._depends)
            {
                LoadAssetBundleSync(dpObj._hashName); //�ݹ������������
            }

            string path1 = GetAssetBundlePath(_hashName);
            abObj._ab = AssetBundle.LoadFromFile(path1);

            _readyABList.Remove(abObj._hashName);
            _loadedABList.Add(abObj._hashName, abObj);

            DoLoadedCallFun(abObj, false); //ǿ����ɣ��ص�

            return abObj;
        }

        //����һ������
        abObj = new AssetBundleObject();
        abObj._hashName = _hashName;

        abObj._refCount = 1;

        string path = GetAssetBundlePath(_hashName);
        abObj._ab = AssetBundle.LoadFromFile(path);

        if (abObj._ab == null)
        {
            try
            {
                //ͬ�����ؽ��
                //byte[] bytes = AssetsDownloadMgr.I.DownloadSync(GetFileName(abObj._hashName));
                //if (bytes != null && bytes.Length != 0)
                //    abObj._ab = AssetBundle.LoadFromMemory(bytes);//��������
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadAssetBundleSync DownloadSync" + ex.Message);
            }
        }

        //����������
        string[] dependsData = null;
        if (_dependsDataList.ContainsKey(_hashName))
        {
            dependsData = _dependsDataList[_hashName];
        }

        if (dependsData != null && dependsData.Length > 0)
        {
            abObj._dependLoadingCount = 0;

            foreach (var dpAssetName in dependsData)
            {
                var dpObj = LoadAssetBundleSync(dpAssetName);

                abObj._depends.Add(dpObj);
            }

        }

        _loadedABList.Add(abObj._hashName, abObj);

        return abObj;
    }

    private void UnloadAssetBundleAsync(string _hashName)
    {
        AssetBundleObject abObj = null;
        if (_loadedABList.ContainsKey(_hashName))
            abObj = _loadedABList[_hashName];
        else if (_loadingABList.ContainsKey(_hashName))
            abObj = _loadingABList[_hashName];
        else if (_readyABList.ContainsKey(_hashName))
            abObj = _readyABList[_hashName];

        if (abObj == null)
        {
            string errormsg = string.Format("UnLoadAssetbundle error ! assetName:{0}", _hashName);
            Debug.LogError(errormsg);
            return;
        }

        if (abObj._refCount == 0)
        {
            string errormsg = string.Format("UnLoadAssetbundle refCount error ! assetName:{0}", _hashName);
            Debug.LogError(errormsg);
            return;
        }

        abObj._refCount--;

        foreach (var dpObj in abObj._depends)
        {
            UnloadAssetBundleAsync(dpObj._hashName);
        }

        if (abObj._refCount == 0)
        {
            _unloadABList.Add(abObj._hashName, abObj);
        }
    }


    private AssetBundleObject LoadAssetBundleAsync(string _hashName, AssetBundleLoadCallBack _callFun)
    {
        AssetBundleObject abObj = null;
        if (_loadedABList.ContainsKey(_hashName)) //�Ѿ�����
        {
            abObj = _loadedABList[_hashName];
            DoDependsRef(abObj);
            _callFun(abObj._ab);
            return abObj;
        }
        else if (_loadingABList.ContainsKey(_hashName)) //�ڼ�����
        {
            abObj = _loadingABList[_hashName];
            DoDependsRef(abObj);
            abObj._callFunList.Add(_callFun);
            return abObj;
        }
        else if (_readyABList.ContainsKey(_hashName)) //��׼��������
        {
            abObj = _readyABList[_hashName];
            DoDependsRef(abObj);
            abObj._callFunList.Add(_callFun);
            return abObj;
        }

        //����һ������
        abObj = new AssetBundleObject();
        abObj._hashName = _hashName;

        abObj._refCount = 1;
        abObj._callFunList.Add(_callFun);

        //����������
        string[] dependsData = null;
        if (_dependsDataList.ContainsKey(_hashName))
        {
            dependsData = _dependsDataList[_hashName];
        }

        if (dependsData != null && dependsData.Length > 0)
        {
            abObj._dependLoadingCount = dependsData.Length;

            foreach (var dpAssetName in dependsData)
            {
                var dpObj = LoadAssetBundleAsync(dpAssetName,
                    (AssetBundle _ab) =>
                    {
                        if (abObj._dependLoadingCount <= 0)
                        {
                            string errormsg = string.Format("LoadAssetbundle depend error ! assetName:{0}", _hashName);
                            Debug.LogError(errormsg);
                            return;
                        }

                        abObj._dependLoadingCount--;

                        //����������
                        if (abObj._dependLoadingCount == 0 && abObj._request != null && abObj._request.isDone)
                        {
                            DoLoadedCallFun(abObj);
                        }
                    }
                );

                abObj._depends.Add(dpObj);
            }

        }

        if (_loadingABList.Count < MAX_LOADING_COUNT) //���ڼ��ص��������ܳ�������
        {
            DoLoad(abObj);

            _loadingABList.Add(_hashName, abObj);
        }
        else _readyABList.Add(_hashName, abObj);

        return abObj;
    }

    private void DoDependsRef(AssetBundleObject abObj)
    {
        abObj._refCount++;

        if (abObj._depends.Count == 0) return;
        foreach (var dpObj in abObj._depends)
        {
            DoDependsRef(dpObj); //�ݹ������������
        }
    }

    private void DoLoad(AssetBundleObject abObj)
    {
        //if (AssetsDownloadMgr.I.IsNeedDownload(GetFileName(abObj._hashName)))
        //{//�����ǹ��������߼�������ʵ���첽�������첽����
        //    AssetsDownloadMgr.I.DownloadAsync(GetFileName(abObj._hashName),
        //        () =>
        //        {
        //            string path = GetAssetBundlePath(abObj._hashName);
        //            abObj._request = AssetBundle.LoadFromFileAsync(path);

        //            if (abObj._request == null)
        //            {
        //                string errormsg = string.Format("LoadAssetbundle path error ! assetName:{0}", abObj._hashName);
        //                Debug.LogError(errormsg);
        //            }
        //        }
        //    );
        //}
        //else
        {
            string path = GetAssetBundlePath(abObj._hashName);
            abObj._request = AssetBundle.LoadFromFileAsync(path);

            if (abObj._request == null)
            {
                string errormsg = string.Format("LoadAssetbundle path error ! assetName:{0}", abObj._hashName);
                Debug.LogError(errormsg);
            }
        }

    }

    private void DoLoadedCallFun(AssetBundleObject abObj, bool isAsync = true)
    {
        //��ȡab
        if (abObj._request != null)
        {
            abObj._ab = abObj._request.assetBundle; //���û�����꣬���첽תͬ��
            abObj._request = null;
            _loadingABList.Remove(abObj._hashName);
            _loadedABList.Add(abObj._hashName, abObj);
        }

        if (abObj._ab == null)
        {
            string errormsg = string.Format("LoadAssetbundle _ab null error ! assetName:{0}", abObj._hashName);
            string path = GetAssetBundlePath(abObj._hashName);
            errormsg += "\n File " + File.Exists(path) + " Exists " + path;

            try
            {//���Զ�ȡ�����ƽ��
                if (File.Exists(path))
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    if (bytes != null && bytes.Length != 0)
                        abObj._ab = AssetBundle.LoadFromMemory(bytes);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadAssetbundle ReadAllBytes Error " + ex.Message);
            }

            //if (abObj._ab == null)
            //{
            //    //ͬ�����ؽ��
            //    byte[] bytes = AssetsDownloadMgr.I.DownloadSync(GetFileName(abObj._hashName));
            //    if (bytes != null && bytes.Length != 0)
            //        abObj._ab = AssetBundle.LoadFromMemory(bytes);

            //    if (abObj._ab == null)
            //    {//ͬ�����ػ����ܽ�����Ƴ�
            //        if (_loadedABList.ContainsKey(abObj._hashName)) _loadedABList.Remove(abObj._hashName);
            //        else if (_loadingABList.ContainsKey(abObj._hashName)) _loadingABList.Remove(abObj._hashName);

            //        Debug.LogError(errormsg);

            //        if (isAsync)
            //        {//�첽���ؽ��
            //            AssetsDownloadMgr.I.AddDownloadSetFlag(GetFileName(abObj._hashName));
            //        }
            //    }
            //}
        }

        //���лص�
        foreach (var callback in abObj._callFunList)
        {
            callback(abObj._ab);
        }
        abObj._callFunList.Clear();
    }

    private void UpdateLoad()
    {
        if (_loadingABList.Count == 0) return;
        //���������
        tempLoadeds.Clear();
        foreach (var abObj in _loadingABList.Values)
        {
            if (abObj._dependLoadingCount == 0 && abObj._request != null && abObj._request.isDone)
            {
                tempLoadeds.Add(abObj);
            }
        }
        //�ص����п��ܶ�_loadingABList���в�������ȡ��ص�
        foreach (var abObj in tempLoadeds)
        {
            //��������лص�
            DoLoadedCallFun(abObj);
        }

    }

    private void DoUnload(AssetBundleObject abObj)
    {
        //������true��ж��Asset�ڴ棬ʵ��ָ��ж��
        if (abObj._ab == null)
        {
            string errormsg = string.Format("LoadAssetbundle DoUnload error ! assetName:{0}", abObj._hashName);
            Debug.LogError(errormsg);
            return;
        }

        abObj._ab.Unload(true);
        abObj._ab = null;
    }

    private void UpdateUnLoad()
    {
        if (_unloadABList.Count == 0) return;

        tempLoadeds.Clear();
        foreach (var abObj in _unloadABList.Values)
        {
            if (abObj._refCount == 0 && abObj._ab != null)
            {//���ü���Ϊ0�����Ѿ������꣬û������ȼ���������
                DoUnload(abObj);
                _loadedABList.Remove(abObj._hashName);

                tempLoadeds.Add(abObj);
            }

            if (abObj._refCount > 0)
            {//���ü����ӻ�����������˲�����¼��أ������٣��������б��Ƴ���
                tempLoadeds.Add(abObj);
            }
        }

        foreach (var abObj in tempLoadeds)
        {
            _unloadABList.Remove(abObj._hashName);
        }
    }

    private void UpdateReady()
    {
        if (_readyABList.Count == 0) return;
        if (_loadingABList.Count >= MAX_LOADING_COUNT) return;

        tempLoadeds.Clear();
        foreach (var abObj in _readyABList.Values)
        {
            DoLoad(abObj);

            tempLoadeds.Add(abObj);
            _loadingABList.Add(abObj._hashName, abObj);

            if (_loadingABList.Count >= MAX_LOADING_COUNT) break;
        }

        foreach (var abObj in tempLoadeds)
        {
            _readyABList.Remove(abObj._hashName);
        }
    }

    public void Update()
    {
        UpdateLoad();
        UpdateReady();
        UpdateUnLoad();
    }
}

