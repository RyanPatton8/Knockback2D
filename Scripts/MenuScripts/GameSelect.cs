using Godot;
using System;

public partial class GameSelect : Control
{
    [Export] public Button PlayBtn { get; private set; }
    [Export] public CheckButton TeamsOn { get; private set; }
    [Export] public OptionButton GameMode { get; private set; }
    GameManager gameManager;
    PlayerManager playerManager;
    public override void _Ready()
    {
        PlayBtn.GrabFocus();
        PlayBtn.ButtonDown += ChangeToLevelScene;
        gameManager = GameManager.Instance;
        playerManager = PlayerManager.Instance;
    }
    private void ChangeToLevelScene()
    {
        GameMode chosenGameMode;
        switch (GameMode.Selected)
        {
            case 0:
                chosenGameMode = new StockBattle(playerManager, gameManager, TeamsOn.ButtonPressed);
                break;
            case 1:
                chosenGameMode = new Elimination(playerManager, gameManager, TeamsOn.ButtonPressed);
                break;
            default:
                chosenGameMode = new StockBattle(playerManager, gameManager, TeamsOn.ButtonPressed);
                GD.Print("Something went wrong with gamemode switchcase");
                break;
        }
        GD.Print($"Current GameMode {GameMode.Selected}");
		gameManager.ReadyUp(chosenGameMode);
	}
}
