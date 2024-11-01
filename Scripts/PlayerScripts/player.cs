/*
    -Clean up codebase
    -changebow to apply damage before knockback
    -change bow damage to be increased depending on arrow velocity
    -add shield
    -make it so you cant punch yourself
*/
using Godot;
using System;

public partial class Player : RigidBody2D
{
    // Exported Fields
    [Export] public int playerIndex { get; set; }
    [Export] public Area2D GroundCheck { get; private set; }
    [Export] public float maxMoveSpeed { get; set; }
    [Export] public Timer KnockBackDuration { get; private set; }
    [Export] public Area2D HurtBox { get; private set; }
    [Export] public Area2D HitBox { get; private set; }
    [Export] public WeaponHolder WeaponHolder { get; private set; }
    // How far from player should hitbox rotate
    private float offsetAmount = 23;
    // Movement Variables
    private bool isGrounded = false;
    private bool isJumping = false;
    public bool canJump = true;
    private int jumpCount = 2;
    // Combat Variables 
    private float comboCount = 1;
    private bool knockedBack = false;
    public float damageTaken = 1500;
    public bool changingWeapon = false;
    // Reference to player manager singleton
    PlayerManager playerManager;
    public override void _Ready()
    {
        GroundCheck.BodyEntered += Grounded;
        GroundCheck.BodyExited += UnGrounded;
        HurtBox.AreaEntered += RecieveMeleeHit;
        HurtBox.BodyEntered += RecieveRangedHit;
        KnockBackDuration.Timeout += AllowMovement;
        playerManager = PlayerManager.Instance;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!knockedBack)
        {
            Move(delta);
        }
        Aim();
        ChangeWeapon();
    }

    private void Move(double delta)
    {
        // Handle Jumps and Dash
        if (!Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && isGrounded)
        {
            PhysicsMaterialOverride.Friction = 0.6f;
        }
        else if (Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && jumpCount > 0 && canJump)
        {//previously just checked if grounded and didnt adjust linear velocity or jumpCount
            LinearVelocity = new Vector2(LinearVelocity.X, 0);
            ApplyImpulse(new Vector2(0, -4500)); //previously 2500
            jumpCount--;
            canJump = false;
            PhysicsMaterialOverride.Friction = 0f;
        }
        else if (!Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && jumpCount > 0 && !canJump)
        {
            canJump = true;
            PhysicsMaterialOverride.Friction = 0.6f;
        }
        else if (isGrounded && !canJump)
        {
            canJump = true;
        }

        // Handle Movement in and out of air
        float direction = Input.GetJoyAxis(playerIndex, JoyAxis.LeftX);

        if (direction == -1 && LinearVelocity.X > 1 || direction == 1 && LinearVelocity.X < -1)
        {
            LinearVelocity = new Vector2(0, LinearVelocity.Y);
        }

        if (direction != 0 && LinearVelocity.X < maxMoveSpeed && LinearVelocity.X > maxMoveSpeed * -1 && WeaponHolder.currentWeapon == 0)
        {
            maxMoveSpeed = 450;
            ApplyForce(new Vector2(15000 * direction, 0));
        }
        else if (direction != 0 && LinearVelocity.X < maxMoveSpeed && LinearVelocity.X > maxMoveSpeed * -1)
        {
            maxMoveSpeed = 300;
            ApplyForce(new Vector2(15000 * direction, 0));
        }
    }

    private void Aim()
    {
        float aimX;
        float aimY;
        // Check if player is using right thumbstick otherwise use left
        if (Input.GetJoyAxis(playerIndex, JoyAxis.RightX) >= 0.5 || Input.GetJoyAxis(playerIndex, JoyAxis.RightX) <= -0.5
          || Input.GetJoyAxis(playerIndex, JoyAxis.RightY) >= 0.5 || Input.GetJoyAxis(playerIndex, JoyAxis.RightY) <= -0.5)
        {
            aimX = Input.GetJoyAxis(playerIndex, JoyAxis.RightX);
            aimY = Input.GetJoyAxis(playerIndex, JoyAxis.RightY);
        }
        else
        {
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

            // change rotation of hitbox so hitbox is always facing outwards
            HitBox.RotationDegrees = Mathf.RadToDeg(GlobalPosition.AngleToPoint(newPosition));
        }
    }

    private void ChangeWeapon()
    {
        if (Input.IsJoyButtonPressed(playerIndex, JoyButton.Y) && !changingWeapon)
        {
            changingWeapon = true;
            WeaponHolder.ChangeWeapon();
        }
        else if (!Input.IsJoyButtonPressed(playerIndex, JoyButton.Y))
        {
            changingWeapon = false;
        }
    }

    private void RecieveMeleeHit(Area2D area)
    {
        Vector2 info = Vector2.Zero;
        // Cast the area to HitBox to access the GiveInfo method
        if (area is HitBox hitBox)
        {
            info = hitBox.GiveInfo();
            DamageFromMelee(info);
        }
    }

    private void RecieveRangedHit(Node2D body)
    {
        Vector2 info = Vector2.Zero;

        if (body is Arrow arrow && arrow.GiveIndexInfo() != playerIndex)
        {
            info = arrow.GiveInfo();
            DamageFromArrow(info, arrow.forceApplied);
        }
        if (body is HookHitbox hookHitBox && hookHitBox.GiveIndexInfo() != playerIndex)
        {
            info = hookHitBox.GiveInfo() - GlobalPosition;
            DamageFromHook(info);
        }
    }

    private void DamageFromMelee(Vector2 info)
    {
        // Make it so youre slidey, bounce, cant move and toss you're character in a direction based on several factors
        //  which ill write about more in depth later because I'm still adding them 
        knockedBack = true;
        PhysicsMaterialOverride.Friction = 0;
        PhysicsMaterialOverride.Bounce = 1;
        Vector2 hitDirection = new Vector2(info.X, info.Y).Normalized();
        damageTaken += 1000;
        ApplyImpulse(hitDirection * (comboCount > 0 ? comboCount + 1 * damageTaken : damageTaken));
        comboCount++;
        KnockBackDuration.WaitTime = damageTaken / 10000;
        KnockBackDuration.Start();
        GD.Print(comboCount);
        GD.Print("Player " + (playerIndex + 1) + ":" + damageTaken);
        // make it so the ground check cant ground player temporarily so that it doesnt reset combo count
        GroundCheck.Monitoring = false;
    }
    private void DamageFromArrow(Vector2 info, double forceApplied)
    {
        // Make it so youre slidey, bounce, cant move and toss you're character in a direction based on several factors
        //  which ill write about more in depth later because I'm still adding them 
        knockedBack = true;
        PhysicsMaterialOverride.Friction = 0;
        PhysicsMaterialOverride.Bounce = 1;
        Vector2 hitDirection = new Vector2(info.X, info.Y).Normalized();
        // Vector2 hitDirection;
        // if(info.X > info.Y){
        //     hitDirection = new Vector2(info.X, 0).Normalized();
        // }else {
        //     hitDirection = new Vector2(0, info.Y).Normalized();
        // }
        damageTaken += 1000;
        LinearVelocity = new Vector2(0, 0);
        ApplyImpulse(hitDirection * (comboCount > 0 ? comboCount * damageTaken : damageTaken));
        comboCount++;
        KnockBackDuration.WaitTime = damageTaken / 10000;
        KnockBackDuration.Start();
        GD.Print(comboCount);
        // make it so the ground check cant ground player temporarily so that it doesnt reset combo count
        GroundCheck.Monitoring = false;
    }

    private void DamageFromHook(Vector2 info)
    {
        // Make it so youre slidey, bounce, cant move and toss you're character in a direction based on several factors
        //  which ill write about more in depth later because I'm still adding them 
        knockedBack = true;
        PhysicsMaterialOverride.Friction = 0;
        PhysicsMaterialOverride.Bounce = 1;
        Vector2 hitDirection = new Vector2(info.X, info.Y).Normalized();
        LinearVelocity = new Vector2(LinearVelocity.X, 0);
        ApplyImpulse(hitDirection * 10000);
        comboCount += 3;
        KnockBackDuration.WaitTime = damageTaken / 10000;
        KnockBackDuration.Start();
        GD.Print(comboCount);
        // make it so the ground check cant ground player temporarily so that it doesnt reset combo count
        GroundCheck.Monitoring = false;
    }

    public void Grapple(Vector2 info)
    {
        KnockBackDuration.WaitTime = 0.01f;
        AllowMovement();
        Vector2 hitDirection = new Vector2(info.X, info.Y * 2).Normalized();
        LinearVelocity = new Vector2(0, 0);
        ApplyImpulse(hitDirection * 8000);
    }

    // States
    private void Grounded(Node body)
    {
        if (body.IsInGroup("Environment") && LinearVelocity.Y >= -10)
        {
            isGrounded = true;
            comboCount = 1;
            canJump = true;
            jumpCount = 3;
        }
    }

    private void UnGrounded(Node2D body)
    {
        if (body.IsInGroup("Environment"))
        {
            isGrounded = false;
        }
    }
    private void AllowMovement()
    {
        knockedBack = false;
        PhysicsMaterialOverride.Bounce = 0;
        PhysicsMaterialOverride.Friction = 0.6f;
        GroundCheck.Monitoring = true;
    }
}