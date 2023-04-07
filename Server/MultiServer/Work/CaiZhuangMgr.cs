using Framework.Pattern;
using ProtoDefine;
using SGF.Codec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiServer.Work
{
    public class CaiZhuangMgr : Singleton<CaiZhuangMgr>
    {
        public override void Init()
        {
            //预检修
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqYuJianCa, OnNetYuJianCa);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqYXia1, OnNetYXia1);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqYXia2, OnNetYXia2);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqYXia3, OnNetYXia3);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqYWanCheng, OnNetYWanCheng);

            //拆
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqLZFL, OnNetReqLZFL);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqChaiQuDing, OnNetChaiQuDing);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqKSXCX, OnNetKSXCX);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqKHCX, OnNetKHCX);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqHXJZQCX, OnNetHXJZQCX);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqQYCX, OnNetQYCX);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqPZQCX, OnNetPZQCX);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqZLCX, OnNetZLCX);

            //天车控制
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqTianCheOpen, OnNetTianCheOpen);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqTianCheCtrl, OnNetTianCheCtrl);

            //装
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqZhuangStep, OnNetZhuangStep);

            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqZhuangCtrl, OnNetZhuangCtrl);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqZhuang2, OnNetZhuang2);
        }

        public void OnNetZhuang2(Client client, byte[] bufByte)
        {
            PtZhuang2 zhuang = PBSerializer.NDeserialize<PtZhuang2>(bufByte);
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspZhuang2, zhuang);
        }

        public void OnNetZhuangStep(Client client, byte[] bufByte)
        {
            PtZhuangStep open = PBSerializer.NDeserialize<PtZhuangStep>(bufByte);
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspZhuangStep, open);
        }

        public void OnNetTianCheOpen(Client client, byte[] bufByte)
        {
            PtTianCheOpen open = PBSerializer.NDeserialize<PtTianCheOpen>(bufByte);
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspTianCheOpen, open);
        }

        public void OnNetTianCheCtrl(Client client, byte[] bufByte)
        {
            Server.Instance.SendByteBufExceptClient(client, MsgIdDefine.RspTianCheCtrl, bufByte);
 
        }

        public void OnNetZLCX(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspZLCX, new VoidSend());
        }


        public void OnNetKSXCX(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspKSXCX, new VoidSend());
        }


        public void OnNetKHCX(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspKHCX, new VoidSend());
        }

        public void OnNetHXJZQCX(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspHXJZQCX, new VoidSend());
        }

        public void OnNetQYCX(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspQYCX, new VoidSend());
        }

        public void OnNetPZQCX(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspPZQCX, new VoidSend());
        }
        public void OnNetYuJianCa(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspYuJianCa, new VoidSend());
        }

        public void OnNetYXia1(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspYXia1, new VoidSend());
        }

        public void OnNetYXia2(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspYXia2, new VoidSend());
        }

        public void OnNetYXia3(Client client, byte[] bufByte)
        {
            PtYXia3 xia = PBSerializer.NDeserialize<PtYXia3>(bufByte);
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspYXia3, xia);
        }

        public void OnNetYWanCheng(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspYWanCheng, new VoidSend());
        }

        public void OnNetReqLZFL(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspLZFL, new VoidSend());
        }

        public void OnNetChaiQuDing(Client client, byte[] bufByte)
        {
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspChaiQuDing, new VoidSend());
        }

        public void OnNetZhuangCtrl(Client client, byte[] bufByte)
        {
            PtZhuangCtrl xia = PBSerializer.NDeserialize<PtZhuangCtrl>(bufByte);
            Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspZhuangCtrl, xia);
        }
    }
}
