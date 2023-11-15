using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFacebookCrewItem : MonoBehaviour
{
	private void Awake()
	{
		if (FacebookHandler.Instance != null && FacebookHandler.Instance.ProfileImageTexture != null)
		{
			this.cachedProfileImage = Sprite.Create(FacebookHandler.Instance.ProfileImageTexture, new Rect(0f, 0f, (float)this.widthAndHeight, (float)this.widthAndHeight), new Vector2(0.5f, 0.5f), 100f);
			this.facebookSkill.GetExtraInfo().Icon = this.cachedProfileImage;
		}
		else
		{
			UnityEngine.Debug.LogWarning("FacebookHandler.Instance or FacebookHandler.Instance.ProfileImageTexture is null inside UIFacebookCrewItem, UIFacebookCrewItem.Awake() is apparantly called before FacebookHandler.Awake()");
		}
	}

	private void Start()
	{
		if (FacebookHandler.Instance != null)
		{
			FacebookHandler.Instance.OnInitComplete += this.Instance_OnInitComplete;
		}
		else
		{
			UnityEngine.Debug.LogWarning("FacebookHandler.Instance is null inside UIFacebookCrewItem, UIFacebookCrewItem.Start() is apparantly called before FacebookHandler.Awake()");
		}
		this.UpdateUIContent();
	}

	private void OnEnable()
	{
		this.UpdateUI();
	}

	private void Instance_OnInitComplete()
	{
		this.UpdateUI();
	}

	private void UpdateUI()
	{
		this.UpdateUIContent();
		FacebookHandler.Instance.UpdateFBInfo(this.widthAndHeight, delegate
		{
			if (this.facebookSkill.CurrentLevel == 0)
			{
				this.facebookSkill.SetHasNotifiedSkillUnlocked(true);
				this.facebookSkill.LevelUpForFree();
			}
			if (this != null)
			{
				this.UpdateUIContent();
			}
		});
	}

	private void UpdateUIContent()
	{
		if (this.IsSomethingNull())
		{
			return;
		}
		if (FacebookHandler.Instance == null)
		{
			UnityEngine.Debug.LogWarning("FacebookHandler.Instance is null inside UIFacebookCrewItem.UpdateUIContent(). It seems FacebookHandler.Awake() has not yet been called.");
			return;
		}
		if (this.cachedProfileImage == null)
		{
			UnityEngine.Debug.LogWarning("cachedProfileImage is null, but shouldn't cause any critical issues.");
		}
		bool flag = false;
		this.connectButton.SetActive(!flag);
		this.questionMark.SetActive(!flag);
		this.likeFacebookPageButton.SetActive(flag);
		this.inviteFriendsButton.SetActive(flag);
		if (flag)
		{
			this.titleLabel.SetVariableText(new string[]
			{
				"Invite friends to Level Up!",
				string.Concat(new object[]
				{
					"Currently have: <color=white>",
					FacebookHandler.Instance.CachedFriendCount,
					" ",
					(FacebookHandler.Instance.CachedFriendCount != 1) ? "friends" : "friend",
					"</color> playing."
				})
			});
			this.fbProfileImage.sprite = this.cachedProfileImage;
			this.fbProfileImage.color = Color.white;
		}
		else
		{
			this.titleLabel.SetVariableText(new string[]
			{
				"Join the Crew as Yourself!",
				string.Empty
			});
		}
		int num = 1 + FacebookHandler.Instance.CachedFriendCount;
		if (num > this.facebookFriendCounterSkill.CurrentLevel)
		{
			this.facebookFriendCounterSkill.SetCurrentLevel(num, LevelChange.LevelUpFree);
		}
		this.lvlLabel.SetVariableText(new string[]
		{
			this.facebookSkill.CurrentLevel.ToString()
		});
		this.nameLabel.SetText((FacebookHandler.Instance.CachedFBFirstName != null) ? FacebookHandler.Instance.CachedFBFirstName : "You");
		this.UpdateSkillAttributeValues();
		this.facebookSkill.GetExtraInfo().TitleText = ((FacebookHandler.Instance.CachedFBFirstName != null) ? FacebookHandler.Instance.CachedFBFirstName : "You");
		this.facebookSkill.GetExtraInfo().Icon = this.cachedProfileImage;
	}

	private bool IsSomethingNull()
	{
		if (FacebookHandler.Instance == null)
		{
			UnityEngine.Debug.LogWarning("FacebookHandler.Instance is null");
		}
		if (this.connectButton == null)
		{
			UnityEngine.Debug.LogWarning("connectButton is null");
		}
		if (this.questionMark == null)
		{
			UnityEngine.Debug.LogWarning("questionMark is null");
		}
		if (this.titleLabel == null)
		{
			UnityEngine.Debug.LogWarning("titleLabel is null");
		}
		if (this.likeFacebookPageButton == null)
		{
			UnityEngine.Debug.LogWarning("likeFacebookPageButton is null");
		}
		if (this.inviteFriendsButton == null)
		{
			UnityEngine.Debug.LogWarning("inviteFriendsButton is null");
		}
		if (this.facebookFriendCounterSkill == null)
		{
			UnityEngine.Debug.LogWarning("facebookFriendCounterSkill is null");
		}
		if (this.facebookSkill == null)
		{
			UnityEngine.Debug.LogWarning("facebookSkill is null");
		}
		if (this.facebookSkill.GetExtraInfo() == null)
		{
			UnityEngine.Debug.LogWarning("facebookSkill.GetExtraInfo() is null");
		}
		if (this.lvlLabel == null)
		{
			UnityEngine.Debug.LogWarning("lvlLabel is null");
		}
		if (this.nameLabel == null)
		{
			UnityEngine.Debug.LogWarning("nameLabel is null");
		}
		bool result;
		if (result = (this.fbProfileImage == null))
		{
			UnityEngine.Debug.LogWarning("fbProfileImage is null");
		}
		return result;
	}

	private void Login(Action<bool> callback = null)
	{
		
	}

	public void Connect()
	{
		this.Login(delegate(bool success)
		{
			if (success && FacebookHandler.Instance != null)
			{
				FacebookHandler.Instance.NotifyFacebookConnected();
			}
		});
	}

	public void RefreshFriends()
	{
		
	}

	public void InviteFriends()
	{
		NativeShare nativeShare = new NativeShare();
		nativeShare.SetTitle("Hooked Inc: Fisher Tycoon");
		nativeShare.SetSubject("Hooked Inc: Fisher Tycoon");
		nativeShare.SetText("http://acegames.se/hi");
		nativeShare.Share();
	}

	public void LikeFacebookPage()
	{
		Application.OpenURL("https://www.facebook.com/Click-Bait-Inc-470448746647892/");
	}

	private void UpdateSkillAttributeValues()
	{
		SkillBehaviour skillBehaviour = this.facebookSkill.SkillBehaviours[0];
		float valueAtLevel = skillBehaviour.GetValueAtLevel(this.facebookSkill.NextLevel);
		string text = (valueAtLevel <= 0f) ? string.Empty : "+";
		float totalValueAtLevel = skillBehaviour.GetTotalValueAtLevel(this.facebookSkill.CurrentLevel);
		string text2 = (totalValueAtLevel <= 0f) ? string.Empty : "+";
		string text3 = FHelper.FindBracketAndReplace(skillBehaviour.Description, new string[]
		{
			string.Concat(new object[]
			{
				"<b>",
				text,
				valueAtLevel,
				skillBehaviour.PostFixCharacter,
				"</b>"
			})
		});
		this.attributeLabel.SetVariableText(new string[]
		{
			string.Empty,
			text3,
			string.Concat(new object[]
			{
				" (",
				text2,
				skillBehaviour.GetTotalValueAtLevel(this.facebookSkill.CurrentLevel),
				skillBehaviour.PostFixCharacter,
				")"
			})
		});
	}

	[SerializeField]
	private Skill facebookFriendCounterSkill;

	[SerializeField]
	private Skill facebookSkill;

	[SerializeField]
	private GameObject connectButton;

	[SerializeField]
	private GameObject likeFacebookPageButton;

	[SerializeField]
	private GameObject inviteFriendsButton;

	[SerializeField]
	private Image fbProfileImage;

	[SerializeField]
	private GameObject questionMark;

	[SerializeField]
	private TextMeshProUGUI titleLabel;

	[SerializeField]
	private TextMeshProUGUI nameLabel;

	[SerializeField]
	private TextMeshProUGUI attributeLabel;

	[SerializeField]
	private TextMeshProUGUI lvlLabel;

	private static readonly string KEY_FB_FIRSTNAME = "KEY_FB_FIRSTNAME";

	private static readonly string KEY_FB_FRIENDCOUNT = "KEY_FB_FRIENDCOUNT";

	private static readonly string KEY_FB_PROFILEIMAGE = "KEY_FB_PROFILEIMAGE";

	private Sprite cachedProfileImage;

	private bool hasRefreshedFBInfo;

	private int widthAndHeight = 128;
}
