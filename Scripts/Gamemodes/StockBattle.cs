using Godot;
using System;

public partial class StockBattle : GameMode
{
	public StockBattle(PlayerManager playerManager, GameManager gameManager, bool teamsOn)
	{
		this.playerManager = playerManager;
		this.gameManager = gameManager;
		this.teamsOn = teamsOn;
	}
	public override bool IsGameOver()
	{
		if (playerManager.IsOneTeamRemaining())
		{
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
