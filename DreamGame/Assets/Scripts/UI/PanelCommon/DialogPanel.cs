using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPanel : GamePanel {
	/// <summary>
	/// 是否隐藏父PopupPanel
	/// </summary>
	public bool isHideLastGamePanel = true;
	/// <summary>
	/// 父PopupPanel
	/// </summary>
	[HideInInspector]
	public DialogPanel lastDialogPanel;
	/// <summary>
	/// 返回上级
	/// </summary>
	public void Close ()
	{
		PanelController.GetInstance().CloseDialogPanel();
	}
}
