using Godot;
using System;

public partial class Range : Node
{
    [Export] public PackedScene arrow {get; private set;}
	private Player playerNode;
    private double throwForce = 40;
    private bool canAttack = true;
    private bool isTriggerDown = false;
    private double attackCoolDown = .1;
    private double maxAttackCoolDown = .1;
    [Export] public AudioStream GunShotAudio {get; private set;}
    PlayerManager playerManager;
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
        playerManager = PlayerManager.Instance;
    }
    public override void _Process(double delta)
	{
        if(!playerNode.knockedBack || playerNode.rocketJumping){
            if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) > 0.7f && canAttack && playerNode.arrowCount > 0 && !isTriggerDown)
            {
                Attack();
                canAttack = false;
                isTriggerDown = true;
            }
            else if(isTriggerDown && Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) < 0.7f){
                isTriggerDown = false;
            } 
            if (!canAttack)
            {
                attackCoolDown -= delta;
                if(attackCoolDown <= Mathf.Epsilon){
                    canAttack = true;
                    attackCoolDown = maxAttackCoolDown;
                }
            }
        }
	}
    private void Attack()
    {
        if(canAttack){
            canAttack = false;
            playerNode.arrowCount--;
            playerManager.playerList[playerNode.playerIndex].SetArrowCount(playerNode.arrowCount);
            playerNode.bulletAndHookCountUi.UpdateArrowCount();
            Vector2 aimDirection = playerNode.HitBox.GlobalPosition - playerNode.GlobalPosition;
            playerNode.WeaponAudio.Stream = GunShotAudio;
            playerNode.WeaponAudio.Play();
            Bullet instance = (Bullet)arrow.Instantiate();
            GetTree().Root.AddChild(instance);
            instance.playerIndex = playerNode.playerIndex;
            instance.playerNode = playerNode;
            if(!playerNode.isWeaponInGround){
                instance.forceApplied = throwForce;
                instance.GlobalPosition = playerNode.HitBox.GlobalPosition;
            }
            else{
                instance.GlobalPosition = playerNode.HitBox.GlobalPosition + (playerNode.HitBox.GlobalPosition - playerNode.GlobalPosition).Normalized() * -20;
            }
            instance.arrowSprite.Modulate = playerNode.playerColor;
		    instance.ApplyImpulse(aimDirection * (float)throwForce + playerNode.LinearVelocity);
        }
    }
}
