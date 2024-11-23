using Godot;
using System;
public partial class Melee : Node
{
    [Export] public PackedScene slash {get; private set;}
    private Player playerNode;

    public int weaponPlayerIndex;
    double currentAttackDuration = .04; 
    double attackStrength = .35;
    double minAttackStrength = .35;
    double maxAttackStrength = 1;
    private int chargeSpeed = 1;
    private bool charged = false;
    float distanceFromPlayer = 0;
    int chargeLevel = 1;
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
    }
    public override void _Process(double delta)
    {
        if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) < 0.5f && !playerNode.isAttacking)
        {
            charged = true;
            attackStrength = maxAttackStrength;
            chargeLevel = 3;
        } 
        else if (charged)
        {
            Attack(distanceFromPlayer, chargeLevel);
            charged = false;
        }
    }
    private void Attack(float distanceFromPlayer, int chargeLevel)
    {
        if(!playerNode.isAttacking){
            playerNode.isAttacking = true;
            Vector2 aimDirection = playerNode.HitBox.GlobalPosition - playerNode.GlobalPosition;
            Slash instance = (Slash)slash.Instantiate();
            playerNode.AddChild(instance);
            instance.GlobalPosition = playerNode.HitBox.GlobalPosition + aimDirection * distanceFromPlayer;
            instance.Rotation = playerNode.HitBox.Rotation;
            instance.Scale = instance.Scale * chargeLevel;
            instance.playerIndex = playerNode.playerIndex;
            instance.player = playerNode;
            instance.chargeLevel = chargeLevel;
            float attackMultiplier;
            attackMultiplier = 1.5f;
            playerNode.ApplyImpulse(new Vector2((instance.GlobalPosition.X - playerNode.GlobalPosition.X) / 2, instance.GlobalPosition.Y - playerNode.GlobalPosition.Y ) * 150);
            instance.attackStrength = attackMultiplier;
            GD.Print(chargeLevel);
        }
    }
}
