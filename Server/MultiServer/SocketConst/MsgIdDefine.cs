
public partial class MsgIdDefine
{
    //心跳
    public const string ReqHeartBeat = "1000,0";//
    public const string RspHeartBeat = "1000,1";
    
    //房间
    public const string ReqID = "0,0"; //得到服务器下发的id
    public const string RspID = "0,1";
    public const string ReqRoomInfo = "0,2";
    public const string RspRoomInfo = "0,3";
    
    public const string ReqStart = "0,4";
    public const string RspStart = "0,5";
    public const string ReqChangeName = "0,6";
    public const string RspChangeName = "0,7";
    public const string ReqReturnMain = "0,8";
    public const string RspReturnMain = "0,9";
    public const string ReqExit = "0,10";
    public const string RspExit = "0,11";
    public const string RspReturnRoom = "0,12";//返回房间界面

    public const string RspRoomAddOne = "0,13";
    public const string RspRoomDeleOne = "0,14";

    //物体位置同步
    public const string ReqPlayerSync = "10,6";
    public const string RspPlayerSync = "10,7";
    public const string ReqObjSync = "10,8";
    public const string RspObjSync = "10,9";

    //物体使用
    public const string RspGrab = "1,1";
    public const string RspRelease = "1,3";
    public const string RspUse = "1,5";
    public const string RspGrabHandChange = "1,7";
    public const string RspRemoteEvent = "1,8";
    public const string RspMechanism = "1,9";
    public const string RspRotatorChange = "1,10"; //手摇角度的改变
    public const string RspBelong = "1,11";
    public const string RspMeDriverChangeState = "1,12"; //导轨改变状态
    //预拆装
    public const string ReqYuJianCa = "2,0";
    public const string RspYuJianCa = "2,1";
    public const string ReqYXia1 = "2,2";
    public const string RspYXia1 = "2,3";
    public const string ReqYXia2 = "2,4";
    public const string RspYXia2 = "2,5";
    public const string ReqYXia3 = "2,6";
    public const string RspYXia3 = "2,7";
    public const string ReqYWanCheng = "2,8";
    public const string RspYWanCheng = "2,9";

    //拆解
    public const string ReqLZFL = "3,0";
    public const string RspLZFL = "3,1";
    public const string ReqChaiQuDing = "3,2";
    public const string RspChaiQuDing = "3,3";
    public const string ReqKSXCX = "3,4";
    public const string RspKSXCX = "3,5";
    public const string ReqZLCX = "3,6";
    public const string RspZLCX = "3,7";

    public const string ReqKHCX = "3,8";
    public const string RspKHCX = "3,9";
    public const string ReqHXJZQCX = "3,10";
    public const string RspHXJZQCX = "3,11";
    public const string ReqQYCX = "3,12";
    public const string RspQYCX = "3,13";
    public const string ReqPZQCX = "3,14";
    public const string RspPZQCX = "3,15";

    //天车控制
    public const string ReqTianCheOpen = "4,0";
    public const string RspTianCheOpen = "4,1";
    public const string ReqTianCheCtrl = "4,2";
    public const string RspTianCheCtrl = "4,3";

    //装
    public const string ReqZhuangStep = "5,0";
    public const string RspZhuangStep = "5,1";

    public const string ReqZhuangCtrl = "5,2";
    public const string RspZhuangCtrl = "5,3";

    public const string ReqZhuang2 = "5,4";
    public const string RspZhuang2 = "5,5";

    //门
    public const string ReqMen = "6,0";
    public const string RspMen = "6,1";

    //sdc3相关
    public const string RspQianJingDing = "7,0";
    public const string RspDuiZhong = "7,1";
    public const string RspGongZhuoDengOpen = "7,2";
    public const string RspTuiDanQiMoveStateChange = "7,3";

    //吊臂相关
    //public const string RspDiaoBiHitPlayer = "8,0";
    public const string RspPlayerTriggerEnterPaoDan = "8,1";//玩家碰到吊着弹
    public const string RspPlayerTriggerExitPaoDan = "8,2";//玩家离开吊着弹
    public const string RspDiaoDanRotate = "8,3";//吊臂旋转
    public const string RspDiaoDanFixLen = "8,4";//吊臂固定角度
    public const string RspBengQiDong = "8,5";
    public const string RspDaoDanUnloadDaoGui = "8,6";//弹放到了导轨上


    //流程相关
    public const string RspRestart = "9,0";
    public const string RspUpdateTask = "9,1";
    public const string RspGetTask = "9,2";
    public const string RspAllExit = "9,3";
    public const string RspPlaySound = "9,4";
}