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

    bool hasCollided= false;
	public override void _Ready()
    {
		BodyEntered += Explode;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (hasCollided)
        {
            SpawnExplosion();
            Freeze = true;
            hasCollided = false;
        }
    }

    private void Explode(Node body)
    {
        // if(body is Player player && player.playerIndex == playerIndex){
        //     return;
        // }
        if(body.IsInGroup("Environment")){
            return;
        }
        hasCollided = true;
    }
    private void SpawnExplosion(){
        playerNode.WeaponAudio.Stream = ExplosionAudio;
        playerNode.WeaponAudio.Play();
        Explosion instance = (Explosion)explosion.Instantiate();
        GetTree().Root.AddChild(instance);
        instance.GlobalPosition = Position;
        instance.playerIndex = playerIndex;
        CallDeferred("queue_free");
    }
    public int GiveIndexInfo(){
        return playerIndex;
    }
}
