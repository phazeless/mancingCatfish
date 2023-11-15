using System;

public class WaterEffectPool : ObjectPool<WaterEffect>
{
	public WaterEffectPool(Func<object, WaterEffect> instantiator, int initialSize, WaterEffect prefab) : base(instantiator, initialSize, prefab)
	{
	}
}
