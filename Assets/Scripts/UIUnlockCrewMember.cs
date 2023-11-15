using System;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUnlockCrewMember : UIListItem<UIUnlockCrewMember.UnlockCrewMemberContent>
{
	protected override void Awake()
	{
		base.Awake();
		for (int i = 0; i < this.imageHolder.Length; i++)
		{
			this.colorHolder.Add(this.imageHolder[i].color);
		}
	}

	public bool IsUnlockCrewmemberAvailable
	{
		get
		{
			return this.unlockCrewMemberSkill.IsAvailableForLevelUp && SkillManager.Instance.GetCrewMembersAvailableForPurchase().Count > 0 && !TournamentManager.Instance.IsInsideTournament;
		}
	}

	private new void Start()
	{
		ResourceManager.Instance.OnResourceChanged += this.ResourceManager_OnResourceChanged;
		SkillManager.Instance.OnSkillLevelChanged += this.Instance_OnSkillLevelChanged;
		this.UpdateCostLabel();
	}

	private void Instance_OnSkillLevelChanged(Skill skill, LevelChange levelChange)
	{
		if (skill.GetExtraInfo().IsCrew && skill.CurrentLevel == 1 && levelChange == LevelChange.LevelUpFree && base.gameObject.activeInHierarchy)
		{
			this.UpdateCostLabel();
		}
	}

	private void ResourceManager_OnResourceChanged(ResourceType type, BigInteger change, BigInteger total)
	{
		if (base.gameObject.activeInHierarchy)
		{
			this.UpdateCostLabel();
		}
	}

	private void UpdateCostLabel()
	{
		this.costLabel.SetVariableText(new string[]
		{
			this.unlockCrewMemberSkill.CostForNextLevelUp.ToString()
		});
		this.unlockButton.interactable = (this.unlockCrewMemberSkill.IsAvailableForLevelUp && SkillManager.Instance.GetCrewMembersAvailableForPurchase().Count > 0 && !TournamentManager.Instance.IsInsideTournament);
		if (!this.unlockButton.interactable)
		{
			this.SetUnavailableLook();
		}
		else
		{
			this.SetAvailableLook();
		}
	}

	private void SetUnavailableLook()
	{
		for (int i = 0; i < this.imageHolder.Length; i++)
		{
			this.imageHolder[i].color = new Color(this.imageHolder[i].color.r, this.imageHolder[i].color.r, this.imageHolder[i].color.r);
		}
		this.unlockLabel.color = new Color(0.7f, 0.7f, 0.7f);
	}

	private void SetAvailableLook()
	{
		for (int i = 0; i < this.imageHolder.Length; i++)
		{
			this.imageHolder[i].color = this.colorHolder[i];
		}
		this.unlockLabel.color = new Color(0.718f, 0.396f, 0.729f);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		this.UpdateCostLabel();
	}

	public void UnlockRandomCrewMember()
	{
	}

	public override void OnUpdateUI(UIUnlockCrewMember.UnlockCrewMemberContent content)
	{
	}

	public override void OnShouldRegisterListeners()
	{
	}

	public override void OnShouldUnregisterListeners()
	{
	}

	[SerializeField]
	private TextMeshProUGUI costLabel;

	private bool canAffordCrewMember = true;

	[SerializeField]
	private Button unlockButton;

	[SerializeField]
	private Skill unlockCrewMemberSkill;

	public TextMeshProUGUI unlockLabel;

	public Image[] imageHolder;

	private List<Color> colorHolder = new List<Color>();

	[SerializeField]
	private UnlockCrewEffect unlockEffect;

	public class UnlockCrewMemberContent : IListItemContent
	{
		public UnlockCrewMemberContent(UIListItem prefab)
		{
			this.prefab = prefab;
		}

		public UIListItem GetPrefab()
		{
			return this.prefab;
		}

		private UIListItem prefab;
	}
}
