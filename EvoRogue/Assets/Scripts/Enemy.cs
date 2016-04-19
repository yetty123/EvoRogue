using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

  public LayerMask obstacleLayer;
  public List<Sprite> enemySprites;
  public float moveSpeed;

  public EnemyData stats = new EnemyData(1, 1, 1, 1, 2, 1.0f);
  Point movement;
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
	}

  /// <summary>
  /// Try to make a move
  /// </summary>
  public void TryMove() 
  {

    if (!playerSighted)    
      movement = MovePickerRandom();
    else  
      movement = MovePickerA();


    Vector3 start = transform.position;
    Vector3 end = start + new Vector3 (movement.x, movement.y);

    // Check if we can move to the next tile
    RaycastHit2D checkValid = Physics2D.Linecast (end, end, obstacleLayer);

    // Collider will be null if the linecast didn't hit an obstacle
    if (checkValid.collider == null || checkValid.collider.transform == this.transform.GetChild(0))
    {
      StartCoroutine (Move (movement));
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

  List<Node> SearchableNodes(Point coordinate)
  {
    List<Node> ReachableNodes = new List<Node>();

    Vector3 start = new Vector3 (coordinate.x, coordinate.y);

    //assign locations to all directions that can be taken
    Vector3[] end = new Vector3[4] {
      start + new Vector3 (1.0f, 0),
      start + new Vector3 (-1.0f, 0),
      start + new Vector3 (0, 1.0f),
      start + new Vector3 (0, -1.0f)
    };

    /*
    for (int i = 0; i < 4; i++)
    {
      // Check if we can move to the next tile
      RaycastHit2D checkValid = Physics2D.Linecast (start, end[i], obstacleLayer);

      // Collider will be null if the linecast didn't hit an obstacle
      if (checkValid.collider == null || checkValid.collider.transform == this.transform.GetChild(0))
      {
        ReachableNodes.Add (new Point ((int)end[i].x, (int)end[i].y));
      }
      else if (checkValid.collider.gameObject.tag == "Player")
      {
        ReachableNodes.Clear ();
        ReachableNodes.Add (new Point ((int)end[i].x, (int)end[i].y));
        return ReachableNodes;
      }
    }
*/
    return ReachableNodes;
  }

  /// <summary>
  /// move A*
  /// </summary>
  Point MovePickerA()
  {
    //get the location of theplayer and enemy to figure out their coordinates
    Vector3 playerLocation = PlayerMgr.Instance.transform.position;
    Vector3 enemyLocation = transform.position;
    //Debug.Log("player = " + playerLocation + "   enemy = " + enemyLocation);

    //Assign the player and enemy coordinates based on their positions in the game
    Point playerCoordinate = new Point ((int)playerLocation.x, (int)playerLocation.y);
    Point enemyCoordinate = new Point ((int)enemyLocation.x, (int)enemyLocation.y);
    //Node currentNode = new Node (enemyCoordinate, enemyCoordinate.TravelCost (playerCoordinate), 0);

    //create an open and closed list as required for A*
    //List<Node> OpenList = SearchableNodes (currentNode);
    //List<Node> ClosedList = new List<Node>(){currentNode};


    /*
    while(OpenList.Count > 0)
    {
      //succesfully found the destination node
      if (OpenList[0].coordinate = playerCoordinate)
      {
        //find the first move to be made in the path found
    
        //create a temporary node for traversal and make it the destination node
        Node traversalNode = OpenList[0];
        while (traversalNode.parent.parent != traversalNode.parent)
        {
          traversalNode = traversalNode.parent;
        }

        return traversalNode.parent.coordinate;
      }


    }
*/
    return new Point();
  }

  /// <summary>
  /// Random move
  /// </summary>
  Point MovePickerRandom()
  {
    switch (Random.Range (0, 5))
    {
      case 0:
        return new Point(0,0);
      case 1:
        return new Point(0,1);
      case 2:
        return new Point(0,-1);
      case 3:
        return new Point(1,0);
      case 4:
        return new Point(-1,0);
    }
    return new Point(0,0);
  }


  /// <summary>
  /// Attack the Player
  /// </summary>
  void Attack()
  {
    PlayerMgr.Instance.Defend (stats.attackPower);
  }

  /// <summary>
  /// Defend the specified attack.
  /// </summary>
  /// <param name="attack">The attack power from the Player</param>
  public int Defend(int attack)
  {
    int damage = Mathf.Max (attack - stats.defense, 0);
    HUDMgr.Instance.PrintAction ("Player attacks Enemy for: " + damage + " damage!");
    DataMgr.Instance.currentLevel.damageGiven += damage;
    stats.currentHealth -= damage;
    if (stats.currentHealth < 0)
    {
      DataMgr.Instance.currentLevel.enemiesKilled += 1;
      GameMgr.Instance.KillEnemy (this);
      Destroy (gameObject);
      HUDMgr.Instance.PrintAction("Enemy killed for 10 xp!");
      return 10;
    }
    return 0;
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

  public void SetCurrentHealth(int value)
  {
    stats.currentHealth = value;
  }

  public void SetMaxHealth(int value)
  {
    stats.maxHealth = value;
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

  public int GetMaxHealth()
  {
    return stats.maxHealth;
  }

  public int GetCurrentHealth()
  {
    return stats.currentHealth;
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

  public Node(Point Coordinate, int Heuristic, int MovementCost, Node Parent)
  {
    coordinate = Coordinate;
    heuristic = Heuristic;
    movementCost = MovementCost;
    totalCost = Heuristic + MovementCost;
    parent = Parent;
  }
}

[System.Serializable]
public class EnemyData
{
    public int attackPower;
    public int defense;
    public int maxHealth;
    public int currentHealth;
    public int damageDone;
    public int combatTurns;
    public int energy;
    public int range;
    public float accuracy;
    public bool alive;
  
  public EnemyData()
  {
    this.attackPower = 1;
    this.defense = 1;
    this.maxHealth = 1;
    this.currentHealth = 1;
    this.damageDone = 0;
    this.combatTurns = 0;
    this.energy = 1;
    this.accuracy = 1;
    this.alive = true;
  }

  public EnemyData(int att, int def, int hp, int energy, int range, float accuracy)
  {
    this.attackPower = att;
    this.defense = def;
    this.maxHealth = hp;
    this.currentHealth = hp;
    this.damageDone = 0;
    this.combatTurns = 0;
    this.energy = energy;
    this.range = range;
    this.accuracy = accuracy;
    this.alive = true;
  }

  public void SetAttackPower(int val)
  {
    attackPower = val;
  }

  public void SetDefense(int val)
  {
    defense = val;
  }

  public void SetHealth(int val)
  {
    currentHealth = val;
    maxHealth = val;
  }

  public void SetEnergy(int val)
  {
    energy = val;
  }

  public void SetAccuracy(float val)
  {
    accuracy = val;
  }
}
