/// <summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameEngine.Events;
using GameEngine.API;

public class GameView : MonoBehaviour
{
	[HideInInspector]
	public string path;
	public Image mask;
	/// <summary>
	/// View打开动画时间
	/// </summary>
	public float moveInTime = 0.3f;
	/// <summary>
	/// View关闭动画时间
	/// </summary>
	public float moveOutTime = 0.3f;
	/// <summary>
	/// 延迟销毁时间，一秒为单位
	/// </summary>
	public float delayDestroyTime = int.MaxValue;
	/// <summary>
	/// 返回键开关 ,默认是true
	/// </summary>
	protected bool escapeFlg = true;
	/// <summary>
	/// 页面显示完成
	/// </summary>
	private bool isShowComplete = false;

	//	private Stack<GameView> childViewStack;
	/// <summary>
	/// 第一次初始化View标志
	/// </summary>
	private bool firstInitializeFlg = true;

	/// <summary>
	/// View状态
	/// </summary>
	private GameViewState gameViewState = GameViewState.CLOSE;
	/// <summary>
	/// 动画回调函数
	/// </summary>
	protected Action<MoveAnimationState> onMoveInOrOutComplete;
	protected bool isIgnoreLoading = false;

	public RectTransform rectTransform {
		get {
			return this.transform as RectTransform;
		}
	}

	/// <summary>
	/// true会显示，false为隐藏.
	/// </summary>
	public bool isActive { get; protected set; }

	/*public void Show (bool isFoce, Action<MoveAnimationState> onMoveInComplete)
	{ 
		gameObject.SetActive (true);
		gameViewState = GameViewState.SHOW;
		//判断是否是第一次初始化
		if (firstInitializeFlg) {
			firstInitializeFlg = false;
			AddEvent ();
			InitData ();
		}
		ShowData ();
		onMoveInOrOutComplete = onMoveInComplete;

		//开始进入动画
		if (onMoveInOrOutComplete != null) {
			onMoveInOrOutComplete (MoveAnimationState.StartMoveIn);
		}
		//强制显示View
		if (isFoce) {
			OnMoveInComplete ();
		} else {
			PlayMoveInAnimation ();
		}
	}*/

	/// <summary>
	/// 显示页面.
	/// </summary>
	/// <param name="isFoce">是否强制显示页面.</param>
	/// <param name="onMoveInComplete">显示页面动画完成.</param>
	public void Show (bool isFoce = false, Action<MoveAnimationState> onMoveInComplete = null)
	{ 
		PanelController.GetInstance ().StartCoroutine (PShow (isFoce, onMoveInComplete));
	}

	/// <summary>
	/// 异步显示页面
	/// </summary>
	/// <returns>IEnumerator.</returns>
	/// <param name="isFoce">是否强制显示页面.</param>
	/// <param name="onMoveInComplete">显示页面动画完成.</param>
	private IEnumerator PShow (bool isFoce, Action<MoveAnimationState> onMoveInComplete)
	{
		//判断是否是第一次初始化
		if (firstInitializeFlg) {
			AddEvent ();
			SingleLoadData ();
			if (!isIgnoreLoading) {
				//api加载数据等待
				while (!APIManager.GetInstance ().IsNoApiPerform) {
					yield return new WaitForFixedUpdate ();
				}
			}
		}

		EveryTimeLoadData ();
		if (!isIgnoreLoading) {
			//api加载数据等待
			while (!APIManager.GetInstance ().IsNoApiPerform) {
				yield return new WaitForFixedUpdate ();
			}
		}

		gameObject.SetActive (true);
		gameViewState = GameViewState.SHOW;
		//判断是否是第一次初始化
		if (firstInitializeFlg) {
			firstInitializeFlg = false;
			InitData ();
		}
		ShowData ();

		onMoveInOrOutComplete = onMoveInComplete;

		//开始进入动画
		if (onMoveInOrOutComplete != null) {
			onMoveInOrOutComplete (MoveAnimationState.StartMoveIn);
		}
		//强制显示View
		if (isFoce) {
			OnMoveInComplete ();
		} else {
			PlayMoveInAnimation ();
		}
	}

	/// <summary>
	/// 添加事件
	/// </summary>
	protected virtual void AddEvent ()
	{
		
	}

	/// <summary>
	/// 移除事件
	/// </summary>
	protected virtual void RemoveEvent ()
	{

	}

	/// <summary>
	/// 单次加载数据
	/// </summary>
	protected virtual void SingleLoadData ()
	{
		
	}

	/// <summary>
	/// 每次打开页面都加载数据
	/// </summary>
	protected virtual void EveryTimeLoadData ()
	{

	}


	/// <summary>
	/// 第一次打开View初始化数据方法
	/// </summary>
	protected virtual void InitData ()
	{

	}

	/// <summary>
	/// 每一次打开View初始化数据方法
	/// </summary>
	protected virtual void ShowData ()
	{
	}


	/// <summary>
	/// 如果是view，允许调用此方法来关闭，如果是 panel，不能直接调用此方法，只能ViewController类中调用
	/// </summary>
	/// <param name="isFoceHide">假如是true不带动画关闭view，false带动画关闭view</param>
	public void Close (bool isFoce, Action<MoveAnimationState> onMoveOutComplete, bool isJumpPanel = false)
	{
		onMoveInOrOutComplete = onMoveOutComplete;
		gameViewState = GameViewState.CLOSE;
		if (OnClose () || isJumpPanel) {
			if (isFoce) {
				OnMoveOutComplete (MoveAnimationState.EndMoveOutAndDestroy);
			} else {
				PlayMoveOutAnimation ();
			}
		} else {
			OnMoveOutComplete (MoveAnimationState.EndMoveOut);
		}
	}

	protected virtual bool OnClose ()
	{
		return true;
	}

	protected virtual void PlayMoveInAnimation ()
	{
		OnMoveInComplete ();
	}

	/// <summary>
	/// 打开view动画完成回调
	/// </summary>
	protected virtual void OnMoveInComplete ()
	{
		isActive = true;
		if (onMoveInOrOutComplete != null) {
			onMoveInOrOutComplete (MoveAnimationState.EndMoveIn);
			onMoveInOrOutComplete = null;
		}
		isShowComplete = true;
		//EngineEventManager.GetInstance ().DispatchEvent (EngineEventType.ON_VIEW_SHOW_OK, this);
	}

	protected virtual void PlayMoveOutAnimation ()
	{
		OnMoveOutComplete (MoveAnimationState.EndMoveOutAndDestroy);
	}

	/// <summary>
	/// 关闭view动画完成回调
	/// </summary>
	protected virtual void OnMoveOutComplete (MoveAnimationState value)
	{
		if (value == MoveAnimationState.EndMoveOutAndDestroy) {
			GameObject.Destroy (gameObject);
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

	private void Awake ()
	{
		if (!Application.isPlaying) {
			return;
		}
		firstInitializeFlg = true;
//		childViewStack = new Stack<GameView> ();
		OnAwake ();
	}

	protected virtual void OnAwake ()
	{
		this.gameObject.SetActive (false);
	}

	private void Start ()
	{
		this.OnStart ();
	}

	protected virtual void OnStart ()
	{
		
	}

	private void Update ()
	{
		if (!isShowComplete)
			return;
		OnUpdate ();
	}

	protected virtual void OnUpdate ()
	{

	}

	/// <summary>
	/// 返回键方法
	/// </summary>
	public void BackKey ()
	{
		//当此页面不支持返回键关闭或者页面未显示成功就直接返回
		if (!this.escapeFlg || !isShowComplete) {
			return;
		}
		OnBackKey ();
	}

	/// <summary>
	/// 返回键回调方法
	/// </summary>
	protected virtual void OnBackKey ()
	{
		
	}

	protected virtual void OnDestroy ()
	{
		firstInitializeFlg = true;
		RemoveEvent ();
//		GameView childView = null;
//		while (childViewStack != null && childViewStack.Count > 0) {
//			childView = childViewStack.Pop ();
//			childView.Close ();
//		}
//		ViewController.RemoveGameView (this);
	}

	/// <summary>
	/// 添加子View
	/// </summary>
	/// <param name="childView">GameView.</param>
	//	public void AddChildView (GameView childView)
	//	{
	//		childViewStack.Push (childView);
	//	}
}

public enum GameViewState
{
	/// <summary>
	/// 显示
	/// </summary>
	SHOW,
	/// <summary>
	/// 隐藏
	/// </summary>
	HIDE,
	/// <summary>
	/// 关闭
	/// </summary>
	CLOSE
}