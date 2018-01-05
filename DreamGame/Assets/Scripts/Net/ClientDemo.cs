using UnityEngine;
using System.Collections;
using Net.Client;
using Net.Protocol;
using Net.Protocol.DTO;
using Net.Framework.Server;

public class ClientDemo : MonoBehaviour
{
    private string account;
    private string password;
    void Start()
    {
        EventCenter.GetInstance().AddListener<AccountModel>(ClientEvent.AccountOnline, OnAccountOnline);
        EventCenter.GetInstance().AddListener<AccountModel>(ClientEvent.AccountInfo, OnAccountInfo);
        EventCenter.GetInstance().AddListener(ClientEvent.LoginSuccess, OnLoginSuccess);
    }
    private void OnLoginSuccess() { Debug.Log("LoginSuccess"); }
    private void OnAccountOnline(AccountModel arg)
    {
        account = arg.Account; password = arg.Password;
        Debug.Log("Online:" + arg.Account + "," + arg.Password);
    }
    private void OnAccountInfo(AccountModel arg)
    {
        account = arg.Account; password = arg.Password;
        Debug.Log("Info:" + arg.Account + "," + arg.Password);
    }



    void OnGUI()
    {
        GUILayout.Label("账号:" + account + ",密码:" + password);
        if (GUILayout.Button("连接"))
        {
            Debug.Log(Connection.Instance.Connect("127.0.0.1", 10000) + "___连接");
        }
        if (GUILayout.Button("关闭"))
        {
            Debug.Log(Connection.Instance.Close() + "___关闭");
        }
        if (GUILayout.Button("注册:小明"))
        {
            AccountModel model = new AccountModel();
            model.Account = "小明";
            model.Password = "123456";
            this.WriteMessage(Protocols.LOGIN_PROTOCOL, 0, Login.RegisterCREQ, model);
        }
        if (GUILayout.Button("登录:小明"))
        {
            AccountModel model = new AccountModel();
            model.Account = "小明";
            model.Password = "123456";
            this.WriteMessage(Protocols.LOGIN_PROTOCOL, 0, Login.LoginCREQ, model);
        }
        if (GUILayout.Button("注册:小亮"))
        {
            AccountModel model = new AccountModel();
            model.Account = "小亮";
            model.Password = "123456";
            this.WriteMessage(Protocols.LOGIN_PROTOCOL, 0, Login.RegisterCREQ, model);
        }
        if (GUILayout.Button("登录:小亮"))
        {
            AccountModel model = new AccountModel();
            model.Account = "小亮";
            model.Password = "123456";
            this.WriteMessage(Protocols.LOGIN_PROTOCOL, 0, Login.LoginCREQ, model);
        }
        if (GUILayout.Button("开始排队"))
        {
            this.WriteMessage(Protocols.MATCH_PROTOCOL, 0, Match.EnterCREQ, null);
        }
        if (GUILayout.Button("取消排队"))
        {
            this.WriteMessage(Protocols.MATCH_PROTOCOL, 0, Match.ExitCREQ, null);
        }
    }
}