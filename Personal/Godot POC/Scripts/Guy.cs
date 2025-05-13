using Godot;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

public partial class Guy : CharacterBody2D
{
	// Animation Objects
	public AnimationTree _animationTree;
	public AnimationNodeStateMachinePlayback _animationStateMachine;
	public Area2D _collision;

	// Movement
	[Export] public int Speed = 200;
	[Export] public int FastSpeed = 400;
	private List<int> movementPressedBuffer = new List<int>();

	//Actions
	private List<int> actionBuffer = new List<int>();
	[Export] public int AttackFrames = 33;
	[Export] public int AttackInterruptibleAfter = 22;
	private int currentActionFrame = 0;
	private bool inAttack = false;
	private bool interruptible = false;
	public List<string> immuneGroups = new List<string>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		_animationStateMachine = (AnimationNodeStateMachinePlayback)(GetNode<AnimationTree>("AnimationTree").Get("parameters/playback"));
		immuneGroups.Add("Player");
	}
	
	public void HandleMovement()
	{
		Velocity = new Vector2();
		if (!inAttack || interruptible)
		{
			if (Input.IsActionPressed("right") && movementPressedBuffer[0] == 0)
			{
				Velocity = Vector2.Right;
			}
			if (Input.IsActionPressed("left") && movementPressedBuffer[0] == 1)
			{
				Velocity = Vector2.Left;
			}
			if (Input.IsActionPressed("down") && movementPressedBuffer[0] == 2)
			{
				Velocity = Vector2.Down;
			}
			if (Input.IsActionPressed("up") && movementPressedBuffer[0] == 3)
			{
				Velocity = Vector2.Up;
			}
			Velocity = Velocity.Normalized() * Speed;
		}
	}
	public void StartAction(Item i)
	{
		var direction = _animationTree.Get("parameters/Idle/blend_position");
		_animationTree.Set($"parameters/{i.AnimationPrefix}/blend_position", direction);
		_animationStateMachine.Stop();
		_animationStateMachine.Start(i.AnimationPrefix);
		currentActionFrame = 0;
		Velocity = new Vector2();
		inAttack = true;
		interruptible = false;
	}

	public void HandleAction(Item i)
	{
		currentActionFrame++;
		if(currentActionFrame == i.InterruptibleAfterFrames)
		{
			interruptible = true;
		}
		if(currentActionFrame == i.ActiveFrames)
		{
			inAttack = false;
		}
	}

	public void HandleCharacter()
	{
		HandleMovement();
		if(inAttack) {
			HandleAction(GetNode<Sword>("Sword"));
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("right"))
		{
			movementPressedBuffer.Insert(0, 0);
		}
		if (@event.IsActionPressed("left"))
		{
			movementPressedBuffer.Insert(0, 1);
		}
		if (@event.IsActionPressed("down"))
		{
			movementPressedBuffer.Insert(0, 2);
		}
		if (@event.IsActionPressed("up"))
		{
			movementPressedBuffer.Insert(0, 3);
		}
		if (@event.IsActionPressed("item1"))
		{
			if (!inAttack || (inAttack && interruptible))
			{
				StartAction(GetNode<Sword>("Sword"));
			}
		}


		if (@event.IsActionReleased("right"))
		{
			movementPressedBuffer.Remove(0);
		}
		if (@event.IsActionReleased("left"))
		{
			movementPressedBuffer.Remove(1);
		}
		if (@event.IsActionReleased("down"))
		{
			movementPressedBuffer.Remove(2);
		}
		if (@event.IsActionReleased("up"))
		{
			movementPressedBuffer.Remove(3);
		}
		// GD.Print(String.Join(", ", movementPressedBuffer));
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleCharacter();
		if(Velocity.Length() > 0.0) {
			_animationStateMachine.Travel("Walk");
			_animationTree.Set("parameters/Idle/blend_position", Velocity);
			_animationTree.Set("parameters/Walk/blend_position", Velocity);
		}
		else if (!inAttack){
			_animationStateMachine.Travel("Idle");
		}
		MoveAndSlide();
		
	}
}
