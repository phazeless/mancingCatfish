using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using UnityEngine;

public class EasterManager : MonoBehaviour
{
	public static EasterManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<List<BaseEasterEgg>> OnAllEggsFound;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BaseEasterEgg> OnEggFound;

	public int MaxEggsCount
	{
		get
		{
			return this.easterEggsPrefabs.Count;
		}
	}

	public bool HasFoundAllEggs
	{
		get
		{
			return this.EggsFoundCount >= this.MaxEggsCount;
		}
	}

	public int EggsFoundCount
	{
		get
		{
			return this.eggsFoundIds.Count;
		}
	}

	public int SecondsLeftOnEvent
	{
		get
		{
			return Mathf.Max(0, (int)(this.availability.GetExpireDate().ToUniversalTime() - TimeManager.Instance.RealNow).TotalSeconds);
		}
	}

	public EasterBasketRewardContent BasketContent
	{
		get
		{
			return this.basketContent;
		}
	}

	public int CostToUnlockRemaining
	{
		get
		{
			if (this.EggsFoundCount <= 0)
			{
				return 1000;
			}
			if (this.EggsFoundCount == 1)
			{
				return 800;
			}
			if (this.EggsFoundCount == 2)
			{
				return 600;
			}
			if (this.EggsFoundCount == 3)
			{
				return 400;
			}
			if (this.EggsFoundCount == 4)
			{
				return 200;
			}
			return 100;
		}
	}

	public bool HasUnlockedBigReward
	{
		get
		{
			return this.hasUnlockedBigReward;
		}
	}

	public BaseEasterEgg GetEggPrefab(int eggIndex)
	{
		if (eggIndex < this.easterEggsPrefabs.Count)
		{
			return this.easterEggsPrefabs[eggIndex];
		}
		return null;
	}

	private bool IsNewDay
	{
		get
		{
			return this.currentDay != TimeManager.Instance.RealNow.Day;
		}
	}

	private bool ShouldEventBeActive
	{
		get
		{
			return this.IsEasterPeriod() && (!this.HasUnlockedBigReward || !this.HasFoundAllEggs);
		}
	}

	private void Awake()
	{
		EasterManager.Instance = this;
		this.OnEggFound += this.InternalOnEggFound;
		this.OnAllEggsFound += this.InternalOnAllEggsFound;
		this.Load();
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.E))
		{
			TimeManager.Instance.UpdateRealNow(delegate(DateTime time)
			{
				this.TryStartEggEvent();
			});
		}
	}

	private void InternalOnEggFound(BaseEasterEgg egg)
	{
		EasterEggOpening easterEggOpening = UnityEngine.Object.Instantiate<EasterEggOpening>(this.eggOpeningDialog, this.canvasTarget);
		easterEggOpening.Init(egg, delegate(BaseEasterEgg activeEgg)
		{
			this.GrantEggContent(activeEgg);
			this.CheckForEndOfEvent();
		});
	}

	private void InternalOnAllEggsFound(List<BaseEasterEgg> egg)
	{
	}

	private void Start()
	{
		bool flag = this.easterEggsPrefabs.Count == this.easterEggsLocations.Count;
		if (flag)
		{
			if (TimeManager.Instance.IsInitializedWithInternetTime)
			{
				this.TryStartEggEvent();
			}
			else
			{
				TimeManager.Instance.OnInitializedWithInternetTime += this.Instance_OnInitializedWithInternetTime;
			}
		}
		else
		{
			UnityEngine.Debug.LogError("The amount of EGGS vs LOCATIONS is different. Make sure they are the same!");
		}
	}

	private void Instance_OnInitializedWithInternetTime(bool success, DateTime realTime)
	{
		if (success)
		{
			this.TryStartEggEvent();
		}
	}

	private bool IsEasterPeriod()
	{
		if (TimeManager.Instance.IsLocalTimeWithinReasonableDiffFromRealTime())
		{
			DateTime realNow = TimeManager.Instance.RealNow;
			return realNow > this.availability.GetAvailableDate().ToUniversalTime() && realNow < this.availability.GetExpireDate().ToUniversalTime();
		}
		return false;
	}

	private void TryStartEggEvent()
	{
		if (this.ShouldEventBeActive)
		{
			UnityEngine.Debug.Log("It is now EASTER!!!");
			if (this.IsNewDay)
			{
				this.dayCounter++;
				this.currentDay = TimeManager.Instance.RealNow.Day;
				this.Save();
			}
			for (int i = 0; i < Mathf.Min(this.dayCounter, this.easterEggsPrefabs.Count); i++)
			{
				if (!this.HasFoundEgg(i) && !this.HasCreatedEgg(i))
				{
					BaseEasterEgg baseEasterEgg = UnityEngine.Object.Instantiate<BaseEasterEgg>(this.easterEggsPrefabs[i], this.easterEggsLocations[i]);
					baseEasterEgg.InitEgg(i);
					this.instantiatedEggsIds.Add(i);
				}
			}
			if (this.easterEventIgn == null)
			{
				this.easterEventIgn = InGameNotificationManager.Instance.Create<IGNEasterEvent>(new IGNEasterEvent());
			}
		}
	}

	private void CheckForEndOfEvent()
	{
		if (!this.ShouldEventBeActive && this.easterEventIgn != null)
		{
			this.easterEventIgn.SetExpiration(0f);
		}
	}

	private int CalculateDailyEggIndex()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.easterEggsPrefabs.Count; i++)
		{
			if (!this.eggsFoundIds.Contains(i))
			{
				list.Add(i);
			}
		}
		int num = UnityEngine.Random.Range(0, list.Count);
		if (num < list.Count)
		{
			return list[num];
		}
		return -1;
	}

	public List<int> GetEggsFoundIndexes()
	{
		return this.eggsFoundIds;
	}

	public bool UnlockBigReward()
	{
		if (!this.hasUnlockedBigReward)
		{
			string contentId = "contentId_easterBasket";
			ResourceChangeData changeData = new ResourceChangeData(contentId, ResourceChangeReason.UnlockEasterBasket.ToString(), this.CostToUnlockRemaining, ResourceType.Gems, ResourceChangeType.Spend, ResourceChangeReason.UnlockEasterBasket);
			if (ResourceManager.Instance.TakeGems(this.CostToUnlockRemaining, changeData))
			{
				PurchaseCrewMemberHandler.Instance.GetCrewMember(this.basketContent.Crew, ResourceChangeReason.UnlockEasterBasket, 0);
				this.basketContent.Item.ChangeItemAmount(1, ResourceChangeReason.UnlockEasterBasket);
				ResourceChangeData gemChangeData = new ResourceChangeData(contentId, ResourceChangeReason.UnlockEasterBasket.ToString(), this.basketContent.GemAmount, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.UnlockEasterBasket);
				ResourceManager.Instance.GiveGems(this.basketContent.GemAmount, gemChangeData);
				this.hasUnlockedBigReward = true;
				this.CheckForEndOfEvent();
				this.Save();
				return true;
			}
		}
		return false;
	}

	public void MarkAsFound(BaseEasterEgg egg)
	{
		if (egg != null && egg.Id >= 0)
		{
			this.eggsFoundIds.Add(egg.Id);
			if (this.OnEggFound != null)
			{
				this.OnEggFound(egg);
			}
			if (this.HasFoundAllEggs && this.OnAllEggsFound != null)
			{
				this.OnAllEggsFound(this.easterEggsPrefabs);
			}
			this.Save();
		}
	}

	private bool HasFoundEgg(int id)
	{
		return this.eggsFoundIds.Contains(id);
	}

	private bool HasCreatedEgg(int id)
	{
		return this.instantiatedEggsIds.Contains(id);
	}

	private void GrantEggContent(BaseEasterEgg egg)
	{
		if (egg.Content.ContainsGems)
		{
			ResourceChangeData gemChangeData = new ResourceChangeData("egg_" + egg.Id, "Found Easter Egg", egg.Content.GemAmount, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.FoundEasterEgg);
			ResourceManager.Instance.GiveGems(egg.Content.GemAmount, gemChangeData);
		}
		else if (egg.Content.ContainsItems)
		{
			for (int i = 0; i < egg.Content.Items.Count; i++)
			{
				egg.Content.Items[i].ChangeItemAmount(1, ResourceChangeReason.FoundEasterEgg);
			}
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			this.Save();
		}
	}

	private void Save()
	{
		EncryptedPlayerPrefs.SetInt("KEY_EASTER_CURRENT_DAY", this.currentDay, true);
		EncryptedPlayerPrefs.SetInt("KEY_EASTER_DAY_COUNTER", this.dayCounter, true);
		EncryptedPlayerPrefs.SetInt("KEY_EASTER_HAS_UNLOCKED_BIG_REWARD", (!this.hasUnlockedBigReward) ? 0 : 1, true);
		EncryptedPlayerPrefs.SetString("KEY_EASTER_EGGS_FOUND_IDS", JsonConvert.SerializeObject(this.eggsFoundIds), true);
	}

	private void Load()
	{
		this.currentDay = EncryptedPlayerPrefs.GetInt("KEY_EASTER_CURRENT_DAY", this.currentDay);
		this.dayCounter = EncryptedPlayerPrefs.GetInt("KEY_EASTER_DAY_COUNTER", this.dayCounter);
		this.hasUnlockedBigReward = (EncryptedPlayerPrefs.GetInt("KEY_EASTER_HAS_UNLOCKED_BIG_REWARD", (!this.hasUnlockedBigReward) ? 0 : 1) == 1);
		string @string = EncryptedPlayerPrefs.GetString("KEY_EASTER_EGGS_FOUND_IDS", null);
		if (!string.IsNullOrEmpty(@string))
		{
			this.eggsFoundIds = JsonConvert.DeserializeObject<List<int>>(@string);
		}
	}

	private void OnDestroy()
	{
		TimeManager.Instance.OnInitializedWithInternetTime -= this.Instance_OnInitializedWithInternetTime;
		this.OnEggFound -= this.InternalOnEggFound;
		this.OnAllEggsFound -= this.InternalOnAllEggsFound;
	}

	private const string KEY_EASTER_EGGS_FOUND_IDS = "KEY_EASTER_EGGS_FOUND_IDS";

	private const string KEY_EASTER_CURRENT_DAY = "KEY_EASTER_CURRENT_DAY";

	private const string KEY_EASTER_DAY_COUNTER = "KEY_EASTER_DAY_COUNTER";

	private const string KEY_EASTER_HAS_UNLOCKED_BIG_REWARD = "KEY_EASTER_HAS_UNLOCKED_BIG_REWARD";

	[SerializeField]
	private HolidayOfferAvailability availability;

	[SerializeField]
	private List<BaseEasterEgg> easterEggsPrefabs = new List<BaseEasterEgg>();

	[SerializeField]
	private List<Transform> easterEggsLocations = new List<Transform>();

	[SerializeField]
	private Skill crewForFindingAllEggs;

	[SerializeField]
	private EasterEggOpening eggOpeningDialog;

	[SerializeField]
	private Transform canvasTarget;

	[SerializeField]
	private EasterBasketRewardContent basketContent;

	private List<int> instantiatedEggsIds = new List<int>();

	private List<int> eggsFoundIds = new List<int>();

	private int currentDay = -1;

	private int dayCounter;

	private bool hasUnlockedBigReward;

	private InGameNotification easterEventIgn;
}
