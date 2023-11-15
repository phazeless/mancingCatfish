using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ACE.Analytics
{
	[RequireComponent(typeof(Button))]
	public class ButtonEvent : MonoBehaviour
	{
		private void Awake()
		{
			this.button = base.GetComponent<Button>();
			this.unityAction = delegate()
			{
				this.Button_OnClick();
			};
			this.button.onClick.AddListener(this.unityAction);
		}

		private void OnDestroy()
		{
			this.button.onClick.RemoveListener(this.unityAction);
		}

		private void Button_OnClick()
		{
			if (!string.IsNullOrEmpty(this.eventID))
			{
				Analytics.PostCustomEvent(this.eventID);
			}
		}

		[SerializeField]
		private string eventID;

		private Button button;

		private UnityAction unityAction;
	}
}
