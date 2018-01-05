/*
  备 注：
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MessageBoxType
{
    OK,
    Both
}

public class MessageBoxController : GameControllerBase<MessageBoxController>
{
    public MessageBoxData data;

    public void ShowMessagePanel(string title, string msg, MessageBoxType messageBoxType = MessageBoxType.OK, UnityAction ok = null, UnityAction cancel = null)
    {
        if (data == null)
            data = new MessageBoxData();
        data.title = title;
        data.msg = msg;
        data.messageBoxType = messageBoxType;
        data.okAction = ok;
        data.cancelAction = cancel;
        PanelController.ShowPanel<MessageBoxPanel>();
    }

}

public class MessageBoxData
{
    public string title;
    public string msg;
    public MessageBoxType messageBoxType;
    public UnityAction okAction;
    public UnityAction cancelAction;
}

