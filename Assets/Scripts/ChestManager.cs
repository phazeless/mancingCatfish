using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<ItemChest, List<Item>> OnChestOpened;

	public static ChestManager Instance { get; private set; }

	private void Awake()
	{
		ChestManager.Instance = this;
		foreach (ItemChest itemChest in this.availableChests)
		{
			this.chestsById.Add(itemChest.Id, itemChest);
		}
		this.LoadChests();
	}

	private void Start()
	{
		this.CreateChestIGNs();
	}

	public void CreateChestIGNs()
	{
		foreach (RecievedChest chest in this.receivedChests)
		{
			IGNItemChest ignitemChest = new IGNItemChest();
			ignitemChest.Chest = chest;
			InGameNotificationManager.Instance.Create<IGNItemChest>(ignitemChest);
		}
	}

	public void OpenChest(RecievedChest recievedChest)
	{
		this.receivedChests.Remove(recievedChest);
		this.OpenChest(this.chestsById[recievedChest.ChestId]);
	}

	public void OpenChest(ItemChest chest)
	{
		List<Rarity> randomCardRarities = chest.GetRandomCardRarities();
		Dictionary<Rarity, List<Item>> dictionary = new Dictionary<Rarity, List<Item>>();
		dictionary.Add(Rarity.Common, ItemManager.Instance.GetItemsWith(Rarity.Common, false));
		dictionary.Add(Rarity.Rare, ItemManager.Instance.GetItemsWith(Rarity.Rare, false));
		dictionary.Add(Rarity.Epic, ItemManager.Instance.GetItemsWith(Rarity.Epic, false));
		List<Item> list = new List<Item>();
		foreach (Rarity key in randomCardRarities)
		{
			List<Item> list2 = dictionary[key];
			Item item = list2[UnityEngine.Random.Range(0, list2.Count)];
			list.Add(item);
		}
		Dictionary<Item, int> dictionary2 = list.DistinctAndCount<Item>();
		foreach (KeyValuePair<Item, int> keyValuePair in dictionary2)
		{
			Item key2 = keyValuePair.Key;
			int value = keyValuePair.Value;
			key2.ChangeItemAmount(value, ResourceChangeReason.ItemChestOpen);
		}
		if (this.OnChestOpened != null)
		{
			this.OnChestOpened(chest, list);
		}
	}

	public ItemChest GetChestById(string id)
	{
		if (this.chestsById.ContainsKey(id))
		{
			return this.chestsById[id];
		}
		return null;
	}

	public void CreateChestFromSettings(ChestSpawnSettings settings)
	{
		IGNItemChest ignitemChest = (IGNItemChest)settings.GetIGN();
		ignitemChest.Chest = new RecievedChest();
		ignitemChest.Chest.RecievedDate = DateTime.Now;
		ignitemChest.Chest.ChestId = settings.Chest.Id;
		InGameNotificationManager.Instance.Create<IGNItemChest>(ignitemChest);
		this.receivedChests.Add(ignitemChest.Chest);
	}

	public void RecieveChest()
	{
		RecievedChest recievedChest = new RecievedChest();
		recievedChest.RecievedDate = DateTime.Now;
		recievedChest.ChestId = this.availableChests[2].Id;
		this.receivedChests.Add(recievedChest);
		IGNItemChest ignitemChest = new IGNItemChest();
		ignitemChest.Chest = recievedChest;
		InGameNotificationManager.Instance.Create<IGNItemChest>(ignitemChest);
	}

	public void CreateReceivedChest(ItemChest chest)
	{
		if (chest != null)
		{
			RecievedChest recievedChest = new RecievedChest();
			recievedChest.RecievedDate = DateTime.Now;
			recievedChest.ChestId = chest.Id;
			this.receivedChests.Add(recievedChest);
			IGNItemChest ignitemChest = new IGNItemChest();
			ignitemChest.Chest = recievedChest;
			InGameNotificationManager.Instance.Create<IGNItemChest>(ignitemChest);
		}
	}

	public List<DateTime> GetUnlockedTimesForRecievedChests()
	{
		List<DateTime> list = new List<DateTime>();
		foreach (RecievedChest recievedChest in this.receivedChests)
		{
			ItemChest chestById = this.GetChestById(recievedChest.ChestId);
			if (chestById != null)
			{
				int num = Mathf.Max(0, chestById.GetSecondsUntilUnlocked(recievedChest.GetElapsedSecondsSinceReceived()));
				list.Add(DateTime.Now.AddSeconds((double)num));
			}
		}
		return list;
	}

	public void LoadChests()
	{
		string @string = EncryptedPlayerPrefs.GetString("KEY_RECEIVED_CHESTS", null);
		if (!string.IsNullOrEmpty(@string))
		{
			this.receivedChests = JsonConvert.DeserializeObject<List<RecievedChest>>(@string);
		}
	}

	public void SaveChests()
	{
		if (TournamentManager.Instance != null && !TournamentManager.Instance.IsInsideTournament)
		{
			EncryptedPlayerPrefs.SetString("KEY_RECEIVED_CHESTS", JsonConvert.SerializeObject(this.receivedChests), true);
		}
	}

	private void OnApplicationPause(bool didPause)
	{
		if (didPause)
		{
			this.SaveChests();
		}
	}

	private const string KEY_RECEIVED_CHESTS = "KEY_RECEIVED_CHESTS";

	[SerializeField]
	private List<ItemChest> availableChests = new List<ItemChest>();

	private Dictionary<string, ItemChest> chestsById = new Dictionary<string, ItemChest>();

	private List<RecievedChest> receivedChests = new List<RecievedChest>();
}
