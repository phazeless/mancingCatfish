using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
	public static SkillTreeManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<int> OnCrownLevelIncreased;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<int> OnSkillTreeReset;

	public Skill CrownLevelSkill
	{
		get
		{
			return this.crownLevelSkill;
		}
	}

	public List<Skill> CrownRewardSkills
	{
		get
		{
			return this.crownRewardSkills;
		}
	}

	public List<Skill> SkillTreeSkills
	{
		get
		{
			return this.skillTreeSkills;
		}
	}

	public int ResetSkillsCost
	{
		get
		{
			return this.resetSkillsCost;
		}
	}

	public bool IsSkillTreeEnabled
	{
		get
		{
			return false;
		}
	}

	private void Awake()
	{
		SkillTreeManager.Instance = this;
		this.ApplyABLogics();
		if (this.crownExpDialog != null)
		{
			this.crownExpDialog.SetActive(this.IsSkillTreeEnabled);
		}
	}

	private void Start()
	{
		ResourceManager.Instance.OnResourceChanged += this.Instance_OnResourceChanged;
		SkillManager.Instance.OnSkillManagerInitialized += this.Instance_OnSkillManagerInitialized;
	}

	private void ApplyABLogics()
	{
		
	}

	private void Instance_OnSkillManagerInitialized(SkillManager obj)
	{
		this.CheckIfCrownRewardSkillShouldLevelUp();
		this.crownSkillLimitManager.SetAndUpdateLimits(this.crownLevelSkill, this.crownRewardSkills);
	}

	private void Instance_OnResourceChanged(ResourceType res, BigInteger newAmount, BigInteger oldAmount)
	{
		if (res == ResourceType.CrownExp && this.crownLevelSkill.TryLevelUp())
		{
			int amount = 1;
			ResourceChangeData gemChangeData = new ResourceChangeData("contentId_skillPointIncreased", null, amount, ResourceType.SkillPoints, ResourceChangeType.Earn, ResourceChangeReason.CrownLevelIncreased);
			ResourceManager.Instance.GiveSkillPoints(1, gemChangeData);
			this.CheckIfCrownRewardSkillShouldLevelUp();
			this.crownSkillLimitManager.SetAndUpdateLimits(this.crownLevelSkill, this.crownRewardSkills);
			if (this.OnCrownLevelIncreased != null)
			{
				this.OnCrownLevelIncreased(this.crownLevelSkill.CurrentLevel);
			}
		}
	}

	private void CheckIfCrownRewardSkillShouldLevelUp()
	{
		for (int i = 0; i < this.crownRewardSkills.Count; i++)
		{
			Skill skill = this.crownRewardSkills[i];
			if (skill.CurrentLevel == 0 && this.crownLevelSkill.CurrentLevel >= skill.MaxLevel)
			{
				skill.TryLevelUp();
			}
		}
	}

	public void AddUIConnection(Skill skill, Transform trans)
	{
		this.uiSkillTreeObjects.Add(skill, trans);
	}

	public Transform GetUIObjectRelatedTo(Skill skill)
	{
		if (skill != null && this.uiSkillTreeObjects.ContainsKey(skill))
		{
			return this.uiSkillTreeObjects[skill];
		}
		return null;
	}

	public void ResetSkills()
	{
		List<Skill> list = this.skillTreeSkills;
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			Skill skill = list[i];
			for (int j = 0; j < skill.CurrentLevel; j++)
			{
				num += (int)skill.GetCostForLevelUp(j);
			}
			SkillManager.Instance.RefreshCachedSkillValuesOfSkill(skill, true, true);
			skill.SetCurrentLevel(0, LevelChange.SkillTreeReset);
		}
		ResourceChangeData gemChangeData = new ResourceChangeData("contentId_skillPointIncreased", null, num, ResourceType.SkillPoints, ResourceChangeType.Earn, ResourceChangeReason.PurchaseSkillTreeReset);
		ResourceManager.Instance.GiveSkillPoints(num, gemChangeData);
		UnityEngine.Debug.Log("Total skill points spent: " + num);
		if (this.OnSkillTreeReset != null)
		{
			this.OnSkillTreeReset(num);
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.U))
		{
			int amount = 32;
			ResourceChangeData gemChangeData = new ResourceChangeData("contentId_skillPointIncreased", null, amount, ResourceType.CrownExp, ResourceChangeType.Earn, ResourceChangeReason.ForTesting);
			ResourceManager.Instance.GiveCrownExp(amount, gemChangeData);
		}
		else if (UnityEngine.Input.GetKeyUp(KeyCode.R))
		{
			this.ResetSkills();
		}
	}

	private void SetupABTestSettingsFor_A()
	{
		
		
	}

	private void SetupABTestSettingsFor_B()
	{
		
		
	}

	private const string contentId_skillPointAddedId = "contentId_skillPointIncreased";

	private const string contentName_crownLevelIncreased = "Crown Level Increased";

	[SerializeField]
	private bool simulateTestGroup_A;

	[SerializeField]
	private bool simulateTestGroup_B;

	[SerializeField]
	private GameObject crownExpDialog;

	[SerializeField]
	private int resetSkillsCost = 100;

	[SerializeField]
	private Skill crownLevelSkill;

	[SerializeField]
	private List<Skill> crownRewardSkills = new List<Skill>();

	[SerializeField]
	private List<Skill> skillTreeSkills = new List<Skill>();

	[SerializeField]
	private CrownSkillLimitManager crownSkillLimitManager;

	private Dictionary<Skill, Transform> uiSkillTreeObjects = new Dictionary<Skill, Transform>();
}
