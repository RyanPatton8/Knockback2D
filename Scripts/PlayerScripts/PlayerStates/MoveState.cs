using Godot;
using System.Collections.Generic;

public partial class MoveState : GenericPlayerState
{
    public override void UpdateState(double dt)
    {
        base.UpdateState(dt);
		if (playerNode.direction > -0.5 && playerNode.direction < 0.5)
        {
            playerNode.playerStateMachine.ChangeState("IdleState", playerNode.playerIndex, null);
        }
		else{
        	Move();
		}
    }
	private void Move()
	{
		if (playerNode.direction == -1 && playerNode.LinearVelocity.X > 1 || playerNode.direction == 1 && playerNode.LinearVelocity.X < -1)
		{
			playerNode.LinearVelocity = new Vector2(0, playerNode.LinearVelocity.Y);
		}

		if (playerNode.direction != 0 && playerNode.LinearVelocity.X < playerNode.maxMoveSpeed && playerNode.LinearVelocity.X > playerNode.maxMoveSpeed * -1)
		{
			playerNode.ApplyForce(new Vector2(15000 * playerNode.direction, 0));
		} 
	}
}
