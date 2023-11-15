using System;
using UnityEngine;

namespace CloudOnce.Internal
{
	public static class ConflictResolver
	{
		public static SyncableItem ResolveConflict(SyncableItem localItem, SyncableItem otherItem)
		{
			if (localItem.Metadata.PersistenceType != otherItem.Metadata.PersistenceType)
			{
				UnityEngine.Debug.LogWarning("Tried to resolve data conflict, but the two items did not have the same PersistenceType! Will use local data.");
				return localItem;
			}
			if (localItem.Metadata.DataType != otherItem.Metadata.DataType)
			{
				UnityEngine.Debug.LogWarning("Tried to resolve data conflict, but the two items did not have the same DataType! Will use local data.");
				return localItem;
			}
			SyncableItem result;
			switch (localItem.Metadata.PersistenceType)
			{
			case PersistenceType.Latest:
				result = ConflictResolver.MergeLatest(localItem, otherItem);
				break;
			case PersistenceType.Highest:
				result = ConflictResolver.MergeHighest(localItem, otherItem);
				break;
			case PersistenceType.Lowest:
				result = ConflictResolver.MergeLowest(localItem, otherItem);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		private static SyncableItem MergeLatest(SyncableItem localItem, SyncableItem otherItem)
		{
			return (localItem.Metadata.Timestamp.CompareTo(otherItem.Metadata.Timestamp) <= 0) ? otherItem : localItem;
		}

		private static SyncableItem MergeHighest(SyncableItem localItem, SyncableItem otherItem)
		{
			switch (localItem.Metadata.DataType)
			{
			case DataType.Bool:
			{
				int num;
				if (int.TryParse(otherItem.ValueString, out num))
				{
					return (num != 1) ? localItem : otherItem;
				}
				return (!Convert.ToBoolean(otherItem.ValueString)) ? localItem : otherItem;
			}
			case DataType.Double:
				return (Convert.ToDouble(localItem.ValueString) <= Convert.ToDouble(otherItem.ValueString)) ? otherItem : localItem;
			case DataType.Float:
				return (Convert.ToSingle(localItem.ValueString) <= Convert.ToSingle(otherItem.ValueString)) ? otherItem : localItem;
			case DataType.Int:
				return (Convert.ToInt32(localItem.ValueString) <= Convert.ToInt32(otherItem.ValueString)) ? otherItem : localItem;
			case DataType.String:
				return (localItem.ValueString.Length <= otherItem.ValueString.Length) ? otherItem : localItem;
			case DataType.UInt:
				return (Convert.ToUInt32(localItem.ValueString) <= Convert.ToUInt32(otherItem.ValueString)) ? otherItem : localItem;
			case DataType.Long:
				return (Convert.ToInt64(localItem.ValueString) <= Convert.ToInt64(otherItem.ValueString)) ? otherItem : localItem;
			case DataType.Decimal:
				return (!(Convert.ToDecimal(localItem.ValueString) > Convert.ToDecimal(otherItem.ValueString))) ? otherItem : localItem;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private static SyncableItem MergeLowest(SyncableItem localItem, SyncableItem otherItem)
		{
			switch (localItem.Metadata.DataType)
			{
			case DataType.Bool:
			{
				int num;
				if (int.TryParse(otherItem.ValueString, out num))
				{
					return (num != 0) ? localItem : otherItem;
				}
				return Convert.ToBoolean(otherItem.ValueString) ? localItem : otherItem;
			}
			case DataType.Double:
				return (Convert.ToDouble(localItem.ValueString) >= Convert.ToDouble(otherItem.ValueString)) ? otherItem : localItem;
			case DataType.Float:
				return (Convert.ToSingle(localItem.ValueString) >= Convert.ToSingle(otherItem.ValueString)) ? otherItem : localItem;
			case DataType.Int:
				return (Convert.ToInt32(localItem.ValueString) >= Convert.ToInt32(otherItem.ValueString)) ? otherItem : localItem;
			case DataType.String:
				return (localItem.ValueString.Length >= otherItem.ValueString.Length) ? otherItem : localItem;
			case DataType.UInt:
				return (Convert.ToUInt32(localItem.ValueString) >= Convert.ToUInt32(otherItem.ValueString)) ? otherItem : localItem;
			case DataType.Long:
				return (Convert.ToInt64(localItem.ValueString) >= Convert.ToInt64(otherItem.ValueString)) ? otherItem : localItem;
			case DataType.Decimal:
				return (!(Convert.ToDecimal(localItem.ValueString) < Convert.ToDecimal(otherItem.ValueString))) ? otherItem : localItem;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
