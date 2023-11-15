using System;
using System.Collections;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace SimpleFirebaseUnity
{
	internal class FirebaseRoot : Firebase
	{
		public FirebaseRoot(string _host, string _cred = "")
		{
			if (FirebaseRoot.firstTimeInitiated)
			{
				Delegate serverCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback;
				if (FirebaseRoot._003C_003Ef__mg_0024cache0 == null)
				{
					FirebaseRoot._003C_003Ef__mg_0024cache0 = new RemoteCertificateValidationCallback(FirebaseRoot.RemoteCertificateValidationCallback);
				}
				ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(serverCertificateValidationCallback, FirebaseRoot._003C_003Ef__mg_0024cache0);
				FirebaseRoot.firstTimeInitiated = false;
			}
			this.root = this;
			this.host = _host;
			this.cred = _cred;
		}

		public override string Endpoint
		{
			get
			{
				return "https://" + this.root.Host + "/.json";
			}
		}

		public override string Credential
		{
			get
			{
				return this.cred;
			}
			set
			{
				this.cred = value;
			}
		}

		public override string RulesEndpoint
		{
			get
			{
				return "https://" + this.root.Host + "/.settings/rules.json";
			}
		}

		public FirebaseRoot Copy()
		{
			return new FirebaseRoot(this.host, this.cred);
		}

		public override string Host
		{
			get
			{
				return this.host;
			}
		}

		private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public void StartCoroutine(IEnumerator routine)
		{
			FirebaseManager.Instance.StartCoroutine(routine);
		}

		public void StopCoroutine(IEnumerator routine)
		{
			FirebaseManager.Instance.StopCoroutine(routine);
		}

		protected static bool firstTimeInitiated = true;

		protected string host;

		protected string cred;

		[CompilerGenerated]
		private static RemoteCertificateValidationCallback _003C_003Ef__mg_0024cache0;
	}
}
