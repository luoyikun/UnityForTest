using System;
using System.Linq;
using System.Text;

namespace MultiServer
{
    class MessageHandle
    {
        private byte[] data = new byte[1024];
        private int startIndex = 0;//我们存取了多少个字节的数据在数组里面

        public byte[] Data
        {
            get { return data; }
        }
        public int StartIndex
        {
            get { return startIndex; }
        }
        public int RemainSize
        {
            get { return data.Length - startIndex; }
        }
        // <summary>
        // 解析数据或者叫做读取数据
        // </summary>
        public void ReadMessage(int newDataAmount)
        {
            startIndex += newDataAmount;
            while (true)
            {
                //粘包分包
                if (startIndex <= 4) return;
                int count = BitConverter.ToInt32(data, 0);
                if ((startIndex - 4) >= count)
                {
                    //RequestCode requestCode = (RequestCode)BitConverter.ToInt32(data, 4);
                    //ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 8);
                    //string s = Encoding.UTF8.GetString(data, 12, count - 8);
                    //processDataCallback(requestCode, actionCode, s);
                    //Array.Copy(data, count + 4, data, 0, startIndex - 4 - count);

                    short mouldID = BitConverter.ToInt16(data, 4);
                    short cmdID = BitConverter.ToInt16(data, 6);
                    byte[] content = new byte[count];
                    for (int i = 0; i < count; i++)
                    {
                        content[i] = data[8 + i];
                    }

                    lock (MessageCenter.Instance._netMessageDataQueue)
                    {
                        //Debug.Log("Get Server:" + tmpNetMessageData.m_key);
                        sEvent_NetMessageData tmpNetMessageData = new sEvent_NetMessageData();
                        tmpNetMessageData._eventData = content;
                        tmpNetMessageData.m_key = mouldID.ToString() + "," + cmdID.ToString();
                        MessageCenter.Instance._netMessageDataQueue.Enqueue(tmpNetMessageData);
                    }
                    startIndex -= (count + 4);
                }
                else
                {
                    break;
                }
            }
        }
        //public static byte[] PackData(ActionCode actionCode,ReasonCode reasonCode, string data)
        //{
        //    byte[] requestCodeBytes = BitConverter.GetBytes((int)actionCode);
        //    byte[] reasonCodeBytes = BitConverter.GetBytes((int)reasonCode);
        //    byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        //    int dataAmount = requestCodeBytes.Length + reasonCodeBytes.Length + dataBytes.Length;
        //    byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
        //    byte[] newBytes = dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>();//Concat(dataBytes);
        //    return newBytes.Concat(reasonCodeBytes).Concat(dataBytes).ToArray<byte>();
        //}
    }
}
