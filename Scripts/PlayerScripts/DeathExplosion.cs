using Godot;
using System;

public partial class DeathExplosion : Sprite2D
{
	[Export] public AnimationPlayer AnimExplosionNode { get; private set; }
    [Export] public AudioStreamPlayer2D ExplosionAudioPlayer {get; private set;}
    public Player player;
	PlayerManager playerManager;
    GameManager gameManager;
    public override void _Ready()
    {
        playerManager = PlayerManager.Instance;
        gameManager = GameManager.Instance;
        ExplosionAudioPlayer.Finished += StopExploding;
    }
    private void StopExploding()
    {
        playerManager.LoseALife(player.playerIndex, player.indexOfFinalAttacker);
        CallDeferred("queue_free");
    }
}
