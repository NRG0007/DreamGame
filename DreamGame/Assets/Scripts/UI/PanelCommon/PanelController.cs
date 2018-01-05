using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using GameEngine.Res;
using GameEngine;
using GameEngine.Events;
using GameEngine.Sound;
using UnityEngine.EventSystems;

public class PanelController : SingleMonoBehaviour<PanelController>
{
	public Transform canvas;
	private GameObject eventSystem;
	private  Stack<ContentPanel> contentPanelList = null;
	private  DialogPanel dialogGamePanel = null;
	private  HeaderPanel headerPanel;
	private  FooterPanel footerPanel;
	private  GameBGPanel gameBGPanel;
	private  PopupPanel popupPanel;
	private  SystemPanel systemPanel;
	private  TutorialPanel tutorialPanel;
	private GameObject backKeyController;
	/// <summary>
	/// 当前Panel类型
	/// </summary>
	private ContentPanel currentContentPanel;
	/// <summary>
	/// Panel关闭等待销毁队列
	/// </summary>
	private Dictionary<string,Spawn> spawnDestroyDic = null;

	public RectTransform uiPanelRootContainer;
	/// <summary>
	/// 游戏背景容器
	/// </summary>
	private RectTransform gameBGContainer;
	/// <summary>
	/// 游戏页面容器
	/// </summary>
	private RectTransform contentPanelContainer;
	/// <summary>
	/// 游戏header 容器
	/// </summary>
	private RectTransform headerPanelContainer;
	/// <summary>
	/// 游戏footer 容器
	/// </summary>
	private RectTransform footerPanelContainer;
	/// <summary>
	/// 游戏弹出框view 容器
	/// </summary>
	private RectTransform dialogPanelContainer;
	/// <summary>
	/// 游戏popup 容器
	/// </summary>
	private RectTransform popupPanelContainer;
	/// <summary>
	/// 系统前置界面 容器
	/// </summary>
	private RectTransform systemPanelContainer;
	/// <summary>
	/// 新手界面
	/// </summary>
	private RectTransform tutorialPanelContainer;
	/// <summary>
	/// 弹出框遮罩
	/// </summary>
	private Image dialogPanelContainerMask;
	/// <summary>
	/// 对话框框遮罩
	/// </summary>
	private Image popupPanelContainerMask;
	/// <summary>
	/// 系统对话框框遮罩
	/// </summary>
	private Image systemPanelContainerMask;

	private RectTransform uiMask = null;

	protected override void OnAwake ()
	{
		base.OnAwake ();

	}

	protected override void OnStart ()
	{
		base.OnStart ();
		eventSystem = GameObject.Find ("EventSystem");

		contentPanelList = new Stack<ContentPanel> ();
		spawnDestroyDic = new Dictionary<string, Spawn> ();
		DontDestroyOnLoad (canvas.gameObject);
		DontDestroyOnLoad (this.transform.parent);

		uiPanelRootContainer = CreateBaseObj ("UIPanelRootContainer", canvas);

		backKeyController = CreateBaseObj ("backKeyController", uiPanelRootContainer).gameObject;
		backKeyController.AddComponent<BackKeyController> ();

		gameBGContainer = CreateBaseObj ("GameBGContainer", uiPanelRootContainer);
		//Image image = gameBGContainer.gameObject.AddComponent<Image> ();
		//image.color = Color.black;
		//image.raycastTarget = false;

		contentPanelContainer = CreateBaseObj ("ContentPanelContainer", uiPanelRootContainer);

		headerPanelContainer = CreateBaseObj ("HeaderPanelContainer", uiPanelRootContainer);

		footerPanelContainer = CreateBaseObj ("FooterPanelContainer", uiPanelRootContainer);

		dialogPanelContainer = CreateBaseObj ("DialogPanelContainer", uiPanelRootContainer);

		dialogPanelContainerMask = CreateBaseObj ("dialogPanelContainerMask", dialogPanelContainer).gameObject.AddComponent<Image> ();
		dialogPanelContainerMask.color = new Color (0, 0, 0, 128 / 255f);
		dialogPanelContainerMask.gameObject.SetActive (false);

		popupPanelContainer = CreateBaseObj ("PopupPanelContainer", uiPanelRootContainer);
		popupPanelContainerMask = popupPanelContainer.gameObject.AddComponent<Image> ();
		popupPanelContainerMask.color = new Color (0, 0, 0, 128 / 255f);
		popupPanelContainerMask.enabled = false;

		tutorialPanelContainer = CreateBaseObj ("TutorialPanelContainer", uiPanelRootContainer);

		systemPanelContainer = CreateBaseObj ("SystemPanelContainer", uiPanelRootContainer);
		systemPanelContainerMask = systemPanelContainer.gameObject.AddComponent<Image> ();
		systemPanelContainerMask.color = new Color (0, 0, 0, 128 / 255f);
		systemPanelContainerMask.enabled = false;


		uiMask = CreateBaseObj ("UIMask", uiPanelRootContainer);
		uiMask.gameObject.AddComponent<Image> ().color = new Color (0, 0, 0, 0.01f);
		HideUIMask ();
	}

	private void ShowUIMask ()
	{
		uiMask.gameObject.SetActive (true);
		//EngineEventManager.GetInstance ().DispatchEvent (new EngineEvent (EngineEventType.WAIT));
	}

	private void HideUIMask ()
	{
		uiMask.gameObject.SetActive (false);
	}

	private void Update ()
	{
		CheckSpawnDestroy ();
	}

	/// <summary>
	/// 创建UI结构对象
	/// </summary>
	/// <returns>RectTransform.</returns>
	/// <param name="objName">对象名字.</param>
	private RectTransform CreateBaseObj (string objName, Transform parent)
	{
		GameObject obj = new GameObject (objName);
		obj.layer = parent.gameObject.layer;
		obj.transform.parent = parent;
		RectTransform rectTransform = obj.AddComponent<RectTransform> ();
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.localPosition = Vector2.zero;
		rectTransform.sizeDelta = Vector2.zero;
		rectTransform.localScale = Vector3.one;

		return rectTransform;
	}

	/// <summary>
	/// 获取Panel
	/// </summary>
	/// <returns>The view.</returns>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	private void GetPanel (Type type, Transform parent, Action<GamePanel> callBack)
	{
		GamePanel t = null;
		t = GetPanelInSpawnDestroy (type);
		if (t != null) {
			callBack (t);
			return;
		}
		PrefabInfo prefabInfo = PrefabPaths.GetPath (type);
		GameAssetManager.InstantiatePrefabAsync<GamePanel> (prefabInfo.path, parent, callBack, prefabInfo.isForceLocal);

	}

	/// <summary>
	/// 移除Panel
	/// </summary>
	/// <param name="gamePanel">Game view.</param>
	public static void RemoveGamePanel (GamePanel gamePanel)
	{
		UnityEngine.GameObject.Destroy (gamePanel.gameObject);
	}


	/// <summary>
	/// 显示一个GamePanel.
	/// </summary>
	/// <returns>The view.</returns>
	/// <param name="isForceShowCurrentPanel">假如是true，则是强制打开view，不带动画；假如是false，则是带动画打开view.</param>
	/// <param name="isForceCloseLastPanel">假如是true，则是强制关闭view，不带动画；假如是false，则是带动画关闭view.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	private static void ShowPanel<T> (bool isForceShowCurrentPanel, bool isForceCloseLastPanel)where T:GamePanel
	{
		ShowPanel (typeof(T), isForceShowCurrentPanel, isForceCloseLastPanel);
	}

	/// <summary>
	/// 显示一个GamePanel.
	/// </summary>
	/// <returns>The view.</returns>
	/// <param name="isForceShowCurrentPanel">假如是true，则是强制打开view，不带动画；假如是false，则是带动画打开view.</param>
	/// <param name="isForceCloseLastPanel">假如是true，则是强制关闭view，不带动画；假如是false，则是带动画关闭view.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	private static void ShowPanel (Type t, bool isForceShowCurrentPanel, bool isForceCloseLastPanel)
	{
		PanelController.GetInstance ().PShowPanel (t, isForceShowCurrentPanel, isForceCloseLastPanel);
	}

	/// <summary>
	/// 显示一个GamePanel.
	/// </summary>
	/// <returns>The view.</returns>
	/// <param name="isForceShowCurrentPanel">假如是true，则是强制打开view，不带动画；假如是false，则是带动画打开view.</param>
	/// <param name="isForceCloseLastPanel">假如是true，则是强制关闭view，不带动画；假如是false，则是带动画关闭view.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	private void PShowPanel (Type type, bool isForceShowCurrentPanel, bool isForceCloseLastPanel)
	{
		switch (type.BaseType.ToString ()) {
		case "FooterPanel":
			PanelController.GetInstance ().ShowFooterPanel (type);
			break;
		case "HeaderPanel":
			PanelController.GetInstance ().ShowHeaderPanel (type);
			break;
		case "GameBGPanel":
			PanelController.GetInstance ().ShowGBPanel (type, isForceShowCurrentPanel, isForceCloseLastPanel);
			break;
		case "ContentPanel":
			PanelController.GetInstance ().ShowContentPanel (type, isForceShowCurrentPanel, isForceCloseLastPanel);
			break;
		case "DialogPanel":
			PanelController.GetInstance ().ShowDialogGamePanel (type, isForceShowCurrentPanel, isForceCloseLastPanel);
			break;
		case "PopupPanel":
			PanelController.GetInstance ().ShowPopupGamePanel (type, isForceShowCurrentPanel, isForceCloseLastPanel);
			break;
		case "SystemPanel":
			PanelController.GetInstance ().ShowSystemPanel (type, isForceShowCurrentPanel, isForceCloseLastPanel);
			break;
		case "TutorialPanel":
			PanelController.GetInstance ().ShowTutorialPanel (type);
			break;
		default:
			throw new Exception ("没有你想创建的Panel类型：" + type.ToString ());
		}

	}

	#region 显示游戏footerPanel

	/// <summary>
	/// 显示游戏footerPanel
	/// </summary>
	/// <typeparam name="T">CustomRootGamePanel 类型.</typeparam>
	private void ShowFooterPanel (Type type)
	{
		if (footerPanel != null) {
			footerPanel.Close (false, (MoveAnimationState value) => {
				GetPanel (type, footerPanelContainer, c => {
					footerPanel = c as FooterPanel;
					footerPanel.Show (true, OnFooterMoveInComplete);
				});
			});
			footerPanel = null;
		} else {
			GetPanel (type, footerPanelContainer, c => {
				footerPanel = c as FooterPanel;
				footerPanel.Show (true, OnFooterMoveInComplete);
			});
		}
	}

	private void OnFooterMoveInComplete (MoveAnimationState value)
	{
		
	}

	#endregion

	#region 显示游戏HeaderPanel

	/// <summary>
	/// 显示游戏footerPanel
	/// </summary>
	/// <typeparam name="T">CustomRootGamePanel 类型.</typeparam>
	private void ShowHeaderPanel (Type type)
	{
		if (headerPanel != null) {
			headerPanel.Close (false, (MoveAnimationState value) => {
				GetPanel (type, headerPanelContainer, c => {
					headerPanel = c as HeaderPanel;
					headerPanel.Show (true, OnHeaderMoveInComplete);
				});

			});
			headerPanel = null;
		} else {
			GetPanel (type, headerPanelContainer, c => {
				headerPanel = c as HeaderPanel;
				headerPanel.Show (true, OnHeaderMoveInComplete);
			});
		}
	}

	private void OnHeaderMoveInComplete (MoveAnimationState value)
	{

	}

	#endregion

	#region 显示游戏BG

	/// <summary>
	/// 显示游戏footerPanel
	/// </summary>
	/// <typeparam name="T">CustomRootGamePanel 类型.</typeparam>
	private void ShowGBPanel (Type type, bool isForceShowCurrentPanel, bool isForceCloseLastPanel)
	{
		if (gameBGPanel != null) {
			gameBGPanel.Close (isForceCloseLastPanel, (MoveAnimationState value) => {
				GetPanel (type, gameBGContainer, c => {
					gameBGPanel = c as GameBGPanel;
					gameBGPanel.Show (isForceShowCurrentPanel, OnGameBGMoveInComplete);
				});
			});

			gameBGPanel = null;
		} else {
			GetPanel (type, gameBGContainer, c => {
				gameBGPanel = c as GameBGPanel;
				gameBGPanel.Show (isForceShowCurrentPanel, OnGameBGMoveInComplete);
			});
		}
	}

	private void OnGameBGMoveInComplete (MoveAnimationState value)
	{

	}

	#endregion

	#region 显示弹出框Panel

	/// <summary>
	/// 显示弹出框Panel
	/// </summary>
	/// <param name="type">弹出框类型.</param>
	private void ShowPopupGamePanel (Type type, bool isForceShowCurrentPanel, bool isForceCloseLastPanel)
	{
		if (popupPanel != null) {
			popupPanel.Close (isForceCloseLastPanel, (MoveAnimationState value) => {
				AddSpawnDestroyDic (popupPanel);
				GetPanel (type, popupPanelContainer, c => {
					popupPanel = c as PopupPanel;
					popupPanel.Show (isForceShowCurrentPanel, OnPopupMoveInComplete);
				});

			});
		} else {
			GetPanel (type, popupPanelContainer, c => {
				popupPanel = c as PopupPanel;
				popupPanel.Show (isForceShowCurrentPanel, OnPopupMoveInComplete);
			});

		}
		popupPanelContainerMask.enabled = true;
	}

	private void OnPopupMoveInComplete (MoveAnimationState value)
	{

	}

	#endregion

	#region 显示游戏普通Panel

	/// <summary>
	/// 显示游戏普通Panel
	/// </summary>
	/// <returns>GamePanel.</returns>
	/// <param name="isForceShowCurrentPanel">假如是true，则是强制打开view，不带动画；假如是false，则是带动画打开view.</param>
	/// <param name="isForceCloseLastPanel">假如是true，则是强制关闭view，不带动画；假如是false，则是带动画关闭view.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	private void ShowContentPanel (Type type, bool isForceShowCurrentPanel, bool isForceCloseLastPanel)
	{
		if (contentPanelList.Count > 0) {
			//判断是否是同一个view
			if (contentPanelList.Peek ().GetType () == type) {
				return;
			}
			ShowUIMask ();
			GamePanel lastGamePanel = contentPanelList.Pop ();
			Type lastGamePanelType = lastGamePanel.GetType ();
			System.Action<MoveAnimationState> onMoveOutComplete = (MoveAnimationState value) => {
				AddSpawnDestroyDic (lastGamePanel);
				GetContentPanel (type, c => {
					if (c == null) {
						Debuger.LogError ("ShowContentPanel t is null.type:" + type);
					}
					c.lastGamePanelType = lastGamePanelType;
					c.Show (isForceShowCurrentPanel, OnContentMoveInComplete);
				});

			};

			lastGamePanel.Close (isForceCloseLastPanel, onMoveOutComplete, true);
		} else {
			ShowUIMask ();
			GetContentPanel (type, c => {
				c.Show (isForceShowCurrentPanel, OnContentMoveInComplete);
			});
		}
	}

	/// <summary>
	/// 获取游戏普通Panel
	/// </summary>
	/// <returns>The child game view.</returns>
	/// <param name="isForceShowCurrentPanel">假如是true，则是强制打开view，不带动画；假如是false，则是带动画打开view.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	private void GetContentPanel (Type type, Action<ContentPanel> callBack)
	{
		GetPanel (type, contentPanelContainer, c => {
			currentContentPanel = c as ContentPanel;
			contentPanelList.Push (currentContentPanel);
			callBack (currentContentPanel);
		});
	}

	/// <summary>
	/// 显示完成回调.
	/// </summary>
	private void OnContentMoveInComplete (MoveAnimationState value)
	{
		//当时开始进入动画时，移除短loading
		if (value == MoveAnimationState.StartMoveIn) {
			//EngineEventManager.GetInstance ().DispatchEvent (new EngineEvent (EngineEventType.WAIT_END));
			return;
		}
		HideUIMask ();
	}

	#endregion

	#region 显示游戏弹出框DialogGamePanel

	/// <summary>
	/// 显示游戏弹出框DialogGamePanel
	/// </summary>
	/// <returns>GamePanel.</returns>
	/// <param name="isForceShowCurrentPanel">假如是true，则是强制打开view，不带动画；假如是false，则是带动画打开view.</param>
	/// <param name="isForceCloseLastPanel">假如是true，则是强制关闭view，不带动画；假如是false，则是带动画关闭view.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	private void ShowDialogGamePanel (Type type, bool isForceShowCurrentPanel, bool isForceCloseLastPanel)
	{
		GetDialogGamePanel (type, c => {
			DialogPanel t = c as DialogPanel;
			if (t.isHideLastGamePanel) {
				if (dialogGamePanel != null) {
					dialogGamePanel.Close (isForceCloseLastPanel, (MoveAnimationState value) => {
						AddSpawnDestroyDic (dialogGamePanel);
						dialogGamePanel = t;
						dialogGamePanel.Show (isForceShowCurrentPanel, OnDialogMoveInComplete);
					});
				} else {
					dialogGamePanel = t;
					dialogGamePanel.Show (isForceShowCurrentPanel, OnDialogMoveInComplete);
				}
			} else {
				t.lastDialogPanel = dialogGamePanel;
				dialogGamePanel = t;
				dialogGamePanel.Show (isForceShowCurrentPanel, OnDialogMoveInComplete);
			}
		});

	}

	/// <summary>
	/// 显示游戏弹出框DialogGamePanel
	/// </summary>
	/// <returns>The child game view.</returns>
	/// <param name="isForceShowCurrentPanel">假如是true，则是强制打开view，不带动画；假如是false，则是带动画打开view.</param>
	/// <typeparam name="T">DialogGamePanel.</typeparam>
	private void GetDialogGamePanel (Type type, Action<GamePanel> callback)
	{
		dialogPanelContainerMask.transform.SetSiblingIndex (dialogPanelContainer.childCount);
		dialogPanelContainerMask.gameObject.SetActive (true);
		GetPanel (type, dialogPanelContainer, c => {
			if (c == null) {
				throw new Exception (type.ToString () + "游戏Dialog GamePanel不存在。");
			}
			c.transform.SetSiblingIndex (dialogPanelContainer.childCount);
			callback (c);
		});
	}

	/// <summary>
	/// 显示完成回调.
	/// </summary>
	/// <param name="value">true：关闭成功，false：关闭失败.</param>
	private void OnDialogMoveInComplete (MoveAnimationState value)
	{

	}

	#endregion

	/// <summary>
	/// 显示弹出框Panel
	/// </summary>
	/// <param name="type">弹出框类型.</param>
	private void ShowSystemPanel (Type type, bool isForceShowCurrentPanel, bool isForceCloseLastPanel)
	{
		if (systemPanel != null) {
			systemPanel.Close (isForceCloseLastPanel, (MoveAnimationState value) => {
				AddSpawnDestroyDic (systemPanel);
				GetPanel (type, systemPanelContainer, c => {

					systemPanel = c as SystemPanel;
					systemPanel.Show (isForceShowCurrentPanel, OnSystemMoveInComplete);	
				});
			});
			systemPanel = null;
		} else {
			GetPanel (type, systemPanelContainer, c => {
				systemPanel = c as SystemPanel;
				systemPanel.Show (isForceShowCurrentPanel, OnSystemMoveInComplete);
			});

		}
		systemPanelContainerMask.enabled = true;
	}

	private	void OnSystemMoveInComplete (MoveAnimationState s)
	{
		
	}

	private void ShowTutorialPanel (Type type)
	{
		if (tutorialPanel != null) {
			tutorialPanel.Close (false, (MoveAnimationState value) => {
				GetPanel (type, tutorialPanelContainer, c => {
					tutorialPanel = c as TutorialPanel;
					tutorialPanel.Show (true, OnTutorialMoveInComplete);
				});

			});
			footerPanel = null;
		} else {
			GetPanel (type, tutorialPanelContainer, c => {
				tutorialPanel = c as TutorialPanel;
				tutorialPanel.Show (true, OnTutorialMoveInComplete);
			});
		}
	}

	private void OnTutorialMoveInComplete (MoveAnimationState value)
	{

	}


	/// <summary>
	/// 关闭GamePanel
	/// </summary>
	/// <param name="gamePanel">Game view.</param>
	private void CloseGamePanel (GamePanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel)
	{
		switch (gamePanel.GetType ().BaseType.ToString ()) {
		case "GameBGPanel":
			CloseBGPanel ();
			break;
		case "HeaderPanel":
			CloseHeaderPanel ();
			break;
		case "FooterPanel":
			CloseFooterPanel ();
			break;
		case "ContentPanel":
			CloseContentPanel (gamePanel, isForceCloseCurrentPanel, isForceShowLastPanel);
			return;
		case "DialogPanel":
			CloseDialogGamePanel (gamePanel as DialogPanel, isForceCloseCurrentPanel, isForceShowLastPanel);
			break;
		case "PopupPanel":
			ClosePopupGamePanel (gamePanel, isForceCloseCurrentPanel, isForceShowLastPanel);
			break;
		case "SystemPanel":
			CloseSystemGamePanel (gamePanel, isForceCloseCurrentPanel, isForceShowLastPanel);
			break;
		case "TutorialPanel":
			CloseTutorialGamePanel (gamePanel, isForceCloseCurrentPanel, isForceShowLastPanel);
			break;
		default:
			throw new Exception ("没有你想关闭的Panel类型：" + gamePanel.GetType ().ToString ());
		}
		AddSpawnDestroyDic (gamePanel);
	}

	/// <summary>
	/// 关闭Header
	/// </summary>
	private void CloseHeaderPanel ()
	{
		if (headerPanel != null) {
			headerPanel.Close (false, null);
			headerPanel = null;
		} 
	}

	/// <summary>
	/// 关闭Footer
	/// </summary>
	private void CloseFooterPanel ()
	{
		if (footerPanel != null) {
			footerPanel.Close (false, null);
			footerPanel = null;
		} 
	}

	/// <summary>
	/// 关闭PopupPanel
	/// </summary>
	private void ClosePopupGamePanel (GamePanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel)
	{
		if (gamePanel != null) {
			Type lastGamePanelType = gamePanel.lastGamePanelType;
			gamePanel.Close (isForceCloseCurrentPanel, (MoveAnimationState value) => {
				if (lastGamePanelType != null) {
					ShowPanel (lastGamePanelType, isForceShowLastPanel, isForceCloseCurrentPanel);
				} else {
					popupPanelContainerMask.enabled = false;
				}
			});
			gamePanel = null;
		} 
	}

	/// <summary>
	/// 关闭SystemPanel
	/// </summary>
	private void CloseSystemGamePanel (GamePanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel)
	{
		if (gamePanel != null) {
			Type lastGamePanelType = gamePanel.lastGamePanelType;
			gamePanel.Close (isForceCloseCurrentPanel, (MoveAnimationState value) => {
				if (lastGamePanelType != null) {
					ShowPanel (lastGamePanelType, isForceShowLastPanel, isForceCloseCurrentPanel);
				} else {
					systemPanelContainerMask.enabled = false;
				}
			});
			gamePanel = null;
		}
	}

	/// <summary>
	/// 关闭tutorial
	/// </summary>
	private void CloseTutorialGamePanel (GamePanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel)
	{
		if (tutorialPanel != null) {
			tutorialPanel.Close (false, null);
			tutorialPanel = null;
		} 
	}


	/// <summary>
	/// 关闭游戏页面次根的子Panel
	/// </summary>
	/// <param name="gamePanel">Game view.</param>
	/// <param name="isForceCloseCurrentPanel">假如是true，则是强制关闭view，不带动画；假如是false，则是带动画关闭view.</param>
	/// <param name="isForceShowLastPanel">假如是true，则是强制打开view，不带动画；假如是false，则是带动画打开view.</param>
	private void CloseContentPanel (GamePanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel)
	{
		if (contentPanelList.Count > 0) {
			if (contentPanelList.Peek () != gamePanel) {
				throw new Exception ("当前关闭的" + gamePanel.name + "Panel和队里里面的" + contentPanelList.Peek ().name + "Panel不匹配。");
			}
		}
		Type lastGamePanelType = gamePanel.lastGamePanelType;
		gamePanel.Close (isForceCloseCurrentPanel, (MoveAnimationState value) => {
			if (value == MoveAnimationState.EndMoveOutAndDestroy) {
				currentContentPanel = null;
				AddSpawnDestroyDic (gamePanel);
				contentPanelList.Pop ();
				if (lastGamePanelType != null) {
					ShowPanel (lastGamePanelType, isForceShowLastPanel, true);
				}
			}
		});
	}

	/// <summary>
	/// 关闭游戏页面Dialog Panel
	/// </summary>
	/// <param name="gamePanel">Game view.</param>
	/// <param name="isForceCloseCurrentPanel">假如是true，则是强制关闭view，不带动画；假如是false，则是带动画关闭view.</param>
	/// <param name="isForceShowLastPanel">假如是true，则是强制打开view，不带动画；假如是false，则是带动画打开view.</param>
	private void CloseDialogGamePanel (DialogPanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel)
	{
		Type lastGamePanelType = gamePanel.lastGamePanelType;

		gamePanel.Close (isForceCloseCurrentPanel, (MoveAnimationState value) => {
			if (!gamePanel.isHideLastGamePanel && gamePanel.lastDialogPanel != null) {
				dialogPanelContainerMask.transform.SetSiblingIndex (dialogPanelContainerMask.transform.GetSiblingIndex () - 1);
				this.dialogGamePanel = gamePanel.lastDialogPanel;
			} else {
				if (lastGamePanelType != null) {
					ShowPanel (lastGamePanelType, isForceShowLastPanel, true);
				} else {
					dialogPanelContainerMask.gameObject.SetActive (false);
				}
			}
		});
	}

	/// <summary>
	/// 把close的view加入到销毁队里中
	/// </summary>
	/// <param name="gamePanel">GamePanel.</param>
	private void AddSpawnDestroyDic (GamePanel gamePanel)
	{
		string key = gamePanel.GetType ().ToString ();
		if (!spawnDestroyDic.ContainsKey (key)) {
			spawnDestroyDic.Add (key, new Spawn (gamePanel));
		}
	}

	/// <summary>
	/// 从销毁队列中获取一个GamePanel
	/// </summary>
	/// <returns>The view in spawn destroy.</returns>
	/// <param name="type">Type.</param>
	private GamePanel GetPanelInSpawnDestroy (Type type)
	{
		string key = type.ToString ();
		GamePanel gamePanel = null;
		if (spawnDestroyDic.ContainsKey (key)) {
			gamePanel = spawnDestroyDic [key].gamePanel;
			spawnDestroyDic.Remove (key);
		}
		return gamePanel; 
	}

	/// <summary>
	/// 销毁队列到期的Panel
	/// </summary>
	private void CheckSpawnDestroy ()
	{
		foreach (var item in spawnDestroyDic) {
			item.Value.delayTime += Time.deltaTime;
			// 达到延迟销毁时间销毁对象
			if (item.Value.isCanDestroy ()) {
				spawnDestroyDic.Remove (item.Key);
				PanelController.RemoveGamePanel (item.Value.gamePanel);
				return;
			}
		}
	}

	/// <summary>
	/// 显示Panel
	/// </summary>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void ShowPanel<T> ()where T:GamePanel
	{
		ShowPanel (typeof(T));
	}

	/// <summary>
	/// 显示Panel
	/// </summary>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void ShowPanel (Type t)
	{
		switch (t.ToString ()) {
		case "LogoSplashPanel":
			ShowPanel (t, false, false);
			break;
		case "UnitTeamPanel":
			ShowPanel (t, false, false);
			break;
		case "UnitListPanel":
			ShowPanel (t, false, false);
			break;
		case "BattleGroupPanel":
		case "MenuTopPanel":
		case "QuestPanel":
		case "QuestSelectedPanel":
		case "QuestStandByPanel":
		case "MyRoomHomePanel":
		case "UnitTopPanel":
		case "ScenarioChapterSelectMainPanel":
		case "GachaMainPanel":
		case "ShopPanel":
		case "MyPagePanel":
		case "TutorialView":
		case "BattlePvpPanel":
		case "PlayQuestPanel":
		case "WaitPanel":
		case "DownLoadCenterPanel":
		case "EventPanel":
		case "PresentBox":
		case "FriendPanel":
		case "GameMenuTopPanel":
		case "SilverShopPanel":
		case "BattleItemShopPanel":
			ShowPanel (t, false, false);
			break;
		default:
			ShowPanel (t, true, true);
			break;
		}
	}

	/// <summary>
	/// 关闭背景
	/// </summary>
	private void CloseBGPanel ()
	{
		if (gameBGPanel != null) {
			gameBGPanel.Close (false, null);
			AddSpawnDestroyDic (gameBGPanel);
			gameBGPanel = null;
		} 
	}

	/// <summary>
	/// 关闭Content Panel
	/// </summary>
	/// <returns><c>true</c>, 关闭成功, <c>false</c> 关闭失败.</returns>
	public void CloseContentPanel ()
	{
		if (currentContentPanel == null) {
			return;
		}
		switch (currentContentPanel.GetType ().ToString ()) {
		case "MyPagePanel":
			break;
		default:
			CloseGamePanel (currentContentPanel, false, false);
			break;
		}
	}

	/// <summary>
	/// 关闭弹出框Panel
	/// </summary>
	/// <returns><c>true</c>, 关闭成功, <c>false</c> 关闭失败.</returns>
	public void CloseDialogPanel ()
	{
		if (dialogGamePanel == null) {
			return;
		}
		DialogPanel temp = dialogGamePanel;
		dialogGamePanel = null;
		CloseGamePanel (temp, false, false);
	}

	/// <summary>
	/// 关闭对应类型的面板
	/// </summary>
	/// <typeparam name="T">面板类型.</typeparam>
	public static void ClosePanel<T> () where T:GamePanel
	{
		Type t = typeof(T).BaseType;
		PanelController.GetInstance ().ClosePanel (t);
	}

	/// <summary>
	/// 关闭对应类型的面板
	/// </summary>
	/// <typeparam name="t">面板类型.</typeparam>
	public void ClosePanel (Type t)
	{
		if (t == typeof(ContentPanel)) {
			CloseContentPanel ();
		} else if (t == typeof(HeaderPanel)) {
			CloseHeaderPanel ();
		} else if (t == typeof(FooterPanel)) {
			CloseFooterPanel ();
		} else if (t == typeof(PopupPanel)) {
			ClosePopupPanel ();
		} else if (t == typeof(SystemPanel)) {
			CloseSytemPanel ();
		} else if (t == typeof(TutorialPanel)) {
			CloseTutorialPanel ();
		} else if (t == typeof(DialogPanel)) {
			CloseDialogPanel ();
		} else if (t == typeof(GameBGPanel)) {
			CloseBGPanel ();
		}
	}

	/// <summary>
	/// 关闭Popup Panel
	/// </summary>
	/// <returns><c>true</c>, 关闭成功, <c>false</c> 关闭失败.</returns>
	public void ClosePopupPanel ()
	{
		//是否有popupPanel
		if (popupPanel == null) {
			return;
		}
		PopupPanel temp = popupPanel;
		popupPanel = null;
		CloseGamePanel (temp, false, false);
	}

	/// <summary>
	/// 关闭System Panel
	/// </summary>
	/// <returns><c>true</c>, 关闭成功, <c>false</c> 关闭失败.</returns>
	public void CloseSytemPanel ()
	{
		if (systemPanel == null) {
			return;
		}
		SystemPanel temp = systemPanel;
		systemPanel = null;
		CloseGamePanel (temp, false, false);
	}

	/// <summary>
	/// 关闭Tutorial Panel
	/// </summary>
	/// <returns><c>true</c>, 关闭成功, <c>false</c> 关闭失败.</returns>
	public void CloseTutorialPanel ()
	{
		if (tutorialPanel == null) {
			return;
		}
		var temp = tutorialPanel;
		tutorialPanel = null;
		CloseGamePanel (temp, false, false);
	}

	/// <summary>
	/// 返回按键
	/// </summary>
	public void BackKey ()
	{
		//系统面板
		if (systemPanel != null) {
			systemPanel.BackKey ();
			return;
		}
		//提示框面板
		if (popupPanel != null) {
			popupPanel.BackKey ();
			return;
		}
		//对话框面板
		if (dialogGamePanel != null) {
			dialogGamePanel.BackKey ();
			return;
		}
		//普通面板
		if (currentContentPanel != null) {
			currentContentPanel.BackKey ();
			return;
		}
	}

	/// <summary>
	/// 是否启用UICanvas
	/// </summary>
	/// <param name="value">If set to <c>true</c> value.</param>
	public void EnabledCanvas (bool value)
	{
		this.canvas.gameObject.SetActive (value);
		this.StartCoroutine (ActiveEventSystem (value));
	}

	/// <summary>
	/// 延迟激活EventSystem
	/// </summary>
	/// <returns>The event system.</returns>
	/// <param name="value">If set to <c>true</c> value.</param>
	private IEnumerator ActiveEventSystem (bool value)
	{
		yield return new WaitForFixedUpdate ();
		this.eventSystem.SetActive (value);
		SoundController.GetInstance ().gameObject.SetActive (value);
	}

	/// <summary>
	/// Spawn.
	/// </summary>
	public class Spawn
	{
		public Spawn (GamePanel gamePanel)
		{
			this.gamePanel = gamePanel;
		}

		public GamePanel gamePanel{ get; private set; }

		/// <summary>
		/// 延迟销毁计时器
		/// </summary>
		public float delayTime;

		/// <summary>
		/// 是否达到销毁时间，能销毁
		/// </summary>
		/// <returns><c>true</c>, if can destroy was ised, <c>false</c> otherwise.</returns>
		public bool isCanDestroy ()
		{
			return this.gamePanel.delayDestroyTime < delayTime;
		}
			
	}
}


/// <summary>
/// Panel移动动画状态.
/// </summary>
public enum MoveAnimationState
{
	/// <summary>
	/// 进入动画开始
	/// </summary>
	StartMoveIn,
	/// <summary>
	/// 进入动画结束
	/// </summary>
	EndMoveIn,
	/// <summary>
	/// 退出动画开始
	/// </summary>
	StartMoveOut,
	/// <summary>
	/// 退出动画结束,但不销毁对象
	/// </summary>
	EndMoveOut,
	/// <summary>
	/// 退出动画结束,销毁对象.
	/// </summary>
	EndMoveOutAndDestroy
}
