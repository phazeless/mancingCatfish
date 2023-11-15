using System;
using SimpleFirebaseUnity.MiniJSON;

namespace SimpleFirebaseUnity
{
	public class FirebaseQueue
	{
		public FirebaseQueue(bool _autoStart = true)
		{
			this.autoStart = _autoStart;
			this.count = 0;
		}

		protected void AddQueue(Firebase firebase, FirebaseQueue.FirebaseCommand command, string param, object obj = null)
		{
			FirebaseQueue.CommandLinkedList next = new FirebaseQueue.CommandLinkedList(firebase, command, param, obj);
			if (this.head == null)
			{
				this.head = next;
				this.tail = next;
				if (this.autoStart)
				{
					this.head.DoCommand();
				}
			}
			else
			{
				this.tail.next = next;
				this.tail = next;
			}
			this.count++;
		}

		protected void ClearQueueTopDown(FirebaseQueue.CommandLinkedList node)
		{
			FirebaseQueue.CommandLinkedList next = node.next;
			node.next = null;
			this.ClearQueueTopDown(next);
		}

		protected void StartNextCommand()
		{
			this.head = this.head.next;
			if (this.head != null)
			{
				this.head.DoCommand();
			}
			else
			{
				this.tail = null;
				if (this.OnQueueFinished != null)
				{
					this.OnQueueFinished();
				}
			}
		}

		protected void OnSuccess(Firebase sender, DataSnapshot snapshot)
		{
			this.count--;
			this.StartNextCommand();
			this.ClearCallbacks(sender);
		}

		protected void OnFailed(Firebase sender, FirebaseError err)
		{
			this.count--;
			this.StartNextCommand();
			this.ClearCallbacks(sender);
		}

		protected void ClearCallbacks(Firebase sender)
		{
			sender.OnGetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Remove(sender.OnGetSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			sender.OnGetFailed = (Action<Firebase, FirebaseError>)Delegate.Remove(sender.OnGetFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			sender.OnUpdateSuccess = (Action<Firebase, DataSnapshot>)Delegate.Remove(sender.OnUpdateSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			sender.OnUpdateFailed = (Action<Firebase, FirebaseError>)Delegate.Remove(sender.OnUpdateFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			sender.OnPushSuccess = (Action<Firebase, DataSnapshot>)Delegate.Remove(sender.OnPushSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			sender.OnPushFailed = (Action<Firebase, FirebaseError>)Delegate.Remove(sender.OnPushFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			sender.OnDeleteSuccess = (Action<Firebase, DataSnapshot>)Delegate.Remove(sender.OnDeleteSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			sender.OnDeleteFailed = (Action<Firebase, FirebaseError>)Delegate.Remove(sender.OnDeleteFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public bool IsEmpty()
		{
			return this.count == 0;
		}

		public void AddQueueGet(Firebase firebase, string param = "")
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnGetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnGetSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnGetFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnGetFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Get, param, null);
		}

		public void AddQueueGet(Firebase firebase, FirebaseParam param)
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnGetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnGetSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnGetFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnGetFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Get, param.Parameter, null);
		}

		public void AddQueueSet(Firebase firebase, object val, string param = "")
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnSetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnSetSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnSetFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnSetFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Set, param, val);
		}

		public void AddQueueSet(Firebase firebase, object val, FirebaseParam param)
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnSetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnSetSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnSetFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnSetFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Set, param.Parameter, val);
		}

		public void AddQueueSet(Firebase firebase, string json, bool isJson, string param = "")
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnSetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnSetSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnSetFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnSetFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			if (!isJson)
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Set, param, json);
			}
			else
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Set, param, Json.Deserialize(json));
			}
		}

		public void AddQueueSet(Firebase firebase, string json, bool isJson, FirebaseParam param)
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnSetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnSetSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnSetFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnSetFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			if (!isJson)
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Set, param.Parameter, json);
			}
			else
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Set, param.Parameter, Json.Deserialize(json));
			}
		}

		public void AddQueueUpdate(Firebase firebase, object val, string param = "")
		{
			firebase.OnUpdateSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase.OnUpdateSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			firebase.OnUpdateFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase.OnUpdateFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			this.AddQueue(firebase, FirebaseQueue.FirebaseCommand.Update, param, val);
		}

		public void AddQueueUpdate(Firebase firebase, object val, FirebaseParam param)
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnUpdateSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnUpdateSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnUpdateFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnUpdateFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Update, param.Parameter, val);
		}

		public void AddQueueUpdate(Firebase firebase, string json, bool isJson, string param = "")
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnUpdateSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnUpdateSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnUpdateFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnUpdateFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			if (!isJson)
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Update, param, json);
			}
			else
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Update, param, Json.Deserialize(json));
			}
		}

		public void AddQueueUpdate(Firebase firebase, string json, bool isJson, FirebaseParam param)
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnUpdateSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnUpdateSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnUpdateFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnUpdateFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			if (!isJson)
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Update, param.Parameter, json);
			}
			else
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Update, param.Parameter, Json.Deserialize(json));
			}
		}

		public void AddQueuePush(Firebase firebase, object val, string param = "")
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnPushSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnPushSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnPushFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnPushFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Push, param, val);
		}

		public void AddQueuePush(Firebase firebase, object val, FirebaseParam param)
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnPushSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnPushSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnPushFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnPushFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Push, param.Parameter, val);
		}

		public void AddQueuePush(Firebase firebase, string json, bool isJson, string param = "")
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnPushSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnPushSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnPushFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnPushFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			if (!isJson)
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Push, param, json);
			}
			else
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Push, param, Json.Deserialize(json));
			}
		}

		public void AddQueuePush(Firebase firebase, string json, bool isJson, FirebaseParam param)
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnPushSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnPushSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnPushFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnPushFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			if (!isJson)
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Push, param.Parameter, json);
			}
			else
			{
				this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Push, param.Parameter, Json.Deserialize(json));
			}
		}

		public void AddQueueDelete(Firebase firebase, string param = "")
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnDeleteSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnDeleteSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnDeleteFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnDeleteFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Delete, param, null);
		}

		public void AddQueueDelete(Firebase firebase, FirebaseParam param)
		{
			Firebase firebase2 = firebase.Copy(true);
			Firebase firebase3 = firebase2;
			firebase3.OnDeleteSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnDeleteSuccess, new Action<Firebase, DataSnapshot>(this.OnSuccess));
			Firebase firebase4 = firebase2;
			firebase4.OnDeleteFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnDeleteFailed, new Action<Firebase, FirebaseError>(this.OnFailed));
			this.AddQueue(firebase2, FirebaseQueue.FirebaseCommand.Delete, param.Parameter, null);
		}

		public void AddQueueSetTimeStamp(Firebase firebase, string keyName)
		{
			Firebase firebase2 = firebase.Child(keyName, false);
			this.AddQueueSet(firebase2, "{\".sv\": \"timestamp\"}", true, "print=silent");
		}

		public void AddQueueSetTimeStamp(Firebase firebase, string keyName, Action<Firebase, DataSnapshot> _OnSuccess, Action<Firebase, FirebaseError> _OnFailed)
		{
			Firebase firebase2 = firebase.Child(keyName, false);
			Firebase firebase3 = firebase2;
			firebase3.OnSetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnSetSuccess, _OnSuccess);
			Firebase firebase4 = firebase2;
			firebase4.OnSetFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase4.OnSetFailed, _OnFailed);
			this.AddQueueSet(firebase2, "{\".sv\": \"timestamp\"}", true, string.Empty);
		}

		public void ForceClearQueue()
		{
			this.ClearQueueTopDown(this.head);
		}

		private const string SERVER_VALUE_TIMESTAMP = "{\".sv\": \"timestamp\"}";

		public Action OnQueueFinished;

		protected FirebaseQueue.CommandLinkedList head;

		protected FirebaseQueue.CommandLinkedList tail;

		protected bool autoStart;

		protected int count;

		protected enum FirebaseCommand
		{
			Get,
			Set,
			Update,
			Push,
			Delete
		}

		protected class CommandLinkedList
		{
			public CommandLinkedList(Firebase _firebase, FirebaseQueue.FirebaseCommand _command, string _param, object _obj = null)
			{
				this.firebase = _firebase;
				this.command = _command;
				this.param = _param;
				this.obj = _obj;
				this.next = null;
			}

			public CommandLinkedList(Firebase _firebase, FirebaseQueue.FirebaseCommand _command, FirebaseParam firebaseParam, object _obj = null)
			{
				this.firebase = _firebase;
				this.command = _command;
				this.param = firebaseParam.Parameter;
				this.obj = _obj;
				this.next = null;
			}

			public void AddNext(FirebaseQueue.CommandLinkedList _next)
			{
				this.next = _next;
			}

			public void DoCommand()
			{
				switch (this.command)
				{
				case FirebaseQueue.FirebaseCommand.Get:
					this.firebase.GetValue(this.param);
					break;
				case FirebaseQueue.FirebaseCommand.Set:
					this.firebase.SetValue(this.obj, this.param);
					break;
				case FirebaseQueue.FirebaseCommand.Update:
					this.firebase.UpdateValue(this.obj, this.param);
					break;
				case FirebaseQueue.FirebaseCommand.Push:
					this.firebase.Push(this.obj, this.param);
					break;
				case FirebaseQueue.FirebaseCommand.Delete:
					this.firebase.Delete(this.param);
					break;
				}
			}

			public Firebase firebase;

			private FirebaseQueue.FirebaseCommand command;

			private string param;

			private object obj;

			public FirebaseQueue.CommandLinkedList next;
		}
	}
}
