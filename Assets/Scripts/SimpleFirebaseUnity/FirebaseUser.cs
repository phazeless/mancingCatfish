using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using SimpleFirebaseUnity.MiniJSON;
using UnityEngine;

namespace SimpleFirebaseUnity
{
	public class FirebaseUser
	{
		public bool ShouldRefreshIdToken { get; private set; }

		private bool HasValidIdToken
		{
			get
			{
				return (DateTime.Now - this.LastRefreshedIdToken).TotalSeconds < (double)FirebaseUser.REFRESH_ID_TOKEN_SAFETY_EXPIRATION;
			}
		}

		private DateTime IdTokenExpiration
		{
			get
			{
				return this.LastRefreshedIdToken.AddSeconds((double)FirebaseUser.REFRESH_ID_TOKEN_SAFETY_EXPIRATION);
			}
		}

		private int SecondsUntilIdTokenExpires
		{
			get
			{
				float num = (float)(this.IdTokenExpiration - DateTime.Now).TotalSeconds;
				num = Mathf.Max(0f, num);
				num = Mathf.Min(num, 3600f);
				return (int)num;
			}
		}

		protected void SetData(object dataDictAsObject)
		{
			if (dataDictAsObject is Dictionary<string, object>)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)dataDictAsObject;
				this.IdToken = (string)dictionary["idToken"];
				this.RefreshToken = (string)dictionary["refreshToken"];
				this.LocalId = (string)dictionary["localId"];
				this.LastRefreshedIdToken = DateTime.Now;
				this.ScheduleIdTokenRenewal(0, null);
			}
			this.Info = Firebase.CreateNew(this.Host, this.IdToken).Child("users", false).Child(this.LocalId, false);
		}

		[JsonIgnore]
		public Firebase Info { get; private set; }

		public void Init(Action<FirebaseUser, FirebaseError> onComplete)
		{
			this.RefreshIdToken(true, delegate(FirebaseUser firebaseUser, FirebaseError error)
			{
				if (error == null)
				{
					this.Info = Firebase.CreateNew(this.Host, this.IdToken).Child("users", false).Child(this.LocalId, false);
				}
				if (onComplete != null)
				{
					onComplete(firebaseUser, error);
				}
			});
		}

		private void ScheduleIdTokenRenewal(int rollOffTimeInSeconds, Action<FirebaseUser, FirebaseError> onResponse = null)
		{
			if (this.SecondsUntilIdTokenExpires + rollOffTimeInSeconds > 0)
			{
				this.idTokenTimer.Stop();
				this.idTokenTimer.AutoReset = false;
				double num = (double)((this.SecondsUntilIdTokenExpires + rollOffTimeInSeconds) * 1000);
				if (num <= 0.0)
				{
					num = 3600.0;
				}
				this.idTokenTimer.Interval = num;
				this.idTokenTimer.Elapsed -= this.OnTimerIntervalReached;
				this.idTokenTimer.Elapsed += this.OnTimerIntervalReached;
				this.idTokenTimer.Start();
				if (onResponse != null)
				{
					onResponse(this, null);
				}
			}
			else
			{
				this.RefreshIdToken(true, onResponse);
			}
		}

		private void OnTimerIntervalReached(object sender, ElapsedEventArgs args)
		{
			this.idTokenTimer.Stop();
			this.ShouldRefreshIdToken = true;
		}

		public void RefreshIdToken(bool forceRefreshIdToken, Action<FirebaseUser, FirebaseError> onResponse = null)
		{
			this.ShouldRefreshIdToken = false;
			if (!this.HasValidIdToken || forceRefreshIdToken)
			{
				string url = "https://securetoken.googleapis.com/v1/token?key=" + this.ApiKey;
				string s = "{\"grant_type\":\"refresh_token\",\"refresh_token\":\"" + this.RefreshToken + "\"}";
				FirebaseManager.Instance.StartCoroutine(FirebaseUser.RequestCoroutine(url, Encoding.GetEncoding("utf-8").GetBytes(s), new Dictionary<string, string>
				{
					{
						"Content-Type",
						"application/json"
					}
				}, delegate(DataSnapshot snapshot)
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)snapshot.RawValue;
					this.IdToken = (string)dictionary["id_token"];
					this.RefreshToken = (string)dictionary["refresh_token"];
					this.LastRefreshedIdToken = DateTime.Now;
					this.ScheduleIdTokenRenewal(0, null);
					this.currentBackOffCount = FirebaseUser.MINIMUM_BACK_OFF_COUNT;
					if (onResponse != null)
					{
						onResponse(this, null);
					}
				}, delegate(FirebaseError error)
				{
					int rollOffTimeInSeconds = (int)((Mathf.Pow(2f, (float)this.currentBackOffCount) - 1f) / 2f);
					this.ScheduleIdTokenRenewal(rollOffTimeInSeconds, onResponse);
					this.currentBackOffCount++;
					if (onResponse != null)
					{
						onResponse(this, error);
					}
				}));
			}
		}

		public static void CreateAnonymousUser<T>(string host, string apiKey, Action<T, FirebaseError> onResponse) where T : FirebaseUser, new()
		{
			string url = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + apiKey;
			string s = "{\"returnSecureToken\":true}";
			FirebaseManager.Instance.StartCoroutine(FirebaseUser.RequestCoroutine(url, Encoding.GetEncoding("iso-8859-1").GetBytes(s), new Dictionary<string, string>
			{
				{
					"Content-Type",
					"application/json"
				}
			}, delegate(DataSnapshot snapshot)
			{
				T t = Activator.CreateInstance<T>();
				t.Host = host;
				t.ApiKey = apiKey;
				t.SetData(snapshot.RawValue);
				if (onResponse != null)
				{
					onResponse(t, null);
				}
			}, delegate(FirebaseError error)
			{
				if (onResponse != null)
				{
					onResponse((T)((object)null), error);
				}
			}));
		}

		private static IEnumerator RequestCoroutine(string url, byte[] postData, Dictionary<string, string> headers, Action<DataSnapshot> OnSuccess, Action<FirebaseError> OnFailed)
		{
			using (WWW www = (headers == null) ? ((postData == null) ? new WWW(url) : new WWW(url, postData)) : new WWW(url, postData, headers))
			{
				yield return www;
				if (!string.IsNullOrEmpty(www.error))
				{
					HttpStatusCode status = (HttpStatusCode)0;
					string text = string.Empty;
					if (www.responseHeaders != null && www.responseHeaders.ContainsKey("STATUS"))
					{
						string text2 = www.responseHeaders["STATUS"];
						string[] array = text2.Split(new char[]
						{
							' '
						});
						int num = 0;
						if (array.Length >= 3 && int.TryParse(array[1], out num))
						{
							status = (HttpStatusCode)num;
						}
					}
					if (www.responseHeaders == null || www.error.Contains("crossdomain.xml") || www.error.Contains("Couldn't resolve"))
					{
						text = "No internet connection or crossdomain.xml policy problem";
					}
					else
					{
						try
						{
							if (!string.IsNullOrEmpty(www.text))
							{
								Dictionary<string, object> dictionary = Json.Deserialize(www.text) as Dictionary<string, object>;
								if (dictionary != null && dictionary.ContainsKey("error"))
								{
									text = (dictionary["error"] as string);
								}
							}
						}
						catch
						{
						}
					}
					if (OnFailed != null)
					{
						if (string.IsNullOrEmpty(text))
						{
							text = www.error;
						}
						if (text.Contains("Failed downloading"))
						{
							text = "Request failed with no info of error.";
						}
						OnFailed(new FirebaseError(status, text));
					}
				}
				else
				{
					DataSnapshot obj = new DataSnapshot(www.text);
					if (OnSuccess != null)
					{
						OnSuccess(obj);
					}
				}
			}
			yield break;
		}

		private static readonly int REFRESH_ID_TOKEN_SAFETY_EXPIRATION = 3601;

		private static readonly int MINIMUM_BACK_OFF_COUNT = 4;

		private int currentBackOffCount = FirebaseUser.MINIMUM_BACK_OFF_COUNT;

		public string ApiKey;

		public string IdToken;

		public string LocalId;

		public string RefreshToken;

		public string Host;

		public DateTime LastRefreshedIdToken = DateTime.MinValue;

		private Timer idTokenTimer = new Timer();
	}
}
