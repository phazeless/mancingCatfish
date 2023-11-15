using System;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharacterConversationHandler : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<int> OnConversationCompleted;

	private void Awake()
	{
		CharacterConversationHandler.Instance = this;
	}

	private void Start()
	{
		this.dialogBaloon.OnSubmit += this.DialogBaloon_OnSubmit;
		this.dialogBaloon.gameObject.SetActive(false);
	}

	private void DialogBaloon_OnSubmit(string name)
	{
		if (string.IsNullOrEmpty(name) || name == " ")
		{
			return;
		}
		SettingsManager.Instance.SetPlayerName(name, true, false);
		RectTransform component = base.GetComponent<RectTransform>();
		if (component != null)
		{
			component.anchoredPosition = new Vector2(0f, 0f);
		}
		else
		{
			UnityEngine.Debug.LogWarning("rectTransform inside CharacterConversationHandler is null");
		}
		this.RunAfterDelay(0.5f, delegate()
		{
			this.conversationIndex++;
			this.Next();
		});
	}

	public void TutorialFishing()
	{
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tutorial);
		this.EnableCharacterAnimationMode();
		this.currentConversation = this.intro;
		this.currentConversationID = 0;
		base.Invoke("Next", 0.5f);
	}

	public void TutorialBadGuyEnter()
	{
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tutorial);
		this.EnableCharacterAnimationMode();
		this.currentConversation = this.badGuyIntro;
		this.currentConversationID = 1;
		base.Invoke("Next", 0.5f);
	}

	public void TutorialBadGuyExit()
	{
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tutorial);
		this.EnableCharacterAnimationMode();
		this.currentConversation = this.badGuyOutro;
		this.currentConversationID = 2;
		base.Invoke("Next", 0.5f);
	}

	public void TutorialBossFishing()
	{
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tutorial);
		this.EnableCharacterAnimationMode();
		this.currentConversation = this.bossFishExplanation;
		this.currentConversationID = 3;
		base.Invoke("Next", 0.5f);
	}

	public void TutorialCollectFishingExperiance()
	{
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tutorial);
		this.EnableCharacterAnimationMode();
		this.currentConversation = this.collectFishingExperienceExplanation;
		this.currentConversationID = 4;
		base.Invoke("Next", 0.5f);
	}

	public void TutorialItemBoxtOpened()
	{
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tutorial);
		this.EnableCharacterAnimationMode();
		this.currentConversation = this.itemBoxOpened;
		this.currentConversationID = 5;
		base.Invoke("Next", 0.5f);
	}

	public void TutorialBadGuyEnterItemGift()
	{
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tutorial);
		this.EnableCharacterAnimationMode();
		this.currentConversation = this.badGuyItemGift;
		this.currentConversationID = 6;
		base.Invoke("Next", 0.5f);
	}

	public void TutorialRoyalGuyIntro()
	{
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tutorial);
		this.EnableCharacterAnimationMode();
		this.currentConversation = this.skilltreeIntro;
		this.currentConversationID = 7;
		base.Invoke("Next", 0.5f);
	}

	public void TutorialRoyalGuyIntroGive()
	{
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tutorial);
		this.EnableCharacterAnimationMode();
		this.currentConversation = this.skilltreeIntro;
		this.currentConversationID = 8;
		base.Invoke("Next", 0.5f);
	}

	private void SetCharacter(Animator characterAnimator)
	{
		this.currentAnimator = characterAnimator;
		this.currentAnimator.gameObject.SetActive(true);
	}

	private void Next()
	{
		if (this.conversationIndex < this.currentConversation.Length)
		{
			if (this.currentConversation[this.conversationIndex].orange == "!Character")
			{
				if (this.currentConversation[this.conversationIndex].gray == "!BG")
				{
					this.ExitCharacter();
					this.SetCharacter(this.BGAnimator);
				}
				else if (this.currentConversation[this.conversationIndex].gray == "!GG")
				{
					this.ExitCharacter();
					this.SetCharacter(this.GGAnimator);
				}
				else if (this.currentConversation[this.conversationIndex].gray == "!RG")
				{
					this.ExitCharacter();
					this.SetCharacter(this.RGAnimator);
				}
				else
				{
					UnityEngine.Debug.Log("NO SUCH CHARACTER");
				}
				this.conversationIndex++;
				this.RunAfterDelay(1f, delegate()
				{
					this.Next();
				});
			}
			else if (!(this.currentConversation[this.conversationIndex].orange == "!BGLeaveAnimation"))
			{
				if (this.currentConversation[this.conversationIndex].orange == "!Special")
				{
					if (this.currentConversation[this.conversationIndex].gray == "!Name")
					{
						this.RunAfterDelay(1f, delegate()
						{
							this.dialogBaloon.ShowInputfield();
						});
						this.disableNexting = true;
						this.RunAfterDelay(1.5f, delegate()
						{
							base.GetComponent<RectTransform>().DOAnchorPosY(800f, 0.2f, false);
						});
					}
				}
				else
				{
					if (this.currentConversation[this.conversationIndex].orange.Contains("{name}"))
					{
						this.currentConversation[this.conversationIndex].orange = this.currentConversation[this.conversationIndex].orange.Replace("{name}", SettingsManager.Instance.PlayerName);
					}
					if (this.currentConversation[this.conversationIndex].gray == "!Next")
					{
						this.dialogBaloon.DisplayText(this.currentConversation[this.conversationIndex].orange, string.Empty);
						this.conversationIndex++;
						this.Next();
					}
					else
					{
						this.DelayNexting(1);
						this.dialogBaloon.DisplayText(this.currentConversation[this.conversationIndex].orange, this.currentConversation[this.conversationIndex].gray);
						this.conversationIndex++;
					}
				}
			}
		}
		else
		{
			this.ExitCharacter();
			this.DisableCharacterAnimationMode();
		}
	}

	private void DelayNexting(int delayTime)
	{
		this.disableNexting = true;
		this.RunAfterDelay((float)delayTime, delegate()
		{
			if (this.currentAnimator != null)
			{
				this.disableNexting = false;
			}
		});
	}

	private void ExitCharacter()
	{
		if (this.currentAnimator != null)
		{
			this.currentAnimator.SetTrigger("Exit");
			this.dialogBaloon.CloseBaloon();
			this.animatorTodisable = this.currentAnimator;
			this.RunAfterDelay(0.5f, delegate()
			{
				this.animatorTodisable.gameObject.SetActive(false);
			});
		}
	}

	public void OnClicked()
	{
		if (this.disableNexting)
		{
			return;
		}
		this.Next();
	}

	private void DisableCharacterAnimationMode()
	{
		this.TweenKiller();
		base.Invoke("TransitionBackToMain", 0.6f);
		this.disableNexting = true;
		this.currentAnimator = null;
		this.shade.DOFade(0f, 0.5f).OnComplete(delegate
		{
			this.shade.gameObject.SetActive(false);
		});
		if (this.OnConversationCompleted != null)
		{
			this.OnConversationCompleted(this.currentConversationID);
		}
	}

	private void TransitionBackToMain()
	{
		if (ScreenManager.Instance.CurrentScreen == ScreenManager.Screen.Tutorial)
		{
			ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Main);
		}
		this.isInConversation = false;
		base.gameObject.SetActive(false);
	}

	private void EnableCharacterAnimationMode()
	{
		base.gameObject.SetActive(true);
		this.isInConversation = true;
		this.TweenKiller();
		base.CancelInvoke("TransitionBackToMain");
		this.conversationIndex = 0;
		this.shade.gameObject.SetActive(true);
		this.shade.DOFade(0.27f, 0.5f).OnComplete(delegate
		{
		});
	}

	private void TweenKiller()
	{
		this.shade.DOKill(false);
	}

	public static CharacterConversationHandler Instance;

	[SerializeField]
	private CharacterBaloonBehaviour dialogBaloon;

	[SerializeField]
	private Image shade;

	[SerializeField]
	private Animator GGAnimator;

	[SerializeField]
	private Animator BGAnimator;

	[SerializeField]
	private Animator RGAnimator;

	[SerializeField]
	private CharacterConversationHandler.ConversationStrings[] intro;

	[SerializeField]
	private CharacterConversationHandler.ConversationStrings[] badGuyIntro;

	[SerializeField]
	private CharacterConversationHandler.ConversationStrings[] badGuyOutro;

	[SerializeField]
	private CharacterConversationHandler.ConversationStrings[] bossFishExplanation;

	[SerializeField]
	private CharacterConversationHandler.ConversationStrings[] collectFishingExperienceExplanation;

	[SerializeField]
	private CharacterConversationHandler.ConversationStrings[] itemBoxOpened;

	[SerializeField]
	private CharacterConversationHandler.ConversationStrings[] badGuyItemGift;

	[SerializeField]
	private CharacterConversationHandler.ConversationStrings[] skilltreeIntro;

	[SerializeField]
	private CharacterConversationHandler.ConversationStrings[] skilltreeIntroGive;

	private CharacterConversationHandler.ConversationStrings[] currentConversation;

	private int conversationIndex;

	public bool disableNexting = true;

	private Animator currentAnimator;

	private Animator animatorTodisable;

	private int currentConversationID = -1;

	public bool isInConversation;

	[Serializable]
	private class ConversationStrings
	{
		public string orange;

		public string gray;
	}
}
