using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer
{
	public class fsAotConfiguration : ScriptableObject
	{
		public bool TryFindEntry(Type type, out fsAotConfiguration.Entry result)
		{
			string fullName = type.FullName;
			foreach (fsAotConfiguration.Entry entry in this.aotTypes)
			{
				if (entry.FullTypeName == fullName)
				{
					result = entry;
					return true;
				}
			}
			result = default(fsAotConfiguration.Entry);
			return false;
		}

		public void UpdateOrAddEntry(fsAotConfiguration.Entry entry)
		{
			for (int i = 0; i < this.aotTypes.Count; i++)
			{
				if (this.aotTypes[i].FullTypeName == entry.FullTypeName)
				{
					this.aotTypes[i] = entry;
					return;
				}
			}
			this.aotTypes.Add(entry);
		}

		public List<fsAotConfiguration.Entry> aotTypes = new List<fsAotConfiguration.Entry>();

		public string outputDirectory = "Assets/AotModels";

		public enum AotState
		{
			Default,
			Enabled,
			Disabled
		}

		[Serializable]
		public struct Entry
		{
			public Entry(Type type)
			{
				this.FullTypeName = type.FullName;
				this.State = fsAotConfiguration.AotState.Default;
			}

			public Entry(Type type, fsAotConfiguration.AotState state)
			{
				this.FullTypeName = type.FullName;
				this.State = state;
			}

			public fsAotConfiguration.AotState State;

			public string FullTypeName;
		}
	}
}
