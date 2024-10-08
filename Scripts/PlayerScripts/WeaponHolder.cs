using Godot;
using System;
using System.Linq;

public partial class WeaponHolder : Node
{
	private int weaponIndex = 0;
	private double weaponChangeTime = .5;
	public bool changingWeapon = false;
	[Export] private Node[] weapons;
	public int currentWeapon;

	public override void _Ready()
	{
		foreach (Node weapon in weapons)
		{
			weapon.SetProcess(false);
		}
		weapons[weaponIndex].SetProcess(true);
		currentWeapon = 0;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (changingWeapon)
		{
			weaponChangeTime -= delta;
			if (weaponChangeTime <= Mathf.Epsilon)
			{
				weaponChangeTime = .5;
				changingWeapon = false;
			}
		}
	}
	public void ChangeWeapon()
	{
		changingWeapon = true;
		// disable current weapons physics process
		weapons[weaponIndex].SetProcess(false);
		// change the weapon
		weaponIndex++;
		currentWeapon++;
		
		if (weaponIndex > weapons.Length - 1)
		{
			weaponIndex = 0;
			currentWeapon = 0;
		}
		// enable the current weapon
		weapons[weaponIndex].SetProcess(true);
	}
}
