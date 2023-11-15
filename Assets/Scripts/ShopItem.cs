using System;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
	private void Start()
	{
		this.textContent.SetVariableText(new string[]
		{
			"10",
			"hejsam"
		});
	}

	private void Update()
	{
	}

	[SerializeField]
	private ACETextMeshPro textContent;

	private float testValue = 2456f;

	private string testcolor = "<#314214>";

	private string testEndcolor = "</color>";
}
