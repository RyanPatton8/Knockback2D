using Godot;
using System.Collections.Generic;

public partial class GenericPlayerState : SimpleState
{
    protected Player playerNode;
    public override void _Ready()
    {
        playerNode = GetOwner<Player>();
    }
}