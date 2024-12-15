using Godot;

public partial class CutsceneDemo : Node
{
	private CutscenePlayer _cutscenePlayer;

	public override void _Ready()
	{
		_cutscenePlayer = new CutscenePlayer();
		AddChild(_cutscenePlayer);

		// Connect to completion signal
		_cutscenePlayer.CutsceneCompleted += OnCutsceneCompleted;

		// Start the demo cutscene
		_cutscenePlayer.PlayCutscene("Content/cutscenes/intro.lua");
	}

	private void OnCutsceneCompleted()
	{
		GD.Print("Demo cutscene completed!");
	}
}
