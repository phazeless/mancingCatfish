using System;
using UnityEngine;
using UnityEngine.Events;

public class DialogCloseHelper : MonoBehaviour
{
	public void Close()
	{
		this.CloseTweenScript.Invoke();
	}

	[SerializeField]
	private UnityEvent CloseTweenScript;
}
