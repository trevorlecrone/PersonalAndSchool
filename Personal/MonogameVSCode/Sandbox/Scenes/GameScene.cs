using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;

namespace Sandbox.Scenes;

public class GameScene : Scene
{
    // Defines the Protagonist
    private Protag _protag;

    // Defines the bat animated sprite.
    private Bat _bat;

    // Speed multiplier when moving.
    private const float MOVEMENT_SPEED = 5.0f;

    // Defines the tilemap to draw.
    private Tilemap _tilemap;

    // Defines the bounds of the room that the slime and bat are contained within.
    private Rectangle _roomBounds;

    // Defines the top of the room
    private CollisionRectangle _roomTop;

    // Defines the bottom of the room
    private CollisionRectangle _roomBottom;

    // Defines the left of the room
    private CollisionRectangle _roomLeft;

    // Defines the right of the room
    private CollisionRectangle _roomRight;

    // the CollisionChecker
    private CollisionChecker _collisionChecker;

    // The sound effect to play when the bat bounces off the edge of the screen.
    private SoundEffect _bounceSoundEffect;

    // The sound effect to play when the slime eats a bat.
    private SoundEffect _collectSoundEffect;

    // The SpriteFont Description used to draw text
    private SpriteFont _font;

    // Tracks the players score.
    private int _score;

    // Defines the position to draw the score text at.
    private Vector2 _scoreTextPosition;

    // Defines the origin used when drawing the score text.
    private Vector2 _scoreTextOrigin;

    private bool DEBUG = true;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen
        Core.ExitOnEscape = false;

        Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;

        _roomBounds = new Rectangle(
            (int)_tilemap.TileWidth,
            (int)_tilemap.TileHeight,
            screenBounds.Width - (int)_tilemap.TileWidth * 2,
            screenBounds.Height - (int)_tilemap.TileHeight * 2
        );
        
        _roomTop = new CollisionRectangle(
            CollisionGroups.GROUNDED | CollisionGroups.ACTIONLESS | CollisionGroups.AIRBORN,
            CollisionProperties.BLOCKING,
            new Vector2(640, 80),
            80,
            1160,
            (CollisionGroups colG, CollisionProperties colP, Vector2 anchor, int height, int width) => { return; },
            2
        );

        _roomBottom = new CollisionRectangle(
            CollisionGroups.GROUNDED | CollisionGroups.ACTIONLESS | CollisionGroups.AIRBORN,
            CollisionProperties.BLOCKING,
            new Vector2(640, 680),
            80,
            1160,
            (CollisionGroups colG, CollisionProperties colP, Vector2 anchor, int height, int width) => { return; },
            3
        );

        _roomLeft = new CollisionRectangle(
            CollisionGroups.GROUNDED | CollisionGroups.ACTIONLESS | CollisionGroups.AIRBORN,
            CollisionProperties.BLOCKING,
            new Vector2(40, 360),
            560,
            80,
            (CollisionGroups colG, CollisionProperties colP, Vector2 anchor, int height, int width) => { return; },
            5
        );

        _roomRight = new CollisionRectangle(
            CollisionGroups.GROUNDED | CollisionGroups.ACTIONLESS | CollisionGroups.AIRBORN,
            CollisionProperties.BLOCKING,
            new Vector2(1240, 360),
            560,
            80,
            (CollisionGroups colG, CollisionProperties colP, Vector2 anchor, int height, int width) => { return; },
            4
        );

        _collisionChecker = new CollisionChecker();

        _collisionChecker.CollisionRects.Add(_roomTop);
        _collisionChecker.CollisionRects.Add(_roomBottom);
        _collisionChecker.CollisionRects.Add(_roomLeft);
        _collisionChecker.CollisionRects.Add(_roomRight);

        // Initial protagonist position will be the center tile of the tile map.
        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;
        _protag.Position = new Vector2(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);
        _protag.Hitbox.Anchor = _protag.Position;

        _collisionChecker.CollisionRects.Add(_protag.Hitbox);

        // Initial bat position will the in the top left corner of the room.
        _bat.Position = new Vector2(_roomLeft.Right() + 100, _roomTop.Bottom() + 100);

        // Set the position of the score text to align to the left edge of the
        // room bounds, and to vertically be at the center of the first tile.
        _scoreTextPosition = new Vector2(_roomBounds.Left, _tilemap.TileHeight * 0.5f);

        // Set the origin of the text so it is left-centered.
        float scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
        _scoreTextOrigin = new Vector2(0, scoreTextYOrigin);

        _bat.AssignRandomVelocity();

        _collisionChecker.CollisionRects.Add(_bat.Hitbox);
    }

    public override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file.
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        // Create the slime animated sprite from the atlas.
        _protag = new Protag(atlas, new Vector2());

        // Create the bat animated sprite from the atlas.
        _bat = new Bat(atlas, new Vector2());

        // Create the tilemap from the XML configuration file.
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);

        // Load the bounce sound effect.
        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Load the collect sound effect.
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        // Load the font.
        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");
    }

    public override void Update(GameTime gameTime)
    {

        // Update the bat.
        _bat.Update(gameTime);

        // Check for keyboard input and handle it.
        CheckSystemKeyboardInput();
        _protag.CheckKeyboardInput();

        // Check for gamepad input and handle it.
        CheckGamePadInput();

        _collisionChecker.DetectCollisions();

        // Update the protagonist.
        _protag.Update(gameTime);


        // // Calculate the new position of the bat based on the velocity.
        // Vector2 newBatPosition = _batPosition + _batVelocity;

        // Vector2 normal = Vector2.Zero;

        // // Use distance based checks to determine if the bat is within the
        // // bounds of the game screen, and if it is outside that screen edge,
        // // reflect it about the screen edge normal.
        // if (batBounds.Left < _roomBounds.Left)
        // {
        //     normal.X = Vector2.UnitX.X;
        //     newBatPosition.X = _roomBounds.Left;
        // }
        // else if (batBounds.Right > _roomBounds.Right)
        // {
        //     normal.X = -Vector2.UnitX.X;
        //     newBatPosition.X = _roomBounds.Right - _bat.Width;
        // }

        // if (batBounds.Top < _roomBounds.Top)
        // {
        //     normal.Y = Vector2.UnitY.Y;
        //     newBatPosition.Y = _roomBounds.Top;
        // }
        // else if (batBounds.Bottom > _roomBounds.Bottom)
        // {
        //     normal.Y = -Vector2.UnitY.Y;
        //     newBatPosition.Y = _roomBounds.Bottom - _bat.Height;
        // }

        // // If the normal is anything but Vector2.Zero, this means the bat had
        // // moved outside the screen edge so we should reflect it about the
        // // normal.
        // if (normal != Vector2.Zero)
        // {
        //     normal.Normalize();
        //     _batVelocity = Vector2.Reflect(_batVelocity, normal);

        //     // Play the bounce sound effect.
        //     Core.Audio.PlaySoundEffect(_bounceSoundEffect);
        // }

        // _batPosition = newBatPosition;

        // if (slimeBounds.Intersects(batBounds))
        // {
        //     // Choose a random row and column based on the total number of each
        //     int column = Random.Shared.Next(1, _tilemap.Columns - 1);
        //     int row = Random.Shared.Next(1, _tilemap.Rows - 1);

        //     // Change the bat position by setting the x and y values equal to
        //     // the column and row multiplied by the width and height.
        //     _batPosition = new Vector2(column * _bat.Width, row * _bat.Height);

        //     // Assign a new random velocity to the bat.
        //     AssignRandomBatVelocity();

        //     // Play the collect sound effect.
        //     Core.Audio.PlaySoundEffect(_collectSoundEffect);

        //     // Increase the player's score.
        //     _score += 100;
        // }
    }

    

    private void CheckSystemKeyboardInput()
    {
        // Get a reference to the keyboard inof
        KeyboardInfo keyboard = Core.Input.Keyboard;

        // If the escape key is pressed, return to the title screen.
        if (Core.Input.Keyboard.KeyPressed(Keys.Escape))
        {
            Core.ChangeScene(new TitleScene());
        }

        // If the space key is held down, the movement speed increases by 1.5
        float speed = MOVEMENT_SPEED;
        if (keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        // If the M key is pressed, toggle mute state for audio.
        if (keyboard.KeyPressed(Keys.M))
        {
            Core.Audio.ToggleMute();
        }

        // If the + button is pressed, increase the volume.
        if (keyboard.KeyPressed(Keys.OemPlus))
        {
            Core.Audio.SongVolume += 0.1f;
            Core.Audio.SoundEffectVolume += 0.1f;
        }

        // If the - button was pressed, decrease the volume.
        if (keyboard.KeyPressed(Keys.OemMinus))
        {
            Core.Audio.SongVolume -= 0.1f;
            Core.Audio.SoundEffectVolume -= 0.1f;
        }
    }

    private void CheckGamePadInput()
    {
        // Get the gamepad info for gamepad one.
        GamePadInfo gamePadOne = Core.Input.GamePads[(int)PlayerIndex.One];

        // If the A button is held down, the movement speed increases by 1.5
        // and the gamepad vibrates as feedback to the player.
        float speed = MOVEMENT_SPEED;
        if (gamePadOne.IsButtonDown(Buttons.A))
        {
            speed *= 1.5f;
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
        }
        else
        {
            GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
        }

    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap
        _tilemap.Draw(Core.SpriteBatch);

        // Draw the slime sprite.
        _protag.Draw(Core.SpriteBatch);

        // Draw the bat sprite.
        _bat.Draw(Core.SpriteBatch);

        // Draw the score.
        Core.SpriteBatch.DrawString(
            _font,              // spriteFont
            $"Score: {_score}", // text
            _scoreTextPosition, // position
            Color.White,        // color
            0.0f,               // rotation
            _scoreTextOrigin,   // origin
            1.0f,               // scale
            SpriteEffects.None, // effects
            0.0f                // layerDepth
        );

        if(DEBUG)
        {
            foreach (var colR in _collisionChecker.CollisionRects)
            {
                colR.DebugSprite.Draw(Core.SpriteBatch, new Vector2(colR.Left(), colR.Top()));
            }
        }

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }
}
