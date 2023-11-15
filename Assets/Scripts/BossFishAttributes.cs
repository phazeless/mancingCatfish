using System;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BossFishAttributes : FishBehaviour
{
	protected override void Awake()
	{
		base.Awake();
		this.hpBar = base.GetComponentInChildren<UIMeterBigInteger>();
	}

	private void Start()
	{
		this.startColor = this.spriteRenderers[0].color;
		this.startScale = base.transform.localScale;
		this.hpBar.SetMax(this.hp.Value);
	}

	public override bool IsCaught()
	{
		return this.totalDmgRecieved >= this.hp.Value;
	}

	public override void OnSwiped(CircleCatcher catcher)
	{
		bool flag = catcher is FireworkCatcher;
		Color color = Color.white;
		float num = 1f;
		float num2 = 1f;
		bool flag2 = this.DetermineIfCriticalBossDamage();
		if (flag2)
		{
			num2 = SkillManager.Instance.GetCurrentTotalValueFor<Skills.CriticalStrikeBossDamageMultiplier>();
			color = Color.red;
			num = 1.4f;
		}
		float value = SkillManager.Instance.GetCurrentTotalValueFor<Skills.BossDamageModifier>() * num2;
		BigInteger bigInteger = (this.BASE_DMG + SkillManager.Instance.PrestigeSkill.CurrentLevelAsLong).MultiplyFloat(value);
		bigInteger = bigInteger.MultiplyFloat(UnityEngine.Random.Range(0.9f, 1.1f));
		if (flag)
		{
			BigInteger right = (BigInteger)SkillManager.Instance.GetCurrentTotalValueFor<Skills.FireworkBossDamageMultiplier>();
			bigInteger *= right;
		}
		this.totalDmgRecieved += bigInteger;
		TextMeshPro txt = TextObjectPool.Instance.TextMeshProPool.GetObject();
		txt.color = color;
		txt.gameObject.SetActive(true);
		txt.SetText(bigInteger.ToString());
		txt.fontSize = 6f * num;
		txt.alignment = TextAlignmentOptions.Midline;
		txt.transform.position = base.transform.position;
		txt.transform.DOMoveY(txt.transform.position.y + 1f, 0.5f, false);
		txt.DOColor(new Color(1f, 1f, 1f, 0f), 0.7f).OnComplete(delegate
		{
			txt.gameObject.SetActive(false);
			TextObjectPool.Instance.TextMeshProPool.ReturnObject(txt);
		});
		if (this.totalDmgRecieved < this.hp.Value)
		{
			AudioManager.Instance.FishJump();
			this.hpBar.SetCurrent(this.totalDmgRecieved);
			SpriteRenderer[] array = this.spriteRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				SpriteRenderer sr = array[i];
				sr.DOColor(Color.white, 0.2f).OnComplete(delegate
				{
					sr.color = this.startColor;
				});
			}
			base.transform.DOKill(true);
			base.transform.DOPunchScale(new UnityEngine.Vector3(0.1f, 0.1f, 0f), 0.2f, 10, 1f);
			HookedVibration.BossFishSwipeHaptic();
		}
		else
		{
			AudioManager.Instance.BossTimeEnd();
			ColorChangerHandler.BossmodeColorEnd();
			CameraMovement.Instance.BossTimeZoomEnd();
			this.hpBar.SetCurrent(this.totalDmgRecieved);
			base.transform.DOKill(true);
			HookedVibration.BossFishCaughtHaptic();
		}
	}

	public void Reset()
	{
		this.totalDmgRecieved = 0;
		this.hpBar.transform.localScale = UnityEngine.Vector3.one;
		this.hpBar.SetMax(this.hp.Value);
		this.hpBar.SetCurrent(this.totalDmgRecieved);
	}

	private void Update()
	{
		this.hpBar.transform.eulerAngles = UnityEngine.Vector3.zero;
		this.hpBar.transform.position = base.transform.position + new UnityEngine.Vector3(0f, 0f, 0f);
	}

	public override void OnOutOfScreen(NotifyOutOfScreen.OutOfScreenMethod outOfScreenMethod, NotifyOutOfScreen.ListenerMode listenerMode, GameObject gameObject)
	{
		base.OnOutOfScreen(outOfScreenMethod, listenerMode, gameObject);
		CameraMovement.Instance.BossTimeZoomEnd();
		AudioManager.Instance.BossTimeEnd();
		ColorChangerHandler.BossmodeColorEnd();
	}

	private bool DetermineIfCriticalBossDamage()
	{
		float num = UnityEngine.Random.Range(0f, 1f);
		float num2 = 100f;
		float currentTotalValueFor = SkillManager.Instance.GetCurrentTotalValueFor<Skills.CriticalStrikeBossDamageChance>();
		float num3 = currentTotalValueFor / (num2 + currentTotalValueFor);
		return num < num3;
	}

	private readonly BigInteger BASE_DMG = 10;

	private UIMeterBigInteger hpBar;

	[SerializeField]
	private BigIntWrapper hp;

	public SpriteRenderer[] spriteRenderers;

	private Color startColor;

	private UnityEngine.Vector3 startScale;

	private BigInteger totalDmgRecieved = 0;
}
