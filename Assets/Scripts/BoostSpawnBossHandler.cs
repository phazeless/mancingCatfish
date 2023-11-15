using System;
using ACE.IAPS;
using TMPro;
using UnityEngine;

public class BoostSpawnBossHandler : MonoBehaviour
{
	private void Awake()
	{
		this.gemCost.SetVariableText(new string[]
		{
			5.ToString()
		});
		this.spawnBossSkill.OnSkillActivation += this.SpawnBossSkill_OnSkillActivation;
		this.spawnBossSkill.OnSkillCooldownZero += this.SpawnBossSkill_OnSkillCooldownZero;
		StoreManager storeManager = ResourceManager.StoreManager;
		storeManager.OnGoodBalanceChanged = (Action<string, int, int>)Delegate.Combine(storeManager.OnGoodBalanceChanged, new Action<string, int, int>(this.Instance_OnGoodBalanceChanged));
	}

	private void Start()
	{
		this.spawnBossSkill.ResetCooldown();
	}

	private void Instance_OnGoodBalanceChanged(string itemId, int balance, int amountAdded)
	{
		if ("se.ace.boost_boss_fish" == itemId && amountAdded == 1)
		{
			this.spawnBossSkill.Activate();
		}
	}

	private void SpawnBossSkill_OnSkillCooldownZero(Skill obj)
	{
		this.buyBlocker.SetActive(false);
	}

	private void SpawnBossSkill_OnSkillActivation(Skill obj)
	{
		ResourceManager.StoreManager.TakeItem("se.ace.boost_boss_fish", 1);
		this.buyBlocker.SetActive(true);
		this.RunAfterDelay(0.5f, delegate()
		{
			ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Main);
			this.bossFishSpawner.Spawn();
		});
	}

	private void Update()
	{
		if (this.timeLeft.gameObject.activeInHierarchy)
		{
			this.timeLeft.SetVariableText(new string[]
			{
				Mathf.CeilToInt(this.spawnBossSkill.GetTotalSecondsLeftOnCooldown()).ToString()
			});
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && this.spawnBossSkill.IsOnCooldown)
		{
			this.spawnBossSkill.ResetCooldown();
		}
	}

	[SerializeField]
	private Skill spawnBossSkill;

	[SerializeField]
	private GameObject buyBlocker;

	[SerializeField]
	private TextMeshProUGUI timeLeft;

	[SerializeField]
	private BossFishSpawner bossFishSpawner;

	[SerializeField]
	private TextMeshProUGUI gemCost;
}
