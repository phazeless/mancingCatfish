using System;
using UnityEngine;

namespace Soomla.Singletons
{
	public abstract class BaseBehaviour : MonoBehaviour
	{
		public Transform CachedTransform
		{
			get
			{
				Transform result;
				if ((result = this.cashedTransform) == null)
				{
					result = (this.cashedTransform = base.transform);
				}
				return result;
			}
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
		}

		protected virtual void OnDestroy()
		{
		}

		private Transform cashedTransform;
	}
}
