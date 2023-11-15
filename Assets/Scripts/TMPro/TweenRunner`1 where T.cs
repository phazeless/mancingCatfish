using System;
using System.Collections;
using UnityEngine;

namespace TMPro
{
	internal class TweenRunner<T> where T : struct, ITweenValue
	{
		private static IEnumerator Start(T tweenInfo)
		{
			if (!tweenInfo.ValidTarget())
			{
				yield break;
			}
			float elapsedTime = 0f;
			while (elapsedTime < tweenInfo.duration)
			{
				elapsedTime += ((!tweenInfo.ignoreTimeScale) ? Time.deltaTime : Time.unscaledDeltaTime);
				float percentage = Mathf.Clamp01(elapsedTime / tweenInfo.duration);
				tweenInfo.TweenValue(percentage);
				yield return null;
			}
			tweenInfo.TweenValue(1f);
			yield break;
		}

		public void Init(MonoBehaviour coroutineContainer)
		{
			this.m_CoroutineContainer = coroutineContainer;
		}

		public void StartTween(T info)
		{
			if (this.m_CoroutineContainer == null)
			{
				UnityEngine.Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
				return;
			}
			this.StopTween();
			if (!this.m_CoroutineContainer.gameObject.activeInHierarchy)
			{
				info.TweenValue(1f);
				return;
			}
			this.m_Tween = TweenRunner<T>.Start(info);
			this.m_CoroutineContainer.StartCoroutine(this.m_Tween);
		}

		public void StopTween()
		{
			if (this.m_Tween != null)
			{
				this.m_CoroutineContainer.StopCoroutine(this.m_Tween);
				this.m_Tween = null;
			}
		}

		protected MonoBehaviour m_CoroutineContainer;

		protected IEnumerator m_Tween;
	}
}
