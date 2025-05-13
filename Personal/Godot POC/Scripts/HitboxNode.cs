using Godot;
using System;

public partial class HitboxNode : Area2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	[Export] public Health HealthObject;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	
	public void TakeDamage(int damage)
	{
		HealthObject.Damage(damage);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
