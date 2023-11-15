using System;
using FullInspector;
using UnityEngine;

[Serializable]
public class SkillBehaviour : BaseScriptableObject
{
	public string Description
	{
		get
		{
			return this.description;
		}
	}

	public string PostFixCharacter
	{
		get
		{
			return this.postFixCharacter;
		}
	}

	public ISkillAttribute ChangeValue
	{
		get
		{
			return this.changeValue;
		}
	}

	public AttributeValueType ValueType
	{
		get
		{
			return this.changeValueType;
		}
	}

	public AttributeValueCalculationType CalculationType
	{
		get
		{
			return this.calculationType;
		}
	}

	public AttributeValueChangeMethod ChangeMethod
	{
		get
		{
			return this.changeMethod;
		}
	}

	public float InitialValue
	{
		get
		{
			return this.initialValue;
		}
	}

	public bool UseCustomInitialValue
	{
		get
		{
			return this.initialValueUsage == AttributeValueInitialValueUsage.UseCustomInitialValue;
		}
	}

	public float GetTotalValueAtLevel(int level)
	{
		float num = 0f;
		for (int i = 0; i <= level; i++)
		{
			if (this.calculationType == AttributeValueCalculationType.AddPercentOfCurrent)
			{
				num += this.GetValueAtLevel(i) + num * (this.GetValueAtLevel(i) / 100f);
			}
			else
			{
				num += this.GetValueAtLevel(i);
			}
		}
		return num;
	}

	public float GetValueAtLevel(int level)
	{
		if (level <= 0)
		{
			return 0f;
		}
		if (this.UseConstantChangeMethod)
		{
			return this.constantChangePerLevel;
		}
		if (this.UseCustomChangeMethod)
		{
			if (level < this.customChangePerLevel.Length)
			{
				return this.customChangePerLevel[level];
			}
			return this.customChangePerLevel[this.customChangePerLevel.Length - 1];
		}
		else
		{
			if (this.UseCurveChangeMethod)
			{
				return this.curveChangePerLevel.Evaluate((float)level);
			}
			if (this.UseNeverChangeMethod)
			{
				return this.constantValue;
			}
			return -1f;
		}
	}

	private bool UseConstantChangeMethod
	{
		get
		{
			return this.changeMethod == AttributeValueChangeMethod.ChangeByConstant;
		}
	}

	private bool UseCustomChangeMethod
	{
		get
		{
			return this.changeMethod == AttributeValueChangeMethod.ChangeByCustom;
		}
	}

	private bool UseCurveChangeMethod
	{
		get
		{
			return this.changeMethod == AttributeValueChangeMethod.ChangeByCurve;
		}
	}

	private bool UseNeverChangeMethod
	{
		get
		{
			return this.changeMethod == AttributeValueChangeMethod.NeverChange;
		}
	}

	private bool IsCalculation
	{
		get
		{
			return this.changeValueType == AttributeValueType.Calculation;
		}
	}

	private bool ShowInitialValueUsage
	{
		get
		{
			return this.changeValueType != AttributeValueType.Base;
		}
	}

	private bool ShowInitialValue
	{
		get
		{
			return this.changeValueType == AttributeValueType.Base || this.initialValueUsage == AttributeValueInitialValueUsage.UseCustomInitialValue;
		}
	}

	[SerializeField]
	private string description;

	[SerializeField]
	private string postFixCharacter = string.Empty;

	[SerializeField]
	[InspectorHeader("---")]
	private ISkillAttribute changeValue;

	[SerializeField]
	private AttributeValueType changeValueType;

	[SerializeField]
	[InspectorShowIf("IsCalculation")]
	private AttributeValueCalculationType calculationType = AttributeValueCalculationType.AddPercentOfBase;

	[InspectorShowIf("ShowInitialValueUsage")]
	[SerializeField]
	private AttributeValueInitialValueUsage initialValueUsage;

	[InspectorShowIf("ShowInitialValue")]
	[SerializeField]
	private float initialValue;

	[FullInspector.InspectorName("Change Per Level Method")]
	[SerializeField]
	[InspectorHeader("---")]
	private AttributeValueChangeMethod changeMethod;

	[InspectorShowIf("UseConstantChangeMethod")]
	[FullInspector.InspectorName("Change Per Level")]
	[SerializeField]
	private float constantChangePerLevel = -1f;

	[SerializeField]
	[InspectorShowIf("UseCustomChangeMethod")]
	[FullInspector.InspectorName("Change Per Level")]
	private float[] customChangePerLevel;

	[SerializeField]
	[InspectorShowIf("UseCurveChangeMethod")]
	[FullInspector.InspectorName("Change Per Level")]
	private AnimationCurve curveChangePerLevel;

	[InspectorShowIf("UseNeverChangeMethod")]
	[SerializeField]
	private float constantValue = -1f;
}
