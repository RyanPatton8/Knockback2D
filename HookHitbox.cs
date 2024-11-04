using Godot;
using System;

public partial class HookHitbox : Area2D
{
	private Hook hookNode;
	private Player playerNode;
    private FishingRod fishingRod;
    public double forceApplied;
	
	public override void _Ready()
    {
        hookNode = GetOwner<Hook>();
		playerNode = GetNode<Player>("../../../.."); 
		fishingRod = GetNode<FishingRod>("../.."); 
		BodyEntered += CallDespawn;
    }

    private void CallDespawn(Node2D body)
    {
        if(body is Player player && player.playerIndex == playerNode.playerIndex)
        {
            return;
        }
		if(body.IsInGroup("Environment"))
        {
			playerNode.Grapple(GlobalPosition - playerNode.GlobalPosition);
        }
        fishingRod.hookOut = false;
        hookNode.QueueFree();
    }
    public Vector2 GiveInfo(){
		return playerNode.GlobalPosition;
	}
    public int GiveIndexInfo(){
        return playerNode.playerIndex;
    }
}
