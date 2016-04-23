using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

  public LayerMask obstacleLayer;
  public List<Sprite> enemySprites;
  public float moveSpeed;

  public EnemyData stats = new EnemyData(1, 1, 1, 1, 6, 1.0f);
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
	
	  rows = MapGenerator.Instance.mapHeight;
    cols = MapGenerator.Instance.mapWidth;
	}
  

  /// <summary>
  /// Try to make a move
  /// </summary>
  public void TryMove() 
  {
    //get the location of theplayer and enemy to figure out their coordinates
    Vector3 playerLocation = PlayerMgr.Instance.transform.position;
    Vector3 enemyLocation = transform.position;
    //Debug.Log("player = " + playerLocation + "   enemy = " + startLocation);

    if (!playerSighted)
      targetCoordinate = Pathfinding.Instance.MovePickerRandom(enemyLocation);
    else
      targetCoordinate = Pathfinding.Instance.MovePickerA(enemyLocation, playerLocation, obstacleLayer);

    Debug.Log("Target Coordinate: " + targetCoordinate.x + " , " + targetCoordinate.y);

    Point endDirection = new Point(targetCoordinate.x - enemyLocation.x, targetCoordinate.y - enemyLocation.y);
    Vector3 endCoordinate = new Vector3(targetCoordinate.x, targetCoordinate.y);

    //Debug.Log(endCoordinate);

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
    DataMgr.Instance.currentLevel.damageGiven += damage;
    stats.currentHealth -= damage;
    if (stats.currentHealth < 0)
    {
      DataMgr.Instance.currentLevel.enemiesKilled += 1;
      GameMgr.Instance.KillEnemy (this);
      Destroy (gameObject);
      DataMgr.Instance.score += EvolutionMgr.Instance.fitness(this);
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

  public void SetHealth(int value)
  {
    stats.currentHealth = value;
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
    return stats.currentHealth;
  }

  public int GetMaxHealth()
  {
    return stats.maxHealth;
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
  /*
  public override bool Equals(object obj)
  {
    if (obj == null) return false;
    Node objAsNode = obj as Node;
    if (objAsNode == null) return false;
    else return Equals(objAsNode);
  }

  public override int GetHashCode()
  {
    return heuristic;
  }

  public bool Equals(Node other)
  {
    if (other == null) return false;
    return (
      this.coordinate == other.coordinate &&
      this.movementCost == other.movementCost &&
      this.totalCost == other.totalCost &&
      this.heuristic == other.heuristic);
  }
*/
  public static bool operator !=(Node node1, Node node2) 
  {
    return !(
      node1.coordinate == node2.coordinate &&
      node1.movementCost == node2.movementCost &&
      node1.totalCost == node2.totalCost &&
      node1.heuristic == node2.heuristic);
  }

  public static bool operator ==(Node node1, Node node2) 
  {
    return (
      node1.coordinate == node2.coordinate &&
      node1.movementCost == node2.movementCost &&
      node1.totalCost == node2.totalCost &&
      node1.heuristic == node2.heuristic);
  }

  public Node()
  {
    heuristic = 0;
    movementCost = 0;
    totalCost = 0;
    parent = null;
  }

  public Node(Node otherNode)
  {
    coordinate = otherNode.coordinate;
    heuristic = otherNode.heuristic;
    movementCost = otherNode.movementCost;
    totalCost = otherNode.heuristic + otherNode.movementCost;
    parent = otherNode.parent;
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

[System.Serializable]
public class EnemyData
{
    public int attackPower;
    public int defense;
    public int currentHealth;
    public int maxHealth;
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

    // ADD CONSTRUCTORS, SETTERS, GETTERS
  public EnemyData(int att, int def, int hp, int energy, int range, float accuracy)
  {
    this.attackPower = att;
    this.defense = def;
    this.currentHealth = hp;
    this.maxHealth = hp;
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