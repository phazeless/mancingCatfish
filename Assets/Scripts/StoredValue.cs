using System;
using System.Collections;
using System.Collections.Generic;

public class StoredValue
{
	public StoredValue()
	{
		this.enumKeys = Enum.GetValues(typeof(AttributeValueCalculationType));
		IEnumerator enumerator = this.enumKeys.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				AttributeValueCalculationType key = (AttributeValueCalculationType)obj;
				this.Calculations.Add(key, 0f);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void AddCalculation(AttributeValueCalculationType type, float calculation)
	{
		Dictionary<AttributeValueCalculationType, float> calculations = this.Calculations;
		if (type == AttributeValueCalculationType.AddPercentOfCurrent)
		{
			(calculations = this.Calculations)[type] = calculations[type] + this.Calculations[type] * (calculation / 100f);
		}
		(calculations = this.Calculations)[type] = calculations[type] + calculation;
	}

	public void CalculateTotal()
	{
		this.Total = this.Base + this.GetCalculation();
	}

	public void ClearCalculations()
	{
		IEnumerator enumerator = this.enumKeys.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				AttributeValueCalculationType key = (AttributeValueCalculationType)obj;
				this.Calculations[key] = 0f;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	private float GetCalculation()
	{
		float num = 0f;
		IEnumerator enumerator = this.enumKeys.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				AttributeValueCalculationType type = (AttributeValueCalculationType)obj;
				num += this.GetTotalFor(type);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		return num;
	}

	private float GetCalculation(AttributeValueCalculationType type)
	{
		return this.Calculations[type];
	}

	private float GetTotalFor(AttributeValueCalculationType type)
	{
		if (type == AttributeValueCalculationType.AddToBase)
		{
			return this.GetCalculation(type);
		}
		if (type == AttributeValueCalculationType.AddPercentOfBase)
		{
			return this.Base * (this.GetCalculation(type) / 100f);
		}
		if (type == AttributeValueCalculationType.AddPercentOfCurrent)
		{
			return this.Base * (this.GetCalculation(type) / 100f);
		}
		throw new InvalidOperationException("Invalid AttributeValueCalculationType is being used.");
	}

	public float Base;

	public float Total;

	private Array enumKeys = new AttributeValueCalculationType[0];

	private Dictionary<AttributeValueCalculationType, float> Calculations = new Dictionary<AttributeValueCalculationType, float>();
}
