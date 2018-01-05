using Net.Framework.Server;
using Net.Protocol;
using System;
using UnityEngine;

namespace Net.Client
{
    /// <summary>
    /// 匹配处理
    /// </summary>
    public class MatchHandler : MonoBehaviour, IBaseHandler
    {
        public void MessageReceive(SocketModel message)
        {
            try
            {

                switch (message.Command)
                {
                    case Match.EnterSelectBroadcast:
                        OnEnterSelect();
                        break;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 进入选择
        /// </summary>
        private void OnEnterSelect()
        {
            try
            {
                EventCenter.GetInstance().Broadcast(ClientEvent.MatchEnterSelect);
            }
            catch (Exception) { }
        }
    }
}