using UnityEngine;
using System.Collections;

public class PlayerMgr : MonoBehaviour 
{
  public static PlayerMgr Instance;

  public int health;
  public int attackPower;
  public int defense;
  public int experience;
  public int level;

  void Awake()
  {
    Instance = this;
  }

  public int GetHealth()
  {
    return health;
  }

  public int GetAttack()
  {
    return attackPower;
  }

  public int GetDefense()
  {
    return defense;
  }

  public int GetXP()
  {
    return experience;
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
    HUDMgr.Instance.PrintAction ("Enemy attacks Player for: " + damage + " damage!");
  }

  // check if you have enough experience to level up
  // true => you need to level up, false => you don't need to level up
  public bool CheckLevelUp()
  {
    // at level x, if you have enough xp to go to level y, return true
    switch (level)
    {
      case 1:
        if (experience >= 100)
        {
          return true;
        }
        return false;
      case 2:
        if (experience >= 250)
        {
          return true;
        }
        return false;
      case 3:
        if (experience >= 500)
        {
          return true;
        }
        return false;
      case 4:
        if (experience >= 1000)
        {
          return true;
        }
        return false;
      default:
        return false;
    }
  }

  public void LevelUp()
  {
    health += 1;
    attackPower += 1;
    defense += 1;
    level += 1;
    HUDMgr.Instance.PrintAction("You leveled up! You are now level " + level);
  }
}
