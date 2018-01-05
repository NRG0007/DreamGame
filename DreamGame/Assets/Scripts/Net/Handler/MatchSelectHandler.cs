using Net.Framework.Server;
using Net.Protocol.DTO;
using System;
using UnityEngine;

namespace Net.Client
{
    /// <summary>
    /// 匹配选择处理
    /// </summary>
    public class MatchSelectHandler : MonoBehaviour, IBaseHandler
    {
        private SelectRoomModel m_SelectRoom;

        public void MessageReceive(SocketModel message)
        {
            try
            {
                switch (message.Command)
                {
                    case MatchSelect.DestroyBroadcast:
                        OnDestroyBroadcast();
                        break;
                    case MatchSelect.EnterSRES:
                        OnEnter(message.GetMessage<SelectRoomModel>());
                        break;
                    case MatchSelect.EnterBroadcast:
                        OnEnterBroadcast(message.GetMessage<int>());
                        break;
                    case MatchSelect.SelectFailedSRES:
                        OnSelectFailed();
                        break;
                    case MatchSelect.SelectBroadcast:
                        OnSelectBroadcast(message.GetMessage<SelectModel>());
                        break;
                    case MatchSelect.ReadyBroadcast:
                        OnReadyBroadcast(message.GetMessage<SelectModel>());
                        break;
                    case MatchSelect.FightBroadcast:
                        OnFightBroadcast();
                        break;
                    case MatchSelect.ChatBroadcast:
                        OnChatBroadcast(message.GetMessage<string>());
                        break;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 广播销毁
        /// </summary>
        private void OnDestroyBroadcast()
        {
            try
            {
                EventCenter.GetInstance().Broadcast(ClientEvent.MatchSelectDestroyBroadcast);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="value"></param>
        private void OnEnter(SelectRoomModel value)
        {
            try
            {
                m_SelectRoom = value;
                EventCenter.GetInstance().Broadcast<SelectRoomModel>(ClientEvent.MatchSelectEnter, value);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 广播进入
        /// </summary>
        /// <param name="userId"></param>
        private void OnEnterBroadcast(int userId)
        {
            try
            {
                if (m_SelectRoom != null)
                {
                    foreach (SelectModel item in m_SelectRoom.TeamOne)
                    {
                        if (item.UserId == userId)
                        {
                            item.isEnter = true;
                            EventCenter.GetInstance().Broadcast<SelectRoomModel>(ClientEvent.MatchSelectRefreshBroadcast, m_SelectRoom);
                            return;
                        }
                    }
                    foreach (SelectModel item in m_SelectRoom.TeamTwo)
                    {
                        if (item.UserId == userId)
                        {
                            item.isEnter = true;
                            EventCenter.GetInstance().Broadcast<SelectRoomModel>(ClientEvent.MatchSelectRefreshBroadcast, m_SelectRoom);
                            return;
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 选择失败
        /// </summary>
        private void OnSelectFailed()
        {
            try
            {
                EventCenter.GetInstance().Broadcast(ClientEvent.MatchSelectSelectFailed);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 广播选择
        /// </summary>
        /// <param name="value"></param>
        private void OnSelectBroadcast(SelectModel value)
        {
            try
            {
                foreach (SelectModel item in m_SelectRoom.TeamOne)
                {
                    if (item.UserId == value.UserId)
                    {
                        item.Hero = value.Hero;
                        EventCenter.GetInstance().Broadcast<SelectRoomModel>(ClientEvent.MatchSelectRefreshBroadcast, m_SelectRoom);
                        return;
                    }
                }
                foreach (SelectModel item in m_SelectRoom.TeamTwo)
                {
                    if (item.UserId == value.UserId)
                    {
                        item.Hero = value.Hero;
                        EventCenter.GetInstance().Broadcast<SelectRoomModel>(ClientEvent.MatchSelectRefreshBroadcast, m_SelectRoom);
                        return;
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 广播准备好
        /// </summary>
        /// <param name="value"></param>
        private void OnReadyBroadcast(SelectModel value)
        {
            try
            {
                EventCenter.GetInstance().Broadcast<SelectModel>(ClientEvent.MatchSelectReadyBroadcast, value);

                foreach (SelectModel item in m_SelectRoom.TeamOne)
                {
                    if (item.UserId == value.UserId)
                    {
                        item.Hero = value.Hero;
                        item.isReady = true;
                        EventCenter.GetInstance().Broadcast<SelectRoomModel>(ClientEvent.MatchSelectRefreshBroadcast, m_SelectRoom);
                        return;
                    }
                }
                foreach (SelectModel item in m_SelectRoom.TeamTwo)
                {
                    if (item.UserId == value.UserId)
                    {
                        item.Hero = value.Hero;
                        item.isReady = true;
                        EventCenter.GetInstance().Broadcast<SelectRoomModel>(ClientEvent.MatchSelectRefreshBroadcast, m_SelectRoom);
                        return;
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 广播战斗
        /// </summary>
        private void OnFightBroadcast()
        {
            try
            {
                EventCenter.GetInstance().Broadcast(ClientEvent.MatchSelectFightBroadcast);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 广播聊天
        /// </summary>
        /// <param name="value"></param>
        private void OnChatBroadcast(string value)
        {
            try
            {
                EventCenter.GetInstance().Broadcast(ClientEvent.MatchSelectChatBroadcast, value);
            }
            catch (Exception) { }
        }
    }
}