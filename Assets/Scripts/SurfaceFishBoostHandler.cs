using System;
using UnityEngine;

public class SurfaceFishBoostHandler : MonoBehaviour
{
	public static SurfaceFishBoostHandler Instance { get; private set; }

	public bool IsBoostActive
	{
		get
		{
			DateTime? dateTime = this.lastActivated;
			if (dateTime == null || !this.magicWormsItem.IsEquipped)
			{
				return false;
			}
			DateTime now = DateTime.Now;
			DateTime? dateTime2 = this.lastActivated;
			bool flag = (now - dateTime2.Value).TotalSeconds < 10.0;
			if (!flag)
			{
				this.lastActivated = null;
			}
			return flag;
		}
	}

	private void Awake()
	{
		SurfaceFishBoostHandler.Instance = this;
	}

	private void Start()
	{
		SkillManager.Instance.OnSkillLevelChanged += this.Instance_OnSkillLevelChanged;
	}

	private void Instance_OnSkillLevelChanged(Skill skill, LevelChange levelChange)
	{
		if (this.magicWormsItem.IsEquipped && skill.IsTierSkill && FHelper.DidRollWithChance(ItemAndSkillValues.GetCurrentTotalValueFor<Skills.DoubleSurfaceFishChance>()))
		{
			this.lastActivated = new DateTime?(DateTime.Now);
		}
	}

	[SerializeField]
	private Item magicWormsItem;

	private DateTime? lastActivated;
}
