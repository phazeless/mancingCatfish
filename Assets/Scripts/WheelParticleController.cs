using System;
using UnityEngine;

public class WheelParticleController : MonoBehaviour
{
	public void SetWheelEffect(WheelRewardAndChances.Reward reward)
	{
		if (reward == WheelRewardAndChances.Reward.Jackpot)
		{
			this.yellowParticle.Play();
			this.greenParticle.Play();
		}
		this.purpGreenParticle.Play();
		this.whiteParticle.Play();
	}

	[SerializeField]
	private ParticleSystem purpGreenParticle;

	[SerializeField]
	private ParticleSystem yellowParticle;

	[SerializeField]
	private ParticleSystem whiteParticle;

	[SerializeField]
	private ParticleSystem greenParticle;
}
