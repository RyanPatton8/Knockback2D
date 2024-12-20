using Godot;
using System;

public partial class FishingRod : Node
{
	[Export] public PackedScene hook {get; private set;}
	private Player playerNode;
    private double throwForce = 45;
    private bool charged = false;
    private bool canAttack = true;
    public bool hookOut = false;
    private double attackCoolDown = 1;
    Hook instance;
    PlayerManager playerManager;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
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
                attackCoolDown = .6;
            }
        }
	}
	private void Attack()
    {
        if(canAttack){
            canAttack = false;
            playerNode.hookCount--;
            playerManager.playerList[playerNode.playerIndex].SetHookCount(playerNode.hookCount);
            Vector2 aimDirection = playerNode.HitBox.GlobalPosition - playerNode.GlobalPosition;
            instance = (Hook)hook.Instantiate();
            AddChild(instance);
            instance.GlobalPosition = playerNode.HitBox.GlobalPosition;
		    instance.ApplyImpulse(aimDirection * (float)throwForce);
        }
    }
}
