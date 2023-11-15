using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DWLProgressBehaviour : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action OnDwProgressing;

	private void Start()
	{
		this.coreDeepWaterSkill = SkillManager.Instance.DeepWaterSkill;
		int num = 0;
		while (this.dwChildHolder.childCount - num > 0)
		{
			this.deepWaterIconsTransforms.Add(this.dwChildHolder.GetChild(this.dwChildHolder.childCount - num - 1));
			num++;
		}
		this.coreDeepWaterSkill.OnSkillLevelUp += this.CoreDeepWaterSkill_OnSkillLevelUp;
		this.coreDeepWaterSkill.OnSkillAvailabilityChanged += this.CoreDeepWaterSkill_OnSkillAvailabilityChanged;
		this.coreDeepWaterSkill.OnSkillCostChange += this.CoreDeepWaterSkill_OnSkillCostChange;
		this.CoreDeepWaterSkill_OnSkillAvailabilityChanged(this.coreDeepWaterSkill, this.coreDeepWaterSkill.IsAvailableForLevelUp);
		this.prestigeSkill.OnSkillLevelUp += this.PrestigeSkill_OnSkillLevelUp;
		this.SetStartStateOfDws();
		this.UpdateMainHeaderDWIcon();
	}

	private void PrestigeSkill_OnSkillLevelUp(Skill skill, LevelChange levelChange)
	{
		this.RunAfterDelay(0f, delegate()
		{
			this.SetStartStateOfDws();
		});
	}

	private void CoreDeepWaterSkill_OnSkillCostChange(Skill skill, BigInteger newCost)
	{
		this.Show();
	}

	private void UpdateMainHeaderDWIcon()
	{
		if (this.mainHeaderDWIcon != null)
		{
			UnityEngine.Object.Destroy(this.mainHeaderDWIcon.gameObject);
		}
		if (!TournamentManager.Instance.IsInsideTournament)
		{
			this.mainHeaderDWIcon = UnityEngine.Object.Instantiate<GameObject>(this.deepWaterIconsTransforms[this.coreDeepWaterSkill.CurrentLevel].gameObject, this.mainHeaderDWHolder, false).GetComponent<UIDeepWaterItem>();
		}
		else
		{
			this.mainHeaderDWIcon = UnityEngine.Object.Instantiate<GameObject>(this.dwWaterTournamentPrefab, this.mainHeaderDWHolder, false).GetComponent<UIDeepWaterItem>();
		}
		this.mainHeaderDWIcon.transform.localPosition = UnityEngine.Vector2.zero;
		this.mainHeaderDWIcon.transform.localScale = new UnityEngine.Vector2(0.7f, 0.7f);
		this.mainHeaderDWIcon.SetState(UIDeepWaterItem.DWState.Current);
		this.mainHeaderDWIcon.FullHolder.transform.GetChild(0).gameObject.SetActive(false);
	}

	private void CoreDeepWaterSkill_OnSkillLevelUp(Skill skill, LevelChange levelChange)
	{
		this.Show();
		this.UpdateMainHeaderDWIcon();
		if (this.coreDeepWaterSkill.NextLevel < this.deepWaterIconsTransforms.Count)
		{
			if (this.isAvaliable)
			{
				this.goDialog.GetComponent<GoToDialogTweens>().Enter();
				this.goDialog.GetComponent<GoToDialogTweens>().SetAvaliable();
				this.deepWaterIconsTransforms[this.coreDeepWaterSkill.NextLevel].GetComponent<UIDeepWaterItem>().AnimateDiscoverAvaliable();
				this.button.interactable = true;
			}
			else
			{
				this.goDialog.GetComponent<GoToDialogTweens>().SetUnaviable();
				this.goDialog.GetComponent<GoToDialogTweens>().Enter();
			}
		}
	}

	private void CoreDeepWaterSkill_OnSkillAvailabilityChanged(Skill skill, bool availability)
	{
		this.isAvaliable = availability;
		this.Show();
		if (this.coreDeepWaterSkill.NextLevel < this.deepWaterIconsTransforms.Count)
		{
			if (availability)
			{
				this.goDialog.GetComponent<GoToDialogTweens>().Enter();
				this.goDialog.GetComponent<GoToDialogTweens>().SetAvaliable();
				this.button.interactable = true;
				this.deepWaterIconsTransforms[this.coreDeepWaterSkill.NextLevel].GetComponent<UIDeepWaterItem>().AnimateDiscoverAvaliable();
			}
			else
			{
				this.goDialog.GetComponent<GoToDialogTweens>().SetUnaviable();
				this.goDialog.GetComponent<GoToDialogTweens>().Enter();
				this.button.interactable = false;
				this.deepWaterIconsTransforms[this.coreDeepWaterSkill.NextLevel].GetComponent<UIDeepWaterItem>().AnimateDiscover();
			}
		}
	}

	private void SetStartStateOfDws()
	{
		for (int i = 0; i < this.deepWaterIconsTransforms.Count; i++)
		{
			if (i < this.coreDeepWaterSkill.CurrentLevel)
			{
				this.deepWaterIconsTransforms[i].GetComponent<UIDeepWaterItem>().SetState(UIDeepWaterItem.DWState.Reached);
			}
			else if (i == this.coreDeepWaterSkill.CurrentLevel)
			{
				this.deepWaterIconsTransforms[this.coreDeepWaterSkill.CurrentLevel].GetComponent<UIDeepWaterItem>().SetState(UIDeepWaterItem.DWState.Current);
			}
			else if (i == this.coreDeepWaterSkill.NextLevel)
			{
				if (this.coreDeepWaterSkill.NextLevel < this.deepWaterIconsTransforms.Count)
				{
					Transform transform = this.deepWaterIconsTransforms[this.coreDeepWaterSkill.NextLevel];
					if (this.isAvaliable)
					{
						transform.GetComponent<UIDeepWaterItem>().SetState(UIDeepWaterItem.DWState.DiscoveredAvaliable);
					}
					else
					{
						transform.GetComponent<UIDeepWaterItem>().SetState(UIDeepWaterItem.DWState.Discovered);
					}
				}
			}
			else
			{
				this.deepWaterIconsTransforms[i].GetComponent<UIDeepWaterItem>().SetState(UIDeepWaterItem.DWState.UnDiscovered);
			}
		}
	}

	public void Show()
	{
		if (this.coreDeepWaterSkill.NextLevel >= base.transform.childCount)
		{
			this.goDialog.gameObject.SetActive(false);
			return;
		}
		Transform parent = this.deepWaterIconsTransforms[this.coreDeepWaterSkill.NextLevel];
		this.goDialog.transform.SetParent(parent, false);
		this.goDialog.transform.SetAsFirstSibling();
		this.goDialog.GetComponent<RectTransform>().anchoredPosition = UnityEngine.Vector2.zero;
		BigInteger costForLevelUp = this.coreDeepWaterSkill.GetCostForLevelUp(this.coreDeepWaterSkill.CurrentLevel);
		string text = CashFormatter.SimpleToCashRepresentation(costForLevelUp, 3, false, true);
		this.uiCost.SetText(text);
	}

	public void AnimateProgress()
	{
		if (DWLProgressBehaviour.OnDwProgressing != null)
		{
			DWLProgressBehaviour.OnDwProgressing();
		}
		this.deepWaterIconsTransforms[this.coreDeepWaterSkill.NextLevel].GetComponent<UIDeepWaterItem>().AnimateArrive();
		this.deepWaterIconsTransforms[this.coreDeepWaterSkill.CurrentLevel].GetComponent<UIDeepWaterItem>().AnimateLeave();
	}

	public void OnGoToDeeperWatersClick()
	{
		this.AnimateProgress();
		this.goDialog.GetComponent<GoToDialogTweens>().Exit();
	}

	[SerializeField]
	private Skill prestigeSkill;

	[SerializeField]
	private GameObject goDialog;

	[SerializeField]
	private TextMeshProUGUI uiCost;

	[SerializeField]
	private Button button;

	[SerializeField]
	private Transform mainHeaderDWHolder;

	[SerializeField]
	private Transform dwChildHolder;

	[SerializeField]
	private GameObject dwWaterTournamentPrefab;

	private Skill coreDeepWaterSkill;

	private List<Transform> deepWaterIconsTransforms = new List<Transform>();

	private bool isAvaliable;

	private UIDeepWaterItem mainHeaderDWIcon;
}
