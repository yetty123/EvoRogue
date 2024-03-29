using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMgr : MonoBehaviour {

  public static GameMgr Instance;
  public bool playersTurn = true;

  public List<Enemy> enemies;
  public List<Enemy> previousGen;
  private bool enemiesMoving;
	int enemiesDone = 0;

	void Awake () {
    enemies = new List<Enemy> ();
    previousGen = new List<Enemy> ();
    Instance = this;
    ResetLevel ();
	}
	
	// Update is called once per frame
	void Update () 
  {
    if (Input.GetKeyDown (KeyCode.G))
    {
      ResetLevel ();
    }
    if (playersTurn || enemiesMoving)
    {
      return;
    }

    StartCoroutine (MoveEnemies ());
	}

  /// <summary>
  /// Clears the current level, updates the data,
  /// and generates a new level
  /// </summary>
  public void ResetLevel()
  {
    Destroy (GameObject.Find("LevelMap"));
    Destroy (GameObject.Find ("Exit(Clone)"));
    for (int i = 0; i < enemies.Count; i++)
    {
      Destroy (enemies[i].gameObject);
    }
    enemies.Clear ();
    DataMgr.Instance.PrepareForNextLevel ();
    MapGenerator.Instance.GenerateLevel ();
    playersTurn = true;
		enemiesMoving = false;
		enemiesDone = 0;
		PlayerController.Instance.currentEnergy = PlayerController.Instance.maxEnergy;
  }

  /// <summary>
  /// Add the given Enemy to the list
  /// </summary>
  /// <param name="enemy">The Enemy being added</param>
  public void AddEnemy(Enemy enemy)
  {
    enemies.Add (enemy);
    previousGen.Add (enemy);
  }

  public void ClearPreviousGen()
  {
    previousGen.Clear ();
  }

  /// <summary>
  /// Kill the given Enemy and remove it
  /// from the list of enemies
  /// </summary>
  /// <param name="enemy">The Enemy to kill</param>
  public void KillEnemy(Enemy enemy)
  {
    Debug.Log ("Enemy killed");
    enemies.Remove (enemy);
  }

  /// <summary>
  /// Moves all the enemies in the level
  /// </summary>
  IEnumerator MoveEnemies()
  {
    // Prevent enemies from moving for a time while Player is moving
    enemiesMoving = true;
    yield return new WaitForSeconds (0.1f); 
    
    if(enemies.Count > 0)
    {
    
    for (int i = 0; i < enemies.Count; i++)
    {
     StartCoroutine (enemies [i].TryMove ());
      // Wait to prevent enemies from occupying
      // the same tile.
      yield return new WaitForSeconds (0.05f);
    }
		yield return new WaitForSeconds (0.1f); 
    }
    else
    {
    	playersTurn = true;
			enemiesMoving = false;
    	
    	
    }
  }

	//called when an enemy is done moving
	public void EnemyDoneMoving()
	{
		if (enemiesDone++ == enemies.Count-1) {
			playersTurn = true;
			enemiesMoving = false;
			enemiesDone = 0;
		}
	}

}
