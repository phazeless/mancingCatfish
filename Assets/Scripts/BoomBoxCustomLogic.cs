using System;
using UnityEngine;

[Serializable]
public class BoomBoxCustomLogic : BaseItemCustomLogic
{
	private DateTime LastGeneratedRocket
	{
		get
		{
			DateTime? dateTime = this.lastGeneratedRocket;
			return dateTime.Value;
		}
	}

	protected override void Load()
	{
		if (EncryptedPlayerPrefs.HasKey("KEY_LAST_GENERATED_ROCKET"))
		{
			string @string = EncryptedPlayerPrefs.GetString("KEY_LAST_GENERATED_ROCKET", TimeManager.Instance.RealNow.Ticks.ToString());
			long ticks = long.Parse(@string);
			this.lastGeneratedRocket = new DateTime?(new DateTime(ticks));
		}
	}

	public override void Save()
	{
		DateTime? dateTime = this.lastGeneratedRocket;
		if (dateTime != null)
		{
			EncryptedPlayerPrefs.SetString("KEY_LAST_GENERATED_ROCKET", this.LastGeneratedRocket.Ticks.ToString(), true);
		}
	}

	public override void OnEquipped()
	{
		this.lastGeneratedRocket = new DateTime?(TimeManager.Instance.RealNow);
	}

	public override void Update(Item item)
	{
		if (TimeManager.Instance.IsInitializedWithInternetTime)
		{
			base.Update(item);
			DateTime? dateTime = this.lastGeneratedRocket;
			if (dateTime == null)
			{
				this.lastGeneratedRocket = new DateTime?(TimeManager.Instance.RealNow);
			}
			if (FHelper.HasSecondsPassedSince(3600f, this.LastGeneratedRocket, true))
			{
				int amount = ConsumableManager.Instance.GetAmount(this.firework);
				int num = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.FireworkMaxAmount>();
				int num2 = num - amount;
				if (num2 > 0)
				{
					int b = this.CalculateFireworkAmountBasedOnLastGeneratedWithNoLimits();
					int amount2 = Mathf.Min(num2, b);
					ConsumableManager.Instance.Grant(this.firework, amount2, ResourceChangeReason.ItemCustomLogics, false);
				}
				this.lastGeneratedRocket = new DateTime?(TimeManager.Instance.RealNow);
			}
		}
	}

	private int CalculateFireworkAmountBasedOnLastGeneratedWithNoLimits()
	{
		return (int)((TimeManager.Instance.RealNow - this.LastGeneratedRocket).TotalSeconds / 3600.0);
	}

	private const string KEY_LAST_GENERATED_ROCKET = "KEY_LAST_GENERATED_ROCKET";

	[SerializeField]
	private FireworkConsumable firework;

	private const float GENERATE_ROCKET_EVERY_SECOND = 3600f;

	private DateTime? lastGeneratedRocket;
}
