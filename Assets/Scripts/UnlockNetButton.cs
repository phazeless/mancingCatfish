using System;
using TMPro;
using UnityEngine;

public class UnlockNetButton : MonoBehaviour
{
	public void SetPrice(int price)
	{
		this.priceLabel.SetVariableText(new string[]
		{
			price.ToString()
		});
	}

	public void AddNet()
	{
	}

	[SerializeField]
	private TextMeshProUGUI priceLabel;
}
