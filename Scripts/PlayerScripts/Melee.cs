using Godot;
using System;
public partial class Melee : Node
{
    [Export] public PackedScene slash {get; private set;}
    private Player playerNode;
    private bool isTriggerDown = false;
    public Slash slashInstance;
    [Export] public AudioStream SlashAudio {get; private set;}
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
    }
    public override void _Process(double delta)
    {
        if(!playerNode.knockedBack || playerNode.rocketJumping){
            if (Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) > 0.7f && !playerNode.isAttacking && !isTriggerDown){
                if(playerNode.isWeaponInGround){
                    var hb = playerNode.HitBox as HitBox;
                    hb.Clash();
                    playerNode.isClashing = true;
			        playerNode.Clashed(playerNode.GlobalPosition - playerNode.HitBox.GlobalPosition, playerNode.playerIndex, false);
                    isTriggerDown = true; 
                }
                else{
                    playerNode.WeaponAudio.Stream = SlashAudio;
                    playerNode.WeaponAudio.Play();
                    Attack();
                    isTriggerDown = true; 
                }
                
            } 
            else if(isTriggerDown && Input.GetJoyAxis(playerNode.playerIndex, JoyAxis.TriggerRight) < 0.7f){
                isTriggerDown = false;
            }
        }
        
    }
    private void Attack()
    {
        if(!playerNode.isAttacking){
            playerNode.isAttacking = true;
            slashInstance = (Slash)slash.Instantiate();
            playerNode.AddChild(slashInstance);
            slashInstance.GlobalPosition = playerNode.HitBox.GlobalPosition;
            slashInstance.Rotation = playerNode.HitBox.Rotation;
            slashInstance.playerIndex = playerNode.playerIndex;
            slashInstance.player = playerNode;
            if(playerNode.justJumped){
                playerNode.LinearVelocity = new Vector2(playerNode.LinearVelocity.X, 0);
                playerNode.ApplyImpulse(new Vector2(0, -4500));
                playerNode.ApplyImpulse(new Vector2((slashInstance.GlobalPosition.X - playerNode.GlobalPosition.X) / 2, slashInstance.GlobalPosition.Y - playerNode.GlobalPosition.Y ) * 110);        
            }
            else{
                playerNode.ApplyImpulse(new Vector2((slashInstance.GlobalPosition.X - playerNode.GlobalPosition.X) / 2, slashInstance.GlobalPosition.Y - playerNode.GlobalPosition.Y ) * 110);            
            }
        }
    }
}
