using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soomla.Singletons
{
	public abstract class UnitySingleton : BaseBehaviour
	{
		 protected bool IsInstanceReady {  get; private set; }

		private void RegisterAsSingleInstanceAndInit()
		{
			UnitySingleton.instances.Add(base.GetType(), this);
			this.InnerInit();
		}

		private void InnerInit()
		{
			this.InitAfterRegisteringAsSingleInstance();
			if (this.DontDestroySingleton)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		private static S GetOrCreateInstanceOnGameObject<S>(Type type) where S : CodeGeneratedSingleton
		{
			S s = (S)((object)null);
			GameObject gameObject = Resources.Load<GameObject>(type.Name);
			if (gameObject)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
				if (!gameObject2)
				{
					throw new Exception("Failed to instantiate prefab: " + type.Name);
				}
				s = gameObject2.GetComponent<S>();
				if (!s)
				{
					s = gameObject2.AddComponent<S>();
				}
			}
			if (!s)
			{
				s = new GameObject(type.Name).AddComponent<S>();
			}
			return s;
		}

		private void NotifyInstanceListeners()
		{
			Type type = base.GetType();
			if (UnitySingleton.instanceListeners.ContainsKey(type))
			{
				foreach (KeyValuePair<MonoBehaviour, Action<UnitySingleton>> keyValuePair in UnitySingleton.instanceListeners[type].ToArray<KeyValuePair<MonoBehaviour, Action<UnitySingleton>>>())
				{
					if (keyValuePair.Key && keyValuePair.Value != null)
					{
						keyValuePair.Value(this);
					}
					UnitySingleton.instanceListeners[type].Remove(keyValuePair.Key);
				}
			}
		}

		protected void DeclareAsReady()
		{
			this.IsInstanceReady = true;
			this.NotifyInstanceListeners();
		}

		protected static S GetSynchronousCodeGeneratedInstance<S>() where S : CodeGeneratedSingleton
		{
			Type typeFromHandle = typeof(S);
			S s;
			if (!UnitySingleton.instances.ContainsKey(typeFromHandle))
			{
				s = UnityEngine.Object.FindObjectOfType<S>();
				if (!s)
				{
					s = UnitySingleton.GetOrCreateInstanceOnGameObject<S>(typeFromHandle);
				}
				s.RegisterAsSingleInstanceAndInit();
			}
			else
			{
				s = (UnitySingleton.instances[typeFromHandle] as S);
			}
			if (!s)
			{
				throw new Exception("No instance was created: " + typeFromHandle.Name);
			}
			s.IsInstanceReady = true;
			return s;
		}

		public static void DoWithCodeGeneratedInstance<C>(MonoBehaviour sender, Action<C> whatToDoWithInstanceWhenItsReady) where C : CodeGeneratedSingleton
		{
			UnitySingleton.GetSynchronousCodeGeneratedInstance<C>();
			UnitySingleton.DoWithExistingInstance<C>(sender, whatToDoWithInstanceWhenItsReady);
		}

		public static void DoWithSceneInstance<S>(MonoBehaviour sender, Action<S> whatToDoWithInstanceWhenItsReady) where S : SceneSingleton
		{
			UnitySingleton.DoWithExistingInstance<S>(sender, whatToDoWithInstanceWhenItsReady);
		}

		private static void DoWithExistingInstance<S>(MonoBehaviour sender, Action<S> whatToDoWithInstanceWhenItsReady) where S : UnitySingleton
		{
			Type typeFromHandle = typeof(S);
			bool flag = true;
			if (UnitySingleton.instances.ContainsKey(typeFromHandle))
			{
				S s = UnitySingleton.instances[typeFromHandle] as S;
				if (s && s.IsInstanceReady)
				{
					flag = false;
					whatToDoWithInstanceWhenItsReady(s);
				}
			}
			if (flag)
			{
				if (!UnitySingleton.instanceListeners.ContainsKey(typeFromHandle))
				{
					UnitySingleton.instanceListeners.Add(typeFromHandle, new Dictionary<MonoBehaviour, Action<UnitySingleton>>());
				}
				if (!UnitySingleton.instanceListeners[typeFromHandle].ContainsKey(sender))
				{
					UnitySingleton.instanceListeners[typeFromHandle].Add(sender, null);
				}
				Dictionary<MonoBehaviour, Action<UnitySingleton>> dictionary= UnitySingleton.instanceListeners[typeFromHandle];
				(dictionary = UnitySingleton.instanceListeners[typeFromHandle])[sender] = (Action<UnitySingleton>)Delegate.Combine(dictionary[sender], new Action<UnitySingleton>(delegate(UnitySingleton singleton)
				{
					whatToDoWithInstanceWhenItsReady(singleton as S);
				}));
			}
		}

		protected sealed override void Start()
		{
			base.Start();
			Type type = base.GetType();
			bool flag = false;
			if (UnitySingleton.instances.ContainsKey(type))
			{
				if (UnitySingleton.instances[type] != this)
				{
					if (this is CodeGeneratedSingleton)
					{
						throw new Exception("There's already an instance for " + type.Name);
					}
					if (this is SceneSingleton && this.DontDestroySingleton)
					{
						flag = true;
					}
				}
			}
			else if (this is CodeGeneratedSingleton)
			{
				throw new NotSupportedException(string.Format("{0} is a {1} and needs to be created via code, and not placed on a scene!", type.Name, typeof(CodeGeneratedSingleton).Name));
			}
			if (flag)
			{
				UnityEngine.Debug.LogWarning(string.Format("There's already a {0} instance on the current scene, there's no point in staying.. goodbye.. I'm gonna go now :(", type.Name));
				UnityEngine.Object.Destroy(this);
			}
			else if (this is SceneSingleton)
			{
				this.RegisterAsSingleInstanceAndInit();
				this.SetReadyAndNotifyAfterRegistering();
			}
		}

		protected virtual void SetReadyAndNotifyAfterRegistering()
		{
			this.DeclareAsReady();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			Type type = base.GetType();
			if (UnitySingleton.instances.ContainsKey(type) && UnitySingleton.instances[type] == this)
			{
				UnitySingleton.instances.Remove(type);
			}
		}

		protected virtual void InitAfterRegisteringAsSingleInstance()
		{
		}

		protected virtual bool DontDestroySingleton
		{
			get
			{
				return false;
			}
		}

		private static readonly Dictionary<Type, UnitySingleton> instances = new Dictionary<Type, UnitySingleton>();

		private static readonly Dictionary<Type, Dictionary<MonoBehaviour, Action<UnitySingleton>>> instanceListeners = new Dictionary<Type, Dictionary<MonoBehaviour, Action<UnitySingleton>>>();
	}
}
