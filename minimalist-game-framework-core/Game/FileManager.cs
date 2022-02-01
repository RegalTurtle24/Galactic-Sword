using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;

class FileManager
{
    public int dungeonsCompleted;
    public int currentArea;
    public bool hasSword;
    public bool hasBag;
    public bool hasDetonator;
    public int bombs;
    public int hearts;
    public Vector2 overworldOffset;
    public Vector2 dungeon1Offset;
    public Vector2 dungeon2Offset;
    public Vector2 dungeon3Offset;
    public List<Enemy> dun1Enemies = new List<Enemy>();
    public List<Enemy> dun2Enemies = new List<Enemy>();
    public List<Enemy> dun3Enemies = new List<Enemy>();
    public List<Item> dum1Items = new List<Item>();
    public List<Item> dum2Items = new List<Item>();
    public List<Item> dum3Items = new List<Item>();
    public int keys;

    public void load()
    {
        TextFieldParser parser = new TextFieldParser("assets/save.csv");
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");

        while (!parser.EndOfData)
        {
            String[] sets = parser.ReadFields();

            foreach (string data in sets)
            {
                String[] pairs = data.Split(":");

                switch (pairs[0])
                {
                    case "dungeonsCompleted":
                        dungeonsCompleted = Int32.Parse(pairs[1]);
                        break;
                    case "currentArea":
                        currentArea = Int32.Parse(pairs[1]);
                        break;
                    case "hasSword":
                        hasSword = Boolean.Parse(pairs[1]);
                        break;
                    case "hasBag":
                        hasBag = Boolean.Parse(pairs[1]);
                        break;
                    case "hasDetonator":
                        hasDetonator = Boolean.Parse(pairs[1]);
                        break;
                    case "bombs":
                        bombs = Int32.Parse(pairs[1]);
                        break;
                    case "hearts":
                        hearts = Int32.Parse(pairs[1]);
                        break;
                    case "overworldOffset":
                        overworldOffset = new Vector2(float.Parse(pairs[1]), float.Parse(pairs[2]));
                        break;
                    case "dungeon1Offset":
                        dungeon1Offset = new Vector2(float.Parse(pairs[1]), float.Parse(pairs[2]));
                        break;
                    case "dungeon2Offset":
                        dungeon2Offset = new Vector2(float.Parse(pairs[1]), float.Parse(pairs[2]));
                        break;
                    case "dungeon3Offset":
                        dungeon3Offset = new Vector2(float.Parse(pairs[1]), float.Parse(pairs[2]));
                        break;
                    case "enemy1":
                        dun1Enemies.Add(new Enemy(pairs[1], Int32.Parse(pairs[2]), int.Parse(pairs[3])));
                        break;
                    case "enemy2":
                        dun2Enemies.Add(new Enemy(pairs[1], Int32.Parse(pairs[2]), int.Parse(pairs[3])));
                        break;
                    case "enemy3":
                        dun3Enemies.Add(new Enemy(pairs[1], Int32.Parse(pairs[2]), int.Parse(pairs[3])));
                        break;
                    case "item1":
                        dum1Items.Add(new Item(pairs[1], Int32.Parse(pairs[2]), int.Parse(pairs[3])));
                        break;
                    case "item2":
                        dum2Items.Add(new Item(pairs[1], Int32.Parse(pairs[2]), int.Parse(pairs[3])));
                        break;
                    case "item3":
                        dum3Items.Add(new Item(pairs[1], Int32.Parse(pairs[2]), int.Parse(pairs[3])));
                        break;
                    case "keys":
                        keys = Int32.Parse(pairs[1]);
                        break;
                }
            }
        }
    }

    public void writeToFile(String contents)
    {
        File.AppendAllText("assets/save.csv", contents + "\n");
    }

    public void wipeFile()
    {
        File.WriteAllText("assets/save.csv", "");
    }
}