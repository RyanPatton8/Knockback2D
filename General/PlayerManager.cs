using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerManager : Node
{
	PackedScene player = (PackedScene)ResourceLoader.Load("res://Scenes/player.tscn");
	private static PlayerManager _instance;
	public Dictionary<int, int> playerLives = new Dictionary<int, int>();
	private List<Marker2D> spawnPoints = new List<Marker2D>();

	public static PlayerManager Instance
	{
		get
		{
			return _instance;
		}
	}
	public override void _EnterTree()
	{
		GD.Print("Entered");
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			QueueFree();
		}
	}
	public void AddPlayer(int playerIndex)
	{
		if (playerLives.ContainsKey(playerIndex))
			return;
		playerLives.Add(playerIndex, 3);
	}
	public void RemovePlayer(int playerIndex)
	{
		if (!playerLives.ContainsKey(playerIndex))
			return;
		playerLives.Remove(playerIndex);
	}
	public void SpawnPlayer(int playerIndex, Vector2 spawnPoint)
	{
		Player instance = (Player)player.Instantiate();
		GetTree().Root.AddChild(instance);
		instance.GlobalPosition = spawnPoint;
		instance.playerIndex = playerIndex;
	}
	public void LoseALife(int playerIndex)
	{
		playerLives[playerIndex] -= 1;
		if (playerLives[playerIndex] <= 0)
		{
			RemovePlayer(playerIndex);
		}
		else
		{
			Random rnd = new Random();
			SpawnPlayer(playerIndex, spawnPoints[rnd.Next(0, spawnPoints.Count)].GlobalPosition);
		}
	}
}
