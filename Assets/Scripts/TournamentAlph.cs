using System;
using System.Collections.Generic;
using UnityEngine;

public static class TournamentAlph
{
	public static string GetAlphScore(string score)
	{
		string result;
		try
		{
			int index = UnityEngine.Random.Range(0, TournamentAlph.literalKeys.Count);
			string text = TournamentAlph.literalKeys[index];
			List<char> list = TournamentAlph.literals[text];
			string text2 = string.Empty;
			for (int i = 0; i < score.Length; i++)
			{
				text2 += TournamentAlph.GetConversion(score[i], list);
			}
			result = text + text2;
		}
		catch
		{
			result = "ALPH-SCORE-FAILED";
		}
		return result;
	}

	private static char GetConversion(char numericChar, List<char> literals)
	{
		char result;
		try
		{
			int index = int.Parse(numericChar.ToString());
			result = literals[index];
		}
		catch
		{
			result = 'X';
		}
		return result;
	}

	private const string literalKey3 = "CIHIEIAIT-";

	private static readonly List<char> literals1 = new List<char>
	{
		'P',
		'O',
		'I',
		'U',
		'Y',
		'T',
		'R',
		'E',
		'W',
		'Q'
	};

	private const string literalKey2 = "BIAICIK-";

	private static readonly List<char> literals2 = new List<char>
	{
		'A',
		'B',
		'C',
		'D',
		'E',
		'F',
		'G',
		'H',
		'I',
		'J'
	};

	private const string literalKey1 = "OINIE-";

	private static readonly List<char> literals3 = new List<char>
	{
		'U',
		'J',
		'N',
		'Y',
		'H',
		'B',
		'T',
		'G',
		'V',
		'R'
	};

	private static readonly List<string> literalKeys = new List<string>
	{
		"OINIE-",
		"BIAICIK-",
		"CIHIEIAIT-"
	};

	private static readonly Dictionary<string, List<char>> literals = new Dictionary<string, List<char>>
	{
		{
			TournamentAlph.literalKeys[0],
			TournamentAlph.literals1
		},
		{
			TournamentAlph.literalKeys[1],
			TournamentAlph.literals2
		},
		{
			TournamentAlph.literalKeys[2],
			TournamentAlph.literals3
		}
	};
}
