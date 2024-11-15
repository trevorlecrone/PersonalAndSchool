using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Sword : Node2D
{
	// Declare member variables here. Examples:
	public Sprite _sprite;
	public Area2D _collision;
	
	// Movement
	[Export] public int Dmg = 1;
	[Export] public int StrongDmg = 2;
	
	[Export] public List<string> immuneGroups = new List<string>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_collision = GetNode<Area2D>("SwordCollision");
		immuneGroups.Add("Player");
		_collision.Connect("area_entered", this, "HandleCollision");
	}

	// Called when the node enters the scene tree for the first time.
	public void HandleAttack()
	{
		
	}
	
	public void HandleCollision(Area2D otherHitbox)
	{
		if (!otherHitbox.GetParent().IsInGroup(immuneGroups.First()))
		{
			if(otherHitbox.GetParent().IsInGroup("Enemy")) 
			{
				GD.Print("swordHit");
			}
		}
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
