using Godot;
using System;

public partial class Slash : Area2D
{
	public int playerIndex;
	public Player player;
	double attackDuration = .25;
	public int chargeLevel;
	public float attackStrength = 0;

    public override void _Ready()
    {
		BodyEntered += Reflect;
    }
    public override void _Process(double delta)
	{
		attackDuration -= delta;
		if(attackDuration < Mathf.Epsilon)
			QueueFree();
	}

	public void Reflect(Node2D body)
	{
		GD.Print("Reflecting");
		if (body is RigidBody2D rb && (body is Arrow || body is Hook) && chargeLevel == 3)
		{
			rb.LinearVelocity = rb.LinearVelocity * -1;
			if(body is Arrow arrow)
			{
				arrow.playerIndex = playerIndex;
			}
		}
	}
	public (Vector2, float) GiveInfo()
	{
		return (GlobalPosition - player.GlobalPosition, attackStrength);
	}
}
