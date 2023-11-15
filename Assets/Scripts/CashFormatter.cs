using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public static class CashFormatter
{
	private static void ToGameCashRepresentation(BigInteger val, int decimals, bool useFullPostfix, out double cappedValue, out string postFix)
	{
		CashFormatter.ExponentAndPostfix exponentAndPostfix = CashFormatter.Below1Million;
		cappedValue = 0.0;
		int num = (int)Mathf.Pow(10f, (float)decimals);
		if (val > (long)CashFormatter.USE_POSTFIX_THRESHOLD)
		{
			int num2 = 0;
			int num3 = 0;
			BigIntWrapper.GetPartialNumberAndFullDigitsCount(val, 6, out num2, out num3);
			int num4 = num3 / 3;
			int num5 = num4 - 2;
			if (num5 < CashFormatter.expsAndPostfixes.Count)
			{
				exponentAndPostfix = CashFormatter.expsAndPostfixes[num5];
			}
			cappedValue = Math.Truncate((double)num * ((double)num2 / CashFormatter.pow[num3 % 3])) / (double)num;
		}
		else
		{
			cappedValue = (double)((int)val);
		}
		postFix = ((!useFullPostfix) ? exponentAndPostfix.PartialPostfix : exponentAndPostfix.FullPostfix);
	}

	public static string SimpleToCashRepresentation(BigInteger val, int decimals, bool useFullPostfix, bool includeDollarSign = false)
	{
		return CashFormatter.SimpleToCashRepresentation(val, decimals, useFullPostfix, includeDollarSign, null);
	}

	public static string SimpleToCashRepresentation(BigInteger val, int decimals, bool useFullPostfix, bool includeDollarSign, string overrideStringFormatting)
	{
		if (!(val > (long)CashFormatter.USE_POSTFIX_THRESHOLD))
		{
			return ((!includeDollarSign) ? string.Empty : "$") + val;
		}
		CashFormatter.ToGameCashRepresentation(val, decimals, useFullPostfix, out CashFormatter.cappedVal, out CashFormatter.postFix);
		if (overrideStringFormatting == null)
		{
			return ((!includeDollarSign) ? string.Empty : "$") + CashFormatter.cappedVal.ToString("N3") + " " + CashFormatter.postFix;
		}
		return ((!includeDollarSign) ? string.Empty : "$") + CashFormatter.cappedVal.ToString(overrideStringFormatting) + " " + CashFormatter.postFix;
	}

	public static void GetMainCashFormat(BigInteger val, int decimals, out string valueAsString, out string postFixAsString)
	{
		if (val > (long)CashFormatter.USE_POSTFIX_THRESHOLD)
		{
			CashFormatter.ToGameCashRepresentation(val, decimals, true, out CashFormatter.cappedVal, out CashFormatter.postFix);
			valueAsString = CashFormatter.cappedVal.ToString("N3");
		}
		else
		{
			CashFormatter.postFix = null;
			valueAsString = val.ToString();
		}
		postFixAsString = CashFormatter.postFix;
	}

	public static readonly int USE_POSTFIX_THRESHOLD = 999999;

	private static readonly double[] pow = new double[]
	{
		1000000.0,
		100000.0,
		10000.0
	};

	private static double cappedVal = 0.0;

	private static string postFix = null;

	private static readonly CashFormatter.ExponentAndPostfix Below1Million = new CashFormatter.ExponentAndPostfix(0, string.Empty, string.Empty);

	private static readonly List<CashFormatter.ExponentAndPostfix> expsAndPostfixes = new List<CashFormatter.ExponentAndPostfix>
	{
		new CashFormatter.ExponentAndPostfix(6, "MILLION", "Mil"),
		new CashFormatter.ExponentAndPostfix(9, "BILLION", "Bil"),
		new CashFormatter.ExponentAndPostfix(12, "TRILLION", "Tri"),
		new CashFormatter.ExponentAndPostfix(15, "QUADRILLION", "Qua"),
		new CashFormatter.ExponentAndPostfix(18, "QUINTILLION", "Qui"),
		new CashFormatter.ExponentAndPostfix(21, "SEXTILLION", "Sex"),
		new CashFormatter.ExponentAndPostfix(24, "SEPTILLION", "Sep"),
		new CashFormatter.ExponentAndPostfix(27, "OCTILLION", "Oct"),
		new CashFormatter.ExponentAndPostfix(30, "NONILLION", "Non"),
		new CashFormatter.ExponentAndPostfix(33, "DECILLION", "Dec"),
		new CashFormatter.ExponentAndPostfix(36, "UNDECILLION", "Und"),
		new CashFormatter.ExponentAndPostfix(39, "DUODECILLION", "Duo"),
		new CashFormatter.ExponentAndPostfix(42, "TREDECILLION", "Tre"),
		new CashFormatter.ExponentAndPostfix(45, "QUATTUORDECILLION", "Qtd"),
		new CashFormatter.ExponentAndPostfix(48, "QUINQUADECILLION", "Qqd"),
		new CashFormatter.ExponentAndPostfix(51, "SEXDECILLION", "Sed"),
		new CashFormatter.ExponentAndPostfix(54, "SEPTENDECILLION", "Std"),
		new CashFormatter.ExponentAndPostfix(57, "OCTODECILLION", "Ocd"),
		new CashFormatter.ExponentAndPostfix(60, "NOVENDECILLION", "Nov"),
		new CashFormatter.ExponentAndPostfix(63, "VIGINTILLION", "Vig"),
		new CashFormatter.ExponentAndPostfix(66, "UNVIGINTILLION", "Unv"),
		new CashFormatter.ExponentAndPostfix(69, "DUOVIGINTILLION", "Duv"),
		new CashFormatter.ExponentAndPostfix(72, "TRESVIGINTILLION", "Trv"),
		new CashFormatter.ExponentAndPostfix(75, "QUATTUORVIGINTILLION", "Qtv"),
		new CashFormatter.ExponentAndPostfix(78, "QUINQUAVIGINTILLION", "Qqv")
	};

	public struct ExponentAndPostfix
	{
		public ExponentAndPostfix(int exponent, string fullPostfix, string partialPostfix)
		{
			this.Exponent = exponent;
			this.FullPostfix = fullPostfix;
			this.PartialPostfix = partialPostfix;
		}

		public int Exponent;

		public string FullPostfix;

		public string PartialPostfix;
	}
}
