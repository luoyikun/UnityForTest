namespace SGF.Network.Core
{
    public class ProtocolHead
    {
        public int packetLength  = 0;
        public short moduleId = 0;
        public short cmd = 0;

        public ProtocolHead Deserialize(NetBuffer buffer)
        {
            ProtocolHead head = this;
            head.packetLength = buffer.ReadInt();
            head.moduleId = buffer.ReadShort();
            head.cmd = buffer.ReadShort();

            return head;
        }

        public NetBuffer Serialize(NetBuffer buffer)
        {
            buffer.WriteInt(packetLength);
            buffer.WriteShort(moduleId);
            
            buffer.WriteShort(cmd);
            return buffer;
        }

    }
}