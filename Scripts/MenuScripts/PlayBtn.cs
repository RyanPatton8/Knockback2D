using Godot;
using System;

public partial class PlayBtn : Button
{
	// Called when the node enters the scene tree for the first time.
	[Export] public PackedScene Level {get; private set;}
	GameManager gameManager;
	public override void _Ready()
	{
		GrabFocus();
		ButtonDown += ChangeToLevelScene;
		gameManager = GameManager.Instance;
	}

	private void ChangeToLevelScene()
    {
		gameManager.ReadyUp(Level);
	}
}
