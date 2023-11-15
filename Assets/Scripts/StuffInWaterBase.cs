using System;
using UnityEngine;

public class StuffInWaterBase : MonoBehaviour, INotifyOutOfScreen, IHasSpeed, IHasSpawnSettings
{
	public float ActualSpeed
	{
		get
		{
			return this.spawnSettings.GetSpeed();
		}
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		this.OnSwiped();
	}

	public void OnOutOfScreen(NotifyOutOfScreen.OutOfScreenMethod outOfScreenMethod, NotifyOutOfScreen.ListenerMode listenerMode, GameObject gameObject)
	{
		UnityEngine.Object.Destroy(gameObject);
	}

	public void SetSpawnSettings(ISpawnSettings settings)
	{
		this.spawnSettings = settings;
	}

	public ISpawnSettings GetSpawnSettings()
	{
		return this.spawnSettings;
	}

	protected virtual void OnSwiped()
	{
		InGameNotification ign = this.spawnSettings.GetIGN();
		if (ign != null)
		{
			AudioManager.Instance.PickupStuffFromWater();
			InGameNotificationManager.Instance.Create<InGameNotification>(ign);
			Transform transform = UnityEngine.Object.Instantiate<Transform>(this.pickupSplash, base.transform.root, false);
			transform.position = base.transform.position;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected ISpawnSettings spawnSettings;

	[SerializeField]
	protected Transform pickupSplash;
}
