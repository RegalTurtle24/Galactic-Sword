class GameObject
{
    // Location and size of the object
    protected Bounds2 bounds;

    /// <summary>
    /// Gets the bounds of the object
    /// </summary>
    /// <returns>
    /// Returns the Bounds2 of the current object
    /// </returns>
    public Bounds2 getBounds()
    {
        return bounds;
    }

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
        Bounds2 currentBounds = new Bounds2(bounds.Position + mapOffset, bounds.Size);

        // checks if the current bounds overlap with the resolution
        return (currentBounds.Overlaps(new Bounds2(new Vector2(0, 0), resolution)));
    }
}
