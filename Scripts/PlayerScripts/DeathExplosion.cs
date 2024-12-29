using Godot;
using System;

public partial class DeathExplosion : Sprite2D
{
	[Export] public AnimationPlayer AnimExplosionNode { get; private set; }
    public int playerIndex;
	PlayerManager playerManager;
    public override void _Ready()
    {
		playerManager = PlayerManager.Instance;
        AnimExplosionNode.AnimationFinished += StopExploding;
    }
    private void StopExploding(StringName animName)
    {
		playerManager.LoseALife(playerIndex);
        CallDeferred("queue_free");
    }
}
