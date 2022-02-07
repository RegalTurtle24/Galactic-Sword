using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;

public enum Areas
{
    Overworld,
    Dungeon1,
    Dungeon2,
    Dungeon3
}

public enum GameState
{
    Home,
    Game,
    Info,
    Controls,
    Paused,
    End,
    Win
}

class Game
{
    public static readonly string Title = "Minimalist Game Framework";
    public static readonly Vector2 Resolution = new Vector2(640, 480);

    readonly Texture homescreen = Engine.LoadTexture("home.png");
    readonly Texture paused = Engine.LoadTexture("paused.png");
    readonly Texture inventory = Engine.LoadTexture("inventory.png");
    readonly Texture swordAnimation = Engine.LoadTexture("sword_slash.png");
    readonly Texture background = Engine.LoadTexture("background.png");
    readonly Texture howToPlay = Engine.LoadTexture("howToPlay.png");
    readonly Texture heart = Engine.LoadTexture("heart.png");
    readonly Texture bombExplode1 = Engine.LoadTexture("bomb explode.png");
    readonly Texture bombExplode2 = Engine.LoadTexture("bomb explode 2.png");
    readonly Texture bombExplode3 = Engine.LoadTexture("bomb explode 3.png");
    readonly Texture detonator = Engine.LoadTexture("detonation device.png");
    readonly Texture sword = Engine.LoadTexture("sword.png");
    readonly Texture key = Engine.LoadTexture("key.png");
    readonly Texture endscreen = Engine.LoadTexture("endscreen.png");
    readonly Texture winscreen = Engine.LoadTexture("winscreen.png");
    readonly Font defaultFont = Engine.LoadFont("segoe.ttf", 20);
    readonly Color gameBlue = new Color(0x51, 0xBB, 0xFC);

    Music BGM = Engine.LoadMusic("overworldBGM.wav");
    Music homeBGM = Engine.LoadMusic("homeBGM.mp3");
    bool playMusic = false;

    Vector2 movement;

    GameState curGameState = GameState.Home;

    int currentArea = (int)Areas.Overworld;

    static List<Map> maps = new List<Map>();

    List<Item> dungeon1ItemList;
    List<Item> dungeon2ItemList;
    List<Item> dungeon3ItemList;

    public static Boolean detonationDeviceAcquired = false;

    Player mainCharacter = new Player();
    Bounds2 playerCoords = new Bounds2();

    // number indicating how many triangles are in place at the center
    public static int dungeonsCompleted = 0;

    //variable for timer for sword animation
    public static int time = -1;

    //variable for timer for bomb explosion animation
    int bombTime = -2;

    public Boolean bombDropped = false;

    List<Item> detonatingBombs = new List<Item>();

    public static int bombCount = 0;

    public static int keyCount = 0;

    string startSave = "Assets/startSave.csv";
    string gameSave = "Assets/gameSave.csv";
    string dun1Start = "Assets/dungeon1.csv";
    string dun1Save = "Assets/dungeon1save.csv";
    string dun2Start = "Assets/dungeon2.csv";
    string dun2Save = "Assets/dungeon2save.csv";
    string dun3Start = "Assets/dungeon3.csv";
    string dun3Save = "Assets/dungeon3save.csv";

    public Game()
    {
    }

    public void Update()
    {
        userInputMK();
        if (curGameState == GameState.Game) updateGame();
        drawGame();
        drawSwordAnimation();
        drawBombDetonationAnimation();
    }

    private void userInputMK()
    {
        if (Engine.GetKeyDown(Key.R) && curGameState == GameState.Paused)
        {
            if (currentArea == 1)
                maps[currentArea].changeOffSet(new Vector2(-1035, -2800));

            else if (currentArea == 2)
                maps[currentArea].changeOffSet(new Vector2(-1350, -4140));

            else if (currentArea == 3)
                maps[currentArea].changeOffSet(new Vector2(-2000, -4860));

            curGameState = GameState.Game;
        }

        if (Engine.GetKeyDown(Key.Escape) && curGameState != GameState.Home)
        {
            if (curGameState != GameState.Paused)
            {
                curGameState = GameState.Paused;
            }
            else
            {
                curGameState = GameState.Game;
            }
        }


        if (Engine.GetKeyDown(Key.I) && curGameState == GameState.Home)
        {
            curGameState = GameState.Info;
        }

        if (Engine.GetKeyDown(Key.Right) && curGameState == GameState.Info)
        {
            curGameState = GameState.Controls;
        }

        if (Engine.GetKeyDown(Key.Left) && curGameState != GameState.Game)
        {
            if (curGameState == GameState.Info)
            {
                curGameState = GameState.Home;
            }
            if (curGameState == GameState.Controls)
            {
                curGameState = GameState.Info;
            }
        }

        /*
        if (!Engine.GetKeyDown(Key.I) && (Engine.TypedText.Length != 0) && curGameState == GameState.Home)
        {
            startupWithFile();
            curGameState = GameState.Game;
            playMusic = false;
        }
        */

        if (Engine.GetKeyDown(Key.S) && curGameState == GameState.Home)
        {
            startupWithFile(startSave, dun1Start, dun2Start, dun3Start);
            curGameState = GameState.Game;
            playMusic = false;
        }

        if (Engine.GetKeyDown(Key.F) && curGameState == GameState.Home)
        {
            startupWithFile(gameSave, dun1Save, dun2Save, dun3Save);
            curGameState = GameState.Game;
            playMusic = false;
        }

        if (Engine.GetKeyDown(Key.S) && curGameState == GameState.Paused)
        {
            saveGame();
        }

        int x = 0;
        int y = 0;
        if (Engine.GetKeyHeld(Key.W))
        {
            y -= 1;
        }
        if (Engine.GetKeyHeld(Key.S))
        {
            y += 1;
        }
        if (Engine.GetKeyHeld(Key.A))
        {
            x -= 1;
        }
        if (Engine.GetKeyHeld(Key.D))
        {
            x += 1;
        }
        movement = new Vector2(x, y);

        if (Engine.GetKeyDown(Key.C) && bombCount > 0 && curGameState == GameState.Game)
        {
            maps[currentArea].dropBomb();
            bombCount--;
            bombDropped = true;
        }

        if (Engine.GetKeyDown(Key.P) &&
            (curGameState == GameState.Paused || curGameState == GameState.End))
        {
            startupWithFile(gameSave, dun1Save, dun2Save, dun3Save);
            curGameState = GameState.Game;
        }

        if (curGameState == GameState.Game)
        {
            playerCoords = new Bounds2(new Vector2(Resolution.X / 2, Resolution.Y / 2) - maps[currentArea].mapOffset, new Vector2(32, 32));
        }
    }

    private void updateGame()
    {
        // moves the player based off of the movement Vector2 and the current map and map offset
        // map offsets and maps are changed through currentArea var
        // this method only runs if isGame == true, so player will not move if the game is paused
        mainCharacter.move(movement, ref maps[currentArea].mapOffset, maps[currentArea], Resolution, ref currentArea, dungeonsCompleted);
    }

    private void drawGame()
    {
        // uses the map function to draw itself based off of the current map and the current map offset
        // both are found in arrays, and dynamically change what map it uses based on currentArea
        if (curGameState == GameState.Game)
        {
            maps[currentArea].drawMap(Resolution);
            maps[currentArea].drawEnemies(Resolution, curGameState, mainCharacter, playerCoords);
            maps[currentArea].drawBombs();
            drawDungeonItems(currentArea);
        }

        // uses the player draw function to draw the player
        mainCharacter.draw();

        if (curGameState == GameState.Info)
        {
            Engine.DrawTexture(background, new Vector2(0, 0));
        }

        if (curGameState == GameState.End)
        {
            Engine.DrawTexture(endscreen, new Vector2(0, 0));
        }

        if (curGameState == GameState.Win)
        {
            Engine.DrawTexture(winscreen, new Vector2(0, 0));
        }

        if (curGameState == GameState.Controls)
        {
            Engine.DrawTexture(howToPlay, new Vector2(0, 0));
        }

        if (curGameState == GameState.Game)
        {
            drawInventory();
            if (!playMusic)
            {
                playMusic = true;
                Engine.PlayMusic(BGM, true, 0);
            }
        }
        else
        {
            if (!playMusic)
            {
                playMusic = true;
                Engine.PlayMusic(homeBGM, true, 1);
            }
        }

        if (curGameState == GameState.Paused)
        {
            pauseGame();
        }

        if (curGameState == GameState.Home)
        {
            Engine.DrawTexture(homescreen, new Vector2(0, 0));
        }
    }

    private void drawSwordAnimation()
    {
        float swordAnimationFrame = 9;
        if (Engine.GetKeyDown(Key.Space) && mainCharacter.hasSword && curGameState == GameState.Game)
        {
            time = 1;
        }
        if (time > -1)
        {
            //draw
            Bounds2 swordBounds = new Bounds2((time % 3) * 48, (time / 3) * 48, 48, 48);
            Bounds2 bounds = new Bounds2(322, 216, 16, 16);
            TextureMirror swordMirror = !mainCharacter.isPlayerFacingLeft() ? TextureMirror.None : TextureMirror.Horizontal;

            if (mainCharacter.isPlayerFacingLeft())
            {
                bounds.Position.X = 269;
            }

            Engine.DrawTexture(swordAnimation, bounds.Position, source: swordBounds, mirror: swordMirror);

            if (time < swordAnimationFrame - 1)
                time++;
            else
                time = -1;
        }
    }

    private void drawBombDetonationAnimation()
    {
        Vector2 explosionSize = new Vector2(200, 200);
        if (Engine.GetKeyDown(Key.E) && detonationDeviceAcquired)
        {
            bombTime = 0;
            maps[currentArea].detonateBomb(ref detonatingBombs);
        }

        for (int i = 0; i < detonatingBombs.Count; i++)
        {
            Item item = detonatingBombs[i];

            if (bombTime == 1 || bombTime == 2)
            {
                Engine.DrawTexture(bombExplode1, item.getPosition().Position + maps[currentArea].mapOffset - (explosionSize / 2), size: explosionSize);
            }
            if (bombTime == 3 || bombTime == 4)
            {
                Engine.DrawTexture(bombExplode2, item.getPosition().Position + maps[currentArea].mapOffset - (explosionSize / 2), size: explosionSize);
            }
            if (bombTime == 5)
            {
                Engine.DrawTexture(bombExplode3, item.getPosition().Position - (explosionSize / 2), size: explosionSize);
                Bounds2 explosion = new Bounds2(item.getPosition().Position - (explosionSize / 2), explosionSize);
                for (int j = 0; j < maps[currentArea].getTiles().Count; j++)
                {
                    Tile t = maps[currentArea].getTiles()[j];
                    if (t.isOnscreen(Resolution, maps[currentArea].mapOffset))
                    {
                        if (t.isBreakable() && t.Bounds.Overlaps(explosion))
                        {
                            t.turnDoor();
                        }
                    }
                }
                List<Enemy> e = maps[currentArea].getEnemies();
                for (int j = 0; j < maps[currentArea].getEnemies().Count; j++)
                {
                    if (e[j].isOnscreen(Resolution, maps[currentArea].mapOffset))
                    {
                        if (e[j].Bounds.Overlaps(explosion))
                        {
                            e[j].damage(2);
                        }
                    }
                }
                detonatingBombs.Clear();
            }
        }
        if (bombTime == 6 || bombTime == -1)
        {
            bombDropped = false;
            bombTime = -2;
        }
        bombTime++;
    }

    private void drawDungeonItems(int currentArea)
    {
        if (currentArea == 1)
        {
            maps[currentArea].drawItems(ref dungeon1ItemList, maps[1].mapOffset, mainCharacter);
        }
        else if (currentArea == 2)
        {
            maps[currentArea].drawItems(ref dungeon2ItemList, maps[2].mapOffset, mainCharacter);
        }
        else if (currentArea == 3)
        {
            maps[currentArea].drawItems(ref dungeon3ItemList, maps[3].mapOffset, mainCharacter);
        }
    }

    private void pauseGame()
    {
        Engine.DrawTexture(paused, new Vector2(0, 0));
    }

    private void drawInventory()
    {
        // draw hotbar
        Engine.DrawTexture(inventory, new Vector2(0, 480 * 5 / 6));
        // draw health bar
        for (int i = 0; i < mainCharacter.getHealth(); i++)
        {
            Engine.DrawTexture(heart, new Vector2(500 + 20 * i, 430), null, new Vector2(25, 25));
        }
        if (mainCharacter.getHealth().Equals(0) && curGameState == GameState.Game)
        {
            curGameState = GameState.End;
        }
        // draw bomb count
        Engine.DrawString(" x " + bombCount, new Vector2(62, 427), gameBlue, defaultFont);
        // draw detonator in inventory
        if (detonationDeviceAcquired)
        {
            Engine.DrawTexture(detonator, new Vector2(342, 427));
        }
        // draw sword in inventory
        if (mainCharacter.hasSword)
        {
            Engine.DrawTexture(sword, new Vector2(262, 415));
        }
        // draw key count
        if (keyCount >= 5)
        {
            curGameState = GameState.Win;
        }
        Engine.DrawTexture(key, new Vector2(112, 421));
        Engine.DrawString(" x " + keyCount, new Vector2(142, 427), gameBlue, defaultFont);

    }

    public static void finishDugeon()
    {
        if (dungeonsCompleted < 4)
        {
            dungeonsCompleted++;
        }
        if (dungeonsCompleted == 1)
        {
            maps[0].addTriangle(32 * 16 + 8, 20 * 16);
        }
        else if (dungeonsCompleted == 2)
        {
            maps[0].addTriangle(32 * 16, 21 * 16);
        }
        else if (dungeonsCompleted == 3)
        {
            maps[0].addTriangle(33 * 16, 21 * 16);
        }
    }

    public void startupWithFile(String fileName, string dun1, string dun2, string dun3)
    {
        FileManager fm = new FileManager();
        fm.loadFromExistingFile(fileName);

        currentArea = fm.currentArea;
        keyCount = fm.keys;
        mainCharacter.setHealth(fm.hearts);

        detonationDeviceAcquired = fm.hasDetonator;
        bombCount = fm.bombs;

        maps.Clear();
        maps.Add(new Map(Areas.Overworld, fm.overworldOffset, new List<Enemy>(), null));
        maps.Add(new Map(Areas.Dungeon1, fm.dungeon1Offset, fm.dun1Enemies, dun1));
        maps.Add(new Map(Areas.Dungeon2, fm.dungeon2Offset, fm.dun2Enemies, dun2));
        maps.Add(new Map(Areas.Dungeon3, fm.dungeon3Offset, fm.dun3Enemies, dun3));

        dungeon1ItemList = maps[currentArea].dungeon1ItemListMaker();
        dungeon2ItemList = maps[currentArea].dungeon2ItemListMaker();
        dungeon3ItemList = maps[currentArea].dungeon3ItemListMaker();

        dungeonsCompleted = 0;
        for (int i = 0; i < fm.dungeonsCompleted; i++)
        {
            finishDugeon();
        }

        if(fileName.Equals(startSave))
        {
            saveGame();
        }    
    }

    public void saveGame()
    {
        FileManager fm = new FileManager();
        fm.wipeFile();

        String generalInfo =
            "dungeonsCompleted:" + dungeonsCompleted + "," +
            "currentArea:" + currentArea + "," +
            "hasSword:" + true + "," +
            "hasBag:" + false + "," +
            "hasDetonator:" + detonationDeviceAcquired + "," +
            "bombs:" + bombCount + "," +
            "hearts:" + mainCharacter.getHealth() + "," +
            "keys:" + keyCount;

        fm.writeToGameSave(generalInfo);

        String overworldInfo =
            "overworldOffset:" + maps[(int)Areas.Overworld].mapOffset.X + ":" + maps[(int)Areas.Overworld].mapOffset.Y;

        fm.writeToGameSave(overworldInfo);

        String dungeon1Info =
            "dungeon1Offset:" + maps[(int)Areas.Dungeon1].mapOffset.X + ":" + maps[(int)Areas.Dungeon1].mapOffset.Y;
        foreach (Enemy e in maps[1].getEnemies())
        {
            dungeon1Info = dungeon1Info + ",enemy1:" + e.getType() + ":" + (int)e.Bounds.Position.X + ":" + (int)e.Bounds.Position.Y;
        }

        fm.writeToGameSave(dungeon1Info);

        String dungeon2Info =
            "dungeon2Offset:" + maps[(int)Areas.Dungeon2].mapOffset.X + ":" + maps[(int)Areas.Dungeon2].mapOffset.Y;
        foreach (Enemy e in maps[2].getEnemies())
        {
            dungeon2Info = dungeon2Info + ",enemy2:" + e.getType() + ":" + (int)e.Bounds.Position.X + ":" + (int)e.Bounds.Position.Y;
        }

        fm.writeToGameSave(dungeon2Info);

        String dungeon3Info =
            "dungeon3Offset:" + maps[(int)Areas.Dungeon3].mapOffset.X + ":" + maps[(int)Areas.Dungeon3].mapOffset.Y;
        foreach (Enemy e in maps[3].getEnemies())
        {
            dungeon3Info = dungeon3Info + ",enemy3:" + e.getType() + ":" + (int)e.Bounds.Position.X + ":" + (int)e.Bounds.Position.Y;
        }

        fm.writeToGameSave(dungeon3Info);

        string dun1 = "";
        foreach(Tile t in maps[1].getTiles())
        {
            if (t.Bounds.Position.X == 0 && t.Bounds.Position.Y != 0)
            {
                dun1 = dun1.Substring(0, dun1.Length - 1);
                dun1 = dun1 + "\n";
            }
            dun1 = dun1 + t.ID;
            dun1 = dun1 + ",";
        }
        dun1 = dun1.Substring(0, dun1.Length - 3);
        File.WriteAllText(dun1Save, dun1);
    }
}