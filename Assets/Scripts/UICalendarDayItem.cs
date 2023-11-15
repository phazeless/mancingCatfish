using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICalendarDayItem : MonoBehaviour
{
	private void Awake()
	{
		this.bgImage = base.GetComponent<Image>();
		this.dayLabel = base.GetComponentInChildren<TextMeshProUGUI>();
	}

	public void SetAsToday()
	{
		this.bgImage.color = this.today;
	}

	public void SetAsReached()
	{
		this.bgImage.color = this.reached;
	}

	public void SetAsNotReached()
	{
		this.bgImage.color = this.notReached;
	}

	public void SetDay(int day)
	{
		this.dayLabel.text = day.ToString();
	}

	private Color reached = new Color(0.8f, 0.278f, 0.165f);

	private Color notReached = new Color(0.8f, 0.278f, 0.165f, 0.25f);

	private Color today = new Color(0.22f, 0.733f, 0.408f);

	private Image bgImage;

	private TextMeshProUGUI dayLabel;
}
