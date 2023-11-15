using System;
using TMPro;
using UnityEngine;

public class TextObjectPool : MonoBehaviour
{
	public static TextObjectPool Instance { get; private set; }

	private void Awake()
	{
		TextObjectPool.Instance = this;
		this.textMeshProPool = new BasicObjectPool<TextMeshPro>(new Func<TextMeshPro>(this.Instantiator));
		this.textMeshProUGUIPool = new BasicObjectPool<TextMeshProUGUI>(new Func<TextMeshProUGUI>(this.InstantiatorUGUI));
	}

	private TextMeshPro Instantiator()
	{
		return UnityEngine.Object.Instantiate<TextMeshPro>(this.textMesh, base.transform, false);
	}

	private TextMeshProUGUI InstantiatorUGUI()
	{
		return UnityEngine.Object.Instantiate<TextMeshProUGUI>(this.textMeshUI, base.transform, false);
	}

	public BasicObjectPool<TextMeshPro> TextMeshProPool
	{
		get
		{
			return this.textMeshProPool;
		}
	}

	public BasicObjectPool<TextMeshProUGUI> TextMeshProPoolUGUI
	{
		get
		{
			return this.textMeshProUGUIPool;
		}
	}

	private BasicObjectPool<TextMeshPro> textMeshProPool;

	private BasicObjectPool<TextMeshProUGUI> textMeshProUGUIPool;

	[SerializeField]
	private TextMeshPro textMesh;

	[SerializeField]
	private TextMeshProUGUI textMeshUI;
}
