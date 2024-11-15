using Godot;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class Guy : KinematicBody2D
{
	// Animation Objects
	public AnimationTree _animationTree;
	public AnimationNodeStateMachinePlayback _animationStateMachine;
	public Area2D _collision;

	// Movement
	[Export] public int Speed = 200;
	[Export] public int FastSpeed = 400;
	private Vector2 velocity = new Vector2();
	private List<int> movementPressedBuffer = new List<int>();

	//Actions
	private List<int> actionBuffer = new List<int>();
	[Export] public int AttackFrames = 33;
	[Export] public int AttackInterruptibleAfter = 22;
	private int currentAttackFrame = 0;
	private bool inAttack = false;
	private bool interruptible = false;
	[Export] public List<string> immuneGroups = new List<string>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		_animationStateMachine = (AnimationNodeStateMachinePlayback)(GetNode<AnimationTree>("AnimationTree").Get("parameters/playback"));
		immuneGroups.Add("Player");
	}
	
	public void HandleMovement()
	{
		velocity = new Vector2();
		if (!inAttack || interruptible)
		{
			if (Input.IsActionPressed("right") && movementPressedBuffer[0] == 0)
			{
				velocity.x = 1;
			}
			if (Input.IsActionPressed("left") && movementPressedBuffer[0] == 1)
			{
				velocity.x = -1;
			}
			if (Input.IsActionPressed("down") && movementPressedBuffer[0] == 2)
			{
				velocity.y = 1;
			}
			if (Input.IsActionPressed("up") && movementPressedBuffer[0] == 3)
			{
				velocity.y = -1;
			}
			velocity = velocity.Normalized() * Speed;
		}
	}
	public void StartAttack()
	{
		var direction = _animationTree.Get("parameters/Idle/blend_position");
		_animationTree.Set("parameters/Sword/blend_position", direction);
		_animationStateMachine.Stop();
		_animationStateMachine.Start("Sword");
		currentAttackFrame = 0;
		velocity = new Vector2();
		inAttack = true;
		interruptible = false;
	}

	public void HandleAttack()
	{
		currentAttackFrame++;
		if(currentAttackFrame == AttackInterruptibleAfter)
		{
			interruptible = true;
		}
		if(currentAttackFrame == AttackFrames)
		{
			inAttack = false;
		}
	}

	public void HandleCharacter()
	{
		HandleMovement();
		if(inAttack) {
			HandleAttack();
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
				StartAttack();
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

	public override void _PhysicsProcess(float delta)
	{
		HandleCharacter();
		if(velocity.Length() > 0.0) {
			_animationStateMachine.Travel("Walk");
			_animationTree.Set("parameters/Idle/blend_position", velocity);
			_animationTree.Set("parameters/Walk/blend_position", velocity);
		}
		else if (!inAttack){
			_animationStateMachine.Travel("Idle");
		}
		MoveAndSlide(velocity);
		
	}
}
