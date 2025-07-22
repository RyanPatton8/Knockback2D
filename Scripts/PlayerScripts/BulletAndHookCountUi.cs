using Godot;
using System;

public partial class BulletAndHookCountUi : Node
{
    [Export] public Sprite2D BulletSprite;
    public Color bulletColor;
    [Export] public Label ArrowCount { get; private set; }
    [Export] public Label HookCount { get; private set; }
    PlayerManager playerManager;
    public int playerIndex;

    public override void _Ready()
    {
        playerManager = PlayerManager.Instance;
    }
    public void UpdateArrowCount()
    {
        ArrowCount.Text = $"x{playerManager.playerList[playerIndex].GetArrowCount()}";
    }
	public void UpdateHookCount(){
		HookCount.Text = $"x{playerManager.playerList[playerIndex].GetHookCount()}";
	}
}
