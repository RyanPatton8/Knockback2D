using Godot;
using System;
using System.Dynamic;

public partial class PlayerCard : MarginContainer
{
	[Export] public Panel OuterBackground {get; private set;}
	[Export] public Label GoalCount {get; private set;}
	[Export] public Label Health {get; private set;}

	public void SetAll(string goal, string health, Color backgroundColor)
	{
		SetGoalCount(goal);
		SetHealth(health);
		SetBackgroundColor(new Color(backgroundColor.R * 2, backgroundColor.G * 2, backgroundColor.B * 2, 0.85f));
	}
	public void SetGoalCount(string change){
		GoalCount.Text = change;
	}
	public void SetHealth(string change){
		Health.Text = change + "%";
	}
	private void SetBackgroundColor(Color backgroundColor){
		OuterBackground.Modulate = backgroundColor;
	}
	public void MakeBlank(){
		GoalCount.Text = "";
		Health.Text = "";
	}
}
