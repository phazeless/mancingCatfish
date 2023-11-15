using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using ACE.IAPS;
using UnityEngine;

public class PurchaseCrewMemberHandler : MonoBehaviour
{
	public static PurchaseCrewMemberHandler Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill, ResourceChangeReason, int> OnCrewUnlocked;

	private void Awake()
	{
		PurchaseCrewMemberHandler.Instance = this;
	}

	private void Start()
	{
		StoreManager storeManager = ResourceManager.StoreManager;
		storeManager.OnGoodBalanceChanged = (Action<string, int, int>)Delegate.Combine(storeManager.OnGoodBalanceChanged, new Action<string, int, int>(this.Instance_OnGoodBalanceChanged));
	}

	private void Instance_OnGoodBalanceChanged(string itemId, int balance, int amountAdded)
	{
		if ("se.ace.unlock_crew_member" == itemId && amountAdded == 1)
		{
			long prestigeSkillLvl = SkillManager.Instance.PrestigeSkill.CurrentLevelAsLong;
			int deepWaterLvl = SkillManager.Instance.DeepWaterSkill.HighestLevel;
			List<Skill> list = (from x in SkillManager.Instance.CrewMembers
			where x.CurrentLevel == 0 && prestigeSkillLvl >= (long)x.GetExtraInfo().RequiredFishingExperience && deepWaterLvl >= x.GetExtraInfo().RequiredDeepWaterLevel
			select x).ToList<Skill>();
			if (list.Count > 0 || PurchaseCrewMemberHandler.OverrideRandomWithCrewMember != null)
			{
				Skill skill;
				if (PurchaseCrewMemberHandler.OverrideRandomWithCrewMember == null)
				{
					skill = list[UnityEngine.Random.Range(0, list.Count)];
				}
				else
				{
					skill = PurchaseCrewMemberHandler.OverrideRandomWithCrewMember;
					PurchaseCrewMemberHandler.OverrideRandomWithCrewMember = null;
				}
				if (skill != null)
				{
					BigInteger costForNextLevelUp = this.unlockCrewMemberSkill.CostForNextLevelUp;
					if (this.unlockCrewMemberSkill.TryLevelUp())
					{
						this.GetCrewMember(skill, ResourceChangeReason.UnlockRandomCrew, (int)costForNextLevelUp);
					}
				}
				else
				{
					UnityEngine.Debug.LogWarning("There's no more crew members to unlock!");
				}
			}
		}
	}

	public void GetCrewMember(Skill crewMember, ResourceChangeReason source, int gemsSpent = 0)
	{
		if (this.queuedUnlocks.Count == 0)
		{
			this.CreateUnlockEffect(crewMember, source, gemsSpent);
		}
		else
		{
			this.queuedUnlocks.Enqueue(delegate
			{
				this.CreateUnlockEffect(crewMember, source, gemsSpent);
			});
		}
	}

	private void CreateUnlockEffect(Skill crewMember, ResourceChangeReason source, int gemsSpent)
	{
		UnlockCrewEffect unlockCrewEffect = UnityEngine.Object.Instantiate<UnlockCrewEffect>(this.unlockEffect, this.dialogCanvas, false);
		unlockCrewEffect.UnlockEffect(crewMember, delegate
		{
			if (this.queuedUnlocks.Count > 0)
			{
				this.queuedUnlocks.Dequeue()();
			}
		});
		crewMember.SetHasNotifiedSkillUnlocked(true);
		crewMember.LevelUpForFree();
		SkillManager.Instance.CanSkillBeLeveledUp(crewMember, true);
		if (this.OnCrewUnlocked != null)
		{
			this.OnCrewUnlocked(crewMember, source, gemsSpent);
		}
	}

	public static Skill OverrideRandomWithCrewMember;

	[SerializeField]
	private Skill unlockCrewMemberSkill;

	[SerializeField]
	private UnlockCrewEffect unlockEffect;

	[SerializeField]
	private Transform dialogCanvas;

	private Queue<Action> queuedUnlocks = new Queue<Action>();
}
