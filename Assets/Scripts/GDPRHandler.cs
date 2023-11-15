using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GDPRHandler : MonoBehaviour
{
	public static GDPRHandler Instance { get; private set; }

	private void Awake()
	{
		GDPRHandler.Instance = this;
	}
    void Start()
    {
        GDPRHandler.Instance.StartGdprSequence(0);
    }
	public void StartGdprSequence(int startIndex = 0)
	{
		GDPRComplianceData.HasSeenConsentDialogBefore = true;
		this.gRaycaster.enabled = true;
		foreach (GameObject gameObject in this.dialogs)
		{
			gameObject.SetActive(false);
			gameObject.transform.localScale = Vector3.zero;
		}
		this.dialogIndex = startIndex;
		this.dialogs[this.dialogIndex].SetActive(true);
		this.dialogs[this.dialogIndex].SetActive(true);
		this.animationActive = true;
		this.dialogs[this.dialogIndex].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.3f).OnComplete(delegate
		{
			this.animationActive = false;
		});
	}

	public void NextDialog()
	{
		if (this.dialogIndex < this.dialogs.Length && !this.animationActive)
		{
			this.animationActive = true;
			int currentIndext = this.dialogIndex;
			this.dialogs[this.dialogIndex].transform.DOScale(0f, 0.4f).SetEase(Ease.InBack).OnComplete(delegate
			{
				this.dialogs[currentIndext].SetActive(false);
			});
			this.dialogIndex++;
			this.dialogs[this.dialogIndex].SetActive(true);
			this.dialogs[this.dialogIndex].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.3f).OnComplete(delegate
			{
				this.animationActive = false;
			});
		}
	}

	public void PreviousDialog()
	{
		if (this.dialogIndex > 0 && !this.animationActive)
		{
			this.animationActive = true;
			int currentIndext = this.dialogIndex;
			this.dialogs[this.dialogIndex].transform.DOScale(0f, 0.4f).SetEase(Ease.InBack).OnComplete(delegate
			{
				this.dialogs[currentIndext].SetActive(false);
			});
			this.dialogIndex--;
			this.dialogs[this.dialogIndex].SetActive(true);
			this.dialogs[this.dialogIndex].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.3f).OnComplete(delegate
			{
				this.animationActive = false;
			});
		}
	}

	public void CloseDialogAcceptAll()
	{
		if (!this.animationActive)
		{
			GDPRComplianceData.Apply(true);
			this.animationActive = true;
			int currentIndext = this.dialogIndex;
			this.dialogs[this.dialogIndex].transform.DOScale(0f, 0.4f).SetEase(Ease.InBack).OnComplete(delegate
			{
				this.dialogs[currentIndext].SetActive(false);
				this.animationActive = false;
			});
			this.gRaycaster.enabled = false;
		}
	}

	public void CloseDialog()
	{
		if (!this.animationActive)
		{
			GDPRComplianceData.Apply(false);
			GDPRComplianceData.SetAdConsent(GDPRComplianceData.HasAdConsent);
			GDPRComplianceData.SetAnalyticConsent(GDPRComplianceData.HasAnalyticConsent);
			this.animationActive = true;
			int currentIndext = this.dialogIndex;
			this.dialogs[this.dialogIndex].transform.DOScale(0f, 0.4f).SetEase(Ease.InBack).OnComplete(delegate
			{
				this.dialogs[currentIndext].SetActive(false);
				this.animationActive = false;
			});
			this.gRaycaster.enabled = false;
		}
	}

	private void OnDestroy()
	{
		this.KillTweens();
		if (SceneManager.GetActiveScene().name == "GDPRScene")
		{
			DOTween.KillAll(false);
		}
	}

	private void KillTweens()
	{
		foreach (GameObject gameObject in this.dialogs)
		{
			base.transform.DOKill(false);
		}
	}

	[SerializeField]
	private GameObject[] dialogs;

	[SerializeField]
	private GraphicRaycaster gRaycaster;

	private int dialogIndex;

	private bool animationActive;
}
