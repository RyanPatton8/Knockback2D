using Godot;
using System;

public partial class StockBattle : GameMode
{
	public override bool IsGameOver()
	{
		if (playerManager.playersAlive <= 1)
		{
			return true;
		}
		return false;
	}

	public override bool ShouldRespawn(int playerIndex)
	{
		if (playerManager.playerList[playerIndex].GetLives() > 1)
		{
			return true;
		}
		return false;
    }
}
