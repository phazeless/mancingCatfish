using System;
using TMPro;
using UnityEngine;

public class CrownExpMeter : MonoBehaviour
{
	private void Start()
	{
		this.maxWidth = this.fillMeter.sizeDelta.x;
		this.UpdateUi();
	}

	public void UpdateUi()
	{
		float num = 1f;
		float num2 = 5f;
		this.extLabel.SetVariableText(new string[]
		{
			((int)num).ToString(),
			((int)num2).ToString()
		});
		this.fillMeter.sizeDelta = new Vector2(this.maxWidth * (num / num2), this.fillMeter.sizeDelta.y);
	}

	[SerializeField]
	private RectTransform fillMeter;

	[SerializeField]
	private TextMeshProUGUI extLabel;

	private float maxWidth;
}
