using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataMgr : MonoBehaviour {

  public static DataMgr Instance;

  public LevelData currentLevel;

  int levelsPlayed;

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

  void Start () 
  {
    Instance = this;
    levels = new List<LevelData> ();
    currentLevel = new LevelData ();

    // Initialize all totals
    levelsPlayed = 1;
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

  public void PrepareForNextLevel ()
  {
    levels.Add (currentLevel);
    currentLevel = new LevelData ();
    CalculateStats ();
  }

  public void CalculateStats ()
  {
    levelsPlayed += 1;
    CalculateTotals ();
    CalculateAverages ();
  }

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
    //Debug.Log (totalMoves + " " + totalAttacks + " " + totalNumRooms);
  }

  void CalculateAverages ()
  {
    averageMoves = totalMoves / levelsPlayed;
    averageAttacks = totalAttacks / levelsPlayed;
    averageDmgGiven = totalDmgGiven / levelsPlayed;
    averageDmgTaken = totalDmgTaken / levelsPlayed;
    averageNumEnemies = totalNumEnemies / levelsPlayed;
    averageEnemiesKilled = totalEnemiesKilled / levelsPlayed;
    averageNumRooms = totalNumRooms / levelsPlayed;
    averageRoomWidth = totalAvgRoomWidth / levelsPlayed;
    averageRoomHeight = totalAvgRoomHeight / levelsPlayed;
    averageRoomsEntered = totalRoomsEntered / levelsPlayed;
  }

}

public struct LevelData
{
  public int level;
  public int numMoves;
  public int numRooms;
  public int roomsEntered;
  public float averageRoomWidth;
  public float averageRoomHeight;
  public int numAttacks, damageGiven, damageTaken;
  public int numEnemies, enemiesKilled;
}
