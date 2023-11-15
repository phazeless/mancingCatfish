using System;
using System.Diagnostics;
using ACE.Ads;
using UnityEngine;
using UnityEngine.UI;

public class UnityEditorAdPopup : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BaseAdFormat, bool, bool> OnAdClosed;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BaseAdFormat> OnAdShow;

	public void Show(BaseAdFormat adFormat)
	{
		this.adFormat = adFormat;
		this.adFormatLabel.text = adFormat.GetType().Name;
		if (this.OnAdShow != null)
		{
			this.OnAdShow(adFormat);
		}
	}

	public void CloseClicked()
	{
		if (this.OnAdClosed != null)
		{
			this.OnAdClosed(this.adFormat, false, false);
		}
	}

	public void WatchAdWithDidClick()
	{
		if (this.OnAdClosed != null)
		{
			this.OnAdClosed(this.adFormat, true, false);
		}
	}

	public void WatchAdWithDidComplete()
	{
		if (this.OnAdClosed != null)
		{
			this.OnAdClosed(this.adFormat, false, true);
		}
	}

	public void WatchAdWithDidClickAndComplete()
	{
		if (this.OnAdClosed != null)
		{
			this.OnAdClosed(this.adFormat, true, true);
		}
	}

	[SerializeField]
	private Text adFormatLabel;

	private BaseAdFormat adFormat;
}
