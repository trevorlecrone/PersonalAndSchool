
using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Collision;

public static class CollisionUtil
{
    public static Vector2 HandleBlocking(CollisionRectangle r, Vector2 velocity, Vector2 blockAnchor, int blockHeight, int blockWidth)
    {
        var blockTop = blockAnchor.Y - blockHeight / 2;
        var blockBottom = blockAnchor.Y + blockHeight / 2;
        var blockLeft = blockAnchor.X - blockWidth / 2;
        var blockRight = blockAnchor.X + blockWidth / 2;

        var corY = 0.0f;
        var corX = 0.0f;

        if(r.Top() <= blockBottom && r.Bottom() > blockBottom)
        {
            corY = blockBottom - r.Top();
        }
        else if(r.Bottom() >= blockTop && r.Top() < blockTop)
        {
            corY = blockTop - r.Bottom();
        }

        if(r.Left() <= blockRight && r.Right() > blockRight)
        {
            corX = blockRight - r.Left();
        }
        else if(r.Right() >= blockLeft && r.Left() < blockLeft)
        {
            corX = blockLeft - r.Right();
        }

        Vector2 correction = velocity * -1;

        float magX = corX * corX;
        float magY = corY * corY;
        if ((magX < magY && magX > 0) || (magX > magY && magY == 0))
        {
            correction = new Vector2(corX * 1.05f, 0.0f);
        }
        else if (magY > 0)
        {
            correction = new Vector2(0.0f, corY* 1.05f);

        }

        return correction;
    }

    public static CollisionGroups groundedMask = ~CollisionGroups.GROUNDED;
}