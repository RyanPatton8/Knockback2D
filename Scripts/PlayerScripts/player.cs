using Godot;
using System;

public partial class Player : RigidBody2D
{
    [Export] public int playerIndex {get; set;}

    [Export] public PlayerStateMachine playerStateMachine {get; private set;}
	[Export] public Area2D GroundCheck { get; private set; }
	[Export] public float maxMoveSpeed { get; private set; }
    [Export] public Area2D HitBox { get; private set; }
	[Export] public Area2D HurtBox { get; private set; }
    [Export] public Timer KnockBackDuration {get; private set;}

    [Export] public Timer AttackDuration {get; private set;}
    [Export] public Timer AttackCoolDown {get; private set;}
    [Export] public KnockedState knockedState {get; private set;}

    protected float offsetAmount = 23;
	public bool isGrounded = false;
    protected bool canAttack = true;

    private bool knockedBack = false;

    public float direction = 0;

	public override void _Ready()
	{
		GroundCheck.BodyEntered += Grounded;
		GroundCheck.BodyExited += UnGrounded;
        HurtBox.AreaEntered += RecieveHit;
        AttackDuration.Timeout += StopAttacking;
        AttackCoolDown.Timeout += ResetAttack;
	}

	public override void _PhysicsProcess(double delta)
	{
        direction = Input.GetJoyAxis(playerIndex, JoyAxis.LeftX);
        Jump();
		Aim();
	}

    private void Jump()
	{
		if(Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && isGrounded)
		{
			ApplyImpulse(new Vector2(0, -2500));
		}
	}

	private void Grounded(Node body)
	{
		if (body.IsInGroup("Environment")){
			isGrounded = true;
            if (!Input.IsJoyButtonPressed(playerIndex, JoyButton.A))
                PhysicsMaterialOverride.Friction = 0.6f;
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
            HitBox.RotationDegrees = Mathf.RadToDeg(GlobalPosition.AngleToPoint(newPosition));
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
        GD.Print("ouch");
        playerStateMachine.ChangeState("KnockedState", playerIndex, null);
    
        // Get the instance of the current state and call KnockedBack
        knockedState.KnockedBack(area);
    }

    private void StopAttacking()
    {
        HitBox.Monitoring = false;
        HitBox.Monitorable = false;
        AttackCoolDown.Start();
        GD.Print("AttackOver");
    }

    private void ResetAttack()
    {
        canAttack = true;
    }
}
