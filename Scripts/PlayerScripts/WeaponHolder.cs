using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WeaponHolder : Node
{
	public int weaponIndex = 0;
	[Export] public Node[] weapons;
	private Texture2D meleeAimTexture = (Texture2D)GD.Load("res://Sprites/MeleeAim.svg");
	private Texture2D rangedAimTexture = (Texture2D)GD.Load("res://Sprites/RangedAim.svg");
	private Dictionary<int,Texture2D> aimTextureKVP = new Dictionary<int, Texture2D>(3);
	[Export] public Sprite2D hitBoxSprite {get; private set;}
	public int currentWeapon;

	public override void _Ready()
	{
		aimTextureKVP.Add(0, meleeAimTexture); // attach melee color in dictionary
		aimTextureKVP.Add(1, rangedAimTexture);// attach ranged color in dictionary
		weapons[1].SetProcess(false);// disable ranged
	}

    public void ChangeWeapon()
	{
		// disable current weapons physics process
		weapons[weaponIndex].SetProcess(false);
		// change the weapon back and forth between 0 and 1.
		// alter color of hitbox to reflect chosen weapon
		weaponIndex = weaponIndex == 1 ? 0 : 1;
		hitBoxSprite.Texture = aimTextureKVP[weaponIndex];
		// enable the current weapon
		weapons[weaponIndex].SetProcess(true);
	}
}
