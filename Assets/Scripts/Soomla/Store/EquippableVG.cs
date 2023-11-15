using System;

namespace Soomla.Store
{
	public class EquippableVG : LifetimeVG
	{
		public EquippableVG(EquippableVG.EquippingModel equippingModel, string name, string description, string itemId, PurchaseType purchaseType) : base(name, description, itemId, purchaseType)
		{
			this.Equipping = equippingModel;
		}

		public EquippableVG(JSONObject jsonItem) : base(jsonItem)
		{
			string str = jsonItem["equipping"].str;
			this.Equipping = EquippableVG.EquippingModel.CATEGORY;
			if (str != null)
			{
				if (str == "local")
				{
					this.Equipping = EquippableVG.EquippingModel.LOCAL;
					return;
				}
				if (str == "global")
				{
					this.Equipping = EquippableVG.EquippingModel.GLOBAL;
					return;
				}
			}
			this.Equipping = EquippableVG.EquippingModel.CATEGORY;
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jsonobject = base.toJSONObject();
			jsonobject.AddField("equipping", this.Equipping.ToString());
			return jsonobject;
		}

		public void Equip()
		{
			this.Equip(true);
		}

		public void Equip(bool notify)
		{
			if (VirtualGoodsStorage.GetBalance(this) > 0)
			{
				if (this.Equipping == EquippableVG.EquippingModel.CATEGORY)
				{
					VirtualCategory virtualCategory = null;
					try
					{
						virtualCategory = StoreInfo.GetCategoryForVirtualGood(base.ItemId);
					}
					catch (VirtualItemNotFoundException)
					{
						SoomlaUtils.LogError(EquippableVG.TAG, "Tried to unequip all other category VirtualGoods but there was no associated category. virtual good itemId: " + base.ItemId);
						return;
					}
					foreach (string text in virtualCategory.GoodItemIds)
					{
						try
						{
							EquippableVG equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(text);
							if (equippableVG != null && equippableVG != this)
							{
								equippableVG.Unequip(notify);
							}
						}
						catch (VirtualItemNotFoundException)
						{
							SoomlaUtils.LogError(EquippableVG.TAG, "On equip, couldn't find one of the itemIds in the category. Continuing to the next one. itemId: " + text);
						}
						catch (InvalidCastException)
						{
							SoomlaUtils.LogDebug(EquippableVG.TAG, "On equip, an error occurred. It's a debug message b/c the VirtualGood may just not be an EquippableVG. itemId: " + text);
						}
					}
				}
				else if (this.Equipping == EquippableVG.EquippingModel.GLOBAL)
				{
					foreach (VirtualGood virtualGood in StoreInfo.Goods)
					{
						if (virtualGood != this && virtualGood is EquippableVG)
						{
							((EquippableVG)virtualGood).Unequip(notify);
						}
					}
				}
				VirtualGoodsStorage.Equip(this, notify);
				return;
			}
			throw new NotEnoughGoodsException(base.ItemId);
		}

		public void Unequip()
		{
			this.Unequip(true);
		}

		public void Unequip(bool notify)
		{
			VirtualGoodsStorage.UnEquip(this, notify);
		}

		private static string TAG = "SOOMLA EquippableVG";

		public EquippableVG.EquippingModel Equipping;

		public sealed class EquippingModel
		{
			private EquippingModel(int value, string name)
			{
				this.name = name;
				this.value = value;
			}

			public override string ToString()
			{
				return this.name;
			}

			public int toInt()
			{
				return this.value;
			}

			private readonly string name;

			private readonly int value;

			public static readonly EquippableVG.EquippingModel LOCAL = new EquippableVG.EquippingModel(0, "local");

			public static readonly EquippableVG.EquippingModel CATEGORY = new EquippableVG.EquippingModel(1, "category");

			public static readonly EquippableVG.EquippingModel GLOBAL = new EquippableVG.EquippingModel(2, "global");
		}
	}
}
