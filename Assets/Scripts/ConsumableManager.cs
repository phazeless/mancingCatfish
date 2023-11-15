using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BaseConsumable, int> OnConsumableConsumed;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BaseConsumable, int> OnConsumableGranted;

	public static ConsumableManager Instance { get; private set; }

	private void Awake()
	{
		ConsumableManager.Instance = this;
		this.Load();
	}

	public void Grant(string consumableId, int amount, ResourceChangeReason reason, bool overrideMaxLimit = false)
	{
		this.Grant(this.consumables.Find((BaseConsumable x) => x.Id == consumableId), amount, reason, overrideMaxLimit);
	}

	public void Grant(BaseConsumable consumable, int amount, ResourceChangeReason reason, bool overrideMaxLimit = false)
	{
		if (consumable == null)
		{
			return;
		}
		if (overrideMaxLimit)
		{
			this.InternalGrant(consumable, amount, reason);
		}
		else
		{
			int amount2 = this.GetAmount(consumable);
			int a = Mathf.Max(0, consumable.MaxAmount - amount2);
			int amount3 = Mathf.Min(a, amount);
			this.InternalGrant(consumable, amount3, reason);
		}
	}

	public bool ConsumeRandom(ResourceChangeReason reason)
	{
		return this.consumables.Count != 0 && this.Consume(this.consumables[UnityEngine.Random.Range(0, this.consumables.Count)], 1, reason);
	}

	public bool Consume(BaseConsumable consumable, int amount, ResourceChangeReason reason)
	{
		return this.InternalConsume(consumable, amount, reason);
	}

	public int ConsumeAll(BaseConsumable consumable, ResourceChangeReason reason)
	{
		int amount = this.GetAmount(consumable);
		if (this.InternalConsume(consumable, amount, reason))
		{
			return amount;
		}
		return 0;
	}

	public bool Has(BaseConsumable consumable)
	{
		return this.consumableBalance.ContainsKey(consumable.Id) && this.consumableBalance[consumable.Id] > 0;
	}

	public int GetAmount(BaseConsumable consumable)
	{
		return (!this.consumableBalance.ContainsKey(consumable.Id)) ? 0 : this.consumableBalance[consumable.Id];
	}

	public void Save()
	{
		EncryptedPlayerPrefs.SetString("KEY_CONSUMABLE_BALANCE", JsonConvert.SerializeObject(this.consumableBalance), true);
	}

	public void Load()
	{
		string @string = EncryptedPlayerPrefs.GetString("KEY_CONSUMABLE_BALANCE", null);
		if (@string != null)
		{
			this.consumableBalance = JsonConvert.DeserializeObject<Dictionary<string, int>>(@string);
		}
	}

	private void OnApplicationPause(bool didPause)
	{
		if (didPause)
		{
			this.Save();
		}
	}

	private bool InternalConsume(BaseConsumable consumable, int amount, ResourceChangeReason reason)
	{
		if (this.Has(consumable))
		{
			Dictionary<string, int> dictionary= this.consumableBalance;
			string id = consumable.Id;
			(dictionary = this.consumableBalance)[id = consumable.Id] = dictionary[id] - amount;
			consumable.Consume(amount);
			if (this.OnConsumableConsumed != null)
			{
				this.OnConsumableConsumed(consumable, amount);
			}
			return true;
		}
		return false;
	}

	private void InternalGrant(BaseConsumable consumable, int amount, ResourceChangeReason reason)
	{
		if (this.consumableBalance.ContainsKey(consumable.Id))
		{
			Dictionary<string, int> dictionary = this.consumableBalance;
			string id = consumable.Id;
			(dictionary = this.consumableBalance)[id = consumable.Id] = dictionary[id] + amount;
		}
		else
		{
			this.consumableBalance.Add(consumable.Id, amount);
		}
		consumable.Grant(amount);
		if (this.OnConsumableGranted != null)
		{
			this.OnConsumableGranted(consumable, amount);
		}
	}

	private const string KEY_CONSUMABLE_BALANCE = "KEY_CONSUMABLE_BALANCE";

	[SerializeField]
	private List<BaseConsumable> consumables = new List<BaseConsumable>();

	private Dictionary<string, int> consumableBalance = new Dictionary<string, int>();
}
