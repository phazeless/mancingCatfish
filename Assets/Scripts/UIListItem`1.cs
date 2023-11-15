using System;
using UnityEngine;

public abstract class UIListItem<T> : UIListItem where T : IListItemContent
{
	protected virtual void Awake()
	{
		this.mainCamera = Camera.main;
		this.viewPort = (RectTransform)base.transform.parent.parent;
		this.itemHeight = ((RectTransform)base.transform).rect.height;
		this.holderObject = base.transform.GetChild(0).gameObject;
		this.cachedRect = (RectTransform)base.transform;
	}

	protected virtual void Start()
	{
	}

	private void Instance_OnCameraOrthographicSizeChanged(float arg1, float arg2)
	{
	}

	protected virtual void Update()
	{
		this.cachedRect.pivot = this.newPivot;
		this.rectScreenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, this.cachedRect.position);
		this.itemHeight = this.cachedRect.rect.height;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewPort, this.rectScreenPoint, this.mainCamera, out this.rectPositionRelativeToViewPort);
		this.bottomPosition = -this.viewPort.rect.height;
		if (this.rectPositionRelativeToViewPort.y > this.itemHeight || this.rectPositionRelativeToViewPort.y < this.bottomPosition)
		{
			if (this.holderObject.activeInHierarchy)
			{
				this.holderObject.SetActive(false);
			}
		}
		else if (this.rectPositionRelativeToViewPort.y < this.itemHeight && this.rectPositionRelativeToViewPort.y > this.bottomPosition && !this.holderObject.activeInHierarchy)
		{
			this.holderObject.SetActive(true);
		}
	}

	public override void SetSizes(float pixelRelation)
	{
		if (this.viewPort == null)
		{
			this.viewPort = (RectTransform)base.transform.parent.parent;
			UnityEngine.Debug.LogWarning("viewPort was null. Setting it now to prevent errors, but should be fixed for real (eg. find out why it's happening)", this);
		}
		Vector3[] array = new Vector3[4];
		this.viewPort.GetWorldCorners(array);
		this.heightInWorldCoordinates = 2f * this.itemHeight * pixelRelation;
		this.topPositionPlusHeight = array[1].y + this.heightInWorldCoordinates;
		this.bottomPosition = array[0].y - this.itemHeight * pixelRelation;
	}

	public abstract void OnUpdateUI(T content);

	public override void OnUpdateUI(IListItemContent content)
	{
		this.OnUpdateUI((T)((object)content));
	}

	public override void OnAddedToPool(ObjectPool<UIListItem> pool)
	{
		this.pool = pool;
	}

	public override void OnRetrieved(ObjectPool<UIListItem> pool)
	{
	}

	public override void OnReturned(ObjectPool<UIListItem> pool)
	{
	}

	protected ObjectPool<UIListItem> pool;

	private float itemHeight;

	private float heightInWorldCoordinates;

	private float topPositionPlusHeight;

	private float bottomPosition;

	private RectTransform viewPort;

	private GameObject holderObject;

	private RectTransform cachedRect;

	private Vector2 rectPositionRelativeToViewPort = Vector2.zero;

	private Vector2 rectScreenPoint = Vector2.zero;

	private Vector2 newPivot = new Vector2(0f, 1f);

	private Camera mainCamera;
}
