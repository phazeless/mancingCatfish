using System;
using UnityEngine;

namespace CloudOnce.QuickStart
{
	[AddComponentMenu("CloudOnce/Initialize CloudOnce", 0)]
	public class InitializeCloudOnce : MonoBehaviour
	{
		private void Start()
		{
			Cloud.Initialize(this.cloudSaveEnabled, this.autoSignIn, this.autoCloudLoad);
		}

		[SerializeField]
		private bool cloudSaveEnabled = true;

		[SerializeField]
		private bool autoSignIn = true;

		[SerializeField]
		private bool autoCloudLoad = true;
	}
}
