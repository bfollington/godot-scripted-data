using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Quest
{
	public string Id { get; set; }
	public string Title { get; set; }
	public int MinLevel { get; set; }
	public List<string> Objectives { get; } = new();
	public List<QuestReward> Rewards { get; } = new();

	public void AddObjective(string objective)
	{
		Objectives.Add(objective);
	}

	public void AddReward(QuestReward reward)
	{
		Rewards.Add(reward);
	}

	public override string ToString() =>
		$"Quest: {Title} (Level {MinLevel}) - {Objectives.Count} objectives";
}

[MoonSharpUserData]
public class QuestReward
{
	public string Type { get; set; }
	public string ItemId { get; set; }
	public int Amount { get; set; }

	public override string ToString() =>
		$"{Amount}x {ItemId} ({Type})";
}
