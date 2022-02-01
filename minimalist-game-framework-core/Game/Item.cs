using System;

class Item
{
    // identifies what the Item object is
    private String Type;
    // the image that represents it on the screen
    private Texture Texture;
    // its location on the map
    private Bounds2 position;
        
    // constructor
    public Item(String type, int x, int y)
    {
        setPosition(x, y);
        Type = type;
        if(Type.Equals("Bomb"))
        {
            Texture = Engine.LoadTexture("bomb.png");
        }
        else if(Type.Equals("Heart"))
        {
            Texture = Engine.LoadTexture("heart.png");
        }
        else if(Type.Equals("DetonationDevice"))
        {
            Texture = Engine.LoadTexture("detonation device.png");
        }
        else if(Type.Equals("BombHolder"))
        {
            Texture = Engine.LoadTexture("bomb holder.png");
        }
        else if (Type.Equals("Sword"))
        {
            Texture = Engine.LoadTexture("sword.png");
        }
        else if(Type.Equals("Key"))
        {
            Texture = Engine.LoadTexture("key.png");
    }
    }

    public Texture getTexture()
    {
        return Texture;
    }

    public String getType()
    {
        return Type;
    }

    // determines coordinates of Item on map
    public void setPosition(int x, int y)
    {
        position.Position.X = (x);
        position.Position.Y = (y);
        position.Size.X = 36;
        position.Size.Y = 36;
}

    // returns position on map
    public Bounds2 getPosition()
    {
        return position;
    }

}