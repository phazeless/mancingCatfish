using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StarGainEffect : MonoBehaviour
{
	public void GainStars(int amount)
	{
		this.starsLabel.SetText((this.starSkill.CurrentLevel - amount).ToString());
		AudioManager.Instance.MenuWhoosh();
		for (int i = 0; i < amount; i++)
		{
			Transform starInstance = UnityEngine.Object.Instantiate<Transform>(this.starPrefab, this.starHolder, false);
			starInstance.localPosition = Vector2.zero;
			starInstance.transform.DOLocalMove(new Vector3((float)UnityEngine.Random.Range(-2, 2), (float)UnityEngine.Random.Range(-2, 2), starInstance.transform.position.y), 0.4f, false).SetLoops(2, LoopType.Yoyo).OnComplete(delegate
			{
				this.RunAfterDelay(0.1f, delegate()
				{
					UnityEngine.Object.Destroy(starInstance.gameObject);
					this.starsLabel.SetText(this.starSkill.CurrentLevel.ToString());
					this.audioSource.Play();
					Vector3 localScale = this.bookShineEffect.localScale;
					this.bookShineEffect.DOScale(6f * localScale + new Vector3(1f, 0f, 0f), 0.2f);
					this.bookShineEffect.GetComponent<SpriteRenderer>().DOFade(0f, 0.2f);
				});
				this.RunAfterDelay(1f, delegate()
				{
					UnityEngine.Object.Destroy(this.gameObject);
				});
			});
		}
	}

	[SerializeField]
	private Transform starPrefab;

	[SerializeField]
	private Transform starHolder;

	[SerializeField]
	private Transform bookShineEffect;

	[SerializeField]
	private TextMeshPro starsLabel;

	[SerializeField]
	private Skill starSkill;

	[SerializeField]
	private AudioSource audioSource;
}
