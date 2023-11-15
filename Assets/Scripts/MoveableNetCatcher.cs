using System;
using UnityEngine;

public class MoveableNetCatcher : CircleCatcher
{
	protected override void Start()
	{
		base.Start();
		this.mainCamera = Camera.main;
	}

	protected override void Update()
	{
		base.Update();
		if (DialogInteractionHandler.Instance.HasDialogActive())
		{
			return;
		}
		if (!this.isHoldingCatcher && Input.GetMouseButtonDown(0))
		{
			this.catcherWorldPos = this.mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
			if (this.cc2d.bounds.Contains(this.catcherWorldPos))
			{
				this.isHoldingCatcher = true;
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			this.isHoldingCatcher = false;
		}
	}

	protected void FixedUpdate()
	{
		if (this.isHoldingCatcher)
		{
			this.catcherWorldPos = this.mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
			this.rb2d.MovePosition(this.catcherWorldPos);
		}
	}

	[SerializeField]
	private Rigidbody2D rb2d;

	[SerializeField]
	private CircleCollider2D cc2d;

	private bool isHoldingCatcher;

	private Vector2 catcherWorldPos = Vector2.zero;

	private Camera mainCamera;
}
