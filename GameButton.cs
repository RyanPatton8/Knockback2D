using Godot;
using System;

public partial class GameButton : StaticBody2D
{
    [Export] public AnimationPlayer anim { get; private set; }
    [Export] public Area2D btnArea { get; private set; }

    public override void _Ready()
    {
        btnArea.BodyEntered += ButtonDown;
        btnArea.BodyExited += ButtonUp;
    }
    private void ButtonDown(Node body)
    {
        anim.Play("button_down");
    }

    private void ButtonUp(Node body)
    {
        anim.Play("button_up");
    }
}
