using Godot;
using System.Collections.Generic;

public partial class PlayerStateMachine : SimpleStateMachine
{
	protected Player playerNode;
    public override void _Ready()
    {
        base._Ready();
		playerNode = GetOwner<Player>();
        ChangeState("IdleState", null); // Start with the IdleState
    }

	public void ChangeState(string stateName, int playerIndex, Dictionary<string, object> message = null)
	{
		if (playerIndex == playerNode.playerIndex){
			base.ChangeState(stateName, message);
		}
	}
}

