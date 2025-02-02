using Godot;
using System;
using System.Data.Common;

public partial class Slash : Area2D
{
	public int playerIndex;
	public Player player;
	double attackDuration = .35;
    public override void _Ready()
    {
		BodyEntered += Reflect;
		AreaEntered += Clash;
    }
    public override void _Process(double delta)
	{
		attackDuration -= delta;
		if(attackDuration < Mathf.Epsilon){
			if(player.isClashing){
				player.isClashing = false;
			}
			QueueFree();
		}
	}
	public void Reflect(Node2D body)
	{
		if (body is RigidBody2D rb && (body is Arrow || body is Hook))
		{
			rb.LinearVelocity = Vector2.Zero;
			Vector2 reflectDir = (player.HitBox.GlobalPosition - player.GlobalPosition).Normalized();
			rb.ApplyImpulse(reflectDir * 1500);
			player.isAttacking = false;
			CallDeferred("queue_free");
			if(body is Arrow arrow)
			{
				arrow.playerIndex = playerIndex;
			}
		}
	}
	public (Vector2, int) GiveInfo()
	{
		attackDuration = 0.1f;
		return (GlobalPosition - player.GlobalPosition, playerIndex);
	}
	private void Clash(Area2D area){
		if(area is Slash slash){
			player.isClashing = true;
			player.Clashed(player.GlobalPosition - slash.player.GlobalPosition, slash.playerIndex);
			CallDeferred("queue_free");
		}
	}
}
