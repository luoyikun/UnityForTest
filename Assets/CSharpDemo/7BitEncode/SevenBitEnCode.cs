using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SevenBitEnCode : MonoBehaviour
{
    public int m_intValue = -1;
    // Start is called before the first frame update
    void Start()
    {
        string fileName = Application.streamingAssetsPath + "/Test.txt";
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
        using (var stream = File.Open(fileName, FileMode.OpenOrCreate))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                Write7BitEncodedInt32(writer, m_intValue);
            }
        }


        using (var stream = File.Open(fileName, FileMode.Open))
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
            {
                int value = Read7BitEncodedInt32(reader);
                Debug.Log(value);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ���������д������� 32 λ�з���������
    /// </summary>
    /// <param name="binaryWriter">Ҫд��Ķ���������</param>
    /// <param name="value">Ҫд��� 32 λ�з���������</param>
    public static void Write7BitEncodedInt32(BinaryWriter binaryWriter, int value)
    {
        uint num = (uint)value;
        while (num >= 0x80)
        {
            binaryWriter.Write((byte)(num | 0x80));
            num >>= 7;
        }

        binaryWriter.Write((byte)num);
    }

    /// <summary>
    /// �Ӷ���������ȡ������ 32 λ�з���������
    /// </summary>
    /// <param name="binaryReader">Ҫ��ȡ�Ķ���������</param>
    /// <returns>��ȡ�� 32 λ�з���������</returns>
    public static int Read7BitEncodedInt32( BinaryReader binaryReader)
    {
        int value = 0;
        int shift = 0;
        byte b;
        do
        {
            if (shift >= 35)
            {
                Debug.Log("7 bit encoded int is invalid.");
            }

            b = binaryReader.ReadByte();
            value |= (b & 0x7f) << shift;
            shift += 7;
        } while ((b & 0x80) != 0);

        return value;
    }

}
