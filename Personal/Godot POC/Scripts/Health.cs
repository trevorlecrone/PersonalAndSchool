using Godot;
using System;

[GlobalClass]
public partial class Health : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	
	[Export] public int BaseMax;
	[Export] public int BonusMax;
	public int current = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		current = BaseMax;
	}
	
	public void Damage(int damage)
	{
		current -= damage;
		GD.Print("Health Damage");
		if(current <= 0) {
			GD.Print("dead");
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
