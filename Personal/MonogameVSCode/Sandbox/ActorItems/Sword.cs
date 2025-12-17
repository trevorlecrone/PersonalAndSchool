using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Graphics;

namespace Sandbox;

public class Sword : Item
{
    // Animation Objects
    private Dictionary<Direction, AnimatedSprite> spriteTree = new Dictionary<Direction, AnimatedSprite>();
    private AnimatedSprite currentSprite;
    private TextureRegion currentFrame;
    private Vector2 currentOffset;
    private Dictionary<Direction, SpriteEffects> facingEffectsMap = new Dictionary<Direction, SpriteEffects>
    {
        {Direction.LEFT, SpriteEffects.None},
        {Direction.UP, SpriteEffects.FlipHorizontally},
        {Direction.RIGHT, SpriteEffects.FlipHorizontally},
        {Direction.DOWN, SpriteEffects.FlipVertically},

    };
    public Direction Facing = Direction.DOWN;
    public int Height;
    public int Width;

    // Movement
    public Vector2 Position;

    // Collision
    public CollisionProperties CollisionProperties = new CollisionProperties() | CollisionProperties.DOESDAMAGETOENEMIES;
    public CollisionGroups CollisionGroups = CollisionGroups.GROUNDED | CollisionGroups.AIRBORN | CollisionGroups.ACTIONLESS;
    public CollisionRectangle Hitbox = new CollisionRectangle(10);
    

    public Sword (TextureAtlas atlas, Vector2 position, Direction facing)
    {
        // Pre-Initialize as single instance and then only have to pass position and direction
        this.ConstructSpriteTrees(atlas);
        this.Position = position;
        this.Facing = facing;
        this.currentSprite = this.spriteTree[this.Facing];
        this.Height = (int)this.currentSprite.Height;
        this.Width = (int)this.currentSprite.Width;
        this.Hitbox = new CollisionRectangle(
            this.CollisionGroups,
            this.CollisionProperties,
            this.Position,
            this.Height+10,
            this.Width+15,
            this.HandleCollision,
            55);
        this.currentSprite.Effects = this.facingEffectsMap[this.Facing];
        this.Hitbox.DebugSprite.Color = Color.Red * 0.5f;
        
    }
    
     private void HandleCollision(CollisionGroups colG, CollisionProperties colP, Vector2 anchor, int height, int width)
    {
        return;
    }

    private AnimatedSprite ConstructSprite(TextureAtlas atlas)
    {
        var sprite = atlas.CreateAnimatedSprite("sword-attack");
        return sprite;
    }

    private void ConstructSpriteTrees(TextureAtlas atlas)
    {
        this.spriteTree.Add(Direction.UP, atlas.CreateAnimatedSprite("sword-attack-up"));
        this.spriteTree.Add(Direction.DOWN, atlas.CreateAnimatedSprite("sword-attack-down"));
        this.spriteTree.Add(Direction.LEFT,  atlas.CreateAnimatedSprite("sword-attack-left"));
        this.spriteTree.Add(Direction.RIGHT, atlas.CreateAnimatedSprite("sword-attack-right"));
    }

    public void Update(GameTime gameTime)
    {
        this.currentSprite.Update(gameTime);
        this.currentFrame = this.currentSprite.Animation.Frames[this.currentSprite._currentFrame];
        this.currentOffset = this.currentSprite.Animation.Offsets[this.currentSprite._currentFrame];
        (int width, int height, int xOffset, int yOffset) hitboxTuple;
        if (this.currentSprite.Animation.HitboxInfo.TryGetValue(this.currentSprite._currentFrame, out hitboxTuple))
        {
            Core.Collision.Remove(this.Hitbox);
            this.Hitbox.SetHeight(hitboxTuple.height);
            this.Hitbox.SetWidth(hitboxTuple.width);
            this.Hitbox.Anchor = this.HitBoxCenter(hitboxTuple);
            Core.Collision.Add(this.Hitbox);
        }
    }
    public void SetFacing(Direction facing)
    {
        this.Facing = facing;
        this.currentSprite = this.spriteTree[this.Facing];
        this.currentSprite.Effects = this.facingEffectsMap[this.Facing];
    }

    public void SetPosition(Vector2 position)
    {
        this.Position = position;
    }

    public void ResetSprite()
    {
        Core.Collision.Remove(this.Hitbox);
        this.currentSprite.Reset();
    }

    public Vector2 HitBoxCenter((int width, int height, int xOffset, int yOffset) hitboxTuple)
    {
        var x =  this.Position.X + this.currentOffset.X + this.currentFrame.Width/2 + hitboxTuple.xOffset;
        var y =  this.Position.Y + this.currentOffset.Y + this.currentFrame.Height/2 + hitboxTuple.yOffset;
        return new Vector2(x,y);
    }

    public void Draw(SpriteBatch sb)
    {
        this.currentSprite.Draw(sb, this.Position + this.currentSprite.GetOffset());
    }
}
