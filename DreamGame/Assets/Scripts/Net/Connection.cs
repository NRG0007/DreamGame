using Net.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Net.Client
{
    /// <summary>
    /// 连接状态
    /// </summary>
    public enum ConnectionStatus
    {
        None,//未连接
        Connected,//已连接
    }

    /// <summary>
    /// 连接
    /// </summary>
    public class Connection
    {
        public const int BUFFER_SIZE = 1024;//缓冲区大小
        private Socket m_Socket;//与服务器连接的Socket
        private byte[] m_ReadBuffer = new byte[BUFFER_SIZE];//读取缓冲区
        private List<byte> m_Caches = new List<byte>();//缓存
        public List<SocketModel> messages = new List<SocketModel>();//消息列表
        private bool isReading = false;//标识读取中

        /// <summary>
        /// 连接状态
        /// </summary>
        public ConnectionStatus connectionStatus { get; protected set; }

        public void Update()
        {
            try
            {

            }
            catch (Exception) { }
        }

        /// <summary>
        /// 连接远程主机
        /// </summary>
        /// <param name="host">IP</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public bool Connect(string host, int port)
        {
            try
            {
                if (connectionStatus == ConnectionStatus.Connected)
                {
                    Close();
                }

                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //连接远程主机
                m_Socket.Connect(host, port);
                //异步接收数据
                m_Socket.BeginReceive(
                    m_ReadBuffer,
                    0,
                    BUFFER_SIZE,
                    SocketFlags.None,
                    ReceiveCallback,
                    m_ReadBuffer);

                connectionStatus = ConnectionStatus.Connected;

                MessageManager.Instance.Init();
                return true;
            }
            catch (Exception)
            {
                Close();
            }
            return false;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                if (m_Socket != null)
                {
                    m_Socket.Close();
                }
                connectionStatus = ConnectionStatus.None;

                return true;
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// 接收数据回调
        /// </summary>
        /// <param name="value">异步操作状态</param>
        private void ReceiveCallback(IAsyncResult value)
        {
            try
            {
                int length = m_Socket.EndReceive(value);
                byte[] message = new byte[length];
                Buffer.BlockCopy(m_ReadBuffer, 0, message, 0, length);
                m_Caches.AddRange(message);
                if (!isReading)
                {
                    isReading = true;
                    OnProcessData();
                }

                m_Socket.BeginReceive(
                    m_ReadBuffer,
                    0,
                    BUFFER_SIZE,
                    SocketFlags.None,
                    ReceiveCallback,
                    m_ReadBuffer);
            }
            catch (Exception) { }//Close();
        }

        /// <summary>
        /// 处理缓存数据
        /// </summary>
        private void OnProcessData()
        {
            try
            {
                //长度解码
                byte[] result = LengthEncoding.GetInstance().Decode(ref m_Caches);
                if (result == null)
                {
                    isReading = false;
                    return;
                }

                SocketModel model = MessageEncoding.GetInstance().Decode(result) as SocketModel;
                if (model == null)
                {
                    isReading = false;
                    return;
                }
                messages.Add(model);

                OnProcessData();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="type"></param>
        /// <param name="area"></param>
        /// <param name="command"></param>
        /// <param name="message"></param>
        public void Write(byte type, int area, int command, object message)
        {
            try
            {
                BaseProtocol protocol = new BaseProtocol();
                protocol.Write(type);
                protocol.Write(area);
                protocol.Write(command);
                if (message != null)
                {
                    protocol.Write(SerializeHelper.GetInstance().Serialize(message));
                }

                BaseProtocol sendProtocol = new BaseProtocol();
                sendProtocol.Write(protocol.GetLength());
                sendProtocol.Write(protocol.GetBuffer());
                m_Socket.Send(sendProtocol.GetBuffer());
            }
            catch (Exception) { }//Close();
        }



        private static Connection _Instance;
        public static Connection Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Connection();
                }
                return _Instance;
            }
        }
        private Connection() { MessageManager.Instance.Init(); }
    }
}