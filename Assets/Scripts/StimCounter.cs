using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StimCounter : MonoBehaviour
{
	public static StimCounter Instance { get; private set; }

	public bool IsMaxMultiplier
	{
		get
		{
			return (float)this.currentMultiplier == this.cachedMultiplierMax;
		}
	}

	private void Awake()
	{
		StimCounter.Instance = this;
	}

	private void Start()
	{
		this.cachedMultiplierRate = SkillManager.Instance.GetCurrentTotalValueFor<Skills.StimValueMultiplierRate>();
		this.cachedMultiplierMax = SkillManager.Instance.GetCurrentTotalValueFor<Skills.StimValueMultiplierMax>();
		SkillManager.Instance.OnSkillAttributeValueChanged += this.Instance_OnSkillAttributeValueChanged;
		base.gameObject.SetActive(false);
	}

	private void Instance_OnSkillAttributeValueChanged(ISkillAttribute attr, float val)
	{
		if (attr is Skills.StimValueMultiplierMax)
		{
			this.cachedMultiplierMax = val;
		}
		else if (attr is Skills.StimValueMultiplierRate)
		{
			this.cachedMultiplierRate = val;
		}
	}

	private void Update()
	{
		if (!this.isLosingStreakTweening && FHelper.HasSecondsPassed(this.breakComboAfterSeconds * 0.8f, ref this.timerToBreakCombo, true))
		{
			this.isLosingStreakTweening = true;
			base.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
		}
		if (FHelper.HasSecondsPassed(this.breakComboAfterSeconds, ref this.timerToBreakCombo, true))
		{
			this.Disable(this.currentStimSpawnerId);
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.G))
		{
			ShoalSpawnerUtil.Instance.SpawnShoal();
		}
	}

	public int GetCurrentComboMultiplier(int spawnerId)
	{
		if (this.IsRelevantStimSpawnerId(spawnerId))
		{
			return this.currentMultiplier;
		}
		return 1;
	}

	public void IncreaseCounter(int spawnerId)
	{
		if (!this.IsActivated)
		{
			this.Activate(spawnerId);
		}
		if (!this.IsRelevantStimSpawnerId(spawnerId))
		{
			this.ResetCounter(spawnerId);
		}
		this.timerToBreakCombo = 0f;
		this.counter++;
		bool flag = !this.IsMaxMultiplier && (float)(this.counter - 1) % this.cachedMultiplierRate == 0f;
		if (flag)
		{
			this.currentMultiplier = (int)Mathf.Min(Mathf.Max(1f, 1f + (float)this.counter / this.cachedMultiplierRate), this.cachedMultiplierMax);
		}
		this.UpdateCounterUI(flag);
	}

	private void UpdateCounterUI(bool shouldUpdateMultiplier)
	{
		if (shouldUpdateMultiplier)
		{
			if (this.IsMaxMultiplier)
			{
				this.currentColor = this.maxedColor;
				this.maxLabel.gameObject.SetActive(true);
			}
			else
			{
				this.currentColor = this.unmaxedColor;
			}
			this.multiplierLabel.SetText("x" + this.currentMultiplier.ToString());
			this.multiplierLabel.transform.DOKill(true);
			this.multiplierLabel.transform.localScale = Vector3.one * 0.8f;
			this.multiplierLabel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
			this.multiplierLabel.DOKill(false);
			this.multiplierLabel.color = Color.white;
			this.multiplierLabel.DOColor(this.currentColor, 0.3f);
		}
		if (this.currentScaleTarget < 2f)
		{
			this.currentScaleTarget += 0.004f;
		}
		if (this.isLosingStreakTweening)
		{
			base.transform.DOKill(false);
			base.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
		}
		this.isLosingStreakTweening = false;
		this.fishCounterLabel.DOKill(false);
		this.fishCounterLabel.color = Color.white;
		this.fishCounterLabel.DOColor(this.currentColor, 0.2f);
		this.LabelHolder.DOKill(false);
		this.LabelHolder.DOScale(this.currentScaleTarget, 0.3f).SetEase(Ease.OutBack);
		this.fishCounterLabel.SetText("(" + this.counter + ")");
		this.fishCounterLabel.transform.DOKill(false);
		this.fishCounterLabel.transform.localScale = Vector3.one * 1.3f;
		this.fishCounterLabel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
	}

	public bool IsActivated
	{
		get
		{
			return base.gameObject.activeSelf;
		}
	}

	private void ResetCounter(int spawnerId)
	{
		this.timerToBreakCombo = 0f;
		this.counter = 0;
		this.currentScaleTarget = 1f;
		this.LabelHolder.localScale = new Vector3(0.5f, 1f, 1f);
		base.transform.position = this.PositionTarget.transform.position;
		float num = 0.1f;
		if (base.transform.position.y > 3f)
		{
			num = 0.7f;
		}
		base.transform.position = new Vector3(base.transform.position.x * 0.75f, base.transform.position.y * 0.9f - num, base.transform.position.z);
		this.currentStimSpawnerId = spawnerId;
	}

	private void Activate(int spawnerId)
	{
		this.ResetCounter(spawnerId);
		this.UpdateCounterUI(true);
		this.maxLabel.gameObject.SetActive(false);
		this.currentColor = this.unmaxedColor;
		base.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
		base.gameObject.SetActive(true);
	}

	private void Disable(int spawnerId)
	{
		if (!this.IsRelevantStimSpawnerId(spawnerId))
		{
			return;
		}
		this.isLosingStreakTweening = false;
		this.currentMultiplier = 1;
		this.timerToBreakCombo = 0f;
		this.maxLabel.gameObject.SetActive(false);
		this.counter = 0;
		base.gameObject.SetActive(false);
	}

	private bool IsRelevantStimSpawnerId(int spawnerId)
	{
		return spawnerId == this.currentStimSpawnerId;
	}

	private void OnDestroy()
	{
		base.transform.DOKill(false);
		this.LabelHolder.DOKill(false);
		this.multiplierLabel.transform.DOKill(false);
		SkillManager.Instance.OnSkillAttributeValueChanged -= this.Instance_OnSkillAttributeValueChanged;
	}

	[SerializeField]
	private TextMeshPro multiplierLabel;

	[SerializeField]
	private TextMeshPro fishCounterLabel;

	[SerializeField]
	private Transform PositionTarget;

	[SerializeField]
	private Transform LabelHolder;

	[SerializeField]
	private Transform maxLabel;

	private int currentStimSpawnerId;

	private int counter;

	private float timerToBreakCombo;

	private float breakComboAfterSeconds = 1f;

	private float cachedMultiplierRate = 10f;

	private float cachedMultiplierMax = 10f;

	private Color maxedColor = new Color(0.984f, 0.31f, 0.11f);

	private Color unmaxedColor = new Color(1f, 0.886f, 0f);

	private Color currentColor = new Color(1f, 0.886f, 0f);

	private int currentMultiplier = 1;

	private float currentScaleTarget = 1f;

	private bool isLosingStreakTweening;
}
