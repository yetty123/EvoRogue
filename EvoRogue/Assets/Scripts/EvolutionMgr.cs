using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
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
    int fitness(int index)
    {
        // ADD IN SOMETHING ABOUT BEING ALIVE IF TURNS IN COMBAT IS MORE THAN 0
        Enemy enemy = population.ElementAt(index);
        int statNormalization = enemy.stats.attackPower + enemy.stats.defense + enemy.stats.health;
        return enemy.stats.damageDone + enemy.stats.combatTurns + statNormalization;
    }

    // generate the next level's enemies
    List<Enemy> evolve()
    {
        // calculate fitness values
        List<int> fitnessVals = new List<int>();
        for(int i = 0; i < population.Count; i++ )
        {
            fitnessVals.Add(fitness(i));
        }

        // get average fitness
        int total = 0;
        int average = 0;
        for (int i = 0; i < fitnessVals.Count; i++)
        {
            total += fitnessVals.ElementAt(i);
        }
        average = total / fitnessVals.Count;

        // remove below average performance enemies
        for (int i = 0; i < fitnessVals.Count; i++)
        {
            if (fitnessVals.ElementAt(i) < average) {
                population.RemoveAt(i);
            }
        }

        List<Enemy> nextGen = new List<Enemy>();
        UnityEngine.Random rand = new UnityEngine.Random();
        Enemy mom;
        Enemy dad;
        int mutationChance = 0;
        // for now, constant 10 enemies per level
        for (int i = 0; i < 10; i++)
        {
            // create a child based on two high fitness enemies
            mom = population.ElementAt(UnityEngine.Random.Range(0, population.Count + 1));
            dad = population.ElementAt(UnityEngine.Random.Range(0, population.Count + 1));
            Enemy child = new Enemy();
            child.SetAttackPower(mom.GetAttackPower());
            child.SetHealth(dad.GetHealth());
            child.SetDefense(mom.GetDefense());
            child.SetEnergy(dad.GetEnergy());
            child.SetAccuracy(mom.GetAccuracy());

            // small chance to mutate the child
            mutationChance = UnityEngine.Random.Range(0, 101);
            if (mutationChance < 5)
            {
                child = mutate(child);
            }
            nextGen.Add(child);

        }

        return nextGen;
    }

    // slightly mutate a single enemy
    Enemy mutate(Enemy enemy)
    {
        int choice = UnityEngine.Random.Range(0, 4);
        switch (choice)
        {
            case 0:
                enemy.stats.energy += 1;
                break;
            case 1:
                enemy.stats.health += 1;
                break;
            case 2:
                enemy.stats.attackPower += 1;
                break;
            case 3:
                enemy.stats.defense += 1;
                break;
            default:
                Debug.Log("Switch error");
                break;

        }
        return enemy;
    }
}
