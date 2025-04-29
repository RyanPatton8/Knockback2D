using Godot;
using System;

public partial class HitBox : Area2D
{
    [Export] public Sprite2D HitSprite;
    [Export] public Timer SpriteTimer;
    [Export] public Texture2D ClashTexture;
    [Export] public Texture2D HitTexture;
    [Export] public AudioStream ClashAudio {get; private set;}
	[Export] public AudioStream ImpactAudio {get; private set;}
    [Export] public AudioStreamPlayer2D WeaponAudio {get; private set;}

    public override void _Ready(){
        SpriteTimer.Timeout += EraseSprite;
    }

    public void Clash(){
        WeaponAudio.Stream = ClashAudio;
        WeaponAudio.Play();
        HitSprite.Texture = ClashTexture;
        SpriteTimer.Start();
    }

    public void Hit(){
        WeaponAudio.Stream = ImpactAudio;
        WeaponAudio.Play();
        HitSprite.Texture = HitTexture;
        SpriteTimer.Start();
    }

    public void EraseSprite(){
        HitSprite.Texture = null;
    }
}
