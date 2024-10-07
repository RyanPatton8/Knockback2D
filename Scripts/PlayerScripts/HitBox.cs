using Godot;
using System;

public partial class HitBox : Area2D
{
	[Export] public RigidBody2D Player { get; private set; }
	public Vector2 GiveInfo()
	{
		return this.GlobalPosition - Player.GlobalPosition;
	}
}
