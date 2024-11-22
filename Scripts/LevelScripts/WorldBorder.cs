using Godot;
using System;

public partial class WorldBorder : Area2D
{
	PlayerManager playerManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerManager = PlayerManager.Instance;
		BodyExited += Destroy;
	}

    private void Destroy(Node2D body)
    {
        if(body is Player player){
			playerManager.LoseALife(player.playerIndex);
		}
		else if(body is Hook hook){
			hook.GetParent<FishingRod>().hookOut = false;
		}
		body.CallDeferred("queue_free");
    }

}
