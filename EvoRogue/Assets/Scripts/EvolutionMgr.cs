using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class EvolutionMgr : MonoBehaviour
{
    public static EvolutionMgr Instance;
    public List<Enemy> population;
    public DataMgr data;
    private List<Enemy> parents;

    // initialization
    void Start()
    {
    this.population = GameMgr.Instance.previousGen;
        this.data = DataMgr.Instance;
        Instance = this;
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
  public List<EnemyData> Evolve()
    {
        // calculate fitness values
        List<int> fitnessVals = new List<int>();
        Debug.Log (population.Count);
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

        List<EnemyData> nextGen = new List<EnemyData>();
        Enemy mom;
        Enemy dad;
        int mutationChance = 0;
        // for now, constant 5 enemies per level
        for (int i = 0; i < 5; i++)
        {
            Debug.Log (population.Count);
            // create a child based on two high fitness enemies
            mom = population[Random.Range(0, population.Count)];
            dad = population[Random.Range(0, population.Count)];
            EnemyData child = new EnemyData();
            child.SetAttackPower(mom.GetAttackPower() + 1);
            child.SetHealth(dad.GetHealth() + 1);
            child.SetDefense(mom.GetDefense() + 1);
            child.SetEnergy(dad.GetEnergy() + 1);
            child.SetAccuracy(mom.GetAccuracy() + 1);

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
    EnemyData mutate(EnemyData enemy)
    {
        int choice = UnityEngine.Random.Range(0, 4);
        switch (choice)
        {
            case 0:
                enemy.SetEnergy(enemy.energy + 1);
                break;
            case 1:
                enemy.SetHealth(enemy.health + 1);
                break;
            case 2:
                enemy.SetAttackPower(enemy.attackPower + 1);
                break;
            case 3:
                enemy.SetDefense(enemy.defense + 1);
                break;
            default:
                Debug.Log("Switch error");
                break;

        }
        return enemy;
    }
}
