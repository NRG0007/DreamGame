using Net.Framework.Server;
using Net.Protocol;
using Net.Protocol.DTO;
using System;
using UnityEngine;

namespace Net.Client
{
    /// <summary>
    /// 登录处理
    /// </summary>
    public class LoginHandler : MonoBehaviour, IBaseHandler
    {
        public void MessageReceive(SocketModel message)
        {
            try
            {
                switch (message.Command)
                {
                    case Login.RegisterSRES:
                        OnRegister(message.GetMessage<byte>());
                        break;
                    case Login.LoginSRES:
                        OnLogin(message.GetMessage<byte>());
                        break;
                    case Login.InfoSRES:
                        OnInfo(message.GetMessage<AccountModel>());
                        break;
                    case Login.CreateSRES:
                        OnCreate(message.GetMessage<bool>());
                        break;
                    case Login.OnlineSRES:
                        Online(message.GetMessage<AccountModel>());
                        break;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="value"></param>
        private void OnInfo(AccountModel value)
        {
            try
            {
                if (value != null)
                {
                    EventCenter.GetInstance().Broadcast<AccountModel>(ClientEvent.AccountInfo, value);

                    //this.WriteMessage(Protocols.LOGIN_PROTOCOL, 0, Login.OnlineCREQ, null);
                }
                else
                {
                    EventCenter.GetInstance().Broadcast(ClientEvent.NoAccountInfo);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="value"></param>
        private void OnRegister(byte value)
        {
            try
            {
                switch (value)
                {
                    case Login.RegisterError:
                        EventCenter.GetInstance().Broadcast<byte>(ClientEvent.RegisterError, value);
                        break;
                    case Login.RegisterSuccess:
                        EventCenter.GetInstance().Broadcast(ClientEvent.RegisterSuccess);
                        break;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="value"></param>
        private void OnLogin(byte value)
        {
            try
            {
                switch (value)
                {
                    case Login.LoginError:
                        EventCenter.GetInstance().Broadcast<byte>(ClientEvent.LoginError, value);
                        break;
                    case Login.LoginSuccess:
                        EventCenter.GetInstance().Broadcast(ClientEvent.LoginSuccess);
                        this.WriteMessage(Protocols.LOGIN_PROTOCOL, 0, Login.OnlineCREQ, null);
                        break;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 在线
        /// </summary>
        /// <param name="value"></param>
        private void Online(AccountModel value)
        {
            try
            {
                if (value != null)
                {
                    EventCenter.GetInstance().Broadcast<AccountModel>(ClientEvent.AccountOnline, value);
                }
                else
                {
                    EventCenter.GetInstance().Broadcast(ClientEvent.NoAccountInfo);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="value"></param>
        private void OnCreate(bool value)
        {
            try
            {
                EventCenter.GetInstance().Broadcast<bool>(ClientEvent.AccountCreate, value);

                if (value)
                {
                    this.WriteMessage(Protocols.LOGIN_PROTOCOL, 0, Login.OnlineCREQ, null);
                }
            }
            catch (Exception) { }
        }
    }
}