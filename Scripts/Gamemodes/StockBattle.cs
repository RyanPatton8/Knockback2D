using Godot;
using System;

public partial class StockBattle : GameMode
{
	PlayerManager playerManager;
	public StockBattle(){
		playerManager = PlayerManager.Instance;
	}

	public override bool IsGameOver(){
		if (playerManager.playersAlive <= 1)
        {
            return true;
        }
		return false;
	}
}
