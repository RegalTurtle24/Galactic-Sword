class Tile : GameObject
{
    // Is tile a wall?
    bool collidable = false;
    // Is tile a door?
    Areas door = Areas.Overworld;
    // Where is the tile on the tilesheet
    Bounds2 textureBounds;
    // How many degrees to rotate the tile
    int rotation = 0;
    // What file to use for the tile
    Texture textureFileName;
    TextureMirror mirror = TextureMirror.None;
    bool breakable = false;

    /// <summary>
    /// Returns true if the tile is collidable
    /// </summary>
    /// <returns>Returns true if the tile is collidable</returns>
    public bool getCollidable()
    {
        return collidable;
    }

    /// <summary>
    /// Returns the tile's texture bounds
    /// </summary>
    /// <returns>Returns a Bounds2 from the tile</returns>
    public Bounds2 getTextureBounds()
    {
        return textureBounds;
    }

    /// <summary>
    /// Returns the tile's rotation
    /// </summary>
    /// <returns>0, 90, 180, or 270 based on the tile's rotation</returns>
    public int getRotation()
    {
        return rotation;
    }

    /// <summary>
    /// Gets the tilesheet for the tile
    /// </summary>
    /// <returns>A Texture of the tilesheet for the tile</returns>
    public Texture getTexture()
    {
        return textureFileName;
    }

    public TextureMirror getMirror()
    {
        return mirror;
    }

    public Areas getDoorExit()
    {
        return door;
    }

    public bool isBreakable()
    {
        return breakable;
    }

    /// <summary>
    /// Constructor for a tile
    /// </summary>
    /// <param name="bounds">The bounds of the tile</param>
    /// <param name="collidable">Whether or not the tile is a wall</param>
    /// <param name="door">Whether or not the tile is a door</param>
    /// <param name="textureBounds">Where on the spritesheet is the texture</param>
    /// <param name="rotation">How many degrees should the texture be rotated</param>
    /// <param name="fileName">The texture file for the tile</param>
    public Tile(Bounds2 bounds, bool collidable, Areas door, Bounds2 textureBounds, int rotation, Texture fileName, TextureMirror mirror = TextureMirror.None, bool breakable = false)
    {
        this.bounds = bounds;
        this.collidable = collidable;
        this.door = door;
        this.textureBounds = textureBounds;
        this.rotation = rotation;
        textureFileName = fileName;
        this.mirror = mirror;
        this.breakable = breakable;
    }
}
