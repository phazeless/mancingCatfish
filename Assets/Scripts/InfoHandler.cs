using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SimpleFirebaseUnity;
using UnityEngine;

public class InfoHandler : MonoBehaviour
{
	private void Awake()
	{
		this.lastReceivedInfoId = EncryptedPlayerPrefs.GetString(InfoHandler.KEY_INFO, null);
		this.infoRef = Firebase.CreateNew(InfoHandler.HOST, string.Empty).Child("info", false);
		string param = "orderBy=\"created\"&limitToFirst=1";
		InfoHandler.InfoModel info = null;
		Firebase firebase = this.infoRef;
		firebase.OnGetFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase.OnGetFailed, new Action<Firebase, FirebaseError>(delegate(Firebase a, FirebaseError error)
		{
			this.OnInfoRetrieved(info);
		}));
		Firebase firebase2 = this.infoRef;
		firebase2.OnGetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase2.OnGetSuccess, new Action<Firebase, DataSnapshot>(delegate(Firebase a, DataSnapshot snapshot)
		{
			if (snapshot != null)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)snapshot.RawValue;
				if (dictionary != null)
				{
					foreach (KeyValuePair<string, object> keyValuePair in dictionary)
					{
						info = JsonConvert.DeserializeObject<InfoHandler.InfoModel>(JsonConvert.SerializeObject(keyValuePair.Value));
						if (info != null)
						{
							info.Id = keyValuePair.Key;
							info = ((!info.Expired) ? info : null);
						}
					}
				}
			}
			this.OnInfoRetrieved(info);
		}));
		this.infoRef.GetValue(param);
	}

	private void OnInfoRetrieved(InfoHandler.InfoModel info)
	{
		if (info != null)
		{
			if (info.Id != this.lastReceivedInfoId)
			{
				this.lastReceivedInfoId = info.Id;
				IGNInfo igninfo = new IGNInfo();
				igninfo.Info = info;
				InGameNotificationManager.Instance.Create<IGNInfo>(igninfo);
			}
		}
		else
		{
			UnityEngine.Debug.Log("Failed to retrieve Info from DB!");
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			EncryptedPlayerPrefs.SetString(InfoHandler.KEY_INFO, this.lastReceivedInfoId, true);
		}
	}

	private static readonly string KEY_INFO = "KEY_INFO";

	private static readonly string HOST = "fishinc-app.firebaseio.com";

	private static readonly string API_KEY = "AIzaSyBm9T8MScrMElaadDWev9JkiWWaQUt-qY0";

	private string lastReceivedInfoId;

	private Firebase infoRef;

	public class InfoModel
	{
		public string MainTitle;

		public string SubTitle;

		public string Content;

		public string Id;

		public bool Expired;
	}
}
