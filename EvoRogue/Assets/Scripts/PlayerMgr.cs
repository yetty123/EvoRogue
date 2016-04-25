using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMgr : MonoBehaviour 
{
  public static PlayerMgr Instance;

  private const int EXP_ONE = 100;
  private const int EXP_TWO = 250;
  private const int EXP_THREE = 500;
  private const int EXP_FOUR = 1000;

  public int health;
  public int maxHealth;
  public int attackPower;
  public int defense;
  public int experience;
  public int maxExperience;
  public int level;
	float t= 0;

  void Awake()
  {
    Instance = this;
  }

  public int GetHealth()
  {
    return health;
  }

  public int GetMaxHealth()
  {
    return maxHealth;
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

  public int GetMaxXP()
  {
    return maxExperience;
  }

  public int GetLevel()
  {
    return level;
  }

  /// <summary>
  /// Defend the specified attack from an Enemy.
  /// </summary>
  /// <param name="attack">The attack power from the Enemy</param>
  public void Defend(int attack)
  {
    int damage = Mathf.Max(attack - defense, 0);
    DataMgr.Instance.currentLevel.damageTaken += damage;
	Debug.Log ("Enemy attacks Player for: " + damage + " damage!");
	HUDMgr.Instance.PrintAction ("Enemy attacks Player for: " + damage + " damage!");

		if ((health -= damage) <= 0) {
			HUDMgr.Instance.PrintAction ("PLAYER HAS DIED!!!");
			PlayerController.Instance.gameObject.SetActive (false);
		}
  }

  // check if you have enough experience to level up
  // true => you need to level up, false => you don't need to level up
  public bool CheckLevelUp()
  {
    // at level x, if you have enough xp to go to level y, return true
    switch (level)
    {
      case 1:
        if (experience >= EXP_ONE)
        {
          maxExperience = EXP_TWO;
          return true;
        }
        return false;
      case 2:
        if (experience >= EXP_TWO)
        {
          maxExperience = EXP_THREE;
          return true;
        }
        return false;
      case 3:
        if (experience >= EXP_THREE)
        {
          maxExperience = EXP_FOUR;
          return true;
        }
        return false;
      case 4:
        if (experience >= EXP_FOUR)
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
    health += level;
    maxHealth += level;
    attackPower += level;
    defense += level;
    level += 1;
    HUDMgr.Instance.PrintAction("You leveled up! You are now level " + level);
  }
}
