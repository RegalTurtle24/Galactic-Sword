class Player : Entity
{
    int scale = 2;
    int characterSize = 16;

    public bool hasSword;

    // info on the texture and how to draw it
    bool playerFaceLeft = false;
    Bounds2 knightFrameBounds;

    // info on the movement
    bool knightIdle;
    float Framerate = 10;
    float WalkSpeed = 300;

    // textures for the character
    readonly Texture playerIdle = Engine.LoadTexture("knight_idle_spritesheet.png");
    readonly Texture playerRunning = Engine.LoadTexture("knight_run_spritesheet.png");

    /// <summary>
    /// Constructor for the player's character
    /// </summary>
    /// <param name="bounds">The bounds of the player's character</param>
    public Player()
    {
        health = 5;
        bounds = new Bounds2(320 - characterSize / 2 * scale, 240 - characterSize / 2 * scale, characterSize * scale, characterSize * scale);
        hasSword = true;
    }
    public Bounds2 getPlayerBounds()
    {
        return bounds;
    }

    public Vector2 getPlayerLocation()
    {
        return new Vector2(bounds.Position.X, bounds.Position.Y);
    }
    /// <summary>
    /// Moves the player
    /// </summary>
    /// <param name="dir">The velocity of the player</param>
    /// <param name="mapOffset">Current map offset as a reference</param>
    /// <param name="map">The map you are trying to move on</param>
    /// <param name="res">The resolution of the screen</param>
    public void move(Vector2 dir, ref Vector2 mapOffset, Map map, Vector2 res, ref int curArea, int dunComp)
    {
        // checks if the player is idle
        knightIdle = (dir.Length() == 0);
        if (!knightIdle)
        {
            // checks what direction the player is facing
            if (dir.X != 0) playerFaceLeft = dir.X < 0;

            // move x
            // Change the X value of the map offset
            mapOffset.X += dir.X * Engine.TimeDelta * WalkSpeed * -1;

            // manage x collision
            // For each tile in the map
            foreach (Tile t in map.getTiles())
            {
                // if the tile is collidable and onscreen
                if (t.getCollidable() && t.isOnscreen(res, mapOffset))
                {
                    // find the current position of the tile relative to the player
                    Bounds2 curTilePos = new Bounds2(t.getBounds().Position + mapOffset, t.getBounds().Size);

                    if (bounds.Overlaps(curTilePos) && curArea != (int)t.getDoorExit())
                    {
                        if (dunComp + 1 >= (int)t.getDoorExit())
                        {
                            mapOffset += dir * 30 * 1;

                            curArea = (int)t.getDoorExit();
                        }
                    }

                    // check if the tile is overlapping the player
                    if (bounds.Overlaps(curTilePos))
                    {
                        // Finds the center of the tile and player
                        float tileCenter = curTilePos.Position.X + curTilePos.Size.X / 2;
                        float playerCenter = bounds.Position.X + bounds.Size.X / 2;

                        // Sets the default direction of where the tile is going to push the player
                        int moveDir = -1;
                        // Flips this direction if the player is on the other side of the tile
                        if (tileCenter > playerCenter)
                        {
                            moveDir *= -1;
                        }

                        // While the player and tile overlap
                        while (bounds.Overlaps(curTilePos))
                        {
                            // push the player out of the tile
                            mapOffset.X += moveDir;
                            curTilePos = new Bounds2(t.getBounds().Position + mapOffset, t.getBounds().Size);
                        }
                    }
                }
            }

            // move y
            // Change the Y value of the map offset
            mapOffset.Y += dir.Y * Engine.TimeDelta * WalkSpeed * -1;

            // manage y collision
            foreach (Tile t in map.getTiles())
            {
                // if the tile is collidable and onscreen
                if (t.getCollidable() && t.isOnscreen(res, mapOffset))
                {
                    // find the current position of the tile relative to the player
                    Bounds2 curTilePos = new Bounds2(t.getBounds().Position + mapOffset, t.getBounds().Size);

                    if (bounds.Overlaps(curTilePos) && curArea != (int)t.getDoorExit())
                    {
                        if (dunComp + 1 >= (int)t.getDoorExit())
                        {
                            mapOffset += dir * 30 * 1;

                            curArea = (int)t.getDoorExit();
                        }
                    }

                    // check if the tile is overlapping the player
                    if (bounds.Overlaps(curTilePos))
                    {
                        // Finds the center of the tile and player
                        float tileCenter = curTilePos.Position.Y + curTilePos.Size.Y / 2;
                        float playerCenter = bounds.Position.Y + bounds.Size.Y / 2;

                        // Sets the default direction of where the tile is going to push the player
                        int moveDir = -1;
                        // Flips this direction if the player is on the other side of the tile
                        if (tileCenter > playerCenter)
                        {
                            moveDir *= -1;
                        }

                        // While the player and tile overlap
                        while (bounds.Overlaps(curTilePos))
                        {
                            // push the player out of the tile
                            mapOffset.Y += moveDir;
                            curTilePos = new Bounds2(t.getBounds().Position + mapOffset, t.getBounds().Size);
                        }
                    }
                }
            }
        }
    }

    public bool isPlayerFacingLeft()
    {
        return playerFaceLeft;
    }

    /// <summary>
    /// Draws the player's character
    /// </summary>
    public void draw()
    {
        frameIndex = (frameIndex + Engine.TimeDelta * Framerate) % 6.0f;

        knightFrameBounds = new Bounds2(((int)frameIndex) * characterSize, 0, characterSize, characterSize);
        TextureMirror knightMirror = playerFaceLeft ? TextureMirror.Horizontal : TextureMirror.None;

        if (knightIdle) Engine.DrawTexture(playerIdle, bounds.Position, source: knightFrameBounds, mirror: knightMirror,
            size: new Vector2(characterSize * scale, characterSize * scale));
        else Engine.DrawTexture(playerRunning, bounds.Position, source: knightFrameBounds, mirror: knightMirror, 
            size: new Vector2(characterSize * scale, characterSize * scale));
    }

    public int getHealth()
    {
        return health;
    }

    public void resetHealth()
    {
        health = 5;
    }

    public void takeDamage()
    {
        health--;
    }

    public void setHealth()
    {
        health++;
    }

    public void setHealth(int health)
    {
        this.health = health;
    }
}
