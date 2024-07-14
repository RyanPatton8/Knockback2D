using Godot;
using System;

public partial class player : RigidBody2D
{
	[Export] public Area2D GroundCheck { get; private set; }
	[Export] public float maxMoveSpeed { get; private set; }
	//[Export] public Sprite2D HitSprite { get; private set; }

	private bool isGrounded = false;

	public override void _Ready()
	{
		GroundCheck.BodyEntered += Grounded;
		GroundCheck.BodyExited += UnGrounded;
	}

	public override void _PhysicsProcess(double delta)
	{
		//Aim();

		if (!Input.IsActionPressed("jump") && isGrounded)
		{
			PhysicsMaterialOverride.Friction = 0.6f;
		}
		else if (Input.IsActionPressed("jump") && isGrounded){
			ApplyImpulse(new Vector2(0, -2500));
		}

		float direction = Input.GetAxis("moveLeft", "moveRight");

		if (direction == -1 && LinearVelocity.X > 1 || direction == 1 && LinearVelocity.X < -1)
		{
			LinearVelocity = new Vector2(0, LinearVelocity.Y);
		}

		if (direction != 0 && LinearVelocity.X < maxMoveSpeed && LinearVelocity.X > maxMoveSpeed * -1)
		{
			ApplyForce(new Vector2(15000 * direction, 0));
		}
	}

	private void Grounded(Node body)
	{
		if (body.IsInGroup("Environment")){
			isGrounded = true;
		}
	}

	private void UnGrounded(Node2D body)
	{
		if (body.IsInGroup("Environment")){
			isGrounded = false;
			PhysicsMaterialOverride.Friction = 0;
		}
	}

	// private void Aim()
	// {
	// 	float aimX = Input.GetActionStrength("aimRight") - Input.GetActionStrength("aimLeft");
	// 	float aimY = Input.GetActionStrength("aimDown") - Input.GetActionStrength("aimUp");

	// 	Vector2 aimDirection = new Vector2(aimX, aimY);

	// 	 if (aimDirection.Length() > 0.3)
    // 	{
	// 		aimDirection = aimDirection.Normalized();
	// 		HitSprite.RotationDegrees = Mathf.RadToDeg(GlobalPosition.AngleToPoint(GlobalPosition + aimDirection));
    // 	}
	// }
}
