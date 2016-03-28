using UnityEngine;
using System.Collections;

public class LevelExit : MonoBehaviour 
{

  void OnTriggerEnter2D(Collider2D col)
  {
    if (col.gameObject.name == "Player")
    {
      PlayerController pController = GameObject.Find ("Player").GetComponent<PlayerController> ();
      pController.StopAllCoroutines ();
      pController.moving = false;
      GameMgr.Instance.ResetLevel ();
    }
  }
}
