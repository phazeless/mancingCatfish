using System;

public static class DWHelper
{
	public static int CurrentDWLevel
	{
		get
		{
			return DWHelper.CurrentDWSkill.CurrentLevel;
		}
	}

	public static Skill CurrentDWSkill
	{
		get
		{
			if (TournamentManager.Instance.IsInsideTournament)
			{
				return TournamentManager.Instance.TournamentDWLevelSkill;
			}
			return SkillManager.Instance.DeepWaterSkill;
		}
	}
}
