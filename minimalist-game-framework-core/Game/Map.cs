using System;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

class Map
{
    // List of the tiles in the map
    List<Tile> tiles = new List<Tile>();
    List<Enemy> enemies = new List<Enemy>();
    public Vector2 mapOffset = new Vector2(0, 0);

    int scale = 1;
    int tileSize = 64;

    List<Item> droppedBombs = new List<Item>();

    // Tiles for yellow triangles
    readonly Texture triangle = Engine.LoadTexture("yellow_triangle.png");

    /// <summary>
    /// Constructor for a map
    /// </summary>
    /// <param name="mapType">What type of map is this: 0 for overworld</param>
    public Map(Areas mapType, Vector2 mapOffset, List<Enemy> enemies, string fileName)
    {
        // map type 0 is an overworld map, so starts to use that
        if (mapType == Areas.Overworld)
        {
            createOverworldMap();
        }
        else
        {
            createDungeon(fileName, mapType);
        }

        this.mapOffset = mapOffset;
        this.enemies = enemies;
    }

    /// <summary>
    /// Draws the map to the screen
    /// </summary>
    public void drawMap(Vector2 res)
    {
        // for each tile, if it is on screen, draw it
        foreach (Tile t in tiles)
        {
            if (t.isOnscreen(res, mapOffset))
            {
                Engine.DrawTexture(t.getTexture(), t.Bounds.Position + mapOffset,
                    rotation: t.getRotation(),
                    source: t.getTextureBounds(),
                    mirror: t.getMirror(),
                    size: new Vector2(tileSize * scale, tileSize * scale));
            }
        }
    }

    public void drawEnemies(Vector2 res, GameState curGameState, Player mainCharacter, Bounds2 playerCoords)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy a = enemies[i];

            a.damage(mainCharacter, playerCoords);

            if (a.getHealth() <= 0)
            {
                enemies.Remove(enemies[i]);
            }

            if (a.isOnscreen(res, mapOffset))
            {
                if (a.getType().Equals("projectile"))
                    if (a.getAge() == 0)
                        a.setAngle(playerCoords.Position);
                if (a.getType().Equals("gunner") && a.getAge() % 120 == 0)
                    enemies.Add(new Enemy("projectile", a.Bounds.Position.X, a.Bounds.Position.Y));

                a.collide(tiles, res, mapOffset);
                a.move(playerCoords.Position);

                if (a.getType().Equals("projectile"))
                    Engine.DrawTexture(a.Texture, a.Bounds.Position + mapOffset, size: new Vector2(8, 8));
                else
                    Engine.DrawTexture(a.Texture, a.Bounds.Position + mapOffset, size: new Vector2(a.Scale * 16, a.Scale * 16));

                if (a.getType().Equals("projectile") && !a.isOnscreen(res, mapOffset))
                {
                    enemies.Remove(a);
                }
            }
        }
    }

    public void drawItems(ref List<Item> items, Vector2 mapOffset, Player character)
    {
        Vector2 itemSize = new Vector2(36, 36);
        List<Item> inInventory = new List<Item>();

        foreach (Item item in items)
        {
            Engine.DrawTexture(item.getTexture(), item.getPosition().Position + mapOffset, size: itemSize);
            if (character.getPlayerBounds().Overlaps(new Bounds2(item.getPosition().Position + mapOffset, item.getPosition().Size)))
            {
                inInventory.Add(item);
                if (item.getType().Equals("Bomb"))
                {
                    Game.bombCount++;
                }
                if (item.getType().Equals("Heart"))
                {
                    character.setHealth();
                }

                if (item.getType().Equals("DetonationDevice"))
                {
                    Game.detonationDeviceAcquired = true;
                    Game.finishDugeon();
                }
                if (item.getType().Equals("Key"))
                {
                    Game.keyCount++;
                    if (Game.dungeonsCompleted == 1 || Game.dungeonsCompleted == 2)
                    {
                        Game.finishDugeon();
                    }
                }
            }
        }
        foreach (Item item in inInventory)
        {
            items.Remove(item);
        }
    }

    public List<Item> dungeon1ItemListMaker()
    {
        List<Item> dungeon1Items = new List<Item>();
        dungeon1Items.Add(new Item("Bomb", 977, 2126));
        dungeon1Items.Add(new Item("Heart", 2966, 2319));
        dungeon1Items.Add(new Item("Bomb", 2447, 1997));
        dungeon1Items.Add(new Item("Bomb", 1870, 911));
        dungeon1Items.Add(new Item("Bomb", 2640, 720));
        dungeon1Items.Add(new Item("Bomb", 975, 1102));
        dungeon1Items.Add(new Item("Bomb", 334, 2513));
        dungeon1Items.Add(new Item("Bomb", 464, 2513));
        dungeon1Items.Add(new Item("Bomb", 590, 2513));
        dungeon1Items.Add(new Item("DetonationDevice", 2256, 80));
        return dungeon1Items;
    }

    public List<Item> dungeon2ItemListMaker()
    {
        List<Item> dungeon2Items = new List<Item>();
        dungeon2Items.Add(new Item("Bomb", 1552, 3470));
        dungeon2Items.Add(new Item("Bomb", 1806, 3470));
        dungeon2Items.Add(new Item("Bomb", 2958, 3661));
        dungeon2Items.Add(new Item("Bomb", 462, 3661));
        dungeon2Items.Add(new Item("Bomb", 526, 2061));
        dungeon2Items.Add(new Item("Bomb", 1677, 2060));
        dungeon2Items.Add(new Item("Bomb", 1677, 2125));
        dungeon2Items.Add(new Item("Bomb", 1677, 2189));
        dungeon2Items.Add(new Item("Bomb", 2831, 2062));
        dungeon2Items.Add(new Item("Heart", 3215, 591));
        dungeon2Items.Add(new Item("Key", 1675, 145));
        return dungeon2Items;
    }

    public List<Item> dungeon3ItemListMaker()
    {
        List<Item> dungeon3Items = new List<Item>();
        dungeon3Items.Add(new Item("Key", 4494, 3854));
        dungeon3Items.Add(new Item("Key", 2321, 2316));
        dungeon3Items.Add(new Item("Key", 399, 1290));
        dungeon3Items.Add(new Item("Heart", 271, 2831));
        dungeon3Items.Add(new Item("Key", 2321, 133));
        return dungeon3Items;
    }

    public void changeOffSet(Vector2 newOffset)
    {
        mapOffset = newOffset;
    }
    /// <summary>
    /// Gets the list of tiles in the map
    /// </summary>
    /// <returns>
    /// Returns an ArrayList of all tiles
    /// </returns>
    public List<Tile> getTiles()
    {
        return tiles;
    }

    public List<Enemy> getEnemies()
    {
        return enemies;
    }

    public void addTriangle(int xPos, int yPos)
    {
        tiles.Add(new Tile(new Bounds2(xPos * scale, yPos * scale, 16 * scale, 16 * scale), false, Areas.Overworld, new Bounds2(0, 0, 35, 35), 0, triangle, 0));
    }

    // Tilesheets for the overworld maps
    public readonly Texture dead_overworld_tiles = Engine.LoadTexture("dead_overworld_tiles.png");
    int deadOverworldTilesWidth = 14;
    public readonly Texture scifi_overworld_tiles = Engine.LoadTexture("scifi_overworld_tiles.png");
    int scifiOverworldTilesWidth = 18;
    int scifiOverworldTilesSheetTotal = 323;
    int overworldTlesSheetCombinedTotal = 574;
    // KEEP THIS OR ELSE IDK WHERE TO RESET THE FILE TO
    // Vector2 overworldStart = new Vector2(-1840, -440); 
    List<int> collidableTilesOverworld = new List<int> { 40, 41, 58, 59, 90, 76, 77, 94, 95, 351, 352, 365, 366, 355, 356, 369, 370, 357, 358, 371, 372 };
    List<int> overworldToD1 = new List<int> { 351, 352, 365, 366 };
    List<int> overworldToD2 = new List<int> { 355, 356, 369, 370 };
    List<int> overworldToD3 = new List<int> { 357, 358, 371, 372 };

    public void createOverworldMap()
    {
        scale = 2;
        tileSize = 16;

        // sets up parsing the csv
        TextFieldParser parser = new TextFieldParser("assets/overworld_map.csv");
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");

        // y offset for how many rows down to go on the tileset
        int yOffset = 0;

        // loop through data of csv
        while (!parser.EndOfData)
        {
            String[] IDs = parser.ReadFields();

            // x offset for how many columns to go over on the tileset
            int xOffset = 0;

            // loops through the tiles in the row of the csv
            foreach (string ID in IDs)
            {
                // make the ID an int for general use
                int intID = Int32.Parse(ID);

                // get the current tile location in relationship to the player
                Bounds2 tileLocation = new Bounds2(xOffset * 16 * scale, yOffset * 16 * scale, 16 * scale, 16 * scale);

                // set the collidable property to default value false
                bool coll = false;

                // set the door value to the default value false
                // used for checking if you should be teleported to a dungeon
                Areas door = Areas.Overworld;

                // gets the actual id for each texture
                int textureID = intID % overworldTlesSheetCombinedTotal;

                // gets the rotation of the tile
                int rotation = (intID / overworldTlesSheetCombinedTotal) * 90;

                // makes any of the 5 collidable tile types collidable
                if (collidableTilesOverworld.Contains(textureID))
                {
                    coll = true;
                }

                // makes the 4 door tiles marked as door tiles
                if (overworldToD1.Contains(textureID))
                {
                    door = Areas.Dungeon1;
                }
                else if (overworldToD2.Contains(textureID))
                {
                    door = Areas.Dungeon2;
                }
                else if (overworldToD3.Contains(textureID))
                {
                    door = Areas.Dungeon3;
                }

                // sets up the columns and file name given the we are reading the 1st tileset
                int columns = scifiOverworldTilesWidth;
                Texture fileName = scifi_overworld_tiles;
                // changes the textureID, columns, and fileName if we are working with the other tileset
                if (textureID > scifiOverworldTilesSheetTotal)
                {
                    textureID -= scifiOverworldTilesSheetTotal;
                    columns = deadOverworldTilesWidth;
                    fileName = dead_overworld_tiles;
                }

                // sets up the bounds of the texture for the tile
                Bounds2 textureBounds = new Bounds2(textureID % columns * 16, textureID / columns * 16, 16, 16);

                // creates the tile and adds it to the list of tiles
                tiles.Add(new Tile(tileLocation, coll, door, textureBounds, rotation, fileName, intID));

                // moves to the next tile position to the right
                xOffset++;
            }
            // moves to the next row of tiles
            yOffset++;
        }
    }

    int DungeonDoorTile = 293;
    long dungeonsRotationOffset = 536870912;
    int dungeonsMaxTextures = 510;
    int dungeonsTextureSheetWidth = 30;
    Texture dungeonsTileSheet = Engine.LoadTexture("tileset_sf.png");
    // KEEP THIS OR ELSE IDK WHERE TO RESET THE FILE TO
    // Vector2 dungeon1Start = new Vector2(-1035, -2800);
    // Vector2 dungeon2Start = new Vector2(-1350, -4140);
    // Vector2 dungeon3Start = new Vector2(-2000, -4860);

    List<int> dungeonsCollidableTextures = new List<int> { -1, 190, 191, 192, 194, 196, 199, 200, 201, 220, 221, 222, 223, 224, 229, 231, 250, 251, 252, 253, 254, 256, 257, 258, 259, 260, 261, 280, 281, 282, 283, 284, 285, 290, 291, 292, 462, 463, 489, 490,
          286, 289, 293 };

    private void createDungeon(String fileName, Areas area)
    {
        TextFieldParser parser = new TextFieldParser(fileName);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");

        // y offset for how many rows down to go on the tileset
        int yOffset = 0;

        while (!parser.EndOfData)
        {
            String[] IDs = parser.ReadFields();

            // x offset for how many columns to go over on the tileset
            int xOffset = 0;

            // loops through the tiles in the row of the csv
            foreach (string ID in IDs)
            {
                // make the ID an int for general use
                long intID = long.Parse(ID);

                // get the current tile location in relationship to the player
                Bounds2 tileLocation = new Bounds2(xOffset * 64, yOffset * 64, 64, 64);

                // set the collidable property to default value false
                bool coll = false;

                // set the door value to the default value false
                // used for checking if you should be teleported to a dungeon
                // no doors in dungeon1
                Areas door = area;

                // gets the actual id for each texture
                int textureID = 0;

                // gets the rotation of the tile
                int rotation = 0;

                TextureMirror mirror = TextureMirror.None;

                if (intID < dungeonsMaxTextures)
                {
                    textureID = (int)intID - 1;
                    rotation = 0;
                }
                else if (intID < 2 * dungeonsRotationOffset + dungeonsMaxTextures)
                {
                    textureID = (int)(intID - 2 * dungeonsRotationOffset - 1);
                    rotation = 0;
                    mirror = TextureMirror.Vertical;
                }
                else if (intID < 3 * dungeonsRotationOffset + dungeonsMaxTextures)
                {
                    textureID = (int)(intID - 3 * dungeonsRotationOffset - 1);
                    rotation = 270;
                }
                else if (intID < 4 * dungeonsRotationOffset + dungeonsMaxTextures)
                {
                    textureID = (int)(intID - 4 * dungeonsRotationOffset - 1);
                    rotation = 180;
                    mirror = TextureMirror.Vertical;
                }
                else if (intID < 5 * dungeonsRotationOffset + dungeonsMaxTextures)
                {
                    textureID = (int)(intID - 5 * dungeonsRotationOffset - 1);
                    rotation = 90;
                }
                else if (intID < 6 * dungeonsRotationOffset + dungeonsMaxTextures)
                {
                    textureID = (int)(intID - 6 * dungeonsRotationOffset - 1);
                    rotation = 180;
                }
                else if (intID < 7 * dungeonsRotationOffset + dungeonsMaxTextures)
                {
                    textureID = (int)(intID - 7 * dungeonsRotationOffset - 1);
                    rotation = 270;
                    mirror = TextureMirror.Vertical;
                }

                // makes any of the 5 collidable tile types collidable
                if (dungeonsCollidableTextures.Contains(textureID))
                {
                    coll = true;
                }

                bool breakable = false;
                if (textureID == 289 || textureID == 286)
                {
                    breakable = true;
                }

                if (DungeonDoorTile == textureID)
                {
                    door = Areas.Overworld;
                }

                // sets up the bounds of the texture for the tile
                Bounds2 textureBounds = new Bounds2(
                    textureID % dungeonsTextureSheetWidth * 64, textureID / dungeonsTextureSheetWidth * 64,
                    64, 64);

                // creates the tile and adds it to the list of tiles
                tiles.Add(new Tile(tileLocation, coll, door, textureBounds, rotation, dungeonsTileSheet, intID, mirror, breakable));

                // moves to the next tile position to the right
                xOffset++;
            }
            // moves to the next row of tiles
            yOffset++;
        }
    }

    public void dropBomb()
    {
        droppedBombs.Add(new Item("Bomb", (int)(320 - mapOffset.X), (int)(240 - mapOffset.Y)));
    }

    public void detonateBomb(ref List<Item> detonatingBombs)
    {
        foreach (Item item in droppedBombs)
        {
            detonatingBombs.Add(item);
        }
        droppedBombs.Clear();
    }

    public void drawBombs()
    {
        foreach (Item item in droppedBombs)
        {
            Engine.DrawTexture(item.getTexture(), item.getPosition().Position + mapOffset, size: new Vector2(36, 36));
        }
    }
}