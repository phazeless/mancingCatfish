using System;
using UnityEngine;

public interface IGrantable
{
	Sprite Icon { get; }

	string Title { get; }

	string Description { get; }

	void Grant(string contentIdForAnalytics, ResourceChangeReason resourceChangeReason);
}
