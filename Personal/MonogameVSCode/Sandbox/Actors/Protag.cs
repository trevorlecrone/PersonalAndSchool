using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace Sandbox;

public class Protag : IControllable
{

    // Animation Objects
    private Dictionary<(Direction, bool), AnimatedSprite> spriteTree = new Dictionary<(Direction, bool), AnimatedSprite>();
    private AnimatedSprite currentSprite;
    public int Height;
    public int Width;

    // Movement
    public Vector2 Position;
    public Vector2 Velocity;
	public const int Speed = 5;
	public const int FastSpeed = 7;
   
    public bool Sprint = false;
    public Direction Facing = Direction.DOWN;
    public CollisionProperties CollisionProperties = new CollisionProperties() | CollisionProperties.BLOCKING;
    public CollisionGroups CollisionGroups = new CollisionGroups() | CollisionGroups.GROUNDED;
    public CollisionRectangle Hitbox = new CollisionRectangle(1);
     private const int damageImpulse = 7;
    private bool facingLocked = false;
	private List<Direction> movementPressedBuffer = new List<Direction>();

    //Actions
	private List<int> actionBuffer = new List<int>();
	public const int AttackFrames = 33;
	public const int AttackInterruptibleAfter = 22;
    private const int courtesyFrames = 10;
    private const int damageImpulseFrames = 3;
	private int currentActionFrame = 0;
    private int CurrentImpuseFrame = 0;
    private int currentCourtesyFrame = 22;
	private bool inAttack = false;
	private bool interruptible = false;
	public List<string> ImmuneGroups = new List<string>();

    public Protag (TextureAtlas atlas, Vector2 position)
    {
        this.ConstructSpriteTree(atlas);
        this.Position = position;
        this.currentSprite = this.spriteTree[(Direction.DOWN, false)];
        this.Height = (int)this.currentSprite.Height;
        this.Width = (int)this.currentSprite.Width;
        this.Hitbox = new CollisionRectangle(
            this.CollisionGroups,
            this.CollisionProperties,
            this.Position,
            this.Height,
            this.Width,
            this.HandleCollision,
            1);
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
		this.Velocity = new Vector2();
		if (!inAttack || interruptible)
		{
			if (this.movementPressedBuffer.Count > 0)
            {
                this.EvaluateFacingAndVelocity_KeyBoard(keyboard);
            }
            this.Position = this.Position + this.Velocity;
            this.Hitbox.Anchor = this.Position;
            this.currentSprite = this.spriteTree[(this.Facing, this.Velocity.Length() > 0)];
		}
	}

    private void EvaluateFacingAndVelocity_KeyBoard(KeyboardInfo keyboard)
	{
        if (this.movementPressedBuffer.Count < 2 || (int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] == 1 || (int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] == 5)
		{
			if (keyboard.IsKeyDown(Keys.W) && this.movementPressedBuffer[0] == Direction.UP)
			{
				this.Velocity.Y = -1;
                this.Facing = Direction.UP;
			}
			else if (keyboard.IsKeyDown(Keys.S) && this.movementPressedBuffer[0] == Direction.DOWN)
			{
				this.Velocity.Y = 1;
                this.Facing = Direction.DOWN;
			}
            else if (keyboard.IsKeyDown(Keys.A) && this.movementPressedBuffer[0] == Direction.LEFT)
			{
				this.Velocity.X = -1;
                this.Facing = Direction.LEFT;
			}
            else if (keyboard.IsKeyDown(Keys.D) && this.movementPressedBuffer[0] == Direction.RIGHT)
			{
				this.Velocity.X = 1;
                this.Facing = Direction.RIGHT;
			}
			this.Velocity.Normalize();
            this.Velocity *= Speed;
		}
        // first two items in buffer are not up & down or left and right
        else if ((int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] != 1 && (int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] != 5)
		{
			if (keyboard.IsKeyDown(Keys.W) && (this.movementPressedBuffer[0] == Direction.UP || this.movementPressedBuffer[1] == Direction.UP))
			{
				this.Velocity.Y = -1;
			}
			if (keyboard.IsKeyDown(Keys.S) && (this.movementPressedBuffer[0] == Direction.DOWN || this.movementPressedBuffer[1] == Direction.DOWN))
			{
				this.Velocity.Y = 1;
			}
            if (keyboard.IsKeyDown(Keys.A) && (this.movementPressedBuffer[0] == Direction.LEFT || this.movementPressedBuffer[1] == Direction.LEFT))
			{
				this.Velocity.X = -1;
			}
            if (keyboard.IsKeyDown(Keys.D) && (this.movementPressedBuffer[0] == Direction.RIGHT || this.movementPressedBuffer[1] == Direction.RIGHT))
			{
				this.Velocity.X = 1;
			}
			this.Velocity.Normalize();
            this.Velocity *= Speed;

            //el-ifs so we only evaluate once
            if(this.Facing == Direction.UP && this.Velocity.Y > 0 || this.Facing == Direction.DOWN && this.Velocity.Y < 0)
            {
                this.Facing = this.Velocity.X > 0 ? Direction.RIGHT : Direction.LEFT;
            }
            else if((this.Facing == Direction.RIGHT && this.Velocity.X < 0) || this.Facing == Direction.LEFT && this.Velocity.X > 0)
            {
                this.Facing = this.Velocity.Y > 0 ? Direction.DOWN : Direction.UP;
            }
		}
	}

    private void HandleCollision(CollisionGroups colG, CollisionProperties colP, Vector2 anchor, int height, int width)
    {
        if(colP.HasFlag(CollisionProperties.BLOCKING)){
            this.Position = CollisionUtil.HandleBlocking(this.Hitbox, this.Velocity, anchor, height, width);
            this.Hitbox.Anchor = this.Position;
        }
        if(colP.HasFlag(CollisionProperties.DOESDAMGETOPROTAG)){
            //this.Velocity = this.Position - anchor * damageImpulse;
            //this.currentCourtesyFrame = 0;
        }
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
        if(this.currentCourtesyFrame < courtesyFrames)
        {
            this.currentCourtesyFrame ++;
        }
        this.currentSprite.Update(gameTime);
        this.Hitbox.Anchor = this.Position;
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