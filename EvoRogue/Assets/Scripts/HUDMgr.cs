using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

  private List<string> logList;

	// Use this for initialization
	void Awake () 
  {
    Instance = this;
    logList = new List<string> ();
    logList.Add (">");
    logText.text = ">";
	}

  public void PrintAction(string newContent)
  {
    logList.Insert (0, newContent);
    UpdateLog ();
  }

  void UpdateLog()
  {
    string message = "";
    int toDisplay = (logList.Count > 2) ? 2 : logList.Count - 1;
    for (int i = toDisplay; i >= 0; i--)
    {
      if (!logList[i].Equals (">") && logList.Count > 1)
      {
        if (i == 0)
        {
          message += "> ";
        }
        message += logList[i] + "\n";
      }
    }
    logText.text = message;
  }
	
	// Update is called once per frame
	void Update () 
  {
    healthText.text = HEALTH + PlayerMgr.Instance.GetHealth().ToString();
    attackText.text = ATT + PlayerMgr.Instance.GetAttack().ToString();
    defenseText.text = DEF + PlayerMgr.Instance.GetDefense().ToString();
    floorText.text = FLOOR + "1";
    scoreText.text = SCORE + "0";
    UpdateLog ();
	}
}
