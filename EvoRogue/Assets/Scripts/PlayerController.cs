using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

  public LayerMask obstacleLayer;
  public float moveSpeed;
  public bool moving;

  // Use this for initialization
  void Start ()
  {
    obstacleLayer |= 1 << LayerMask.NameToLayer ("Enemy");
    moving = false;
  }
	
  // Update is called once per frame
  void Update ()
  {

    int xMove = 0;
    int yMove = 0;

    if (Input.GetKeyDown (KeyCode.LeftArrow))
      xMove = -1;
    else if (Input.GetKeyDown (KeyCode.RightArrow))
      xMove = 1;
    else if (Input.GetKeyDown (KeyCode.UpArrow))
      yMove = 1;
    else if (Input.GetKeyDown (KeyCode.DownArrow))
      yMove = -1;

    if (xMove != 0)
      yMove = 0;
    else if (yMove != 0)
      xMove = 0;

    if ((xMove != 0 || yMove != 0) && !moving && GameMgr.Instance.playersTurn)
    {
      Vector3 start = transform.position;
      Vector3 end = start + new Vector3 (xMove, yMove);

      // Check if we can move to the next tile
      RaycastHit2D checkValid = Physics2D.Linecast (start, end, obstacleLayer);

      // Collider will be null if the linecast didn't hit an obstacle
      if (checkValid.collider == null)
      {
        StartCoroutine (Move (xMove, yMove));
        moving = true;
        DataMgr.Instance.currentLevel.numMoves += 1;
      }
      else if (checkValid.collider.gameObject.tag == "Enemy")
      {
        Attack (checkValid.collider.gameObject);
      }
    }
  }

  void Attack(GameObject enemy)
  {
    Debug.Log ("Player attacks Enemy");
    DataMgr.Instance.currentLevel.numAttacks += 1;
    enemy.GetComponent<Enemy> ().Defend (Player.Instance.attackPower);
  }

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

    // Allows the player to move again
    // now that the current move is complete
    moving = false;
    GameMgr.Instance.playersTurn = false;
  }
}
