using System;
using System.Collections.Generic;

namespace MonoGameLibrary.Collision;

public class CollisionChecker
{
    public List<CollisionRectangle> CollisionRects;

    public CollisionChecker()
    {
        this.CollisionRects = new List<CollisionRectangle>();
    }

    public CollisionChecker(List<CollisionRectangle> _collisionRects)
    {
        this.CollisionRects = _collisionRects;
    }

    public void Clear ()
    {
        this.CollisionRects.Clear();
    }

    public void Add (CollisionRectangle _rect)
    {
        this.CollisionRects.Add(_rect);
    }

    public void Remove (CollisionRectangle _rect)
    {
        this.CollisionRects.Remove(_rect);
    }

    public void DetectCollisions()
    {
        var offset = 1;
        for (int i = 0; i < (CollisionRects.Count == 0 ? CollisionRects.Count : CollisionRects.Count - 1); i++) {
            CollisionRectangle r1 = CollisionRects[i];
            for (int j = offset; j < CollisionRects.Count; j++) {
                CollisionRectangle r2 = CollisionRects[j];
                if (RectanglesCollide(r1, r2)) {
                    if(!r1.CollisionGroups.HasFlag(CollisionGroups.ACTIONLESS)) {
                        r1.OnCollide(r2.CollisionGroups, r2.CollisionProperties, r2.Anchor, r2.Height, r2.Width);
                    }
                    if(!r2.CollisionGroups.HasFlag(CollisionGroups.ACTIONLESS)) {
                        r2.OnCollide(r1.CollisionGroups, r1.CollisionProperties, r1.Anchor, r1.Height, r1.Width);
                    }
                }
            }
            offset++;
    }
    }

    private bool RectanglesCollide(CollisionRectangle r1, CollisionRectangle r2) {
        if (Math.Abs(r1.Anchor.X - r2.Anchor.X) > (r1.Width + r2.Width) / 2) 
        {
            return false;
        }
        if (Math.Abs(r1.Anchor.Y - r2.Anchor.Y) > (r1.Height + r2.Height) / 2) 
        {
            return false;
        }
        return true;
    }

}