using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class BoatMovementHandler : MonoBehaviour
{
	private void Awake()
	{
		if (!TournamentManager.Instance.IsInsideTournament)
		{
			if (SwimStraight.isSimulatingBoatMovement && BoatMovementHandler.boatMovementSpeed >= 0.2f)
			{
				this.localboatMovementSpeedTimerFixer = BoatMovementHandler.boatMovementSpeed - 0.1f;
				this.DWLProgressBehaviour_OnDwProgressing();
			}
			else
			{
				this.TweenKiller(true);
			}
			DWLProgressBehaviour.OnDwProgressing += this.DWLProgressBehaviour_OnDwProgressing;
		}
		else
		{
			this.localboatMovementSpeedTimerFixer = BoatMovementHandler.boatMovementSpeed - 0.1f;
			this.StartTournamentMovement(TournamentManager.Instance.TimeLeft);
		}
	}

	private void DWLProgressBehaviour_OnDwProgressing()
	{
		SwimStraight.isSimulatingBoatMovement = true;
		this.TweenKiller(true);
		BoatMovementHandler.boatMovementSpeed = this.localboatMovementSpeedTimerFixer;
		BoatMovementHandler.waterShaderMovementFix = 2f;
		BoatMovementHandler.effectboost = 1f;
		for (int i = 0; i < this.objectsToActivateWhen.Length; i++)
		{
			this.objectsToActivateWhen[i].SetActive(true);
		}
		DOTween.To(() => BoatMovementHandler.boatMovementSpeed, delegate(float x)
		{
			BoatMovementHandler.boatMovementSpeed = x;
		}, 0f, 5f * this.localboatMovementSpeedTimerFixer).SetDelay(3f * this.localboatMovementSpeedTimerFixer).OnComplete(delegate
		{
			SwimStraight.isSimulatingBoatMovement = false;
			this.localboatMovementSpeedTimerFixer = 1f;
			for (int j = 0; j < this.objectsToActivateWhen.Length; j++)
			{
				if (this.objectsToActivateWhen[j] != null)
				{
					this.objectsToActivateWhen[j].SetActive(false);
				}
			}
		}).SetId("BoatMovementHandlerSpeedValueTweener");
		DOTween.To(() => BoatMovementHandler.waterShaderMovementFix, delegate(float x)
		{
			BoatMovementHandler.waterShaderMovementFix = x;
		}, 1f, 3f * this.localboatMovementSpeedTimerFixer).SetEase(Ease.Linear).SetId("BoatMovementHandlerwaterShaderMovementFixTweener");
	}

	private void StartTournamentMovement(float tournamentTime)
	{
		this.TweenKiller(true);
		BoatMovementHandler.boatMovementSpeed = 0.3f;
		BoatMovementHandler.waterShaderMovementFix = tournamentTime * 0.3f;
		BoatMovementHandler.effectboost = 3f;
		this.localboatMovementSpeedTimerFixer = 0.3f;
		SwimStraight.isSimulatingBoatMovement = true;
		for (int i = 0; i < this.objectsToActivateWhen.Length; i++)
		{
			this.objectsToActivateWhen[i].SetActive(true);
		}
		DOTween.To(() => BoatMovementHandler.boatMovementSpeed, delegate(float x)
		{
			BoatMovementHandler.boatMovementSpeed = x;
		}, 0f, 5f).SetDelay(tournamentTime).OnComplete(delegate
		{
			SwimStraight.isSimulatingBoatMovement = false;
			this.localboatMovementSpeedTimerFixer = 0.3f;
			for (int j = 0; j < this.objectsToActivateWhen.Length; j++)
			{
				this.objectsToActivateWhen[j].SetActive(false);
			}
		}).SetId("TournamentBoatMovementHandlerSpeedValueTweener");
		DOTween.To(() => BoatMovementHandler.waterShaderMovementFix, delegate(float x)
		{
			BoatMovementHandler.waterShaderMovementFix = x;
		}, 1f, tournamentTime).SetEase(Ease.Linear).SetId("TournamentBoatMovementHandlerwaterShaderMovementFixTweener");
	}

	private void TweenKiller(bool complete = true)
	{
		DOTween.Kill("BoatMovementHandlerSpeedValueTweener", complete);
		DOTween.Kill("BoatMovementHandlerwaterShaderMovementFixTweener", complete);
		DOTween.Kill("TournamentBoatMovementHandlerSpeedValueTweener", complete);
		DOTween.Kill("TournamentBoatMovementHandlerwaterShaderMovementFixTweener", complete);
	}

	private void OnDestroy()
	{
		DWLProgressBehaviour.OnDwProgressing -= this.DWLProgressBehaviour_OnDwProgressing;
	}

	[SerializeField]
	private GameObject[] objectsToActivateWhen;

	public static float boatMovementSpeed = 1f;

	public static float waterShaderMovementFix;

	public static float effectboost = 1f;

	private float localboatMovementSpeedTimerFixer = 1f;
}
