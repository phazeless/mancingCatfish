using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoxTweener : MonoBehaviour
{
	public void Setup(ItemChest chest, List<Item> items)
	{
		items.Sort((Item y, Item x) => x.Rarity.CompareTo(y.Rarity));
		this.itemsToRecieve = new Stack<Item>(items);
		this.OpenBox(items);
	}

	public void OpenBox(List<Item> items)
	{
		this.BackgroundFade.rectTransform.sizeDelta = new Vector2((float)Screen.width, (float)Screen.height);
		this.BackgroundFade.transform.localScale = Vector3.one * 3f;
		this.BackgroundFade.gameObject.SetActive(true);
		this.BackgroundFade.DOFade(0.95f, 0.3f);
		this.boxAnimation.Stop();
		this.boxAnimation.clip = this.boxEnterAnimation;
		this.boxAnimation.Play();
		this.recievedItemGridInstance = UnityEngine.Object.Instantiate<GameObject>(this.recievedItemGridPrefab, base.transform);
		this.RunAfterDelay(1f, delegate()
		{
			this.NextItem();
			AudioManager.Instance.SetGlimmerLoop(this.audioGlimmer);
		});
		this.RunAfterDelay(0.5f, delegate()
		{
			AudioManager.Instance.OneShooter(this.audioBump, 1f);
		});
		this.RunAfterDelay(0.8f, delegate()
		{
			AudioManager.Instance.OneShooter(this.audioUnlock, 1f);
		});
	}

	public void NextBox()
	{
		if (!this.opened)
		{
			return;
		}
		if (this.itemsToRecieve.Count > 0)
		{
			this.NextItem();
			this.boxAnimation.Stop();
			this.boxAnimation.clip = this.boxNextAnimation;
			this.boxAnimation.Play();
			return;
		}
		this.ExitBoxOpening();
	}

	private void NextItem()
	{
		if (this.itemsToRecieve.Count > 0)
		{
			this.itemBackground.color = this.rarityColors[(int)this.itemsToRecieve.Peek().Rarity];
			this.itemImage.sprite = this.itemsToRecieve.Peek().Icon;
			this.itemsListed++;
			Transform transform = this.recievedItemGridInstance.transform.GetChild(0).GetChild(0).GetChild(0).transform;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.recievedItemPrefab, transform);
			gameObject.transform.GetChild(2).GetComponent<Image>().color = this.rarityColors[(int)this.itemsToRecieve.Peek().Rarity];
			gameObject.transform.GetChild(3).GetComponent<Image>().sprite = this.itemsToRecieve.Peek().Icon;
			if (this.itemsListed <= 8)
			{
				RectTransform component = this.recievedItemGridInstance.GetComponent<RectTransform>();
				component.DOKill(true);
				component.anchoredPosition += new Vector2(80f, 0f);
				component.DOAnchorPosX(component.anchoredPosition.x - 80f, 0.3f, false);
			}
			gameObject.transform.localScale = Vector3.zero;
			gameObject.transform.DOScale(1f, 1f).SetEase(Ease.OutElastic).SetDelay(0.3f);
			this.tweensToKill.Add(gameObject.transform);
			this.itemsToRecieve.Pop();
		}
		if (this.itemsToRecieve.Count <= 0)
		{
			this.shines.SetActive(false);
			AudioManager.Instance.StopGlimmerLoop();
		}
		this.itemAnimation.Stop();
		this.itemAnimation.clip = this.itemNextAnimation;
		this.itemAnimation.Play();
		AudioManager.Instance.OneShooter(this.audioWhoosh, 1f);
		base.CancelInvoke("PlayAudioSwish");
		base.Invoke("PlayAudioSwish", 0.2f);
		this.opened = true;
	}

	private void PlayAudioSwish()
	{
		AudioClip clip = this.audioSwishes[UnityEngine.Random.Range(0, this.audioSwishes.Length)];
		AudioManager.Instance.OneShooter(clip, 1f);
	}

	private void ExitBoxOpening()
	{
		this.boxAnimation.Stop();
		this.itemAnimation.Stop();
		base.CancelInvoke("PlayAudioSwish");
		this.boxAnimation.transform.DOScale(0f, 0.2f);
		this.itemAnimation.transform.DOScale(0f, 0.2f);
		this.recievedItemGridInstance.transform.DOScale(0f, 0.2f);
		this.recievedItemGridInstance.GetComponent<RectTransform>().DOAnchorPosX(this.recievedItemGridInstance.GetComponent<RectTransform>().anchoredPosition.x + (float)Screen.width * 0.75f, 0.2f, false);
		OpenChestVisualManager.Instance.ChestOpeningFinished();
		this.BackgroundFade.DOFade(0f, 0.3f).OnComplete(delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		});
		this.opened = false;
	}

	private void OnDestroy()
	{
		this.boxAnimation.DOKill(true);
		this.itemAnimation.DOKill(true);
		this.BackgroundFade.DOKill(true);
		this.recievedItemGridInstance.transform.DOKill(false);
		this.recievedItemGridInstance.GetComponent<RectTransform>().DOKill(false);
		foreach (Transform target in this.tweensToKill)
		{
			target.DOKill(false);
		}
	}

	[SerializeField]
	private AnimationClip boxEnterAnimation;

	[SerializeField]
	private AnimationClip boxNextAnimation;

	[SerializeField]
	private AnimationClip itemNextAnimation;

	[SerializeField]
	private Animation boxAnimation;

	[SerializeField]
	private Animation itemAnimation;

	[SerializeField]
	private GameObject shines;

	[SerializeField]
	private ParticleSystem passiveGlimmer;

	[SerializeField]
	private Image BackgroundFade;

	[SerializeField]
	private Image itemRarityColorEdge;

	[SerializeField]
	private Image itemBackground;

	[SerializeField]
	private Image itemImage;

	[SerializeField]
	private GameObject recievedItemGridPrefab;

	private GameObject recievedItemGridInstance;

	[SerializeField]
	private GameObject recievedItemPrefab;

	[SerializeField]
	[Header("Audio")]
	private AudioClip audioGlimmer;

	[SerializeField]
	private AudioClip audioWhoosh;

	[SerializeField]
	private AudioClip[] audioSwishes;

	[SerializeField]
	private AudioClip audioUnlock;

	[SerializeField]
	private AudioClip audioBump;

	private Stack<Item> itemsToRecieve;

	private bool opened;

	private Color[] rarityColors = new Color[]
	{
		new Color(0.376f, 0.722f, 0.773f),
		new Color(0.325f, 0.345f, 0.714f),
		new Color(0.729f, 0.259f, 0.463f)
	};

	private List<Transform> tweensToKill = new List<Transform>();

	private int itemsListed;
}
