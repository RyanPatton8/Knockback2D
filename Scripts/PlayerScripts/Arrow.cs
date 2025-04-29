using Godot;
using System;

public partial class Arrow : RigidBody2D
{
    [Export] public CollisionShape2D collision {get; private set;}
    [Export] public PackedScene explosion {get; private set;}
    [Export] public Sprite2D arrowSprite {get; private set;}
    [Export] public AudioStream ExplosionAudio {get; private set;}

    public int playerIndex;
    public double forceApplied;
    private bool hittingPlayer;
    private bool objectHit = false;
    private double timeToLive = 0.4;
    public Player playerNode;
	public override void _Ready()
    {
		BodyEntered += Explode;
    }
    private void Explode(Node body)
    {
        if(body is Player player && player.playerIndex == playerIndex){
            return;
        }
        CallDeferred(nameof(SpawnExplosion));
        CallDeferred("queue_free");
    }
    private void SpawnExplosion(){
        playerNode.WeaponAudio.Stream = ExplosionAudio;
        playerNode.WeaponAudio.Play();
        Explosion instance = (Explosion)explosion.Instantiate();
        playerNode.AddChild(instance);
        instance.GlobalPosition = GlobalPosition;
        instance.playerIndex = playerIndex;
    }
    public int GiveIndexInfo(){
        return playerIndex;
    }
}
