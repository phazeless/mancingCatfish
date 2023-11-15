using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class CrownExpGranterManager : MonoBehaviour
{
	public static CrownExpGranterManager Instance { get; private set; }

	private void Awake()
	{
		CrownExpGranterManager.Instance = this;
		this.Load();
		this.crownExpGranterAvailabilities = new CrownExpGranterAvailabilities(this, SkillTreeManager.Instance, ResourceManager.Instance, TournamentManager.Instance);
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.M))
		{
			ResourceChangeData changeData = new ResourceChangeData("testing", ResourceChangeReason.ForTesting.ToString(), 0, ResourceType.CrownExp, ResourceChangeType.Earn, ResourceChangeReason.ForTesting);
			this.Grant(AdPlacement.SpinWheel, changeData);
		}
	}

	private void UpdateCrownExpGrantersState()
	{
		for (int i = 0; i < this.granters.Count; i++)
		{
			this.granters[i].UpdateState();
		}
	}

	private void OnGranterPotentiallyVisible(CrownExpGranter granter)
	{
	}

	public void UpdateCrownExpGranterState(GranterLocation location)
	{
		for (int i = 0; i < this.granters.Count; i++)
		{
			CrownExpGranter crownExpGranter = this.granters[i];
			if (crownExpGranter.Location == location)
			{
				crownExpGranter.UpdateState();
				break;
			}
		}
	}

	public void Grant(IAPPlacement placement, ResourceChangeData changeData)
	{
		this.Grant(GranterLocation.Create(placement), changeData);
	}

	public void Grant(AdPlacement placement, ResourceChangeData changeData)
	{
		this.Grant(GranterLocation.Create(placement), changeData);
	}

	private void Grant(GranterLocation location, ResourceChangeData changeData)
	{
		if (this.IsCrownExpAvailableAtLocation(location))
		{
			CrownExpGrantedByLocation crownExpGrantedByLocation = this.crownExpGrantedByLocationCounter.Find((CrownExpGrantedByLocation x) => x.Location == location);
			if (crownExpGrantedByLocation == null)
			{
				crownExpGrantedByLocation = new CrownExpGrantedByLocation();
				crownExpGrantedByLocation.Location = location;
				this.crownExpGrantedByLocationCounter.Add(crownExpGrantedByLocation);
			}
			crownExpGrantedByLocation.Increase();
			int crownExpAmountAtLocation = this.GetCrownExpAmountAtLocation(location);
			changeData.Amount = crownExpAmountAtLocation;
			ResourceManager.Instance.GiveCrownExp(crownExpAmountAtLocation, changeData);
		}
		this.UpdateCrownExpGrantersState();
	}

	public Action<CrownExpGranter> RegisterGranter(CrownExpGranter granter)
	{
		this.granters.Add(granter);
		return new Action<CrownExpGranter>(this.OnGranterPotentiallyVisible);
	}

	public void UnregisterGranter(CrownExpGranter granter)
	{
		this.granters.Remove(granter);
	}

	public int GetCrownExpAmountAtLocation(IAPPlacement location)
	{
		return this.GetCrownExpAmountAtLocation(GranterLocation.Create(location));
	}

	public int GetCrownExpAmountAtLocation(AdPlacement location)
	{
		return this.GetCrownExpAmountAtLocation(GranterLocation.Create(location));
	}

	public int GetCrownExpAmountAtLocation(GranterLocation location)
	{
		return this.crownExpGranterAvailabilities.GetCrownExpAmountAtLocation(location);
	}

	public bool IsCrownExpAvailableAtLocation(GranterLocation location)
	{
		return this.crownExpGranterAvailabilities.IsCrownExpAvailableAtLocation(location);
	}

	public CrownExpGrantedByLocation CheckForReset(GranterLocation location, int hourLimit)
	{
		CrownExpGrantedByLocation crownExpGrantedByLocation = this.crownExpGrantedByLocationCounter.Find((CrownExpGrantedByLocation x) => x.Location == location);
		if (crownExpGrantedByLocation != null && (DateTime.Now - crownExpGrantedByLocation.LastGranted).TotalHours > (double)hourLimit)
		{
			crownExpGrantedByLocation.Count = 0;
		}
		return crownExpGrantedByLocation;
	}

	private void Save()
	{
		string value = JsonConvert.SerializeObject(this.crownExpGrantedByLocationCounter);
		if (!string.IsNullOrEmpty(value))
		{
			PlayerPrefs.SetString("KEY_CROWNEXP_GRANTED_BY_LOCATION_COUNTER", value);
		}
	}

	private void Load()
	{
		string @string = PlayerPrefs.GetString("KEY_CROWNEXP_GRANTED_BY_LOCATION_COUNTER", null);
		if (!string.IsNullOrEmpty(@string))
		{
			this.crownExpGrantedByLocationCounter = JsonConvert.DeserializeObject<List<CrownExpGrantedByLocation>>(@string);
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			this.Save();
		}
	}

	private const string KEY_CROWNEXP_GRANTED_BY_LOCATION_COUNTER = "KEY_CROWNEXP_GRANTED_BY_LOCATION_COUNTER";

	private CrownExpGranterAvailabilities crownExpGranterAvailabilities;

	private List<CrownExpGranter> granters = new List<CrownExpGranter>();

	private List<CrownExpGrantedByLocation> crownExpGrantedByLocationCounter = new List<CrownExpGrantedByLocation>();
}
