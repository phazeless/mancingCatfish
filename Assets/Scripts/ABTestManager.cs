using System;

public static class ABTestManager
{
	

	private static string GetABTestStringKey(ABTest test, ABTestGroup testGroup)
	{
		return string.Concat(new object[]
		{
			"ABTest_",
			(int)test,
			"_",
			testGroup.ToString()
		});
	}

	private static ABTestGroup ParseTestGroupString(string testGroupAsString)
	{
		ABTestGroup result;
		try
		{
			result = (ABTestGroup)Enum.Parse(typeof(ABTestGroup), testGroupAsString);
		}
		catch
		{
			result = ABTestGroup.A;
		}
		return result;
	}

	private const string CONST_TRUE = "true";

	private const string CONST_FALSE = "false";
}
