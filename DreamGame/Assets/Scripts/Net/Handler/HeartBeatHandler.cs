using Net.Framework.Server;
using Net.Protocol;
using System;
using UnityEngine;

namespace Net.Client
{
    /// <summary>
    /// 心跳处理
    /// </summary>
    public class HeartBeatHandler : MonoBehaviour, IBaseHandler
    {
        public string methodName = "SendHeartBeat";

        void OnDestroy()
        {
            try
            {
                if (IsInvoking(methodName))
                {
                    CancelInvoke(methodName);
                }
            }
            catch (Exception) { }
        }

        public void MessageReceive(SocketModel message) { }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            try
            {
                if (IsInvoking(methodName))
                {
                    CancelInvoke(methodName);
                }
                InvokeRepeating(methodName, HeartBeat.ClientHeartBeatTime, HeartBeat.ClientHeartBeatTime);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 发送心跳
        /// </summary>
        private void SendHeartBeat()
        {
            try
            {
                this.WriteMessage(Protocols.HEARTBEAT_PROTOCOL, 0, 0, null);
            }
            catch (Exception) { }
        }
    }
}