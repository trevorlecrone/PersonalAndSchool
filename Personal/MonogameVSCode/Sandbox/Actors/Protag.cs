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
    private Dictionary<(Direction, bool), AnimatedSprite> spriteTree = new Dictionary<(Direction, bool), AnimatedSprite>();
    private AnimatedSprite currentSprite;

    // Movement
    public Vector2 Position;
	public const int Speed = 5;
	public const int FastSpeed = 7;
    public bool Sprint = false;
    public Direction Facing = Direction.DOWN;
    private bool facingLocked = false;
	private List<Direction> movementPressedBuffer = new List<Direction>();

    //Actions
	private List<int> actionBuffer = new List<int>();
	public const int AttackFrames = 33;
	public const int AttackInterruptibleAfter = 22;
	private int currentActionFrame = 0;
	private bool inAttack = false;
	private bool interruptible = false;
	public List<string> ImmuneGroups = new List<string>();

    public Protag (TextureAtlas atlas, Vector2 position)
    {
        this.ConstructSpriteTree(atlas);
        this.Position = position;
        this.currentSprite = this.spriteTree[(Direction.DOWN, false)];
    }

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
                velocity = this.EvaluateFacingAndVelocity_KeyBoard(velocity, keyboard);
            }
            this.Position = this.Position + velocity;
            this.currentSprite = this.spriteTree[(this.Facing, velocity.Length() > 0)];
		}
	}

    private Vector2 EvaluateFacingAndVelocity_KeyBoard(Vector2 velocity, KeyboardInfo keyboard)
	{
        if (this.movementPressedBuffer.Count < 2 || (int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] == 1 || (int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] == 5)
		{
			if (keyboard.IsKeyDown(Keys.W) && this.movementPressedBuffer[0] == Direction.UP)
			{
				velocity.Y = -1;
                this.Facing = Direction.UP;
			}
			else if (keyboard.IsKeyDown(Keys.S) && this.movementPressedBuffer[0] == Direction.DOWN)
			{
				velocity.Y = 1;
                this.Facing = Direction.DOWN;
			}
            else if (keyboard.IsKeyDown(Keys.A) && this.movementPressedBuffer[0] == Direction.LEFT)
			{
				velocity.X = -1;
                this.Facing = Direction.LEFT;
			}
            else if (keyboard.IsKeyDown(Keys.D) && this.movementPressedBuffer[0] == Direction.RIGHT)
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
        else
        {
            
        }
        return velocity;

	}

    private void ConstructSpriteTree(TextureAtlas atlas)
    {
        this.spriteTree.Add((Direction.UP, false), atlas.CreateAnimatedSprite("idle-up"));
        this.spriteTree.Add((Direction.UP, true), atlas.CreateAnimatedSprite("walk-up"));
        this.spriteTree.Add((Direction.DOWN, false), atlas.CreateAnimatedSprite("idle-down"));
        this.spriteTree.Add((Direction.DOWN, true), atlas.CreateAnimatedSprite("walk-down"));
        this.spriteTree.Add((Direction.LEFT, false), atlas.CreateAnimatedSprite("idle-left"));
        this.spriteTree.Add((Direction.LEFT, true), atlas.CreateAnimatedSprite("walk-left"));
        this.spriteTree.Add((Direction.RIGHT, false), atlas.CreateAnimatedSprite("idle-right"));
        this.spriteTree.Add((Direction.RIGHT, true), atlas.CreateAnimatedSprite("walk-right"));
    }

    public void Update(GameTime gameTime)
    {
        this.currentSprite.Update(gameTime);
    }

    public void Draw(SpriteBatch sb)
    {
        this.currentSprite.Draw(sb, this.Position);
    }

    public void CheckGamepadInput()
	{
		// Get a reference to the keyboard inof
        KeyboardInfo keyboard = Core.Input.Keyboard;
	}

}