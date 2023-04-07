using System.Collections.Generic;
using ProtoBuf;
namespace ProtoDefine {
[ProtoContract]


public class ReqHeartBeatMessage {
    /**
     * 账户ID
     */
[ProtoMember(1)]
    public long? accountId;

    public ReqHeartBeatMessage() {
    }

    public ReqHeartBeatMessage(long? accountId) {
        this.accountId = accountId;
    }

    public long? getAccountId() {
        return accountId;
    }

    public void setAccountId(long? accountId) {
        this.accountId = accountId;
    }
}
}