using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace Sandbox;

public class Protag : IControllable
{

    // Animation Objects
    private Dictionary<(Direction, bool), Sprite> spriteTree;
    private Sprite currentSprite;

    // Movement
    public Vector2 Position;
	public int Speed = 200;
	public int FastSpeed = 400;
    public bool Sprint = false;
    public Direction Facing = Direction.DOWN;
    private bool facingLocked = false;
	private List<Direction> movementPressedBuffer = new List<Direction>();

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

        this.Sprint = false;
        if (keyboard.IsKeyDown(Keys.LeftShift))
        {
            this.Sprint = true;
        }
        
        if (keyboard.KeyPressed(Keys.W))
        {
            this.movementPressedBuffer.Insert(0, Direction.UP);
        }
        if (keyboard.KeyPressed(Keys.S))
        {
            this.movementPressedBuffer.Insert(0, Direction.DOWN);
        }
        if (keyboard.KeyPressed(Keys.A))
        {
            this.movementPressedBuffer.Insert(0, Direction.LEFT);
        }
        if (keyboard.KeyPressed(Keys.D))
        {
           this.movementPressedBuffer.Insert(0, Direction.RIGHT);
        }

        if (keyboard.KeyReleased(Keys.W))
        {
            this.movementPressedBuffer.Remove(Direction.UP);
        }
        if (keyboard.KeyReleased(Keys.S))
        {
            this.movementPressedBuffer.Remove(Direction.DOWN);
        }
        if (keyboard.KeyReleased(Keys.A))
        {
            this.movementPressedBuffer.Remove(Direction.LEFT);
        }
        if (keyboard.KeyReleased(Keys.D))
        {
           this.movementPressedBuffer.Remove(Direction.RIGHT);
        }

        this.HandleMovement(keyboard);
	}

    public void HandleMovement(KeyboardInfo keyboard)
	{
		var velocity = new Vector2();
		if (!inAttack || interruptible)
		{
			if (this.movementPressedBuffer.Count > 0)
            {
                this.EvaluateFacingAndVelocity_KeyBoard(velocity);
            }
            this.Position = this.Position + velocity;
            this.currentSprite = this.spriteTree[(this.Facing, velocity.Length() > 0)];
		}
	}

    private void EvaluateFacingAndVelocity_KeyBoard(Vector2 velocity)
	{
        KeyboardInfo keyboard = Core.Input.Keyboard;
        if (this.movementPressedBuffer.Count < 2)
		{
			if (keyboard.IsKeyDown(Keys.W) && this.movementPressedBuffer[0] == Direction.UP)
			{
				velocity.Y = -1;
                this.Facing = Direction.UP;
			}
			if (keyboard.IsKeyDown(Keys.S) && this.movementPressedBuffer[0] == Direction.DOWN)
			{
				velocity.Y = 1;
                this.Facing = Direction.DOWN;
			}
            if (keyboard.IsKeyDown(Keys.A) && this.movementPressedBuffer[0] == Direction.LEFT)
			{
				velocity.X = -1;
                this.Facing = Direction.LEFT;
			}
            if (keyboard.IsKeyDown(Keys.D) && this.movementPressedBuffer[0] == Direction.RIGHT)
			{
				velocity.X = 1;
                this.Facing = Direction.RIGHT;
			}
			velocity.Normalize();
            velocity *= Speed;
		}
        // first two items in buffer are not up & down or left and right
        else if ((int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] != 1 && (int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] != 5)
		{
			if (keyboard.IsKeyDown(Keys.W) && (this.movementPressedBuffer[0] == Direction.UP || this.movementPressedBuffer[1] == Direction.UP))
			{
				velocity.Y = -1;
			}
			if (keyboard.IsKeyDown(Keys.S) && (this.movementPressedBuffer[0] == Direction.DOWN || this.movementPressedBuffer[1] == Direction.DOWN))
			{
				velocity.Y = 1;
			}
            if (keyboard.IsKeyDown(Keys.A) && (this.movementPressedBuffer[0] == Direction.LEFT || this.movementPressedBuffer[1] == Direction.LEFT))
			{
				velocity.X = -1;
			}
            if (keyboard.IsKeyDown(Keys.D) && (this.movementPressedBuffer[0] == Direction.RIGHT || this.movementPressedBuffer[1] == Direction.RIGHT))
			{
				velocity.X = 1;
			}
			velocity.Normalize();
            velocity *= Speed;

            //el-ifs so we only evaluate once
            if(this.Facing == Direction.UP && velocity.Y > 0 || this.Facing == Direction.DOWN && velocity.Y < 0)
            {
                this.Facing = velocity.X > 0 ? Direction.RIGHT : Direction.LEFT;
            }
            else if((this.Facing == Direction.RIGHT && velocity.X < 0) || this.Facing == Direction.LEFT && velocity.X > 0)
            {
                this.Facing = velocity.Y > 0 ? Direction.DOWN : Direction.UP;
            }
		}

	}

    private void ConstructSpriteTree(TextureAtlas atlas)
    {
        this.spriteTree.Add((Direction.UP, false), atlas.CreateSprite("up-1"));
        this.spriteTree.Add((Direction.UP, true), atlas.CreateAnimatedSprite("walk-up"));
        this.spriteTree.Add((Direction.DOWN, false), atlas.CreateSprite("down-1"));
        this.spriteTree.Add((Direction.DOWN, true), atlas.CreateAnimatedSprite("walk-down"));
        this.spriteTree.Add((Direction.LEFT, false), atlas.CreateSprite("left-1"));
        this.spriteTree.Add((Direction.LEFT, true), atlas.CreateSprite("walk-left"));
        this.spriteTree.Add((Direction.RIGHT, false), atlas.CreateSprite("right-1"));
        this.spriteTree.Add((Direction.RIGHT, true), atlas.CreateAnimatedSprite("walk-right"));
    }

    public void Draw(SpriteBatch sb, Vector2 position)
    {
        this.currentSprite.Draw(sb, this.Position);
    }

    public void CheckGamepadInput()
	{
		// Get a reference to the keyboard inof
        KeyboardInfo keyboard = Core.Input.Keyboard;
	}

}