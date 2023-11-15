using System;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CrownDialogButton : MonoBehaviour
{
	private void Awake()
	{
		this.crownHolderStartingScale = this.crownHolder.localScale;
		this.fishStartingPosition = this.fish.localPosition;
		this.bobberStartingPosition = this.bobber.localPosition;
		this.startingCrownHolderPosition = this.crownHolder.localPosition;
	}

	private void Start()
	{
		ResourceManager.Instance.OnResourceChanged += this.Instance_OnResourceChanged;
		this.crownLevelSkill.OnSkillLevelUp += this.CrownLevelSkill_OnSkillLevelUp;
		this.startingCrownExp = (int)ResourceManager.Instance.GetResourceAmount(ResourceType.CrownExp);
		this.costForNextLevel = (int)this.crownLevelSkill.CostForNextLevelUp;
		this.crownLevelExpLabel.SetVariableText(new string[]
		{
			this.startingCrownExp.ToString(),
			this.costForNextLevel.ToString()
		});
		this.currentCrownLevel = this.crownLevelSkill.CurrentLevel;
		this.crownLevelLabel.SetText(this.currentCrownLevel.ToString());
		if (this.crownLevelSkill.CurrentLevel == 0 && ResourceManager.Instance.GetResourceAmount(ResourceType.CrownExp) == 0L)
		{
			base.transform.localScale = UnityEngine.Vector3.zero;
		}
		this.expMeter.SetMax((float)this.costForNextLevel);
		this.expMeter.SetCurrent((float)this.startingCrownExp);
	}

	private void CrownLevelSkill_OnSkillLevelUp(Skill arg1, LevelChange arg2)
	{
		this.targetExpGain -= (int)this.crownLevelSkill.GetCostForLevelUp(this.crownLevelSkill.CurrentLevel - 1);
	}

	private void CheckIfLevelUp()
	{
		if (this.currentCrownExp >= (float)this.costForNextLevel)
		{
			this.currentCrownLevel++;
			this.costForNextLevel = (int)this.crownLevelSkill.GetCostForLevelUp(this.currentCrownLevel);
			this.currentCrownExp = 0f;
			this.expMeter.SetMax((float)this.costForNextLevel);
			AudioManager.Instance.OneShooter(this.levelUp, 1f);
			this.crownLevelLabel.SetText(this.currentCrownLevel.ToString());
			this.crownLevelLabel.transform.DOKill(false);
			this.crownLevelLabel.transform.localScale = UnityEngine.Vector3.one;
			this.crownLevelLabel.transform.DOPunchScale(new UnityEngine.Vector3(0.5f, 0.5f, 0f), 0.2f, 10, 1f);
		}
	}

	private void SetCorrectValues()
	{
		this.currentCrownExp = (float)((int)ResourceManager.Instance.GetResourceAmount(ResourceType.CrownExp));
		this.currentCrownLevel = this.crownLevelSkill.CurrentLevel;
		this.costForNextLevel = (int)this.crownLevelSkill.CostForNextLevelUp;
		this.crownLevelLabel.SetText(this.currentCrownLevel.ToString());
		this.crownLevelExpLabel.SetVariableText(new string[]
		{
			this.currentCrownExp.ToString(),
			this.costForNextLevel.ToString()
		});
	}

	public void SkillDialogOpened()
	{
		this.notisHolder.DOScale(0f, 0.5f).SetEase(Ease.OutCubic).OnComplete(delegate
		{
			this.notisHolder.gameObject.SetActive(false);
		});
	}

	private void UpdateNotis()
	{
		BigInteger resourceAmount = ResourceManager.Instance.GetResourceAmount(ResourceType.SkillPoints);
		if (resourceAmount > 0L && !this.isUpdatingNotis)
		{
			this.isUpdatingNotis = true;
			this.notisHolder.gameObject.SetActive(true);
			this.notisHolder.DOScale(1f, 0.5f).SetEase(Ease.OutElastic).SetDelay(0.5f).OnStart(delegate
			{
				this.skillpointOnNotisCountLabel.SetText(ResourceManager.Instance.GetResourceAmount(ResourceType.SkillPoints).ToString());
			}).OnComplete(delegate
			{
				this.isUpdatingNotis = false;
			});
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			this.UpdateNotis();
		}
	}

	private void Update()
	{
		if (this.isIncrementMeter && this.isTweenComplete)
		{
			this.isParticlesSpawned = false;
			if (this.currentExpGainCounted < (float)this.targetExpGain)
			{
				float num = (float)this.targetExpGain * Time.deltaTime;
				this.currentExpGainCounted += num;
				this.currentCrownExp += num;
				this.CheckIfLevelUp();
				this.expMeter.SetCurrent(this.currentCrownExp);
				if (this.currentExpGainCounted >= (float)this.targetExpGain)
				{
					this.SetCorrectValues();
				}
				else
				{
					this.crownLevelExpLabel.SetVariableText(new string[]
					{
						Mathf.RoundToInt(this.currentCrownExp).ToString(),
						this.costForNextLevel.ToString()
					});
				}
			}
			else
			{
				this.startingCrownExp = (int)this.currentCrownExp;
				this.currentExpGainCounted = 0f;
				this.targetExpGain = 0;
				this.isIncrementMeter = false;
				this.ReturnTween();
			}
		}
	}

	private void Instance_OnResourceChanged(ResourceType resourceType, BigInteger added, BigInteger total)
	{
		if (resourceType == ResourceType.SkillPoints)
		{
			this.UpdateNotis();
		}
		if (resourceType == ResourceType.CrownExp && added > 0L)
		{
			this.SpawnParticles();
			if (!this.isIncrementMeter)
			{
				this.currentCrownExp = (float)this.startingCrownExp;
				this.ExpGainTween();
			}
			int num = this.targetExpGain;
			this.targetExpGain = (int)added + num;
			this.isIncrementMeter = true;
		}
	}

	private void SpawnParticles()
	{
		if (this.isParticlesSpawned)
		{
			return;
		}
		this.isParticlesSpawned = true;
		Transform particleInstance = UnityEngine.Object.Instantiate<Transform>(this.expParticlePrefab, this.expMeterHolder);
		particleInstance.position = UnityEngine.Vector3.zero;
		particleInstance.localPosition = new UnityEngine.Vector3(particleInstance.localPosition.x, particleInstance.localPosition.y, 0f);
		particleInstance.DOLocalMoveX(-400f, this.a, false).SetEase(Ease.OutQuad).OnComplete(delegate
		{
			particleInstance.DOLocalMoveX(200f, this.b, false).SetEase(Ease.InOutQuad).OnComplete(delegate
			{
				particleInstance.DOLocalMoveX(0f, this.c, false).SetEase(Ease.InQuad).OnComplete(delegate
				{
				});
			});
		});
		particleInstance.DOLocalMoveY(200f, this.d, false).SetEase(Ease.OutQuad).OnComplete(delegate
		{
			particleInstance.DOLocalMoveY(-100f, this.e, false).SetEase(Ease.InOutQuad).OnComplete(delegate
			{
				particleInstance.DOLocalMoveY(0f, this.f, false).SetEase(Ease.InQuad).OnComplete(delegate
				{
				});
			});
		});
	}

	private void ExpGainTween()
	{
		this.TweenKiller(false);
		AudioManager.Instance.OneShooter(this.woosh, 1f);
		base.transform.localScale = UnityEngine.Vector3.one;
		this.buttonBodyHolder.DOScale(0f, 0.3f);
		this.crownHolder.localEulerAngles = UnityEngine.Vector2.zero;
		this.crownHolder.DOMoveX(0f, 0.3f, false).SetEase(Ease.OutBack);
		this.crownHolder.DOLocalMoveY(-250f, 0.3f, false).SetEase(Ease.OutBack);
		this.crownHolder.DOPunchRotation(new UnityEngine.Vector3(0f, 0f, 10f), 0.5f, 10, 1f);
		this.crownHolder.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
		this.expMeterHolder.gameObject.SetActive(true);
		this.expMeterHolder.DOScaleX(1f, 0.3f).SetEase(Ease.OutBack);
		this.expMeterHolder.DOLocalMoveY(-62f, 0.3f, false).SetEase(Ease.OutBack).SetDelay(0.2f).OnComplete(delegate
		{
			AudioManager.Instance.StartSpecific(this.expGainClip);
			this.isTweenComplete = true;
			this.expMeterHolder.DOScale(1.05f, 0.2f);
		});
		this.fish.localPosition = UnityEngine.Vector2.zero;
		this.fish.DOLocalMove(new UnityEngine.Vector3(132f, 10f, 0f), 0.5f, false).SetEase(Ease.OutBack).SetDelay(0.1f);
		this.bobber.localPosition = UnityEngine.Vector2.zero;
		this.bobber.DOLocalMove(new UnityEngine.Vector3(-121f, 10f, 0f), 0.5f, false).SetEase(Ease.OutBack).SetDelay(0.2f);
		this.bobber.localEulerAngles = new UnityEngine.Vector3(0f, 0f, -12f);
		this.bobber.DORotate(UnityEngine.Vector3.zero, 1.5f, RotateMode.Fast).SetDelay(0.3f).SetEase(Ease.OutBack);
		this.fish.localEulerAngles = new UnityEngine.Vector3(0f, 0f, 12f);
		this.fish.DORotate(UnityEngine.Vector3.zero, 1.5f, RotateMode.Fast).SetDelay(0.2f).SetEase(Ease.OutBack);
	}

	private void ReturnTween()
	{
		this.TweenKiller(false);
		this.isTweenComplete = false;
		AudioManager.Instance.StopSpecific(this.expGainClip);
		AudioManager.Instance.OneShooter(this.expEndClip, 1f);
		this.buttonBodyHolder.DOScale(1f, 0.3f).SetDelay(0.3f);
		this.crownHolder.localEulerAngles = UnityEngine.Vector2.zero;
		this.crownHolder.DOLocalMove(this.startingCrownHolderPosition, 0.3f, false).SetEase(Ease.OutBack).SetDelay(0.3f).OnStart(delegate
		{
			AudioManager.Instance.OneShooter(this.woosh, 1f);
		});
		this.crownHolder.DOPunchRotation(new UnityEngine.Vector3(0f, 0f, 10f), 0.5f, 10, 1f).SetDelay(0.3f);
		this.crownHolder.DOScale(1.2f, 0.1f).SetEase(Ease.OutQuad).OnComplete(delegate
		{
			this.crownHolder.DOScale(0.8f, 0.1f).SetEase(Ease.InOutQuad).OnComplete(delegate
			{
				this.crownHolder.DOScale(0.5473548f, 0.3f).SetEase(Ease.InBack).SetDelay(0.1f);
			});
		});
		this.expMeterHolder.DOScale(1f, 0.2f).SetEase(Ease.OutBack).OnComplete(delegate
		{
			this.expMeterHolder.DOScaleX(0f, 0.2f).SetEase(Ease.OutBack).OnComplete(delegate
			{
				this.expMeterHolder.gameObject.SetActive(false);
			});
		});
		this.expMeterHolder.DOLocalMoveY(0f, 0.3f, false).SetEase(Ease.InBack);
		this.fish.DOLocalMove(new UnityEngine.Vector3(0f, 10f, 0f), 0.5f, false).SetEase(Ease.OutCirc);
		this.bobber.DOLocalMove(new UnityEngine.Vector3(0f, 10f, 0f), 0.5f, false).SetEase(Ease.OutCirc);
	}

	private void TweenKiller(bool finish = true)
	{
		this.crownHolder.DOKill(finish);
		this.crownLevelLabelTransform.DOKill(finish);
		this.fish.DOKill(finish);
		this.bobber.DOKill(finish);
		this.expMeterHolder.DOKill(finish);
		this.buttonBodyHolder.DOKill(finish);
		this.crownLevelLabel.transform.DOKill(false);
	}

	private void OnDestroy()
	{
		this.TweenKiller(true);
		ResourceManager.Instance.OnResourceChanged -= this.Instance_OnResourceChanged;
		this.crownLevelSkill.OnSkillLevelUp -= this.CrownLevelSkill_OnSkillLevelUp;
		this.notisHolder.DOKill(true);
	}

	[SerializeField]
	[Header("References")]
	private Skill crownLevelSkill;

	[SerializeField]
	private Transform crownHolder;

	[SerializeField]
	private Transform fish;

	[SerializeField]
	private Transform bobber;

	[SerializeField]
	private Transform crownLevelLabelTransform;

	[SerializeField]
	private Transform buttonBodyHolder;

	[SerializeField]
	private Transform expMeterHolder;

	[SerializeField]
	private UIMeter expMeter;

	[SerializeField]
	private TextMeshProUGUI crownLevelLabel;

	[SerializeField]
	private TextMeshProUGUI crownLevelExpLabel;

	[SerializeField]
	private Transform expParticlePrefab;

	[SerializeField]
	private Transform notisHolder;

	[SerializeField]
	private TextMeshProUGUI skillpointOnNotisCountLabel;

	[SerializeField]
	private AudioClip expGainClip;

	[SerializeField]
	private AudioClip expEndClip;

	[SerializeField]
	private AudioClip woosh;

	[SerializeField]
	private AudioClip levelUp;

	private UnityEngine.Vector2 crownHolderStartingScale = UnityEngine.Vector2.one;

	private UnityEngine.Vector2 fishStartingPosition = UnityEngine.Vector2.one;

	private UnityEngine.Vector2 bobberStartingPosition = UnityEngine.Vector2.one;

	private UnityEngine.Vector2 startingCrownHolderPosition = UnityEngine.Vector2.one;

	private bool isIncrementMeter;

	private int targetExpGain;

	private float currentExpGainCounted;

	private int costForNextLevel;

	private float currentCrownExp;

	private int startingCrownExp;

	private int currentCrownLevel;

	private int incrementationSpeed = 1;

	private bool isUpdatingNotis;

	private float prevVal;

	private float a = 0.07f;

	private float b = 0.25f;

	private float c = 0.1f;

	private float d = 0.2f;

	private float e = 0.2f;

	private float f = 0.1f;

	private bool isParticlesSpawned;

	private bool isTweenComplete;
}
