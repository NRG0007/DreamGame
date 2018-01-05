using UnityEngine;
using System.Collections;
using System;
using Net.Framework.Server;
using Net.Protocol;

namespace Net.Client
{
    /// <summary>
    /// 消息管理器
    /// </summary>
    [RequireComponent(typeof(LoginHandler))]
    [RequireComponent(typeof(MatchHandler))]
    [RequireComponent(typeof(MatchSelectHandler))]
    [RequireComponent(typeof(FightHandler))]
    [RequireComponent(typeof(HeartBeatHandler))]
    public class MessageManager : MonoBehaviour
    {
        private static MessageManager _Instance;
        public static MessageManager Instance 
        {
            get
            {
                if (_Instance == null)
                {
                    GameObject value = GameObject.Find("ClientMessageManager");
                    if (value == null)
                    {
                        value = new GameObject("ClientMessageManager");
                    }
                    _Instance = value.GetComponent<MessageManager>();
                    if (_Instance == null)
                    {
                        _Instance = value.AddComponent<MessageManager>();
                    }
                    DontDestroyOnLoad(_Instance.gameObject);
                }
                return _Instance;
            }
        }

        public string methodName = "MessageReceive";
        private IBaseHandler m_LoginHandler;
        private IBaseHandler m_MatchHandler;
        private IBaseHandler m_MatchSelectHandler;
        private IBaseHandler m_FightHandler;
        private IBaseHandler m_HeartBeatHandler;

        void Update()
        {
            try
            {
                while (Connection.Instance.messages.Count > 0)
                {
                    SocketModel model = Connection.Instance.messages[0];
                    StartCoroutine(methodName, model);
                    //MessageReceive(model);
                    Connection.Instance.messages.RemoveAt(0);
                }
            }
            catch (Exception) { }
        }

        void OnDestroy()
        {
            //CancelInvoke(methodName);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            try
            {
                m_LoginHandler = GetComponent<LoginHandler>();
                m_MatchHandler = GetComponent<MatchHandler>();
                m_MatchSelectHandler = GetComponent<MatchSelectHandler>();
                m_FightHandler = GetComponent<FightHandler>();
                m_HeartBeatHandler = GetComponent<HeartBeatHandler>();

                ((HeartBeatHandler)m_HeartBeatHandler).Init();
                //CancelInvoke(methodName);
                //InvokeRepeating(methodName, 1f / 60, 1f / 60);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 接收服务器消息
        /// </summary>
        /// <param name="value"></param>
        private void MessageReceive(SocketModel value)
        {
            try
            {
                switch (value.Type)
                {
                    case Protocols.LOGIN_PROTOCOL:
                        m_LoginHandler.MessageReceive(value);
                        break;
                    case Protocols.MATCH_PROTOCOL:
                        m_MatchHandler.MessageReceive(value);
                        break;
                    case Protocols.MATCHSELECT_PROTOCOL:
                        m_MatchSelectHandler.MessageReceive(value);
                        break;
                    case Protocols.FIGHT_PROTOCOL:
                        m_FightHandler.MessageReceive(value);
                        break;
                    case Protocols.HEARTBEAT_PROTOCOL:
                        m_HeartBeatHandler.MessageReceive(value);
                        break;
                    default: break;
                }
            }
            catch (Exception) { }
        }
    }
}