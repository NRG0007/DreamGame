using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemPanel : GamePanel {
	/// <summary>
	/// 返回上级
	/// </summary>
	public void Close ()
	{
		PanelController.GetInstance().CloseSytemPanel();
	}
}
