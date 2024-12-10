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

	public void SetAll(string arrows, string hooks, string lives, string health, string comboCount){
		SetArrowCount(arrows);
		SetHookCount(hooks);
		SetLivesCount(lives);
		SetHealth(health);
		SetComboCount(comboCount);
	}
	public void SetArrowCount(string change){
		ArrowCount.Text = "Arrows: " + change;
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
}
