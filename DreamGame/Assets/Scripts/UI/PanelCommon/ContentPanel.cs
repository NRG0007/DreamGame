using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ContentPanel : GamePanel
{
    protected void ShowMessagePanel(string title, string msg, MessageBoxType messageBoxType = MessageBoxType.OK, UnityAction ok = null, UnityAction cancel = null)
    {
        MessageBoxController.GetInstance().ShowMessagePanel(title, msg, messageBoxType, ok, cancel);
    }
}
