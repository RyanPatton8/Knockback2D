using Godot;
using System;

public partial class Range : Node
{
	private Player playerNode;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
    }
    public override void _Process(double delta)
	{
		if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) > 0.5f)
        {
           Attack();
        }
	}
	private void Attack()
    {
        if(playerNode.canAttack){
            GD.Print("Ranged");
            playerNode.canAttack = false;
            playerNode.HitBox.Monitoring = true;
            playerNode.HitBox.Monitorable = true;
            playerNode.AttackDuration.Start();
        }
    }
}
