using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace Sandbox;

public class Protag : IControllable
{

    // Animation Objects
    private Dictionary<int, Sprite> spriteTree;
    private Sprite currentSprite;

    // Movement
	public int Speed = 200;
	public int FastSpeed = 400;
	private List<int> movementPressedBuffer = new List<int>();

    //Actions
	private List<int> actionBuffer = new List<int>();
	public int AttackFrames = 33;
	public int AttackInterruptibleAfter = 22;
	private int currentActionFrame = 0;
	private bool inAttack = false;
	private bool interruptible = false;
	public List<string> ImmuneGroups = new List<string>();

    public void CheckKeyboardInput()
	{
		// Get a reference to the keyboard inof
        KeyboardInfo keyboard = Core.Input.Keyboard;
	}

    public void CheckGamepadInput()
	{
		// Get a reference to the keyboard inof
        KeyboardInfo keyboard = Core.Input.Keyboard;
	}

}