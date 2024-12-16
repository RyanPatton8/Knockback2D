using Godot;
using System;
using System.Dynamic;

public partial class PlayerCard : MarginContainer
{
	[Export] public Panel OuterBackground {get; private set;}
	[Export] public Label ArrowCount {get; private set;}
	[Export] public Label HookCount {get; private set;}
	[Export] public Label LivesCount {get; private set;}
	[Export] public Label Health {get; private set;}
	[Export] public Label ComboCount {get; private set;}

	public void SetAll(string arrows, string hooks, string lives, string health, string comboCount, Color backgroundColor){
		SetArrowCount(arrows);
		SetHookCount(hooks);
		SetLivesCount(lives);
		SetHealth(health);
		SetComboCount(comboCount);
		SetBackgroundColor(new Color(backgroundColor.R * 2, backgroundColor.G * 2 , backgroundColor.B * 2, 0.85f));
	}
	public void SetArrowCount(string change){
		ArrowCount.Text = "Bullets: " + change;
	}
	public void SetHookCount(string change){
		HookCount.Text = "Hooks: " + change;
	}
	public void SetLivesCount(string change){
		LivesCount.Text = "Lives: " + change;
	}
	public void SetHealth(string change){
		Health.Text = change + "%";
	}
	public void SetComboCount(string change){
		if(change == "1"){
			ComboCount.Text = "";
		}
		else{
			ComboCount.Text = "x" + change;
		}
	}
	private void SetBackgroundColor(Color backgroundColor){
		OuterBackground.Modulate = backgroundColor;
	}
	public void MakeBlank(){
		ArrowCount.Text = "";
		HookCount.Text = "";
		LivesCount.Text = "";
		Health.Text = "";
		ComboCount.Text = "";
	}
}
