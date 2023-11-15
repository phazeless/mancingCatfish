using System;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class SwipeCatcher : CircleCatcher
{
	public static SwipeCatcher Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnFishSwiped;

	protected override void Awake()
	{
		base.Awake();
		SwipeCatcher.Instance = this;
	}

	protected override void Start()
	{
		base.Start();
		this.particleSplasher.gameObject.SetActive(false);
		ScreenManager.Instance.OnScreenTransitionStarted += this.Instance_OnScreenTransitionStarted;
		this.mainCamera = Camera.main;
	}

	protected override void OnFishCaught(FishBehaviour fish)
	{
		base.OnFishCaught(fish);
		if (this.OnFishSwiped != null)
		{
			this.OnFishSwiped();
		}
	}

	private void Instance_OnScreenTransitionStarted(ScreenManager.Screen to, ScreenManager.Screen from)
	{
		this.isInMenu = (to != ScreenManager.Screen.Main && to != ScreenManager.Screen.Tutorial && to != ScreenManager.Screen.Tournament);
		if (this.isInMenu)
		{
			base.gameObject.SetActive(false);
			this.DisableEffects();
		}
		else
		{
			base.gameObject.SetActive(true);
		}
	}

	protected override void Update()
	{
		if (this.isInMenu)
		{
			return;
		}
		base.Update();
		if (Input.GetMouseButtonDown(0))
		{
			this.EnableEffects();
		}
		if (Input.GetMouseButtonUp(0))
		{
			this.DisableEffects();
		}
	}

	protected void FixedUpdate()
	{
		if (this.isInMenu)
		{
			return;
		}
		if (Input.GetMouseButton(0) && !DialogInteractionHandler.Instance.HasDialogActive())
		{
			this.rb2D.MovePosition(this.mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition));
		}
	}

	private void EnableEffects()
	{
		this.particleSplasher.gameObject.SetActive(true);
		this.col2D.enabled = true;
		ParticleSystem.EmissionModule emission = this.particleSplasher.emission;
		ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
		rateOverTime.constantMax = 50f;
		emission.rateOverTime = rateOverTime;
		this.trailRenderer.Clear();
	}

	private void DisableEffects()
	{
		this.particleSplasher.gameObject.SetActive(false);
		this.col2D.enabled = false;
		ParticleSystem.EmissionModule emission = this.particleSplasher.emission;
		ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
		rateOverTime.constantMax = 0f;
		emission.rateOverTime = rateOverTime;
	}

	private void OnDisable()
	{
		this.particleSplasher.gameObject.SetActive(false);
	}

	[SerializeField]
	private ParticleSystem particleSplasher;

	[SerializeField]
	private Rigidbody2D rb2D;

	[SerializeField]
	private TrailRenderer trailRenderer;

	private bool isInMenu;

	private Camera mainCamera;
}
