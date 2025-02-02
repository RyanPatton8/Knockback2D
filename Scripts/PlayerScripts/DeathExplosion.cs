using Godot;
using System;

public partial class DeathExplosion : Sprite2D
{
	[Export] public AnimationPlayer AnimExplosionNode { get; private set; }
    public Player player;
	PlayerManager playerManager;
    public override void _Ready()
    {
		playerManager = PlayerManager.Instance;
        AnimExplosionNode.AnimationFinished += StopExploding;
    }
    private void StopExploding(StringName animName)
    {
		playerManager.LoseALife(player.playerIndex, player.indexOfFinalAttacker);
        CallDeferred("queue_free");
    }
}
