/// <summary>
/// 网络消息处理中心
/// 
/// create at 2014.8.26 by sun
/// </summary>



using System.Collections;
using System.Collections.Generic;
using System;
using Framework.Pattern;
using MultiServer;

public enum eGameLogicEventType
{
    NoticeInfo,
}

public struct sEvent_GameLogicData
{
    public eGameLogicEventType _eventType;
    public object _eventData;
}
public enum eProtocalCommand
{
    sc_binary_login = 0x1000,
    sc_protobuf_login = 0x2000,
}

public struct sEvent_NetMessageData
{
    public string m_key;
    public eProtocalCommand _eventType;
    public byte[] _eventData;
    public Client m_client;
}

public delegate void Callback_GameLogic_Handle(object _data);
public delegate void Callback_NetMessage_Handle(byte[] _data);

public class MessageCenter : Singleton<MessageCenter>
{
    private Dictionary<eProtocalCommand, Callback_NetMessage_Handle> _netMessage_EventList = new Dictionary<eProtocalCommand, Callback_NetMessage_Handle>();
    public Queue<sEvent_NetMessageData> _netMessageDataQueue = new Queue<sEvent_NetMessageData>();

    private Dictionary<eGameLogicEventType, Callback_GameLogic_Handle> _gameLogic_EventList = new Dictionary<eGameLogicEventType, Callback_GameLogic_Handle>();
    public Queue<sEvent_GameLogicData> _gameLogicDataQueue = new Queue<sEvent_GameLogicData>();



    //添加网络事件观察者
    public void addObsever(eProtocalCommand _protocalType, Callback_NetMessage_Handle _callback)
    {
        if (_netMessage_EventList.ContainsKey(_protocalType))
        {
            _netMessage_EventList[_protocalType] += _callback;
        }
        else
        {
            _netMessage_EventList.Add(_protocalType, _callback);
        }
    }
    //删除网络事件观察者
    public void removeObserver(eProtocalCommand _protocalType, Callback_NetMessage_Handle _callback)
    {
        if (_netMessage_EventList.ContainsKey(_protocalType))
        {
            _netMessage_EventList[_protocalType] -= _callback;
            if (_netMessage_EventList[_protocalType] == null)
            {
                _netMessage_EventList.Remove(_protocalType);
            }
        }
    }
    

    //添加普通事件观察者
	public void AddEventListener(eGameLogicEventType _eventType, Callback_GameLogic_Handle _callback)
    {
        if (_gameLogic_EventList.ContainsKey(_eventType))
        {
            _gameLogic_EventList[_eventType] += _callback;
        }
        else
        {
            _gameLogic_EventList.Add(_eventType, _callback);
        }
    }
    //删除普通事件观察者
	public void RemoveEventListener(eGameLogicEventType _eventType, Callback_GameLogic_Handle _callback)
    {
        if (_gameLogic_EventList.ContainsKey(_eventType))
        {
            _gameLogic_EventList[_eventType] -= _callback;
            if (_gameLogic_EventList[_eventType] == null)
            {
                _gameLogic_EventList.Remove(_eventType);
            }
        }
    }
    //推送消息
	public void PostEvent(eGameLogicEventType _eventType, object data = null)
    {
        if (_gameLogic_EventList.ContainsKey(_eventType))
        {
            _gameLogic_EventList[_eventType](data);
        }
    }
    


    public void Update()
    {

        while (_gameLogicDataQueue.Count > 0)
        {
            sEvent_GameLogicData tmpGameLogicData = _gameLogicDataQueue.Dequeue();
            if (_gameLogic_EventList.ContainsKey(tmpGameLogicData._eventType))
            {
                _gameLogic_EventList[tmpGameLogicData._eventType](tmpGameLogicData._eventData);
            }
        }

        while (_netMessageDataQueue.Count > 0)
        {
            lock (_netMessageDataQueue)
            {
                sEvent_NetMessageData tmpNetMessageData = _netMessageDataQueue.Dequeue();

                //if (tmpNetMessageData.m_key != "10,6" && tmpNetMessageData.m_key != "1000,0")
                //{
                //    Console.WriteLine("Rec:ID:(" + tmpNetMessageData.m_client.m_player.id + "):Key:" + tmpNetMessageData.m_key);
                //}
                
                NetEventMgr.Instance.DispatchEvent(tmpNetMessageData.m_client, tmpNetMessageData.m_key, tmpNetMessageData._eventData);
            }
        }
    }
}