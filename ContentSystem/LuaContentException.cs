using System;

public class LuaContentException : Exception
{
	public string ScriptPath { get; }
	public string LuaStackTrace { get; }
	public int? LineNumber { get; }
	
	public LuaContentException(string message, string scriptPath, string luaStackTrace, int? lineNumber, Exception inner = null) 
		: base(FormatMessage(message, scriptPath, lineNumber), inner)
	{
		ScriptPath = scriptPath;
		LuaStackTrace = luaStackTrace;
		LineNumber = lineNumber;
	}
	
	private static string FormatMessage(string message, string scriptPath, int? lineNumber)
	{
		var location = lineNumber.HasValue ? $" at line {lineNumber}" : "";
		return $"Error in {scriptPath}{location}: {message}";
	}
}
