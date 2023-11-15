using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseEasterEgg : MonoBehaviour
{
	public int Id
	{
		get
		{
			return this.eggId;
		}
	}

	public Sprite EggSprite
	{
		get
		{
			return this.eggImage.sprite;
		}
	}

	public Transform BigEgg
	{
		get
		{
			return this.bigEgg;
		}
	}

	public EasterEggContent Content
	{
		get
		{
			return this.content;
		}
	}

	public void InitEgg(int eggId)
	{
		this.eggId = eggId;
		base.gameObject.SetActive(true);
	}

	public virtual void ActivateVisualHolder()
	{
		this.visualHolder.SetActive(true);
	}

	public virtual void Collect()
	{
		EasterManager.Instance.MarkAsFound(this);
		this.OnEggCollected();
	}

	public abstract void OnEggCollected();

	[SerializeField]
	protected EasterEggContent content;

	[SerializeField]
	protected GameObject visualHolder;

	[SerializeField]
	protected Image eggImage;

	[SerializeField]
	protected Transform bigEgg;

	protected int eggId = -1;
}
