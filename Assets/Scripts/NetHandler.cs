using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class NetHandler : MonoBehaviour
{
	private void Awake()
	{
		SkillManager.Instance.OnSkillAttributeValueChanged += this.Instance_OnSkillAttributeValueChanged;
		AFKManager.Instance.OnUserLeaveCallback += this.Instance_OnUserLeaveCallback;
		AFKManager.Instance.OnUserReturnCallback += this.Instance_OnUserReturnCallback;
	}

	private void Instance_OnUserReturnCallback(bool fromApplicationRestart, DateTime now, float afkTime)
	{
	}

	private void Instance_OnUserLeaveCallback(DateTime arg1, bool arg2)
	{
		this.catcherPositions.Clear();
		foreach (MoveableNetCatcher moveableNetCatcher in this.catchers)
		{
			this.catcherPositions.Add(moveableNetCatcher.transform.position);
		}
		if (this.catcherPositions != null && this.catcherPositions.Count > 0)
		{
			string text = JsonConvert.SerializeObject(this.catcherPositions);
			if (text != null)
			{
				EncryptedPlayerPrefs.SetString("KEY_CATCHER_POSITIONS", text, true);
			}
		}
	}

	private void Instance_OnSkillAttributeValueChanged(ISkillAttribute skillAttribute, float value)
	{
		if (skillAttribute is Skills.MoveableNets)
		{
			int num = (int)value - this.currentMoveableNetsCount;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.CreateNets(1);
				}
			}
			else
			{
				this.RemoveNet(Mathf.Abs(num));
			}
			this.currentMoveableNetsCount = (int)value;
		}
	}

	private void CreateNets(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			MoveableNetCatcher moveableNetCatcher = UnityEngine.Object.Instantiate<MoveableNetCatcher>(this.prefabMoveableNet, base.transform, true);
			moveableNetCatcher.transform.position = new Vector2(UnityEngine.Random.Range(0f, 0.5f), UnityEngine.Random.Range(0f, 0.5f));
			this.catchers.Add(moveableNetCatcher);
		}
	}

	private void RemoveNet(int amount)
	{
		List<MoveableNetCatcher> list = new List<MoveableNetCatcher>();
		int num = this.catchers.Count - 1;
		while (num >= this.catchers.Count - amount && num >= 0)
		{
			MoveableNetCatcher moveableNetCatcher = this.catchers[num];
			this.catchers.RemoveAt(num);
			UnityEngine.Object.Destroy(moveableNetCatcher.gameObject);
			num--;
		}
	}

	private const string KEY_CATCHER_POSITIONS = "KEY_CATCHER_POSITIONS";

	[SerializeField]
	private MoveableNetCatcher prefabMoveableNet;

	private List<MoveableNetCatcher> catchers = new List<MoveableNetCatcher>();

	private List<Vector2> catcherPositions = new List<Vector2>();

	private int currentMoveableNetsCount;
}
