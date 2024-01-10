using ProtoBuf;
using System;
using System.Collections.Generic;

namespace ProtoDefine
{
    [ProtoContract]
    public class PtOnlyBool
    {
        [ProtoMember(1)]
        public bool value;
    }

    [ProtoContract]
    public class PtArrByte
    {
        [ProtoMember(1)]
        public byte[] value;
    }

    [ProtoContract]
    public class PtPdu
    {
        [ProtoMember(1)]
        public uint id;
        [ProtoMember(2)]
        public byte type;
        [ProtoMember(3)]
        public float x;
        [ProtoMember(4)]
        public float y;
        [ProtoMember(5)]
        public float z;
        [ProtoMember(6)]
        public float dirX;
        [ProtoMember(7)]
        public float dirY;
        [ProtoMember(8)]
        public float dirZ;
        [ProtoMember(9)]
        public float sendTime;
        [ProtoMember(10)]
        public float speed;

    }

    [ProtoContract]
    public class PtLong
    {
        [ProtoMember(1)]
        public long value;
    }

    [ProtoContract]
    public class PtString
    {
        [ProtoMember(1)]
        public string value;
    }

    [ProtoContract]
    public class PtUint
    {
        [ProtoMember(1)]
        public uint value;
    }

    [ProtoContract]
    public class PtMechanism
    {
        [ProtoMember(1)]
        public string id;
        [ProtoMember(2)]
        public float velocity;

        //[ProtoMember(3)]
        //public List<float> pos = new List<float>();

        //[ProtoMember(4)]
        //public List<float> angle = new List<float>();

    }

    [ProtoContract]
    public class PtRotatorOrSildr
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public float cur;

        [ProtoMember(3)]
        public float delta;

    }


    [ProtoContract]
    public class PtRemoteObjSyncPosOrAngle
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public bool isPos;

        [ProtoMember(3)]
        public List<float> list = new List<float>();

    }

    [ProtoContract]
    public class PtObjFloat
    {
        [ProtoMember(1)]
        public string id;
        [ProtoMember(2)]
        public float value;
    }

    [ProtoContract]
    public class PtRemoteEvent
    {
        [ProtoMember(1)]
        public string id;
        [ProtoMember(2)]
        public string param;
    }

    [ProtoContract]
    public class HeartBeat
    {
        [ProtoMember(1)]
        public int id;
    }


    [ProtoContract]
    public class VoidSend
    {
        [ProtoMember(1)]
        public byte value;
    }

    [ProtoContract]
    public class RspID
    {
        [ProtoMember(1)]
        public int id;
    }

    [ProtoContract]
    public class StartSceneID
    {
        [ProtoMember(1)]
        public int sceneId;
    }


    [ProtoContract]
    public class PlayerInfo
    {
        [ProtoMember(1)]
        public int id;

        [ProtoMember(2)]
        public string name;
    }

    /// <summary>
    /// 返回当前房间的所有人信息
    /// </summary>
    [ProtoContract]
    public class RspRoomInfo
    {
        [ProtoMember(1)]
        public List<PlayerInfo> listPlayer = new List<PlayerInfo>();
    }

    [ProtoContract]
    public class RspRoomAddOne
    {
        [ProtoMember(1)]
        public PlayerInfo player;
    }

    [ProtoContract]
    public class ProtoPlayerSync
    {
        [ProtoMember(1)]
        public string id;
        [ProtoMember(2)]
        public List<float> posHead = new List<float>();
        [ProtoMember(3)]
        public List<float> posLeftHand = new List<float>();
        [ProtoMember(4)]
        public List<float> posRightHand = new List<float>();

        [ProtoMember(5)]
        public List<float> anglesHead = new List<float>();
        [ProtoMember(6)]
        public List<float> anglesLeftHand = new List<float>();
        [ProtoMember(7)]
        public List<float> anglesRightHand = new List<float>();
        [ProtoMember(8)]
        public List<float> posPan = new List<float>();
        [ProtoMember(9)]
        public List<float> anglesPan = new List<float>();
    }

    [ProtoContract]
    public class ProtoObjSync
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public int belongID;

        [ProtoMember(3)]
        public List<float> pos = new List<float>();

        [ProtoMember(4)]
        public List<float> rot = new List<float>();

    }

    [ProtoContract]
    public class PtObjUse
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public string belongID;

        [ProtoMember(3)]
        public int hand; // 左右手

        [ProtoMember(4)]
        public int use;

    }

    //我手上东西被某个远程强制夺取，并设置为某个远程的子物体
    [ProtoContract]
    public class PtObjRelease
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public string belongID;

        [ProtoMember(3)]
        public int hand; // 左右手

        [ProtoMember(4)]
        public PtObjGrab grab = new PtObjGrab(); // 被哪个远程获取了
    }


    [ProtoContract]
    public class PtObjGrab
    {
        [ProtoMember(1)]
        public string id;

        [ProtoMember(2)]
        public string belongID;

        [ProtoMember(3)]
        public int hand; // 左右手

        [ProtoMember(4)]
        public int grab;  //拿起方下

        [ProtoMember(5)]
        public List<float> pos = new List<float>();

        [ProtoMember(6)]
        public List<float> rot = new List<float>();
    }

    [ProtoContract]
    public class PtYXia3
    {
        [ProtoMember(1)]
        public int id; // 哪个人生成了手电筒
        [ProtoMember(2)]
        public int type; //创建0   使用1
        [ProtoMember(3)]
        public int value;//对创建 0 是销毁  1是创建   ：：：对使用  0是不使用  1是使用

    }

    [ProtoContract]
    public class PtTianCheOpen
    {
        [ProtoMember(1)]
        public string id; // 天车的id
        [ProtoMember(2)]
        public int belongID; //现在谁操作
        [ProtoMember(3)]
        public int open;

    }

    [ProtoContract]
    public class PtTianCheCtrl
    {
        [ProtoMember(1)]
        public string id; // 天车的id
        [ProtoMember(2)]
        public int belongID; //现在谁操作
        [ProtoMember(3)]
        public int ctrlIdx; //现在按下的键
        [ProtoMember(4)]
        public List<float> listPos = new List<float>(); //现在按下的键
        [ProtoMember(5)]
        public List<float> listRot = new List<float>(); //现在按下的键
    }

    //针对装1
    [ProtoContract]
    public class PtZhuangStep
    {
        [ProtoMember(1)]
        public int id; //操作者的id
        [ProtoMember(2)]
        public int idx; //当前是第几步
       
    }

    //装2
    [ProtoContract]
    public class PtZhuangCtrl
    {
        [ProtoMember(1)]
        public int id; //操作者的id
        [ProtoMember(2)]
        public string name; //当前是第几步

    }

    //装3
    [ProtoContract]
    public class PtZhuang2
    {
        [ProtoMember(1)]
        public int id; //操作者的id
        [ProtoMember(2)]
        public string name; //当前是第几步

    }

    //打开柜门
    [ProtoContract]
    public class PtMen
    {
        [ProtoMember(1)]
        public string id; //门的id
        [ProtoMember(2)]
        public int isOpen; //当前是第几步

    }

    [ProtoContract]
    public class FaSheTongGaiData
    {
        [ProtoMember(1)]
        public List<string> list = new List<string>();
        [ProtoMember(2)]
        public bool isOpen;
    }

    [ProtoContract]
    public class PtMeDriverChangeState
    {
        [ProtoMember(1)]
        public string id;
        [ProtoMember(2)]
        public int state;
    }

    [ProtoContract]
    public class PtPlaySound
    {
        [ProtoMember(1)]
        public int id;
        [ProtoMember(2)]
        public bool isPlay;
    }
}
