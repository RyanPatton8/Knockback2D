using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ReadyUp : Node2D
{
	PlayerManager playerManager;
	[Export] public Node PlayerSpawn { get; private set;}
	public List<Marker2D> spawns = new List<Marker2D>();
	private Random rnd = new Random();
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
		
		spawns = PlayerSpawn.GetChildren().OfType<Marker2D>().ToList();
		playerManager.spawnPoints = spawns;

		foreach (KeyValuePair<int, PlayerManager.PlayerInfo> kvp in playerManager.playerList)
		{
			Vector2 spawnPoint = spawns[rnd.Next(0, spawns.Count())].GlobalPosition;
			playerManager.SpawnPlayer(kvp.Key, spawnPoint);
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
			foreach (Node child in GetTree().Root.GetChildren())
			{
				if (child is Player player)
				{
					player.QueueFree();
				}
			}
			GetTree().ChangeSceneToFile("res://Scenes/Menus/LevelSelect.tscn");
		}
	}
	private void AddPlayer(int playerIndex)
	{
		if (playerManager.playerList.ContainsKey(playerIndex))
            return;
		playerManager.playersAlive++;
		playerManager.AddPlayer(playerIndex);
		Vector2 spawnPoint = spawns[rnd.Next(0, spawns.Count())].GlobalPosition;
		playerManager.SpawnPlayer(playerIndex, spawnPoint);
	}

	private void RemovePlayer(int playerIndex)
	{
		if (!playerManager.playerList.ContainsKey(playerIndex))
            return;
		playerManager.playersAlive--;
		playerManager.RemovePlayer(playerIndex);
		foreach (Node child in GetTree().Root.GetChildren())
		{
			if (child is Player player && player.playerIndex == playerIndex)
			{
				player.QueueFree();
			}
		}
	}
}
