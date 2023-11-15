using System;
using System.Collections.Generic;

public static class BackManager
{
	public static void Push(BackManager.BackItemType item)
	{
		BackManager.items.Push(item);
	}

	public static void Pop()
	{
		if (BackManager.items.Count > 0)
		{
			BackManager.items.Pop();
		}
	}

	public static bool Back()
	{
		if (BackManager.items.Count > 0)
		{
			BackManager.BackItemType backItemType = BackManager.items.Peek();
			if (backItemType == BackManager.BackItemType.Dialog)
			{
				UpgradeDialogTween currentTopDialog = DialogInteractionHandler.Instance.CurrentTopDialog;
				if (currentTopDialog != null && currentTopDialog is UIAfkFishingDialog)
				{
					DialogInteractionHandler.Instance.DisableCloseByClickingShade = false;
				}
				DialogInteractionHandler.Instance.CloseTop();
			}
			else if (backItemType == BackManager.BackItemType.Menu)
			{
				ScreenManager instance = ScreenManager.Instance;
				if (instance.CurrentScreen == ScreenManager.Screen.Map)
				{
					instance.GoToScreen(ScreenManager.Screen.Main);
				}
				else if (instance.CurrentScreen == ScreenManager.Screen.Shop)
				{
					instance.GoToScreen(ScreenManager.Screen.Main);
				}
			}
			return false;
		}
		return true;
	}

	private static Stack<BackManager.BackItemType> items = new Stack<BackManager.BackItemType>();

	public enum BackItemType
	{
		Dialog,
		Menu,
		Other
	}
}
