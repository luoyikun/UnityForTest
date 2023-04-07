using ProtoBuf;
using System;
using System.Collections.Generic;

namespace ProtoDefine
{
    [ProtoContract]
    public class PtInt
    {
        [ProtoMember(1)]
        public int value;
    }

    [ProtoContract]
    public class PtAccount
    {
        [ProtoMember(1)]
        public string accountId;
    }

    

    //游戏数据改变
    [ProtoContract]
    public class PtGameValueChange
    {
        [ProtoMember(1)]
        public string id;
        [ProtoMember(2)]
        public string value;
    }

    //手增加物品
    [ProtoContract]
    public class PtHandAddObj
    {
        [ProtoMember(1)]
        public string playerID;
        [ProtoMember(2)]
        public string objID;
        [ProtoMember(3)]
        public int hand;
    }

    //物品返回到手上
    [ProtoContract]
    public class PtObjReturnHand
    {
        [ProtoMember(1)]
        public string playerID;
        [ProtoMember(2)]
        public string objID;
        [ProtoMember(3)]
        public int hand;
    }


    [ProtoContract]
    public class PtRemoteHandRend
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public int hand;

        [ProtoMember(3)]
        public bool isActice;
    }

    //远程物品事件
    [ProtoContract]
    public class PtRemoteObjEvent
    {
        [ProtoMember(1)]
        public string id;//物品自身id

        [ProtoMember(2)]
        public string objEvent; //事件id

        [ProtoMember(3)]
        public string hitParID;//碰到的最大父物体

        [ProtoMember(4)]
        public string hitChildPath;//子物体路径
    }

   

    [ProtoContract]
    public class PtRemoteFSM
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public int state;
    }



    /// <summary>
    /// 当前有多少房间，包括两种不同的房间类型
    /// </summary>
    [ProtoContract]
    public class PtRoomList
    {
        [ProtoMember(1)]
        public List<PtCreateRoom> listPlayer = new List<PtCreateRoom>();
    }

    /// <summary>
    /// 一个房间里返回房间里面人的id
    /// </summary>
    [ProtoContract]
    public class PtRoomInfo
    {
        [ProtoMember(1)]
        public List<string> listPlayer = new List<string>();
    }

    /// <summary>
    /// 一个房间里返回房间里面人的id
    /// </summary>
    [ProtoContract]
    public class PtListPlayerID
    {
        [ProtoMember(1)]
        public List<int> listPlayer = new List<int>();
    }


    [ProtoContract]
    public class PtSyncHand
    {
        [ProtoMember(1)]
        public List<float> pos = new List<float>();
        [ProtoMember(2)]
        public List<float> rot = new List<float>();
    }

    [ProtoContract]
    public class PtSyncPlayer
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public string roomID;

        [ProtoMember(3)]
        public PtSyncHand right = new PtSyncHand();

        [ProtoMember(4)]
        public PtSyncHand left = new PtSyncHand();
    }
    [Serializable]
    //创建新物品
    [ProtoContract]
    public class PtCreateNewTool
    {
        [ProtoMember(1)]
        public string accountID; // 控制者id

        [ProtoMember(2)]
        public string resID;//物品的资源id

        [ProtoMember(3)]
        public string toolID;//物品网络id

        [ProtoMember(4)]
        public int hand;//生成的左右手
    }

    [ProtoContract]
    public class PtPlayerWalk
    {
        [ProtoMember(1)]
        public string accountID;

        [ProtoMember(2)]
        public List<float> pos = new List<float>();

        [ProtoMember(3)]
        public List<float> Rotate = new List<float>();

    }

    [ProtoContract]
    public class PtObjSyncImmediately
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public List<float> pos = new List<float>();

        [ProtoMember(3)]
        public List<float> rot = new List<float>();
    }

    [ProtoContract]
    public class PtObjSync
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public List<float> pos = new List<float>();

        [ProtoMember(3)]
        public List<float> rot = new List<float>();

    }

    [ProtoContract]
    public class PtDeleObj
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public int hand;

        [ProtoMember(3)]
        public bool isActive;
    }

    [ProtoContract]
    public class PtCreateRoom
    {
        [ProtoMember(1)]
        public string playerID;

        [ProtoMember(2)]
        public int roomType;

    }

    [ProtoContract]
    public class PtMenCtrl
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public bool isOpen;
    }

    [ProtoContract]
    public class PtWalkFixDest
    {
        [ProtoMember(1)]
        public string id;//玩家id

        [ProtoMember(2)]
        public int dest;//目标的id
    }

    [ProtoContract]
    public class PtRemoteObjEnable
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public int enable;
    }

    [ProtoContract]
    public class PtRemoteTouchXEvent
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public int hand;

        [ProtoMember(3)]
        public int btnID;

        [ProtoMember(4)]
        public int btnEvent;
    }
}
