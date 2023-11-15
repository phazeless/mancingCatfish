using System;
using System.Numerics;
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private void Awake()
	{
		AudioManager.Instance = this;
		this.musicBaseVolume = this.musicAudioSource.volume;
	}

	private void Start()
	{
		ResourceManager.Instance.OnResourceChanged += this.Instance_OnResourceChanged;
	}

	private void Instance_OnResourceChanged(ResourceType resourceType, BigInteger arg2, BigInteger arg3)
	{
		if (resourceType == ResourceType.Gems)
		{
			this.GemGain();
		}
	}

	public void FishJump()
	{
		int num = UnityEngine.Random.Range(0, this.fishJump_0.Length);
		this.effectAudioSource1.clip = this.fishJump_0[num];
		this.effectAudioSource1.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
		this.effectAudioSource1.Play();
	}

	public void FishLand()
	{
		this.effectAudioSource2.clip = this.fishLand_0;
		this.effectAudioSource2.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
		this.effectAudioSource2.Play();
	}

	public void NavigationEnter()
	{
		this.navigationAudioSource.clip = this.navigationEnter;
		this.navigationAudioSource.volume = 1f;
		this.navigationAudioSource.pitch = 1f;
		this.navigationAudioSource.Play();
		this.MuffleAmbientAudio();
	}

	public void HarborEnter()
	{
		this.navigationAudioSource.clip = this.harborEnter;
		this.navigationAudioSource.volume = 0.2f;
		this.navigationAudioSource.pitch = 1f;
		this.navigationAudioSource.Play();
		this.MuffleAmbientAudio();
	}

	public void MainEnter()
	{
		this.UnMuffleAmbientAudio();
	}

	public void MenuClick()
	{
		this.oneShoter.PlayOneShot(this.menuClick);
	}

	public void GemGain()
	{
		this.oneShoter.PlayOneShot(this.gemGain);
	}

	public void MenuWhoosh()
	{
		this.oneShoter.PlayOneShot(this.menuWhoosh);
	}

	public void MuffleAmbientAudio()
	{
		this.ambientWaterAusioSource.volume = 0.2f;
		this.ambientWaterAusioSource.pitch = 0.8f;
	}

	public void UnMuffleAmbientAudio()
	{
		this.ambientWaterAusioSource.volume = 0.3f;
		this.ambientWaterAusioSource.pitch = 1f;
	}

	public void PlaySpecial(AudioClip PlaySpecial)
	{
		this.specialAudioSource.loop = false;
		this.specialAudioSource.clip = PlaySpecial;
		this.specialAudioSource.pitch = 1f;
		this.specialAudioSource.volume = 1f;
		this.specialAudioSource.Play();
	}

	public void PickupStuffFromWater()
	{
		this.oneShoter.PlayOneShot(this.pickupStuffFromWater);
	}

	public void BossTimeStart()
	{
		DOTween.To(() => this.musicAudioSource.volume, delegate(float x)
		{
			this.musicAudioSource.volume = x;
		}, 0f, 1f);
	}

	public void BossTimeEnd()
	{
		DOTween.To(() => this.musicAudioSource.volume, delegate(float x)
		{
			this.musicAudioSource.volume = x;
		}, this.musicBaseVolume, 1f);
	}

	public void BossStormAudio()
	{
		this.oneShoter.PlayOneShot(this.bossStormSound);
	}

	public void BossCaught()
	{
		this.PlaySpecial(this.bossCaught);
	}

	public void BossLanding()
	{
		this.PlaySpecial(this.bossLanding);
	}

	public void Cacthing()
	{
		AudioManager.Instance.PlaySpecial(this.cacthing);
	}

	public void GemSpawn()
	{
		this.oneShoter.PlayOneShot(this.gemSpawn);
	}

	public void OneShooter(AudioClip clip, float volume = 1f)
	{
		this.oneShoter.PlayOneShot(clip, volume);
	}

	public void StartSpecific(AudioClip Clip)
	{
		this.specificAudioSource.clip = Clip;
		this.specificAudioSource.Play();
	}

	public void StopSpecific(AudioClip Clip)
	{
		if (this.specificAudioSource.clip == Clip)
		{
			this.specificAudioSource.Stop();
		}
	}

	public void SetGlimmerLoop(AudioClip PlaySpecial)
	{
		this.specialAudioSource.loop = true;
		this.specialAudioSource.clip = PlaySpecial;
		this.specialAudioSource.pitch = 1f;
		this.specialAudioSource.volume = 1f;
		this.specialAudioSource.Play();
	}

	public void StopGlimmerLoop()
	{
		this.specialAudioSource.loop = false;
		this.specialAudioSource.Stop();
	}

	public static AudioManager Instance;

	[SerializeField]
	private AudioSource ambientWaterAusioSource;

	[SerializeField]
	private AudioSource musicAudioSource;

	[SerializeField]
	private AudioSource effectAudioSource1;

	[SerializeField]
	private AudioSource effectAudioSource2;

	[SerializeField]
	private AudioSource navigationAudioSource;

	[SerializeField]
	private AudioSource specialAudioSource;

	[SerializeField]
	private AudioSource specificAudioSource;

	[SerializeField]
	private AudioSource oneShoter;

	[SerializeField]
	private AudioClip[] fishJump_0;

	[SerializeField]
	private AudioClip fishLand_0;

	[SerializeField]
	private AudioClip navigationEnter;

	[SerializeField]
	private AudioClip harborEnter;

	[SerializeField]
	private AudioClip menuClick;

	[SerializeField]
	private AudioClip menuWhoosh;

	[SerializeField]
	private AudioClip cacthing;

	[SerializeField]
	private AudioClip gemGain;

	[SerializeField]
	private AudioClip bossCaught;

	[SerializeField]
	private AudioClip bossLanding;

	[SerializeField]
	private AudioClip gemSpawn;

	[SerializeField]
	private AudioClip pickupStuffFromWater;

	[SerializeField]
	private AudioClip bossStormSound;

	private float musicBaseVolume;
}
