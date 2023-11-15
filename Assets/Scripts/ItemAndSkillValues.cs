using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ItemAndSkillValues
{
	static ItemAndSkillValues()
	{
		ItemAndSkillValues.InitCachedValuesKeys();
	}

	public static void ClearValues(SkillBehaviour skillBehaviour)
	{
		StoredValue storedValue = ItemAndSkillValues.cachedSkillValues[skillBehaviour.ChangeValue.GetId()];
		storedValue.ClearCalculations();
		storedValue.CalculateTotal();
	}

	public static StoredValue GetStoredValue(SkillBehaviour skillBehaviour)
	{
		StoredValue result = new StoredValue();
		if (skillBehaviour == null)
		{
			UnityEngine.Debug.LogError("skillBehaviour is null inside GetStoredValue");
		}
		else if (skillBehaviour.ChangeValue == null)
		{
			UnityEngine.Debug.LogError("skillBehaviour.ChangeValue is null on object with name: " + skillBehaviour.name);
		}
		else
		{
			result = ItemAndSkillValues.GetStoredValue(skillBehaviour.ChangeValue.GetId());
		}
		return result;
	}

	public static StoredValue GetStoredValue(Guid guid)
	{
		return ItemAndSkillValues.cachedSkillValues[guid];
	}

	public static float GetCurrentTotalValueFor(Guid id)
	{
		return ItemAndSkillValues.GetStoredValue(id).Total;
	}

	public static float GetCurrentTotalValueFor<T>() where T : ISkillAttribute
	{
		return ItemAndSkillValues.GetCurrentTotalValueFor(Skills.GetSkillId(typeof(T)));
	}

	public static float GetCurrentTotalValueFor(ISkillAttribute changeValue)
	{
		return ItemAndSkillValues.GetStoredValue(changeValue.GetId()).Total;
	}

	private static void InitCachedValuesKeys()
	{
		ItemAndSkillValues.cachedSkillValues.Clear();
		Assembly assembly = typeof(ISkillAttribute).Assembly;
		List<Type> list = (from t in assembly.GetTypes()
		where t.IsSubclassOf(typeof(Skills.BaseSkillAttribute)) && !t.IsAbstract
		select t).ToList<Type>();
		foreach (Type type in list)
		{
			Guid skillId = Skills.GetSkillId(type);
			try
			{
				ItemAndSkillValues.cachedSkillValues.Add(skillId, new StoredValue());
			}
			catch (ArgumentException ex)
			{
				UnityEngine.Debug.LogError(string.Concat(new object[]
				{
					"The skill attribute '",
					type.Name,
					"' has a duplicate id of  ",
					skillId,
					"\n ",
					ex.Message
				}));
			}
		}
	}

	private static Dictionary<Guid, StoredValue> cachedSkillValues = new Dictionary<Guid, StoredValue>();
}
