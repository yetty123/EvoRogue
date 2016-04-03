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

  /// <summary>
  /// Defend the specified attack from an Enemy.
  /// </summary>
  /// <param name="attack">The attack power from the Enemy</param>
  public void Defend(int attack)
  {
    int damage = Mathf.Max(attack - defense, 0);
    DataMgr.Instance.currentLevel.damageTaken += damage;
    health -= damage;
    Debug.Log ("Enemy attacks Player for: " + damage + " damage!");
  }
}
