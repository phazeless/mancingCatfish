using System;
using UnityEngine;

public class WebLink : MonoBehaviour
{
	public void GoToLink()
	{
		Application.OpenURL(this.url);
	}

	[SerializeField]
	private string url;
}
