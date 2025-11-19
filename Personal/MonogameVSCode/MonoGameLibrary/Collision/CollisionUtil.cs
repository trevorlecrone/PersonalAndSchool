
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
            magY = blockBottom - r.Top();
        }
        else if(r.Bottom() >= blockTop && r.Top() < blockTop)
        {
            magY = r.Bottom() - blockTop;
        }

        if(r.Left() <= blockRight && r.Right() > blockRight)
        {
            magX = blockRight - r.Left();
        }
        else if(r.Right() >= blockLeft && r.Left() < blockLeft)
        {
            magX = r.Right() - blockLeft;
        }

        var velocityScalar = 1.0f;
        Vector2 correction = velocity * -1;
        if(magX > magY && (velocity.X * velocity.X) > 0)
        {
            velocityScalar = magX/velocity.X * 1.05f;
            velocityScalar *= velocityScalar > 0 ? -1 : 1;
            correction = new Vector2(velocity.X * velocityScalar, 0.0f);
        }
        else if ((velocity.Y * velocity.Y) > 0)
        {
            velocityScalar = magY/velocity.Y * 1.05f;
            velocityScalar *= velocityScalar > 0 ? -1 : 1;
            correction = new Vector2(0.0f, velocity.Y * velocityScalar);

        }

        return r.Anchor + correction;
    }
}