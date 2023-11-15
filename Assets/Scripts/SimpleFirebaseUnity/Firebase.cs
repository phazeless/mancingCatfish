using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SimpleFirebaseUnity.MiniJSON;
using UnityEngine;

namespace SimpleFirebaseUnity
{
	[Serializable]
	public class Firebase
	{
		internal Firebase(Firebase _parent, string _key, FirebaseRoot _root, bool inheritCallback = false)
		{
			this.parent = _parent;
			this.key = _key;
			this.root = _root;
			this.fullKey = this.parent.FullKey + "/" + this.key;
			if (inheritCallback)
			{
				this.OnGetSuccess = this.parent.OnGetSuccess;
				this.OnGetFailed = this.parent.OnGetFailed;
				this.OnSetSuccess = this.parent.OnSetSuccess;
				this.OnSetFailed = this.parent.OnSetFailed;
				this.OnUpdateSuccess = this.parent.OnUpdateSuccess;
				this.OnUpdateFailed = this.parent.OnUpdateFailed;
				this.OnPushSuccess = this.parent.OnPushSuccess;
				this.OnPushFailed = this.parent.OnPushFailed;
				this.OnDeleteSuccess = this.parent.OnDeleteSuccess;
				this.OnDeleteFailed = this.parent.OnDeleteFailed;
			}
		}

		internal Firebase()
		{
			this.parent = null;
			this.key = string.Empty;
			this.root = null;
		}

		public Firebase Parent
		{
			get
			{
				return this.parent;
			}
		}

		public Firebase Root
		{
			get
			{
				return this.root;
			}
		}

		public virtual string Endpoint
		{
			get
			{
				return "https://" + this.Host + this.FullKey + "/.json";
			}
		}

		public virtual string Host
		{
			get
			{
				return this.root.Host;
			}
		}

		public string FullKey
		{
			get
			{
				return this.fullKey;
			}
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public virtual string Credential
		{
			get
			{
				return this.root.Credential;
			}
			set
			{
				this.root.Credential = value;
			}
		}

		public virtual string RulesEndpoint
		{
			get
			{
				return this.root.RulesEndpoint;
			}
		}

		public Firebase Child(string _key, bool inheritCallback = false)
		{
			return new Firebase(this, _key, this.root, inheritCallback);
		}

		public List<Firebase> Childs(List<string> _keys)
		{
			List<Firebase> list = new List<Firebase>();
			foreach (string text in _keys)
			{
				list.Add(this.Child(text, false));
			}
			return list;
		}

		public List<Firebase> Childs(string[] _keys)
		{
			List<Firebase> list = new List<Firebase>();
			foreach (string text in _keys)
			{
				list.Add(this.Child(text, false));
			}
			return list;
		}

		public Firebase Copy(bool inheritCallback = false)
		{
			Firebase firebase;
			if (this.parent == null)
			{
				firebase = this.root.Copy();
			}
			else
			{
				firebase = new Firebase(this.parent, this.key, this.root, false);
			}
			if (inheritCallback)
			{
				firebase.OnGetSuccess = this.OnGetSuccess;
				firebase.OnGetFailed = this.OnGetFailed;
				firebase.OnSetSuccess = this.OnSetSuccess;
				firebase.OnSetFailed = this.OnSetFailed;
				firebase.OnUpdateSuccess = this.OnUpdateSuccess;
				firebase.OnUpdateFailed = this.OnUpdateFailed;
				firebase.OnPushSuccess = this.OnPushSuccess;
				firebase.OnPushFailed = this.OnPushFailed;
				firebase.OnDeleteSuccess = this.OnDeleteSuccess;
				firebase.OnDeleteFailed = this.OnDeleteFailed;
			}
			return firebase;
		}

		public void GetValue(FirebaseParam query)
		{
			this.GetValue(query.Parameter);
		}

		public void GetValue(string param = "")
		{
			try
			{
				if (this.Credential != string.Empty)
				{
					FirebaseParam firebaseParam = new FirebaseParam(param);
					param = firebaseParam.Auth(this.Credential).Parameter;
				}
				string text = this.Endpoint;
				if (param != string.Empty)
				{
					text = text + "?" + param;
				}
				this.root.StartCoroutine(this.RequestCoroutine(text, null, null, this.OnGetSuccess, this.OnGetFailed));
			}
			catch (WebException webEx)
			{
				if (this.OnGetFailed != null)
				{
					this.OnGetFailed(this, FirebaseError.Create(webEx));
				}
			}
			catch (Exception ex)
			{
				if (this.OnGetFailed != null)
				{
					this.OnGetFailed(this, new FirebaseError(ex.Message));
				}
			}
		}

		public void SetValue(string json, bool isJson, string param = "")
		{
			if (!isJson)
			{
				this.SetValue(json, param);
			}
			else
			{
				this.SetValue(Json.Deserialize(json), param);
			}
		}

		public void SetValue(object _val, string param = "")
		{
			try
			{
				if (this.Credential != string.Empty)
				{
					FirebaseParam firebaseParam = new FirebaseParam(param);
					param = firebaseParam.Auth(this.Credential).Parameter;
				}
				string text = this.Endpoint;
				if (param != string.Empty)
				{
					text = text + "?" + param;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Content-Type", "application/json");
				dictionary.Add("X-HTTP-Method-Override", "PUT");
				byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(Json.Serialize(_val));
				this.root.StartCoroutine(this.RequestCoroutine(text, bytes, dictionary, this.OnSetSuccess, this.OnSetFailed));
			}
			catch (WebException webEx)
			{
				if (this.OnSetFailed != null)
				{
					this.OnSetFailed(this, FirebaseError.Create(webEx));
				}
			}
			catch (Exception ex)
			{
				if (this.OnSetFailed != null)
				{
					this.OnSetFailed(this, new FirebaseError(ex.Message));
				}
			}
		}

		public void SetValue(string json, bool isJson, FirebaseParam query)
		{
			if (!isJson)
			{
				this.SetValue(json, query.Parameter);
			}
			else
			{
				this.SetValue(Json.Deserialize(json), query.Parameter);
			}
		}

		public void SetValue(object _val, FirebaseParam query)
		{
			this.SetValue(_val, query.Parameter);
		}

		public void UpdateValue(object _val, string param = "")
		{
			try
			{
				if (!(_val is Dictionary<string, object>))
				{
					if (this.OnUpdateFailed != null)
					{
						this.OnUpdateFailed(this, new FirebaseError(HttpStatusCode.BadRequest, "Invalid data; couldn't parse JSON object. Are you sending a JSON object with valid key names?"));
					}
				}
				else
				{
					if (this.Credential != string.Empty)
					{
						FirebaseParam firebaseParam = new FirebaseParam(param);
						param = firebaseParam.Auth(this.Credential).Parameter;
					}
					string text = this.Endpoint;
					if (param != string.Empty)
					{
						text = text + "?" + param;
					}
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("Content-Type", "application/json");
					dictionary.Add("X-HTTP-Method-Override", "PATCH");
					byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(Json.Serialize(_val));
					this.root.StartCoroutine(this.RequestCoroutine(text, bytes, dictionary, this.OnUpdateSuccess, this.OnUpdateFailed));
				}
			}
			catch (WebException webEx)
			{
				if (this.OnUpdateFailed != null)
				{
					this.OnUpdateFailed(this, FirebaseError.Create(webEx));
				}
			}
			catch (Exception ex)
			{
				if (this.OnUpdateFailed != null)
				{
					this.OnUpdateFailed(this, new FirebaseError(ex.Message));
				}
			}
		}

		public void UpdateValue(string json, bool isJson, FirebaseParam query)
		{
			if (!isJson)
			{
				this.UpdateValue(json, query.Parameter);
			}
			else
			{
				this.UpdateValue(Json.Deserialize(json), query.Parameter);
			}
		}

		public void UpdateValue(object _val, FirebaseParam query)
		{
			this.UpdateValue(_val, query.Parameter);
		}

		public void Push(string json, bool isJson, string param = "")
		{
			if (!isJson)
			{
				this.Push(json, param);
			}
			else
			{
				this.Push(Json.Deserialize(json), param);
			}
		}

		public void Push(object _val, string param = "")
		{
			try
			{
				if (this.Credential != string.Empty)
				{
					FirebaseParam firebaseParam = new FirebaseParam(param);
					param = firebaseParam.Auth(this.Credential).Parameter;
				}
				string text = this.Endpoint;
				if (param != string.Empty)
				{
					text = text + "?" + param;
				}
				byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(Json.Serialize(_val));
				this.root.StartCoroutine(this.RequestCoroutine(text, bytes, null, this.OnPushSuccess, this.OnPushFailed));
			}
			catch (WebException webEx)
			{
				if (this.OnPushFailed != null)
				{
					this.OnPushFailed(this, FirebaseError.Create(webEx));
				}
			}
			catch (Exception ex)
			{
				if (this.OnPushFailed != null)
				{
					this.OnPushFailed(this, new FirebaseError(ex.Message));
				}
			}
		}

		public void Push(string json, bool isJson, FirebaseParam query)
		{
			if (!isJson)
			{
				this.Push(json, query.Parameter);
			}
			else
			{
				this.Push(Json.Deserialize(json), query.Parameter);
			}
		}

		public void Push(object _val, FirebaseParam query)
		{
			this.Push(_val, query.Parameter);
		}

		public void Delete(string param = "")
		{
			try
			{
				if (this.Credential != string.Empty)
				{
					FirebaseParam firebaseParam = new FirebaseParam(param);
					param = firebaseParam.Auth(this.Credential).Parameter;
				}
				string text = this.Endpoint;
				if (param != string.Empty)
				{
					text = text + "?" + param;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Content-Type", "application/json");
				dictionary.Add("X-HTTP-Method-Override", "DELETE");
				byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes("{ \"dummy\" : \"dummies\"}");
				this.root.StartCoroutine(this.RequestCoroutine(text, bytes, dictionary, this.OnDeleteSuccess, this.OnDeleteFailed));
			}
			catch (WebException webEx)
			{
				if (this.OnDeleteFailed != null)
				{
					this.OnDeleteFailed(this, FirebaseError.Create(webEx));
				}
			}
			catch (Exception ex)
			{
				if (this.OnDeleteFailed != null)
				{
					this.OnDeleteFailed(this, new FirebaseError(ex.Message));
				}
			}
		}

		public void Delete(FirebaseParam query)
		{
			this.Delete(query.Parameter);
		}

		public void SetTimeStamp(string keyName)
		{
			this.Child(keyName, false).SetValue("{\".sv\": \"timestamp\"}", true, string.Empty);
		}

		public void SetTimeStamp(string keyName, Action<Firebase, DataSnapshot> OnSuccess, Action<Firebase, FirebaseError> OnFailed)
		{
			Firebase firebase = this.Child(keyName, false);
			Firebase firebase2 = firebase;
			firebase2.OnSetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase2.OnSetSuccess, OnSuccess);
			Firebase firebase3 = firebase;
			firebase3.OnSetFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase3.OnSetFailed, OnFailed);
			firebase.SetValue("{\".sv\": \"timestamp\"}", true, string.Empty);
		}

		public void GetRules(Action<Firebase, DataSnapshot> OnSuccess, Action<Firebase, FirebaseError> OnFailed, string secret = "")
		{
			try
			{
				if (string.IsNullOrEmpty(secret) && !string.IsNullOrEmpty(this.Credential))
				{
					secret = this.Credential;
				}
				string text = this.RulesEndpoint;
				text = text + "?auth=" + secret;
				this.root.StartCoroutine(this.RequestCoroutine(text, null, null, OnSuccess, OnFailed));
			}
			catch (WebException webEx)
			{
				if (OnFailed != null)
				{
					OnFailed(this, FirebaseError.Create(webEx));
				}
			}
			catch (Exception ex)
			{
				if (OnFailed != null)
				{
					OnFailed(this, new FirebaseError(ex.Message));
				}
			}
		}

		public void SetRules(string json, Action<Firebase, DataSnapshot> OnSuccess, Action<Firebase, FirebaseError> OnFailed, string secret = "")
		{
			try
			{
				if (string.IsNullOrEmpty(secret) && !string.IsNullOrEmpty(this.Credential))
				{
					secret = this.Credential;
				}
				string text = this.RulesEndpoint;
				text = text + "?auth=" + secret;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Content-Type", "application/json");
				dictionary.Add("X-HTTP-Method-Override", "PUT");
				byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(json);
				this.root.StartCoroutine(this.RequestCoroutine(text, bytes, dictionary, OnSuccess, OnFailed));
			}
			catch (WebException webEx)
			{
				if (OnFailed != null)
				{
					OnFailed(this, FirebaseError.Create(webEx));
				}
			}
			catch (Exception ex)
			{
				if (OnFailed != null)
				{
					OnFailed(this, new FirebaseError(ex.Message));
				}
			}
		}

		public void SetRules(string json, string secret = "")
		{
			this.SetRules(json, null, null, secret);
		}

		public void SetRules(Dictionary<string, object> rules, Action<Firebase, DataSnapshot> OnSuccess, Action<Firebase, FirebaseError> OnFailed, string secret = "")
		{
			this.SetRules(Json.Serialize(rules), OnSuccess, OnFailed, secret);
		}

		public void SetRules(Dictionary<string, object> rules, string secret = "")
		{
			this.SetRules(Json.Serialize(rules), null, null, secret);
		}

		protected IEnumerator RequestCoroutine(string url, byte[] postData, Dictionary<string, string> headers, Action<Firebase, DataSnapshot> OnSuccess, Action<Firebase, FirebaseError> OnFailed)
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
						OnFailed(this, new FirebaseError(status, text));
					}
				}
				else
				{
					DataSnapshot arg = new DataSnapshot(www.text);
					if (OnSuccess != null)
					{
						OnSuccess(this, arg);
					}
				}
			}
			yield break;
		}

		public static Firebase CreateNew(string host, string credential = "")
		{
			return new FirebaseRoot(host, credential);
		}

		public static DateTime TimeStampToDateTime(long unixTimeStamp)
		{
			DateTime result = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			result = result.AddMilliseconds((double)unixTimeStamp).ToLocalTime();
			return result;
		}

		private const string SERVER_VALUE_TIMESTAMP = "{\".sv\": \"timestamp\"}";

		public Action<Firebase, DataSnapshot> OnGetSuccess;

		public Action<Firebase, FirebaseError> OnGetFailed;

		public Action<Firebase, DataSnapshot> OnSetSuccess;

		public Action<Firebase, FirebaseError> OnSetFailed;

		public Action<Firebase, DataSnapshot> OnUpdateSuccess;

		public Action<Firebase, FirebaseError> OnUpdateFailed;

		public Action<Firebase, DataSnapshot> OnPushSuccess;

		public Action<Firebase, FirebaseError> OnPushFailed;

		public Action<Firebase, DataSnapshot> OnDeleteSuccess;

		public Action<Firebase, FirebaseError> OnDeleteFailed;

		protected Firebase parent;

		internal FirebaseRoot root;

		protected string key;

		protected string fullKey;
	}
}
