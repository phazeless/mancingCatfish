using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace ACE.Analytics
{
	public class Analytics : BaseBehavior
	{
		[SerializeField]
		public HashSet<string> Whitelist
		{
			get
			{
				return this.whitelist;
			}
			set
			{
				this.whitelist = value;
				if (Analytics.dispatcher != null)
				{
					Analytics.dispatcher.Whitelist = this.whitelist;
				}
			}
		}

		public static void PostEvent(IEvent evnt)
		{
			if (!Analytics.dispatcher.Post(evnt))
			{
				UnityEngine.Debug.LogWarning(string.Format("Event '{0}' not sent, it's not whitelisted", evnt.ID));
			}
		}

		public static void PostCustomEvent(string id)
		{
			Analytics.PostEvent(new CustomEvent(id));
		}

		public static void PostProgressionEvent(string id, ProgressionStatus status)
		{
			Analytics.PostEvent(new ProgressionEvent(id, status));
		}

		public static void PostRevenueEvent(string id, string cartName, string category, int amount, string currency)
		{
			Analytics.PostEvent(new RevenueEvent(id, category, amount, currency));
		}

		public static void PostResourceEvent(string id, ResourceFlow flow, string category, int amount, string currency)
		{
			Analytics.PostEvent(new ResourceEvent(id, category, flow, amount, currency));
		}

		protected override void Awake()
		{
			base.Awake();
			if (!Application.isPlaying)
			{
				return;
			}
			Analytics.dispatcher = new EventDispatcher(new GameAnalyticsService(this.gameKey, this.secretKey, Application.version, base.gameObject));
			Analytics.dispatcher.Whitelist = this.whitelist;
		}

		[SerializeField]
		private string gameKey = string.Empty;

		[SerializeField]
		private string secretKey = string.Empty;

		private HashSet<string> whitelist = new HashSet<string>();

		private static IEventDispatcher dispatcher;
	}
}
