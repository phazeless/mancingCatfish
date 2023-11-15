using System;
using System.Numerics;
using DG.Tweening;
using FullInspector;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SwimBehaviourMono))]
public class FishBehaviour : BaseBehavior, INotifyOutOfScreen, IPooledObject<FishBehaviour>, IHasSpeed
{
	public int StimSpawnerId { get; set; }

	public bool IsPartOfStim
	{
		get
		{
			return this.StimSpawnerId != 0;
		}
	}

	public BigInteger OverrideValue { get; set; }

	public FishAttributes FishInfo
	{
		get
		{
			return this.fishInfo;
		}
	}

	[HideInInspector]
	public float ActualSpeed { get; set; }

	public float ActualWeight
	{
		get
		{
			return this.actualWeight;
		}
	}

	public int DeepWaterLvl
	{
		get
		{
			return this.deepWaterLvl;
		}
	}

	public bool HasGiantism
	{
		get
		{
			return this.GiantismValueMultiplier > 1;
		}
	}

	public int GiantismValueMultiplier
	{
		get
		{
			return this.giantismValueMultiplier;
		}
		set
		{
			if (this.giantismValueMultiplier != value)
			{
				this.giantismValueMultiplier = value;
				if (this.HasGiantism)
				{
					base.transform.localScale = this.originalScale * 2.7f;
				}
				else
				{
					base.transform.localScale = this.originalScale;
				}
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (this.aboveSurfaceFishAlternativeLooks.Length > 0)
		{
			int num = UnityEngine.Random.Range(0, this.aboveSurfaceFishAlternativeLooks.Length);
			this.aboveSurfaceFish = this.aboveSurfaceFishAlternativeLooks[num];
		}
		this.notifyOutOfScreen = base.GetComponent<NotifyOutOfScreen>();
		if (this.aboveSurfaceFish != null)
		{
			this.fishCaughtVisualBehaviour = this.aboveSurfaceFish.GetComponent<FishCaughtVisualBehaviour>();
		}
		this.deepWaterLvl = DWHelper.CurrentDWLevel;
		if (this.swimBehaviourMono == null)
		{
			this.swimBehaviourMono = base.GetComponent<SwimBehaviourMono>();
		}
		this.originalScale = base.transform.localScale;
	}

	public void InvokeWhenJumpFinished(Action<FishBehaviour> invokeWhenJumpFinished)
	{
		this.invokeWhenJumpFinished = invokeWhenJumpFinished;
	}

	public SwimBehaviourMono GetSwimBehaviourMono()
	{
		return this.swimBehaviourMono;
	}

	public void OnCaught(float fishWeightModifier = 1f)
	{
		this.underSurfaceFish.SetActive(false);
		if (this.swimBehaviourMono != null)
		{
			this.swimBehaviourMono.enabled = false;
		}
		base.transform.rotation = new UnityEngine.Quaternion(0f, 0f, 0f, 0f);
		this.weightModifier = fishWeightModifier;
		this.actualWeight = this.weightModifier * this.fishInfo.BaseWeight;
		HookedVibration.NormalFishCaughtHaptic();
	}

	public void OnCollected(UnityEngine.Vector2 fromPosition)
	{
		if (this.underSurfaceFish != null)
		{
			this.underSurfaceFish.SetActive(false);
			this.underSurfaceFish.transform.localPosition = new UnityEngine.Vector3(0f, 0f, 15f);
		}
		if (this.aboveSurfaceFish != null)
		{
			this.aboveSurfaceFish.SetActive(true);
			this.aboveSurfaceFish.transform.localPosition = UnityEngine.Vector3.zero;
		}
		if (this.fishCaughtVisualBehaviour != null)
		{
			this.fishCaughtVisualBehaviour.gameObject.SetActive(true);
			this.fishCaughtVisualBehaviour.Jump(UnityEngine.Vector2.zero);
		}
		this.CreateValueText();
		this.wasSwiped = false;
	}

	public void ActivateSwimming(Transform parent)
	{
		this.SetToInitialState();
		this.swimBehaviourMono.enabled = true;
		this.underSurfaceFish.SetActive(true);
	}

	public void SetToInitialState()
	{
		if (this.fishInfo != null)
		{
			this.ActualSpeed = UnityEngine.Random.Range(this.fishInfo.MinSpeed, this.fishInfo.MaxSpeed);
		}
		else
		{
			UnityEngine.Debug.LogWarning("fishInfo is null. Shouldn't be possible, has serialization failed?");
		}
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(true);
		}
		else
		{
			UnityEngine.Debug.LogWarning("FishBehaviour.gameObject is null. Has it been destroyed?");
		}
		if (this.underSurfaceFish != null)
		{
			this.underSurfaceFish.SetActive(false);
			this.underSurfaceFish.transform.localPosition = new UnityEngine.Vector3(0f, 0f, 15f);
		}
		else
		{
			UnityEngine.Debug.LogWarning("underSurfaceFish is null. Has it been destroyed?");
		}
		if (this.aboveSurfaceFish != null)
		{
			this.aboveSurfaceFish.SetActive(false);
			this.aboveSurfaceFish.transform.localPosition = UnityEngine.Vector3.zero;
		}
	}

	public virtual void OnSwiped(CircleCatcher catcher)
	{
		this.wasSwiped = true;
		if (this.DetermineIfSpawnFishOnSwipe())
		{
			SpawnUtil.Instance.SpawnFish();
		}
		if (this.IsPartOfStim)
		{
			StimCounter.Instance.IncreaseCounter(this.StimSpawnerId);
		}
	}

	private void CreateValueText()
	{
		TextMeshPro txt = TextObjectPool.Instance.TextMeshProPool.GetObject();
		txt.transform.DOKill(true);
		txt.DOKill(true);
		txt.gameObject.SetActive(true);
		bool flag = false;
		Color color = Color.white;
		txt.SetText(CashFormatter.SimpleToCashRepresentation(this.GetValue(out flag, false), 0, false, true, "N0"));
		if (flag)
		{
			float num = 1.5f;
			color = FishBehaviour.CRITCOLOR;
			txt.fontSize = (2f + CameraMovement.Instance.Zoom * 0.2f) * num;
			txt.color = color;
			txt.alignment = TextAlignmentOptions.Midline;
			txt.transform.position = base.transform.position;
			txt.transform.DOPunchScale(new UnityEngine.Vector3(1f, 1f, 0f), 0.2f, 1, 1f);
			txt.transform.DOMoveY(txt.transform.position.y + 0.5f + CameraMovement.Instance.Zoom * 0.02f, 0.5f, false).SetEase(Ease.OutBack);
			txt.DOColor(Color.white, 0.5f).OnComplete(delegate
			{
				txt.DOColor(this.TXTFADECOLOR, 0.5f).OnComplete(delegate
				{
					txt.gameObject.SetActive(false);
					TextObjectPool.Instance.TextMeshProPool.ReturnObject(txt);
				});
			});
		}
		else
		{
			txt.fontSize = 2f + CameraMovement.Instance.Zoom * 0.2f;
			txt.color = color;
			txt.alignment = TextAlignmentOptions.Midline;
			txt.transform.position = base.transform.position;
			txt.transform.DOMoveY(txt.transform.position.y + 0.5f + CameraMovement.Instance.Zoom * 0.02f, 0.5f, false).SetEase(Ease.OutBack);
			txt.DOColor(this.TXTFADECOLOR, 0.5f).SetDelay(0.5f).OnComplete(delegate
			{
				txt.gameObject.SetActive(false);
				TextObjectPool.Instance.TextMeshProPool.ReturnObject(txt);
			});
		}
	}

	public virtual bool IsCaught()
	{
		return true;
	}

	public virtual BigInteger GetValue(out bool isCriticalValue, bool isAfk = false)
	{
		float num = 1f;
		isCriticalValue = false;
		if (this.DetermineIfCriticalFishValue())
		{
			isCriticalValue = true;
			num = SkillManager.Instance.GetCurrentTotalValueFor<Skills.CriticalStrikeFishValueMultiplier>();
		}
		float num2 = (float)((int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.FishValue>());
		if (!isAfk)
		{
			num2 = num2 * SkillManager.Instance.GetCurrentTotalValueFor<Skills.InGameFishValueModifier>() * num;
		}
		if (this.wasSwiped)
		{
			num2 *= SkillManager.Instance.GetCurrentTotalValueFor<Skills.FishValueSwipeMultiplier>();
		}
		else
		{
			num2 *= SkillManager.Instance.GetCurrentTotalValueFor<Skills.FishValueBoatMultiplier>();
		}
		int value = (int)num2;
		int currentComboMultiplier = StimCounter.Instance.GetCurrentComboMultiplier(this.StimSpawnerId);
		if (this.OverrideValue <= 0L)
		{
			BigInteger left = this.fishInfo.BaseValue.Value * (int)this.weightModifier;
			return left * ValueModifier.GetTotalValueIncrease() * value * this.giantismValueMultiplier * currentComboMultiplier;
		}
		return ValueModifier.GetTotalValueIncrease() * this.OverrideValue * value * this.giantismValueMultiplier * currentComboMultiplier;
	}

	public BigInteger GetValue(bool isAfk = false)
	{
		bool flag = false;
		return this.GetValue(out flag, isAfk);
	}

	private bool DetermineIfCriticalFishValue()
	{
		float num = UnityEngine.Random.Range(0f, 1f);
		float num2 = 100f;
		float currentTotalValueFor = SkillManager.Instance.GetCurrentTotalValueFor<Skills.CriticalStrikeFishValueChance>();
		float num3 = currentTotalValueFor / (num2 + currentTotalValueFor);
		return num < num3;
	}

	private bool DetermineIfSpawnFishOnSwipe()
	{
		float num = UnityEngine.Random.Range(0f, 1f);
		float num2 = 100f;
		float currentTotalValueFor = SkillManager.Instance.GetCurrentTotalValueFor<Skills.SpawnFishOnSwipeChance>();
		float num3 = currentTotalValueFor / (num2 + currentTotalValueFor);
		return num < num3;
	}

	public void OnJumpFinished()
	{
		if (this.IsCurrentlyPooled)
		{
			this.fishPool.ReturnObject(this);
		}
		if (this.invokeWhenJumpFinished != null)
		{
			this.invokeWhenJumpFinished(this);
			this.invokeWhenJumpFinished = null;
		}
	}

	public virtual void OnOutOfScreen(NotifyOutOfScreen.OutOfScreenMethod outOfScreenMethod, NotifyOutOfScreen.ListenerMode listenerMode, GameObject gameObject)
	{
		if (this.IsCurrentlyPooled)
		{
			this.fishPool.ReturnObject(this);
		}
	}

	public void OnAddedToPool(ObjectPool<FishBehaviour> pool)
	{
		this.fishPool = pool;
	}

	public void OnRetrieved(ObjectPool<FishBehaviour> pool)
	{
		base.gameObject.SetActive(true);
	}

	public void OnReturned(ObjectPool<FishBehaviour> pool)
	{
		if (this.IsPartOfStim)
		{
			this.StimSpawnerId = 0;
		}
		this.GiantismValueMultiplier = 1;
		base.gameObject.SetActive(false);
	}

	private bool IsCurrentlyPooled
	{
		get
		{
			return this.fishPool != null;
		}
	}

	[SerializeField]
	protected GameObject underSurfaceFish;

	[SerializeField]
	protected GameObject aboveSurfaceFish;

	[SerializeField]
	protected GameObject[] aboveSurfaceFishAlternativeLooks;

	[SerializeField]
	private FishAttributes fishInfo = new FishAttributes();

	private float actualWeight;

	private int deepWaterLvl;

	private float weightModifier = 1f;

	private bool wasSwiped;

	private UnityEngine.Vector3 originalScale = UnityEngine.Vector3.one;

	private static Color CRITCOLOR = new Color(0.878f, 0.392f, 0.098f);

	private NotifyOutOfScreen notifyOutOfScreen;

	private FishCaughtVisualBehaviour fishCaughtVisualBehaviour;

	private ObjectPool<FishBehaviour> fishPool;

	private SwimBehaviourMono swimBehaviourMono;

	private Action<FishBehaviour> invokeWhenJumpFinished;

	private Color TXTFADECOLOR = new Color(1f, 1f, 1f, 0f);

	private int giantismValueMultiplier = 1;

	public enum FishType
	{
		DW0,
		DW1,
		DW2,
		DW3,
		DW4,
		DW5,
		DW6,
		DW7,
		DW8,
		DW9,
		DW10,
		DW11,
		DW12,
		DW13,
		DW14,
		DW15,
		DW16,
		DW17,
		DW18,
		DW19,
		DW0Boss,
		DW1Boss,
		DW2Boss,
		DW3Boss,
		DW4Boss,
		DW5Boss,
		DW6Boss,
		DW7Boss,
		DW8Boss,
		DW9Boss,
		DW10Boss,
		DW11Boss,
		DW12Boss,
		DW13Boss,
		DW14Boss,
		DW15Boss,
		DW16Boss,
		DW17Boss,
		DW18Boss,
		DW19Boss,
		Special0,
		Special1,
		Special2,
		Special3,
		Special4,
		Special5,
		Special6,
		Special7,
		Special8,
		SpecialTournament0,
		SpecialTournament1,
		SpecialTournament2,
		SpecialTournament3,
		SpecialTournament4,
		Special9,
		Special10,
		Special11,
		Special12,
		Special13
	}
}
