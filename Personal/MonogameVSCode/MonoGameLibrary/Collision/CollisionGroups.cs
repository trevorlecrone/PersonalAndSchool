using System;

namespace MonoGameLibrary.Collision;

/// <summary>
/// Defines layers and groups of collidable objects. 
/// Used to allow jumping over enemies, allow enemies to walk through one another etc
/// </summary>
[Flags]
public enum CollisionGroups
{
    //loewst 3 bits for layers
    GROUNDED = 0x1,
    AIRBORN = 0x2,
    ACTIONLESS = 0x4
}