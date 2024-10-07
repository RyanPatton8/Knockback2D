using Godot;
using System;

public partial class Range : Node
{
    [Export] public PackedScene arrow {get; private set;}
	private Player playerNode;
    private double throwForce = 20;
    private bool charged = false;
    private bool canAttack = true;
    private double attackCoolDown = .8;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
    }
    public override void _Process(double delta)
	{
		if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) > 0.5f && canAttack)
        {
            charged = true;
            throwForce += delta * 30;
            if (throwForce > 25) {throwForce = 50;}
        } 
        else if (charged)
        {
            Attack();
            charged = false;
            throwForce = 20;
        }
        else if (!canAttack)
        {
            attackCoolDown -= delta;
            if(attackCoolDown <= Mathf.Epsilon){
                canAttack = true;
                attackCoolDown = .8;
            }
        }
	}
	private void Attack()
    {
        if(playerNode.canAttack){
            canAttack = false;
            Vector2 aimDirection = playerNode.HitBox.GlobalPosition - playerNode.GlobalPosition;
            Arrow instance = (Arrow)arrow.Instantiate();
            GetTree().Root.AddChild(instance);
            instance.GlobalPosition = playerNode.HitBox.GlobalPosition;
            instance.playerIndex = playerNode.playerIndex;
            instance.forceApplied = throwForce;
		    instance.ApplyImpulse(aimDirection * (float)throwForce);
        }
    }
}
