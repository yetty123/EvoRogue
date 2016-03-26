using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMgr : MonoBehaviour {

  public static GameMgr Instance;

  public bool playersTurn = true;

  private List<Enemy> enemies;
  private bool enemiesMoving;

	void Awake () {
    enemies = new List<Enemy> ();
    Instance = this;
	}
	
	// Update is called once per frame
	void Update () 
  {
    if (playersTurn || enemiesMoving)
    {
      return;
    }

    StartCoroutine (MoveEnemies ());
	}

  public void AddEnemy(Enemy enemy)
  {
    enemies.Add (enemy);
  }

  IEnumerator MoveEnemies()
  {
    enemiesMoving = true;
    for (int i = 0; i < enemies.Count; i++)
    {
      enemies[i].TryMove ();
      yield return null;
    }

    playersTurn = true;
    enemiesMoving = false;
  }
}
