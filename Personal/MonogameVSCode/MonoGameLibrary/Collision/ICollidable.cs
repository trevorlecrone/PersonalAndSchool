using System;
using System.Data.Common;
using Microsoft.Xna.Framework;
namespace MonoGameLibrary.Collision;

public interface ICollidable
{
    /// <summary>
    /// Defines the groups this collision object belongs to
    /// </summary>
    CollisionGroups CollisionGroups
    {
        get;
        set;
    }

    /// <summary>
    /// Defines the properties of the collision object
    /// </summary>
    CollisionProperties CollisionProperties
    {
        get;
        set;
    }

    /// <summary>
    /// The anochor point of the object, always the center
    /// </summary>
    Vector2 Anchor
    {
        get;
        set;
    }

    /// <summary>
    /// Passed in by parent, the function that should be called when this object collides with another
    /// </summary>
     Action<CollisionGroups, CollisionProperties, Vector2, int, int> OnCollide
    {
        get;
        set;
    }

    /// <summary>
    /// ID of the collision object, for debugging and action triggers
    /// </summary>
    int Id
    {
        get;
    }

}