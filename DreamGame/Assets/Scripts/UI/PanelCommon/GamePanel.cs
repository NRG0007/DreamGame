using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameEngine.Events;

public class GamePanel : GameView
{

	/// <summary>
	/// 上一个页面的类型
	/// </summary>
	public virtual Type lastGamePanelType{ get; set; }

	protected override void OnMoveOutComplete (MoveAnimationState value)
	{
		//当状态等于销毁时
		if (value == MoveAnimationState.EndMoveOutAndDestroy) {
			this.lastGamePanelType = null;
			gameObject.SetActive (false);
			isActive = false;
		}
		//		foreach (GameView item in childViewStack) {
		//			item.gameObject.SetActive (false);
		//		}
		if (onMoveInOrOutComplete != null) {
			onMoveInOrOutComplete (value);
			onMoveInOrOutComplete = null;
		}
	}

    protected void AddEventListener(string eventType, EngineEventManager.EventCallback eventCallback)
    {
        EngineEventManager.GetInstance().AddEventListener(eventType, eventCallback);
    }

    protected void RemoveEventListener(string eventType, EngineEventManager.EventCallback eventCallback)
    {
        EngineEventManager.GetInstance().RemoveEventListener(eventType, eventCallback);
    }

    protected void DispatchEvent(string eventType,object eventParams=null)
    {
        EngineEventManager.GetInstance().DispatchEvent(eventType, eventParams);
    }

    protected override void OnBackKey()
    {
        PanelController.GetInstance().ClosePanel(this.GetType().BaseType);
    }

}
