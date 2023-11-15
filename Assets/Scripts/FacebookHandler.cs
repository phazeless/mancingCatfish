using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FacebookHandler : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnInitComplete;

	public static FacebookHandler Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnFacebookConnected;

	public void NotifyFacebookConnected()
	{
		if (this.OnFacebookConnected != null)
		{
			this.OnFacebookConnected();
		}
	}

	public void SetHasRefreshedFBInfo(bool hasRefreshed)
	{
		this.hasRefreshedFBInfo = hasRefreshed;
	}

	private void Awake()
	{
		FacebookHandler.Instance = this;
		this.ProfileImageTexture = new Texture2D(this.widthAndHeight, this.widthAndHeight, TextureFormat.RGBA32, false);
		
	}

	private void Start()
	{
		this.Load();
	}

	public void UpdateFBInfo(int widthAndHeightOfProfilePic, Action onComplete)
	{
		
	}

	private void Load()
	{
		this.CachedFBFirstName = EncryptedPlayerPrefs.GetString(FacebookHandler.KEY_FB_FIRSTNAME, this.CachedFBFirstName);
		this.CachedFriendCount = EncryptedPlayerPrefs.GetInt(FacebookHandler.KEY_FB_FRIENDCOUNT, this.CachedFriendCount);
		byte[] array = Convert.FromBase64String(EncryptedPlayerPrefs.GetString(FacebookHandler.KEY_FB_PROFILEIMAGE, string.Empty));
		if (array != null && array.Length > 0 && this.ProfileImageTexture != null)
		{
			this.ProfileImageTexture.LoadImage(array);
			this.ProfileImageTexture.Apply();
		}
	}

	private void Save()
	{
		EncryptedPlayerPrefs.SetString(FacebookHandler.KEY_FB_FIRSTNAME, this.CachedFBFirstName, true);
		EncryptedPlayerPrefs.SetInt(FacebookHandler.KEY_FB_FRIENDCOUNT, this.CachedFriendCount, true);
		if (this.ProfileImageTexture != null)
		{
			EncryptedPlayerPrefs.SetString(FacebookHandler.KEY_FB_PROFILEIMAGE, Convert.ToBase64String(this.ProfileImageTexture.EncodeToPNG()), true);
		}
	}

	private void OnApplicationPause(bool didPause)
	{
	
	}

	private static readonly string KEY_FB_FIRSTNAME = "KEY_FB_FIRSTNAME";

	private static readonly string KEY_FB_FRIENDCOUNT = "KEY_FB_FRIENDCOUNT";

	private static readonly string KEY_FB_PROFILEIMAGE = "KEY_FB_PROFILEIMAGE";

	public Texture2D ProfileImageTexture;

	public string CachedFBFirstName;

	public int CachedFriendCount;

	private bool hasRefreshedFBInfo;

	private int widthAndHeight = 128;
}
