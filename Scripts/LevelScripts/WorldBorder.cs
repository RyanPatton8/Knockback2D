using Godot;

public partial class WorldBorder : Area2D
{
	PlayerManager playerManager;
	PackedScene Explosion = (PackedScene) ResourceLoader.Load("res://Scenes/Player/death_explosion.tscn");
	private float offset = 300;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerManager = PlayerManager.Instance;
		BodyExited += Destroy;
	}

    private void Destroy(Node2D body)
    {
        if(body is Player player){
			// Instantiate the explosion
			DeathExplosion instance = (DeathExplosion)Explosion.Instantiate();
			AddChild(instance);
			instance.GlobalPosition = player.GlobalPosition + (GlobalPosition - player.GlobalPosition).Normalized() * offset;
			instance.RotationDegrees = Mathf.RadToDeg(GlobalPosition.AngleToPoint(player.GlobalPosition)) - 90;
		}
		else if(body is Hook hook){
			hook.GetParent<FishingRod>().hookOut = false;
		}
		body.CallDeferred("queue_free");
    }

}
