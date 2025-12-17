using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class Animation
{
    /// <summary>
    /// The texture regions that make up the frames of this animation.  The order of the regions within the collection
    /// are the order that the frames should be displayed in.
    /// </summary>
    public List<TextureRegion> Frames { get; set; }

    /// <summary>
    /// Offset we want to apply as the sprite animates, for swinging tools etc.
    /// </summary>
    public List<Vector2> Offsets { get; set; }

    /// <summary>
    /// Dictionary of start frames to hitbox data for objects with hitboxes
    /// </summary>
    public Dictionary<int, (int width, int height, int xOffset, int yOffset)> HitboxInfo;

    /// <summary>
    /// The amount of time to delay between each frame before moving to the next frame for this animation.
    /// </summary>
    public TimeSpan Delay { get; set; }

    /// <summary>
    /// Creates a new animation.
    /// </summary>
    public Animation()
    {
        Frames = new List<TextureRegion>();
        Offsets = new List<Vector2>();
        Delay = TimeSpan.FromMilliseconds(100);
    }

    /// <summary>
    /// Creates a new animation with the specified frames and delay.
    /// </summary>
    /// <param name="frames">An ordered collection of the frames for this animation.</param>
    /// <param name="delay">The amount of time to delay between each frame of this animation.</param>
    public Animation(List<TextureRegion> frames, TimeSpan delay)
    {
        Frames = frames;
        Delay = delay;
    }

    /// <summary>
    /// Creates a new animation with the specified frames and delay.
    /// </summary>
    /// <param name="frames">An ordered collection of the frames for this animation.</param>
    /// <param name="offsets">An ordered collection the translations for the frames for this animation.</param>
    /// <param name="delay">The amount of time to delay between each frame of this animation.</param>
    public Animation(List<TextureRegion> frames, List<Vector2> offsets, TimeSpan delay)
    {
        Frames = frames;
        Offsets = offsets;
        Delay = delay;
    }

    /// <summary>
    /// Creates a new animation with the specified frames and delay.
    /// </summary>
    /// <param name="frames">An ordered collection of the frames for this animation.</param>
    /// <param name="offsets">An ordered collection the translations for the frames for this animation.</param>
    /// <param name="delay">The amount of time to delay between each frame of this animation.</param>
    public Animation(List<TextureRegion> frames, List<Vector2> offsets, TimeSpan delay, Dictionary<int, (int width, int height, int xOffset, int yOffset)> hitboxinfo)
    {
        Frames = frames;
        Offsets = offsets;
        Delay = delay;
        HitboxInfo = hitboxinfo;
    }

}