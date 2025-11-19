using System;

namespace MonoGameLibrary.Collision;

[Flags]
public enum CollisionProperties
{ 
    BLOCKING = 0x1,
    ACTION = 0x2,
    ONEWAY = 0x4,
    DOESDAMGE = 0x8,
    TAKESDAMAGE = 0x16
}