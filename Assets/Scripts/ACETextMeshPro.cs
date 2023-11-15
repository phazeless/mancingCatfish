using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

[AddComponentMenu("UI/TextMeshPro - Text (UI)", 11)]
[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasRenderer))]
[SelectionBase]
[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class ACETextMeshPro : TMP_Text, ISerializationCallbackReceiver
{
	protected override void Start()
	{
		base.Start();
	}

	public void SetVariableText(params string[] text)
	{
		if (this.unchangedText == null)
		{
			this.textToBeSet = text;
			return;
		}
		List<string> list = new List<string>();
		int num = this.unchangedText.Count((char f) => f == '{');
		for (int i = 0; i < num; i++)
		{
			bool flag = i < text.Length;
			list.Add((!flag) ? string.Empty : text[i]);
		}
		if (num > 0)
		{
			string text2 = new StringBuilder().AppendLine(string.Format(this.unchangedText, list.ToArray())).ToString();
			string text3 = text2.TrimEnd(new char[]
			{
				'\r',
				'\n'
			});
			base.SetText(text3);
		}
		else
		{
			UnityEngine.Debug.LogWarning("The TextMeshProUGUI-object '" + base.gameObject.name + "' is missing parameters ({0}, {1} etc) in the text-field. Either set parameters in the text or use SetText()-method instead.", base.gameObject);
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		this.unchangedText = base.text;
	}

	private string unchangedText;

	private string[] textToBeSet;
}
