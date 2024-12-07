using Godot;
using System;

public partial class Explosion : Area2D
{
    [Export] public AnimationPlayer AnimExplosionNode { get; private set; }
    public int playerIndex;
    public override void _Ready()
    {
        CallDeferred(nameof(Explode));
        AnimExplosionNode.AnimationFinished += StopExploding;
    }

    private void StopExploding(StringName animName)
    {
        CallDeferred("queue_free");
    }


    private void Explode(){
        AnimExplosionNode.Play("GrowingExplosion");
    }
    public Vector2 GiveInfo(){
		return GlobalPosition;
	}
    public int GiveIndexInfo(){
        return playerIndex;
    }
}
