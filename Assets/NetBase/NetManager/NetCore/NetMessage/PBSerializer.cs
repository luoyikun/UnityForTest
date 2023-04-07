﻿
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.IO;
using UnityEngine;

namespace SGF.Codec
{
    /// <summary>
    /// 序列化帮助类
    /// </summary>
    public class PBSerializer
    {

        public static T Clone<T>(T data)
        {
            byte[] buffer = NSerialize<T>(data);
            return NDeserialize<T>(buffer);
        }


        /// <summary>
        /// 序列化pb数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static byte[] NSerialize<T>(T t)
        {
            byte[] buffer = null;

            using (MemoryStream m = new MemoryStream())
            {
                Serializer.Serialize<T>(m, t);

                m.Position = 0;
                int length = (int)m.Length;
                buffer = new byte[length];
                m.Read(buffer, 0, length);
            }

            return buffer;
        }

        public static byte[] NSerialize(object t)
        {
            byte[] buffer = null;

            using (MemoryStream m = new MemoryStream())
            {
                if (t != null)
                {
                    RuntimeTypeModel.Default.Serialize(m, t);
                }

                m.Position = 0;
                int length = (int)m.Length;
                buffer = new byte[length];
                m.Read(buffer, 0, length);
            }

            return buffer;
        }


        public static int NSerialize(object t, byte[] buffer)
        {
            using (MemoryStream m = new MemoryStream())
            {
                if (t != null)
                {
                    RuntimeTypeModel.Default.Serialize(m, t);
                }

                m.Position = 0;
                int length = (int)m.Length;
                m.Read(buffer, 0, length);
                return length;
            }
            return 0;
        }



        /// <summary>
        /// 反序列化pb数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T NDeserialize<T>(byte[] buffer)
        {

            T t = default(T);
            try
            {
                
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    ////将消息写入流中
                    //ms.Write(buffer, 0, buffer.Length);
                    ////将流的位置归0
                    //ms.Position = 0;

                    //t =  (T)ProtoBufSerializer.Deserialize(ms, typeof(T), (int)buffer.Length);
                    t = ProtoBuf.Serializer.Deserialize<T>(ms);
                }
                return t;
            }
            catch (Exception ex)
            {
                Debug.Log("反序列化失败: " + ex.ToString());
                return t;
            }


        }

        public static object NDeserialize(byte[] buffer, System.Type type)
        {
            object t = null;
            using (MemoryStream m = new MemoryStream(buffer))
            {
                t = RuntimeTypeModel.Default.Deserialize(m, null, type);
            }
            return t;
        }

        public static object NDeserialize(byte[] buffer, int len, System.Type type)
        {
            object t = null;
            using (MemoryStream m = new MemoryStream(buffer))
            {
                t = RuntimeTypeModel.Default.Deserialize(m, null, type, len);
            }
            return t;
        }


        public static T NDeserialize<T>(Stream stream)
        {
            T t = default(T);
            t = Serializer.Deserialize<T>(stream);
            return t;
        }
    }

}