using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserSettingsPopup : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Start()
	{
		this.rm = RestorationManager.Instance;
		this.userIdInputField.text = UserManager.Instance.LocalUser.LocalId;
		this.rm.OnRestoreStateChanged += this.Rm_OnRestoreStateChanged;
	}

	private void Rm_OnRestoreStateChanged()
	{
		this.UpdateUI();
	}

	public void EnableClaiming()
	{
		this.checkCounter++;
		if (this.checkCounter % 3 == 0)
		{
			this.rm.CheckForClaim();
		}
	}

	public void CheckOrClaim()
	{
		if (!this.rm.HasPendingClaimData && this.rm.HasCheckedForClaim)
		{
			this.rm.RequestClaim();
			this.claimButton.interactable = false;
		}
		else if (!this.rm.PendingClaimData.claimed)
		{
			this.rm.Claim();
			this.claimButton.interactable = false;
		}
	}

	public void UpdateUI()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (!this.rm.HasPendingClaimData && this.rm.HasCheckedForClaim && !this.rm.HasRequestedClaim && !this.rm.HasClaimed)
		{
			this.claimButton.interactable = true;
			this.claimButtonText.SetText("Request");
		}
		else if (this.rm.HasPendingClaimData && !this.rm.PendingClaimData.claimed)
		{
			if (this.rm.PendingClaimData.ready)
			{
				this.claimButton.interactable = true;
				this.claimButtonText.SetText("Claim");
			}
			else
			{
				this.claimButton.interactable = false;
				this.claimButtonText.SetText("Processing");
			}
		}
		else
		{
			this.claimButton.interactable = false;
			this.claimButtonText.SetText("Claimed");
		}
	}

	private void OnDestroy()
	{
		this.rm.OnRestoreStateChanged += this.Rm_OnRestoreStateChanged;
	}

	[SerializeField]
	private TextMeshProUGUI claimButtonText;

	[SerializeField]
	private Button claimButton;

	[SerializeField]
	private TMP_InputField userIdInputField;

	private RestorationManager rm;

	private int checkCounter;
}
