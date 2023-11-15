using System;
using FullInspector;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class GranterLocation : IEquatable<GranterLocation>
{
	public GranterLocation()
	{
	}

	protected GranterLocation(AdPlacement location)
	{
		this.granterType = GranterType.Ad;
		this.adLocation = location;
		this.iapLocation = IAPPlacement.None;
	}

	protected GranterLocation(IAPPlacement location)
	{
		this.granterType = GranterType.IAP;
		this.iapLocation = location;
		this.adLocation = AdPlacement.None;
	}

	private bool IsGranterType_Ad
	{
		get
		{
			return this.granterType == GranterType.Ad;
		}
	}

	private bool IsGranterType_IAP
	{
		get
		{
			return this.granterType == GranterType.IAP;
		}
	}

	[JsonIgnore]
	public GranterType GranterType
	{
		get
		{
			return this.granterType;
		}
	}

	[JsonIgnore]
	public int LocationTypeAsInt
	{
		get
		{
			if (this.granterType == GranterType.Ad)
			{
				return (int)this.adLocation;
			}
			if (this.granterType == GranterType.IAP)
			{
				return (int)this.iapLocation;
			}
			return -1;
		}
	}

	public static GranterLocation Create(AdPlacement location)
	{
		return new GranterLocation(location);
	}

	public static GranterLocation Create(IAPPlacement location)
	{
		return new GranterLocation(location);
	}

	public static bool operator ==(GranterLocation l1, GranterLocation l2)
	{
		if (object.ReferenceEquals(l1, null))
		{
			return object.ReferenceEquals(l2, null);
		}
		if (object.ReferenceEquals(l1, l2))
		{
			return true;
		}
		if (l1.granterType == l2.granterType)
		{
			GranterType granterType = l1.granterType;
			if (granterType == GranterType.Ad)
			{
				return l1.adLocation == l2.adLocation;
			}
			if (granterType == GranterType.IAP)
			{
				return l1.iapLocation == l2.iapLocation;
			}
		}
		return false;
	}

	public static bool operator !=(GranterLocation l1, GranterLocation l2)
	{
		return !(l1 == l2);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public bool Equals(GranterLocation other)
	{
		return this == other;
	}

	[JsonProperty]
	[SerializeField]
	private GranterType granterType;

	[InspectorShowIf("IsGranterType_Ad")]
	[FullInspector.InspectorName("Location")]
	[JsonProperty]
	[SerializeField]
	private AdPlacement adLocation = AdPlacement.None;

	[SerializeField]
	[JsonProperty]
	[FullInspector.InspectorName("Location")]
	[InspectorShowIf("IsGranterType_IAP")]
	private IAPPlacement iapLocation = IAPPlacement.None;
}
