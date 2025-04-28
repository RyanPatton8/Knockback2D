using Godot;
using System;

public partial class PlayBtn : Button
{
	// Called when the node enters the scene tree for the first time.
	[Export] public PackedScene Level {get; private set;}
	public override void _Ready()
	{
		GrabFocus();
		ButtonDown += ChangeToLevelScene;
	}

	private void ChangeToLevelScene()
    {
		GetTree().ChangeSceneToPacked(Level);
	}
}
