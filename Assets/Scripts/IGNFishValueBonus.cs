using System;

[Serializable]
public class IGNFishValueBonus : InGameNotification
{
	public void Refresh(Skill fishValueBonusSkill)
	{
		this.fishValueBonusSkill = fishValueBonusSkill;
		base.SetExpiration(fishValueBonusSkill.GetTotalSecondsLeftOnDuration());
		this.HasChanged = true;
	}

	public bool IsReady
	{
		get
		{
			return this.fishValueBonusSkill != null;
		}
	}

	public float TotalSecondsLeftOnDuration
	{
		get
		{
			if (this.IsReady)
			{
				return this.fishValueBonusSkill.GetTotalSecondsLeftOnDuration();
			}
			return 0f;
		}
	}

	public int CurrentBonus
	{
		get
		{
			if (this.IsReady)
			{
				return this.fishValueBonusSkill.CurrentLevel;
			}
			return 1;
		}
	}

	public override bool IsClearable
	{
		get
		{
			return false;
		}
	}

	public override InGameNotification.IGN Type
	{
		get
		{
			return InGameNotification.IGN.FishValueBonus;
		}
	}

	public override bool RemoveOnReset
	{
		get
		{
			return false;
		}
	}

	public bool HasChanged;

	private Skill fishValueBonusSkill;
}
