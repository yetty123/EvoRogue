﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMgr : MonoBehaviour {

  public static GameMgr Instance;

  public bool playersTurn = true;

  public List<Enemy> enemies;
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
  }

  /// <summary>
  /// Add the given Enemy to the list
  /// </summary>
  /// <param name="enemy">The Enemy being added</param>
  public void AddEnemy(Enemy enemy)
  {
    enemies.Add (enemy);
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
