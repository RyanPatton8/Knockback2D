using Godot;
using System;

public partial class Slash : Area2D
{
	public int playerIndex;
	public Player player;
	double attackDuration = .25;

	public float attackStrength = 0;
	public override void _Process(double delta)
	{
		attackDuration -= delta;
		if(attackDuration < Mathf.Epsilon)
			QueueFree();
	}

	public (Vector2, float) GiveInfo()
	{
		return (GlobalPosition - player.GlobalPosition, attackStrength);
	}
}
