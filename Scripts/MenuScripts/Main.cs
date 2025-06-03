using Godot;
using System;

public partial class Main : Control
{
    [Export] public Button PlayBtn { get; private set; }
    [Export] public CheckButton TeamsOn { get; private set; }
    [Export] public OptionButton GameMode { get; private set; }
    GameManager gameManager;
    public override void _Ready()
    {
        PlayBtn.GrabFocus();
        PlayBtn.ButtonDown += ChangeToLevelScene;
        gameManager = GameManager.Instance;
    }
    private void ChangeToLevelScene()
    {
        GD.Print($"Check Button {TeamsOn.ButtonPressed}");
        GD.Print($"Option Button {GameMode.Selected}");
		gameManager.ReadyUp();
	}
}
