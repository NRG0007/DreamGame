using Net.Framework.Server;
using Net.Protocol;
using Net.Protocol.DTO;
using System;
using UnityEngine;

namespace Net.Client
{
    /// <summary>
    /// 战斗处理
    /// </summary>
    public class FightHandler : MonoBehaviour, IBaseHandler
    {
        private FightRoomModel m_FightRoom;

        public void MessageReceive(SocketModel message)
        {
            try
            {
                switch (message.Command)
                {
                    case Fight.StartBroadcast:
                        OnStartBroadcast(message.GetMessage<FightRoomModel>());
                        break;
                    case Fight.MoveBroadcast:
                        //OnMoveBroadcast(message.GetMessage<MoveModel>());
                        break;
                    case Fight.AttackBroadcast:
                        //OnAttackBroadcast(message.GetMessage<AttackModel>());
                        break;
                    case Fight.SkillBroadcast:
                        //OnSkillBroadcast(message.GetMessage<SkillAttackModel>());
                        break;
                    case Fight.DamageBroadcast:
                        //OnDamageBroadcast(message.GetMessage<DamageModel>());
                        break;
                    case Fight.DeadBroadcast:
                        //OnDeadBroadcast(message.GetMessage<DeadModel>());
                        break;
                    case Fight.RefreshMonsterBroadcast:
                        //OnRefreshMonsterBroadcast(message.GetMessage<FightMonsterModel>());
                        break;
                    case Fight.GameOverBroadcast:
                        OnGameOverBroadcast(message.GetMessage<int>());
                        break;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 广播开始
        /// </summary>
        /// <param name="value"></param>
        private void OnStartBroadcast(FightRoomModel value)
        {
            try
            {
                m_FightRoom = value;
                EventCenter.GetInstance().Broadcast<FightRoomModel>(ClientEvent.FightStartBroadcast, value);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 广播游戏结束
        /// </summary>
        /// <param name="value"></param>
        private void OnGameOverBroadcast(int value)
        {
            try
            {
                EventCenter.GetInstance().Broadcast<int>(ClientEvent.FightGameOverBroadcast, value);
            }
            catch (Exception) { }
        }
    }
}