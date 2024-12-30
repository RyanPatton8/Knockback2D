using Godot;
using System;

public partial class FishingRod : Node
{
    //Access hook scene for instantiation and create a global reference for it to keep track of
	[Export] public PackedScene hook {get; private set;}
    public Hook instance;
    //Access to player to check variables within it
	private Player playerNode;
    private double throwForce = 45;
    private bool canAttack = true;
    public bool hookOut = false;
    private double attackCoolDown = .6;
    private double maxAttackCoolDown = .6;
    PlayerManager playerManager;
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
        playerManager = PlayerManager.Instance;
    }
    public override void _Process(double delta)
	{
		if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerLeft) > 0.7f && canAttack && !hookOut && playerNode.hookCount > 0)
        {
            Attack();
            canAttack = false;
            hookOut = true;
        } 
        else if (!canAttack)
        {
            attackCoolDown -= delta;
            if(attackCoolDown <= Mathf.Epsilon){
                canAttack = true;
                attackCoolDown = maxAttackCoolDown;
            }
        }
	}
	private void Attack()
    {
        //Alter physical hook count and hook count UI in playermanager
        playerNode.hookCount--;
        playerManager.playerList[playerNode.playerIndex].SetHookCount(playerNode.hookCount);
        //Set the aim direction based on current aim direction relative to player
        Vector2 aimDirection = playerNode.HitBox.GlobalPosition - playerNode.GlobalPosition;
        //Instantiate the hook at placement of hitbox with the set throwforce in the aim direction
        instance = (Hook)hook.Instantiate();
        AddChild(instance);
        instance.GlobalPosition = playerNode.HitBox.GlobalPosition;
        instance.ApplyImpulse(aimDirection * (float)throwForce);
    }
}
