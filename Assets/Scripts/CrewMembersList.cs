using System;
using System.Collections.Generic;
using UnityEngine;

public class CrewMembersList : UIListNormal
{
	private void Start()
	{
		IList<Skill> crewMembers = SkillManager.Instance.CrewMembers;
		foreach (Skill skill in crewMembers)
		{
			skill.SetVisualPrefab(this.prefabCrewMemberItem);
		}
		base.AddItem(new UIUnlockCrewMember.UnlockCrewMemberContent(this.prefabUnlockNewCrewMember), true);
		foreach (Skill listItemContent in crewMembers)
		{
			base.AddItem(listItemContent, true);
		}
	}

	private void Update()
	{
	}

	private void UnlockCrewMember()
	{
	}

	[SerializeField]
	private UIListItem prefabCrewMemberItem;

	[SerializeField]
	private UIListItem prefabUnlockNewCrewMember;
}
