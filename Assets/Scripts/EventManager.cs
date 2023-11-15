using System;
using System.Collections.Generic;
using System.Linq;
using FullInspector;
using UnityEngine;

public class EventManager : BaseBehavior
{
	protected override void Awake()
	{
		base.Awake();
		List<FishBehaviour> fishPrefabs = FishPoolManager.Instance.FishPrefabs;
		foreach (FishBehaviour fishBehaviour in fishPrefabs)
		{
			int rarity = fishBehaviour.FishInfo.Rarity;
			if (rarity != 0)
			{
				for (int i = 0; i < rarity; i++)
				{
					this.rareSidesList.Add(rarity);
				}
				if (!this.fishesByRarity.ContainsKey(rarity))
				{
					this.fishesByRarity.Add(rarity, new List<FishBehaviour>());
				}
				this.fishesByRarity[rarity].Add(fishBehaviour);
			}
		}
		this.hasShownFacebookIGN = (EncryptedPlayerPrefs.GetInt(EventManager.KEY_HAS_SHOWN_FACEBOOK_IGN, 0) != 0);
		this.lastClaimedChristmasPresentDay = EncryptedPlayerPrefs.GetInt(EventManager.KEY_CLAIMED_CHRISTMAS_PRESENT_DAY, this.lastClaimedChristmasPresentDay);
	}

	private void Start()
	{
		if (SkillManager.Instance.TierSkill.CurrentLevel == 0)
		{
			SkillManager.Instance.TierSkill.SetCurrentLevel(1, LevelChange.LevelUp);
		}
		this.questFirstAvailable.OnQuestClaimed += this.QuestFirstAvailable_OnQuestClaimed;
		this.crewCatchLobster.OnSkillActivation += this.CrewCatchLobster_OnSkillActivation;
		this.shoalSkill.TupleOne.OnSkillActivation += this.ShoalSkill_OnSkillActivation;
		this.autoFishSkill.TupleOne.OnSkillActivation += this.AutoFishSkill_OnSkillActivation;
		SkillManager.Instance.DeepWaterSkill.OnSkillLevelUp += this.DeepWaterSkill_OnSkillLevelUp1;
		SkillManager.Instance.DeepWaterSkill.OnSkillLevelUp += this.DeepWaterSkill_OnSkillLevelUp;
		this.DeepWaterSkill_OnSkillLevelUp(SkillManager.Instance.DeepWaterSkill, LevelChange.Initialization);
		if (this.shoalSkill.TupleThree == null)
		{
			UnityEngine.Debug.LogWarning("Missing TupleThree reference in inspector. (Not there because it didn't get commited properly!");
		}
		this.fishValueBonusNotification = (IGNFishValueBonus)InGameNotificationManager.Instance.GetActiveNotifications(InGameNotification.IGN.FishValueBonus).FirstOrDefault<InGameNotification>();
		if (this.fishValueBonusNotification != null)
		{
			this.fishValueBonusNotification.Refresh(SkillManager.Instance.FishValueBonusSkill);
		}
		if (CharacterConversationHandler.Instance != null)
		{
			CharacterConversationHandler.Instance.OnConversationCompleted += this.Instance_OnConversationCompleted;
		}
	}

	private void Instance_OnConversationCompleted(int tutorialIndex)
	{
		if (tutorialIndex == 2)
		{
			DialogInteractionHandler.Instance.DisableCloseByClickingShade = true;
			InGameNotificationDialog inGameNotificationDialog = InGameNotificationManager.Instance.OpenFirstOccurrenceOfIGN<IGNNewCrew>();
			this.RunAfterDelay(0.5f, delegate()
			{
				DialogInteractionHandler.Instance.DisableCloseByClickingShade = false;
			});
			if (inGameNotificationDialog != null)
			{
				InGameNotification inGameNotification = inGameNotificationDialog.GetInGameNotification();
				if (inGameNotification == null)
				{
					inGameNotification.SetExpiration(3600f);
				}
			}
		}
	}

	private void DeepWaterSkill_OnSkillLevelUp1(Skill skill, LevelChange levelChange)
	{
		if ((levelChange == LevelChange.LevelUp || levelChange == LevelChange.LevelUpFree) && skill.CurrentLevel == 1 && !this.hasShownFacebookIGN)
		{
			IGNConnectFacebook ign = new IGNConnectFacebook();
			InGameNotificationManager.Instance.Create<IGNConnectFacebook>(ign);
			EncryptedPlayerPrefs.SetInt(EventManager.KEY_HAS_SHOWN_FACEBOOK_IGN, 1, true);
			this.hasShownFacebookIGN = true;
		}
	}

	private void QuestFirstAvailable_OnQuestClaimed(Quest obj)
	{
		this.crewFirstAvailable.IsUnlocked = true;
	}

	private void CrewCatchLobster_OnSkillActivation(Skill obj)
	{
		IGNLobster ignlobster = new IGNLobster();
		ignlobster.RanomizeContentInCage();
		InGameNotificationManager.Instance.Create<IGNLobster>(ignlobster);
	}

	private void DeepWaterSkill_OnSkillLevelUp(Skill skill, LevelChange levelChange)
	{
		Color color = ((Skills.DeepWaterTier)skill.GetExtraInfo().SkillSpecifics).WaterColors[skill.CurrentLevel];
		if (skill.CurrentLevel == 0)
		{
			ColorChangerHandler.SetColor(color);
		}
		else
		{
			ColorChangerHandler.TweenToColor(color, 3f);
		}
	}

	private void AutoFishSkill_OnSkillActivation(Skill obj)
	{
		foreach (CircleCatcher circleCatcher in this.autoFishSkill.TupleTwo)
		{
			circleCatcher.gameObject.SetActive(true);
		}
		this.isAutoCatchersActive = true;
	}

	private void ShoalSkill_OnSkillActivation(Skill skill)
	{
		this.CreateStimSpawner().Activate((int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.ShoalPowerUp>());
	}

	private void Update()
	{
		if (FHelper.HasSecondsPassed(1800f, ref this.saveToCloudTimer, true) && !TournamentManager.Instance.IsInsideTournament && CloudOnceManager.Instance != null && !CloudOnceManagerHelper.IsLoginInProgress)
		{
			CloudOnceManager.Instance.SaveDataToCache();
			CloudOnceManager.Instance.SaveDataToCloud();
		}
		if (FHelper.HasSecondsPassed(420f, ref this.saveToDiskTimer, true) && !TournamentManager.Instance.IsInsideTournament)
		{
			EncryptedPlayerPrefs.Save();
		}
		if (this.isAutoCatchersActive && FHelper.HasSecondsPassed(SkillManager.Instance.GetCurrentTotalValueFor<Skills.AutoFishPowerUp>(), ref this.autoCatcherTimer, true))
		{
			this.isAutoCatchersActive = false;
			foreach (CircleCatcher circleCatcher in this.autoFishSkill.TupleTwo)
			{
				circleCatcher.gameObject.SetActive(false);
			}
		}
		if (FHelper.HasSecondsPassed(SkillManager.Instance.GetCurrentTotalValueFor<Skills.ShoalAfterSeconds>(), ref this.spawnShoalTimer, true) && FHelper.DidRollWithChance(SkillManager.Instance.GetCurrentTotalValueFor<Skills.ShoalSpawnChance>()))
		{
			this.CreateStimSpawner().Activate((int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.ShoalAmount>());
		}
	}

	public StimSpawner CreateStimSpawner()
	{
		StimSpawner stimSpawner = UnityEngine.Object.Instantiate<StimSpawner>(this.shoalSkill.TupleTwo);
		stimSpawner.Init(this.shoalSkill.TupleThree);
		return stimSpawner;
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			EncryptedPlayerPrefs.SetInt(EventManager.KEY_CLAIMED_CHRISTMAS_PRESENT_DAY, this.lastClaimedChristmasPresentDay, true);
		}
	}

	private static readonly string KEY_HAS_SHOWN_FACEBOOK_IGN = "KEY_HAS_SHOWN_FACEBOOK_IGN";

	private static readonly string KEY_CLAIMED_CHRISTMAS_PRESENT_DAY = "KEY_CLAIMED_CHRISTMAS_PRESENT_DAY";

	private const float SECONDS_UNTIL_AUTO_SAVE_TO_DISK = 420f;

	private const float SECONDS_UNTIL_AUTO_SAVE_TO_CLOUD = 1800f;

	private float saveToCloudTimer;

	private float saveToDiskTimer;

	private float autoCatcherTimer;

	private bool isAutoCatchersActive;

	private float spawnShoalTimer;

	private float pinkySpawnTimer;

	[SerializeField]
	public EventManager.Tuple<Skill, StimSpawner, Transform> shoalSkill;

	[SerializeField]
	public EventManager.Tuple<Skill, List<CircleCatcher>> autoFishSkill;

	[SerializeField]
	private Skill crewCatchLobster;

	[SerializeField]
	private Skill crewFirstAvailable;

	[SerializeField]
	private Quest questFirstAvailable;

	private List<int> rareSidesList = new List<int>();

	private Dictionary<int, List<FishBehaviour>> fishesByRarity = new Dictionary<int, List<FishBehaviour>>();

	private IGNFishValueBonus fishValueBonusNotification;

	private bool hasShownFacebookIGN;

	private int lastClaimedChristmasPresentDay;

	[Serializable]
	public class Tuple<T, K>
	{
		public T TupleOne = default(T);

		public K TupleTwo = default(K);
	}

	[Serializable]
	public class Tuple<T, K, U>
	{
		public T TupleOne = default(T);

		public K TupleTwo = default(K);

		public U TupleThree = default(U);
	}
}
