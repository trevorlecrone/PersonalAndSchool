using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
namespace MonoGameLibrary.Collision;

public class CollisionRectangle : ICollidable
{
    public CollisionGroups CollisionGroups { get; set; }
    public CollisionProperties CollisionProperties { get; set; }
    public Vector2 Anchor { get; set; }
    public Sprite DebugSprite;

    public Action<CollisionGroups, CollisionProperties, Vector2, int, int> OnCollide { get; set; }
    public int Height;
    public int Width;
    public int Id { get; }

    public CollisionRectangle(int _id)
    {
        this.CollisionGroups = new CollisionGroups() | CollisionGroups.ACTIONLESS;
        this.CollisionProperties = new CollisionProperties();
        this.Anchor = new Vector2();
        this.Height = 1;
        this.Width = 1;
        this.DebugSprite = new Sprite(DebugConsts.DEBUG_WHITE);
        this.OnCollide = (CollisionGroups colG, CollisionProperties colP, Vector2 anchor, int height, int width) => { return; };
        this.Id = _id;
    }

    public CollisionRectangle(
        CollisionGroups _collisionGroups,
        CollisionProperties _collisionProperties,
        Vector2 _anchor,
        int _height,
        int _width,
        Action<CollisionGroups, CollisionProperties, Vector2, int, int> _onCollide,
        int _id)
    {
        this.CollisionGroups = _collisionGroups;
        this.CollisionProperties = _collisionProperties;
        this.Anchor = _anchor;
        this.Height = _height;
        this.Width = _width;
        this.DebugSprite = new Sprite(DebugConsts.DEBUG_WHITE);
        this.DebugSprite.Scale = new Vector2(this.Width, this.Height);
        this.DebugSprite.Color = Color.Blue * 0.5f;
        this.OnCollide = _onCollide;
        this.Id = _id;
    }

    public float Top()
    {
        return this.Anchor.Y - Height/2;
    }

    public float Bottom()
    {
        return this.Anchor.Y + Height/2;
    }

    public float Left()
    {
        return this.Anchor.X - Width/2;
    }

    public float Right()
    {
        return this.Anchor.X + Width/2;
    }
}