using Godot;
using System;

public partial class Item : Node2D
{
	// Declare member variables here. Examples:
	public Sprite2D _sprite;
	public Area2D _collision;

	[Export] public string AnimationPrefix;
	[Export] public int ActiveFrames;
	[Export] public int InterruptibleAfterFrames;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public virtual void HandleActivate()
	{

	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
