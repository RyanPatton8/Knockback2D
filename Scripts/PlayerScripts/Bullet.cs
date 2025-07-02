using Godot;
using System;

public partial class Bullet : RigidBody2D
{
    [Export] public CollisionShape2D collision {get; private set;}
    [Export] public PackedScene explosion {get; private set;}
    [Export] public Sprite2D arrowSprite {get; private set;}
    [Export] public AudioStream ExplosionAudio {get; private set;}

    public int playerIndex;
    public double forceApplied;
    private bool hittingPlayer;
    public bool grounded;
    private bool objectHit = false;
    private double timeToLive = 0.4;
    public Player playerNode;
    GameManager gameManager;

    public bool hasCollided = false;
    public override void _Ready()
    {
        BodyEntered += Explode;
        gameManager = GameManager.Instance;
        var cb = new Callable(this, nameof(Delete));
        gameManager.Connect(GameManager.SignalName.PlayerDeath, cb);
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

    public void Explode(Node body)
    {
        // if(body is Player player && player.playerIndex == playerIndex){
        //     return;
        // }
        if (body is Bullet)
        {
            return;
        }
        else if (body.IsInGroup("Environment"))
        {
            grounded = true;
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
    private void Delete(int playerIndex, int killerIndex)
    {
        if (playerIndex == this.playerIndex)
        {
            CallDeferred("queue_free");
        }
    }
    public int GiveIndexInfo()
    {
        return playerIndex;
    }
}
