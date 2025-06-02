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
			// Set the explosion's position to the player's current position
			instance.GlobalPosition = player.GlobalPosition;
			instance.player = player;
			// Retrieve the player's velocity
			Vector2 playerVelocity = player.LinearVelocity;
			// Calculate the direction opposite to the player's movement
			Vector2 reverseDirection = playerVelocity.Normalized();
			// Apply an offset to position the explosion slightly behind the player
			float offset = -250f; // Adjust this value as needed
			instance.GlobalPosition += reverseDirection * offset;
			// Set the rotation of the explosion to align with the reverse direction
			instance.Rotation = reverseDirection.Angle() - Mathf.DegToRad(90);
		}
		else if(body is Hook hook){
			hook.GetParent<FishingRod>().hookOut = false;
		}
		body.CallDeferred("queue_free");
    }

}
