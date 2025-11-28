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
    private Dictionary<Direction, AnimatedSprite> airbornSpriteTree = new Dictionary<Direction, AnimatedSprite>();
    private AnimatedSprite currentSprite;
    private Vector2 animationOffset = new Vector2(0,0);
    public int Height;
    public int Width;

    // Movement
    public const int Speed = 5;
    public const int FastSpeed = 7;
    public Vector2 Position;
    public Vector2 Velocity;
    private Vector2 impulseVel;
    public bool Sprint = false;
    public Direction Facing = Direction.DOWN;
    public CollisionProperties CollisionProperties = new CollisionProperties() | CollisionProperties.BLOCKING;
    public CollisionGroups CollisionGroups = new CollisionGroups() | CollisionGroups.GROUNDED;
    public CollisionRectangle Hitbox = new CollisionRectangle(1);
    private const int damageImpulse = 10;
    private bool facingLocked = false;
    private bool airborn = false;
	private List<Direction> movementPressedBuffer = new List<Direction>();

    //Actions
	private const int jumpFrames = 30;
	private const int AttackFrames = 33;
	private const int AttackInterruptibleAfter = 22;
    private const int courtesyFrames = 44;
    private const int damageImpulseFrames = 8;
	private int currentActionFrame = 0;
    private int currentImpulseFrame = damageImpulseFrames;
    private int currentCourtesyFrame = courtesyFrames;
    private int currentJumpFrame = jumpFrames;
	private bool inAttack = false;
	private bool interruptible = false;
    private List<int> actionBuffer = new List<int>();
	public List<string> ImmuneGroups = new List<string>();

    public Protag (TextureAtlas atlas, Vector2 position)
    {
        this.ConstructSpriteTrees(atlas);
        this.Position = position;
        this.Velocity = new Vector2(0,0);
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
        if (keyboard.KeyPressed(Keys.Space) && this.currentJumpFrame == jumpFrames)
        {
           this.airborn = true;
           this.currentJumpFrame = 0;
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
        

        this.EvaluateFacingAndVelocity_KeyBoard(keyboard);
	}

    public void HandleMovement()
	{
		if (!inAttack || interruptible)
		{
            this.Position = this.Position + this.Velocity;
            this.Hitbox.Anchor = this.Center();
		}
	}

    public void ChooseSprite()
	{
        if(this.airborn)
        {
            this.currentSprite = this.airbornSpriteTree[this.Facing];
            return;
        }

        this.currentSprite = this.spriteTree[(this.Facing, this.Velocity.Length() > 0)];
	}

    private void EvaluateFacingAndVelocity_KeyBoard(KeyboardInfo keyboard)
	{
        var inputVel = new Vector2(0,0);
        if (this.movementPressedBuffer.Count < 2 || (int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] == 1 || (int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] == 5)
		{
			if (keyboard.IsKeyDown(Keys.W) && this.movementPressedBuffer[0] == Direction.UP)
			{
				inputVel.Y = -1;
                this.Facing = Direction.UP;
			}
			else if (keyboard.IsKeyDown(Keys.S) && this.movementPressedBuffer[0] == Direction.DOWN)
			{
				inputVel.Y = 1;
                this.Facing = Direction.DOWN;
			}
            else if (keyboard.IsKeyDown(Keys.A) && this.movementPressedBuffer[0] == Direction.LEFT)
			{
				inputVel.X = -1;
                this.Facing = Direction.LEFT;
			}
            else if (keyboard.IsKeyDown(Keys.D) && this.movementPressedBuffer[0] == Direction.RIGHT)
			{
				inputVel.X = 1;
                this.Facing = Direction.RIGHT;
			}
            if(inputVel.Length() > 0){
                inputVel.Normalize();
                inputVel *= Speed;
            }
		}
        // first two items in buffer are not up & down or left and right
        else if ((int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] != 1 && (int)this.movementPressedBuffer[0] + (int)this.movementPressedBuffer[1] != 5)
		{
			if (keyboard.IsKeyDown(Keys.W) && (this.movementPressedBuffer[0] == Direction.UP || this.movementPressedBuffer[1] == Direction.UP))
			{
				inputVel.Y = -1;
			}
			if (keyboard.IsKeyDown(Keys.S) && (this.movementPressedBuffer[0] == Direction.DOWN || this.movementPressedBuffer[1] == Direction.DOWN))
			{
				inputVel.Y = 1;
			}
            if (keyboard.IsKeyDown(Keys.A) && (this.movementPressedBuffer[0] == Direction.LEFT || this.movementPressedBuffer[1] == Direction.LEFT))
			{
				inputVel.X = -1;
			}
            if (keyboard.IsKeyDown(Keys.D) && (this.movementPressedBuffer[0] == Direction.RIGHT || this.movementPressedBuffer[1] == Direction.RIGHT))
			{
				inputVel.X = 1;
			}
			if(inputVel.Length() > 0){
                inputVel.Normalize();
                inputVel *= Speed;
            }

            //el-ifs so we only evaluate once
            if(this.Facing == Direction.UP && inputVel.Y > 0 || this.Facing == Direction.DOWN && inputVel.Y < 0)
            {
                this.Facing = inputVel.X > 0 ? Direction.RIGHT : Direction.LEFT;
            }
            else if((this.Facing == Direction.RIGHT && inputVel.X < 0) || this.Facing == Direction.LEFT && inputVel.X > 0)
            {
                this.Facing = inputVel.Y > 0 ? Direction.DOWN : Direction.UP;
            }
		}

        this.Velocity = inputVel;
        
	}

    private void HandleCollision(CollisionGroups colG, CollisionProperties colP, Vector2 anchor, int height, int width)
    {
        if(colP.HasFlag(CollisionProperties.BLOCKING)){
            this.Position += CollisionUtil.HandleBlocking(this.Hitbox, this.Velocity, anchor, height, width);
            this.Hitbox.Anchor = this.Center();
        }
        if(colP.HasFlag(CollisionProperties.DOESDAMGETOPROTAG)){
            if (this.currentCourtesyFrame == courtesyFrames)
            {
                this.currentCourtesyFrame = 0;
                this.currentImpulseFrame = 0;
                this.impulseVel = this.Position - anchor;
                this.impulseVel.Normalize();
                this.impulseVel *= damageImpulse;
            }
        }
        return;
    }

    private void ConstructSpriteTrees(TextureAtlas atlas)
    {
        this.spriteTree.Add((Direction.UP, false), atlas.CreateAnimatedSprite("idle-up"));
        this.spriteTree.Add((Direction.UP, true), atlas.CreateAnimatedSprite("walk-up"));
        this.spriteTree.Add((Direction.DOWN, false), atlas.CreateAnimatedSprite("idle-down"));
        this.spriteTree.Add((Direction.DOWN, true), atlas.CreateAnimatedSprite("walk-down"));
        this.spriteTree.Add((Direction.LEFT, false), atlas.CreateAnimatedSprite("idle-left"));
        this.spriteTree.Add((Direction.LEFT, true), atlas.CreateAnimatedSprite("walk-left"));
        this.spriteTree.Add((Direction.RIGHT, false), atlas.CreateAnimatedSprite("idle-right"));
        this.spriteTree.Add((Direction.RIGHT, true), atlas.CreateAnimatedSprite("walk-right"));

        this.airbornSpriteTree.Add(Direction.UP, atlas.CreateAnimatedSprite("jump-up"));
        this.airbornSpriteTree.Add(Direction.DOWN, atlas.CreateAnimatedSprite("jump-down"));
        this.airbornSpriteTree.Add(Direction.LEFT, atlas.CreateAnimatedSprite("jump-left"));
        this.airbornSpriteTree.Add(Direction.RIGHT, atlas.CreateAnimatedSprite("jump-right"));
    }

    public void Update(GameTime gameTime)
    {
        if (this.currentCourtesyFrame < courtesyFrames)
        {
            this.currentCourtesyFrame++;
        }

        if (this.currentImpulseFrame < damageImpulseFrames)
        {
            this.Velocity += this.impulseVel;
            this.currentImpulseFrame++;
        }
        else
        {
            this.impulseVel = new Vector2(0,0);
        }

        if (this.currentJumpFrame < jumpFrames)
        {
            if (this.currentJumpFrame < 10)
            {
                this.animationOffset.Y -= 6;
            }
            if (this.currentJumpFrame > 20)
            {
                this.animationOffset.Y += 6;
            }
            this.currentJumpFrame++;
        }
        else
        {
            this.airborn = false;
            this.animationOffset = new Vector2(0,0);
        }
        
        this.ChooseSprite();
        this.HandleMovement();
        this.currentSprite.Update(gameTime);
        this.Hitbox.Anchor = this.Center();
    }

    public void Draw(SpriteBatch sb)
    {
        this.currentSprite.Draw(sb, this.Position + this.animationOffset);
    }

    public Vector2 Center()
    {
        return new Vector2(this.Position.X + this.Width/2, this.Position.Y + this.Height/2);
    }

    public void CheckGamepadInput()
	{
		// Get a reference to the keyboard inof
        KeyboardInfo keyboard = Core.Input.Keyboard;
	}

}