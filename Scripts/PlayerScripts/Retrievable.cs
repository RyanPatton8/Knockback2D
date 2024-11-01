using Godot;
using System;

public partial class Retrievable : Area2D
{
	// Called when the node enters the scene tree for the first time.
	private Arrow arrow;
	public override void _Ready()
	{
		arrow = GetParent<Arrow>();
		BodyEntered += RetrieveArrow;
	}

    private void RetrieveArrow(Node2D body)
    {
		//Check if its a player and then check the players ranged weapon variables to see if they already have the max amount of arrows
        if(body is Player player && player.WeaponHolder.weapons[1] is Range range && range.arrowCount < range.maxArrows){
			range.arrowCount++;
			arrow.QueueFree();
		}
    }
}
