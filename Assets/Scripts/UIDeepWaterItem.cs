using System;
using DG.Tweening;
using FullInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIDeepWaterItem : BaseBehavior
{
	private void Start()
	{
		this.SetState();
	}

	private void SetState()
	{
		if (this.dwState == UIDeepWaterItem.DWState.UnDiscovered)
		{
			this.SetUndiscovered();
		}
		else if (this.dwState == UIDeepWaterItem.DWState.Discovered)
		{
			this.SetDiscovered();
		}
		else if (this.dwState == UIDeepWaterItem.DWState.Reached)
		{
			this.SetReached();
		}
		else if (this.dwState == UIDeepWaterItem.DWState.Current)
		{
			this.SetCurrent();
		}
		else if (this.dwState == UIDeepWaterItem.DWState.DiscoveredAvaliable)
		{
			this.SetDiscoveredAvaliable();
		}
	}

	public void SetState(UIDeepWaterItem.DWState s)
	{
		this.dwState = s;
		this.SetState();
	}

	public void SetUndiscovered()
	{
		this.dwIconPassiveTween.SetNotCurrentAnymore();
		this.TweenKiller();
		this.IconHolder.SetActive(false);
		this.CircleOverlayHolder.gameObject.SetActive(true);
		this.FullHolder.SetActive(true);
		this.CicleOverlayImage.enabled = true;
		this.CicleOverlayImage.color = this.CircleOverlayUnreachedOverlay;
		this.FullHolder.transform.localScale = Vector2.one * 0.5f;
		foreach (GameObject gameObject in this.Banderoll)
		{
			gameObject.SetActive(false);
		}
		for (int j = 0; j < this.CirclePath.Length; j++)
		{
			this.CirclePath[j].color = new Color(1f, 1f, 1f, 0.25f);
			this.CirclePath[j].transform.localScale = Vector2.one * 0.7f;
		}
	}

	public void SetDiscovered()
	{
		this.dwIconPassiveTween.SetNotCurrentAnymore();
		this.TweenKiller();
		this.IconHolder.SetActive(false);
		this.CircleOverlayHolder.gameObject.SetActive(true);
		this.FullHolder.SetActive(true);
		this.CicleOverlayImage.enabled = true;
		this.CicleOverlayImage.color = this.CircleOverlayUnreachedOverlay;
		this.FullHolder.transform.localScale = Vector2.one;
		foreach (GameObject gameObject in this.Banderoll)
		{
			gameObject.SetActive(true);
			if (gameObject.transform.localScale.x >= 0f)
			{
				gameObject.transform.localScale = Vector2.one;
			}
			else if (gameObject.transform.localScale.x < 0f)
			{
				gameObject.transform.localScale = new Vector2(-1f, 1f);
			}
			gameObject.GetComponent<Image>().color = this.UnreachedBannerColor;
		}
		for (int j = 0; j < this.CirclePath.Length; j++)
		{
			this.CirclePath[j].color = new Color(1f, 1f, 1f, 0.25f);
			this.CirclePath[j].transform.localScale = Vector2.one * 0.7f;
		}
	}

	public void SetDiscoveredAvaliable()
	{
		this.dwIconPassiveTween.SetNotCurrentAnymore();
		this.TweenKiller();
		this.IconHolder.SetActive(false);
		this.CircleOverlayHolder.gameObject.SetActive(true);
		this.FullHolder.SetActive(true);
		this.CicleOverlayImage.enabled = true;
		this.CicleOverlayImage.color = this.CircleOverlayUnreachedOverlay;
		this.FullHolder.transform.localScale = Vector2.one;
		foreach (GameObject gameObject in this.Banderoll)
		{
			gameObject.SetActive(true);
			if (gameObject.transform.localScale.x >= 0f)
			{
				gameObject.transform.localScale = Vector2.one;
			}
			else if (gameObject.transform.localScale.x < 0f)
			{
				gameObject.transform.localScale = new Vector2(-1f, 1f);
			}
			gameObject.GetComponent<Image>().color = this.UnreachedBannerColor;
		}
		for (int j = 0; j < this.CirclePath.Length; j++)
		{
			this.CirclePath[j].color = new Color(1f, 1f, 1f, 0.25f);
			this.CirclePath[j].transform.localScale = Vector2.one * 0.7f;
		}
	}

	public void SetReached()
	{
		this.dwIconPassiveTween.SetNotCurrentAnymore();
		this.TweenKiller();
		this.IconHolder.SetActive(true);
		this.CircleOverlayHolder.gameObject.SetActive(false);
		this.FullHolder.SetActive(true);
		this.CicleOverlayImage.enabled = false;
		this.FullHolder.transform.localScale = Vector2.one * 0.5f;
		foreach (GameObject gameObject in this.Banderoll)
		{
			gameObject.SetActive(false);
		}
		for (int j = 0; j < this.CirclePath.Length; j++)
		{
			this.CirclePath[j].color = new Color(1f, 1f, 1f, 1f);
			this.CirclePath[j].transform.localScale = Vector2.one;
		}
	}

	public void SetCurrent()
	{
		this.TweenKiller();
		this.dwIconPassiveTween.SetCurrentPassiveTweens();
		this.IconHolder.SetActive(true);
		this.CircleOverlayHolder.gameObject.SetActive(false);
		this.FullHolder.SetActive(true);
		this.CicleOverlayImage.enabled = false;
		foreach (GameObject gameObject in this.Banderoll)
		{
			gameObject.SetActive(true);
			if (gameObject.transform.localScale.x >= 0f)
			{
				gameObject.transform.localScale = Vector2.one;
			}
			else if (gameObject.transform.localScale.x < 0f)
			{
				gameObject.transform.localScale = new Vector2(-1f, 1f);
			}
			gameObject.GetComponent<Image>().color = this.ReachedBannerColor;
		}
		this.FullHolder.transform.localScale = Vector2.one;
		this.Icon.material = this.startingMaterial;
		for (int j = 0; j < this.CirclePath.Length; j++)
		{
			this.CirclePath[j].color = new Color(1f, 1f, 1f, 1f);
			this.CirclePath[j].transform.localScale = Vector2.one;
		}
		if (this.dwIconPassiveTween != null)
		{
			this.dwIconPassiveTween.SetCurrentPassiveTweens();
		}
	}

	public void AnimateArrive()
	{
		this.TweenKiller();
		for (int i = 0; i < this.CirclePath.Length; i++)
		{
			this.CirclePath[i].DOColor(new Color(1f, 1f, 1f, 1f), 0.2f).SetDelay((float)i * 0.1f);
			this.CirclePath[i].transform.DOScale(1f, 0.2f).SetDelay((float)i * 0.1f).SetEase(Ease.OutBack);
		}
		foreach (GameObject gameObject in this.Banderoll)
		{
			gameObject.SetActive(true);
			if (gameObject.transform.localScale.x >= 0f)
			{
				gameObject.transform.localScale = Vector2.one;
			}
			else if (gameObject.transform.localScale.x < 0f)
			{
				gameObject.transform.localScale = new Vector2(-1f, 1f);
			}
			gameObject.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 10, 1f);
			gameObject.GetComponent<Image>().color = this.ReachedBannerColor;
		}
		this.Icon.material = this.startingMaterial;
		this.FullHolder.transform.localScale = Vector2.one;
		this.FullHolder.transform.DOPunchScale(new Vector3(0.5f, 0.3f, 0f), 0.5f, 8, 0.5f);
		this.Shine.gameObject.SetActive(true);
		this.Shine.DOScale(new Vector3(10f, 10f, 1f), 0.5f).OnComplete(delegate
		{
			this.Shine.gameObject.SetActive(false);
			SkillManager.Instance.DeepWaterSkill.TryLevelUp();
		});
		this.Shine.DORotate(new Vector3(0f, 0f, 300f), 0.5f, RotateMode.Fast);
		this.Shine.GetComponent<Image>().DOFade(0f, 0.3f).SetDelay(0.2f);
		this.Icon.material = this.startingMaterial;
		this.CicleOverlayImage.enabled = false;
		this.dwIconPassiveTween.SetCurrentPassiveTweens();
		this.IconHolder.SetActive(true);
		this.CircleOverlayHolder.gameObject.SetActive(false);
	}

	public void AnimateLeave()
	{
		this.TweenKiller();
		GameObject[] banderoll2 = this.Banderoll;
		for (int i = 0; i < banderoll2.Length; i++)
		{
			GameObject banderoll = banderoll2[i];
			if (banderoll.transform.localScale.x >= 0f)
			{
				banderoll.transform.DOScale(0.1f, 0.3f).OnComplete(delegate
				{
					banderoll.SetActive(false);
				});
			}
			else if (banderoll.transform.localScale.x < 0f)
			{
				banderoll.transform.DOScale(new Vector2(-0.1f, 0.1f), 0.3f).OnComplete(delegate
				{
					banderoll.SetActive(false);
				});
			}
		}
		this.FullHolder.transform.DOScale(Vector2.one * 0.5f, 0.5f).SetEase(Ease.InBack).OnComplete(delegate
		{
			this.dwIconPassiveTween.SetNotCurrentAnymore();
		});
	}

	public void AnimateDiscover()
	{
		this.TweenKiller();
		for (int i = 0; i < this.CirclePath.Length; i++)
		{
			this.CirclePath[i].DOColor(new Color(1f, 1f, 1f, 0.25f), 0.2f).SetDelay((float)i * 0.2f);
			this.CirclePath[i].transform.DOScale(0.7f, 0.2f).SetDelay((float)i * 0.2f).SetEase(Ease.OutBack);
		}
		this.FullHolder.SetActive(true);
		this.FullHolder.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
		foreach (GameObject gameObject in this.Banderoll)
		{
			gameObject.SetActive(true);
			gameObject.transform.localScale = gameObject.transform.localScale * 0.2f;
			if (gameObject.transform.localScale.x >= 0f)
			{
				gameObject.transform.DOScale(1f, 0.3f);
			}
			else if (gameObject.transform.localScale.x < 0f)
			{
				gameObject.transform.DOScale(new Vector2(-1f, 1f), 0.3f);
			}
		}
	}

	public void AnimateDiscoverAvaliable()
	{
		this.TweenKiller();
		for (int i = 0; i < this.CirclePath.Length; i++)
		{
			this.CirclePath[i].DOColor(new Color(1f, 1f, 1f, 0.25f), 0.2f).SetDelay((float)i * 0.2f);
			this.CirclePath[i].transform.DOScale(0.7f, 0.2f).SetDelay((float)i * 0.2f).SetEase(Ease.OutBack);
		}
		this.FullHolder.SetActive(true);
		this.FullHolder.transform.localScale = Vector2.zero;
		this.FullHolder.transform.DOScale(Vector2.one, 0.5f).SetEase(Ease.OutElastic);
		if (this.Banderoll[0].transform.localScale.x > 0f)
		{
			foreach (GameObject gameObject in this.Banderoll)
			{
				gameObject.SetActive(true);
				if (gameObject.transform.localScale.x >= 0f)
				{
					gameObject.transform.DOScale(1f, 0.3f);
				}
				else if (gameObject.transform.localScale.x < 0f)
				{
					gameObject.transform.DOScale(new Vector2(-1f, 1f), 0.3f);
				}
			}
		}
	}

	private void TweenKiller()
	{
		this.FullHolder.transform.DOKill(false);
		base.transform.DOKill(false);
		this.Shine.DOKill(false);
		this.Shine.GetComponent<Image>().DOKill(false);
		foreach (GameObject gameObject in this.Banderoll)
		{
			gameObject.transform.DOKill(false);
		}
		for (int j = 0; j < this.CirclePath.Length; j++)
		{
			this.CirclePath[j].DOKill(false);
			this.CirclePath[j].transform.DOKill(false);
		}
	}

	[SerializeField]
	public Image Icon;

	[SerializeField]
	public GameObject IconHolder;

	[SerializeField]
	public TextMeshProUGUI Name;

	[SerializeField]
	public GameObject FullHolder;

	[SerializeField]
	public Transform Shine;

	[SerializeField]
	public Image[] CirclePath;

	public bool currentDwl;

	[SerializeField]
	public GameObject[] Banderoll;

	[SerializeField]
	private Transform CircleOverlayHolder;

	[SerializeField]
	private Image CicleOverlayImage;

	[SerializeField]
	private Color CircleOverlayUnreachedOverlay;

	[SerializeField]
	private DWIconPassiveTween dwIconPassiveTween;

	private Color UnreachedBannerColor = new Color(0.404f, 0.341f, 0.29f);

	private Color ReachedBannerColor = new Color(0.816f, 0.384f, 0.29f);

	public Material startingMaterial;

	public UIDeepWaterItem.DWState dwState;

	public enum DWState
	{
		UnDiscovered,
		Discovered,
		Reached,
		Current,
		DiscoveredAvaliable
	}
}
