using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NativeShare
{
	public NativeShare()
	{
		this.subject = string.Empty;
		this.text = string.Empty;
		this.title = string.Empty;
		this.files = new List<string>(0);
		this.mimes = new List<string>(0);
	}

	private static AndroidJavaClass AJC
	{
		get
		{
			if (NativeShare.m_ajc == null)
			{
				NativeShare.m_ajc = new AndroidJavaClass("com.yasirkula.unity.NativeShare");
			}
			return NativeShare.m_ajc;
		}
	}

	private static AndroidJavaObject Context
	{
		get
		{
			if (NativeShare.m_context == null)
			{
				using (AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					NativeShare.m_context = androidJavaObject.GetStatic<AndroidJavaObject>("currentActivity");
				}
			}
			return NativeShare.m_context;
		}
	}

	public NativeShare SetSubject(string subject)
	{
		if (subject != null)
		{
			this.subject = subject;
		}
		return this;
	}

	public NativeShare SetText(string text)
	{
		if (text != null)
		{
			this.text = text;
		}
		return this;
	}

	public NativeShare SetTitle(string title)
	{
		if (title != null)
		{
			this.title = title;
		}
		return this;
	}

	public NativeShare AddFile(string filePath, string mime = null)
	{
		if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
		{
			this.files.Add(filePath);
			this.mimes.Add(mime ?? string.Empty);
		}
		else
		{
			UnityEngine.Debug.LogError("File does not exist at path or permission denied: " + filePath);
		}
		return this;
	}

	public void Share()
	{
		if (this.files.Count == 0 && this.subject.Length == 0 && this.text.Length == 0)
		{
			UnityEngine.Debug.LogWarning("Share Error: attempting to share nothing!");
			return;
		}
		NativeShare.AJC.CallStatic("Share", new object[]
		{
			NativeShare.Context,
			this.files.ToArray(),
			this.mimes.ToArray(),
			this.subject,
			this.text,
			this.title
		});
	}

	private static AndroidJavaClass m_ajc;

	private static AndroidJavaObject m_context;

	private string subject;

	private string text;

	private string title;

	private List<string> files;

	private List<string> mimes;
}
