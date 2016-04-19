using UnityEngine;
using System.Collections;

public class Point
{
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

  public int x { get; set; }
  public int y { get; set; }
}

public class Room
{
  private int x;
  private int y;
  private int roomWidth;
  private int roomHeight;
  private int top;
  private int bottom;
  private int left;
  private int right;

  public int X { get { return x; } }
  public int Y { get { return y; } }
  public int Width { get { return roomWidth; } }
  public int Height { get { return roomHeight; } }
  public int Top { get { return top; } }
  public int Bottom { get { return bottom; } }
  public int Left { get { return left; } }
  public int Right { get { return right; } }

  public Room(int x, int y, int width, int height)
  {
    this.x          = x;
    this.y          = y;
    this.roomWidth  = width;
    this.roomHeight = height;
    this.top        = this.y;
    this.bottom     = this.y + this.roomHeight;
    this.left       = this.x;
    this.right      = this.x + this.roomWidth;
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

}
