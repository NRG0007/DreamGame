using UnityEngine;

//namespace Net.Client
//{
    /// <summary>
    /// 消息助手(扩展)
    /// </summary>
    public static class MessageHelper 
    {
        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="type"></param>
        /// <param name="area"></param>
        /// <param name="command"></param>
        /// <param name="message"></param>
        public static void WriteMessage(this MonoBehaviour mono, byte type, int area, int command, object message)
        {
            Net.Client.Connection.Instance.Write(type, area, command, message);
        }
    }
//}