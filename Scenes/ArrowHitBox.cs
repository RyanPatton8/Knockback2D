using Godot;
using System;

public partial class ArrowHitBox : Area2D
{
	private Arrow arrowNode;
    public double forceApplied;
	public override void _Ready()
    {
        arrowNode = GetOwner<Arrow>();
        forceApplied = arrowNode.forceApplied;
		BodyEntered += CallDespawn;
    }

    private void CallDespawn(Node2D body)
    {
        if(body is Player player && player.playerIndex == arrowNode.playerIndex)
        {
            return;
        }
        arrowNode.QueueFree();
    }

    public Vector2 GiveInfo(){
		return GlobalPosition;
	}
    public int GiveIndexInfo(){
        return arrowNode.playerIndex;
    }
}
