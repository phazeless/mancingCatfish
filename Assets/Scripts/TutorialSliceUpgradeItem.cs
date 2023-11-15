using System;
using System.Diagnostics;

public class TutorialSliceUpgradeItem : TutorialSliceBase
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnCompleted;

	protected override void Entered()
	{
	}

	protected override void Setup()
	{
		base.Setup();
		TutorialSliceUpgradeItem.Instance = this;
	}

	public void FirstTimeUpgrade()
	{
		this.Enter();
		TutorialManager.Instance.SetGraphicRaycaster(true);
	}

	public void LeveledUp()
	{
		base.Exit(true);
	}

	public void AttemptLevelUp()
	{
		UIEquipmentDialog componentInChildren = DialogInteractionHandler.Instance.gameObject.GetComponentInChildren<UIEquipmentDialog>();
		if (componentInChildren != null)
		{
			componentInChildren.Upgrade();
		}
	}

	protected override void Exited()
	{
		if (this.OnCompleted != null)
		{
			this.OnCompleted();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		TutorialSliceUpgradeItem.Instance = null;
	}

	public static TutorialSliceUpgradeItem Instance;
}
