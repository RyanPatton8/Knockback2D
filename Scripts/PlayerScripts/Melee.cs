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

    public Slash slashInstance;
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
    }
    public override void _Process(double delta)
    {
        if(!playerNode.knockedBack){
            if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) < 0.7f && !playerNode.isAttacking){
                charged = true;
                attackStrength = maxAttackStrength;
                chargeLevel = 3;
            } 
            else if (charged){
                Attack(distanceFromPlayer, chargeLevel);
                charged = false;
            }
        }
        
    }
    private void Attack(float distanceFromPlayer, int chargeLevel)
    {
        if(!playerNode.isAttacking){
            playerNode.isAttacking = true;
            Vector2 aimDirection = playerNode.HitBox.GlobalPosition - playerNode.GlobalPosition;
            slashInstance = (Slash)slash.Instantiate();
            playerNode.AddChild(slashInstance);
            slashInstance.GlobalPosition = playerNode.HitBox.GlobalPosition + aimDirection * distanceFromPlayer;
            slashInstance.Rotation = playerNode.HitBox.Rotation;
            slashInstance.Scale = slashInstance.Scale * chargeLevel;
            slashInstance.playerIndex = playerNode.playerIndex;
            slashInstance.player = playerNode;
            slashInstance.chargeLevel = chargeLevel;
            float attackMultiplier;
            attackMultiplier = 1.5f;
            playerNode.ApplyImpulse(new Vector2((slashInstance.GlobalPosition.X - playerNode.GlobalPosition.X) / 2, slashInstance.GlobalPosition.Y - playerNode.GlobalPosition.Y ) * 125);
            slashInstance.attackStrength = attackMultiplier;
        }
    }
}
