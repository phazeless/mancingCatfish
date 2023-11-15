using System;
using UnityEngine;

public class WheelBonusHandler : MonoBehaviour
{
	public bool HasCreatedWheelBonusIGN
	{
		get
		{
			return this.wheelBonusIGN != null;
		}
	}

	private void Awake()
	{
		InGameNotificationManager.Instance.OnInGameNotificationCreated += this.Instance_OnInGameNotificationCreated;
	}

	private void Start()
	{
		this.wheelBonusSkill.OnSkillDurationEnd += this.WheelBonusSkill_OnSkillDurationEnd;
		this.wheelBonusSkill.OnSkillActivation += this.WheelBonusSkill_OnSkillActivation;
	}

	private void Instance_OnInGameNotificationCreated(UIInGameNotificationItem ui, InGameNotification ign)
	{
		if (!(ign is IGNFishValueBonus))
		{
			return;
		}
		this.wheelBonusIGN = (IGNFishValueBonus)ign;
		this.wheelBonusIGN.Refresh(this.wheelBonusSkill);
	}

	private void WheelBonusSkill_OnSkillActivation(Skill skill)
	{
		if (!this.HasCreatedWheelBonusIGN)
		{
			this.wheelBonusIGN = new IGNFishValueBonus();
			InGameNotificationManager.Instance.Create<IGNFishValueBonus>(this.wheelBonusIGN);
		}
		this.wheelBonusIGN.Refresh(skill);
	}

	private void WheelBonusSkill_OnSkillDurationEnd(Skill skill)
	{
		if (this.wheelBonusIGN != null)
		{
			this.wheelBonusIGN.Refresh(skill);
		}
		this.wheelBonusIGN = null;
		skill.SetCurrentLevel(1, LevelChange.SoftReset);
	}

	[SerializeField]
	private Skill wheelBonusSkill;

	private IGNFishValueBonus wheelBonusIGN;
}
