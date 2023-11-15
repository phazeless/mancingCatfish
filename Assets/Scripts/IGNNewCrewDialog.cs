using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IGNNewCrewDialog : InGameNotificationDialog<IGNNewCrew>
{
	private bool IsActiveSkill
	{
		get
		{
			return this.inGameNotification.Skill.IsActiveSkill;
		}
	}

	private void SetActivationState()
	{
		if (this.IsActiveSkill)
		{
			if (this.inGameNotification.Skill.IsOnCooldown)
			{
				this.cooldownCover.DOKill(true);
				this.activationIcon.DOKill(true);
				this.cooldownCover.gameObject.SetActive(true);
				this.activationIcon.gameObject.SetActive(false);
				float totalSecondsLeftOnCooldown = this.inGameNotification.Skill.GetTotalSecondsLeftOnCooldown();
				int cooldown = this.inGameNotification.Skill.Cooldown;
				float num = Mathf.Min((float)cooldown, totalSecondsLeftOnCooldown);
				this.cooldownCover.transform.localScale = new Vector2(1f, num / (float)cooldown);
				this.cooldownCover.DOScaleY(0f, num).SetEase(Ease.Linear).OnComplete(delegate
				{
					this.activationIcon.gameObject.SetActive(true);
					this.activationIcon.DOPunchScale(Vector2.one, 0.5f, 10, 1f);
					this.cooldownCover.gameObject.SetActive(false);
				});
			}
		}
		else
		{
			UnityEngine.Object.Destroy(this.activationIcon.gameObject);
			UnityEngine.Object.Destroy(this.cooldownCover.gameObject);
		}
	}

	protected override void OnAboutToOpen()
	{
		this.dialogContentHolder.SetActive(true);
		this.UpdateUI();
	}

	public void Upgrade()
	{
		this.inGameNotification.Skill.TryLevelUp();
		this.UpdateUI();
	}

	public void Activate()
	{
		if (this.IsActiveSkill)
		{
			this.inGameNotification.Skill.Activate();
			this.SetActivationState();
		}
		this.Close(false);
	}

	public void Unlock()
	{
		this.inGameNotification.OverrideClearable = true;
		PurchaseCrewMemberHandler.OverrideRandomWithCrewMember = this.skill;
		this.Close(true);
	}

	public void InviteFriends()
	{
		NativeShare nativeShare = new NativeShare();
		nativeShare.SetTitle("Hooked Inc: Fisher Tycoon");
		nativeShare.SetSubject("Hooked Inc: Fisher Tycoon");
		nativeShare.SetText("http://acegames.se/hi");
		nativeShare.Share();
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
		this.skill = this.inGameNotification.Skill;
		this.IsNewCrewDialog = this.inGameNotification.IsNewCrew;
		this.portraitImage.sprite = this.skill.GetExtraInfo().Icon;
		this.ActivateGameObjects();
		this.SetActivationState();
	}

	protected override void OnOpened()
	{
	}

	protected override void OnRemovedFromList()
	{
	}

	protected override void OnReturned()
	{
		this.dialogContentHolder.SetActive(false);
	}

	private void ActivateGameObjects()
	{
		this.unlockButton.gameObject.SetActive(this.IsNewCrewDialog);
		this.upgradeButton.gameObject.SetActive(!this.IsNewCrewDialog);
		this.activateButton.gameObject.SetActive(!this.IsNewCrewDialog);
		for (int i = 0; i < this.skill.SkillBehaviours.Count; i++)
		{
			this.attributes[i].gameObject.SetActive(true);
		}
	}

	private void UpdateUI()
	{
		this.levelLabel.gameObject.SetActive(!this.IsNewCrewDialog);
		this.unlockButton.interactable = this.unlockCrewMemberSkill.IsAvailableForLevelUp;
		this.unlockLabel.SetVariableText(new string[]
		{
			this.unlockCrewMemberSkill.CostForNextLevelUp.ToString()
		});
		this.title.SetText(this.skill.GetExtraInfo().MiscText);
		this.description.SetText(this.skill.GetExtraInfo().DescriptionText);
		this.levelLabel.SetVariableText(new string[]
		{
			this.skill.CurrentLevel.ToString()
		});
		bool isFacebookCrew = this.skill.GetExtraInfo().IsFacebookCrew;
		this.upgradeButton.gameObject.SetActive(!isFacebookCrew && !this.IsNewCrewDialog);
		this.inviteFriendsButton.gameObject.SetActive(isFacebookCrew);
		this.upgradeButton.interactable = (this.skill.IsAvailableForLevelUp && !this.skill.GetExtraInfo().HasLevelUpDependency && !TournamentManager.Instance.IsInsideTournament);
		if (this.skill.GetExtraInfo().HasLevelUpDependency)
		{
			this.upgradeLabel.SetText("N/A");
		}
		else if (this.skill.IsMaxLevel)
		{
			this.upgradeLabel.SetText("MAX");
		}
		else
		{
			this.upgradeLabel.SetVariableText(new string[]
			{
				this.skill.CostForNextLevelUp.ToString()
			});
		}
		if (isFacebookCrew)
		{
			this.expireLabel.SetText("You currently have <b><color=orange>" + FacebookHandler.Instance.CachedFriendCount + "</color></b> friends playing.");
		}
		this.activateButton.interactable = !this.skill.IsOnCooldown;
		if (this.IsActiveSkill)
		{
			this.activateLabel.SetText("Activate");
		}
		else
		{
			this.activateLabel.SetText("Okay");
		}
		this.SetupAttributeValues();
	}

	private void SetupAttributeValues()
	{
		for (int i = 0; i < this.skill.SkillBehaviours.Count; i++)
		{
			SkillBehaviour skillBehaviour = this.skill.SkillBehaviours[i];
			TextMeshProUGUI textMeshProUGUI = this.attributes[i];
			float valueAtLevel = skillBehaviour.GetValueAtLevel(this.skill.NextLevel);
			string text = (valueAtLevel <= 0f) ? string.Empty : "+";
			float totalValueAtLevel = skillBehaviour.GetTotalValueAtLevel(this.skill.CurrentLevel);
			string text2 = (totalValueAtLevel <= 0f) ? string.Empty : "+";
			string text3 = FHelper.FindBracketAndReplace(skillBehaviour.Description, new string[]
			{
				string.Concat(new object[]
				{
					"<b>",
					text,
					valueAtLevel,
					skillBehaviour.PostFixCharacter,
					"</b>"
				})
			});
			this.attributes[i].SetVariableText(new string[]
			{
				string.Empty,
				text3,
				string.Concat(new object[]
				{
					" (",
					text2,
					skillBehaviour.GetTotalValueAtLevel(this.skill.CurrentLevel),
					skillBehaviour.PostFixCharacter,
					")"
				})
			});
		}
	}

	private void Update()
	{
		if (!this.dialogContentHolder.activeInHierarchy)
		{
			return;
		}
		if (this.IsNewCrewDialog)
		{
			string text = FHelper.FromSecondsToHoursMinutesSecondsFormat(this.inGameNotification.ExpiresInSeconds);
			if (string.IsNullOrEmpty(text))
			{
				this.expireLabel.SetText("Last chance...");
			}
			else
			{
				this.expireLabel.SetText("Expires: " + text);
			}
			if (this.unlockButton.interactable != this.unlockCrewMemberSkill.IsAvailableForLevelUp)
			{
				this.unlockButton.interactable = this.unlockCrewMemberSkill.IsAvailableForLevelUp;
			}
		}
		else if (this.IsActiveSkill)
		{
			this.expireLabel.SetText(string.Empty);
			if (this.inGameNotification.Skill.IsOnCooldown)
			{
				float totalSecondsLeftOnCooldown = this.inGameNotification.Skill.GetTotalSecondsLeftOnCooldown();
				this.expireLabel.SetText("Cooldown: " + FHelper.FromSecondsToHoursMinutesSecondsFormat(totalSecondsLeftOnCooldown));
			}
			else if (!this.activateButton.interactable)
			{
				this.activateButton.interactable = true;
			}
		}
		else if (!this.skill.GetExtraInfo().IsFacebookCrew)
		{
			this.expireLabel.gameObject.SetActive(false);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.cooldownCover != null && this.cooldownCover.transform != null)
		{
			this.cooldownCover.transform.DOKill(true);
		}
		if (this.activationIcon != null && this.activationIcon.transform != null)
		{
			this.activationIcon.transform.DOKill(true);
		}
	}

	private Skill skill;

	[SerializeField]
	private TextMeshProUGUI title;

	[SerializeField]
	private TextMeshProUGUI description;

	[SerializeField]
	private TextMeshProUGUI unlockLabel;

	[SerializeField]
	private TextMeshProUGUI upgradeLabel;

	[SerializeField]
	private TextMeshProUGUI activateLabel;

	[SerializeField]
	private TextMeshProUGUI expireLabel;

	[SerializeField]
	private TextMeshProUGUI levelLabel;

	[SerializeField]
	private Button unlockButton;

	[SerializeField]
	private Button upgradeButton;

	[SerializeField]
	private Button activateButton;

	[SerializeField]
	private Button closeButton;

	[SerializeField]
	private Button returnButton;

	[SerializeField]
	private Button inviteFriendsButton;

	[SerializeField]
	private Image portraitImage;

	[SerializeField]
	private TextMeshProUGUI[] attributes;

	[SerializeField]
	private Skill unlockCrewMemberSkill;

	public bool IsNewCrewDialog = true;

	[SerializeField]
	private Transform cooldownCover;

	[SerializeField]
	private Transform activationIcon;

	[SerializeField]
	private GameObject dialogContentHolder;
}
