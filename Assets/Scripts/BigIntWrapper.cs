using System;
using System.Numerics;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class BigIntWrapper : ISerializationCallbackReceiver
{
	public BigInteger Value { get; private set; }

	public void SetValue(string valueAsString)
	{
		this.stringValue = valueAsString;
		this.Value = BigInteger.Parse(this.stringValue);
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		if (this.hasSerializaed)
		{
			return;
		}
		this.Value = BigInteger.Parse(this.stringValue);
		this.hasSerializaed = true;
	}

	public static void GetPartialNumberAndFullDigitsCount(BigInteger bigInteger, int digitsToShow, out int partialIntValue, out int fullDigitCount)
	{
		fullDigitCount = (int)BigInteger.Log10(bigInteger + 1);
		if (digitsToShow > fullDigitCount)
		{
			partialIntValue = (int)bigInteger;
			return;
		}
		BigInteger value = bigInteger / BigInteger.Pow(10, fullDigitCount - digitsToShow);
		partialIntValue = (int)value;
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
	}

	private bool hasSerializaed;

	[SerializeField]
	public string stringValue = "0";
}
