using Godot;
using System;
using System.Collections.Generic;

public partial class LevelChoice : TextureButton
{
	[Export] public PackedScene Level {get; private set;}
	[Export] public Texture2D LevelImg {get; private set;}
	GameManager gameManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(GetParent().GetChild(0) == this)
			GrabFocus();
		if(LevelImg.GetWidth() > 240){
			throw new Exception($"Level img too big");
		}
		TextureNormal = LevelImg;
		gameManager = GameManager.Instance;
		ButtonDown += ChangeToLevelScene;
	}
    private void ChangeToLevelScene()
    {
		gameManager.StartGame(Level);
    }
}
