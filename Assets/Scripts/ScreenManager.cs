using System;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<ScreenManager.Screen, ScreenManager.Screen> OnScreenTransitionStarted;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<ScreenManager.Screen, ScreenManager.Screen> OnScreenTransitionEnded;

	public ScreenManager.Screen CurrentScreen
	{
		get
		{
			return this.currentScreen;
		}
	}

	public static ScreenManager Instance { get; private set; }

	private void Awake()
	{
		ScreenManager.Instance = this;
	}

	private void Start()
	{
		this.RunAfterDelay(0.5f, delegate()
		{
			if (this.currentScreen == ScreenManager.Screen.Tutorial && !this.isFirstTime)
			{
				this.GoToScreen(ScreenManager.Screen.Main);
			}
		});
	}

	public void DelayedScreenDisable()
	{
		if (this.currentScreen != ScreenManager.Screen.Shop)
		{
			this.shopRect.gameObject.SetActive(false);
		}
		if (this.currentScreen != ScreenManager.Screen.Map)
		{
			this.mapRect.gameObject.SetActive(false);
		}
	}

	public void GoToScreen(int screen)
	{
		this.GoToScreen((ScreenManager.Screen)screen, null);
	}

	public void GoToScreen(ScreenManager.Screen screen)
	{
		this.GoToScreen(screen, null);
	}

	public void GoToScreen(ScreenManager.Screen screen, Action callback)
	{
		if (screen == ScreenManager.Screen.Main)
		{
			BackManager.Pop();
		}
		else if (screen == ScreenManager.Screen.Shop)
		{
			BackManager.Push(BackManager.BackItemType.Menu);
		}
		else if (screen == ScreenManager.Screen.Map)
		{
			BackManager.Push(BackManager.BackItemType.Menu);
		}
		this.internalScreenTransitionEnded = callback;
		if (this.currentScreen == screen)
		{
			return;
		}
		switch (screen)
		{
		case ScreenManager.Screen.Main:
			this.TransitionToMain();
			break;
		case ScreenManager.Screen.Map:
			this.TransitionToMap();
			break;
		case ScreenManager.Screen.Shop:
			this.TransitionToShop();
			break;
		case ScreenManager.Screen.Tutorial:
			this.TransitionToTutorial();
			break;
		case ScreenManager.Screen.Tournament:
			this.TransitionToTournament();
			break;
		}
		this.previousScreen = this.currentScreen;
		this.currentScreen = screen;
		if (this.OnScreenTransitionStarted != null)
		{
			this.OnScreenTransitionStarted(this.currentScreen, this.previousScreen);
		}
	}

	public void OnScreenTransitionFinished(ScreenManager.Screen screen)
	{
		if (screen == this.previousScreen)
		{
			return;
		}
		if (this.internalScreenTransitionEnded != null)
		{
			this.internalScreenTransitionEnded();
			this.internalScreenTransitionEnded = null;
		}
		if (this.OnScreenTransitionEnded != null)
		{
			this.OnScreenTransitionEnded(screen, this.previousScreen);
		}
	}

	private void TransitionToMain()
	{
		this.TweenKiller();
		this.ShowMainScreenStuff();
		this.ShowShopAndMapButtons();
		if (this.currentScreen == ScreenManager.Screen.Shop)
		{
			this.shopRect.DOAnchorPosX(this.screenSize, 0.5f, false).OnComplete(delegate
			{
				this.OnScreenTransitionFinished(ScreenManager.Screen.Main);
				this.shopRect.gameObject.SetActive(false);
			});
		}
		else if (this.currentScreen == ScreenManager.Screen.Map)
		{
			this.mapRect.DOAnchorPosX(-this.screenSize, 0.5f, false).OnComplete(delegate
			{
				this.OnScreenTransitionFinished(ScreenManager.Screen.Main);
				this.mapRect.gameObject.SetActive(false);
			});
		}
		else if (this.currentScreen == ScreenManager.Screen.Tutorial)
		{
			this.OnScreenTransitionFinished(ScreenManager.Screen.Main);
		}
	}

	private void TransitionToShop()
	{
		this.TweenKiller();
		this.HideMainScreenStuff();
		this.shopRect.gameObject.SetActive(true);
		this.shopRect.DOAnchorPosX(-this.screenSize, 0.5f, false).OnComplete(delegate
		{
			this.OnScreenTransitionFinished(ScreenManager.Screen.Shop);
		});
	}

	private void TransitionToMap()
	{
		this.TweenKiller();
		this.HideMainScreenStuff();
		this.mapRect.gameObject.SetActive(true);
		this.mapRect.DOAnchorPosX(this.screenSize, 0.5f, false).OnComplete(delegate
		{
			this.OnScreenTransitionFinished(ScreenManager.Screen.Map);
		});
	}

	private void TransitionToTutorial()
	{
		this.TweenKiller();
		this.HideMainScreenStuff();
		if (this.currentScreen == ScreenManager.Screen.Shop)
		{
			this.shopRect.DOAnchorPosX(this.screenSize, 0.5f, false).OnComplete(delegate
			{
				this.OnScreenTransitionFinished(ScreenManager.Screen.Main);
				this.shopRect.gameObject.SetActive(false);
			});
		}
		else if (this.currentScreen == ScreenManager.Screen.Map)
		{
			this.mapRect.DOAnchorPosX(-this.screenSize, 0.5f, false).OnComplete(delegate
			{
				this.OnScreenTransitionFinished(ScreenManager.Screen.Main);
				this.mapRect.gameObject.SetActive(false);
			});
		}
		this.OnScreenTransitionFinished(ScreenManager.Screen.Tutorial);
	}

	private void TransitionToTournament()
	{
		this.TweenKiller();
		this.HideShopAndMapButtons();
	}

	private void ShowMainScreenStuff()
	{
		this.crewRect.DOAnchorPosX(25f, 0.5f, false);
		this.ignRect.DOAnchorPosX(-75f, 0.5f, false);
		this.bottomButtonRect.DOAnchorPosY(161f, 0.5f, false);
		this.fireworkButtonRect.DOScale(1f, 0.4f).SetDelay(0.5f).SetEase(Ease.OutBack);
		this.ShowShopAndMapButtons();
	}

	private void HideMainScreenStuff()
	{
		this.crewRect.DOAnchorPosX(-135f, 0.5f, false);
		this.ignRect.DOAnchorPosX(135f, 0.5f, false);
		this.bottomButtonRect.DOAnchorPosY(-161f, 0.5f, false);
		this.fireworkButtonRect.DOScale(0f, 0.25f).SetEase(Ease.InBack);
	}

	private void HideShopAndMapButtons()
	{
		this.mapButtonRect.DOAnchorPosX(-135f, 0.5f, false);
		this.shopButtonRect.DOAnchorPosX(135f, 0.5f, false);
	}

	private void ShowShopAndMapButtons()
	{
		this.mapButtonRect.DOAnchorPosX(121f, 0.3f, false);
		this.shopButtonRect.DOAnchorPosX(-121f, 0.3f, false);
	}

	private void TweenKiller()
	{
		this.mapRect.DOKill(true);
		this.shopRect.DOKill(true);
		this.mainRect.DOKill(true);
		this.crewRect.DOKill(true);
		this.ignRect.DOKill(true);
		this.bottomButtonRect.DOKill(true);
		this.shopButtonRect.DOKill(true);
		this.mapButtonRect.DOKill(true);
		this.fireworkButtonRect.DOKill(true);
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.Escape) && BackManager.Back() && !this.IsInsideTournament)
		{
			this.quitDialog.Open();
		}
	}

	private bool IsInsideTournament
	{
		get
		{
			return TournamentManager.Instance != null && TournamentManager.Instance.IsInsideTournament;
		}
	}

	[SerializeField]
	private Animator screenAnimator;

	[SerializeField]
	private UIQuitDialog quitDialog;

	private ScreenManager.Screen previousScreen = ScreenManager.Screen.Tutorial;

	private ScreenManager.Screen currentScreen = ScreenManager.Screen.Tutorial;

	private Action internalScreenTransitionEnded;

	public bool isFirstTime;

	[SerializeField]
	private RectTransform mainRect;

	[SerializeField]
	private RectTransform shopRect;

	[SerializeField]
	private RectTransform mapRect;

	[SerializeField]
	private RectTransform crewRect;

	[SerializeField]
	private RectTransform ignRect;

	[SerializeField]
	private RectTransform bottomButtonRect;

	[SerializeField]
	private RectTransform shopButtonRect;

	[SerializeField]
	private RectTransform mapButtonRect;

	[SerializeField]
	private RectTransform fireworkButtonRect;

	private float screenSize = 540f;

	public enum Screen
	{
		Main,
		Map,
		Shop,
		Tutorial,
		Tournament
	}
}
