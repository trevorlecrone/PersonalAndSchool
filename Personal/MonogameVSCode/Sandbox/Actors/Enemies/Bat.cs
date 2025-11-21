using Microsoft.Xna.Framework;
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
    public Vector2 Position;
    public Vector2 Velocity;

    public CollisionProperties CollisionProperties = new CollisionProperties() | CollisionProperties.DOESDAMGETOPROTAG | CollisionProperties.BLOCKING;
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

     private void HandleCollision(CollisionGroups colG, CollisionProperties colP, Vector2 anchor, int height, int width)
    {
        if(colP.HasFlag(CollisionProperties.BLOCKING) && colG.HasFlag(CollisionGroups.AIRBORN)){
            this.Position = CollisionUtil.HandleBlocking(this.Hitbox, this.Velocity, anchor, height, width);
            Vector2 normal = this.Position - this.Hitbox.Anchor;
            this.Velocity = Vector2.Reflect(Velocity, normal);
            this.Hitbox.Anchor = this.Position;
        }
    }

    private AnimatedSprite ConstructSprite(TextureAtlas atlas)
    {
        return atlas.CreateAnimatedSprite("bat-animation");
    }

}
