using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnTutorialSetup;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<TutorialSliceBase> OnEnterTutorial;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<TutorialSliceBase> OnExitedTutorial;

	private void Awake()
	{
		TutorialManager.Instance = this;
		string @string = EncryptedPlayerPrefs.GetString(TutorialManager.KEY_COMPLETED_SLICES, null);
		if (@string == null)
		{
			foreach (TutorialSliceBase tutorialSliceBase in this.listOfTutorialSlices)
			{
				if (PlayerPrefs.GetInt(tutorialSliceBase.Id, 0) == 1)
				{
					this.completedSlices.Add(tutorialSliceBase.Id);
				}
			}
			this.listOfTutorialSlices.RemoveAll((TutorialSliceBase x) => PlayerPrefs.GetInt(x.Id, 0) == 1);
		}
		else
		{
			this.completedSlices = JsonConvert.DeserializeObject<List<string>>(@string);
			using (List<string>.Enumerator enumerator2 = this.completedSlices.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					string sliceId = enumerator2.Current;
					this.listOfTutorialSlices.RemoveAll((TutorialSliceBase x) => x.Id == sliceId);
				}
			}
		}
		if (this.slicesCompleted >= this.listOfTutorialSlices.Count)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		foreach (TutorialSliceBase original in this.listOfTutorialSlices)
		{
			TutorialSliceBase tutorialSliceBase2 = UnityEngine.Object.Instantiate<TutorialSliceBase>(original, this.sliceParent, false);
			tutorialSliceBase2.RegisterListener();
			tutorialSliceBase2.OnEnterTutorial += this.TutorialSliceInstance_OnEnterTutorial;
			tutorialSliceBase2.OnExitedTutorial += this.TutorialSliceInstance_OnExitedTutorial;
			this.instantiatedSlices.Add(tutorialSliceBase2);
		}
	}

	private void TutorialSliceInstance_OnEnterTutorial(TutorialSliceBase slice)
	{
		if (this.OnEnterTutorial != null)
		{
			this.OnEnterTutorial(slice);
		}
	}

	private void TutorialSliceInstance_OnExitedTutorial(TutorialSliceBase slice)
	{
		if (this.OnExitedTutorial != null)
		{
			this.OnExitedTutorial(slice);
		}
	}

	public void SetGraphicRaycaster(bool state)
	{
		if (state)
		{
			this.activeTutorials++;
		}
		else if (this.activeTutorials > 0)
		{
			this.activeTutorials--;
		}
		if (this.activeTutorials > 0)
		{
			base.GetComponent<GraphicRaycaster>().enabled = true;
		}
		else
		{
			base.GetComponent<GraphicRaycaster>().enabled = false;
		}
	}

	private void Start()
	{
		if (this.OnTutorialSetup != null)
		{
			this.OnTutorialSetup();
		}
	}

	private void OnApplicationPause(bool didPause)
	{
		if (didPause)
		{
			this.SaveTutorialCompleteStates();
		}
	}

	public bool IsTutorialSliceCompleted(string id)
	{
		return this.completedSlices.Contains(id);
	}

	public void TutorialSliceCompleted(string id)
	{
		foreach (TutorialSliceBase tutorialSliceBase in this.instantiatedSlices)
		{
			if (tutorialSliceBase.Id == id)
			{
				tutorialSliceBase.HasCompleted = true;
			}
		}
		this.completedSlices.Add(id);
	}

	public void SaveTutorialCompleteStates()
	{
		foreach (TutorialSliceBase tutorialSliceBase in this.instantiatedSlices)
		{
			EncryptedPlayerPrefs.SetInt(tutorialSliceBase.Id, (!tutorialSliceBase.HasCompleted) ? 0 : 1, true);
		}
		EncryptedPlayerPrefs.SetString(TutorialManager.KEY_COMPLETED_SLICES, JsonConvert.SerializeObject(this.completedSlices), true);
	}

	private static readonly string KEY_COMPLETED_SLICES = "KEY_COMPLETED_SLICES";

	private int slicesCompleted;

	[SerializeField]
	private List<TutorialSliceBase> listOfTutorialSlices = new List<TutorialSliceBase>();

	private List<TutorialSliceBase> instantiatedSlices = new List<TutorialSliceBase>();

	[SerializeField]
	private Transform sliceParent;

	[SerializeField]
	private bool isDisableTutorial;

	private List<string> completedSlices = new List<string>();

	private int activeTutorials;
}
