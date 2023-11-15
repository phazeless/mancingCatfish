using System;
using DG.Tweening;
using UnityEngine;

public class BoatUpgradeEffectElement : MonoBehaviour
{
	private void Start()
	{
		SkillManager.Instance.OnSkillLevelChanged += this.Instance_OnSkillLevelChanged;
	}

	private void Instance_OnSkillLevelChanged(Skill skill, LevelChange arg2)
	{
		if (skill.IsTierSkill && arg2 == LevelChange.LevelUp)
		{
			this.upgradeEffect();
		}
	}

	private void upgradeEffect()
	{
		if (!BoatUpgradeEffectElement.tempParticleDisabler)
		{
			this.ParticleCreation();
		}
		if (!BoatUpgradeEffectElement.tempTintDisabler)
		{
			this.BoatHighlighting();
		}
		this.ScaleEffect();
	}

	private void ScaleEffect()
	{
		this.transformToScale.DOKill(true);
		this.transformToScale.DOPunchScale(new Vector3(0.05f, 0.05f, 0f), 0.5f, 10, 1f);
	}

	private void ParticleCreation()
	{
		if (this.isParticleEmitting)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.upgradeParticleEffect);
			gameObject.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
		}
	}

	private void BoatHighlighting()
	{
		for (int i = 0; i < this.partsToHighlight.Length; i++)
		{
			this.partsToHighlight[i].DOKill(true);
			this.partsToHighlight[i].DOColor(BoatUpgradeEffectElement.UPGRADE_GLIMMER_COLOR, 0.3f).From<Tweener>();
		}
	}

	private void OnDestroy()
	{
		SkillManager.Instance.OnSkillLevelChanged -= this.Instance_OnSkillLevelChanged;
		this.transformToScale.DOKill(false);
		for (int i = 0; i < this.partsToHighlight.Length; i++)
		{
			this.partsToHighlight[i].DOKill(false);
		}
	}

	private static Color UPGRADE_GLIMMER_COLOR = new Color(1f, 1f, 1f);

	public GameObject upgradeParticleEffect;

	public Transform transformToScale;

	public SpriteRenderer[] partsToHighlight;

	public static bool tempParticleDisabler = false;

	public static bool tempTintDisabler = false;

	public bool isParticleEmitting = true;
}
