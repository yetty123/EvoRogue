using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

  public LayerMask obstacleLayer;
  public float moveSpeed;
  public bool moving;
  int experience;
public int maxEnergy = 3;
	public int currentEnergy = 3; 
	public static PlayerController Instance;

  void Start ()
  {
	Instance = this;
    obstacleLayer |= 1 << LayerMask.NameToLayer ("Enemy");
    moving = false;
  }
	
  void Update ()
  {

    Point move = MoveAttempt ();

    if ((move.x != 0 || move.y != 0) && !moving && GameMgr.Instance.playersTurn)
    {
      Vector3 start = transform.position;
      Vector3 end = start + new Vector3 (move.x, move.y);

      // Check if we can move to the next tile
      RaycastHit2D checkValid = Physics2D.Linecast(start, end, obstacleLayer);

      Debug.Log (checkValid.collider);

      // Collider will be null if the linecast didn't hit an obstacle
      if (checkValid.collider == null)
      {
        StartCoroutine (Move (move));
        moving = true;
        currentEnergy--;
        DataMgr.Instance.currentLevel.numMoves += 1;
      }
      else if (checkValid.collider.gameObject.tag == "Enemy")
      {
      	currentEnergy--;
        Attack (checkValid.collider.gameObject);
      }
    }
  }


  /// <summary>
   /// return the movement value specified by player inputs
  /// </summary>
  Point MoveAttempt()
  {
    if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.A))
      return new Point (-1, 0);
    else if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.D))
      return new Point (1, 0);
    else if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W))
      return new Point (0, 1);
    else if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S))
      return new Point (0, -1);

    return new Point (0, 0);
  }

  /// <summary>
  /// Attack the specified enemy.
  /// </summary>
  /// <param name="enemy">The Enemy to attack</param>
  void Attack(GameObject enemy)
  {
    Debug.Log ("Player attacks Enemy");
    DataMgr.Instance.currentLevel.numAttacks += 1;
    int xp = (enemy.GetComponent<Enemy>()).Defend(PlayerMgr.Instance.attackPower);
    if (xp > 0) {
      PlayerMgr.Instance.experience += xp;
      if (PlayerMgr.Instance.CheckLevelUp())
      {
        PlayerMgr.Instance.LevelUp();
      }
    }

		moving = false;
		if (currentEnergy == 0) {
			GameMgr.Instance.playersTurn = false;
			currentEnergy = maxEnergy;
		}
  }

  /// <summary>
  /// Move the specified x and y distances.
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

    // Allows the player to move again
    // now that the current move is complete
    moving = false;
    if (currentEnergy == 0) {
			GameMgr.Instance.playersTurn = false;
			currentEnergy = maxEnergy;
		}
  }
}
