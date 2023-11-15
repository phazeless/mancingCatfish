using System;
using UnityEngine;
using UnityEngine.UI;

public class TournamentButtonBehaviour : MonoBehaviour
{
	public void SetButtonInteractability(bool isInteractable)
	{
		this.button.interactable = isInteractable;
		if (isInteractable)
		{
			this.buttonBody.color = this.interactableColor;
		}
		else
		{
			this.buttonBody.color = this.noninteractableColor;
		}
	}

	public void ButtonClick()
	{
	}

	[SerializeField]
	private Image buttonBody;

	[SerializeField]
	private Button button;

	private Color interactableColor = new Color(0.145f, 0.706f, 0.694f);

	private Color noninteractableColor = new Color(0.788f, 0.788f, 0.788f);
}
