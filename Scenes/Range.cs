using Godot;
using System;

public partial class Range : Node
{
    [Export] public PackedScene arrow {get; private set;}
	private Player playerNode;
    private double startingThrowForce = 30;
    private double throwForce = 30;
    private double maxThrowForce = 60;
    private int chargeSpeed = 40;
    private bool charged = false;
    private bool canAttack = true;
    private double attackCoolDown = .5;

    public int arrowCount = 5;
    public int maxArrows = 5;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
    }
    public override void _Process(double delta)
	{
		if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) > 0.5f && canAttack && arrowCount > 0)
        {
            charged = true;
            throwForce += delta * chargeSpeed;
            if (throwForce > maxThrowForce) {throwForce = maxThrowForce;}
        } 
        else if (charged)
        {
            Attack();
            canAttack = false;
            charged = false;
            throwForce = startingThrowForce;
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
            arrowCount--;
            Vector2 aimDirection = playerNode.HitBox.GlobalPosition - playerNode.GlobalPosition;
            Arrow instance = (Arrow)arrow.Instantiate();
            AddChild(instance);
            instance.GlobalPosition = playerNode.HitBox.GlobalPosition;
            instance.playerIndex = playerNode.playerIndex;
            instance.forceApplied = throwForce;
		    instance.ApplyImpulse(aimDirection * (float)throwForce);
        }
    }
}
