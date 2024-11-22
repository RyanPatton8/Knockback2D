using Godot;
using System;
public partial class Melee : Node
{
    [Export] public PackedScene slash {get; private set;}
    private Player playerNode;
    double currentAttackDuration = .04; 
    double attackStrength = .35;
    double minAttackStrength = .35;
    double maxAttackStrength = 1;
    private int chargeSpeed = 1;
    private bool charged = false;
    private bool canAttack = true;
    private double attackCoolDown = .2;

    float distanceFromPlayer = 0;
    int chargeLevel = 1;
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
    }
    public override void _Process(double delta)
    {
        

        if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) > 0.5f && canAttack)
        {
            charged = true;
            attackStrength += delta * chargeSpeed;
            if (attackStrength >= maxAttackStrength)
            {
                attackStrength = maxAttackStrength;
                chargeLevel = 3;
            }
            else if(attackStrength >= (maxAttackStrength / 3) * 2)
            {
                distanceFromPlayer = .5f;
                chargeLevel = 2;
            }
            else
            {
                distanceFromPlayer = 0;
                chargeLevel = 1;
            }    
        } 
        else if (charged)
        {
            Attack(distanceFromPlayer, chargeLevel);
            canAttack = false;
            charged = false;
            attackStrength = minAttackStrength;
            distanceFromPlayer = 0;
            chargeLevel = 1;
        }
        else if (!canAttack)
        {
            attackCoolDown -= delta;
            if(attackCoolDown <= Mathf.Epsilon){
                canAttack = true;
                attackCoolDown = .3;
            }
        }
    }
    
    private void Attack(float distanceFromPlayer, int chargeLevel)
    {
        if(canAttack){
            canAttack = false;
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
            switch(chargeLevel){
                case 3:
                    attackMultiplier = 1.5f;
                    playerNode.ApplyImpulse(new Vector2(instance.GlobalPosition.X - playerNode.GlobalPosition.X, instance.GlobalPosition.Y - playerNode.GlobalPosition.Y ) * 100);
                    break;
                case 2:
                    attackMultiplier = 1;
                    playerNode.ApplyImpulse(new Vector2(instance.GlobalPosition.X - playerNode.GlobalPosition.X, instance.GlobalPosition.Y - playerNode.GlobalPosition.Y ) * 50);
                    break;
                default:
                    attackMultiplier = .2f;
                    break;
            }
            instance.attackStrength = attackMultiplier;
            GD.Print(chargeLevel);
        }
    }
}
