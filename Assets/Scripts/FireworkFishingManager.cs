using System;
using DG.Tweening;
using UnityEngine;

public class FireworkFishingManager : MonoBehaviour
{
	public static FireworkFishingManager Instance { get; private set; }

	public BaseConsumable Firework
	{
		get
		{
			return this.firework;
		}
	}

	public HolidayOfferAvailability Availability
	{
		get
		{
			return this.eventContent.Availability;
		}
	}

	public bool CanWatchAdForRockets
	{
		get
		{
			return false;
		}
	}

	public int RocketsFromAdsLeft
	{
		get
		{
			return this.rocketsLeftFromAds;
		}
	}

	public string TimeUntilFireworkForAdResetAsReadableString
	{
		get
		{
			if (!TimeManager.Instance.IsInitializedWithInternetTime)
			{
				return "N/A";
			}
			DateTime realNow = TimeManager.Instance.RealNow;
			DateTime d = realNow.AddDays(1.0);
			float seconds = (float)(d - realNow).TotalSeconds;
			return FHelper.FromSecondsToHoursMinutesSecondsFormat(seconds);
		}
	}

	private void Awake()
	{
		FireworkFishingManager.Instance = this;
		this.Load();
	}

	private void Start()
	{
		this.firework.OnConsumed += this.FireworkConsumable_OnConsumed;
		this.firework.OnGranted += this.FireworkConsumable_OnGranted;
		TimeManager.Instance.OnInitializedWithInternetTime += this.Instance_OnInitializedWithInternetTime;
		this.fireworkTriggerButton.Init(this.eventContent, this.firework);
		this.UpdateUI();
		TournamentManager.Instance.OnJoinTournament += this.Instance_OnJoinTournament;
		TournamentManager.Instance.OnLeftTournament += this.Instance_OnLeftTournament;
	}

	private void Instance_OnLeftTournament(string arg1, int arg2, string arg3)
	{
		this.UpdateUI();
	}

	private void Instance_OnJoinTournament(string obj)
	{
		this.UpdateUI();
	}

	private void Instance_OnInitializedWithInternetTime(bool hasInternetRealTime, DateTime realNow)
	{
		if (this.receivedRocketsOnDay != realNow.Day)
		{
			this.receivedRocketsOnDay = realNow.Day;
			this.rocketsLeftFromAds = 2;
		}
		this.UpdateUI();
	}

	public void WatchAdForRockets()
	{
		
	}

	public void UseFirework()
	{
		if (!this.isUsingFirework && ConsumableManager.Instance.GetAmount(this.firework) > 0)
		{
			this.isUsingFirework = !this.isUsingFirework;
			this.TriggerFirework();
			this.UpdateUI();
		}
	}

	private void FireworkConsumable_OnGranted(BaseConsumable arg1, int amount)
	{
		this.UpdateUI();
	}

	private void FireworkConsumable_OnConsumed(BaseConsumable arg1, int amount)
	{
		this.isUsingFirework = false;
		this.ToggleShootToast();
		this.UpdateUI();
	}

	private void UpdateUI()
	{
		this.fireworkTriggerButton.UpdateUI();
	}

	private void TriggerFirework()
	{
		ColorChangerHandler.FireworkModeColorStart();
		CameraMovement.Instance.FireworkZoomStart();
		FireworkBehaviour fireworkInstance = UnityEngine.Object.Instantiate<FireworkBehaviour>(this.fireworkPrefab, base.transform);
		fireworkInstance.AnimateFirework(Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.1f, 0.95f), UnityEngine.Random.Range(0.8f, 0.95f), 10f)));
		this.RunAfterDelay(0.1f, delegate()
		{
			fireworkInstance = UnityEngine.Object.Instantiate<FireworkBehaviour>(this.fireworkPrefab, this.transform);
			fireworkInstance.AnimateFirework(Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.05f, 0.1f), UnityEngine.Random.Range(0.4f, 0.65f), 10f)));
		});
		this.RunAfterDelay(0.2f, delegate()
		{
			fireworkInstance = UnityEngine.Object.Instantiate<FireworkBehaviour>(this.fireworkPrefab, this.transform);
			fireworkInstance.AnimateFirework(Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.4f, 0.7f), 0.15f, 10f)));
		});
		this.RunAfterDelay(0.4f, delegate()
		{
			fireworkInstance = UnityEngine.Object.Instantiate<FireworkBehaviour>(this.fireworkPrefab, this.transform);
			fireworkInstance.AnimateFirework(Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.85f, 0.95f), UnityEngine.Random.Range(0.35f, 0.6f), 10f)));
			AudioManager.Instance.OneShooter(this.crowdAwe[UnityEngine.Random.Range(0, this.crowdAwe.Length)], 0.2f);
		});
		this.RunAfterDelay(0.7f, delegate()
		{
			fireworkInstance = UnityEngine.Object.Instantiate<FireworkBehaviour>(this.fireworkPrefab, this.transform);
			fireworkInstance.AnimateFirework(Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.4f, 0.7f), 0.25f, 10f)));
		});
		this.RunAfterDelay(1.5f, delegate()
		{
			ScreenParticleEffect.Instance.StarSwipe(0.5f, 1f);
			ColorChangerHandler.FireworkModeColorEnd();
			CameraMovement.Instance.FireworkZoomEnd();
		});
		ConsumableManager.Instance.Consume(this.firework, 1, ResourceChangeReason.UseEventAbility);
	}

	private void ToggleShootToast()
	{
		this.fireworkTapToShoot.gameObject.SetActive(this.isUsingFirework);
		if (this.isUsingFirework)
		{
			this.fireworkTapToShoot.DOScaleY(1f, 0.15f);
			this.fireworkTapToShoot.DOShakePosition(0.8f, new Vector3(0f, 12f, 0f), 10, 90f, false, true).SetLoops(-1);
		}
		else
		{
			this.fireworkTapToShoot.localScale = new Vector3(1f, 0f, 1f);
			this.TweenKiller();
		}
	}

	private void Save()
	{
		EncryptedPlayerPrefs.SetInt("KEY_RECEIVED_ROCKETS_ON_DAY", this.receivedRocketsOnDay, true);
		EncryptedPlayerPrefs.SetInt("KEY_RCCKETS_LEFT_FROM_ADS", this.rocketsLeftFromAds, true);
	}

	private void Load()
	{
		this.receivedRocketsOnDay = EncryptedPlayerPrefs.GetInt("KEY_RECEIVED_ROCKETS_ON_DAY", this.receivedRocketsOnDay);
		this.rocketsLeftFromAds = EncryptedPlayerPrefs.GetInt("KEY_RCCKETS_LEFT_FROM_ADS", this.rocketsLeftFromAds);
	}

	private void OnDestroy()
	{
		this.TweenKiller();
		TournamentManager.Instance.OnJoinTournament -= this.Instance_OnJoinTournament;
		TournamentManager.Instance.OnLeftTournament -= this.Instance_OnLeftTournament;
	}

	private void TweenKiller()
	{
		this.fireworkTapToShoot.DOKill(true);
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			this.Save();
		}
	}

	[SerializeField]
	private BaseConsumable firework;

	[SerializeField]
	private EventContent eventContent;

	[SerializeField]
	private FireworkBehaviour fireworkPrefab;

	[SerializeField]
	private FireworkTriggerButton fireworkTriggerButton;

	[SerializeField]
	private Transform fireworkTapToShoot;

	[SerializeField]
	private AudioClip[] crowdAwe;

	private bool isUsingFirework;

	public const int ROCKETS_TO_RECEIVE_FROM_AD = 1;

	public const int MAX_ROCKETS_FROM_ADS_PER_DAY = 2;

	private const string KEY_RECEIVED_ROCKETS_ON_DAY = "KEY_RECEIVED_ROCKETS_ON_DAY";

	private const string KEY_RCCKETS_LEFT_FROM_ADS = "KEY_RCCKETS_LEFT_FROM_ADS";

	private int receivedRocketsOnDay = -1;

	private int rocketsLeftFromAds = 2;
}
