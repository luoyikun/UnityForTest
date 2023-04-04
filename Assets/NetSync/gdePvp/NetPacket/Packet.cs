using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 用来序列化string
/// </summary>
public class MyBitConverter
{
    static Encoding m_codePage = Encoding.UTF8;
    static public byte[] GetBytes(string sTmp)
    {
        if (sTmp == null)
        {
            return null;
        }
        return m_codePage.GetBytes(sTmp);
    }

    static public string GetString(byte[] bufByte, int idx, int iLen)
    {
        return m_codePage.GetString(bufByte, idx, iLen);
    }
}
public class Packet : System.Object
{
    public byte[] m_buf;
    public int m_iPos = 0;
    public int m_iSize = 0;

    /// <summary>
    /// 重新设置buf的size
    /// </summary>
    /// <param name="iNewSize"></param>
    public void sizeSet(int iNewSize)
    {
        if (iNewSize < m_iSize)
            return;
        byte[] tmp = new byte[iNewSize];//产生一个新大小的buff
        m_iSize = iNewSize;
        if (m_iPos > 0)
            m_buf.CopyTo(tmp, 0);
        m_buf = tmp;
    }

    /// <summary>
    /// 复制操作把一个数组给m_buf
    /// </summary>
    /// <param name="bufByte"></param>
    /// <param name="iLen"></param>
    /// <returns></returns>
    public int write(byte[] bufByte, int iLen)
    {
        int iNewPos = m_iPos + iLen;
        //当新产生的位置大于老的size时，扩容
        if (iNewPos > m_iSize)
            sizeSet(iNewPos);

        if (iLen < bufByte.Length)
        {
            Array.Copy(bufByte, 0, m_buf, m_iPos, iLen);//只复制当前部分的数组
        }
        else if (iLen > bufByte.Length)
        {
            bufByte.CopyTo(m_buf, m_iPos);
            for (int i = 0; i < iLen - bufByte.Length; i++)
            {
                m_buf[m_iPos + bufByte.Length + i] = 0;
            }
        }
        else if (iLen == bufByte.Length)
        {
            bufByte.CopyTo(m_buf, m_iPos);
        }
        m_iPos = iNewPos;
        return m_iPos;
    }

    static public Packet operator +(Packet self, int tmp)
    {
        byte[] bufByte = BitConverter.GetBytes(tmp);
        self.write(bufByte, bufByte.Length);
        return self;
    }

    static public Packet operator +(Packet self, float tmp)
    {
        byte[] bufByte = BitConverter.GetBytes(tmp);
        self.write(bufByte, bufByte.Length);
        return self;
    }

    static public Packet operator +(Packet self, ushort tmp)
    {
        byte[] bufByte = BitConverter.GetBytes(tmp);
        self.write(bufByte, bufByte.Length);
        return self;
    }

    static public Packet operator +(Packet self, string tmp)
    {
        if (tmp == null)
        {
            byte[] bufByte = BitConverter.GetBytes(0);
            self.write(bufByte, sizeof(int));
        }

        if (tmp.Length > 0)
        {
            byte[] bufByte = MyBitConverter.GetBytes(tmp);
            byte[] bufLen = BitConverter.GetBytes(tmp.Length);
            self.write(bufLen, sizeof(int));
            self.write(bufByte, bufByte.Length);
        }
        else if (tmp.Length == 0)
        {
            byte[] bufByte = BitConverter.GetBytes(0);
            self.write(bufByte, sizeof(int));
        }
        return self;
    }

    public Packet to(ref int tmp)
    {
        int iOffset = 4;
        if (m_iPos + iOffset <= m_buf.Length)
        {
            tmp = BitConverter.ToInt32(m_buf, m_iPos);
            m_iPos += iOffset;
        }
        return this;
    }

    public Packet to(ref uint tmp)
    {
        int iOffset = 4;
        if (m_iPos + iOffset <= m_buf.Length)
        {
            tmp = BitConverter.ToUInt32(m_buf, m_iPos);
            m_iPos += iOffset;
        }
        return this;
    }

    public Packet to(ref float tmp)
    {
        int iOffset = 4;
        if (m_iPos + iOffset <= m_buf.Length)
        {
            tmp = BitConverter.ToSingle(m_buf, m_iPos);
            m_iPos += iOffset;
        }
        return this;
    }

    public Packet to(ref ushort tmp)
    {
        int iOffset = 2;
        if (m_iPos + iOffset <= m_buf.Length)
        {
            tmp = BitConverter.ToUInt16(m_buf, m_iPos);
            m_iPos += iOffset;
        }
        return this;
    }

    public Packet to(ref string tmp)
    {
        if (tmp == null)
            tmp = "";
        int iLen = 0;
        this.to(ref iLen);
        if (iLen == 0)
        {
            tmp = "";
            return this;
        }
        tmp = MyBitConverter.GetString(m_buf, m_iPos, iLen);
        m_iPos += iLen;
        return this;
    }

    public void posSet(int tmp)
    {
        m_iPos = tmp;
    }

    static public Packet bufByteToPkt(byte[] bufByte, int iLen)
    {
        Packet pkt = new Packet();
        pkt.sizeSet(iLen);
        System.Array.Copy(bufByte, 0, pkt.m_buf, 0, iLen);
        pkt.m_iPos = 0;
        return pkt;
    }

}

