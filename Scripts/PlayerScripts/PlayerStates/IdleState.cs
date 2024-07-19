using Godot;
using System.Collections.Generic;

public partial class IdleState : GenericPlayerState
{
    public override void UpdateState(double dt)
    {
        base.UpdateState(dt);
		if (playerNode.direction < -0.5 || playerNode.direction > 0.5) {
			playerNode.playerStateMachine.ChangeState("MoveState", playerNode.playerIndex, null);
		}
		else{
			//implement Idle logic
		}
    }
}

