using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

  public LayerMask obstacleLayer;
  public List<Sprite> enemySprites;
  public float moveSpeed;

  public EnemyData stats = new EnemyData(1, 1, 1, 1, 2, 1.0f);
  Point targetCoordinate;
  int rows;
  int cols;
  public bool playerSighted = false;

	// Use this for initialization
	void Start () 
	{
    int spriteChoice = Random.Range (0, enemySprites.Count);
    GetComponent<SpriteRenderer> ().sprite = enemySprites[spriteChoice];


    obstacleLayer |= 1 << LayerMask.NameToLayer ("Player");
    obstacleLayer |= 1 << LayerMask.NameToLayer ("Enemy");
    GameMgr.Instance.AddEnemy (this);
	
	  rows = MapGenerator.Instance.mapHeight;
    cols = MapGenerator.Instance.mapWidth;
	}

  /// <summary>
  /// Try to make a move
  /// </summary>
  public void TryMove() 
  {
    //get the location of theplayer and enemy to figure out their coordinates
    Vector3 playerLocation = Player.Instance.transform.position;
    Vector3 enemyLocation = transform.position;
    //Debug.Log("player = " + playerLocation + "   enemy = " + startLocation);

    if (!playerSighted)
      targetCoordinate = MovePickerRandom(enemyLocation);
    else  
      targetCoordinate = MovePickerA(enemyLocation, playerLocation);


    Point endDirection = new Point(enemyLocation.x - targetCoordinate.x, enemyLocation.y - targetCoordinate.y);
    Vector3 endCoordinate = new Vector3(targetCoordinate.x, targetCoordinate.y);

    // Check if we can move to the next tile
    RaycastHit2D checkValid = Physics2D.Linecast (endCoordinate, endCoordinate, obstacleLayer);

    // Collider will be null if the linecast didn't hit an obstacle
    if (checkValid.collider == null || checkValid.collider.transform == this.transform.GetChild(0))
    {
      StartCoroutine (Move (endDirection));
    }
    else if (checkValid.collider.gameObject.tag == "Player")
    {
      Attack ();
    }
    else
    {
      Debug.Log (checkValid.collider.gameObject.tag);
    }
  }

  List<Node> SearchableNodes(Point currentCoordinate, Point destinationCoordinate, List<Node> OpenNodes)
  {
    List<Node> ReachableNodes = new List<Node>();

    //assign locations to all directions that can be taken
    Point[] end = new Point[4] {
      currentCoordinate + new Point (1, 0),
      currentCoordinate + new Point (-1, 0),
      currentCoordinate + new Point (0, 1),
      currentCoordinate + new Point (0, -1)
    };

    for (int i = 0; i < 4; i++)
    {
      // Check if we can move to the next tile
      Vector3 pointChecking = new Vector3(end[i].x, end[i].y);
      RaycastHit2D checkValid = Physics2D.Linecast (pointChecking, pointChecking, obstacleLayer);

      // Collider will be null if the linecast didn't hit an obstacle
      if (checkValid.collider == null || checkValid.collider.GetComponent<PerceptionField>() != null)
      {
        ReachableNodes.Add (new Node(end[i], end[i].TravelCost(destinationCoordinate), 1));
      }
      else if (checkValid.collider.gameObject.tag == "Player")
      {
        ReachableNodes.Clear ();
        ReachableNodes.Add (new Node(end[i], end[i].TravelCost(destinationCoordinate), 1));
        return ReachableNodes;
      }
    }
    /*
    //time to find the next node to focus on
    for(int i = 0; i < ReachableNodes.Count; i++)
    {
      for(int k = 0; k < OpenNodes.Count; k++)
      {
        if(ReachableNodes[i].coordinate == OpenNodes[k].coordinate && ReachableNodes[i].totalCost < OpenNodes[k].totalCost)
        {

        }
      }
    }
    */
    return ReachableNodes;
  }

  /// <summary>
  /// move A*
  /// </summary>
  Point MovePickerA(Vector3 startLocation, Vector3 endLocation)
  {
    

    //Assign the player and enemy coordinates based on their positions in the game
    Point playerCoordinate = new Point ((int)endLocation.x, (int)endLocation.y);
    Point enemyCoordinate = new Point ((int)startLocation.x, (int)startLocation.y);
    Node currentNode = new Node (enemyCoordinate, enemyCoordinate.TravelCost (playerCoordinate), 0);

    //create an open and closed list as required for A*
    List<Node> OpenList = SearchableNodes (currentNode.coordinate, playerCoordinate, new List<Node>());
    List<Node> ClosedList = new List<Node>(){currentNode};

    List<Node> AdjacentNodes = OpenList;

    while(OpenList.Count > 0)
    {
      //succesfully found the destination node
      if (OpenList[0].coordinate == playerCoordinate)
      {
        //find the first move to be made in the path found
        //create a temporary node for traversal and make it the destination node
        Node traversalNode = OpenList[0];

        //if the nodes parent's parent isn't itself, then keep traversing through the parents
        while (traversalNode.parent.parent != traversalNode.parent)
        {
          traversalNode = traversalNode.parent;
        }

        //return the point to traverse towards
        return traversalNode.parent.coordinate;
      }
      else
      {
        //time to find the next node to focus on
        Node nextNode = OpenList[0];

        for(int i = 0; i < AdjacentNodes.Count; i++)
        {
          for(int k = 0; k < OpenList.Count; k++)
          {
            if(AdjacentNodes[i].coordinate == OpenList[k].coordinate && AdjacentNodes[i].totalCost <= OpenList[k].totalCost)
            {
              OpenList[k].parent = currentNode;
            }
          }
        }
      }


    }

    return new Point();
  }

  /// <summary>
  /// Random move
  /// </summary>
  Point MovePickerRandom(Vector3 currentLocation)
  {
    switch (Random.Range (0, 5))
    {
      case 0:
        return new Point((int)currentLocation.x, (int)currentLocation.y);
      case 1:
        return new Point((int)currentLocation.x, (int)currentLocation.y+1);
      case 2:
        return new Point((int)currentLocation.x, (int)currentLocation.y-1);
      case 3:
        return new Point((int)currentLocation.x+1, (int)currentLocation.y);
      case 4:
        return new Point((int)currentLocation.x-1, (int)currentLocation.y);
    }

    return new Point((int)currentLocation.x, (int)currentLocation.y); 
  }


  /// <summary>
  /// Attack the Player
  /// </summary>
  void Attack()
  {
    Player.Instance.Defend (stats.attackPower);
  }

  /// <summary>
  /// Defend the specified attack.
  /// </summary>
  /// <param name="attack">The attack power from the Player</param>
  public void Defend(int attack)
  {
    int damage = Mathf.Max (attack - stats.defense, 0);
    DataMgr.Instance.currentLevel.damageGiven += damage;
    stats.health -= damage;
    if (stats.health < 0)
    {
      DataMgr.Instance.currentLevel.enemiesKilled += 1;
      GameMgr.Instance.KillEnemy (this);
      Destroy (gameObject);
    }
  }

  /// <summary>
  /// Move the specified x and y.
  /// </summary>
  /// <param name="x">The distance to move in the X-Direction</param>
  /// <param name="y">The distance to move in the Y-Direction</param>
  IEnumerator Move(Point direction)
  {
    // Where the player is moving
    Vector3 end = transform.position + new Vector3 (direction.x, direction.y);

    // Move the player towards the destination
    while (Vector3.Distance(transform.position, end) > 0)
    {
      transform.position = Vector3.MoveTowards (transform.position, end, moveSpeed);
      yield return null;
    }
  }

  public enum CommandType
  {
    Idle,
    Search,
    Combat,
    Flee
  };

  // overly verbose getters and setters
  public void SetAttackPower(int value)
  {
    stats.attackPower = value;
  }

  public void SetDefense(int value)
  {
    stats.defense = value;
  }

  public void SetHealth(int value)
  {
    stats.health = value;
  }

  public void SetEnergy(int value)
  {
    stats.energy = value;
  }

  public void SetAccuracy(float value)
  {
    stats.accuracy = value;
  }

  public int GetAttackPower()
  {
    return stats.attackPower;
  }

  public int GetDefense()
  {
    return stats.defense;
  }

  public int GetHealth()
  {
    return stats.health;
  }

  public int GetEnergy()
  {
    return stats.energy;
  }

  public float GetAccuracy()
  {
    return stats.accuracy;
  }
}

public class Node
{
  public int heuristic;
  public int movementCost;
  public int totalCost;
  public Node parent;
  public Point coordinate;

  public Node()
  {
    heuristic = 0;
    movementCost = 0;
    totalCost = 0;
    parent = null;
  }

  public Node(Point Coordinate, int Heuristic, int MovementCost)
  {
    coordinate = Coordinate;
    heuristic = Heuristic;
    movementCost = MovementCost;
    totalCost = Heuristic + MovementCost;
    parent = this;
  }

  public Node(Point Coordinate, int Heuristic, int MovementCost, Node Parent)
  {
    coordinate = Coordinate;
    heuristic = Heuristic;
    movementCost = MovementCost;
    totalCost = Heuristic + MovementCost;
    parent = Parent;
  }
}

public class EnemyData
{
    public int attackPower;
    public int defense;
    public int health;
    public int damageDone;
    public int combatTurns;
    public int energy;
    public int range;
    public float accuracy;
    public bool alive;

    // ADD CONSTRUCTORS, SETTERS, GETTERS
  public EnemyData(int att, int def, int hp, int energy, int range, float accuracy)
  {
    this.attackPower = att;
    this.defense = def;
    this.health = hp;
    this.damageDone = 0;
    this.combatTurns = 0;
    this.energy = energy;
    this.range = range;
    this.accuracy = accuracy;
    this.alive = true;
  }
}
