using System;
using System.Diagnostics;
using FullInspector;
using TMPro;
using UnityEngine;

[fiInspectorOnly]
public class CrownExpGranter : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action<CrownExpGranter> onPotentiallyVisible;

	public GranterLocation Location
	{
		get
		{
			return this.location;
		}
		set
		{
			this.location = value;
		}
	}

	private void Awake()
	{
		this.cem = CrownExpGranterManager.Instance;
		this.onPotentiallyVisible = this.cem.RegisterGranter(this);
	}

	private void OnEnable()
	{
		this.OnPotentiallyVisible();
	}

	private void OnPotentiallyVisible()
	{
		if (this.onPotentiallyVisible != null)
		{
			this.onPotentiallyVisible(this);
		}
		this.UpdateState();
	}

	public void UpdateState()
	{
		bool flag = this.cem.IsCrownExpAvailableAtLocation(this.location);
		if (flag)
		{
			int crownExpAmountAtLocation = this.cem.GetCrownExpAmountAtLocation(this.location);
			this.amountLabel.SetVariableText(new string[]
			{
				crownExpAmountAtLocation.ToString()
			});
		}
		this.holder.SetActive(flag);
	}

	private void OnDestroy()
	{
		this.cem.UnregisterGranter(this);
	}

	[SerializeField]
	private GranterLocation location;

	[SerializeField]
	private TextMeshProUGUI amountLabel;

	[SerializeField]
	private GameObject holder;

	private CrownExpGranterManager cem;
}
