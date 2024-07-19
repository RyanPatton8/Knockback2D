using Godot;
using System;
using System.Collections.Generic;

public partial class KnockedState : GenericPlayerState
{
    public override void _Ready()
    {
        base._Ready();
		playerNode.KnockBackDuration.Timeout += AllowMovement;
    }

	public void KnockedBack(Area2D area)
	{
		Vector2 info = Vector2.Zero;
         // Cast the area to HitBox to access the GiveInfo method
        if (area is HitBox hitBox)
        {
            info = hitBox.GiveInfo();
        }

        playerNode.PhysicsMaterialOverride.Friction = 0;
        playerNode.KnockBackDuration.WaitTime = 0.5;
        playerNode.ApplyImpulse(new Vector2(info.X * -1, info.Y * -2) * -150);
        GD.Print("Ouch");
        playerNode.KnockBackDuration.Start();
	}

	private void AllowMovement()
	{
		playerNode.playerStateMachine.ChangeState("MoveState", playerNode.playerIndex, null);
	}
}
