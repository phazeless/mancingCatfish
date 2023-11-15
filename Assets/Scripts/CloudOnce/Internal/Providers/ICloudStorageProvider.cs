using System;

namespace CloudOnce.Internal.Providers
{
	public interface ICloudStorageProvider
	{
		void Save();

		void Load();

		void Synchronize();

		bool ResetVariable(string key);

		bool DeleteVariable(string key);

		string[] ClearUnusedVariables();

		void DeleteAll();
	}
}
