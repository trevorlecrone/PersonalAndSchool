using Godot;
using System;

public class Health : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	
	[Export] public int BaseMax = 12;
	[Export] public int BonusMax = 4;
	public int current = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		current = BaseMax;
	}
	
	public void Damage(int damage)
	{
		current -= damage;
		if(current <= 0) {
			this.GetParent().QueueFree();
		}
	}

	public void Heal(int health)
	{
		current = Math.Min(current + health, BaseMax);
	}

	public void Overheal(int health)
	{
		current = Math.Min(BaseMax + health, BaseMax + BonusMax);
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
