using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

  public LayerMask obstacleLayer;
  public List<Sprite> enemySprites;
  public float moveSpeed;

	// Use this for initialization
	void Start () {
    int spriteChoice = Random.Range (0, enemySprites.Count);
    GetComponent<SpriteRenderer> ().sprite = enemySprites[spriteChoice];
    GameMgr.Instance.AddEnemy (this);
	}

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
    RaycastHit2D checkValid = Physics2D.Linecast (start, end, obstacleLayer);

    // Collider will be null if the linecast didn't hit an obstacle
    if (checkValid.collider == null)
    {
      StartCoroutine (Move (xMove, yMove));
    }
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
  }
}
