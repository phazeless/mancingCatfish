using System;
using TMPro;
using UnityEngine;

public class UIIAPPendingBlocker : MonoBehaviour
{
	public static UIIAPPendingBlocker Instance { get; private set; }

	private void Awake()
	{
		UIIAPPendingBlocker.Instance = this;
	}

	public void Show()
	{
		this.crossTimer = 0f;
		this.cross.SetActive(false);
		this.background.SetActive(true);
	}

	public void Hide()
	{
		this.background.SetActive(false);
	}

	private void Update()
	{
		if (!this.background.activeSelf)
		{
			return;
		}
		this.loadingIcon.transform.Rotate(0f, 0f, -Time.deltaTime * 90f);
		if (FHelper.HasSecondsPassed(5f, ref this.crossTimer, false))
		{
			this.cross.SetActive(true);
		}
	}

	[SerializeField]
	private GameObject background;

	[SerializeField]
	private TextMeshProUGUI loadingText;

	[SerializeField]
	private GameObject loadingIcon;

	[SerializeField]
	private GameObject cross;

	private float crossTimer;
}
