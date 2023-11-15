using System;
using UnityEngine;

public class MakeSureAFKCrewIsReceivedHandler : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnApplicationPause(bool didPause)
	{
		if (didPause && SkillManager.Instance != null && TutorialManager.Instance != null && !TournamentManager.Instance.IsInsideTournament && this.afkCrewMember.CurrentLevel == 0 && this.firstQuest.IsCompleted)
		{
			this.afkCrewMember.SetHasNotifiedSkillUnlocked(true);
			this.afkCrewMember.LevelUpForFree();
		}
	}

	[SerializeField]
	private Skill afkCrewMember;

	[SerializeField]
	private TutorialSliceBase unlockCrewMemberTutorial;

	[SerializeField]
	private Quest firstQuest;
}
