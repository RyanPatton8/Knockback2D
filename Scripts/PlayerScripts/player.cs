using Godot;
using System;

public partial class Player : RigidBody2D
{
    [Export] public int playerIndex {get; set;}
	[Export] public Area2D GroundCheck { get; private set; }
	[Export] public float maxMoveSpeed { get; private set; }
    [Export] public Area2D HitBox { get; private set; }
	[Export] public Area2D HurtBox { get; private set; }
    [Export] public Timer KnockBackDuration {get; private set;}

    [Export] public Timer AttackDuration {get; private set;}
    [Export] public Timer AttackCoolDown {get; private set;}

    private float offsetAmount = 23;
	private bool isGrounded = false;
    private bool canAttack = true;

    private bool knockedBack = false;

	public override void _Ready()
	{
		GroundCheck.BodyEntered += Grounded;
		GroundCheck.BodyExited += UnGrounded;
        HurtBox.AreaEntered += RecieveHit;
        AttackDuration.Timeout += StopAttacking;
        AttackCoolDown.Timeout += ResetAttack;
        KnockBackDuration.Timeout += AllowMovement;
	}

	public override void _PhysicsProcess(double delta)
	{
        Move();
		Aim();
	}

    private void Move()
    {
        if (!knockedBack){
            if (!Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && isGrounded)
            {
                PhysicsMaterialOverride.Friction = 0.6f;
            }
            else if (Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && isGrounded){
                ApplyImpulse(new Vector2(0, -2500));
            }

            float direction = Input.GetJoyAxis(playerIndex, JoyAxis.LeftX);

            if (direction == -1 && LinearVelocity.X > 1 || direction == 1 && LinearVelocity.X < -1)
            {
                LinearVelocity = new Vector2(0, LinearVelocity.Y);
            }

            if (direction != 0 && LinearVelocity.X < maxMoveSpeed && LinearVelocity.X > maxMoveSpeed * -1)
            {
                ApplyForce(new Vector2(15000 * direction, 0));
            }  
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

	private void Aim()
	{
		float aimX = Input.GetJoyAxis(playerIndex, JoyAxis.RightX);
		float aimY = Input.GetJoyAxis(playerIndex, JoyAxis.RightY);

		Vector2 aimDirection = new Vector2(aimX, aimY);

		 if (aimDirection.Length() > 0.3)
        {
            aimDirection = aimDirection.Normalized();

            // Apply the offset to the aimDirection
            Vector2 newPosition = GlobalPosition + aimDirection * offsetAmount;

            // Rotate the HurtBox around the player
            HitBox.GlobalPosition = newPosition;
            //HitBox.RotationDegrees = Mathf.RadToDeg(GlobalPosition.AngleToPoint(newPosition));
        }

        if(Input.IsJoyButtonPressed(playerIndex, JoyButton.RightShoulder))
        {
            Attack();
        }
	}

    private void Attack()
    {
        if(canAttack){
            GD.Print("Attacked");
            canAttack = false;
            HitBox.Monitoring = true;
            HitBox.Monitorable = true;
            AttackDuration.Start();
        }
    }

    private void RecieveHit(Area2D area)
    {
        knockedBack = true;
        PhysicsMaterialOverride.Friction = 0;
        KnockBackDuration.WaitTime = 0.5;
        ApplyImpulse(new Vector2(GlobalPosition.X - area.GlobalPosition.X ,(GlobalPosition.Y - area.GlobalPosition.Y) * -2) * 200);
        GD.Print("Ouch");
        KnockBackDuration.Start();
    }

    private void AllowMovement()
    {
        knockedBack = false;
    }

    private void StopAttacking()
    {
        HitBox.Monitoring = false;
        HitBox.Monitorable = false;
        AttackCoolDown.Start();
    }

    private void ResetAttack()
    {
        canAttack = true;
    }
}
