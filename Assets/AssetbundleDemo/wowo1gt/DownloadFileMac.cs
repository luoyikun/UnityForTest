///https://blog.csdn.net/wowo1gt/article/details/100137480
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

public enum DownloadMacState
{
    None,
    ResetSize,
    Download,
    Md5,
    Complete,
    Error,
}

public class DownloadFileMac
{
    const int oneReadLen = 16 * 1024;       // һ�ζ�ȡ���� 16384 = 16*kb
    const int Md5ReadLen = 16 * 1024;       // һ�ζ�ȡ���� 16384 = 16*kb
    const int ReadWriteTimeOut = 2 * 1000;  // ��ʱ�ȴ�ʱ��
    const int TimeOutWait = 5 * 1000;       // ��ʱ�ȴ�ʱ��

    public DownloadUnit _downUnit;

    public int _curSize = 0;
    public int _allSize = 0;
    DownloadMacState m_pState = DownloadMacState.None;

    public DownloadMacState _state {
        set
        {
            ThreadDebugLog.Log("��������״̬��" + value);
            m_pState = value;
        }

        get {
            return m_pState;
        }
    }
    public int _tryCount = 0; //���Դ���
    public string _error = "";

    public DownloadFileMac(DownloadUnit downUnit)
    {
        _downUnit = downUnit;
    }

    //��ֹʧ��Ƶ���ص���ֻ���ض������ص�
    public bool IsNeedErrorCall()
    {
        if (_tryCount == 3
            || _tryCount == 10
            || _tryCount == 100)
            return true;

        return false;
    }

    //�ڶ��߳��е��ã�
    public void Run()
    {
        _tryCount++;

        _state = DownloadMacState.ResetSize;
        if (!ResetSize()) return;

        _state = DownloadMacState.Download;
        if (!Download()) return;

        _state = DownloadMacState.Md5;
        //bool isMd5OK = CheckMd5();
        //if (isMd5OK)
        //{

        //}
        if (!CheckMd5()) //У��ʧ�ܣ�����һ��
        {
            _state = DownloadMacState.Download;
            if (!Download()) return; //����ִ��һ�飬��temp�ļ�д����ʽ�ļ�

            _state = DownloadMacState.Md5;
            if (!CheckMd5()) return; //���ζ�ʧ�ܣ��ļ�������
        }

        _state = DownloadMacState.Complete;
    }

    private bool ResetSize()
    {
        if (_downUnit.size <= 0)
        {
            _downUnit.size = GetWebFileSize(_downUnit.downUrl);
            if (_downUnit.size == 0) return false;
        }

        _curSize = 0;
        _allSize = _downUnit.size;

        return true;
    }

    private bool CheckMd5()
    {
        if (string.IsNullOrEmpty(_downUnit.md5)) return true; //����У�飬Ĭ�ϳɹ�

        string md5 = GetMD5HashFromFile(_downUnit.savePath);

        if (md5 != _downUnit.md5)
        {
            File.Delete(_downUnit.savePath);
            ThreadDebugLog.Log("�ļ�MD5У�����" + _downUnit.name);
            _state = DownloadMacState.Error;
            _error = "Check MD5 Error ";
            return false;
        }

        return true;
    }

    public bool Download()
    {
        
        long startPos = 0;
        string tempFile = _downUnit.savePath + ".temp";
        FileStream fs = null;
        if (File.Exists(_downUnit.savePath))
        {
            //�ļ��Ѵ��ڣ�����
            ThreadDebugLog.Log("Download���ļ��Ѵ��ڣ�����");
            ThreadDebugLog.Log("File is Exists " + _downUnit.savePath);
            _curSize = _downUnit.size;
            return true;
        }
        //���������ʱ�ļ�
        else if (File.Exists(tempFile))
        {
            fs = File.OpenWrite(tempFile);
            startPos = fs.Length;
            fs.Seek(startPos, SeekOrigin.Current); //�ƶ��ļ����еĵ�ǰָ��

            //�ļ��Ѿ������꣬û�����֣�����
            if (startPos == _downUnit.size)
            {
                ThreadDebugLog.Log("Download����ʱ�ļ�ȫд����ʽ�ļ�");
                fs.Flush();
                fs.Close();
                fs = null;
                if (File.Exists(_downUnit.savePath)) File.Delete(_downUnit.savePath);
                File.Move(tempFile, _downUnit.savePath);

                _curSize = (int)startPos;
                return true;
            }
        }
        else //��һ���������أ����ص���ʱ�ļ���
        {
            ThreadDebugLog.Log("Download����һ���������أ����ص���ʱ�ļ���");
            string direName = Path.GetDirectoryName(tempFile);
            if (!Directory.Exists(direName)) Directory.CreateDirectory(direName);
            fs = new FileStream(tempFile, FileMode.Create);
        }

        // �����߼�
        HttpWebRequest request = null;
        HttpWebResponse respone = null;
        Stream ns = null;
        try
        {
            request = WebRequest.Create(_downUnit.downUrl) as HttpWebRequest;
            request.ReadWriteTimeout = ReadWriteTimeOut;
            request.Timeout = TimeOutWait;
            if (startPos > 0) request.AddRange((int)startPos);  //����Rangeֵ���ϵ�����
                                                                //����������󣬻�÷�������Ӧ������
            respone = (HttpWebResponse)request.GetResponse();
            ns = respone.GetResponseStream();
            ns.ReadTimeout = TimeOutWait;
            long totalSize = DownloadMgr.GetDownSizeByPath(_downUnit.downUrl); 
            long curSize = startPos;
            if (curSize == totalSize)
            {
                fs.Flush();
                fs.Close();
                fs = null;
                if (File.Exists(_downUnit.savePath)) File.Delete(_downUnit.savePath);
                File.Move(tempFile, _downUnit.savePath);

                _curSize = (int)curSize;
            }
            else
            {
                byte[] bytes = new byte[oneReadLen];
                int readSize = ns.Read(bytes, 0, oneReadLen); // ��ȡ��һ������
                while (readSize > 0)
                {
                    fs.Write(bytes, 0, readSize);       // �����ص�������д����ʱ�ļ�
                    curSize += readSize;

                    // �ж��Ƿ��������
                    // ������ɽ�temp�ļ����ĳ���ʽ�ļ�
                    if (curSize == totalSize)
                    {
                        fs.Flush();
                        fs.Close();
                        fs = null;
                        if (File.Exists(_downUnit.savePath)) File.Delete(_downUnit.savePath);
                        File.Move(tempFile, _downUnit.savePath);
                    }

                    // �ص�һ��
                    _curSize = (int)curSize;
                    // ���¼�����ȡ
                    readSize = ns.Read(bytes, 0, oneReadLen);
                }
            }
        }
        catch (Exception ex)
        {
            //����ʧ�ܣ�ɾ����ʱ�ļ�
            if (fs != null) { fs.Flush(); fs.Close(); fs = null; }

            if (File.Exists(tempFile))
                File.Delete(tempFile);
            if (File.Exists(_downUnit.savePath))
                File.Delete(_downUnit.savePath);

            ThreadDebugLog.Log("���س���" + ex.Message);
            _state = DownloadMacState.Error;
            _error = "Download Error " + ex.Message;
        }
        finally
        {
            if (fs != null) { fs.Flush(); fs.Close(); fs = null; }
            if (ns != null) { ns.Close(); ns = null; }
            if (respone != null) { respone.Close(); respone = null; }
            if (request != null) { request.Abort(); request = null; }
        }

        if (_state == DownloadMacState.Error) return false;
        return true;
    }

    private int GetWebFileSize(string url)
    {
        HttpWebRequest request = null;
        HttpWebResponse respone = null;
        int length = 0;
        try
        {
            //request = WebRequest.Create(url) as HttpWebRequest;
            //request.Timeout = TimeOutWait;
            //request.ReadWriteTimeout = ReadWriteTimeOut;
            ////����������󣬻�÷�������Ӧ������
            //respone = request.GetResponse();
            //length = (int)respone.ContentLength;//todo���Ѿ���ȡ�������ȣ�Ҫ�޸��ļ��б�

            //request = (HttpWebRequest)HttpWebRequest.Create(url);
            //request.Method = "HEAD";
            //respone = (HttpWebResponse)request.GetResponse();
            //if (respone.StatusCode == HttpStatusCode.OK)
            //{
            //    //Console.WriteLine(respone.ContentLength);
            //    length = (int)respone.ContentLength;
            //}
            string path = url.Replace(WowAbDownTest.DownUrl, "");
            
            if (WowAbDownTest.m_dicFileSize.ContainsKey(path))
            {
                length = (int)WowAbDownTest.m_dicFileSize[path];
            }
        }
        catch (WebException e)
        {
            ThreadDebugLog.Log("��ȡ�ļ����ȳ���" + e.Message);
            _state = DownloadMacState.Error;
            _error = "Request File Length Error " + e.Message;
        }
        finally
        {
            if (respone != null) { respone.Close(); respone = null; }
            if (request != null) { request.Abort(); request = null; }
        }
        ThreadDebugLog.Log("��ȡ�ļ����ȣ�" + length);
        return length;
    }


    private string GetMD5HashFromFile(string fileName)
    {
        byte[] buffer = new byte[Md5ReadLen];
        int readLength = 0;//ÿ�ζ�ȡ����  
        var output = new byte[Md5ReadLen];

        using (Stream inputStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (System.Security.Cryptography.HashAlgorithm hashAlgorithm = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    //����MD5  
                    hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
                }
                //��������㣬�������(������һ��ѭ���Ѿ�����������㣬���Ե��ô˷���ʱ���������������Ϊ0)  
                hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
                byte[] retVal = hashAlgorithm.Hash;

                System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                hashAlgorithm.Clear();
                inputStream.Close();
                return sb.ToString();
            }
        }
    }

}

