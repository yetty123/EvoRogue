using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
  public static Player Instance;

  public int health;
  public int attackPower;
  public int defense;

  void Awake()
  {
    Instance = this;
  }

  public void Defend(int attack)
  {
    int damage = Mathf.Max(attack - defense, 0);
    health -= damage;
    Debug.Log ("Enemy attacks Player for: " + damage + " damage!");
  }
}
