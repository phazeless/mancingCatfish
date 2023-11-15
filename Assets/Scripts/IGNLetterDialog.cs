using System;
using TMPro;
using UnityEngine;

public class IGNLetterDialog : InGameNotificationDialog<IGNLetter>
{
	protected override void OnAboutToOpen()
	{
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
	}

	protected override void OnOpened()
	{
	}

	protected override void OnRemovedFromList()
	{
		this.iconTween.IconTweenKiller();
	}

	protected override void OnReturned()
	{
	}

	private void Update()
	{
	}

	[SerializeField]
	private IGNPackageTween iconTween;

	[SerializeField]
	private TextMeshProUGUI day;

	[SerializeField]
	private TextMeshProUGUI text;
}
