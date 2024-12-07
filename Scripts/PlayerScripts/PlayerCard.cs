using Godot;
using System;
using System.Dynamic;

public partial class PlayerCard : MarginContainer
{
	[Export] public Label ArrowCount {get; private set;}
	[Export] public Label HookCount {get; private set;}
	[Export] public Label LivesCount {get; private set;}
	[Export] public Label Health {get; private set;}
	[Export] public Label ComboCount {get; private set;}

}
