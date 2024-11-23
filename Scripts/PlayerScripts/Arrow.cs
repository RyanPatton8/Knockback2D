using Godot;
using System;

public partial class Arrow : RigidBody2D
{
    public int playerIndex;
    public double forceApplied;
    private bool hittingPlayer;
	public override void _Ready()
    {
		BodyEntered += CallDespawn;
    }
    private void CallDespawn(Node body)
    {
        if(body is Player player && player.playerIndex == playerIndex)
        {
            return;
        }
        else if(body is Player)
        {
            hittingPlayer = true;
        }
        
        QueueFree();
    }
    
    public Vector2 GiveInfo(){
		return LinearVelocity;
	}
    public int GiveIndexInfo(){
        return playerIndex;
    }
}
