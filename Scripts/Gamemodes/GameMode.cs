using Godot;
using System;

public abstract partial class GameMode : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	public virtual bool IsGameOver(){return false;}
}
