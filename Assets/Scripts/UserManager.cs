using System;
using System.Diagnostics;
using Newtonsoft.Json;
using SimpleFirebaseUnity;
using UnityEngine;

public class UserManager : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<User> OnUserInitialized;

	public static UserManager Instance { get; private set; }

	public User LocalUser { get; private set; }

	private void Awake()
	{
		UserManager.Instance = this;
	}

	private void Start()
	{
		SettingsManager.Instance.OnNameInitialized += this.Instance_OnNameInitialized;
	}

	private void Instance_OnNameInitialized(string name)
	{
		this.LoadUser(delegate(FirebaseUser loadedUser, FirebaseError loadedUserError)
		{
			if (loadedUserError == null)
			{
				bool flag = loadedUser == null;
				if (flag)
				{
					FirebaseUser.CreateAnonymousUser<User>(UserManager.HOST, UserManager.API_KEY, delegate(User newlyCreatedUser, FirebaseError error)
					{
						if (error == null)
						{
							this.LocalUser = newlyCreatedUser;
							this.LocalUser.Username = name;
							this.SaveUser();
						}
						else
						{
							UnityEngine.Debug.LogWarning("Failed to create a New User, error: " + error.Message);
						}
						if (this.OnUserInitialized != null)
						{
							this.OnUserInitialized(this.LocalUser);
						}
					});
				}
				else
				{
					this.LocalUser = (User)loadedUser;
					this.LocalUser.Username = name;
					if (this.OnUserInitialized != null)
					{
						this.OnUserInitialized(this.LocalUser);
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning("Failed to init user with new Refresh Token (probably no internet), error: " + loadedUserError.Message);
				if (this.OnUserInitialized != null)
				{
					this.OnUserInitialized(null);
				}
			}
		});
	}

	private void LoadUser(Action<FirebaseUser, FirebaseError> onResponse)
	{
		string @string = EncryptedPlayerPrefs.GetString(UserManager.KEY_SAVED_FIREBASE_USER, null);
		if (this.IsValidJSON(@string))
		{
			User user = JsonConvert.DeserializeObject<User>(@string);
			user.Init(onResponse);
		}
		else if (onResponse != null)
		{
			onResponse(null, null);
		}
	}

	private void SaveUser()
	{
		if (this.LocalUser != null)
		{
			string value = JsonConvert.SerializeObject(this.LocalUser);
			EncryptedPlayerPrefs.SetString(UserManager.KEY_SAVED_FIREBASE_USER, value, true);
		}
	}

	private void OnApplicationPause(bool didPause)
	{
		if (didPause)
		{
			this.SaveUser();
		}
	}

	private bool IsValidJSON(string json)
	{
		return !string.IsNullOrEmpty(json) && json != "null";
	}

	private void Update()
	{
		if (this.LocalUser != null && this.LocalUser.ShouldRefreshIdToken)
		{
			this.LocalUser.RefreshIdToken(true, null);
		}
	}

	private static readonly string HOST = "fishinc-app.firebaseio.com";

	private static readonly string API_KEY = "AIzaSyBm9T8MScrMElaadDWev9JkiWWaQUt-qY0";

	private static readonly string TMP_USERNAME = "Whyser";

	private static readonly string KEY_SAVED_FIREBASE_USER = "KEY_SAVED_FIREBASE_USER";
}
