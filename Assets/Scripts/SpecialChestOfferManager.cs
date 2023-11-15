using System;
using Newtonsoft.Json;
using UnityEngine;

public class SpecialChestOfferManager : MonoBehaviour
{
	public static SpecialChestOfferManager Instance { get; private set; }

	public bool HasSpecialChestOffer
	{
		get
		{
			return this.currentSpecialChestOffer != null;
		}
	}

	public bool IsCorrectDay
	{
		get
		{
			return this.dayOfMonth == DateTime.Now.Day;
		}
	}

	public DateTime DayOfMonthToCreateOffer
	{
		get
		{
			int day = DateTime.Now.Day;
			int num = this.dayOfMonth - day;
			DateTime result = DateTime.Now.AddDays((double)num);
			if (num <= 0)
			{
				result = result.AddMonths(1);
			}
			return result;
		}
	}

	public int DwLvlThreshold
	{
		get
		{
			return this.dwLvlThreshold;
		}
	}

	private void Awake()
	{
		SpecialChestOfferManager.Instance = this;
		this.Load();
	}

	private void Start()
	{
		if (this.HasSpecialChestOffer && (this.currentSpecialChestOffer.SecondsUntilExpiration <= 0 || this.currentSpecialChestOffer.WasPurchased))
		{
			this.currentSpecialChestOffer = null;
		}
		if (this.HasSpecialChestOffer)
		{
			this.currentSpecialChestOffer.ItemChest = this.itemChestToOffer;
			this.currentSpecialChestOffer.IapSKU = this.chestIapSKU;
			this.CreateUIOffer(this.currentSpecialChestOffer);
		}
		else if (this.IsCorrectDay && SkillManager.Instance.DeepWaterSkill.LifetimeLevel >= this.dwLvlThreshold)
		{
			this.currentSpecialChestOffer = new SpecialOfferChestItem();
			this.currentSpecialChestOffer.ItemChest = this.itemChestToOffer;
			this.currentSpecialChestOffer.IapSKU = this.chestIapSKU;
			this.CreateUIOffer(this.currentSpecialChestOffer);
		}
	}

	private void CreateUIOffer(SpecialOfferChestItem item)
	{
		UISpecialOfferChestItem uispecialOfferChestItem = UnityEngine.Object.Instantiate<UISpecialOfferChestItem>(this.prefabSpecialChestOfferItem, this.parentForSpecialOffer, false);
		uispecialOfferChestItem.transform.SetSiblingIndex(3);
		uispecialOfferChestItem.SetSpecialChestOfferItem(item);
	}

	private void Save()
	{
		if (this.currentSpecialChestOffer == null)
		{
			EncryptedPlayerPrefs.DeleteKey(SpecialChestOfferManager.KEY_CURRENT_SPECIAL_CHEST_OFFER);
		}
		else
		{
			string value = JsonConvert.SerializeObject(this.currentSpecialChestOffer);
			EncryptedPlayerPrefs.SetString(SpecialChestOfferManager.KEY_CURRENT_SPECIAL_CHEST_OFFER, value, true);
		}
	}

	private void Load()
	{
		string @string = EncryptedPlayerPrefs.GetString(SpecialChestOfferManager.KEY_CURRENT_SPECIAL_CHEST_OFFER, null);
		if (@string != null)
		{
			this.currentSpecialChestOffer = JsonConvert.DeserializeObject<SpecialOfferChestItem>(@string);
		}
	}

	private void OnApplicationPause(bool didPause)
	{
		if (didPause)
		{
			this.Save();
		}
	}

	private static readonly string KEY_CURRENT_SPECIAL_CHEST_OFFER = "KEY_CURRENT_SPECIAL_CHEST_OFFER";

	[SerializeField]
	private string chestIapSKU;

	[SerializeField]
	private ItemChest itemChestToOffer;

	[SerializeField]
	private UISpecialOfferChestItem prefabSpecialChestOfferItem;

	[SerializeField]
	private Transform parentForSpecialOffer;

	[SerializeField]
	private int dayOfMonth;

	[SerializeField]
	private int dwLvlThreshold;

	private SpecialOfferChestItem currentSpecialChestOffer;
}
