using System;

namespace CloudOnce.Internal.Providers
{
	public class DummyStorageWrapper : ICloudStorageProvider
	{
		public DummyStorageWrapper(CloudOnceEvents events)
		{
			this.cloudOnceEvents = events;
		}

		public void Save()
		{
			DataManager.SaveToDisk();
			this.cloudOnceEvents.RaiseOnCloudSaveComplete(false);
		}

		public void Load()
		{
			this.cloudOnceEvents.RaiseOnCloudLoadComplete(false);
		}

		public void Synchronize()
		{
			this.Load();
			this.Save();
		}

		public bool ResetVariable(string key)
		{
			return DataManager.ResetCloudPref(key);
		}

		public bool DeleteVariable(string key)
		{
			return DataManager.DeleteCloudPref(key);
		}

		public string[] ClearUnusedVariables()
		{
			return DataManager.ClearStowawayVariablesFromGameData();
		}

		public void DeleteAll()
		{
			DataManager.DeleteAllCloudVariables();
		}

		private readonly CloudOnceEvents cloudOnceEvents;
	}
}
