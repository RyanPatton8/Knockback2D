using Godot;
using System;
using System.Collections.Generic;

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
    [Export] public Sprite2D PlayerSprite { get; private set; }
    [Export] public AnimationPlayer Anim { get; private set; }
    [Export] public AudioStreamPlayer2D WeaponAudio { get; private set; }
    [Export] public AudioStreamPlayer2D HookAudio { get; private set; }
    [Export] public AudioStreamPlayer2D PlayerAudio { get; private set; }
    [Export] public float normalFriction = 0.7f;
    [Export] public BulletAndHookCountUi bulletAndHookCountUi;
    
    // How far from player should hitbox rotate
    private float offsetAmount = 27;
    // Movement Variables
    private bool isGrounded = false;
    public bool canJump = true;
    public bool justJumped = false;
    private double jumpSlashWindow = .2;
    private int jumpCount = 1;
    private int maxJumpCount = 1;
    // Combat Variables 
    private float comboCount = 1;
    public bool knockedBack = false;
    public float damageTaken = 1500;
    public bool changingWeapon = false;
    // Arrow Regen and Ammo
    public int arrowCount = 6;
    public int maxArrows = 6;
    public int hookCount = 4;
    public int maxHooks = 4;
    private double arrowRegenTime = 4;
    private double hookRegenTime = 4;
    private double maxRegenTime = 4;
    private bool regeneratingArrow = false;
    private bool regeneratingHook = false;
    public bool rocketJumping = false;
    //Health Regen
    private double healthRegenTime = 5;
    private double maxHealthRegenTime = 5;
    private bool healthRegenerating = false;
    public bool isAttacking = false;
    public bool isClashing = false;
    public bool isGrappling = false;
    public bool isWeaponInGround = false;
    private double grappleTime = .2;
    public double endLagTime = 0.5f;
    private double maxEndLagTime = 0.5f;
    private double clashDuration = .35;
    private int attacker;
    public Color playerColor;
    public int indexOfFinalAttacker = -1;
    public double slashIndexDuration = 1;
    private List<string> attacksTaken = new List<string>();
    [Export] public AudioStream Running { get; private set; }
    [Export] public Label ComboLabel { get; private set; }
    // Reference to player manager singleton
    PlayerManager playerManager;
    GameManager gameManager;
    public override void _Ready()
    {
        GroundCheck.BodyEntered += Grounded;
        GroundCheck.BodyExited += UnGrounded;
        HurtBox.AreaEntered += RecieveHit;
        BodyEntered += PlayerBump;
        // HurtBox.BodyEntered += RecieveRangedHit;
        KnockBackDuration.Timeout += AllowMovement;
        playerManager = PlayerManager.Instance;
        gameManager = GameManager.Instance;
        PlayerSprite.Modulate = playerColor;
        HitBox.GlobalPosition = GlobalPosition + new Vector2(1, 0) * offsetAmount;
        HitBox.BodyEntered += GroundClashEntered;
        HitBox.BodyExited += GroundClashExit;
        Anim.Play("Idle");
        PlayerAudio.Stream = Running;
        bulletAndHookCountUi.playerIndex = playerIndex;
        bulletAndHookCountUi.BulletSprite.Modulate = playerColor;
        bulletAndHookCountUi.UpdateArrowCount();
        bulletAndHookCountUi.UpdateHookCount();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!isAttacking)
        {   //Ensure the player isn't attacking and currently expiriencing the end lag from that attack
            if (!knockedBack)
            {// Only allow them to move if they also aren't stunned from being hit
                Move(delta);
            }
            Aim();
        }
        else
        {
            AttackEndLag(delta);
        }
        // Mitigate weird glitch where jumping and grappling would cancel each other
        if (!isGrappling)
        {
            HandleJump(delta);
        }
        else
        {
            grappleTime -= delta;
            if (grappleTime <= Mathf.Epsilon)
            {
                isGrappling = false;
                grappleTime = .2;
            }
        }
        Regen(delta);
        ChangeWeapon();

        if (comboCount > 1)
        {
            ComboLabel.Text = $"x{comboCount - 1}";
        }
        else
        {
            ComboLabel.Text = "";
        }
    }
    private void Regen(double delta)
    {
        //Always allow changing weapon and Health regen
        if (damageTaken > 1500 && healthRegenerating)
        {
            RegenerateHealth(delta);
        }
        else if (damageTaken > 1500 && !healthRegenerating)
        {
            healthRegenTime = maxHealthRegenTime;
            RegenerateHealth(delta);
        }
        //Always regenerate arrows if less than max arrows but reset max regen time after every arrow generated
        if (arrowCount <= 0 && regeneratingArrow)
        {
            RegenerateArrows(delta);
        }
        else if (arrowCount < maxArrows && !regeneratingArrow)
        {
            arrowRegenTime = maxRegenTime;
            RegenerateArrows(delta);
        }
        //Same logic for hooks
        if (hookCount < maxHooks && regeneratingHook)
        {
            RegenerateHooks(delta);
        }
        else if (hookCount < maxHooks && !regeneratingHook)
        {
            hookRegenTime = maxRegenTime;
            RegenerateHooks(delta);
        }
        if (attacksTaken.Count > 0)
        {
            slashIndexDuration -= delta;
            if (slashIndexDuration < Mathf.Epsilon)
            {
                attacksTaken.RemoveAt(0);
                slashIndexDuration = 1;
            }
        }
        //And for justJumped
        if (justJumped && jumpSlashWindow > Mathf.Epsilon)
        {
            jumpSlashWindow -= delta;
        }
        else if (justJumped)
        {
            justJumped = false;
            jumpSlashWindow = .2;
        }
        if (isClashing)
        {
            StopClashing(delta);
        }
    }
    private void Move(double delta)
    {
        // Handle Horizontal Movement
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
        if (direction > 0.3 && isGrounded)
        {
            PlayerSprite.FlipH = false;

            if (Anim.CurrentAnimation != "Run" || !Anim.IsPlaying())
                Anim.Play("Run");
            if (!PlayerAudio.IsPlaying() && isGrounded)
                PlayerAudio.Play();
        }
        else if (direction < -0.3 && isGrounded)
        {
            PlayerSprite.FlipH = true;
            if (Anim.CurrentAnimation != "Run" || !Anim.IsPlaying())
                Anim.Play("Run");
            if (!PlayerAudio.IsPlaying() && isGrounded)
                PlayerAudio.Play();
        }
        else if (isGrounded)
        {
            Anim.Play("Idle");
        }
    }
    private void HandleJump(double delta)
    {
        // Handle Jumps
        if (!Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && isGrounded)
        {
            PhysicsMaterialOverride.Friction = normalFriction;
        }
        else if (Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && jumpCount > 0 && canJump)
        {
            LinearVelocity = new Vector2(LinearVelocity.X, 0);
            ApplyImpulse(new Vector2(0, -4500));
            justJumped = true;
            jumpCount--;
            canJump = false;
            PhysicsMaterialOverride.Friction = 0f;

        }
        else if (!Input.IsJoyButtonPressed(playerIndex, JoyButton.A) && jumpCount > 0 && !canJump)
        {
            canJump = true;
            PhysicsMaterialOverride.Friction = normalFriction;
        }
        else if (isGrounded && !canJump)
        {
            canJump = true;
        }
    }
    private void Aim()
    {
        float aimX;
        float aimY;
        // Check if player is using right thumbstick otherwise use left
        bool movingRThumbX = Input.GetJoyAxis(playerIndex, JoyAxis.RightX) >= 0.5 || Input.GetJoyAxis(playerIndex, JoyAxis.RightX) <= -0.5;
        bool movingRThumbY = Input.GetJoyAxis(playerIndex, JoyAxis.RightY) >= 0.5 || Input.GetJoyAxis(playerIndex, JoyAxis.RightY) <= -0.5;
        bool movingRThumb = movingRThumbX || movingRThumbY;
        if (movingRThumb)
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
    private void RegenerateArrows(double delta)
    {
        arrowRegenTime -= delta;
        regeneratingArrow = true;
        if (arrowRegenTime <= Mathf.Epsilon)
        {
            arrowCount += 6;
            playerManager.playerList[playerIndex].SetArrowCount(arrowCount);
            bulletAndHookCountUi.UpdateArrowCount();
            regeneratingArrow = false;
            return;
        }
    }
    private void RegenerateHooks(double delta)
    {
        hookRegenTime -= delta;
        regeneratingHook = true;
        if (hookRegenTime <= Mathf.Epsilon)
        {
            hookCount++;
            playerManager.playerList[playerIndex].SetHookCount(hookCount);
            regeneratingHook = false;
            return;
        }
    }
    private void RegenerateHealth(double delta)
    {
        healthRegenTime -= delta;
        healthRegenerating = true;
        if (healthRegenTime <= Mathf.Epsilon)
        {
            damageTaken -= 1000;
            healthRegenerating = false;
            if (damageTaken < 1500)
            {
                damageTaken = 1500;
            }
            playerManager.playerList[playerIndex].SetDamageTaken(damageTaken);
            return;
        }
    }
    public void AttackEndLag(double delta)
    {
        endLagTime -= delta;
        if (endLagTime <= Mathf.Epsilon)
        {
            isAttacking = false;
            endLagTime = maxEndLagTime;
        }
    }
    public void StopClashing(double delta)
    {
        clashDuration -= delta;
        if (clashDuration <= Mathf.Epsilon)
        {
            isClashing = false;
            clashDuration = .35;
        }
    }
    /*
        When you bump into another player reset velocity to prevent killing yourself off bouncing your opponent

        Before this it was kind of a Poe Belly Bump Kungfu Panda Scenario  
    */
    private void PlayerBump(Node body)
    {
        if (body is Player && !knockedBack)
        {
            LinearVelocity = new Vector2(0, 0);
        }
    }
    // Handle all kinds of attacks and pass info to methods to do stuff
    private void RecieveHit(Area2D area)
    {
        Vector2 info;
        int attackOwner;
        string slashUuid;
        HitBox hb;

        if (area is Slash slash)
        {
            (info, attackOwner, slashUuid, hb) = slash.GiveInfo();
            if (attackOwner != playerIndex && !attacksTaken.Contains(slashUuid))
            {
                attacksTaken.Add(slashUuid);
                hb.Hit();
                DamageFromMelee(info);
                indexOfFinalAttacker = attackOwner;
            }
        }
        else if (area is HookHitbox hookHitBox && hookHitBox.GiveIndexInfo() != playerIndex)
        {
            info = hookHitBox.GiveInfo() - GlobalPosition;
            indexOfFinalAttacker = hookHitBox.GiveIndexInfo();
            DamageFromHook(info);
        }
        else if (area is Explosion explosion)
        {
            Vector2 explosionPos = explosion.GiveInfo();
            (int explosionOwner, string explosionId) = explosion.GiveIndexInfo();
            if (attacksTaken.Contains(explosionId))
                return;
            attacksTaken.Add(explosionId);
            if (explosionOwner != playerIndex)
            {
                indexOfFinalAttacker = explosionOwner;
            }
            info = GlobalPosition - explosionPos;
            DamageFromExplosion(info, explosionOwner);
        }
    }
    public void Clashed(Vector2 redirect, int otherSlashOwner, bool fromSlash)
    {
        int multiplier = fromSlash ? 6000 : 150;
        Vector2 clashDir = redirect.Normalized() * multiplier;
        ApplyImpulse(clashDir);
    }
    /*
        When recieving damage lock the player movement and make them able to slide and bounce.

        Afterwards, take the direction that the player should be launched and normalize it, reset the player velocity apply appropriate damage and launch the player

        The impulse strength checks if the combo count is more than 1 and multiplies damage taken which then multiplies the hitdirection variable
        the damage to the player is unchanged but the force applied is

        then increase combo count, start a timer for when the player is no longer bouncey and stunned and make it so the players ground check is not monitoring

        Melee increases Combo Count after ranged does it before
    */
    private void DamageFromMelee(Vector2 info)
    {
        knockedBack = true;
        PhysicsMaterialOverride.Friction = 0;
        PhysicsMaterialOverride.Bounce = 1;
        Vector2 hitDirection = new Vector2(info.X, info.Y).Normalized();
        LinearVelocity = new Vector2(0, 0);
        damageTaken += 1250;
        ApplyImpulse(hitDirection * (comboCount > 1 ? comboCount * damageTaken : damageTaken));
        comboCount++;
        playerManager.playerList[playerIndex].SetComboCount(comboCount);
        playerManager.playerList[playerIndex].SetDamageTaken(damageTaken);
        KnockBackDuration.WaitTime = damageTaken / 10000;
        KnockBackDuration.Start();
        Anim.Play("Stunned");
        GroundCheck.Monitoring = false;
    }

    private void DamageFromExplosion(Vector2 info, int explosionIndex)
    {
        Vector2 hitDirection = new Vector2(info.X, info.Y).Normalized();
        if (playerIndex != explosionIndex)
        {
            knockedBack = true;
            PhysicsMaterialOverride.Friction = 0;
            PhysicsMaterialOverride.Bounce = 1;
            LinearVelocity = new Vector2(0, 0);
            damageTaken += 2000f;
            comboCount++;
            GD.Print($"Player {playerIndex} Hit Combo count: {comboCount}");
            playerManager.playerList[playerIndex].SetComboCount(comboCount);
            playerManager.playerList[playerIndex].SetDamageTaken(damageTaken);
            float knockBackMultiplier = comboCount > 0 ? comboCount * damageTaken : damageTaken;
            ApplyImpulse(hitDirection * knockBackMultiplier);
            // playerManager.playerList[playerIndex].SetComboCount(comboCount);
            KnockBackDuration.WaitTime = damageTaken / 10000;
            KnockBackDuration.Start();
            Anim.Play("Stunned");
            GroundCheck.Monitoring = false;
        }
        else
        {
            float knockBackMultiplier = 10000;
            ApplyImpulse(hitDirection * knockBackMultiplier * comboCount);
            comboCount++;
        }
    }

    // functionally about the same but not applying damage having a fixed impulse strength
    private void DamageFromHook(Vector2 info)
    {
        knockedBack = true;
        PhysicsMaterialOverride.Friction = 0.3f;
        PhysicsMaterialOverride.Bounce = 1;
        Vector2 hitDirection = new Vector2(info.X, info.Y).Normalized();
        LinearVelocity = new Vector2(0, 0);
        ApplyImpulse(new Vector2(hitDirection.X * 7000, hitDirection.Y * 10000));
        comboCount += 2;
        playerManager.playerList[playerIndex].SetComboCount(comboCount);
        KnockBackDuration.WaitTime = .5;
        KnockBackDuration.Start();
        Anim.Play("Stunned");
        GroundCheck.Monitoring = false;
    }
    // Called to remove stun from knockback
    public void AllowMovement()
    {
        rocketJumping = false;
        knockedBack = false;
        PhysicsMaterialOverride.Bounce = 0;
        PhysicsMaterialOverride.Friction = normalFriction;
        GroundCheck.Monitoring = true;
    }
    // turns off player stun recieved from hit and applies impulse to self in desired direction
    public void Grapple(Vector2 info)
    {
        isGrappling = true;
        KnockBackDuration.WaitTime = 0.01f;
        if (jumpCount < maxJumpCount)
            jumpCount = maxJumpCount;
        AllowMovement();
        Vector2 hitDirection = new Vector2(info.X, info.Y * 2).Normalized();
        LinearVelocity = new Vector2(0, 0);
        ApplyImpulse(hitDirection * 8000);
    }

    // States
    private void Grounded(Node body)
    {
        //checking the y linear velocity to not allow passing through one way platform as grounding
        if (body.IsInGroup("Environment") && LinearVelocity.Y >= -10)
        {
            indexOfFinalAttacker = -1;
            isGrounded = true;
            comboCount = 1;
            playerManager.playerList[playerIndex].SetComboCount(comboCount);
            canJump = true;
            jumpCount = maxJumpCount;
            attacker = playerIndex;
            Anim.Play("Idle");
        }
    }
    private void UnGrounded(Node2D body)
    {
        if (body.IsInGroup("Environment"))
        {
            isGrounded = false;
            if (!knockedBack)
            {
                Anim.Play("Jump");
            }
        }
    }

    private void GroundClashEntered(Node body)
    {
        if (body is TileMapLayer)
        {
            isWeaponInGround = true;
        }
    }
    private void GroundClashExit(Node body)
    {
        if (body is TileMapLayer)
        {
            isWeaponInGround = false;
        }
    }
}