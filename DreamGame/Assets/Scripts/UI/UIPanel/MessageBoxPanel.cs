/*
  备 注：
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxPanel : PopupPanel
{
    public Text title, msg;
    public Button BtnOk, BtnCancel;

    private MessageBoxController MC { get { return MessageBoxController.GetInstance(); } }

    protected override void OnStart()
    {
        base.OnStart();
        BindListener();
        SetData();
    }

    private void BindListener()
    {
        BtnOk.onClick.AddListener(() =>
        {
            if (MC.data.okAction != null)
                MC.data.okAction();
            Close();
        });

        BtnCancel.onClick.AddListener(() =>
        {
            if (MC.data.cancelAction != null)
                MC.data.cancelAction();
            Close();
        });
    }

    private void SetData()
    {
        title.text = MC.data.title;
        msg.text = MC.data.msg;
        if (MC.data.messageBoxType == MessageBoxType.Both)
        {
            BtnCancel.gameObject.SetActive(true);
            BtnOk.transform.localPosition = new Vector3(-70, 0, 0);
            BtnCancel.transform.localPosition = new Vector3(70, 0, 0);
        }
        else
        {
            BtnOk.transform.localPosition = Vector3.zero;
            BtnCancel.gameObject.SetActive(false);
        }
    }



}
