using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Sword : Item
{
	// Declare member variables here. Examples:	
	
	[Export] public int Dmg = 1;
	[Export] public int StrongDmg = 2;
	
	public List<string> immuneGroups = new List<string>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_collision = GetNode<Area2D>("SwordCollision");
		immuneGroups.Add("Player");
		_collision.Connect("area_entered", new Callable(this, "HandleCollision"));
		AnimationPrefix = "Sword";
		ActiveFrames = 33;
		InterruptibleAfterFrames = 22;

	}

	public override void HandleActivate()
	{
		this.HandleAttack();
	}

	// Called when the node enters the scene tree for the first time.
	public void HandleAttack()
	{
		
	}
	
	public void HandleCollision(HitboxNode otherHitbox)
	{

		if (!otherHitbox.GetParent().IsInGroup(immuneGroups.First()))
		{
			if(otherHitbox.GetParent().IsInGroup("Enemy")) 
			{
				GD.Print("swordHit");
				otherHitbox.TakeDamage(Dmg);
			}
		}
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
