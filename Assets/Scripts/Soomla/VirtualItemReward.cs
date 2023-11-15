using System;
using Soomla.Store;

namespace Soomla
{
	public class VirtualItemReward : Reward
	{
		public VirtualItemReward(string rewardId, string name, string associatedItemId, int amount) : base(rewardId, name)
		{
			this.AssociatedItemId = associatedItemId;
			this.Amount = amount;
		}

		public VirtualItemReward(JSONObject jsonReward) : base(jsonReward)
		{
			this.AssociatedItemId = jsonReward["associatedItemId"].str;
			this.Amount = (int)jsonReward["amount"].n;
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jsonobject = base.toJSONObject();
			jsonobject.AddField("associatedItemId", this.AssociatedItemId);
			jsonobject.AddField("amount", this.Amount);
			jsonobject.AddField("className", base.GetType().Name);
			return jsonobject;
		}

		protected override bool giveInner()
		{
			try
			{
				StoreInventory.GiveItem(this.AssociatedItemId, this.Amount);
			}
			catch (VirtualItemNotFoundException ex)
			{
				SoomlaUtils.LogError(VirtualItemReward.TAG, "(give) Couldn't find associated itemId: " + this.AssociatedItemId);
				SoomlaUtils.LogError(VirtualItemReward.TAG, ex.Message);
				return false;
			}
			return true;
		}

		protected override bool takeInner()
		{
			try
			{
				StoreInventory.TakeItem(this.AssociatedItemId, this.Amount);
			}
			catch (VirtualItemNotFoundException ex)
			{
				SoomlaUtils.LogError(VirtualItemReward.TAG, "(take) Couldn't find associated itemId: " + this.AssociatedItemId);
				SoomlaUtils.LogError(VirtualItemReward.TAG, ex.Message);
				return false;
			}
			return true;
		}

		private static string TAG = "SOOMLA VirtualItemReward";

		public string AssociatedItemId;

		public int Amount;
	}
}
