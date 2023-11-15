using System;
using System.Numerics;
using UnityEngine;

public class IGNLobster : InGameNotification
{
	public override InGameNotification.IGN Type
	{
		get
		{
			return InGameNotification.IGN.Lobster;
		}
	}

	public override bool IsClearable
	{
		get
		{
			return true;
		}
	}

	public override bool RemoveOnReset
	{
		get
		{
			return true;
		}
	}

	public void RanomizeContentInCage()
	{
		int[] values = (int[])Enum.GetValues(typeof(IGNLobster.LobsterCageContent));
		int valueWithChance = FHelper.GetValueWithChance(values, IGNLobster.CHANCES);
		int value = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.FishValue>();
		int value2 = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.InGameFishValueModifier>();
		int num = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.SkillTier>();
		int value3 = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.CargoModifier>();
		int currentDWLevel = DWHelper.CurrentDWLevel;
		int value4 = 5;
		BigInteger right = BigInteger.Pow(value4, currentDWLevel);
		int value5 = valueWithChance * valueWithChance;
		int value6 = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.LobsterValue>();
		int value7 = UnityEngine.Random.Range(valueWithChance, valueWithChance + 2);
		this.CashAmount = ValueModifier.GetTotalValueIncrease() * value5 * value6 * right * value7 * value3 * value * value2;
		if (TournamentManager.Instance.IsInsideTournament && this.CashAmount > 500000000L)
		{
			this.CashAmount = 500000000;
		}
		this.Content = (IGNLobster.LobsterCageContent)valueWithChance;
	}

	private void Random()
	{
	}

	private static readonly int[] CHANCES = new int[]
	{
		20,
		30,
		30,
		20
	};

	public BigInteger CashAmount = 0;

	public IGNLobster.LobsterCageContent Content;

	public enum LobsterCageContent
	{
		None,
		Small,
		Great,
		Huge
	}
}
