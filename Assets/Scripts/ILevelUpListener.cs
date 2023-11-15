using System;
using UnityEngine;

public interface ILevelUpListener
{
	void OnLevelUp(GameObject caller, Skill skill);
}
