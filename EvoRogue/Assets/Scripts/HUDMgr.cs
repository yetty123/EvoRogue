using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDMgr : MonoBehaviour {

  public static HUDMgr Instance;

  public const string HEALTH = "HEALTH: ";
  public const string ATT = "ATT: ";
  public const string DEF = "DEF: ";
  public const string FLOOR = "FLOOR: ";
  public const string SCORE = "SCORE: ";

  public Text healthText;
  public Text attackText;
  public Text defenseText;
  public Text floorText;
  public Text scoreText;
  public Text logText;

	// Use this for initialization
	void Awake () 
  {
    Instance = this;
	}

  public void PrintAction(string newContent)
  {
    logText.text = newContent;
  }
	
	// Update is called once per frame
	void Update () 
  {
    healthText.text = HEALTH + PlayerMgr.Instance.GetHealth().ToString();
    attackText.text = ATT + PlayerMgr.Instance.GetAttack().ToString();
    defenseText.text = DEF + PlayerMgr.Instance.GetDefense().ToString();
    floorText.text = FLOOR + "1";
    scoreText.text = SCORE + "0";
	}
}
