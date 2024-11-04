using Godot;
using System;

public partial class HomingArea : Area2D
{
	// Called when the node enters the scene tree for the first time.
	private Player player;
	private RigidBody2D parent;

	private int playerIndex = 0;

	private Vector2 homingDir = Vector2.Zero;
	public override void _Ready()
	{
		parent = GetOwner<RigidBody2D>();
		BodyEntered += LockOn;
		BodyExited += LockOff;
	}

	private void LockOn(Node2D body)
	{
		if (body is Player player && this.player == null && player.playerIndex != playerIndex)
		{
			this.player = player;
			GD.Print("LockOn");
		}
	}

	private void LockOff(Node2D body)
	{
		if (body is Player && player != null)
		{
			player = null;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (player != null)
		{
			homingDir = player.GlobalPosition - parent.GlobalPosition;
			homingDir = homingDir.Normalized();

			parent.AddConstantForce(homingDir * 10000);
		}
	}
}
