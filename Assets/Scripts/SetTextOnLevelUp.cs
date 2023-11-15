using System;
using TMPro;
using UnityEngine;

[Serializable]
public class SetTextOnLevelUp : ILevelUpListener
{
	public void OnLevelUp(GameObject caller, Skill skill)
	{
		this.text.SetVariableText(new string[]
		{
			skill.CurrentLevel.ToString()
		});
	}

	[SerializeField]
	private TextMeshProUGUI text;
}
