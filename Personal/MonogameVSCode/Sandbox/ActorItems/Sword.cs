using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Graphics;

namespace Sandbox;

public class Sword : Item
{
    // Animation Objects
     private Dictionary<Direction, AnimatedSprite> spriteTree = new Dictionary<Direction, AnimatedSprite>();
    private AnimatedSprite currentSprite;
    private Dictionary<Direction, float> facingMap = new Dictionary<Direction, float>
    {
        {Direction.LEFT, 0.0f},
        {Direction.UP, (float)Math.PI/2},
        {Direction.RIGHT, 0.0f},
        {Direction.DOWN, (float)(Math.PI*1.5)},

    };

    private Dictionary<Direction, SpriteEffects> facingEffectsMap = new Dictionary<Direction, SpriteEffects>
    {
        {Direction.LEFT, SpriteEffects.None},
        {Direction.UP, SpriteEffects.None},
        {Direction.RIGHT, SpriteEffects.FlipHorizontally},
        {Direction.DOWN, SpriteEffects.None},

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
            this.Width+5,
            this.HandleCollision,
            1);
        this.currentSprite.Rotation = this.facingMap[this.Facing];
        this.currentSprite.Effects = this.facingEffectsMap[this.Facing];
        
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
        this.spriteTree.Add(Direction.UP, atlas.CreateCenteredAnimatedSprite("sword-attack-up"));
        this.spriteTree.Add(Direction.DOWN, atlas.CreateCenteredAnimatedSprite("sword-attack-down"));
        this.spriteTree.Add(Direction.LEFT,  atlas.CreateCenteredAnimatedSprite("sword-attack-left"));
        this.spriteTree.Add(Direction.RIGHT, atlas.CreateCenteredAnimatedSprite("sword-attack-right"));
    }

    public void Update(GameTime gameTime)
    {
        this.currentSprite.Update(gameTime);
        this.Hitbox.Anchor = this.HitBoxCenter();
    }
    public void SetFacing(Direction facing)
    {
        this.Facing = facing;
        this.currentSprite = this.spriteTree[this.Facing];
        this.currentSprite.Rotation = this.facingMap[this.Facing];
        this.currentSprite.Effects = this.facingEffectsMap[this.Facing];
    }

    public void ResetSprite()
    {
        this.currentSprite.Reset();
    }

    public Vector2 HitBoxCenter()
    {
        return new Vector2(this.Position.X + this.Width/2, this.Position.Y + this.Height/2);
    }

    public void Draw(SpriteBatch sb)
    {
        this.currentSprite.Draw(sb, this.Position + this.currentSprite.GetOffset());
    }
}
