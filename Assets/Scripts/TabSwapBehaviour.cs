using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabSwapBehaviour : MonoBehaviour
{
	public void Init()
	{
		this.isInit = true;
		this.startingHierarchyIndex = this.tabs[this.tabs.Length - 1].GetSiblingIndex();
	}

	public void SetTab(int tabIndex)
	{
		if (!this.isInit)
		{
			this.Init();
		}
		this.image.color = this.colors[tabIndex];
		foreach (GameObject gameObject in this.views)
		{
			gameObject.SetActive(false);
		}
		this.views[tabIndex].SetActive(true);
		foreach (RectTransform rectTransform in this.tabs)
		{
			if (rectTransform != this.tabs[tabIndex])
			{
				rectTransform.DOAnchorPosY(this.tabBaseline, 0.3f, false).SetEase(Ease.OutBack);
				TextMeshProUGUI componentInChildren = rectTransform.GetComponentInChildren<TextMeshProUGUI>();
				if (componentInChildren != null)
				{
					componentInChildren.color = new Color(0f, 0f, 0f, 0.25f);
				}
			}
			else
			{
				rectTransform.DOAnchorPosY(this.tabBaseline + 12f, 0.3f, false).SetEase(Ease.OutBack);
				TextMeshProUGUI componentInChildren2 = rectTransform.GetComponentInChildren<TextMeshProUGUI>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.color = new Color(1f, 1f, 1f, 1f);
				}
			}
		}
		if (this.tabs.Length > 0)
		{
			this.tabs[tabIndex].SetSiblingIndex(this.startingHierarchyIndex + (this.tabs.Length - 1));
		}
	}

	[SerializeField]
	private RectTransform[] tabs;

	[SerializeField]
	private GameObject[] views;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Color[] colors;

	public float tabBaseline = 430f;

	public int startingHierarchyIndex = 4;

	private bool isInit;
}
