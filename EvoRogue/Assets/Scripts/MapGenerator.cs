﻿using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{

  public static MapGenerator Instance;

  public enum Tile
  {
    Wall,
    Ground,
    Obstacle,
    Path,
    InnerWall
  }

  public int mapWidth;
  public int mapHeight;
  public int numEnemies;
  public Point numRooms;
  public Point roomWidth;
  public Point roomHeight;
  public float innerWallDensity;
  public float obstacleDensity;

  public GameObject[] groundTiles;
  public GameObject[] wallTiles;
  public GameObject[] obstacleTiles;
  public GameObject exit;
  public GameObject enemy;

  private Tile[][] map;
  public List<Room> rooms;
  private GameObject levelMap;

  void Awake()
  {
    Instance = this;
  }

  public void AdjustToData()
  {
    LevelData prevLevel = DataMgr.Instance.GetPreviousLevelData ();
    if (DataMgr.Instance.averageEnemiesKilled < 2.5f)
    {
      mapWidth = Mathf.Max (Mathf.RoundToInt(prevLevel.mapWidth * 0.75f), 20);
      mapHeight = Mathf.Max (Mathf.RoundToInt(prevLevel.mapHeight * 0.75f), 20);
      numRooms = new Point (Mathf.Max(2, Mathf.RoundToInt(prevLevel.numRooms * 0.5f)), Mathf.Min(Mathf.RoundToInt(prevLevel.numRooms * 2.0f), 8));
      roomWidth = new Point (3, Mathf.Min(Mathf.RoundToInt(prevLevel.averageRoomWidth * 1.5f), 5));
      roomHeight = new Point (3, Mathf.Min(Mathf.RoundToInt(prevLevel.averageRoomHeight * 1.5f), 5));
      innerWallDensity = 0.25f;
      obstacleDensity = 0.05f;
      numEnemies += 1;
    }
    else
    {
      mapWidth = Mathf.Min (Mathf.RoundToInt(prevLevel.mapWidth * 2.0f), 50);
      mapHeight = Mathf.Min (Mathf.RoundToInt(prevLevel.mapHeight * 2.0f), 50);
      numRooms = new Point (Mathf.Max(3, Mathf.RoundToInt(prevLevel.numRooms * 1.5f)), Mathf.Min(Mathf.RoundToInt(prevLevel.numRooms * 3.0f), 20));
      roomWidth = new Point (4, Mathf.Min(Mathf.RoundToInt(prevLevel.averageRoomWidth * 1.5f), 12));
      roomHeight = new Point (4, Mathf.Min(Mathf.RoundToInt(prevLevel.averageRoomHeight * 1.5f), 12));
      innerWallDensity = 0.20f;
      obstacleDensity = 0.10f;
      if (DataMgr.Instance.averageEnemiesKilled > 3.0f)
      {
        if (numEnemies > 1)
        {
          numEnemies -= 1;
        }
      }
    }

    if (prevLevel.numRooms == 0 || prevLevel.averageRoomHeight == 0 || prevLevel.averageRoomWidth == 0)
    {
      numRooms = new Point (3, 5);
      roomWidth = new Point (5, 10);
      roomHeight = new Point (5, 10);
    }
  }

  /// <summary>
  /// Generates the level.
  /// </summary>
  public void GenerateLevel()
  {
    // Adjust level params based on Data
    AdjustToData ();
    levelMap = new GameObject ("LevelMap");
    rooms = new List<Room> ();
    SetupMapArray ();

    // Create the empty rooms
    GenerateRooms (Random.Range (numRooms.x, numRooms.y));
    InstantiateTiles ();

    // Place the Player and Exit
    float playerX = rooms[0].X + Mathf.Floor(rooms[0].Width / 2);
    float playerY = rooms[0].Y + Mathf.Floor(rooms[0].Height / 2);
    map [(int)playerY] [(int)playerX] = Tile.Path;
    GameObject.Find ("Player").gameObject.transform.position = new Vector2 (playerX, playerY);
    Point exitPoint = GetWalkablePoint (rooms [1]);
    map [exitPoint.y] [exitPoint.x] = Tile.Path;
    Instantiate (exit, new Vector3 (exitPoint.x, exitPoint.y, -1.0f), Quaternion.identity);

    // Clear a path from the Player to the Exit
    // and mark these tiles so they can't be blocked
    ClearAPath (new Vector2 (playerX, playerY), new Vector2 (exitPoint.x, exitPoint.y), PlayerMgr.Instance.gameObject.GetComponent<PlayerController> ().obstacleLayer, 80);

    // Place the enemies
    GenerateEnemies (new Vector2 (exitPoint.x, exitPoint.y));

    // Destroy the current representation
    // of the map that we needed for initial A*
    Destroy(levelMap);
    levelMap = new GameObject ("LevelMap");

    // Add the obstacles and re-instantiate the tiles
    AddInteralObstacles();
    InstantiateTiles ();
    InformDataManager ();
  }

  void ClearAPath(Vector3 start, Vector3 end, LayerMask obstacleLayer, int tries)
  {
    if (tries <= 0)
    {
      Debug.Log ("Bailing");
    }
    if (map [(int)start.y] [(int)start.x] != Tile.Path)
    {
      map [(int)start.y] [(int)start.x] = Tile.Path;
      map [(int)end.y] [(int)end.x] = Tile.Path;
    }
    Point pathPoint = Pathfinding.Instance.MovePickerA (new Point(start), new Point(end), obstacleLayer);
    if (start.x != end.x || start.y != end.y)
    {
      if (pathPoint.x == -1 && pathPoint.y == -1)
      {
        Debug.Log ("Bad Path!");
        return;
      }
      map [pathPoint.y] [pathPoint.x] = Tile.Path;
    }
    else
    {
      Debug.Log ("Path found!");
      return;
    }
    start = new Vector3 (pathPoint.x, pathPoint.y);
    tries -= 1;
    ClearAPath (start, end, obstacleLayer, tries);
  }

  /// <summary>
  /// Updates the Data Manager with the
  /// tracked values of the level
  /// </summary>
  void InformDataManager()
  {
    DataMgr.Instance.currentLevel.mapWidth = mapWidth;
    DataMgr.Instance.currentLevel.mapHeight = mapHeight;
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
   void GenerateEnemies(Vector2 exitPos)
  {
    List<GameObject> tempEnemy = new List<GameObject> ();
    List<EnemyData> nextGen = new List<EnemyData>();
    if (EvolutionMgr.Instance != null)
    {
      if (DataMgr.Instance.levelsPlayed == 1)
      {
        nextGen = EvolutionMgr.Instance.FirstGen();
      } else {
        if (GameMgr.Instance.previousGen.Count != 0)
        {
          EvolutionMgr.Instance.population = GameMgr.Instance.previousGen;
          nextGen = EvolutionMgr.Instance.Evolve();
        }
      }
    }
    for (int i = 0; i < numEnemies; i++)
    {
      int roomNum = Random.Range (0, rooms.Count);
      Point randPos = GetWalkablePoint (rooms [roomNum]);
      map [randPos.y] [randPos.x] = Tile.Path;
      ClearAPath (new Vector2(randPos.x, randPos.y), exitPos, PlayerMgr.Instance.gameObject.GetComponent<PlayerController> ().obstacleLayer, 80);
      Debug.Log (randPos.x + " " + randPos.y);
      var newEnemy = (GameObject)Instantiate (enemy, new Vector2 (randPos.x, randPos.y), Quaternion.identity);
      if (nextGen.Count > 0 && nextGen.Count > i)
      {
        newEnemy.GetComponent<Enemy> ().stats = nextGen[i];
      }
      tempEnemy.Add (newEnemy);
    }
    GameMgr.Instance.ClearPreviousGen ();
    foreach (GameObject e in tempEnemy)
    {
      GameMgr.Instance.AddEnemy (e.GetComponent<Enemy> ());
    }
  }

  Point GetWalkablePoint(Room room)
  {
    Point chosen = room.GetRandomPoint ();
    int tries = 80;
    while (map [chosen.y] [chosen.x] != Tile.Ground)
    {
      chosen = room.GetRandomPoint ();
      if (tries <= 0)
      {
        Debug.Log ("Ran out of tries. We want to avoid infinitely looping.");
        return chosen;
      }
      tries -= 1;
    }
    return chosen;
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

  int GetRoomIndexFromPt(Point pt)
  {
    for (int i = 0; i < this.rooms.Count; i++)
    {
      if (this.rooms [i].ContainsPt (pt))
      {
        return i;
      }
    }
    return -1;
  }

  /// <summary>
  /// Makes the path between two 
  /// Points in the X-Direction
  /// </summary>
  /// <returns>The Point where the path ends</returns>
  /// <param name="leftPt">The left Point of the given Points</param>
  /// <param name="rightPt">The right Point of the given Points</param>
  Point MakeXPath(Point leftPt, Point rightPt, Room prevRoom)
  {
    Point endPoint = leftPt;
    for (int x = leftPt.x; x < rightPt.x; x++)
    {
      map [leftPt.y] [x] = Tile.Path;
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
  void MakeYPath(Point topPt, Point bottomPt, Room prevRoom)
  {
    for (int y = topPt.y; y <= bottomPt.y; y++)
    {
      map [y] [topPt.x] = Tile.Path;
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

    bool switched = false;

    // Make the Horizontal Path
    if (source.x < target.x)
    {
      source.x += 1;
      source = MakeXPath (source, target, rOne);
    }
    else
    {
      target.x += 1;
      Point temp = MakeXPath (target, source, rTwo);
      target = source;
      source = temp;
      switched = true;
    }

    // Make the Vertical Path
    if (source.y < target.y)
    {
      source.y += 1;
      if (switched)
      {
        MakeYPath (source, target, rTwo);
      }
      else
      {
        MakeYPath (source, target, rOne);
      }
    }
    else
    {
      target.y += 1;
      if (switched)
      {
        MakeYPath (target, source, rTwo);
      }
      else
      {
        MakeYPath (target, source, rOne);
      }
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
      for (int y = room.Top; y <= room.Bottom; y++)
      {
        for (int x = room.Left; x <= room.Right; x++)
        {
          map[y][x] = Tile.Ground;
        }
      }
      return true;
    }
    return false;
  }

  void AddInternalWalls(Room room)
  {
    float roomArea = room.Height * room.Width;
    int maxWallTiles = Mathf.RoundToInt(roomArea * innerWallDensity);
    Debug.Log ("Max tiles: " + maxWallTiles);

    int numWalls = 0;
    float wallProb = Random.value;

    if (wallProb <= 0.10f)
    {
      numWalls = 0;
    }
    else if (wallProb <= 0.40f)
    {
      numWalls = 1;
    }
    else if (wallProb <= 0.80f)
    {
      numWalls = 2;
    }
    else
    {
      numWalls = 3;
    }

    if (numWalls == 0)
    {
      return;
    }
    int tilePerWall = Mathf.RoundToInt (maxWallTiles / numWalls);

    for (int i = 0; i < numWalls; i++)
    {
      // TOP, BOTTOM, LEFT, RIGHT
      int wallChoice = Random.Range (0, 4);
      Point startingPoint = new Point ();
      switch (wallChoice)
      {
        // TOP
        case 0:
          startingPoint.x = Random.Range (room.Left, room.Right);
          startingPoint.y = room.Top;
          break;
      
        // BOTTOM
        case 1:
          startingPoint.x = Random.Range (room.Left, room.Right);
          startingPoint.y = room.Bottom;
          break;

        // LEFT
        case 2:
          startingPoint.x = room.Left;
          startingPoint.y = Random.Range (room.Top, room.Bottom);
          break;

        // RIGHT
        case 3:
          startingPoint.x = room.Right;
          startingPoint.y = Random.Range (room.Top, room.Bottom);
          break;
      }
      BuildInnerWall (tilePerWall, wallChoice, startingPoint, room);
    }
  }

  void BuildInnerWall(int numLeft, int lastPlaced, Point currentPos, Room room)
  {
    if (numLeft < 0 || 
        currentPos.x < 0 || currentPos.y < 0 ||
        currentPos.x >= mapWidth || currentPos.y >= mapWidth)
    {
      // We are done
      return;
    }

    // Create the wall tile
    Debug.Log("Point: " + currentPos.y + ", " + currentPos.x);
    if (map [currentPos.y] [currentPos.x] != Tile.Path)
    {
      map [currentPos.y] [currentPos.x] = Tile.InnerWall;
    }
    Point nextPos = currentPos;
    int nextPlacement = -1;

    // 66% we continue same direction
    // 33% we change direction
    float dirChoice = Random.value;

    if (dirChoice <= 0.16f)
    { 
      // Left
      switch (lastPlaced)
      {
        case 0:
          nextPos.x -= 1;
          nextPlacement = 3; // From the Right
          break;
        case 1:
          nextPos.x -= 1;
          nextPlacement = 3; // From the Right
          break;
        case 2:
          nextPos.y -= 1;
          nextPlacement = 1; // From the Bottom
          break;
        case 3:
          nextPos.y += 1;
          nextPlacement = 0; // From the Top
          break;
      }
    }
    else if (dirChoice <= 0.33f)
    {
      // Right
      switch (lastPlaced)
      {
        case 0:
          nextPos.x += 1;
          nextPlacement = 2; // From the Left
          break;
        case 1:
          nextPos.x += 1;
          nextPlacement = 2; // From the Left
          break;
        case 2:
          nextPos.y += 1;
          nextPlacement = 0; // From the Top
          break;
        case 3:
          nextPos.y -= 1;
          nextPlacement = 1; // From the Bottom
          break;
      }
    }
    else
    {
      nextPlacement = lastPlaced; // Continuing in the same direction
      switch (lastPlaced)
      {
        case 0:
          nextPos.y += 1;
          break;
        case 1:
          nextPos.y -= 1;
          break;
        case 2:
          nextPos.x += 1;
          break;
        case 3:
          nextPos.x -= 1;
          break;
      }
    }

    numLeft -= 1;
    BuildInnerWall (numLeft, nextPlacement, nextPos, room); 
  }

  void AddObstacles(Room room)
  {
    float obstacleChance = Random.value;
    float roomArea = room.roomWidth * room.roomHeight;
    if (obstacleChance <= 0.25f)
    {
      return;
    }
    else if (obstacleChance <= 0.75f)
    {
      for (int i = 0; i < Mathf.RoundToInt(roomArea * obstacleDensity); i++)
      {
        Point obstacleSpot = GetWalkablePoint(room);
        map [obstacleSpot.y] [obstacleSpot.x] = Tile.Obstacle;
      }
    }
    else
    {
      for (int i = 0; i < Mathf.RoundToInt(roomArea * obstacleDensity * 2); i++)
      {
        Point obstacleSpot = GetWalkablePoint(room);
        map [obstacleSpot.y] [obstacleSpot.x] = Tile.Obstacle;
      }
    }
  }

  void GetEntryTiles(Room room)
  {
    Point origin = new Point (room.x - 1, room.y - 1);

    // Left/Right Sides
    for (int i = origin.y; i < (room.y + room.roomHeight); i++)
    {
      if (map [i] [origin.x] == Tile.Ground)
      {
        map [i] [origin.x] = Tile.Path;
        room.AddDoor (new Point (origin.x, i));
      }
      if (map [i] [origin.x + room.roomWidth + 1] == Tile.Ground)
      {
        map [i] [origin.x + room.roomWidth + 1] = Tile.Path;
        room.AddDoor (new Point (origin.x + room.roomWidth + 1, i));
      }
    }

    // Top/Bottom Sides
    for (int i = origin.x; i < (room.x + room.roomWidth); i++)
    {
      if (map [origin.y] [i] == Tile.Ground)
      {
        map [origin.y] [i] = Tile.Path;
        room.AddDoor (new Point (i, origin.y));
      }
      if (map [origin.y + room.roomHeight + 1] [i] == Tile.Ground)
      {
        map [origin.y + room.roomHeight + 1] [i] = Tile.Path;
        room.AddDoor (new Point (i, origin.y + room.roomHeight + 1));
      }
    }
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
      int rWidth = Random.Range (roomWidth.x, roomWidth.y + 1);
      int rHeight = Random.Range (roomHeight.x, roomHeight.y + 1);
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

  void AddInteralObstacles()
  {
    for (int x = 0; x < rooms.Count; x++)
    {
      GetEntryTiles (rooms[x]);
      AddInternalWalls (rooms[x]);
      AddObstacles (rooms[x]);
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
        GameObject tile = wallTiles[Random.Range(0, wallTiles.Length-1)];
        if (map [y] [x] == Tile.Ground)
        {
          tile = groundTiles [0];
        }
        else if (map [y] [x] == Tile.Path)
        {
          //tile = groundTiles [0];
          tile = groundTiles [1];
          // ^ uncomment to see places where obstacles can't be placed
        }
        else if (map [y] [x] == Tile.InnerWall)
        {
          //tile = wallTiles [5];
          // ^ uncomment to see the inner walls
        }
        else if (map [y] [x] == Tile.Obstacle) 
        {
          tile = groundTiles [0];
          GameObject groundInstance = Instantiate (tile, new Vector2 (x, y), Quaternion.identity) as GameObject;
          groundInstance.transform.SetParent (levelMap.transform);
          tile = obstacleTiles [0];
        }

        GameObject tileInstance = Instantiate (tile, new Vector2 (x, y), Quaternion.identity) as GameObject;
        tileInstance.transform.SetParent (levelMap.transform);
    }
  }
}
}
