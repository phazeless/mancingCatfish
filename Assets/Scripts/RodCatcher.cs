using System;
using System.Diagnostics;
using DG.Tweening;
using FullInspector;
using TMPro;
using UnityEngine;

public class RodCatcher : BaseCatcher
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<FishBehaviour> OnFishHooked;

	protected override void Awake()
	{
		base.Awake();
	}

	private void FishPool_OnObjectCreated(FishBehaviour fish)
	{
		fish.gameObject.SetActive(false);
	}

	protected override FishBehaviour OnCheckForCatch(bool isSimulation)
	{
		float num = 1f / SkillManager.Instance.GetCurrentTotalValueFor(this.fishTriesPerSecond);
		bool flag;
		if (isSimulation)
		{
			this.timer += 0.016666f;
			flag = (this.timer > num);
			if (flag)
			{
				this.timer = 0f;
			}
		}
		else
		{
			flag = FHelper.HasSecondsPassed(num, ref this.timer, true);
		}
		if (flag)
		{
			if (this.DetermineIfFishIsHooked())
			{
				int randomFishDWLvl = FishSpawnHelper.GetRandomFishDWLvl();
				FishBehaviour fishBehaviour;
				if (isSimulation)
				{
					fishBehaviour = FishPoolManager.Instance.GetFishPrefabAtDW(randomFishDWLvl);
				}
				else
				{
					bool flag2 = false;
					fishBehaviour = FishPoolManager.Instance.GetFishAtDW(randomFishDWLvl, out flag2);
					fishBehaviour.transform.position = base.transform.position;
				}
				return fishBehaviour;
			}
			base.NotifyFishDropped();
		}
		return null;
	}

	protected override void OnFishCaught(FishBehaviour fish)
	{
		fish.OnCaught(this.GenerateWeightModifier(fish));
		fish.OnCollected(base.transform.position);
		if (this.OnFishHooked != null)
		{
			this.OnFishHooked(fish);
		}
		this.CreateCashText(fish);
	}

	private void CreateCashText(FishBehaviour fish)
	{
		TextMeshPro txt = TextObjectPool.Instance.TextMeshProPool.GetObject();
		txt.transform.DOKill(true);
		txt.DOKill(true);
		txt.color = Color.white;
		txt.gameObject.SetActive(true);
		txt.SetText(CashFormatter.SimpleToCashRepresentation(fish.GetValue(false), 0, false, true, "N0"));
		txt.fontSize = 2f + CameraMovement.Instance.Zoom * 0.2f;
		txt.alignment = TextAlignmentOptions.Midline;
		txt.transform.position = base.transform.position;
		txt.transform.DOMoveY(txt.transform.position.y + 0.5f + CameraMovement.Instance.Zoom * 0.02f, 0.5f, false).SetEase(Ease.OutBack);
		txt.DOColor(this.TXTFADECOLOR, 0.5f).SetDelay(0.5f).OnComplete(delegate
		{
			txt.gameObject.SetActive(false);
			TextObjectPool.Instance.TextMeshProPool.ReturnObject(txt);
		});
	}

	private bool DetermineIfFishIsHooked()
	{
		float num = UnityEngine.Random.Range(0f, 1f);
		float num2 = 100f;
		float currentTotalValueFor = SkillManager.Instance.GetCurrentTotalValueFor(this.catchChance);
		float num3 = currentTotalValueFor / (num2 + currentTotalValueFor);
		return num <= num3;
	}

	private int DetermineWhichFishRarityToSpawn(int currentRarity = 0)
	{
		return -1;
	}

	private float GenerateWeightModifier(FishBehaviour fish)
	{
		int num = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.MinimumFishWeightIncrease>();
		return UnityEngine.Random.Range((float)num, (float)num + SkillManager.Instance.GetCurrentTotalValueFor(this.chanceForBiggerFish));
	}

	private float timer;

	private Color TXTFADECOLOR = new Color(1f, 1f, 1f, 0f);

	[InspectorDisabled]
	[SerializeField]
	protected Skills.FishingTools fishTriesPerSecond = new Skills.Rod_FishTriesPerSecond();

	[SerializeField]
	[InspectorDisabled]
	protected Skills.FishingTools catchChance = new Skills.Rod_CatchChance();

	[SerializeField]
	[InspectorDisabled]
	protected Skills.FishingTools chanceForRarerFish = new Skills.Rod_ChanceForRareFish();

	[InspectorDisabled]
	[SerializeField]
	protected Skills.FishingTools chanceForBiggerFish = new Skills.Rod_ChanceForBiggerFish();
}
