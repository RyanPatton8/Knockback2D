using Godot;
using System;

public partial class Range : Node
{
    [Export] public PackedScene arrow {get; private set;}
	private Player playerNode;
    private double startingThrowForce = 30;
    private double throwForce = 35;
    private double maxThrowForce = 60;
    private int chargeSpeed = 40;
    private bool charged = false;
    private bool canAttack = true;
    private double attackCoolDown = .5;
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
    }
    public override void _Process(double delta)
	{
        if(!playerNode.knockedBack){
		    AttackInput(delta);
        }
	}
    private void AttackInput(double delta)
    {
        if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) < 0.7f && canAttack && playerNode.arrowCount > 0)
        {
            charged = true;
            if (throwForce > maxThrowForce) {throwForce = maxThrowForce;}
        } 
        else if (charged)
        {
            Attack();
            canAttack = false;
            charged = false;
        }
        else if (!canAttack)
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
            playerNode.arrowCount--;
            Vector2 aimDirection = playerNode.HitBox.GlobalPosition - playerNode.GlobalPosition;
            Arrow instance = (Arrow)arrow.Instantiate();
            AddChild(instance);
            instance.GlobalPosition = playerNode.HitBox.GlobalPosition;
            instance.playerIndex = playerNode.playerIndex;
            instance.forceApplied = throwForce;
            instance.arrowSprite.Modulate = playerNode.playerColor;
		    instance.ApplyImpulse(aimDirection * (float)throwForce);
        }
    }
}
