using System;
using UnityEngine;

public class VisualSkillShell : MonoBehaviour
{
	private void Awake()
	{
		SkillTreeManager.Instance.AddUIConnection(this.skill, base.transform);
		this.visualSkillInstance = UnityEngine.Object.Instantiate<SkilltreeSkill>(this.visualSkillPrefab, base.transform);
		this.visualSkillInstance.transform.SetParent(base.transform.parent);
	}

	private void Start()
	{
		Transform uiobjectRelatedTo = SkillTreeManager.Instance.GetUIObjectRelatedTo(this.skill.GetExtraInfo().DependancyLevelUp);
		this.visualSkillInstance.Ini(uiobjectRelatedTo, this.skill, this.isAlteredVisuals);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	[SerializeField]
	[Header("References")]
	private SkilltreeSkill visualSkillPrefab;

	[SerializeField]
	private Skill skill;

	[SerializeField]
	private bool isAlteredVisuals;

	private SkilltreeSkill visualSkillInstance;
}
