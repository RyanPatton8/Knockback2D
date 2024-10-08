using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerManager : Node
{
    PackedScene player = (PackedScene)ResourceLoader.Load("res://Scenes/player.tscn");
    private static PlayerManager _instance;

    public Dictionary<int, int> playerLives = new Dictionary<int, int>();
    public List<Marker2D> spawnPoints = new List<Marker2D>();

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

        if (playerLives.Count <= 1)
        {
            CallDeferred(nameof(ChangeScene));
        }
    }

    private void ChangeScene()
    {
        // Ensure the scene change is handled safely after deferring
        GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
    }

    public void SpawnPlayer(int playerIndex, Vector2 spawnPoint)
    {
        // Defer the instantiation and adding to the scene tree
        Player instance = (Player)player.Instantiate();
        GetTree().Root.CallDeferred("add_child", instance);
        instance.GlobalPosition = spawnPoint;
        instance.playerIndex = playerIndex;
    }

    public void LoseALife(int playerIndex)
    {
        playerLives[playerIndex]--;

        if (playerLives[playerIndex] <= 0)
        {
            // Defer the removal to prevent issues with physics callbacks
            CallDeferred(nameof(RemovePlayer), playerIndex);
        }
        else
        {
            // Respawn player after losing a life
            Random rnd = new Random();
            Vector2 spawnPoint = spawnPoints[rnd.Next(0, spawnPoints.Count)].GlobalPosition;
            CallDeferred(nameof(SpawnPlayer), playerIndex, spawnPoint);
        }
    }
}

