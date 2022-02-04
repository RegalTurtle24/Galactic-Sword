class GameObject
{
    // Location and size of the object
    public Bounds2 Bounds { get; protected set; }

    public Texture Texture { get; protected set; }

    /// <summary>
    /// Checks if an object is on the screen or not
    /// </summary>
    /// <param name="resolution">The Vector2 resolution of the screen</param>
    /// <param name="mapOffset">The current map offset</param>
    /// <returns>
    /// Returns true if the given object can be seen on screen
    /// </returns>
    public bool isOnscreen(Vector2 resolution, Vector2 mapOffset)
    {
        // makes the current bounds by adding the map offset
        Bounds2 currentBounds = new Bounds2(Bounds.Position + mapOffset, Bounds.Size);

        // checks if the current bounds overlap with the resolution
        return (currentBounds.Overlaps(new Bounds2(new Vector2(0, 0), resolution)));
    }
}
