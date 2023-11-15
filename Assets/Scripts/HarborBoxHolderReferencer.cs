using System;
using UnityEngine;

public class HarborBoxHolderReferencer : MonoBehaviour
{
	private void Awake()
	{
		HarborBoxHolderReferencer.Instance = this;
	}

	public static HarborBoxHolderReferencer Instance;
}
