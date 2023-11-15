using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillListItem : UIListItem<test>
{
	public override void OnShouldRegisterListeners()
	{
	}

	public override void OnShouldUnregisterListeners()
	{
	}

	public override void OnUpdateUI(test content)
	{
		this.text.text = content.text;
	}

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private Image image;
}
