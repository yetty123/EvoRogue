using UnityEngine;
using System.Collections;

public class LevelExit : MonoBehaviour 
{

  /// <summary>
  /// To check if the Player is exiting the level
  /// </summary>
  /// <param name="col">The collider of the object that triggered this</param>
  void OnTriggerEnter2D(Collider2D col)
  {
    if (col.gameObject.name == "Player")
    {
      PlayerController pController = GameObject.Find ("Player").GetComponent<PlayerController> ();
      pController.StopAllCoroutines ();
      pController.moving = false;
      PlayerMgr.Instance.experience += 100;
      HUDMgr.Instance.PrintAction("100 experience gained!");
      if (PlayerMgr.Instance.CheckLevelUp())
      {
        PlayerMgr.Instance.LevelUp();
      }
      GameMgr.Instance.ResetLevel();
    }
  }
}
