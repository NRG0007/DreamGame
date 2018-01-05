using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : GamePanel {
	/// <summary>
	/// 返回上级
	/// </summary>
	public void Close ()
	{
		PanelController.GetInstance().CloseTutorialPanel();
	}
}
