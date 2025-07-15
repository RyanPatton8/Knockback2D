using Godot;
using System;

public partial class StockBattle : GameMode
{
	public StockBattle(bool teamsOn)
	{
		playerManager = PlayerManager.Instance;
		gameManager = GameManager.Instance;
		audioManager = AudioManager.Instance;
		this.teamsOn = teamsOn;
	}
	public override bool IsGameOver()
	{
		if (playerManager.IsOneTeamRemaining())
		{
			gameManager.isInMenu = true;
			audioManager.PlayNextSong();
			return true;
		}
		return false;
	}

	public override bool ShouldRespawn(int playerIndex)
	{
		if (playerManager.playerList[playerIndex].GetLives() >= 1)
		{
			return true;
		}
		return false;
    }
}
