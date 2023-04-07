using UnityEngine;
using System.Collections;
using System;
using Net;
using Util;

public delegate void TocHandler(object data);
public abstract class BaseModel<T> : MonoBehaviour where T : BaseModel<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    public static bool Exists
    {
        get;
        private set;
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
            Exists = true;
        }
        else if (_instance != this)
        {
            throw new InvalidOperationException("Can't have two instances of a view");
        }
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        InitAddTocHandler();
    }

    protected abstract void InitAddTocHandler();

   

    //protected void SendTos(IMessage obj)
    //{
    //    NetManager.Instance.SendMessage(obj);
    //}
}
