using Godot;
using System;
using System.Collections.Generic;

public partial class Elimination : GameMode
{
	public Elimination(bool teamsOn)
	{
		playerManager = PlayerManager.Instance;
		gameManager = GameManager.Instance;
		audioManager = AudioManager.Instance;
		this.teamsOn = teamsOn;
	}
	public override bool IsGameOver()
	{
		(int highestKills, bool tie) = playerManager.GetHighestKills();
		if(tie){ return false; }

		if (highestKills >= 10)
		{
			gameManager.isInMenu = true;
			audioManager.PlayNextSong();
			return true;
		}
		else if (playerManager.IsOneTeamRemaining())
		{
			gameManager.LoadRandomLevel();
		}
		GD.Print($"Players ALive: {playerManager.IsOneTeamRemaining()}");
		return false;
	}

    public override bool ShouldRespawn(int playerIndex)
    {
		return false;
    }
}
