using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Level : Node2D
{
	PlayerManager playerManager;
	[Export] public Marker2D[] spawns { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Random rnd = new Random();
		playerManager = PlayerManager.Instance;
		playerManager.spawnPoints = spawns.ToList<Marker2D>();

		foreach (KeyValuePair<int, PlayerManager.PlayerInfo> kvp in playerManager.playerList)
		{
			Vector2 spawnPoint = spawns[rnd.Next(0, spawns.Length)].GlobalPosition;
			playerManager.SpawnPlayer(kvp.Key, spawnPoint);
		}
	}
}
