using System;
using System.Collections.Generic;
using UnityEngine;

public class UIActiveCrewMembersList : MonoBehaviour
{
	private void Awake()
	{
		SkillManager.Instance.OnSkillLevelChanged += this.Instance_OnSkillLevelChanged;
		base.gameObject.SetActive(false);
	}

	private void Instance_OnSkillLevelChanged(Skill skill, LevelChange levelChange)
	{
		if (!skill.GetExtraInfo().IsCrew)
		{
			return;
		}
		this.UpdateList(skill);
	}

	private void UpdateList(Skill skill)
	{
		if (!this.activeCrewMembers.Contains(skill) && skill.CurrentLevel > 0)
		{
			base.gameObject.SetActive(true);
			this.activeCrewMembers.Add(skill);
			UIInGameNotificationItem uiinGameNotificationItem = UnityEngine.Object.Instantiate<UIInGameNotificationItem>(this.crewItem, base.transform, false);
			uiinGameNotificationItem.Init(new IGNNewCrew(skill, false), null, this.dialogCanvas);
			uiinGameNotificationItem.gameObject.SetActive(this.isShowing);
			this.uiItems.Add(uiinGameNotificationItem);
		}
		else if (skill.CurrentLevel == 0 && this.activeCrewMembers.Contains(skill))
		{
			UIInGameNotificationItem uiinGameNotificationItem2 = this.uiItems.Find((UIInGameNotificationItem x) => (x.InGameNotification as IGNNewCrew).Skill == skill);
			this.activeCrewMembers.Remove((uiinGameNotificationItem2.InGameNotification as IGNNewCrew).Skill);
			this.uiItems.Remove(uiinGameNotificationItem2);
			UnityEngine.Object.Destroy(uiinGameNotificationItem2.gameObject);
		}
		float num = (this.activeCrewMembers.Count < 10) ? 1f : 0.7f;
		base.transform.localScale = new Vector3(num, num, 0f);
	}

	public UIInGameNotificationItem GetUIInGameNotificationItemFor(Skill skill)
	{
		foreach (UIInGameNotificationItem uiinGameNotificationItem in this.uiItems)
		{
			if (uiinGameNotificationItem.InGameNotification is IGNNewCrew)
			{
				IGNNewCrew ignnewCrew = (IGNNewCrew)uiinGameNotificationItem.InGameNotification;
				if (ignnewCrew.Skill == skill)
				{
					return uiinGameNotificationItem;
				}
			}
		}
		return null;
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.R))
		{
		}
	}

	public void Toggle()
	{
		if (this.uiItems.Count == 0)
		{
			return;
		}
		this.isShowing = !this.isShowing;
		foreach (UIInGameNotificationItem uiinGameNotificationItem in this.uiItems)
		{
			uiinGameNotificationItem.gameObject.SetActive(this.isShowing);
		}
	}

	[SerializeField]
	private UIInGameNotificationItem crewItem;

	[SerializeField]
	private Transform dialogCanvas;

	private bool isShowing = true;

	private List<Skill> activeCrewMembers = new List<Skill>();

	private List<UIInGameNotificationItem> uiItems = new List<UIInGameNotificationItem>();
}
