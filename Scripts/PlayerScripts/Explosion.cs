using Godot;
using System;

public partial class Explosion : Area2D
{
    [Export] public AnimationPlayer AnimExplosionNode { get; private set; }
    public int playerIndex;
    private string uuid = Guid.NewGuid().ToString();

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
        if (body is Bullet bullet && bullet.grounded && (bullet.LinearVelocity.X < 8 || bullet.LinearVelocity.X > -8))
        {
            bullet.hasCollided = true;
        }
    }
    public Vector2 GiveInfo(){
		return GlobalPosition;
	}
    public (int, string) GiveIndexInfo(){
        return (playerIndex, uuid);
    }
}
