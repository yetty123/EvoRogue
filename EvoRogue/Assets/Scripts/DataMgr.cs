using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataMgr : MonoBehaviour {

  public static DataMgr Instance;

  public LevelData currentLevel;

  public int levelsPlayed;

  // Totals
  int totalMoves;
  int totalAttacks;
  int totalDmgGiven;
  int totalDmgTaken;
  int totalNumEnemies;
  int totalEnemiesKilled;
  int totalNumRooms;
  float totalAvgRoomWidth;
  float totalAvgRoomHeight;
  int totalRoomsEntered;
  public int score;
  
  // Averages
  public float averageMoves;
  public float averageAttacks;
  public float averageDmgGiven;
  public float averageDmgTaken;
  public float averageNumEnemies;
  public float averageEnemiesKilled;
  public float averageNumRooms;
  public float averageRoomWidth;
  public float averageRoomHeight;
  public float averageRoomsEntered;

  List<LevelData> levels;

  void Awake () 
  {
    Instance = this;
    levels = new List<LevelData> ();
    currentLevel = new LevelData ();

    // Initialize all totals
    levelsPlayed = 0;
    totalMoves = 0;
    totalAttacks = 0;
    totalDmgGiven = 0;
    totalDmgTaken = 0;
    totalNumEnemies = 0;
    totalEnemiesKilled = 0;
    totalNumRooms = 0;
    totalAvgRoomWidth = 0;
    totalAvgRoomHeight = 0;
    totalRoomsEntered = 0;
    score = 0;

    // Initialize all per-level averages
    averageMoves = 0;
    averageAttacks = 0;
    averageDmgGiven = 0;
    averageDmgTaken = 0;
    averageNumEnemies = 0;
    averageEnemiesKilled = 0;
    averageNumRooms = 0;
    averageRoomWidth = 0;
    averageRoomHeight = 0;
    averageRoomsEntered = 0;
  }

  public LevelData GetPreviousLevelData()
  {
    if (levels.Count == 0)
    {
      return new LevelData ();
    }
    else
    {
      return levels [levels.Count - 1];
    }
  }

  /// <summary>
  /// Adds the current level to the list
  /// of levels that have been played, resets
  /// the states of the 'currentLevel' variable
  /// and calculates the stats based on the level
  /// that was just completed
  /// </summary>
  public void PrepareForNextLevel ()
  {
    levels.Add (currentLevel);
    currentLevel = new LevelData ();
    CalculateStats ();
  }

  /// <summary>
  /// Calculates the total and average stats.
  /// </summary>
  public void CalculateStats ()
  {
    levelsPlayed += 1;
    CalculateTotals ();
    CalculateAverages ();
  }

  /// <summary>
  /// Calculates the totals.
  /// </summary>
  void CalculateTotals ()
  {
    LevelData ld = levels[levels.Count - 1];
    totalMoves += ld.numMoves;
    totalAttacks += ld.numAttacks;
    totalDmgGiven += ld.damageGiven;
    totalDmgTaken += ld.damageTaken;
    totalNumEnemies += ld.numEnemies;
    totalEnemiesKilled += ld.enemiesKilled;
    totalNumRooms += ld.numRooms;
    totalAvgRoomWidth += ld.averageRoomWidth;
    totalAvgRoomHeight += ld.averageRoomHeight;
    totalRoomsEntered += ld.roomsEntered;
  }

  /// <summary>
  /// Calculates the averages.
  /// </summary>
  void CalculateAverages ()
  {
    averageMoves = totalMoves / levelsPlayed;
    averageAttacks = totalAttacks / levelsPlayed;
    averageDmgGiven = totalDmgGiven / levelsPlayed;
    averageDmgTaken = totalDmgTaken / levelsPlayed;
    averageNumEnemies = totalNumEnemies / levelsPlayed;
    averageEnemiesKilled = (float)totalEnemiesKilled / levelsPlayed;
    averageNumRooms = totalNumRooms / levelsPlayed;
    averageRoomWidth = totalAvgRoomWidth / levelsPlayed;
    averageRoomHeight = totalAvgRoomHeight / levelsPlayed;
    averageRoomsEntered = totalRoomsEntered / levelsPlayed;
  }

}

public struct LevelData
{
  public int level;
  public int mapWidth;
  public int mapHeight;
  public int numMoves;
  public int numRooms;
  public int roomsEntered;
  public float averageRoomWidth;
  public float averageRoomHeight;
  public int numAttacks, damageGiven, damageTaken;
  public int numEnemies, enemiesKilled;
}

