using Godot;
using System;

public partial class LevelSelect : Control
{
	// Called when the node enters the scene tree for the first time.
	PlayerManager playerManager;
	GameManager gameManager;
	public override void _Ready()
	{
		playerManager = PlayerManager.Instance;
		gameManager = GameManager.Instance;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsJoyButtonPressed(0, JoyButton.B) && playerManager.playerList.Count > 0){
			gameManager.ReadyUp(gameManager.gameMode, false);
		}
	}
}
