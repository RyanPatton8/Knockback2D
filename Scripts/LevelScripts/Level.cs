using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Level : Node2D
{
	PlayerManager playerManager;
	[Export] public Node PlayerSpawn { get; private set;}
	[Export] public Camera2D camera {get; private set;}
	public List<Marker2D> spawns = new List<Marker2D>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Random rnd = new Random();
		playerManager = PlayerManager.Instance;
		spawns = PlayerSpawn.GetChildren().OfType<Marker2D>().ToList();
		playerManager.spawnPoints = spawns;

		foreach (KeyValuePair<int, PlayerManager.PlayerInfo> kvp in playerManager.playerList)
		{
			Vector2 spawnPoint = spawns[rnd.Next(0, spawns.Count())].GlobalPosition;
			playerManager.SpawnPlayer(kvp.Key, spawnPoint);
		}
	}
}
