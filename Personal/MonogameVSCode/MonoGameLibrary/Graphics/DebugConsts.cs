namespace MonoGameLibrary.Graphics;

public static class DebugConsts
{
    private static TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");
    public static TextureRegion DEBUG_WHITE = atlas.GetRegion("debug-white");
}
