using System;
using System.Collections;
using UnityEngine;

namespace SimpleFirebaseUnity
{
	public class FirebaseObserver
	{
		public FirebaseObserver(Firebase _firebase, float _refreshRate, string _getParam = "")
		{
			this.active = false;
			this.lastSnapshot = null;
			this.firebase = _firebase;
			this.refreshRate = _refreshRate;
			this.getParam = _getParam;
			this.target = _firebase.Copy(false);
			this.routine = null;
		}

		public FirebaseObserver(Firebase _firebase, float _refreshRate, FirebaseParam _getParam)
		{
			this.active = false;
			this.lastSnapshot = null;
			this.firebase = _firebase;
			this.refreshRate = _refreshRate;
			this.getParam = _getParam.Parameter;
			this.target = _firebase.Copy(false);
		}

		public void Start()
		{
			if (this.routine != null)
			{
				this.Stop();
			}
			this.active = true;
			this.firstTime = true;
			Firebase firebase = this.target;
			firebase.OnGetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase.OnGetSuccess, new Action<Firebase, DataSnapshot>(this.CompareSnapshot));
			this.routine = this.RefreshCoroutine();
			this.target.root.StartCoroutine(this.routine);
		}

		public void Stop()
		{
			this.active = false;
			Firebase firebase = this.target;
			firebase.OnGetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Remove(firebase.OnGetSuccess, new Action<Firebase, DataSnapshot>(this.CompareSnapshot));
			this.lastSnapshot = null;
			if (this.routine != null)
			{
				this.target.root.StopCoroutine(this.routine);
				this.routine = null;
			}
		}

		private IEnumerator RefreshCoroutine()
		{
			while (this.active)
			{
				this.target.GetValue(string.Empty);
				yield return new WaitForSeconds(this.refreshRate);
			}
			yield break;
		}

		private void CompareSnapshot(Firebase target, DataSnapshot snapshot)
		{
			if (this.firstTime)
			{
				this.firstTime = false;
				this.lastSnapshot = snapshot;
				return;
			}
			if (!snapshot.RawJson.Equals(this.lastSnapshot.RawJson) && this.OnChange != null)
			{
				this.OnChange(this.firebase, snapshot);
			}
			this.lastSnapshot = snapshot;
		}

		public Action<Firebase, DataSnapshot> OnChange;

		protected Firebase firebase;

		protected Firebase target;

		protected float refreshRate;

		protected string getParam;

		protected bool active;

		protected bool firstTime;

		protected DataSnapshot lastSnapshot;

		protected IEnumerator routine;
	}
}
