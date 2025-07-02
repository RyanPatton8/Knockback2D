using Godot;
using System;
using System.Data.Common;
using System.Diagnostics;

public partial class Slash : Area2D
{
	public int playerIndex;
	public Player player;
	double attackDuration = .35;
	private bool hittingPlayer = false;

	private string uuid = Guid.NewGuid().ToString();
    public override void _Ready()
    {
		BodyEntered += Reflect;
		// AreaEntered += Clash;
    }
    public override void _Process(double delta)
	{
		if(player.isClashing){
			CallDeferred("queue_free");
		}
		attackDuration -= delta;
		if(attackDuration < Mathf.Epsilon){
			QueueFree();
		}
	}
	public void Reflect(Node2D body)
	{
		if (body is RigidBody2D rb && (body is Bullet || body is Hook))
		{
			var hb = player.HitBox as HitBox;
        	hb.Clash();
			rb.LinearVelocity = Vector2.Zero;
			Vector2 reflectDir = (player.HitBox.GlobalPosition - player.GlobalPosition).Normalized();
			rb.ApplyImpulse(reflectDir * 1500);
			player.isAttacking = false;
			// CallDeferred("queue_free");
			if (body is Bullet arrow)
			{
				arrow.playerIndex = playerIndex;
				arrow.arrowSprite.Modulate = player.playerColor;
			}
		}
	}
	public (Vector2, int, string, HitBox) GiveInfo()
	{
		var hb = player.HitBox as HitBox;
		attackDuration = 0.15f;
		hittingPlayer = true;
		return (GlobalPosition - player.GlobalPosition, playerIndex, uuid, hb);
	}
	private void Clash(Area2D area){
		if(area is Slash slash && !hittingPlayer){
			var hb = player.HitBox as HitBox;
       	 	hb.Clash();
			player.isClashing = true;
			player.Clashed(player.GlobalPosition - slash.player.GlobalPosition, slash.playerIndex, true);
			CallDeferred("queue_free");
		}
	}
}
