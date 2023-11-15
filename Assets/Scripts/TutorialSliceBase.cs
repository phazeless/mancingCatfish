using System;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSliceBase : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<TutorialSliceBase> OnEnterTutorial;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<TutorialSliceBase> OnExitedTutorial;

	protected Transform MaskCircle
	{
		get
		{
			return this.maskCircle;
		}
	}

	protected virtual void Awake()
	{
		if (this.visualHolder != null)
		{
			this.visualHolder.SetActive(false);
		}
	}

	public void RegisterListener()
	{
		TutorialManager.Instance.OnTutorialSetup += this.Instance_OnTutorialSetup;
	}

	private void Instance_OnTutorialSetup()
	{
		this.Setup();
	}

	protected virtual void Enter()
	{
		if (this.isEntering)
		{
			return;
		}
		this.isEntering = true;
		if (this.visualHolder != null)
		{
			this.TweenKiller(true);
			this.visualHolder.SetActive(true);
			this.infoBox.DOScale(Vector2.zero, 0.3f).From<Tweener>().SetDelay(0.3f).SetEase(Ease.OutBack).OnComplete(delegate
			{
				this.infoBox.DOLocalMoveY(this.infoBox.localPosition.y + 20f, 3f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
				this.Entered();
			});
			if (this.bgImage.gameObject.activeInHierarchy)
			{
				this.maskCircle.DOScale(Vector2.one * 10f, 0.5f).From<Tweener>();
				this.bgImage.DOFade(0f, 0.2f).From<Tweener>();
			}
		}
		else
		{
			this.Entered();
		}
	}

	public void Exit(bool autoDestroy = true)
	{
		if (this.isExiting)
		{
			return;
		}
		this.isExiting = true;
		TutorialManager.Instance.TutorialSliceCompleted(this.Id);
		TutorialManager.Instance.SetGraphicRaycaster(false);
		if (this.visualHolder != null)
		{
			this.TweenKiller(true);
			this.infoBox.DOScale(Vector2.zero, 0.3f);
			this.maskCircle.DOScale(Vector2.one * 10f, 0.5f).OnComplete(delegate
			{
				this.Exited();
				if (!this.isKillingTween)
				{
					this.TweenKiller(false);
				}
				if (base.gameObject != null)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			});
			if (this.bgImage.gameObject.activeInHierarchy)
			{
				this.bgImage.DOFade(0f, 0.4f);
			}
		}
		else
		{
			this.Exited();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected virtual void Setup()
	{
	}

	protected virtual void Entered()
	{
		if (this.OnEnterTutorial != null)
		{
			this.OnEnterTutorial(this);
		}
	}

	protected virtual void Exited()
	{
		if (this.OnExitedTutorial != null)
		{
			this.OnExitedTutorial(this);
		}
	}

	private void TweenKiller(bool complete = true)
	{
		if (this.infoBox != null)
		{
			this.infoBox.DOKill(complete);
		}
		if (this.maskCircle != null)
		{
			this.maskCircle.DOKill(complete);
		}
		if (this.bgImage != null)
		{
			this.bgImage.DOKill(complete);
		}
	}

	protected virtual void OnDestroy()
	{
		this.isKillingTween = true;
		this.TweenKiller(true);
		TutorialManager.Instance.OnTutorialSetup -= this.Instance_OnTutorialSetup;
		this.OnEnterTutorial = null;
		this.OnExitedTutorial = null;
	}

	[SerializeField]
	private Transform infoBox;

	[SerializeField]
	private Transform maskCircle;

	[SerializeField]
	private Image bgImage;

	[SerializeField]
	private GameObject visualHolder;

	public string Id;

	public bool HasCompleted;

	private bool isExiting;

	private bool isEntering;

	private bool isKillingTween;
}
