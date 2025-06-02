using Godot;
using System;

public partial class Explosion : Area2D
{
    [Export] public AnimationPlayer AnimExplosionNode { get; private set; }
    public int playerIndex;

    public override void _Ready()
    {
        AnimExplosionNode.AnimationFinished += StopExploding;
        BodyEntered += MakeExplode;
    }
    private void StopExploding(StringName animName)
    {
        CallDeferred("queue_free");
    }
    public void MakeExplode(Node body){
        if (body is Arrow arrow && arrow.grounded)
        {
            arrow.hasCollided = true;
        }
    }
    public Vector2 GiveInfo(){
		return GlobalPosition;
	}
    public int GiveIndexInfo(){
        return playerIndex;
    }
}
