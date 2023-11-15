using System;
using UnityEngine;

public interface INotifyOutOfScreen
{
	void OnOutOfScreen(NotifyOutOfScreen.OutOfScreenMethod outOfScreenMethod, NotifyOutOfScreen.ListenerMode listenerMode, GameObject gameObject);
}
