using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Point
{
  public int x;
  public int y;

  public Point()
  {
    this.x = -1;
    this.y = -1;
  }

  public Point(int x, int y)
  {
    this.x = x;
    this.y = y;
  }

  public int TravelCost(Point destination)
  {
    if(this.x == null || this.y == null)
      return -1;

    return Mathf.Abs (this.x - destination.x) + Mathf.Abs (this.y - destination.y);
  }

  public int X { get; set; }
  public int Y { get; set; }
}

[System.Serializable]
public class Room
{
  public int x;
  public int y;
  public int roomWidth;
  public int roomHeight;
  public int top;
  public int bottom;
  public int left;
  public int right;
  public List<Point> doors;

  public int X { get { return x; } }
  public int Y { get { return y; } }
  public int Width { get { return roomWidth; } }
  public int Height { get { return roomHeight; } }
  public int Top { get { return top; } }
  public int Bottom { get { return bottom; } }
  public int Left { get { return left; } }
  public int Right { get { return right; } }
  public List<Point> Doors { get { return doors; } }

  public Room(int x, int y, int width, int height)
  {
    this.x          = x;
    this.y          = y;
    this.roomWidth  = width;
    this.roomHeight = height;
    this.top        = this.y;
    this.bottom     = this.y + this.roomHeight - 1;
    this.left       = this.x;
    this.right      = this.x + this.roomWidth - 1;
    this.doors = new List<Point> ();
  } 

  /// <summary>
  /// Get a random Point within this Room
  /// </summary>
  /// <returns>The random point in this Room</returns>
  public Point GetRandomPoint()
  {
    Point result = new Point ();
    result.x = Random.Range (1, this.roomWidth) + this.x;
    result.y = Random.Range (1, this.roomHeight) + this.y;
    return result;
  }

  public bool ContainsPt(Point pt)
  {
    bool fitX = ((pt.x < this.roomWidth + this.x) && (pt.x > this.x));
    bool fitY = ((pt.y < this.roomHeight + this.y) && (pt.y > this.y));
    return fitX && fitY;
  }

  /// <summary>
  /// Checks if this Room overlaps with the given Room
  /// </summary>
  /// <param name="other">The other Room to check</param>
  public bool Overlap(Room other)
  {
    bool xOverlap = ((this.right >= other.left) && (this.left <= other.right));
    bool yOverlap = ((this.top <= other.bottom) && (this.bottom >= other.top));
    return xOverlap && yOverlap;
  }

  public bool IsDoor(Point doorPt)
  {
    foreach (Point door in this.doors)
    {
      if (door.x == doorPt.x && door.y == doorPt.y)
      {
        return true;
      }
    }
    return false;
  }

  public void AddDoor(Point doorPt)
  {
    this.doors.Add (doorPt);
  }
}
