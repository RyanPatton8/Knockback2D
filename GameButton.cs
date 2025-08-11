using Godot;
using System;

public partial class GameButton : StaticBody2D
{
    [Export] public AnimationPlayer anim { get; private set; }
    [Export] public Area2D btnArea { get; private set; }
    [Export] public Label readyLabel { get; private set; }
    [Export] public bool playerBtn = true;
    [Export] public MarginContainer instructions { get; private set; }

    public override void _Ready()
    {
        btnArea.BodyEntered += ButtonDown;
        btnArea.BodyExited += ButtonUp;
    }
    private void ButtonDown(Node body)
    {
        anim.Play("button_down");
        if (playerBtn)
        {
            readyLabel.Visible = true;
        }
        else
        {
            instructions.Visible = !instructions.Visible;
        }
    }

    private void ButtonUp(Node body)
    {
        anim.Play("button_up");
        if (playerBtn)
        {
            readyLabel.Visible = false;
        }
        else
        {
            // instructions.Visible = false;
        }
    }
}
