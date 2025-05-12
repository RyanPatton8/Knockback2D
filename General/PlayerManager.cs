using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerManager : Node
{
    PackedScene player = (PackedScene)ResourceLoader.Load("res://Scenes/Player/player.tscn");
    PackedScene playerGUI = (PackedScene)ResourceLoader.Load("res://Scenes/Player/player_info_ui_holder.tscn");
    public PlayerInfoGUI playerGUIHolder;
    private static PlayerManager _instance;
    public Dictionary<int, PlayerInfo> playerList = new Dictionary<int, PlayerInfo>();
    public List<Marker2D> spawnPoints = new List<Marker2D>();
    public int playersAlive = 0;
    public AudioStreamPlayer2D Music = new AudioStreamPlayer2D();
    private AudioManager audioManager;
    private GameManager gameManager;
    
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
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            QueueFree();
        }
    }
    public override void _Ready()
    {
        gameManager = GameManager.Instance;
        playerGUIHolder = (PlayerInfoGUI) playerGUI.Instantiate();
        AddChild(playerGUIHolder);
        AddChild(Music);
        audioManager = AudioManager.Instance;
        Music.Finished += PlayNextSong;
        Music.Stream = audioManager.GetSong();
        Music.Play();
    }
    public void PlayNextSong(){
        AudioStream next = audioManager.GetSong();
        while (next == Music.Stream)
        {
            next = audioManager.GetSong();
        }
        Music.Stream = next;
        Music.Play();
    }
    //adds and remove players to and from dictionary indexed by player index
    public void AddPlayer(int playerIndex)
    {
        switch(playerIndex){
            case 0:
                 playerList.Add(playerIndex, new PlayerInfo(playerIndex, 3, new Color(.65f,0,0), 4, 4));
                 playerGUIHolder.AddCard(0);
                 break;
            case 1:
                 playerList.Add(playerIndex, new PlayerInfo(playerIndex, 3, new Color(0,0,.65f), 4, 4));
                 playerGUIHolder.AddCard(1);
                 break;
            case 2:
                 playerList.Add(playerIndex, new PlayerInfo(playerIndex, 3,new Color(.65f,.65f,0), 4, 4));
                 playerGUIHolder.AddCard(2);
                 break;
            case 3:
                 playerList.Add(playerIndex, new PlayerInfo(playerIndex, 3,new Color(0,.65f,0), 4, 4));
                 playerGUIHolder.AddCard(3);
                 break;
            default:
                GD.Print("Player index not found in PlayerManager AddPlayer()");
                break;
        }
    }
    public void RemovePlayer(int playerIndex)
    {
        playerList.Remove(playerIndex);
        playerGUIHolder.RemoveCard(playerIndex);
    }
    public void ClearPlayerList()
    {
        playerList.Clear();
        playerGUIHolder.RemoveAll();
        playerGUIHolder.Visible = false;
    }
    //instantiate the player at a specified spawn
    public void SpawnPlayer(int playerIndex, Vector2 spawnPoint)
    {
        // Defer the instantiation and adding to the scene tree
        Player instance = (Player)player.Instantiate();
        GetTree().Root.CallDeferred("add_child", instance);
        instance.playerColor = playerList[playerIndex].GetColor();
        instance.GlobalPosition = spawnPoint;
        instance.playerIndex = playerIndex;
    }
    //decrement player life if they are not at 0 respawn otherwise check for game over
    public void LoseALife(int playerIndex, int killerIndex)
    {
        playerList[playerIndex].SetLives(-1);
        playerList[playerIndex].SetArrowCount(8);
        playerList[playerIndex].SetHookCount(4);
        playerList[playerIndex].SetDamageTaken(0);
        playerList[playerIndex].SetComboCount(1);
        if(killerIndex != -1){
            playerList[killerIndex].SetKills(1);
            GD.Print($"player {killerIndex} has {playerList[killerIndex].GetKills()} kills");
        }
        else{
            GD.Print("player has killed themself");
        }

        if (playerList[playerIndex].GetLives() > 0){
            // Respawn player after losing a life
            Random rnd = new Random();
            Vector2 spawnPoint = spawnPoints[rnd.Next(0, spawnPoints.Count)].GlobalPosition;
            CallDeferred(nameof(SpawnPlayer), playerIndex, spawnPoint);
        }
        else{
            playerGUIHolder.playerCards[playerIndex].MakeBlank();
            playersAlive--;
            gameManager.CheckForGameOver();
        }
    }
}