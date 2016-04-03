using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{

  public static MapGenerator Instance;

  public enum Tile
  {
    Wall,
    Ground
  }

  public int mapWidth = 40;
  public int mapHeight = 40;
  public int numEnemies = 5;
  public Point numRooms = new Point (5, 15);
  public Point roomWidth = new Point (3, 8);
  public Point roomHeight = new Point (3, 8);


  public GameObject[] groundTiles;
  public GameObject[] wallTiles;
  public GameObject exit;
  public GameObject enemy;

  private Tile[][] map;
  private List<Room> rooms;
  private GameObject levelMap;

  void Start()
  {
    GenerateLevel ();
  }

  void Awake()
  {
    Instance = this;
  }

  /// <summary>
  /// Generates the level.
  /// </summary>
  public void GenerateLevel()
  {
    levelMap = new GameObject ("LevelMap");
    rooms = new List<Room> ();
    SetupMapArray ();
    GenerateRooms (Random.Range (numRooms.x, numRooms.y));
    InstantiateTiles ();
    float playerX = rooms[0].X + Mathf.Floor(rooms[0].Width / 2);
    float playerY = rooms[0].Y + Mathf.Floor(rooms[0].Height / 2);
    GameObject.Find ("Player").gameObject.transform.position = new Vector2 (playerX, playerY);
    GenerateEnemies ();
    float exitX = rooms[1].X + rooms[1].Width / 2;
    float exitY = rooms[1].Y + rooms[1].Height / 2;
    Instantiate (exit, new Vector3 (exitX, exitY, -1.0f), Quaternion.identity);
    InformDataManager ();
  }

  /// <summary>
  /// Updates the Data Manager with the
  /// tracked values of the level
  /// </summary>
  void InformDataManager()
  {
    DataMgr.Instance.currentLevel.numRooms += rooms.Count;
    float totalWidth = 0.0f;
    float totalHeight = 0.0f;
    foreach (Room r in rooms)
    {
      totalWidth += r.Width;
      totalHeight += r.Height;
    }
    DataMgr.Instance.currentLevel.averageRoomWidth += (totalWidth / rooms.Count);
    DataMgr.Instance.currentLevel.averageRoomHeight += (totalHeight / rooms.Count);
    DataMgr.Instance.currentLevel.numEnemies += numEnemies;
  }

  /// <summary>
  /// Generates the enemies.
  /// </summary>
  void GenerateEnemies()
  {
    for (int i = 0; i < numEnemies; i++)
    {
      int roomNum = Random.Range (0, rooms.Count);
      Point randPos = rooms[roomNum].GetRandomPoint ();
      Debug.Log (randPos.x + " " + randPos.y);
      Instantiate (enemy, new Vector2 (randPos.x, randPos.y), Quaternion.identity);
    }
  }
    
  /// <summary>
  /// Creates the array which holds the map data
  /// </summary>
  void SetupMapArray()
  {
    map = new Tile[mapWidth][];
    for (int i = 0; i < map.Length; i++)
    {
      map [i] = new Tile[mapHeight];
    }
  }

  /// <summary>
  /// Checks if the given Room fits in the level
  /// </summary>
  /// <returns><c>true</c>, if the Room fits, <c>false</c> otherwise.</returns>
  /// <param name="room">The Room being checked</param>
  bool RoomFits(Room room)
  {
    bool xFit = (room.Left > 0) && (room.Right < mapWidth);
    bool yFit = (room.Top > 0) && (room.Bottom < mapHeight);
    return xFit && yFit;
  }

  /// <summary>
  /// Checks to make sure the given Room doesn't overlap
  /// with any other existing Room within the level
  /// </summary>
  /// <returns><c>true</c>, if no Rooms overlap with the given one <c>false</c> otherwise.</returns>
  /// <param name="room">The Room being checked</param>
  bool NoRoomsOverlap(Room room)
  {
    foreach (Room r in rooms)
    {
      if (room.Overlap (r))
      {
        return false;
      }
    }
    return true;
  }

  /// <summary>
  /// Makes the path between two 
  /// Points in the X-Direction
  /// </summary>
  /// <returns>The Point where the path ends</returns>
  /// <param name="leftPt">The left Point of the given Points</param>
  /// <param name="rightPt">The right Point of the given Points</param>
  Point MakeXPath(Point leftPt, Point rightPt)
  {
    Point endPoint = leftPt;
    for (int x = leftPt.x; x < rightPt.x; x++)
    {
      map[leftPt.y][x] = Tile.Ground;
      endPoint = new Point (x, leftPt.y);
    }
    return endPoint;
  }

  /// <summary>
  /// Makes the path between two
  /// Points in the Y-Direction
  /// </summary>
  /// <param name="topPt">The top Point of the given Points</param>
  /// <param name="bottomPt">The bottom Point of the given Points</param>
  void MakeYPath(Point topPt, Point bottomPt)
  {
    for (int y = topPt.y; y <= bottomPt.y; y++)
    {
      map[y][topPt.x] = Tile.Ground;
    }
  }

  /// <summary>
  /// Links the given Rooms together
  /// </summary>
  /// <param name="rOne">The first Room</param>
  /// <param name="rTwo">The second Room</param>
  void LinkRooms(Room rOne, Room rTwo)
  {
    Point source = rOne.GetRandomPoint ();
    Point target = rTwo.GetRandomPoint ();

    // Make the Horizontal Path
    if (source.x < target.x)
    {
      source = MakeXPath (source, target);
    }
    else
    {
      Point temp = MakeXPath (target, source);
      target = source;
      source = temp;
    }

    // Make the Vertical Path
    if (source.y < target.y)
    {
      MakeYPath (source, target);
    }
    else
    {
      MakeYPath (target, source);
    }
  }

  /// <summary>
  /// Add the given Room to the level
  /// provided it fits and doesn't overlap
  /// with other Rooms in the level
  /// </summary>
  /// <returns><c>true</c>, if the Room was added, <c>false</c> otherwise.</returns>
  /// <param name="room">The Room to be added</param>
  bool AddRoom(Room room)
  {
    if (RoomFits (room) && NoRoomsOverlap (room))
    {
      rooms.Add (room);
      for (int y = room.Top; y < room.Bottom; y++)
      {
        for (int x = room.Left; x < room.Right; x++)
        {
          map[y][x] = Tile.Ground;
        }
      }
      return true;
    }
    return false;
  }

  /// <summary>
  /// Places a number of Rooms up to the
  /// given number in the level and links
  /// them all together
  /// </summary>
  /// <param name="nRooms">The maximum number of Rooms to create</param>
  void GenerateRooms(int nRooms)
  {
    int nTries = 15;
    int roomsMade = 0;

    while ((roomsMade < nRooms) && nTries > 0)
    {
      int rWidth = Random.Range (roomWidth.x, roomWidth.y);
      int rHeight = Random.Range (roomHeight.x, roomHeight.y);
      int roomX = Random.Range (0, mapWidth - rWidth);
      int roomY = Random.Range (0, mapHeight - rHeight);
      Room newRoom = new Room (roomX, roomY, rWidth, rHeight);
      if (AddRoom (newRoom))
      {
        roomsMade += 1;
        nTries = 15;
      }
      else
      {
        nTries -= 1;
      }
      for (int x = 0; x < rooms.Count; x++)
      {
        if (x == rooms.Count - 1)
        {
          LinkRooms (rooms[x], rooms[0]);
        }
        else
        {
          LinkRooms (rooms[x], rooms[x + 1]);
        }
      }
    }
  }

  /// <summary>
  /// Creates the graphical tile objects
  /// and places them in their positions
  /// within the level
  /// </summary>
  void InstantiateTiles()
  {
    for (int y = 0; y < mapHeight; y++)
    {
      for (int x = 0; x < mapWidth; x++)
      {
        GameObject tile = wallTiles[Random.Range(0, wallTiles.Length)];
        if (map[y][x] == Tile.Ground)
        {
          tile = groundTiles[0];
        }

        GameObject tileInstance = Instantiate (tile, new Vector2 (x, y), Quaternion.identity) as GameObject;
        tileInstance.transform.SetParent (levelMap.transform);
    }
  }
}
}