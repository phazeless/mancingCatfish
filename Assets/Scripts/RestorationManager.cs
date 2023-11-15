using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CloudOnce.Internal.Utils;
using Newtonsoft.Json;
using SimpleFirebaseUnity;
using UnityEngine;

public class RestorationManager : MonoBehaviour
{
	public static RestorationManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnRestoreStateChanged;

	private string UserAuthToken
	{
		get
		{
			return UserManager.Instance.LocalUser.IdToken;
		}
	}

	private string UserId
	{
		get
		{
			return UserManager.Instance.LocalUser.LocalId;
		}
	}

	private string Username
	{
		get
		{
			return UserManager.Instance.LocalUser.Username;
		}
	}

	public bool HasPendingClaimData
	{
		get
		{
			return this.pendingClaimData != null;
		}
	}

	public bool HasCheckedForClaim
	{
		get
		{
			return this.hasCheckedForClaim;
		}
	}

	public bool HasRequestedClaim
	{
		get
		{
			return this.hasRequestedClaim;
		}
	}

	public bool HasClaimed
	{
		get
		{
			return this.hasClaimed;
		}
	}

	public RestorationManager.ClaimData PendingClaimData
	{
		get
		{
			return this.pendingClaimData;
		}
	}

	private void Awake()
	{
		RestorationManager.Instance = this;
	}

	private void LoadGameStateFromServer()
	{
		if (this.HasPendingClaimData)
		{
			string value = this.pendingClaimData.currentGameState.FromBase64StringToString();
			Dictionary<string, object> cachedPlayerPrefs = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
			EncryptedPlayerPrefs.SetCachedPlayerPrefs(cachedPlayerPrefs);
		}
	}

	public void Claim()
	{
		if (this.HasPendingClaimData)
		{
			Firebase firebase = Firebase.CreateNew(RestorationManager.HOST, this.UserAuthToken).Child("claims", false).Child(this.UserId, false).Child("claimed", false);
			firebase.OnSetSuccess = new Action<Firebase, DataSnapshot>(this.OnClaimSuccess);
			firebase.OnSetFailed = new Action<Firebase, FirebaseError>(this.OnClaimFailed);
			firebase.SetValue(true, string.Empty);
		}
	}

	public void CheckForClaim()
	{
		Firebase firebase = Firebase.CreateNew(RestorationManager.HOST, this.UserAuthToken).Child("claims", false).Child(this.UserId, false);
		firebase.OnGetSuccess = new Action<Firebase, DataSnapshot>(this.OnClaimRetrieveSuccess);
		firebase.OnGetFailed = new Action<Firebase, FirebaseError>(this.OnClaimRetrieveFailed);
		firebase.GetValue(string.Empty);
	}

	public void RequestClaim()
	{
		Firebase firebase = Firebase.CreateNew(RestorationManager.HOST, this.UserAuthToken).Child("claims", false).Child(this.UserId, false);
		firebase.OnUpdateSuccess = new Action<Firebase, DataSnapshot>(this.OnRequestClaimSuccess);
		firebase.OnUpdateFailed = new Action<Firebase, FirebaseError>(this.OnRequestClaimFailed);
		RestorationManager.ClaimData claimData = new RestorationManager.ClaimData();
		claimData.username = this.Username;
		try
		{
			claimData.currentGameState = JsonConvert.SerializeObject(EncryptedPlayerPrefs.CachedPlayerPrefs).ToBase64String();
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("ABC: Failed to convert Game State. Error: " + ex.Message);
		}
		string json = JsonConvert.SerializeObject(claimData);
		firebase.UpdateValue(json, true, default(FirebaseParam));
	}

	private void OnRequestClaimSuccess(Firebase fRef, DataSnapshot snapshot)
	{
		UnityEngine.Debug.Log("Request claim Success!");
		this.hasRequestedClaim = true;
		if (this.pendingClaimData == null)
		{
			this.pendingClaimData = new RestorationManager.ClaimData();
		}
		if (this.OnRestoreStateChanged != null)
		{
			this.OnRestoreStateChanged();
		}
	}

	private void OnRequestClaimFailed(Firebase fRef, FirebaseError error)
	{
		UnityEngine.Debug.Log("Request claim failed! Error: " + error.Message);
		this.hasRequestedClaim = false;
		if (this.OnRestoreStateChanged != null)
		{
			this.OnRestoreStateChanged();
		}
	}

	private void OnClaimSuccess(Firebase fRef, DataSnapshot snapshot)
	{
		try
		{
			ResourceChangeData gemChangeData = new ResourceChangeData("contentId_restoreProgress", "Restore Progress", this.pendingClaimData.gems, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.RestoreProgress);
			ResourceManager.Instance.GiveGems(this.pendingClaimData.gems, gemChangeData);
			if (this.pendingClaimData.crownExp > 0)
			{
				ResourceChangeData gemChangeData2 = new ResourceChangeData("contentId_restoreProgress", "Restore Progress", this.pendingClaimData.crownExp, ResourceType.CrownExp, ResourceChangeType.Earn, ResourceChangeReason.RestoreProgress);
				ResourceManager.Instance.GiveCrownExp(this.pendingClaimData.crownExp, gemChangeData2);
			}
			if (this.pendingClaimData.fishExp > 0)
			{
				SkillManager.Instance.PrestigeSkill.SetCurrentLevelAsLong(SkillManager.Instance.PrestigeSkill.CurrentLevelAsLong + (long)this.pendingClaimData.fishExp, LevelChange.LevelUp);
				FishingExperienceHolder.Instance.UpdatePrestigeTextUI();
			}
			IEnumerable<Skill> enumerable = from x in SkillManager.Instance.CrewMembers
			where this.pendingClaimData.crewMemberIds.Contains(x.Id)
			select x;
			foreach (Skill crewMember in enumerable)
			{
				PurchaseCrewMemberHandler.Instance.GetCrewMember(crewMember, ResourceChangeReason.RestoreProgress, 0);
			}
			if (this.pendingClaimData.randomCrewAmount > 0)
			{
				List<Skill> randomCrewMembers = this.GetRandomCrewMembers(this.pendingClaimData.randomCrewAmount);
				foreach (Skill crewMember2 in randomCrewMembers)
				{
					PurchaseCrewMemberHandler.Instance.GetCrewMember(crewMember2, ResourceChangeReason.RestoreProgress, 0);
				}
			}
			foreach (string id in this.pendingClaimData.chestIds)
			{
				ItemChest chestById = ChestManager.Instance.GetChestById(id);
				if (chestById != null)
				{
					ChestManager.Instance.OpenChest(chestById);
				}
			}
			foreach (string text in this.pendingClaimData.fishIds)
			{
				try
				{
					if (!(text == "placeholder"))
					{
						FishBehaviour fishPrefab = FishPoolManager.Instance.GetFishPrefab((FishBehaviour.FishType)Enum.Parse(typeof(FishBehaviour.FishType), text, false));
						FishBook.Instance.TryAddToBook(fishPrefab);
					}
				}
				catch
				{
				}
			}
			foreach (string text2 in this.pendingClaimData.itemIds)
			{
				try
				{
					if (!(text2 == "placeholder"))
					{
						string[] array = text2.Split(new char[]
						{
							'_'
						});
						string actualId = array[0];
						int amount = (array.Length < 2) ? 1 : int.Parse(array[1]);
						Item item = ItemManager.Instance.AllItems.Find((Item x) => x.Id == actualId);
						item.ChangeItemAmount(amount, ResourceChangeReason.RestoreProgress);
					}
				}
				catch
				{
				}
			}
			foreach (string text3 in this.pendingClaimData.consumableIds)
			{
				try
				{
					if (!(text3 == "placeholder"))
					{
						string[] array2 = text3.Split(new char[]
						{
							'_'
						});
						string consumableId = array2[0];
						int amount2 = (array2.Length < 2) ? 1 : int.Parse(array2[1]);
						ConsumableManager.Instance.Grant(consumableId, amount2, ResourceChangeReason.RestoreProgress, false);
					}
				}
				catch
				{
				}
			}
			if (this.pendingClaimData.dailyCatchStreak > 0)
			{
				DailyGiftManager.Instance.SetStreak(this.pendingClaimData.dailyCatchStreak);
			}
			this.pendingClaimData = null;
			this.hasClaimed = true;
		}
		catch (Exception ex)
		{
			this.hasClaimed = false;
			UnityEngine.Debug.Log("Failed to give user Claim stuff. Error: " + ex.Message);
		}
		if (this.OnRestoreStateChanged != null)
		{
			this.OnRestoreStateChanged();
		}
	}

	private void OnClaimFailed(Firebase fRef, FirebaseError error)
	{
		UnityEngine.Debug.Log("Claim failed!");
		this.hasClaimed = false;
		if (this.OnRestoreStateChanged != null)
		{
			this.OnRestoreStateChanged();
		}
	}

	private void OnClaimRetrieveSuccess(Firebase fRef, DataSnapshot snapshot)
	{
		try
		{
			string value = JsonConvert.SerializeObject(snapshot.RawValue);
			this.pendingClaimData = JsonConvert.DeserializeObject<RestorationManager.ClaimData>(value);
			this.hasClaimed = false;
			this.hasRequestedClaim = false;
			UnityEngine.Debug.Log("OnClaimRetrieveSuccess");
		}
		catch
		{
			UnityEngine.Debug.LogError("Failed to convert Claim-data to value types");
		}
		this.hasCheckedForClaim = true;
		if (this.OnRestoreStateChanged != null)
		{
			this.OnRestoreStateChanged();
		}
	}

	private void OnClaimRetrieveFailed(Firebase fRef, FirebaseError error)
	{
		UnityEngine.Debug.LogError("Failed to retrieve claim! Error: " + error.Message);
		this.hasCheckedForClaim = false;
		if (this.OnRestoreStateChanged != null)
		{
			this.OnRestoreStateChanged();
		}
	}

	private List<Skill> GetRandomCrewMembers(int crewAmountToUnlockRandomly)
	{
		List<Skill> list = new List<Skill>();
		List<Skill> list2 = SkillManager.Instance.CrewMembers.ToList<Skill>();
		List<Skill> list3 = list2.FindAll((Skill x) => x.CurrentLevel == 0 && !x.GetExtraInfo().IsFacebookCrew && !x.GetExtraInfo().IsOnlyAvailableThroughPurchase);
		if (crewAmountToUnlockRandomly > 0)
		{
			if (list3.Count < crewAmountToUnlockRandomly)
			{
				List<Skill> source = list2.FindAll((Skill x) => x.CurrentLevel == 0 && !x.GetExtraInfo().IsFacebookCrew && x.GetExtraInfo().IsOnlyAvailableThroughPurchase);
				int count = crewAmountToUnlockRandomly - list3.Count;
				IEnumerable<Skill> collection = source.Take(count);
				list3.AddRange(collection);
			}
			int count2 = list3.Count;
			for (int i = 0; i < Mathf.Min(crewAmountToUnlockRandomly, count2); i++)
			{
				Skill item = list3[UnityEngine.Random.Range(0, list3.Count)];
				list3.Remove(item);
				list.Add(item);
			}
		}
		return list;
	}

	private static readonly string HOST = "fishinc-app.firebaseio.com";

	private RestorationManager.ClaimData pendingClaimData;

	private bool hasCheckedForClaim;

	private bool hasRequestedClaim;

	private bool hasClaimed;

	private const string contentId_restoreProgress = "contentId_restoreProgress";

	private const string contentName_restoreProgress = "Restore Progress";

	public class ClaimData
	{
		public string username = "placeholder";

		public bool claimed;

		public bool ready;

		public int gems;

		public int fishExp;

		public int dailyCatchStreak;

		public int crownExp;

		public List<string> crewMemberIds = new List<string>
		{
			"placeholder"
		};

		public List<string> chestIds = new List<string>
		{
			"placeholder"
		};

		public List<string> fishIds = new List<string>
		{
			"placeholder"
		};

		public List<string> itemIds = new List<string>
		{
			"placeholder"
		};

		public List<string> consumableIds = new List<string>
		{
			"placeholder"
		};

		public int randomCrewAmount;

		public string currentGameState;
	}
}
