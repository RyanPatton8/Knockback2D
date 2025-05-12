using Godot;
using System;

public partial class PlayerInfo : Node
{
	private int playerIndex;
	private int lives = 3;
	private int kills;
	private Color playerColor = new Color();
	private int arrowCount = 8;
	private int hookCount = 4;
	private float startingDamage = 0;
	private float damageTaken = 0;
	private float damageGiven = 0;
	private float startingComboCount = 1;
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
	}

	public void ResetVariables(){
		lives = 3;
		kills = 0;
		arrowCount = 8;
		hookCount = 4;
		startingDamage = 0;
		damageTaken = 0;
		damageGiven = 0;
		startingComboCount = 1;
		comboCount = 1;
		playerManager.playerGUIHolder.ResetCardInfo(playerIndex);
	}
}
