using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

  public const int DEFAULT_SIZE = 5;
  public const int MAX_SIZE = 15;

  public Transform player;
  public Camera camera;

	// Use this for initialization
	void Start () {
    camera = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
    player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform>();
    transform.position = player.transform.position;
    transform.position = new Vector3 (transform.position.x, transform.position.y, -10f);
    if (Input.GetKeyDown (KeyCode.Z))
    {
      CameraZoom ();
    }
	}

  void CameraZoom()
  {
    if (camera.orthographicSize == MAX_SIZE)
    {
      camera.orthographicSize = DEFAULT_SIZE;
    }
    else
    {
      camera.orthographicSize += 5;
    }
  }
}
