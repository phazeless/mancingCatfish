using System;
using System.Diagnostics;
using CloudOnce.Internal.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace CloudOnce.Internal
{
	public class CloudOnceEvents
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction OnInitializeComplete;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction<bool> OnSignedInChanged;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction OnSignInFailed;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction<Texture2D> OnPlayerImageDownloaded;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction<bool> OnCloudSaveComplete;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction<bool> OnCloudLoadComplete;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction<string[]> OnNewCloudValues;

		public void RaiseOnInitializeComplete()
		{
			CloudOnceUtils.SafeInvoke(this.OnInitializeComplete);
		}

		public void RaiseOnSignedInChanged(bool isSignedIn)
		{
			CloudOnceUtils.SafeInvoke<bool>(this.OnSignedInChanged, isSignedIn);
		}

		public void RaiseOnSignInFailed()
		{
			CloudOnceUtils.SafeInvoke(this.OnSignInFailed);
		}

		public void RaiseOnPlayerImageDownloaded(Texture2D playerImage)
		{
			CloudOnceUtils.SafeInvoke<Texture2D>(this.OnPlayerImageDownloaded, playerImage);
		}

		public void RaiseOnCloudSaveComplete(bool success)
		{
			CloudOnceUtils.SafeInvoke<bool>(this.OnCloudSaveComplete, success);
		}

		public void RaiseOnCloudLoadComplete(bool success)
		{
			CloudOnceUtils.SafeInvoke<bool>(this.OnCloudLoadComplete, success);
		}

		public void RaiseOnNewCloudValues(string[] changedKeys)
		{
			CloudOnceUtils.SafeInvoke<string[]>(this.OnNewCloudValues, changedKeys);
		}
	}
}
