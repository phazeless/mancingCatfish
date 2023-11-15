using System;
using System.Collections.Generic;
using UnityEngine;

namespace Soomla
{
	public class KeyValueStorageAndroid : KeyValueStorage
	{
		protected override string _getValue(string key)
		{
			string result = null;
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.data.KeyValueStorage"))
			{
				result = androidJavaClass.CallStatic<string>("getValue", new object[]
				{
					key
				});
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return result;
		}

		protected override void _setValue(string key, string val)
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.data.KeyValueStorage"))
			{
				androidJavaClass.CallStatic("setValue", new object[]
				{
					key,
					val
				});
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _deleteKeyValue(string key)
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.data.KeyValueStorage"))
			{
				androidJavaClass.CallStatic("deleteKeyValue", new object[]
				{
					key
				});
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override List<string> _getEncryptedKeys()
		{
			string text = null;
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.core.unity.SoomlaBridge"))
			{
				text = androidJavaClass.CallStatic<string>("keyValStorage_GetEncryptedKeys", new object[0]);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return new List<string>(text.Split(new char[]
			{
				','
			}));
		}

		protected override void _purge()
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.data.KeyValueStorage"))
			{
				androidJavaClass.CallStatic("purge", new object[0]);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
	}
}
