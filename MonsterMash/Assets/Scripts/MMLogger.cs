using UnityEngine;

public class MMLogger
{
	static bool LoggingAllowed = true;

	public static void Log(string message, Object context=null)
	{
		if (LoggingAllowed)
		{
			Debug.Log(message, context);
		}
	}

	public static void LogWarning(string message, Object context=null)
	{
		if (LoggingAllowed)
		{
			Debug.LogWarning(message, context);
		}
	}

	public static void LogError(string message, Object context=null)
	{
		if (LoggingAllowed)
		{
			Debug.LogError(message, context);
		}
	}
	
}