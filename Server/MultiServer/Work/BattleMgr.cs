
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
    class BattleMgr : Singleton<BattleMgr>
    {
        public override void Init()
        {

            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqPlayerSync, OnNetPlayerSync);
            //NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqObjSync, OnNetObjSync);
            //NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqGrab, OnNetGrab);
            //NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqRelease, OnNetRelease);
            //NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqUse, OnNetUse);
            //NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqGrabHandChange, OnNetGrabHandChange);
            NetEventManager.Instance.AddEventListener(MsgIdDefine.ReqMen, OnNetMen);
        }

        void OnNetMen(Client client, byte[] buf)
        {
            Server.Instance.SendByteBufExceptClient(client, MsgIdDefine.RspMen, buf);
        }
        void OnNetGrabHandChange(Client client, byte[] buf)
        {
            Server.Instance.SendByteBufExceptClient(client, MsgIdDefine.RspGrabHandChange, buf);
        }

        void OnNetUse(Client client, byte[] buf)
        {
            //PtObjUse use = PBSerializer.NDeserialize<PtObjUse>(buf);
            //Server.Instance.SendAllExceptByID(use.belongID, MsgIdDefine.RspUse, use);

            Server.Instance.SendByteBufExceptClient(client, MsgIdDefine.RspUse, buf);
        }


        void OnNetPlayerSync(Client client, byte[] buf)
        {
            //ProtoPlayerSync sync = PBSerializer.NDeserialize<ProtoPlayerSync>(buf);
            //Server.Instance.SendAllExceptByID(sync.id, MsgIdDefine.RspPlayerSync, sync);

            Server.Instance.SendByteBufExceptClient(client, MsgIdDefine.RspPlayerSync, buf);
        }

        //void OnNetObjSync(Client client, byte[] buf)
        //{
        //    ProtoObjSync sync = PBSerializer.NDeserialize<ProtoObjSync>(buf);
        //    Server.Instance.SendAllExcept(sync.belongID, MsgIdDefine.RspObjSync, sync);
        //}

        void OnNetGrab(Client client, byte[] buf)
        {
            //PtObjGrab grab = PBSerializer.NDeserialize<PtObjGrab>(buf);
            //Server.Instance.SendAllExceptByID(grab.belongID, MsgIdDefine.RspGrab, grab);
            Server.Instance.SendByteBufExceptClient(client, MsgIdDefine.RspGrab, buf);
        }

        void OnNetRelease(Client client, byte[] buf)
        {
            //PtObjRelease re = PBSerializer.NDeserialize<PtObjRelease>(buf);
            //Server.Instance.SendAllExceptByClient(client, MsgIdDefine.RspRelease, re);
            Server.Instance.SendAllByteBuf(MsgIdDefine.RspRelease, buf);
            //Server.Instance.SendByteBufExceptClient(client, MsgIdDefine.RspRelease, buf);
        }
    }
}
