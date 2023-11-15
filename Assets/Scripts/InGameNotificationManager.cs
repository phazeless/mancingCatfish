using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class InGameNotificationManager : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<UIInGameNotificationItem, InGameNotification> OnInGameNotificationCreated;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<InGameNotification> OnInGameNotificationRemoved;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<InGameNotification> OnInGameNotificationOpened;

	public static InGameNotificationManager Instance { get; private set; }

	public void NotifyIGNOpened(InGameNotification ign)
	{
		if (this.OnInGameNotificationOpened != null)
		{
			this.OnInGameNotificationOpened(ign);
		}
	}

	private void Awake()
	{
		InGameNotificationManager.Instance = this;
		this.inGameNotificationList.Init(this.maxVisibleNotifications, this.dialogCanvas, this);
		this.OnInGameNotificationCreated += this.inGameNotificationList.OnInGameNotificationCreated;
		this.OnInGameNotificationRemoved += this.inGameNotificationList.OnInGameNotificationRemoved;
		SkillManager.Instance.OnSkillsReset += this.Instance_OnSkillsReset;
		SkillManager.Instance.OnSkillUnlocked += this.Instance_OnSkillUnlocked;
		SkillManager.Instance.OnSkillLevelChanged += this.Instance_OnSkillLevelChanged;
	}

	private void Instance_OnSkillLevelChanged(Skill skill, LevelChange levelChange)
	{
		if (skill.GetExtraInfo().IsCrew && skill.CurrentLevel == 1 && levelChange == LevelChange.LevelUpFree)
		{
			InGameNotification inGameNotification = (from x in this.GetActiveNotifications(InGameNotification.IGN.NewCrew)
			where ((IGNNewCrew)x).Skill == skill
			select x).FirstOrDefault<InGameNotification>();
			if (inGameNotification != null)
			{
				this.Remove(inGameNotification);
			}
		}
	}

	private void Start()
	{
		this.LoadSavedIGNs();
		this.hasHadTheChanceToCallStart = true;
		TimeManager.Instance.OnInitializedWithInternetTime += this.Instance_OnInitializedWithInternetTime;
	}

	private void Instance_OnInitializedWithInternetTime(bool hasInternetTime, DateTime realNow)
	{
		if (hasInternetTime)
		{
			this.CheckForAndCreateTimePeriodSpecificIGNs(false);
		}
		else
		{
			this.RunAfterDelay(30f, delegate()
			{
				this.CheckForAndCreateTimePeriodSpecificIGNs(false);
			});
		}
	}

	public void CheckForAndCreateTimePeriodSpecificIGNs(bool showInfoTab = false)
	{
		for (int i = 0; i < this.timePeriodEvents.Count; i++)
		{
			EventContent eventContent = this.timePeriodEvents[i];
			if (!eventContent.HasCompletedRequiredQuest)
			{
				Quest questNeededUntilActivated = eventContent.QuestNeededUntilActivated;
				questNeededUntilActivated.OnQuestClaimed -= this.QuestNeeded_OnQuestClaimed;
				questNeededUntilActivated.OnQuestClaimed += this.QuestNeeded_OnQuestClaimed;
			}
			else if (eventContent.IsWithinEventPeriod)
			{
				IGNEventChallengesDialog igneventChallengesDialog = this.Create(eventContent.IGNEventChallengesDialogPrefab);
				igneventChallengesDialog.Init(eventContent, showInfoTab);
			}
			else
			{
				eventContent.Clear();
			}
		}
	}

	private void QuestNeeded_OnQuestClaimed(Quest obj)
	{
		this.CheckForAndCreateTimePeriodSpecificIGNs(true);
	}

	private void Instance_OnSkillUnlocked(Skill skill)
	{
		IGNNewCrew ign = new IGNNewCrew(skill, true);
		if (!skill.GetExtraInfo().IgnoreIGN)
		{
			this.Create<IGNNewCrew>(ign);
		}
	}

	private void Instance_OnSkillsReset()
	{
		List<InGameNotification> list = this.activeNotifications.FindAll((InGameNotification x) => x.RemoveOnReset);
		for (int i = 0; i < list.Count; i++)
		{
			this.Remove(list[i]);
		}
	}

	public void ClearIGNs()
	{
		for (int i = this.activeNotifications.Count - 1; i >= 0; i--)
		{
			this.Remove(this.activeNotifications[i]);
		}
	}

	public bool IsAnyIGNActiveOfType<T>() where T : InGameNotification
	{
		return (from x in this.activeNotifications
		where x.GetType() == typeof(T)
		select x).FirstOrDefault<InGameNotification>() != null;
	}

	public InGameNotificationDialog OpenFirstOccurrenceOfIGN<T>() where T : InGameNotification
	{
		InGameNotificationDialog inGameNotificationDialog = this.inGameNotificationList.ActiveUIInGameNotificationDialogsOfType<T>().FirstOrDefault<InGameNotificationDialog>();
		if (inGameNotificationDialog != null && !inGameNotificationDialog.IsOpen)
		{
			inGameNotificationDialog.Open();
		}
		return inGameNotificationDialog;
	}

	public InGameNotificationDialog OpenFirstOccurrenceOfIGN<T>(T ign) where T : InGameNotification
	{
		InGameNotificationDialog inGameNotificationDialog = this.inGameNotificationList.ActiveUIInGameNotificationDialogs.Find((InGameNotificationDialog x) => x.GetInGameNotification() == ign);
		if (inGameNotificationDialog != null && !inGameNotificationDialog.IsOpen)
		{
			inGameNotificationDialog.Open();
		}
		return inGameNotificationDialog;
	}

	public void LoadSavedIGNs()
	{
		JsonSerializerSettings settings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto
		};
		string @string = EncryptedPlayerPrefs.GetString("KEY_ACTIVE_NOTIFICATIONS");
		List<InGameNotification> list = JsonConvert.DeserializeObject<List<InGameNotification>>(EncryptedPlayerPrefs.GetString("KEY_ACTIVE_NOTIFICATIONS"), settings);
		if (list != null)
		{
			foreach (InGameNotification ign in list)
			{
				this.Create<InGameNotification>(ign);
			}
		}
	}

	public int NumberOfIGNsWithType(InGameNotification ign)
	{
		List<InGameNotification> list = this.activeNotifications.FindAll((InGameNotification x) => x.Type == ign.Type);
		int result = 0;
		if (list != null)
		{
			result = list.Count;
		}
		return result;
	}

	public void SaveIGNs()
	{
		List<InGameNotification> list = new List<InGameNotification>(this.activeNotifications);
		list.RemoveAll((InGameNotification x) => x.RemoveOnExit);
		JsonSerializerSettings settings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto
		};
		string value = JsonConvert.SerializeObject(list, Formatting.None, settings);
		EncryptedPlayerPrefs.SetString("KEY_ACTIVE_NOTIFICATIONS", value, true);
	}

	private void OnApplicationPause(bool didPause)
	{
		if (this.hasHadTheChanceToCallStart && didPause && !TournamentManager.Instance.IsInsideTournament)
		{
			this.SaveIGNs();
		}
	}

	private void Update()
	{
		for (int i = this.activeNotifications.Count - 1; i >= 0; i--)
		{
			InGameNotification inGameNotification = this.activeNotifications[i];
			if (inGameNotification.HasExpiration && DateTime.Now >= inGameNotification.Expiration)
			{
				inGameNotification.NotifyExpired();
				this.Remove(inGameNotification);
			}
		}
	}

	public List<InGameNotification> GetActiveNotifications(InGameNotification.IGN type)
	{
		return this.activeNotifications.FindAll((InGameNotification x) => x.Type == type);
	}

	public T Create<T>(T ign) where T : InGameNotification
	{
		UIInGameNotificationItem uiinGameNotificationItem = this.availableNotificationsPrefabs.Find((UIInGameNotificationItem x) => x.IGNType == ign.Type);
		if (uiinGameNotificationItem == null)
		{
			throw new InvalidOperationException("Trying to Instantiate an IGN that hasn't been added to the InGameNotificationManager!");
		}
		UIInGameNotificationItem uiinGameNotificationItem2 = UnityEngine.Object.Instantiate<UIInGameNotificationItem>(uiinGameNotificationItem);
		uiinGameNotificationItem2.Init(ign, this, this.dialogCanvas);
		this.activeNotifications.Add(uiinGameNotificationItem2.InGameNotification);
		if (this.OnInGameNotificationCreated != null)
		{
			this.OnInGameNotificationCreated(uiinGameNotificationItem2, uiinGameNotificationItem2.InGameNotification);
		}
		ign.OnCreated();
		return (T)((object)uiinGameNotificationItem2.InGameNotification);
	}

	public IGNEventChallengesDialog Create(IGNEventChallengesDialog ignDialogPrefab)
	{
		UIInGameNotificationItem uiinGameNotificationItem = UnityEngine.Object.Instantiate<UIInGameNotificationItem>(ignDialogPrefab.ParentHolder);
		InGameNotification igninstance = ignDialogPrefab.GetIGNInstance();
		uiinGameNotificationItem.Init(igninstance, this, this.dialogCanvas);
		this.activeNotifications.Add(uiinGameNotificationItem.InGameNotification);
		if (this.OnInGameNotificationCreated != null)
		{
			this.OnInGameNotificationCreated(uiinGameNotificationItem, uiinGameNotificationItem.InGameNotification);
		}
		igninstance.OnCreated();
		return (IGNEventChallengesDialog)uiinGameNotificationItem.Dialog;
	}

	public InGameNotification Remove(InGameNotification inGameNotification)
	{
		if (this.activeNotifications.Remove(inGameNotification) && this.OnInGameNotificationRemoved != null)
		{
			this.OnInGameNotificationRemoved(inGameNotification);
		}
		return inGameNotification;
	}

	public List<UIInGameNotificationItem> GetUIInGameNotificationItem(InGameNotification.IGN ignType)
	{
		return this.inGameNotificationList.GetUIItem(ignType);
	}

	private const string KEY_ACTIVE_NOTIFICATIONS = "KEY_ACTIVE_NOTIFICATIONS";

	[SerializeField]
	private int maxVisibleNotifications = 9;

	[SerializeField]
	private Transform dialogCanvas;

	[SerializeField]
	private UIInGameNotificationList inGameNotificationList;

	[SerializeField]
	private List<UIInGameNotificationItem> availableNotificationsPrefabs = new List<UIInGameNotificationItem>();

	[SerializeField]
	private List<EventContent> timePeriodEvents = new List<EventContent>();

	private List<InGameNotification> activeNotifications = new List<InGameNotification>();

	private bool hasHadTheChanceToCallStart;
}
