using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrownLevelLimitBehaviour : MonoBehaviour
{
	public void UpdateUi(Skill crownLevelSkill, Skill crownRewardSkill)
	{
		this.crownRewardSkill = crownRewardSkill;
		int maxLevel = crownRewardSkill.MaxLevel;
		this.levelLabel.text = maxLevel + "\n<size=17>Level</size>";
		bool flag = crownLevelSkill.CurrentLevel >= maxLevel;
		if (flag)
		{
			this.background.color = this.reachedColor;
			this.levelLabel.color = this.reachedLabelColor;
			this.crownOutline.color = this.crownReachedColor;
			this.bgEdge.color = this.crownReachedColor;
			this.infoBadge.color = this.crownReachedColor;
			this.infoI.color = this.reachedColor;
			this.TweenKiller();
			this.badgeHolder.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.6f, 8, 0.8f);
		}
		else if (!flag)
		{
			this.levelLabel.color = this.reachedColor;
			this.crownOutline.color = new Color(0f, 0f, 0f, 0.25f);
		}
		this.shine.SetActive(flag);
	}

	public void SetRewardBanner(int index)
	{
		Transform transform = UnityEngine.Object.Instantiate<Transform>(this.rewardBanner[index], this.rewardBannerHolder);
	}

	public void ButtonPeekLimit()
	{
		if (this.crownRewardSkill != null)
		{
			this.CrownSkillLimitManagerInstance.PeekLimit(this.crownRewardSkill, base.transform);
		}
	}

	private void TweenKiller()
	{
		this.badgeHolder.DOKill(true);
	}

	private void OnDisable()
	{
		this.TweenKiller();
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	[SerializeField]
	[Header("References")]
	private Image background;

	[SerializeField]
	private Image crownOutline;

	[SerializeField]
	private Image bgEdge;

	[SerializeField]
	private Image infoBadge;

	[SerializeField]
	private Image infoI;

	[SerializeField]
	private GameObject shine;

	[SerializeField]
	private RectTransform badgeHolder;

	[SerializeField]
	private TextMeshProUGUI levelLabel;

	[SerializeField]
	private Transform rewardBannerHolder;

	[SerializeField]
	private Transform[] rewardBanner;

	[SerializeField]
	[Header("Variables")]
	private Color notReachedColor;

	[SerializeField]
	private Color reachedColor;

	[SerializeField]
	private Color reachedLabelColor;

	[SerializeField]
	private Color crownReachedColor;

	private Skill crownRewardSkill;

	public CrownSkillLimitManager CrownSkillLimitManagerInstance;
}
