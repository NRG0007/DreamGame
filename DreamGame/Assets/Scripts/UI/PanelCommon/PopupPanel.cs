using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupPanel : GamePanel
{
	//protected IPopupMessageData popMessageData;

	protected override void InitData ()
	{
		base.InitData ();
		//popMessageData = PopupMessageController.GetInstance ().popMessageData;
	}

	protected override void ShowData ()
	{
		base.ShowData ();
		//popMessageData = PopupMessageController.GetInstance ().popMessageData;
	}

	/// <summary>
	/// 返回上级
	/// </summary>
	public void Close ()
	{
		PanelController.GetInstance ().ClosePopupPanel ();
	}
}
