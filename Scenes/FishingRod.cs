using Godot;
using System;

public partial class FishingRod : Node
{
	[Export] public PackedScene hook {get; private set;}
	private Player playerNode;
    private double throwForce = 35;
    private bool charged = false;
    private bool canAttack = true;
    public bool hookOut = false;
    private double attackCoolDown = 1;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
    }
    public override void _Process(double delta)
	{
		if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerLeft) > 0.5f && canAttack)
        {
            charged = true;
        } 
        else if (charged)
        {
            Attack();
            canAttack = false;
            hookOut = true;
            charged = false;
        }
        else if (!canAttack && !hookOut)
        {
            attackCoolDown -= delta;
            if(attackCoolDown <= Mathf.Epsilon){
                canAttack = true;
                attackCoolDown = .5;
            }
        }
	}
	private void Attack()
    {
        if(canAttack){
            canAttack = false;
            Vector2 aimDirection = playerNode.HitBox.GlobalPosition - playerNode.GlobalPosition;
            Hook instance = (Hook)hook.Instantiate();
            AddChild(instance);
            instance.GlobalPosition = playerNode.HitBox.GlobalPosition;
		    instance.ApplyImpulse(aimDirection * (float)throwForce);
        }
    }
}
