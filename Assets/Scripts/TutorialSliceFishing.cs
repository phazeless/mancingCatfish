using System;
using System.Diagnostics;

public class TutorialSliceFishing : TutorialSliceBase
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnCompleted;

	protected override void Awake()
	{
		base.Awake();
		TutorialSliceFishing.Instance = this;
	}

	private void Start()
	{
		if (this.started)
		{
			return;
		}
		this.started = true;
		ScreenManager.Instance.isFirstTime = true;
		this.RunAfterDelay(3f, delegate()
		{
			this.DelayedEnter();
		});
	}

	private void DelayedEnter()
	{
		CharacterConversationHandler.Instance.TutorialFishing();
		TutorialManager.Instance.SetGraphicRaycaster(true);
	}

	protected override void Entered()
	{
	}

	protected override void Exited()
	{
		if (this.OnCompleted != null)
		{
			this.OnCompleted();
		}
	}

	protected override void Setup()
	{
		base.Setup();
		CharacterConversationHandler.Instance.OnConversationCompleted += this.Instance_OnConversationCompleted;
	}

	private void Instance_OnConversationCompleted(int conversationID)
	{
		if (conversationID == 0)
		{
			base.Exit(true);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		CharacterConversationHandler.Instance.OnConversationCompleted -= this.Instance_OnConversationCompleted;
	}

	public static TutorialSliceFishing Instance;

	private int swipesToComplete = 3;

	private int swipes;

	private bool started;
}
