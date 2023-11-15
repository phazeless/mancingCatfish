using System;

namespace Soomla.Store
{
	public abstract class VirtualItem : SoomlaEntity<VirtualItem>
	{
		protected VirtualItem(string name, string description, string itemId) : base(itemId, name, description)
		{
		}

		protected VirtualItem(JSONObject jsonItem) : base(jsonItem)
		{
		}

		public string ItemId
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj.GetType() == base.GetType() && ((VirtualItem)obj).ItemId == this.ItemId;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public int Give(int amount)
		{
			return this.Give(amount, true);
		}

		public abstract int Give(int amount, bool notify);

		public int Take(int amount)
		{
			return this.Take(amount, true);
		}

		public abstract int Take(int amount, bool notify);

		public int ResetBalance(int balance)
		{
			return this.ResetBalance(balance, true);
		}

		public abstract int ResetBalance(int balance, bool notify);

		public abstract int GetBalance();

		public void Save(bool saveToDB = true)
		{
			StoreInfo.Save(this, saveToDB);
		}

		private const string TAG = "SOOMLA VirtualItem";
	}
}
