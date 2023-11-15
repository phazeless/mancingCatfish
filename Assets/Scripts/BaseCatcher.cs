using System;
using System.Diagnostics;
using System.Numerics;
using FullInspector;
using UnityEngine;

public abstract class BaseCatcher : BaseBehavior, IHasGPM
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<FishBehaviour> OnFishCollected;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<UnityEngine.Vector3> OnFishDropped;

	protected override void Awake()
	{
		base.Awake();
		AFKManager.Instance.RegisterGPMGenerator(this);
	}

	private void OnDestroy()
	{
		AFKManager.Instance.UnregisterGPMGenerator(this);
	}

	protected virtual void Update()
	{
		this.InternalCheckForCatch(false);
	}

	protected abstract FishBehaviour OnCheckForCatch(bool isSimulation = false);

	protected abstract void OnFishCaught(FishBehaviour fish);

	protected void NotifyFishDropped()
	{
		if (BaseCatcher.OnFishDropped != null)
		{
			BaseCatcher.OnFishDropped(base.transform.position);
		}
	}

	public static void NotifyFishCollected(FishBehaviour fishCollected)
	{
		if (BaseCatcher.OnFishCollected != null)
		{
			BaseCatcher.OnFishCollected(fishCollected);
		}
	}

	private FishBehaviour InternalCheckForCatch(bool isSimulation = false)
	{
		FishBehaviour fishBehaviour = this.OnCheckForCatch(isSimulation);
		if (!isSimulation && fishBehaviour != null)
		{
			this.OnFishCaught(fishBehaviour);
			if (BaseCatcher.OnFishCollected != null)
			{
				BaseCatcher.OnFishCollected(fishBehaviour);
			}
		}
		return fishBehaviour;
	}

	public BigInteger GetGPM()
	{
		BigInteger bigInteger = 0;
		for (int i = 0; i < 3600; i++)
		{
			FishBehaviour fishBehaviour = this.InternalCheckForCatch(true);
			if (fishBehaviour != null)
			{
				bigInteger += fishBehaviour.GetValue(true);
			}
		}
		return bigInteger;
	}
}
