using UnityEngine;
using System;
using System.Collections.Generic;

public class EvolutionMgr : MonoBehaviour
{
    public List<Enemy> population;
    public DataMgr data;
    private List<Enemy> parents;

    // initialization
    void Start()
    {
        this.population = GameMgr.Instance.enemies;
        this.data = DataMgr.Instance;
    }

    // fitness function - which current enemies did the best?
    // fitness is how it is for stats, combined with how it did in combat
    int fitness(Enemy enemy)
    {
        // ADD IN SOMETHING ABOUT BEING ALIVE IF TURNS IN COMBAT IS MORE THAN 0
        int statNormalization = enemy.attackPower + enemy.defense + enemy.health;
        return enemy.damageDone + enemy.turnsInCombat + statNormalization;
    }

    // generate the next level's enemies
    //List<Enemy> evolve()
    //{
      //  List<Enemy> newEnemies;
        

        //return newEnemies;
    //}

    // slightly mutate a single enemy
    Enemy mutate(Enemy enemy)
    {
        int choice = UnityEngine.Random.Range(0, 4);
        switch (choice)
        {
            case 0:
                enemy.moveSpeed += 1;
                break;
            case 1:
                enemy.health += 1;
                break;
            case 2:
                enemy.attackPower += 1;
                break;
            case 3:
                enemy.defense += 1;
                break;
            default:
                Debug.Log("Switch error");
                break;

        }
        return enemy;
    }
}
