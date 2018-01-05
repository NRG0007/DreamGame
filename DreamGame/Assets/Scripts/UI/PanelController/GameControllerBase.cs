using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine;
using GameEngine.Events;

public class GameControllerBase<T> : AbsSingleBase<T> where T : AbsSingleBase<T>, new()
{

	public GameControllerBase()
	{
		AddEvent();
	}
	/// <summary>
	/// 析构函数
	/// </summary>
	~GameControllerBase()
	{
		RemoveEvent();
	}
	/// <summary>
	/// 添加监听事件
	/// </summary>
	protected virtual void AddEvent()
	{
		
	}
	/// <summary>
	/// 移除监听事件
	/// </summary>
	protected virtual void RemoveEvent()
	{
		
	}

    protected void AddEventListener(string eventType, EngineEventManager.EventCallback eventCallback)
    {
        EngineEventManager.GetInstance().AddEventListener(eventType, eventCallback);
    }

    protected void RemoveEventListener(string eventType, EngineEventManager.EventCallback eventCallback)
    {
        EngineEventManager.GetInstance().RemoveEventListener(eventType, eventCallback);
    }

    protected void DispatchEvent(string eventType, object eventParams = null)
    {
        EngineEventManager.GetInstance().DispatchEvent(eventType, eventParams);
    }
}
