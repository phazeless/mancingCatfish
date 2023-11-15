using System;

public interface IHasSpawnSettings
{
	void SetSpawnSettings(ISpawnSettings settings);

	ISpawnSettings GetSpawnSettings();
}
