using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Graphics;

namespace Sandbox;

public class Bat
{

    // Animation Objects
    private AnimatedSprite sprite;
    public int Height;
    public int Width;

    // Movement
    public const float Speed = 5.0f;
    public Vector2 Position;
    public Vector2 Velocity;

    public CollisionProperties CollisionProperties = new CollisionProperties() | CollisionProperties.DOESDAMGETOPROTAG;
    public CollisionGroups CollisionGroups = new CollisionGroups() | CollisionGroups.GROUNDED | CollisionGroups.AIRBORN;
    public CollisionRectangle Hitbox = new CollisionRectangle(6);
    

    public Bat (TextureAtlas atlas, Vector2 position)
    {
        this.sprite = this.ConstructSprite(atlas);
        this.Position = position;
        this.Height = (int)this.sprite.Height;
        this.Width = (int)this.sprite.Width;
        this.Hitbox = new CollisionRectangle(
            this.CollisionGroups,
            this.CollisionProperties,
            this.Position,
            this.Height,
            this.Width,
            this.HandleCollision,
            1);
    }
    public void AssignRandomVelocity()
    {
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);

        this.Velocity = direction * Speed;
    }

     private void HandleCollision(CollisionGroups colG, CollisionProperties colP, Vector2 anchor, int height, int width)
    {
        if(colP.HasFlag(CollisionProperties.BLOCKING) && colG.HasFlag(CollisionGroups.AIRBORN)){
            this.Position = CollisionUtil.HandleBlocking(this.Hitbox, this.Velocity, anchor, height, width);
            Vector2 normal = this.Position - this.Hitbox.Anchor;
            normal.Normalize();
            this.Velocity = Vector2.Reflect(Velocity, normal);
            this.Hitbox.Anchor = this.Position;
        }
    }

    private AnimatedSprite ConstructSprite(TextureAtlas atlas)
    {
        var sprite = atlas.CreateAnimatedSprite("bat-animation");
        sprite.Scale = new Vector2(4.0f, 4.0f);
        return sprite;
    }

    public void Update(GameTime gameTime)
    {
        this.Position += this.Velocity;
        this.sprite.Update(gameTime);
        this.Hitbox.Anchor = this.Position;
    }

    public void Draw(SpriteBatch sb)
    {
        this.sprite.Draw(sb, this.Position);
    }

}
