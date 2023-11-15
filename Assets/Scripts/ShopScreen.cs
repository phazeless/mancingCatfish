using System;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MonoBehaviour
{
	public static ShopScreen Instance { get; private set; }

	private void Awake()
	{
		ShopScreen.Instance = this;
	}

	private void Start()
	{
        UnityEngine.Vector2 anchoredPosition = this.scrollRect.content.anchoredPosition;
		anchoredPosition.y = 0f;
		this.scrollRect.content.anchoredPosition = anchoredPosition;
	}

	public void GoToScreen(Action onScreenTransitionEnded = null)
	{
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Shop, onScreenTransitionEnded);
		this.scrollRect.verticalNormalizedPosition = 1f;
	}

	public void AnimateGemPacksIfPressedUnlockCrewNotAvailable(UIUnlockCrewMember button)
	{
		int num = (int)SkillManager.Instance.UnlockCrewMemberSkill.CostForNextLevelUp;
		bool flag = SkillManager.Instance.GetCrewMembersAvailableForPurchase().Count > 0 && ResourceManager.Instance.GetResourceAmount(ResourceType.Gems) < (long)num;
		if (button != null && flag)
		{
			this.AnimateToGemPack();
		}
	}

	public void AnimateGemPacksIfPressedBoostIsNotAffordable(PurchaseItemButton button)
	{
		if (button != null && !ResourceManager.StoreManager.CanAfford(button.Id))
		{
			this.AnimateToGemPack();
		}
	}

	public void AnimateGemPacksIfPressedItemBoxIsNotAffordable(UIShopChestItem button)
	{
		BigInteger resourceAmount = ResourceManager.Instance.GetResourceAmount(ResourceType.Gems);
		if (button != null && resourceAmount < (long)button.CostToPurchase)
		{
			this.AnimateToGemPack();
		}
	}

	public void AnimateToGemPack()
	{
		if (ScreenManager.Instance.CurrentScreen == ScreenManager.Screen.Shop)
		{
			this.scrollRect.verticalNormalizedPosition = 0f;
			for (int i = 0; i < this.gemPacksToBounceIfNotEnoughtGems.Count; i++)
			{
				Transform transform = this.gemPacksToBounceIfNotEnoughtGems[i];
				transform.DOPunchScale(transform.localScale * 0.3f, 0.2f, 10, 1f).SetDelay(0.1f * (float)(i + 1));
			}
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < this.gemPacksToBounceIfNotEnoughtGems.Count; i++)
		{
			Transform target = this.gemPacksToBounceIfNotEnoughtGems[i];
			target.DOKill(false);
		}
	}

	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private List<Transform> gemPacksToBounceIfNotEnoughtGems = new List<Transform>();
}
