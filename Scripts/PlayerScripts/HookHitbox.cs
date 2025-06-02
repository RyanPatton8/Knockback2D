using Godot;
using System;

public partial class HookHitbox : Area2D
{
	private Hook hookNode;
	private Player playerNode;
    private FishingRod fishingRod;
    public double forceApplied;
    [Export] public AudioStream ImpactAudio {get; private set;}
	
	public override void _Ready()
    {
        //Getting references to important nodes via scaling backwards up scene tree
        hookNode = GetOwner<Hook>();
		playerNode = GetNode<Player>("../../../.."); 
		fishingRod = GetNode<FishingRod>("../.."); 
		BodyEntered += CallDespawn;
    }
    private void CallDespawn(Node2D body)
    {
        //Can't hit yourself
        if(body is Player player && player.playerIndex == playerNode.playerIndex)
        {
            return;
        }
        playerNode.HookAudio.Stream = ImpactAudio;
        playerNode.HookAudio.Play();
        //Grapple if its environment
		if(body.IsInGroup("Environment"))
        {
		    playerNode.Grapple(GlobalPosition - playerNode.GlobalPosition);
        }
        fishingRod.hookOut = false;
        hookNode.QueueFree();
    }
    /*
        if you hit someone remove knowback effects and return info to pull other player toward you
        Knockback is also removed when grappling to environment but this is handled within player script in Grapple()
    */
    public Vector2 GiveInfo(){
        playerNode.KnockBackDuration.WaitTime = 0.01f;
        playerNode.AllowMovement();
		return playerNode.GlobalPosition;
	}
    public int GiveIndexInfo(){
        return playerNode.playerIndex;
    }
}
