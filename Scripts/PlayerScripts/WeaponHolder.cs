using Godot;
using System;
using System.Linq;

public partial class WeaponHolder : Node
{
	private int weaponIndex = 0;
	[Export] private Node[] weapons;
	public override void _Ready()
    {
		foreach(Node weapon in weapons)
		{
			weapon.SetProcess(false);
		}
		weapons[weaponIndex].SetProcess(true);
    }
	public void ChangeWeapon()
	{
		// disable current weapons physics process
		weapons[weaponIndex].SetProcess(false);
		// change the weapon
		weaponIndex++;
		if (weaponIndex > weapons.Length - 1)
		{
			weaponIndex = 0;
		}
		// enable the current weapon
		weapons[weaponIndex].SetProcess(true);
	}
}
