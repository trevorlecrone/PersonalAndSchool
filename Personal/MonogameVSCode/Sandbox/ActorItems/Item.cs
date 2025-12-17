using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandbox;

public interface Item
{
    void Update(GameTime gameTime);
    void ResetSprite();
    void Draw(SpriteBatch sb);
    void SetFacing(Direction sb);
    void SetPosition(Vector2 position);
}