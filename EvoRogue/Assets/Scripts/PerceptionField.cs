using UnityEngine;
using System.Collections;

public class PerceptionField : MonoBehaviour {

	// Use this for initialization
	void Start () 
  {
    GetComponent<BoxCollider2D>().size = transform.parent.GetComponent<BoxCollider2D>().size * (GetComponentInParent<Enemy>().stats.range + 1);
	}

  void OnTriggerEnter2D(Collider2D other) 
  {
    if(other.name == "Player")   GetComponentInParent<Enemy>().playerSighted = true;
  }

  void OnTriggerExit2D(Collider2D other) 
  {
    if(other.name == "Player")    GetComponentInParent<Enemy>().playerSighted = false;
  }
}
