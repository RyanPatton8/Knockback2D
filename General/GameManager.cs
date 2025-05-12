using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
	private static GameManager _instance;
	PlayerManager playerManager;
	public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }
    //ensures PlayerManager is always in every scene
    public override void _EnterTree()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            QueueFree();
        }
    }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerManager = PlayerManager.Instance;
	}

	public void StartGame(PackedScene Level)
	{
		playerManager.playerGUIHolder.Visible = true;
		foreach (KeyValuePair<int, PlayerInfo> kvp in playerManager.playerList)
		{
			kvp.Value.ResetVariables();
		}
		GetTree().ChangeSceneToPacked(Level);
	}

	public void LevelSelect()
	{
		foreach (Node child in GetTree().Root.GetChildren())
		{
			if (child is Player player)
			{
				player.QueueFree();
			}
		}
		GetTree().ChangeSceneToFile("res://Scenes/Menus/LevelSelect.tscn");
	}

	public void ReadyUp(PackedScene Level)
	{
		GetTree().ChangeSceneToPacked(Level);
	}

	public void MainMenu()
	{

	}

	public void CheckForGameOver()
    {
        if (playerManager.playersAlive <= 1)
        {
            CallDeferred(nameof(ChangeScene));
        }
    }
    private void ChangeScene()
    {
		playerManager.playerGUIHolder.Visible = false;
        GetTree().ChangeSceneToFile("res://Scenes/Menus/ready_up.tscn");
		playerManager.playersAlive = playerManager.playerList.Count;
    }
}
