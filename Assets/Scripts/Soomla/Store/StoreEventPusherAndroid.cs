using System;
using UnityEngine;

namespace Soomla.Store
{
	public class StoreEventPusherAndroid : StoreEvents.StoreEventPusher
	{
		protected override void _pushEventSoomlaStoreInitialized(string message)
		{
			this.pushEvent("SoomlaStoreInitialized", message);
		}

		protected override void _pushEventUnexpectedStoreError(string message)
		{
			this.pushEvent("UnexpectedStoreError", message);
		}

		protected override void _pushEventCurrencyBalanceChanged(string message)
		{
			this.pushEvent("SoomlaStoreInitialized", message);
		}

		protected override void _pushEventGoodBalanceChanged(string message)
		{
			this.pushEvent("CurrencyBalanceChanged", message);
		}

		protected override void _pushEventGoodEquipped(string message)
		{
			this.pushEvent("GoodEquipped", message);
		}

		protected override void _pushEventGoodUnequipped(string message)
		{
			this.pushEvent("GoodUnequipped", message);
		}

		protected override void _pushEventGoodUpgrade(string message)
		{
			this.pushEvent("GoodUpgrade", message);
		}

		protected override void _pushEventItemPurchased(string message)
		{
			this.pushEvent("ItemPurchased", message);
		}

		protected override void _pushEventItemPurchaseStarted(string message)
		{
			this.pushEvent("ItemPurchaseStarted", message);
		}

		private void pushEvent(string name, string message)
		{
			AndroidJNI.PushLocalFrame(100);
			
		}
	}
}
