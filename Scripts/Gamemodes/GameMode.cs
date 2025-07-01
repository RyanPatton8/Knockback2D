using Godot;
using System;

public abstract partial class GameMode : Node
{
	public bool teamsOn;
	public PlayerManager playerManager;
	public GameManager gameManager;
	public virtual bool IsGameOver() { return false; }

	public virtual bool ShouldRespawn(int playerIndex) { return false; }
}
