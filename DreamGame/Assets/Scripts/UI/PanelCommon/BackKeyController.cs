using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PC上esc键和Android 手机返回键管理类
/// </summary>
public class BackKeyController : MonoBehaviour
{
	private bool lockEscape = false;
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey (KeyCode.Escape)) {
			if (!lockEscape) {
				lockEscape = true;
				PanelController.GetInstance().BackKey();
			}
		} else {
			lockEscape = false;
		}
	}
}
