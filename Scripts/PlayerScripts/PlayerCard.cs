using Godot;
using System;
using System.Dynamic;

public partial class PlayerCard : MarginContainer
{
	[Export] public Panel OuterBackground {get; private set;}
	[Export] public Label LivesCount {get; private set;}
	[Export] public Label KillCount {get; private set;}
	[Export] public Label Health {get; private set;}

	public void SetAll(string lives, string health, Color backgroundColor){
		SetLivesCount(lives);
		SetHealth(health);
		SetBackgroundColor(new Color(backgroundColor.R * 2, backgroundColor.G * 2 , backgroundColor.B * 2, 0.85f));
	}
	public void SetLivesCount(string change){
		LivesCount.Text = "Lives: " + change;
	}
	public void SetHealth(string change){
		Health.Text = change + "%";
	}
	private void SetBackgroundColor(Color backgroundColor){
		OuterBackground.Modulate = backgroundColor;
	}
	public void MakeBlank(){
		LivesCount.Text = "";
		Health.Text = "";
	}
}
