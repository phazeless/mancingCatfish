using System;
using DG.Tweening;

public class UIFishBookDialog : UpgradeDialogTween
{
	private void TweenKiller()
	{
		DOTween.Kill(base.transform, false);
	}
}
