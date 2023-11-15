using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogInteractionHandler : MonoBehaviour
{
	private void Awake()
	{
		DialogInteractionHandler.Instance = this;
	}

	public UpgradeDialogTween CurrentTopDialog
	{
		get
		{
			if (this.activeDialogs.Count == 0)
			{
				return null;
			}
			return this.activeDialogs[this.activeDialogs.Count - 1].GetComponent<UpgradeDialogTween>();
		}
	}

	public bool DisableCloseByClickingShade { get; set; }

	public void NewActiveDialog(Transform t)
	{
		BackManager.Push(BackManager.BackItemType.Dialog);
		this.graficRayCaster.enabled = true;
		this.hasDialogActive = true;
		this.activeDialogs.Add(t);
		this.Shade.SetAsLastSibling();
		t.SetAsLastSibling();
		this.ShadeTweenOn();
	}

	public void DialogClosed(Transform t)
	{
		BackManager.Pop();
		this.activeDialogs.Remove(t);
		this.Shade.SetAsFirstSibling();
		if (this.activeDialogs.Count < 1)
		{
			this.graficRayCaster.enabled = false;
			this.hasDialogActive = false;
			this.ShadeTweenOff();
		}
	}

	public void CloseTop()
	{
		if (this.activeDialogs.Count == 0 || this.DisableCloseByClickingShade)
		{
			return;
		}
		int num = this.activeDialogs.Count - 1;
		if (num >= 0)
		{
			Transform transform = this.activeDialogs[num];
			if (transform != null)
			{
				UpgradeDialogTween component = transform.GetComponent<UpgradeDialogTween>();
				if (component != null)
				{
					component.Close(false);
				}
				else
				{
					UnityEngine.Debug.LogWarning("Trying to call CloseTop() in DialogInteractionHandler but GetComponent<UpgradeDialogTween>() returned null");
				}
			}
			else
			{
				this.activeDialogs.RemoveAt(num);
			}
		}
		if (this.activeDialogs.Count < 1)
		{
			this.ShadeTweenOff();
		}
	}

	private void ShadeTweenOn()
	{
		this.TweenKiller();
		this.Shade.gameObject.SetActive(true);
		this.ShadeImage.DOColor(new Color(0f, 0f, 0f, 0.25f), 0.5f);
	}

	private void ShadeTweenOff()
	{
		this.ShadeImage.DOFade(0f, 0.5f).OnComplete(delegate
		{
			this.Shade.gameObject.SetActive(false);
		});
	}

	private void TweenKiller()
	{
		this.Shade.DOKill(true);
		this.ShadeImage.DOKill(true);
	}

	public bool HasDialogActive()
	{
		return this.hasDialogActive;
	}

	public T GetDialog<T>() where T : UpgradeDialogTween
	{
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			T component = base.transform.GetChild(i).GetComponent<T>();
			if (component != null)
			{
				return component;
			}
		}
		return (T)((object)null);
	}

	public static DialogInteractionHandler Instance;

	private List<Transform> activeDialogs = new List<Transform>();

	[SerializeField]
	private Transform Shade;

	[SerializeField]
	private Image ShadeImage;

	[SerializeField]
	private GraphicRaycaster graficRayCaster;

	private bool hasDialogActive;
}
