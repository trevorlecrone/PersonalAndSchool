
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

        var magY = 0.0f;
        var magX = 0.0f;

        if(r.Top() <= blockBottom && r.Bottom() > blockBottom)
        {
            magY = Math.Abs(blockBottom - r.Top());
        }
        else if(r.Bottom() >= blockTop && r.Top() < blockTop)
        {
            magY = Math.Abs(r.Bottom() - blockTop);
        }

        if(r.Left() <= blockRight && r.Right() > blockRight)
        {
            magX = Math.Abs(blockRight - r.Left());
        }
        else if(r.Right() >= blockLeft && r.Left() < blockLeft)
        {
            magX = Math.Abs(r.Right() - blockLeft);
        }

        Vector2 correction = velocity * -1;

        float velocityScalar;
        if ((magX < magY && magX > 0) || (magX > magY && magY == 0))
        {
            velocityScalar = magX / velocity.X * 1.05f;
            velocityScalar *= velocityScalar > 0 ? -1 : 1;
            correction = new Vector2(velocity.X * velocityScalar, 0.0f);
        }
        else if ((velocity.Y * velocity.Y) > 0)
        {
            velocityScalar = magY / velocity.Y * 1.05f;
            velocityScalar *= velocityScalar > 0 ? -1 : 1;
            correction = new Vector2(0.0f, velocity.Y * velocityScalar);

        }

        return r.Anchor + correction;
    }
}