using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class FishBook : MonoBehaviour
{
	public static FishBook Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<FishAttributes> OnFishBookChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<FishAttributes> OnNewFishCaught;

	public List<FishAttributes> Fishes
	{
		get
		{
			return this.allFishes.List;
		}
	}

	public List<FishAttributes> TierFishes
	{
		get
		{
			return this.tierFishes.List;
		}
	}

	public List<FishAttributes> BossFishes
	{
		get
		{
			return this.bossFishes.List;
		}
	}

	public List<FishAttributes> SpecialFishes
	{
		get
		{
			return this.specialFishes.List;
		}
	}

	private void Awake()
	{
		FishBook.Instance = this;
		AFKManager.Instance.OnUserLeaveCallback += this.Instance_OnUserLeaveCallback;
		AFKManager.Instance.OnUserReturnCallback += this.Instance_OnUserReturnCallback;
		BaseCatcher.OnFishCollected += this.BaseCatcher_OnFishCollected;
		this.starCollectorSkill = SkillManager.Instance.CollectStarsSkill;
		foreach (FishBehaviour fishBehaviour in FishPoolManager.Instance.TierFishes)
		{
			this.tierFishes.List.Add(fishBehaviour.FishInfo.ShallowCopy());
		}
		foreach (FishBehaviour fishBehaviour2 in FishPoolManager.Instance.BossFishes)
		{
			this.bossFishes.List.Add(fishBehaviour2.FishInfo.ShallowCopy());
		}
		foreach (FishBehaviour fishBehaviour3 in FishPoolManager.Instance.SpecialFishes)
		{
			this.specialFishes.List.Add(fishBehaviour3.FishInfo.ShallowCopy());
		}
		this.allFishes.List.AddRange(this.tierFishes.List);
		this.allFishes.List.AddRange(this.bossFishes.List);
		this.allFishes.List.AddRange(this.specialFishes.List);
	}

	private void Start()
	{
		foreach (FishAttributes obj in this.allFishes.List)
		{
			if (this.OnFishBookChanged != null)
			{
				this.OnFishBookChanged(obj);
			}
		}
		LionAnalytics.TrackStarsAmountReached(this.starCollectorSkill.CurrentLevel);
	}

	public void TryAddToBook(FishBehaviour fish)
	{
		if (TournamentManager.Instance.IsInsideTournament)
		{
			return;
		}
		FishAttributes fishInfo = this.GetFishInfo(fish);
		bool flag = false;
		bool flag2 = false;
		this.IsBiggerThanExisting(fish, ref flag, ref flag2);
		if (flag || flag2)
		{
			fishInfo.BiggestCatch = fish.ActualWeight;
			fishInfo.IsCaught = true;
			if (flag)
			{
				IGNNewFish ignnewFish = new IGNNewFish();
				ignnewFish.FishInfo = fishInfo;
				InGameNotificationManager.Instance.Create<IGNNewFish>(ignnewFish);
				for (int i = 0; i < fishInfo.Stars; i++)
				{
					this.starCollectorSkill.TryLevelUp();
				}
				LionAnalytics.TrackStarsAmountReached(this.starCollectorSkill.CurrentLevel);
				HookedVibration.NewFishHaptic();
				if (this.OnNewFishCaught != null)
				{
					this.OnNewFishCaught(fishInfo);
				}
			}
			if (this.OnFishBookChanged != null)
			{
				this.OnFishBookChanged(fishInfo);
			}
		}
	}

	public bool HasCaught(FishBehaviour fish)
	{
		return this.GetFishInfo(fish).IsCaught;
	}

	public void IsBiggerThanExisting(FishBehaviour fish, ref bool isNewFish, ref bool isBiggerThanPrevious)
	{
		FishAttributes fishInfo = this.GetFishInfo(fish);
		isNewFish = (fishInfo == null || !fishInfo.IsCaught);
		if (!isNewFish)
		{
			isBiggerThanPrevious = (fish.ActualWeight > fishInfo.BiggestCatch);
		}
	}

	public FishAttributes GetFishInfo(FishBehaviour fish)
	{
		foreach (FishAttributes fishAttributes in this.allFishes.List)
		{
			if (fishAttributes.FishType == fish.FishInfo.FishType)
			{
				return fishAttributes;
			}
		}
		return null;
	}

	private void BaseCatcher_OnFishCollected(FishBehaviour fish)
	{
		this.TryAddToBook(fish);
	}

	private void Instance_OnUserReturnCallback(bool fromAppRestart, DateTime now, float afkTimeInSeconds)
	{
		if (!fromAppRestart)
		{
			return;
		}
		string @string = EncryptedPlayerPrefs.GetString("FishBook", "{}");
		FishBook.FishList fishList = JsonUtility.FromJson<FishBook.FishList>(@string);
		int num = 0;
		foreach (FishAttributes fishAttributes in fishList.List)
		{
			foreach (FishAttributes fishAttributes2 in this.allFishes.List)
			{
				if (fishAttributes.FishType == fishAttributes2.FishType)
				{
					fishAttributes2.BiggestCatch = fishAttributes.BiggestCatch;
					fishAttributes2.IsCaught = true;
					num += fishAttributes2.Stars;
				}
			}
		}
		if (this.starCollectorSkill.CurrentLevel != num)
		{
			UnityEngine.Debug.LogWarning("StarCollectorSkills current level is different from the total amount of stars collected. Did a Fish change it's Star-value?");
			this.starCollectorSkill.SetCurrentLevel(num, LevelChange.Initialization);
		}
	}

	private void Instance_OnUserLeaveCallback(DateTime now, bool fromApplicationQuit)
	{
		if (fromApplicationQuit || TournamentManager.Instance.IsInsideTournament)
		{
			return;
		}
		this.SaveFishBook();
	}

	public void SaveFishBook()
	{
		string value = JsonUtility.ToJson(new FishBook.FishList(this.allFishes.List.FindAll((FishAttributes x) => x.IsCaught)));
		EncryptedPlayerPrefs.SetString("FishBook", value, true);
	}

	private FishBook.FishList allFishes = new FishBook.FishList();

	private FishBook.FishList tierFishes = new FishBook.FishList();

	private FishBook.FishList bossFishes = new FishBook.FishList();

	private FishBook.FishList specialFishes = new FishBook.FishList();

	private Skill starCollectorSkill;

	[Serializable]
	public class FishList
	{
		public FishList()
		{
			this.List = new List<FishAttributes>();
		}

		public FishList(List<FishAttributes> list)
		{
			this.List = list;
		}

		public List<FishAttributes> List;
	}
}
