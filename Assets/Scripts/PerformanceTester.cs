using System;
using UnityEngine;

public class PerformanceTester : MonoBehaviour
{
	public void DisableShader()
	{
		this.shader.SetActive(!this.shader.activeInHierarchy);
	}

	public void DisableUpgradeParticles()
	{
		BoatUpgradeEffectElement.tempParticleDisabler = !BoatUpgradeEffectElement.tempParticleDisabler;
	}

	public void DisableUpgradeTint()
	{
		BoatUpgradeEffectElement.tempTintDisabler = !BoatUpgradeEffectElement.tempTintDisabler;
	}

	public GameObject shader;
}
