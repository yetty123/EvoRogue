using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

  public LayerMask obstacleLayer;
  public List<Sprite> enemySprites;
  public float moveSpeed;
  public EnemyData stats = new EnemyData(1, 1, 1, 1, 1.0f);
	
    // Use this for initialization
	void Start () {
    int spriteChoice = Random.Range (0, enemySprites.Count);
    GetComponent<SpriteRenderer> ().sprite = enemySprites[spriteChoice];

    obstacleLayer |= 1 << LayerMask.NameToLayer ("Player");
    obstacleLayer |= 1 << LayerMask.NameToLayer ("Enemy");
    GameMgr.Instance.AddEnemy (this);
	}

  /// <summary>
  /// Try to make a move
  /// </summary>
  public void TryMove() {

    int xMove = 0;
    int yMove = 0;

    // #AI
    int choice = Random.Range (1, 5);
    switch (choice)
    {
      case 1:
        yMove = 1;
        break;
      case 2:
        yMove = -1;
        break;
      case 3:
        xMove = -1;
        break;
      case 4:
        xMove = 1;
        break;
    }

    Vector3 start = transform.position;
    Vector3 end = start + new Vector3 (xMove, yMove);

    // Check if we can move to the next tile
    RaycastHit2D checkValid = Physics2D.Linecast (end, end, obstacleLayer);

    // Collider will be null if the linecast didn't hit an obstacle
    if (checkValid.collider == null)
    {
      StartCoroutine (Move (xMove, yMove));
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
    if (stats.health <= 0)
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
  IEnumerator Move(int x, int y)
  {
    // Where the player is moving
    Vector3 end = transform.position + new Vector3 (x, y);

    // Move the player towards the destination
    while (Vector3.Distance(transform.position, end) > 0)
    {
      transform.position = Vector3.MoveTowards (transform.position, end, moveSpeed);
      yield return null;
    }
  }
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


public class EnemyData
{
    public int attackPower;
    public int defense;
    public int health;
    public int damageDone;
    public int combatTurns;
    public int energy;
    public float accuracy; // float to use as a multiplier
    public bool alive;

    // ADD CONSTRUCTORS, SETTERS, GETTERS
  public EnemyData(int att, int def, int hp, int energy, float accuracy)
  {
    this.attackPower = att;
    this.defense = def;
    this.health = hp;
    this.damageDone = 0;
    this.combatTurns = 0;
    this.energy = energy;
    this.accuracy = accuracy;
    this.alive = true;
  }
}