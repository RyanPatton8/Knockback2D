using Godot;
using System;
public partial class Melee : Node
{
    private Player playerNode;
    public bool attacking = false;
    double maxAttackDuration = .04;
    double currentAttackDuration = .04; 
    double attackStrength = 1;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
    }
    public override void _Process(double delta)
    {
        if(attacking)
        {
            currentAttackDuration -= delta;
            playerNode.HitBox.Monitoring = false;
            playerNode.HitBox.Monitorable = false;
            if(currentAttackDuration <= Mathf.Epsilon && Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) < 0.5f){
                attacking = false;
            }
        }
        if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) > 0.5f && !attacking)
        {
            attacking = true;
            playerNode.HitBox.Monitoring = true;
            playerNode.HitBox.Monitorable = true;
        }
    }
    
}
