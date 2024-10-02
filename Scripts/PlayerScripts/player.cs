/*
    -Change Damage to increase similar to smashbros
    -Change Knockback duration to follow suit
    -Add Bow
    -Add Fishing Rod mechanic
    -Rename Knockback and Dash variables to be Stun and Jump related
    -Add Respawning and lives
*/
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
    [Export] public WeaponHolder WeaponHolder {get; private set;}
    [Export] public Timer DashCoolDown {get; private set;}
    private float offsetAmount = 23;
    private float comboCount = 1;
    private int dashCount = 2;
    private int jumpCount = 3;
    public bool canAttack = true;
	private bool isGrounded = false;
    private bool knockedBack = false;
    private bool isJumping = false;
    public bool canDash = true;

	public override void _Ready()
	{
		GroundCheck.BodyEntered += Grounded;
		GroundCheck.BodyExited += UnGrounded;
        HurtBox.AreaEntered += RecieveHit;
        AttackDuration.Timeout += StopAttacking;
        AttackCoolDown.Timeout += ResetAttack;
        KnockBackDuration.Timeout += AllowMovement;
        DashCoolDown.Timeout += AllowDash;
	}

	public override void _PhysicsProcess(double delta)
	{
        if (!knockedBack){
            Move(delta);
            Aim();
        }
	}

    private void Move(double delta)
    {
        // Handle Jumps and Dash
        if (!Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && isGrounded)
        {
            PhysicsMaterialOverride.Friction = 0.6f;
        }
        else if (Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && jumpCount > 0 && canDash){//previously just checked if grounded and didnt adjust linear velocity or jumpCount
            LinearVelocity = new Vector2(LinearVelocity.X, 0);
            ApplyImpulse(new Vector2(0, -4500)); //previously 2500
            jumpCount--;
            canDash = false;
            DashCoolDown.WaitTime = .25;
            DashCoolDown.Start();
        }
        // else if (Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && !isGrounded && canDash && dashCount > 0){
        //     Vector2 dashDirection = (HitBox.GlobalPosition - GlobalPosition).Normalized();
        //     DashCoolDown.WaitTime = .35;
        //     LinearVelocity = new Vector2(LinearVelocity.X, 0);
        //     ApplyImpulse(new Vector2(dashDirection.X * 2000, dashDirection.Y * 5000));
        //     canDash = false;
        //     DashCoolDown.Start();
        //     dashCount--;
        // }
        
        // Handle Movement in and out of air
        float direction = Input.GetJoyAxis(playerIndex, JoyAxis.LeftX);

        if (direction == -1 && LinearVelocity.X > 1 || direction == 1 && LinearVelocity.X < -1)
        {
            LinearVelocity = new Vector2(0, LinearVelocity.Y);
        }

        if (direction != 0 && LinearVelocity.X < maxMoveSpeed && LinearVelocity.X > maxMoveSpeed * -1)
        {
            ApplyForce(new Vector2(10000 * direction, 0));
        }  
    }

	private void Aim()
	{
        float aimX;
        float aimY;
        // Check if player is using right thumbstick otherwise use left
        if(Input.GetJoyAxis(playerIndex, JoyAxis.RightX) >= 0.5 || Input.GetJoyAxis(playerIndex, JoyAxis.RightX) <= -0.5
          || Input.GetJoyAxis(playerIndex, JoyAxis.RightY) >= 0.5 || Input.GetJoyAxis(playerIndex, JoyAxis.RightY) <= -0.5){
            aimX = Input.GetJoyAxis(playerIndex, JoyAxis.RightX);
		    aimY = Input.GetJoyAxis(playerIndex, JoyAxis.RightY);
        } else {
            aimX = Input.GetJoyAxis(playerIndex, JoyAxis.LeftX);
		    aimY = Input.GetJoyAxis(playerIndex, JoyAxis.LeftY);
        }
		
		Vector2 aimDirection = new Vector2(aimX, aimY);

		 if (aimDirection.Length() > 0.3)
        {
            aimDirection = aimDirection.Normalized();

            // Apply the offset to the aimDirection
            Vector2 newPosition = GlobalPosition + aimDirection * offsetAmount;

            // Rotate the HurtBox around the player
            HitBox.GlobalPosition = newPosition;

            // if wanted to physically rotate hitbox
            //HitBox.RotationDegrees = Mathf.RadToDeg(GlobalPosition.AngleToPoint(newPosition));
        }

        // allow weapon change
        if (Input.IsJoyButtonPressed(playerIndex, JoyButton.Y))
        {
            WeaponHolder.ChangeWeapon();
        }
	}

    private void RecieveHit(Area2D area)
    {
	Vector2 info = Vector2.Zero;
         // Cast the area to HitBox to access the GiveInfo method
        if (area is HitBox hitBox)
        {
            info = hitBox.GiveInfo();
        }
        // Make it so youre slidey, bounce, cant move and toss you're character in a direction based on several factors
        //  which ill write about more in depth later because I'm still adding them 
        knockedBack = true;
        PhysicsMaterialOverride.Friction = 0;
        PhysicsMaterialOverride.Bounce = 1;
        KnockBackDuration.WaitTime = 0.5;
        Vector2 hitDirection = new Vector2(info.X, info.Y).Normalized();
        ApplyImpulse(hitDirection * (comboCount > 0 ? comboCount * 5000 : 5000));    
        comboCount ++;
        KnockBackDuration.Start();
        GD.Print(comboCount);
        // make it so the ground check cant ground player temporarily so that it doesnt reset combo count
        GroundCheck.Monitoring = false;
    }

    // States
    private void Grounded(Node body)
	{
		if (body.IsInGroup("Environment") && LinearVelocity.Y >= -10){
			isGrounded = true;
            comboCount = 0;
            canDash = true;
            dashCount = 2;
            jumpCount = 3;
		}
	}

	private void UnGrounded(Node2D body)
	{
		if (body.IsInGroup("Environment")){
			isGrounded = false;
			// PhysicsMaterialOverride.Friction = 0;
		}
	}
    private void AllowMovement()
    {
        knockedBack = false;
        PhysicsMaterialOverride.Bounce = 0;
        GroundCheck.Monitoring = true;
    }

    private void AllowDash()
    {
        canDash = true;
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