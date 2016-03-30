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

  public void ResetLevel()
  {
    Destroy (GameObject.Find("MapHolder"));
    Destroy (GameObject.Find ("Exit(Clone)"));
    for (int i = 0; i < enemies.Count; i++)
    {
      Destroy (enemies[i].gameObject);
    }
    enemies.Clear ();
    MapGenerator.Instance.GenerateLevel ();
  }

  public void AddEnemy(Enemy enemy)
  {
    enemies.Add (enemy);
  }

  public void KillEnemy(Enemy enemy)
  {
    Debug.Log ("Enemy killed");
    enemies.Remove (enemy);
  }

  IEnumerator MoveEnemies()
  {
    enemiesMoving = true;
    for (int i = 0; i < enemies.Count; i++)
    {
      enemies[i].TryMove ();
      // Wait to prevent enemies from occupying
      // the same tile. We should find a better
      // way of doing this.
      yield return new WaitForSeconds (0.01f);;
    }

    playersTurn = true;
    enemiesMoving = false;
  }
}
