using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
	private static GameManager _instance;

	[Signal] public delegate void PlayerDeathEventHandler(int playerIndex, int indexOfFinalAttacker);
	public GameMode gameMode;
	private List<string> levels;
	Random rnd = new Random();
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
		levels = GetLevelList();
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
				EmitSignal(nameof(PlayerDeath), player.playerIndex, player.indexOfFinalAttacker);
			}
		}
		GetTree().ChangeSceneToFile("res://Scenes/Menus/LevelSelect.tscn");
	}

	public void ReadyUp(GameMode chosenGameMode)
	{
		playerManager.playerGUIHolder.Visible = false;
		GetTree().ChangeSceneToFile("res://Scenes/Menus/ready_up.tscn");
		gameMode = chosenGameMode;
		playerManager.playersAlive = playerManager.playerList.Count;
	}

	public void MainMenu()
	{

	}

	public void CheckForGameOver()
	{
		if (gameMode.IsGameOver())
		{
			ReadyUp(gameMode);
		}
	}

	public void LoadRandomLevel()
	{
		GetTree().ChangeSceneToFile(levels[rnd.Next(0, levels.Count)]);
	}

	public List<string> GetLevelList()
	{
		List<string> levels = new List<string>();
		string folderPath = "res://Scenes/Levels/";
		// Open the directory (res:// is read-only inside the PCK)
		var dir = DirAccess.Open(folderPath);
		if (dir == null)
		{
			GD.PrintErr($"Could not open folder: {folderPath}");
			return [];
		}
		// tell it whether to include “.”/“..” and hidden files
		dir.SetIncludeNavigational(false);  // skip “.” and “..”
		dir.SetIncludeHidden(false);        // skip hidden files

		// now begin the listing
		dir.ListDirBegin();
		string fileName = dir.GetNext();
		while (fileName != string.Empty)
		{
			// Only files (not sub-folders)
			if (!dir.CurrentIsDir())
			{
				// filter by audio extension
				string ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
				if (ext == ".tscn")
				{
					string fullPath = $"{folderPath}/{fileName}";
					levels.Add(fullPath);
				}
			}
			fileName = dir.GetNext();
		}
		dir.ListDirEnd();
		return levels;
	}
}
