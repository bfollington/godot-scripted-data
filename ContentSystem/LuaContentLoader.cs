using MoonSharp.Interpreter;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class LuaContentLoader
{
	private readonly MoonSharp.Interpreter.Script _script;

	public LuaContentLoader()
	{
		// Register specific types we need
		UserData.RegisterType<Quest>();
		UserData.RegisterType<QuestReward>();
		UserData.RegisterType<List<string>>();
		UserData.RegisterType<List<QuestReward>>();

		_script = new MoonSharp.Interpreter.Script();

		// Create factory function that returns a properly wrapped Quest
		_script.Globals["createQuest"] = (Func<string, DynValue>)(id =>
		{
			var quest = new Quest { Id = id };
			return UserData.Create(quest);
		});

		// Create factory function that returns a properly wrapped QuestReward
		_script.Globals["createReward"] = (Func<string, string, int, DynValue>)((type, itemId, amount) =>
		{
			var reward = new QuestReward { Type = type, ItemId = itemId, Amount = amount };
			return UserData.Create(reward);
		});

		_script.Globals["debug"] = (Action<string>)(msg => GD.Print($"[Content Debug] {msg}"));
	}

	public Quest LoadQuest(string scriptPath)
	{
		try
		{
			GD.Print($"Loading script from {scriptPath}");
			var script = File.ReadAllText(scriptPath);
			GD.Print($"Script content:\n{script}");

			DynValue result = _script.DoString(script);

			GD.Print($"Script execution completed. Result type: {result.Type}");

			if (result.Type == DataType.Nil || result.Type == DataType.Void)
			{
				throw new Exception("Script did not return a quest object");
			}

			if (result.Type != DataType.UserData)
			{
				throw new Exception($"Script returned unexpected type: {result.Type}");
			}

			var quest = result.ToObject<Quest>();

			// Validate the quest
			if (quest == null)
			{
				throw new Exception("Failed to convert result to Quest object");
			}

			if (string.IsNullOrEmpty(quest.Title))
			{
				throw new Exception("Quest must have a title");
			}

			return quest;
		}
		catch (SyntaxErrorException e)
		{
			GD.PrintErr($"Lua Syntax Error: {e.Message}");
			GD.PrintErr($"Details: {e.DecoratedMessage}");
			throw;
		}
		catch (ScriptRuntimeException e)
		{
			GD.PrintErr($"Lua Runtime Error: {e.Message}");
			GD.PrintErr($"Details: {e.DecoratedMessage}");
			throw;
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error: {e.Message}");
			GD.PrintErr($"Stack trace: {e.StackTrace}");
			throw;
		}
	}
}
