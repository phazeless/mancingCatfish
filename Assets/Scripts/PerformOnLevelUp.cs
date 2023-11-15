using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

public class PerformOnLevelUp : BaseBehavior
{
	protected override void Awake()
	{
		base.Awake();
		SkillManager.Instance.OnSkillLevelChanged += this.OnSkillLevelUp;
	}

	private void OnSkillLevelUp(Skill skillThatLeveledUp, LevelChange change)
	{
		if (this.listenForLevelUp.name == skillThatLeveledUp.name)
		{
			foreach (PerformOnLevelUp.LevelAndGameObject levelAndGameObject in this.objectsToHandleOnLevelUp)
			{
				if (levelAndGameObject.ActivateOnLevel < 0 || levelAndGameObject.ActivateOnLevel == skillThatLeveledUp.CurrentLevel)
				{
					foreach (ILevelUpListener levelUpListener in levelAndGameObject.LevelUpListeners)
					{
						levelUpListener.OnLevelUp(base.gameObject, skillThatLeveledUp);
					}
				}
			}
		}
	}

	private void OnDestroy()
	{
		SkillManager.Instance.OnSkillLevelChanged -= this.OnSkillLevelUp;
	}

	[SerializeField]
	public Skill listenForLevelUp;

	[SerializeField]
	private List<PerformOnLevelUp.LevelAndGameObject> objectsToHandleOnLevelUp = new List<PerformOnLevelUp.LevelAndGameObject>();

	[Serializable]
	public class LevelAndGameObject
	{
		public int ActivateOnLevel;

		public List<ILevelUpListener> LevelUpListeners = new List<ILevelUpListener>();
	}
}
