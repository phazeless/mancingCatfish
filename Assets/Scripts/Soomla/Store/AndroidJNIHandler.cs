using System;
using UnityEngine;

namespace Soomla.Store
{
	public static class AndroidJNIHandler
	{
		public static void CallVoid(AndroidJavaObject jniObject, string method, string arg0)
		{
			if (!Application.isEditor)
			{
				jniObject.Call(method, new object[]
				{
					arg0
				});
				AndroidJNIHandler.checkExceptions();
			}
		}

		public static void CallVoid(AndroidJavaObject jniObject, string method, AndroidJavaObject arg0, string arg1)
		{
			if (!Application.isEditor)
			{
				jniObject.Call(method, new object[]
				{
					arg0,
					arg1
				});
				AndroidJNIHandler.checkExceptions();
			}
		}

		public static void CallStaticVoid(AndroidJavaClass jniObject, string method, string arg0)
		{
			if (!Application.isEditor)
			{
				jniObject.CallStatic(method, new object[]
				{
					arg0
				});
				AndroidJNIHandler.checkExceptions();
			}
		}

		public static void CallStaticVoid(AndroidJavaClass jniObject, string method, string arg0, string arg1)
		{
			if (!Application.isEditor)
			{
				jniObject.CallStatic(method, new object[]
				{
					arg0,
					arg1
				});
				AndroidJNIHandler.checkExceptions();
			}
		}

		public static void CallStaticVoid(AndroidJavaClass jniObject, string method, string arg0, int arg1)
		{
			if (!Application.isEditor)
			{
				jniObject.CallStatic(method, new object[]
				{
					arg0,
					arg1
				});
				AndroidJNIHandler.checkExceptions();
			}
		}

		public static T CallStatic<T>(AndroidJavaClass jniObject, string method, string arg0)
		{
			if (Application.isEditor)
			{
				return default(T);
			}
			T t = jniObject.CallStatic<T>(method, new object[]
			{
				arg0
			});
			AndroidJNIHandler.checkExceptions();
			if (t is AndroidJavaObject && (t as AndroidJavaObject).GetRawObject() == IntPtr.Zero)
			{
				throw new VirtualItemNotFoundException();
			}
			return t;
		}

		public static T CallStatic<T>(AndroidJavaClass jniObject, string method, string arg0, int arg1)
		{
			if (Application.isEditor)
			{
				return default(T);
			}
			T t = jniObject.CallStatic<T>(method, new object[]
			{
				arg0,
				arg1
			});
			AndroidJNIHandler.checkExceptions();
			if (t is AndroidJavaObject && (t as AndroidJavaObject).GetRawObject() == IntPtr.Zero)
			{
				throw new VirtualItemNotFoundException();
			}
			return t;
		}

		public static T CallStatic<T>(AndroidJavaClass jniObject, string method, int arg0)
		{
			if (Application.isEditor)
			{
				return default(T);
			}
			T t = jniObject.CallStatic<T>(method, new object[]
			{
				arg0
			});
			AndroidJNIHandler.checkExceptions();
			if (t is AndroidJavaObject && (t as AndroidJavaObject).GetRawObject() == IntPtr.Zero)
			{
				throw new VirtualItemNotFoundException();
			}
			return t;
		}

		public static void checkExceptions()
		{
			IntPtr intPtr = AndroidJNI.ExceptionOccurred();
			if (intPtr != IntPtr.Zero)
			{
				AndroidJNI.ExceptionClear();
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.exceptions.InsufficientFundsException");
				if (AndroidJNI.IsInstanceOf(intPtr, androidJavaClass.GetRawClass()))
				{
					UnityEngine.Debug.Log("SOOMLA/UNITY Caught InsufficientFundsException!");
					throw new InsufficientFundsException();
				}
				androidJavaClass.Dispose();
				androidJavaClass = new AndroidJavaClass("com.soomla.store.exceptions.VirtualItemNotFoundException");
				if (AndroidJNI.IsInstanceOf(intPtr, androidJavaClass.GetRawClass()))
				{
					UnityEngine.Debug.Log("SOOMLA/UNITY Caught VirtualItemNotFoundException!");
					throw new VirtualItemNotFoundException();
				}
				androidJavaClass.Dispose();
				androidJavaClass = new AndroidJavaClass("com.soomla.store.exceptions.NotEnoughGoodsException");
				if (AndroidJNI.IsInstanceOf(intPtr, androidJavaClass.GetRawClass()))
				{
					UnityEngine.Debug.Log("SOOMLA/UNITY Caught NotEnoughGoodsException!");
					throw new NotEnoughGoodsException();
				}
				androidJavaClass.Dispose();
				UnityEngine.Debug.Log("SOOMLA/UNITY Got an exception but can't identify it!");
			}
		}
	}
}
