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
		aimTextureKVP.Add(0, meleeAimTexture);
		aimTextureKVP.Add(1, rangedAimTexture);
		foreach (Node weapon in weapons)
		{
			weapon.SetProcess(false);
		}
		weapons[weaponIndex].SetProcess(true);
		weapons[2].SetProcess(true);
	}

    public void ChangeWeapon()
	{
		// disable current weapons physics process
		weapons[weaponIndex].SetProcess(false);
		// change the weapon
		weaponIndex = weaponIndex == 1 ? 0 : 1;
		hitBoxSprite.Texture = aimTextureKVP[weaponIndex];
		// enable the current weapon
		weapons[weaponIndex].SetProcess(true);
	}
}
