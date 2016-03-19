using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

  Rigidbody2D body;

	// Use this for initialization
	void Start () {
    body = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
    Vector2 AxisInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
    body.AddForce (AxisInput * 2f, ForceMode2D.Force);
	}
}
