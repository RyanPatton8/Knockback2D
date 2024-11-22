using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerManager : Node
{
    PackedScene player = (PackedScene)ResourceLoader.Load("res://Scenes/player.tscn");
    private static PlayerManager _instance;

    public Dictionary<int, PlayerInfo> playerList = new Dictionary<int, PlayerInfo>();
    public List<Marker2D> spawnPoints = new List<Marker2D>();
    public int playersAlive = 0;

    //Class to store all sorts of player info in future will be used for storing player color, kills/death stats and more
    public class PlayerInfo
    {
        private int lives = 2;
        public PlayerInfo() {}
        public int getLives(){
            return lives;
        }
        public void setLives(int liveLost){
            lives += liveLost;
        }
    }
    //allows any script to reference PlayerManager
    public static PlayerManager Instance
    {
        get
        {
            return _instance;
        }
    }

    //ensures PlayerManager is always in every scene
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

    //adds and remove players to and from dictionary indexed by player index
    public void AddPlayer(int playerIndex)
    {
        playerList.Add(playerIndex, new PlayerInfo());
    }

    public void RemovePlayer(int playerIndex)
    {
        playerList.Remove(playerIndex);
    }

    //Checks to see if the total players left alive are equal or less than one and brings back to lobby if thats the case
    public void CheckForGameOver()
    {
        if (playersAlive <= 1)
        {
            CallDeferred(nameof(ChangeScene));
            GD.Print("Changing Scene");
        }
    }
    private void ChangeScene()
    {
        // Ensure the scene change is handled safely after deferring
        GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
    }

    //instantiate the player at a specified spawn
    public void SpawnPlayer(int playerIndex, Vector2 spawnPoint)
    {
        // Defer the instantiation and adding to the scene tree
        Player instance = (Player)player.Instantiate();
        GetTree().Root.CallDeferred("add_child", instance);
        instance.GlobalPosition = spawnPoint;
        instance.playerIndex = playerIndex;
    }

    //decrement player life if they are not at 0 respawn otherwise check for game over
    public void LoseALife(int playerIndex)
    {
        playerList[playerIndex].setLives(-1);

        if (playerList[playerIndex].getLives() > 0){
            // Respawn player after losing a life
            Random rnd = new Random();
            Vector2 spawnPoint = spawnPoints[rnd.Next(0, spawnPoints.Count)].GlobalPosition;
            CallDeferred(nameof(SpawnPlayer), playerIndex, spawnPoint);
        }
        else{
            playersAlive--;
            CheckForGameOver();
        }
    }
}

