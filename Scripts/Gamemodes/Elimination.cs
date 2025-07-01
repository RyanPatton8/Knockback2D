using Godot;
using System;
using System.Collections.Generic;

public partial class Elimination : GameMode
{
	public override bool IsGameOver()
	{
		if (playerManager.GetHighestKills() > 1)
		{
			return true;
		}
		else if (playerManager.playersAlive <= 1)
		{
			gameManager.LoadRandomLevel();
		}
		return false;
	}

    public override bool ShouldRespawn(int playerIndex)
    {
		return false;
    }
}
