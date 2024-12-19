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
        playerGUIHolder = (PlayerInfoGUI) playerGUI.Instantiate();
        AddChild(playerGUIHolder);
    }
    //adds and remove players to and from dictionary indexed by player index
    public void AddPlayer(int playerIndex)
    {
        switch(playerIndex){
            case 0:
                 playerList.Add(playerIndex, new PlayerInfo(playerIndex, 3, new Color(.5f,0,0), 4, 4));
                 playerGUIHolder.AddCard(0);
                 break;
            case 1:
                 playerList.Add(playerIndex, new PlayerInfo(playerIndex, 3, new Color(0,0,.5f), 4, 4));
                 playerGUIHolder.AddCard(1);
                 break;
            case 2:
                 playerList.Add(playerIndex, new PlayerInfo(playerIndex, 3,new Color(.5f,.5f,0), 4, 4));
                 playerGUIHolder.AddCard(2);
                 break;
            case 3:
                 playerList.Add(playerIndex, new PlayerInfo(playerIndex, 3,new Color(0,.5f,0), 4, 4));
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
    //Checks to see if the total players left alive are equal or less than one and brings back to lobby if thats the case
    public void CheckForGameOver()
    {
        if (playersAlive <= 1)
        {
            CallDeferred(nameof(ChangeScene));
        }
    }
    private void ChangeScene()
    {
        // Ensure the scene change is handled safely after deferring
        GetTree().ChangeSceneToFile("res://Scenes/Levels/main.tscn");
        CallDeferred(nameof(ClearPlayerList));
        playersAlive = 0;
    }
    private void ClearPlayerList()
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
    public void LoseALife(int playerIndex)
    {
        playerList[playerIndex].SetLives(-1);
        playerList[playerIndex].SetArrowCount(4);
        playerList[playerIndex].SetHookCount(4);
        playerList[playerIndex].SetDamageTaken(0);
        playerList[playerIndex].SetComboCount(1);

        if (playerList[playerIndex].GetLives() > 0){
            // Respawn player after losing a life
            Random rnd = new Random();
            Vector2 spawnPoint = spawnPoints[rnd.Next(0, spawnPoints.Count)].GlobalPosition;
            CallDeferred(nameof(SpawnPlayer), playerIndex, spawnPoint);
        }
        else{
            playerGUIHolder.playerCards[playerIndex].MakeBlank();
            playersAlive--;
            CheckForGameOver();
        }
    }
    public class PlayerInfo
    {
        private int playerIndex;
        private int lives = 3;
        private int kills;
        private Color playerColor = new Color();
        private int arrowCount = 4;
        private int hookCount = 4;
        private float damageTaken = 0;
        private float damageGiven = 0;
        private float comboCount = 1;
        PlayerManager playerManager;
        public PlayerInfo() {}
        public PlayerInfo(int playerIndex, int lives, Color playerColor, int arrowCount, int hookCount){
            this.playerIndex = playerIndex;
            this.lives = lives;
            this.playerColor = playerColor;
            playerManager = PlayerManager.Instance;
        }
        public int GetLives(){
            return lives;
        }
        public void SetLives(int liveLost){
            lives += liveLost;
            playerManager.playerGUIHolder.playerCards[playerIndex].SetLivesCount(lives.ToString());
        }
        public int GetKills(){
            return kills;
        }
        public void SetKills(int newKill){
            kills += newKill;
        }
        public Color GetColor(){
            return playerColor;
        }
        public void SetColor(Color newColor){
            playerColor = newColor;
        }
        public int GetArrowCount(){
            return arrowCount;
        }
        public void SetArrowCount(int arrowCount){
            this.arrowCount = arrowCount;
            playerManager.playerGUIHolder.playerCards[playerIndex].SetArrowCount(arrowCount.ToString());
        }
        public int GetHookCount(){
            return hookCount;
        }
        public void SetHookCount(int hookCount){
            this.hookCount = hookCount;
            playerManager.playerGUIHolder.playerCards[playerIndex].SetHookCount(hookCount.ToString());
        }
        public float GetDamageTaken(){
            return damageTaken;
        }
        public void SetDamageTaken(float damage){
            if(damage <= 1500){
                damageTaken = 0;
            }
            else{
                damageTaken = (damage / 100) - 15;
            }
            playerManager.playerGUIHolder.playerCards[playerIndex].SetHealth(damageTaken.ToString());
        }
        public float GetDamageGiven(){
            return damageGiven;
        }
        public void SetDamageGiven(float damage){
            damageGiven += damage;
        }
        public float GetComboCount(){
            return comboCount;
        }
        public void SetComboCount(float newComboCount){
            comboCount = newComboCount;
            playerManager.playerGUIHolder.playerCards[playerIndex].SetComboCount(comboCount.ToString());
        }
    }
}