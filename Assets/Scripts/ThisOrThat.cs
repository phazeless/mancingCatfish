using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ThisOrThat : MonoBehaviour
{
	private void OnEnable()
	{
		this.StartAd();
	}

	private void StartAd()
	{
		this.currentIndex = 0;
		this.topText.anchoredPosition = new Vector2(-this.edgeMargin, this.topText.anchoredPosition.y);
		this.botText.anchoredPosition = new Vector2(this.edgeMargin, this.botText.anchoredPosition.y);
		this.topVotes.localScale = Vector2.zero;
		this.botVotes.localScale = Vector2.zero;
		this.topAgree.localScale = Vector2.zero;
		this.botAgree.localScale = Vector2.zero;
		this.progressMeter.localScale = new Vector3(0f, 1f, 0f);
		this.handAnimation.SetActive(false);
		base.Invoke("TweenNewQuestion", 1.5f);
		int num = this.thisOrThatQuestions.Length;
		if (num > 4)
		{
			this.questions[0] = UnityEngine.Random.Range(0, num / 3);
			this.questions[1] = UnityEngine.Random.Range(num / 3 + 1, 2 * (num / 3) / (num / 3));
			this.questions[2] = UnityEngine.Random.Range(2 * (num / 3) / (num / 3) + 1, num);
		}
		base.Invoke("InvokeHand", 10f);
	}

	private void InvokeHand()
	{
		this.handAnimation.SetActive(false);
		this.handAnimation.SetActive(true);
	}

	private void TweenNewQuestion()
	{
		this.topVotesLabel.SetVariableText(new string[]
		{
			this.thisOrThatQuestions[this.questions[this.currentIndex]].topVotePercent.ToString()
		});
		this.topTextLabel.SetText(this.thisOrThatQuestions[this.questions[this.currentIndex]].topText);
		this.botVotesLabel.SetVariableText(new string[]
		{
			(100 - this.thisOrThatQuestions[this.questions[this.currentIndex]].topVotePercent).ToString()
		});
		this.botTextLabel.SetText(this.thisOrThatQuestions[this.questions[this.currentIndex]].botText);
		this.topText.DOAnchorPosX(0f, 0.5f, false).SetEase(Ease.OutElastic);
		this.botText.DOAnchorPosX(0f, 0.5f, false).SetEase(Ease.OutElastic).SetDelay(1f).OnComplete(delegate
		{
			this.topButton.SetActive(true);
			this.botButton.SetActive(true);
			if (this.currentIndex == 0)
			{
				this.handAnimation.SetActive(true);
			}
		});
	}

	public void Click(bool top)
	{
		base.CancelInvoke("InvokeHand");
		if (top)
		{
			this.topVotes.DOScale(new Vector2(1f, 1f), 0.5f).SetEase(Ease.OutElastic);
			this.topAgree.DOScale(new Vector2(1f, 1f), 0.5f).SetEase(Ease.OutElastic);
			this.botAgreeLabel.SetText("Disagree");
			this.topAgreeLabel.SetText("Agree");
			this.botVotes.DOScale(new Vector2(1f, 1f), 0.5f).SetEase(Ease.OutElastic).SetDelay(1f);
			this.botAgree.DOScale(new Vector2(1f, 1f), 0.5f).SetEase(Ease.OutElastic).SetDelay(1f);
			if (this.currentIndex >= 3)
			{
				UnityEngine.Debug.Log("GO TO DOWNLOAD PAGE");
			}
		}
		else
		{
			this.topVotes.DOScale(new Vector2(1f, 1f), 0.5f).SetEase(Ease.OutElastic).SetDelay(1f);
			this.topAgree.DOScale(new Vector2(1f, 1f), 0.5f).SetEase(Ease.OutElastic).SetDelay(1f);
			this.botAgreeLabel.SetText("Agree");
			this.topAgreeLabel.SetText("Disagree");
			this.botVotes.DOScale(new Vector2(1f, 1f), 0.5f).SetEase(Ease.OutElastic);
			this.botAgree.DOScale(new Vector2(1f, 1f), 0.5f).SetEase(Ease.OutElastic);
			if (this.currentIndex >= 3)
			{
				UnityEngine.Debug.Log("CLOSE");
			}
		}
		this.handAnimation.SetActive(false);
		this.topButton.SetActive(false);
		this.botButton.SetActive(false);
		this.progressMeter.DOScaleX(this.progressMeter.localScale.x + 0.334f, 0.3f).SetEase(Ease.OutBack).SetDelay(0.5f);
		base.Invoke("NextQuestion", 2f);
	}

	private void NextQuestion()
	{
		base.CancelInvoke("InvokeHand");
		base.Invoke("InvokeHand", 8f);
		this.topVotes.DOScale(Vector2.zero, 0.5f).SetEase(Ease.InBack).SetDelay(0.2f);
		this.topAgree.DOScale(Vector2.zero, 0.5f).SetEase(Ease.InBack).SetDelay(0.4f);
		this.botVotes.DOScale(Vector2.zero, 0.5f).SetEase(Ease.InBack).SetDelay(0.6f);
		this.botAgree.DOScale(Vector2.zero, 0.5f).SetEase(Ease.InBack).SetDelay(0.8f);
		this.topText.DOAnchorPosX(this.edgeMargin, 0.5f, false).SetEase(Ease.OutElastic).SetDelay(0.3f);
		this.botText.DOAnchorPosX(-this.edgeMargin, 0.5f, false).SetEase(Ease.OutElastic).SetDelay(0.6f);
		this.currentIndex++;
		if (this.currentIndex < 3)
		{
			base.Invoke("TweenNewQuestion", 1.2f);
		}
		else if (this.currentIndex == 3)
		{
			base.Invoke("EndTween", 1.2f);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void EndTween()
	{
		int num = UnityEngine.Random.Range(0, 3);
		if (num == 0)
		{
			this.topVotesLabel.SetVariableText(new string[]
			{
				"61"
			});
			this.topTextLabel.SetText("<b>Download</b> this game and live happily ever after!");
			this.botVotesLabel.SetVariableText(new string[]
			{
				"39"
			});
			this.botTextLabel.SetText("Miss the chance of your life");
		}
		if (num == 1)
		{
			this.topVotesLabel.SetVariableText(new string[]
			{
				"33"
			});
			this.topTextLabel.SetText("<b>Download<b> this game NOW!");
			this.botVotesLabel.SetVariableText(new string[]
			{
				"67"
			});
			this.botTextLabel.SetText("Download it later");
		}
		if (num == 2)
		{
			this.topVotesLabel.SetVariableText(new string[]
			{
				"53"
			});
			this.topTextLabel.SetText("Make people around you smile, <b>Get this game! :D</b>");
			this.botVotesLabel.SetVariableText(new string[]
			{
				"47"
			});
			this.botTextLabel.SetText("Think about it next time you're with friends");
		}
		this.topText.DOAnchorPosX(0f, 0.5f, false).SetEase(Ease.OutElastic);
		this.botText.DOAnchorPosX(0f, 0.5f, false).SetEase(Ease.OutElastic).SetDelay(1f).OnComplete(delegate
		{
			this.topButton.SetActive(true);
			this.botButton.SetActive(true);
			if (this.currentIndex == 0)
			{
				this.handAnimation.SetActive(true);
			}
		});
	}

	private void OnDestroy()
	{
		this.topVotes.DOKill(false);
		this.topAgree.DOKill(false);
		this.botVotes.DOKill(false);
		this.botAgree.DOKill(false);
		this.topText.DOKill(false);
		this.botText.DOKill(false);
		this.progressMeter.DOKill(false);
	}

	[SerializeField]
	[Header("Tween References")]
	private RectTransform topVotes;

	[SerializeField]
	private RectTransform topAgree;

	[SerializeField]
	private RectTransform topText;

	[SerializeField]
	private RectTransform botVotes;

	[SerializeField]
	private RectTransform botAgree;

	[SerializeField]
	private RectTransform botText;

	[SerializeField]
	private RectTransform progressMeter;

	[SerializeField]
	[Header("Labels")]
	private TextMeshProUGUI topVotesLabel;

	[SerializeField]
	private TextMeshProUGUI topAgreeLabel;

	[SerializeField]
	private TextMeshProUGUI topTextLabel;

	[SerializeField]
	private TextMeshProUGUI botVotesLabel;

	[SerializeField]
	private TextMeshProUGUI botAgreeLabel;

	[SerializeField]
	private TextMeshProUGUI botTextLabel;

	[Header("Buttons")]
	[SerializeField]
	private GameObject topButton;

	[SerializeField]
	private GameObject botButton;

	[Header("Animation")]
	[SerializeField]
	private GameObject handAnimation;

	[SerializeField]
	private ThisOrThat.ThisOrThatQuestion[] thisOrThatQuestions;

	private float edgeMargin = 1600f;

	private int q1;

	private int q2 = 1;

	private int q3 = 2;

	private int[] questions = new int[]
	{
		0,
		1,
		2
	};

	private int currentIndex;

	[Serializable]
	private class ThisOrThatQuestion
	{
		public int topVotePercent = 50;

		public string topText = "Fart popcorn.";

		public string botText = "Sneeze milk.";
	}
}
