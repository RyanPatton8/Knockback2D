using Godot;
using System;

public partial class ReadyUp : Control
{
	[Export] public ColorRect[] players {get; private set;}
	PlayerManager playerManager;
    // // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Ready()
	{
		playerManager = PlayerManager.Instance;
		foreach (Node child in GetTree().Root.GetChildren())
		{
			if (child is Player player)
			{
				playerManager.playerList.Remove(player.playerIndex);
				player.QueueFree();
			}
		}
	}
    public override void _Process(double delta)
	{
		if(Input.IsJoyButtonPressed(0 , JoyButton.A)){
			AddPlayer(0);
		}else if(Input.IsJoyButtonPressed(1 , JoyButton.A)){
			AddPlayer(1);
		}else if(Input.IsJoyButtonPressed(2 , JoyButton.A)){
			AddPlayer(2);
		}else if(Input.IsJoyButtonPressed(3 , JoyButton.A)){
			AddPlayer(3);
		}

		if(Input.IsJoyButtonPressed(0 , JoyButton.B)){
			RemovePlayer(0);
		}else if(Input.IsJoyButtonPressed(1 , JoyButton.B)){
			RemovePlayer(1);
		}else if(Input.IsJoyButtonPressed(2 , JoyButton.B)){
			RemovePlayer(2);
		}else if(Input.IsJoyButtonPressed(3 , JoyButton.B)){
			RemovePlayer(3);
		}

		if(Input.IsJoyButtonPressed(0, JoyButton.Start) && playerManager.playerList.Count > 0){
			GetTree().ChangeSceneToFile("res://Scenes/Menus/LevelSelect.tscn");
		}
	}
	private void AddPlayer(int playerIndex)
	{
		if (playerManager.playerList.ContainsKey(playerIndex))
            return;
		Color currentColor = players[playerIndex].Color;
		currentColor.A = 255;
		players[playerIndex].Color = currentColor;
		playerManager.playersAlive++;
		playerManager.AddPlayer(playerIndex);
	}

	private void RemovePlayer(int playerIndex)
	{
		if (!playerManager.playerList.ContainsKey(playerIndex))
            return;
		playerManager.playersAlive--;
		Color currentColor = players[playerIndex].Color;
		currentColor.A = 0;
		players[playerIndex].Color = currentColor;
		playerManager.RemovePlayer(playerIndex);
	}
}
