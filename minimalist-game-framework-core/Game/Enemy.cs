using System;
using System.Collections.Generic;

class Enemy : Entity
{
    public int Scale = 2;
    //how many frames the enemy has been alive for
    private int Age;
    //number of hits it takes to kill the enemy
    private int Health;
    //how much damage this enemy does
    private int Damage;
    //movement speed
    private double Speed;
    //angle for PROJECTILES
    private double Angle;
    //name of enemy type - fodder, melee, longer melee, and ranged enemies
    private String Type;

    //constructor
    public Enemy(String typeSet, double x, double y)
    {
        Type = typeSet;
        Age = 0;
        Speed = 2;

        if (Type.Equals("slime"))
        {
            Speed = 3;
            Health = 1;
            Damage = 1;
            Texture = Engine.LoadTexture("slime.png");
        }
        else if (Type.Equals("boxer"))
        {
            Health = 2;
            Damage = 2;
            Texture = Engine.LoadTexture("boxer.png");
        }
        else if (Type.Equals("turret"))
        {
            Health = int.MaxValue;
            Damage = 1;
            Speed = 0;
            Texture = Engine.LoadTexture("fencer.png");
        }
        else if (Type.Equals("gunner"))
        {
            Health = 1;
            Damage = 1;
            Texture = Engine.LoadTexture("gunner.png");
        }
        else if (Type.Equals("projectile"))
        {
            Health = 1;
            Damage = 1;
            Speed = 10;
            Texture = Engine.LoadTexture("projectile.png");
        }

        setCoords(x, y);
        if(typeSet.Equals("projectile"))
        {
            Bounds2 tempB = Bounds;
            tempB.Size.X = 8 * Scale;
            tempB.Size.Y = 8 * Scale;
            Bounds = tempB;
        } else
        {
            Bounds2 tempB = Bounds;
            tempB.Size.X = 16 * Scale;
            tempB.Size.Y = 16 * Scale;
            Bounds = tempB;
        }
    }

    //accesses each of the class variables
    public int getAge()
    {
        return Age;
    }
    public int getHealth()
    {
        return Health;
    }
    public int getDamage()
    {
        return Damage;
    }
    public String getType()
    {
        return Type;
    }

    //mutator for class variables
    public void setAngle(Vector2 playerPos)
    {
        double sin = (playerPos.Y - Bounds.Position.Y);
        double cos = (playerPos.X - Bounds.Position.X);

        if (sin == 0)
            sin += 0.01;

        Angle = Math.Atan(sin / cos);

        //THIS METHOD IS ONLY FOR PROJECTILE ENEMIES
        if (cos < 0)
            Speed = -10;
        else if (cos > 0)
            Speed = 10;
    }
    public void setAngle(double angle)
    {
        Angle = angle;
    }
    public void setCoords(double newX, double newY)
    {
        Bounds2 tempB = Bounds;
        tempB.Position.X = (float)newX;
        tempB.Position.Y = (float)newY;
        Bounds = tempB;
    }

    public void changeCoords(double deltaX, double deltaY)
    {
        setCoords(Bounds.Position.X + deltaX, Bounds.Position.Y + deltaY);
    }

    //moves the enemy
    public void move(Vector2 playerCoords)
    {
        double newX = Bounds.Position.X;
        double newY = Bounds.Position.Y;

        double x = playerCoords.X;
        double y = playerCoords.Y;

        double distToPlayer = Math.Sqrt((newX - x) * (newX - x) + (newY - y) * (newY - y));

        if (Type.Equals("slime"))
        {
            if (distToPlayer < 20)
                moveAway(ref newX, ref newY, x, y);
            else if (distToPlayer > 21 + Speed)
                moveTowards(ref newX, ref newY, x, y);
        }
        if (Type.Equals("boxer"))
        {
            if (distToPlayer < 10)
                moveAway(ref newX, ref newY, x, y);
            else if (distToPlayer > 11 + Speed)
                moveTowards(ref newX, ref newY, x, y);
        }
        if (Type.Equals("gunner"))
        {
            if (distToPlayer < 200)
            {
                moveAway(ref newX, ref newY, x, y);
            }
            else if (distToPlayer >= 201 + Speed)
            {
                moveTowards(ref newX, ref newY, x, y);
            }
        }
        if (Type.Equals("projectile"))
        {
            newX += Speed * Math.Cos(Angle);
            newY += Speed * Math.Sin(Angle);
        }

        setCoords(newX, newY);
        Age++;
    }

    //helper methods for non-bullet enemies that move using player position
    private void moveTowards(ref double newX, ref double newY, double x, double y)
    {
        //move towards player's X
        if (newX > x)
            newX -= Speed;
        if (newX < x)
            newX += Speed;

        //move towards player's Y
        if (newY > y)
            newY -= Speed;
        if (newY < y)
            newY += Speed;
    }
    private void moveAway(ref double newX, ref double newY, double x, double y)
    {
        double tempSpeed = Speed * 0.95;
        //move away from player's X
        if (newX > x)
            newX += tempSpeed;
        if (newX < x)
            newX -= tempSpeed;

        //move away from player's Y
        if (newY > y)
            newY += tempSpeed;
        if (newY < y)
            newY -= tempSpeed;
    }

    //COLLISION METHOD
    public void collide(List<Tile> tiles, Vector2 res, Vector2 mapOffset)
    {
        foreach (Tile t in tiles)
        {
            if (t.Collidable && t.isOnscreen(res, mapOffset) && !Type.Equals("projectile"))
            {
                if (Bounds.Overlaps(t.Bounds))
                {
                    float xPushRight = t.Bounds.Position.X + t.Bounds.Size.X - Bounds.Position.X;
                    float xPushLeft = Bounds.Position.X + Bounds.Size.X - t.Bounds.Position.X;
                    float yPushUp = Bounds.Position.Y + Bounds.Size.Y - t.Bounds.Position.Y;
                    float yPushDown = t.Bounds.Position.Y + t.Bounds.Size.Y - Bounds.Position.Y;

                    if (xPushRight < xPushLeft && xPushRight < yPushUp && xPushRight < yPushDown)
                    {
                        changeCoords(xPushRight, 0);
                    }
                    else if (xPushLeft < xPushRight && xPushLeft < yPushUp && xPushLeft < yPushDown)
                    {
                        changeCoords(-xPushLeft, 0);
                    }
                    else if (yPushUp < xPushLeft && yPushUp < xPushRight && yPushUp < yPushDown)
                    {
                        changeCoords(0, -yPushUp);
                    }
                    else if (yPushDown < xPushLeft && yPushDown < yPushUp && yPushDown < xPushRight)
                    {
                        changeCoords(0, yPushDown);
                    }
                }
            }
        }
    }

    public void damage(Player player, Bounds2 playerCoords)
    {
        double curX = Bounds.Position.X;
        double curY = Bounds.Position.Y;
        double centerX = curX + Bounds.Size.X / 2;
        double centerY = curY + Bounds.Size.Y / 2;

        double x = playerCoords.Position.X;
        double y = playerCoords.Position.Y;

        double distToPlayer = Math.Sqrt((centerX - x) * (centerX - x) + (centerY - y) * (centerY - y));

        if (distToPlayer <= 96 &&
          (((player.isPlayerFacingLeft() && curX < x)) ||
          ((!player.isPlayerFacingLeft() && curX > x))) &&
          Game.time != -1) //distance small, sword in use, player facing correct
        {
            Health--;
        }
        else if (Bounds.Overlaps(playerCoords))
        {
            player.takeDamage();

            //recoil on enemy
            if (playerCoords.Position.X < curX)
            {
                curX += 50;
            }
            if (playerCoords.Position.X > curX)
            {
                curX -= 50;
            }

            if (playerCoords.Position.Y < curY)
            {
                curY += 50;
            }
            if (playerCoords.Position.Y > curY)
            {
                curY -= 50;
            }

            if (Type.Equals("projectile"))
            {
                Health = 0;
            }

            setCoords(curX, curY);
        }
    }

    public void damage(int damageAmount)
    {
        Health -= damageAmount;
    }
}