class Tile : GameObject
{
    // Is tile a wall?
    public bool Collidable { get; private set; } = false;
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
    public double ID { get; private set; }

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
    public Tile(Bounds2 bounds, bool collidable, Areas door, Bounds2 textureBounds, int rotation, Texture fileName, double ID, TextureMirror mirror = TextureMirror.None, bool breakable = false)
    {
        Bounds = bounds;
        Collidable = collidable;
        this.door = door;
        this.textureBounds = textureBounds;
        this.rotation = rotation;
        textureFileName = fileName;
        this.mirror = mirror;
        this.breakable = breakable;
        this.ID = ID;
    }

    public void turnDoor()
    {
        int magicMultNum = 0;
        if (rotation == 0 && mirror == TextureMirror.None)
            magicMultNum = 0;
        if (rotation == 0 && mirror == TextureMirror.Vertical)
            magicMultNum = 2;
        if (rotation == 270 && mirror == TextureMirror.None)
            magicMultNum = 3;
        if (rotation == 180 && mirror == TextureMirror.Vertical)
            magicMultNum = 4;
        if (rotation == 90 && mirror == TextureMirror.None)
            magicMultNum = 5;
        if (rotation == 180 && mirror == TextureMirror.None)
            magicMultNum = 6;
        if (rotation == 270 && mirror == TextureMirror.Vertical)
            magicMultNum = 7;

        ID = (288 + (long)536870912 * magicMultNum);

        Collidable = false;

        textureBounds = new Bounds2(17 * 64, 9 * 64, 64, 64);
    }
}
