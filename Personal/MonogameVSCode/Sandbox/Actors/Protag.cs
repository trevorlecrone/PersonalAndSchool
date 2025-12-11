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

    //--------Animation Objects-------------
    // constants
    
    // externals
    public int Height;
    public int Width;

    // internals
    private Dictionary<(Direction, ProtagState), AnimatedSprite> spriteTree = new Dictionary<(Direction, ProtagState), AnimatedSprite>();
    private AnimatedSprite CurrentSprite;
    private ProtagState CurrentState;
    private Vector2 animationOffset = new Vector2(0,0);
    private bool facingLocked = false;

    //--------Movement-------------
    // constants
    public const int Speed = 5;
    public const int FastSpeed = 7;

    // externals
    public Vector2 Position;
    public Vector2 Velocity;

    // internals
    private Vector2 impulseVel;
    public bool Sprint = false;
    public Direction Facing = Direction.DOWN;

    //--------Collision-------------
    public CollisionProperties CollisionProperties = new CollisionProperties() | CollisionProperties.BLOCKING;
    public CollisionGroups CollisionGroups = new CollisionGroups() | CollisionGroups.GROUNDED;
    public CollisionRectangle Hitbox = new CollisionRectangle(1);
    
    
	private List<Direction> movementPressedBuffer = new List<Direction>();

    //--------Actions-------------
    // constants
    private const int damageImpulse = 8;
	private const int jumpFrames = 30;
	private const int AttackFrames = 21;
	private const int AttackInterruptibleAfter = 22;
    private const int courtesyFrames = 44;
    private const int damageImpulseFrames = 10;

    // externals
    
    // intenals
	private int currentActionFrame = 0;
    private int currentImpulseFrame = damageImpulseFrames;
    private int currentCourtesyFrame = courtesyFrames;
    private int currentJumpFrame = jumpFrames;
	private bool inAttack = false;
	private bool interruptible = false;
    private bool resetSprite = false;
    private List<int> actionBuffer = new List<int>();

    public Protag (TextureAtlas atlas, Vector2 position)
    {
        this.ConstructSpriteTrees(atlas);
        this.Position = position;
        this.Velocity = new Vector2(0,0);
        this.CurrentSprite = this.spriteTree[(Direction.DOWN, ProtagState.IDLE)];
        this.Height = (int)this.CurrentSprite.Height;
        this.Width = (int)this.CurrentSprite.Width;
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
           this.currentJumpFrame = 0;
           this.Hitbox.CollisionGroups &= CollisionUtil.groundedMask;
        }
        if (keyboard.KeyPressed(Keys.E))
        {
           this.inAttack = true;
           this.resetSprite = true;
           this.currentActionFrame = 0;
           this.Velocity = new Vector2(0,0);
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
        this.Position = this.Position + this.Velocity;
        this.Hitbox.Anchor = this.Center();
	}

    public void ChooseSprite()
	{
        this.CurrentSprite = this.spriteTree[(this.Facing, this.CurrentState)];
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

        if (!inAttack)
		{
            this.Velocity = inputVel;
            this.CurrentState = Velocity.Length() > 0 ? ProtagState.MOVING : ProtagState.IDLE;
        }
        
	}

    private void HandleCollision(CollisionGroups colG, CollisionProperties colP, Vector2 anchor, int height, int width)
    {
        var commonLayer = (this.Hitbox.CollisionGroups & colG).HasFlag(CollisionGroups.GROUNDED) || (this.Hitbox.CollisionGroups & colG).HasFlag(CollisionGroups.AIRBORN);
        if(colP.HasFlag(CollisionProperties.BLOCKING) && commonLayer){
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
        this.spriteTree.Add((Direction.UP, ProtagState.IDLE), atlas.CreateAnimatedSprite("idle-up"));
        this.spriteTree.Add((Direction.DOWN, ProtagState.IDLE), atlas.CreateAnimatedSprite("idle-down"));
        this.spriteTree.Add((Direction.LEFT, ProtagState.IDLE), atlas.CreateAnimatedSprite("idle-left"));
        this.spriteTree.Add((Direction.RIGHT, ProtagState.IDLE), atlas.CreateAnimatedSprite("idle-right"));

        this.spriteTree.Add((Direction.UP, ProtagState.MOVING), atlas.CreateAnimatedSprite("walk-up"));
        this.spriteTree.Add((Direction.DOWN, ProtagState.MOVING), atlas.CreateAnimatedSprite("walk-down"));
        this.spriteTree.Add((Direction.LEFT, ProtagState.MOVING), atlas.CreateAnimatedSprite("walk-left"));
        this.spriteTree.Add((Direction.RIGHT, ProtagState.MOVING), atlas.CreateAnimatedSprite("walk-right"));

        this.spriteTree.Add((Direction.UP, ProtagState.AIRBORN), atlas.CreateAnimatedSprite("jump-up"));
        this.spriteTree.Add((Direction.DOWN, ProtagState.AIRBORN), atlas.CreateAnimatedSprite("jump-down"));
        this.spriteTree.Add((Direction.LEFT, ProtagState.AIRBORN), atlas.CreateAnimatedSprite("jump-left"));
        this.spriteTree.Add((Direction.RIGHT, ProtagState.AIRBORN), atlas.CreateAnimatedSprite("jump-right"));

        this.spriteTree.Add((Direction.UP, ProtagState.SWING), atlas.CreateAnimatedSprite("swing-up"));
        this.spriteTree.Add((Direction.DOWN, ProtagState.SWING), atlas.CreateAnimatedSprite("swing-down"));
        this.spriteTree.Add((Direction.LEFT, ProtagState.SWING), atlas.CreateAnimatedSprite("swing-left"));
        this.spriteTree.Add((Direction.RIGHT, ProtagState.SWING), atlas.CreateAnimatedSprite("swing-right"));
    }

    public void Update(GameTime gameTime)
    {
        this.CheckKeyboardInput();
        if (this.currentCourtesyFrame < courtesyFrames)
        {
            this.currentCourtesyFrame++;
        }

        if (this.currentImpulseFrame < damageImpulseFrames)
        {
            this.Velocity = this.impulseVel;
            this.currentImpulseFrame++;
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
            this.CurrentState = ProtagState.AIRBORN;
        }
        else
        {
            this.animationOffset = new Vector2(0,0);
            this.Hitbox.CollisionGroups |= CollisionGroups.GROUNDED;
        }

        if(this.inAttack)
        {
            this.CurrentState = ProtagState.SWING;
            this.currentActionFrame++;
            if(this.currentActionFrame == AttackFrames)
            {
                this.inAttack = false;
            }
        }
        
        this.ChooseSprite();
        this.HandleMovement();
        if (this.currentImpulseFrame == damageImpulseFrames)
        {
            this.impulseVel = new Vector2(0,0);
            this.Velocity = this.impulseVel;
        }
        this.CurrentSprite.Update(gameTime);
        if (this.resetSprite)
        {
            this.resetSprite = false;
            this.CurrentSprite.Reset();
        }
        this.Hitbox.Anchor = this.Center();
    }

    public void Draw(SpriteBatch sb)
    {
        this.CurrentSprite.Draw(sb, this.Position + this.animationOffset);
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