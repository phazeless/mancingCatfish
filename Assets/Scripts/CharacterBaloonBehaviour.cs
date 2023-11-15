using System;
using System.Collections;
using System.Diagnostics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CharacterBaloonBehaviour : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<string> OnSubmit;

	private void Start()
	{
		this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnTextSubmitted));
	}

	private void OnDestroy()
	{
		this.inputField.onEndEdit.RemoveListener(new UnityAction<string>(this.OnTextSubmitted));
		if (base.transform != null)
		{
			base.transform.DOKill(false);
		}
	}

	private void OnTextSubmitted(string text)
	{
		this.OnSubmit(text);
	}

	public void DisplayText(string orangeText, string grayText)
	{
		this.dialogLabel.SetVariableText(new string[]
		{
			orangeText.Replace("\\n", "\n"),
			grayText.Replace("\\n", "\n")
		});
		this.dialogLabel.transform.localScale = Vector2.zero;
		this.OpenBaloon();
		this.RunAfterDelay(0.1f, delegate()
		{
			this.dialogLabel.transform.localScale = Vector2.one;
			base.StartCoroutine(this.RevealText());
		});
	}

	private void OpenBaloon()
	{
		this.TweenKiller();
		base.gameObject.SetActive(true);
		this.inputField.gameObject.SetActive(false);
		if (this.baloonOpen)
		{
			base.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.5f, 8, 0.8f);
		}
		else
		{
			base.transform.DOScale(1f, 0.5f).SetEase(Ease.OutElastic);
		}
		this.baloonOpen = true;
	}

	public void CloseBaloon()
	{
		this.TweenKiller();
		base.gameObject.SetActive(false);
		base.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack);
		this.baloonOpen = false;
	}

	private IEnumerator RevealText()
	{
		int totalVisibleCharacters = this.dialogLabel.textInfo.characterCount;
		int counter = 0;
		for (;;)
		{
			int visibleCount = counter % (totalVisibleCharacters + 1);
			this.dialogLabel.maxVisibleCharacters = visibleCount;
			if (visibleCount >= totalVisibleCharacters)
			{
				break;
			}
			counter++;
			yield return new WaitForSeconds(0.01f);
		}
		yield break;
	}

	public void ShowInputfield()
	{
		this.inputField.gameObject.SetActive(true);
		this.inputField.Select();
	}

	private void TweenKiller()
	{
		base.transform.DOKill(true);
	}

	[SerializeField]
	private TextMeshProUGUI dialogLabel;

	[SerializeField]
	private TMP_InputField inputField;

	private bool baloonOpen;
}
