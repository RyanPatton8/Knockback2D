using Godot;
using System;

public partial class LevelChoice : Button
{
	[Export] public PackedScene Level {get; private set;}
	PlayerManager playerManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(GetParent().GetChild(0) == this)
			GrabFocus();
		playerManager = PlayerManager.Instance;
		ButtonDown += ChangeToLevelScene;
	}
    private void ChangeToLevelScene()
    {
		playerManager.playerGUIHolder.Visible = true;
        GetTree().ChangeSceneToPacked(Level);
    }
}
