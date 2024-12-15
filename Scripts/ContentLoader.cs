using Godot;
using System;

public partial class ContentLoader : Node
{
	private LuaContentLoader _contentLoader;
	
	public override void _Ready()
	{
		_contentLoader = new LuaContentLoader();
		LoadContent();
	}
	
	private void LoadContent()
	{
		try
		{
			var quest = _contentLoader.LoadQuest("Content/quest_main.lua");
			GD.Print($"Successfully loaded quest: {quest}");
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error loading content: {e.Message}");
			GD.PrintErr($"Full exception: {e}");
		}
	}
}
