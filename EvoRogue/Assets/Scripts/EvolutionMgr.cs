using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class EvolutionMgr : MonoBehaviour
{
    public static EvolutionMgr Instance;
    public List<Enemy> population;
    public DataMgr data;
    
    // initialization
    void Awake()
    {
      this.population = new List<Enemy> ();
      if (GameMgr.Instance != null) 
      {
         this.population = GameMgr.Instance.previousGen;
      }
        this.data = DataMgr.Instance;
        Instance = this;
    }

    // fitness function - which current enemies did the best?
    // fitness is how it is for stats, combined with how it did in combat
    public int fitness(int index)
    {
        Enemy enemy = population.ElementAt(index);
        int score = 0;
        if (enemy.stats.alive == true)
        {
            score += enemy.stats.combatTurns;
        }
        score += enemy.stats.damageDone;
        int statNormalization = enemy.stats.attackPower + enemy.stats.defense + enemy.stats.maxHealth;
        return score +  statNormalization;
    }

    // fitness function - which current enemies did the best?
    // fitness is how it is for stats, combined with how it did in combat
    public int fitness(Enemy enemy)
    {
        int score = 0;
        if (enemy.stats.alive == true)
        {
            score += enemy.stats.combatTurns;
        }
        score += enemy.stats.damageDone;
        int statNormalization = enemy.stats.attackPower + enemy.stats.defense + enemy.stats.maxHealth;
        return score + statNormalization;
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

        List<Enemy> parents = new List<Enemy>();

        // remove below average performance enemies
        for (int i = 0; i < fitnessVals.Count; i++)
        {
            if (population.ElementAt(i)) { 
              if (fitnessVals.ElementAt(i) > average) {
                    parents.Add(population.ElementAt(i));
                }
            }
        }
        // prevent parent access out of bounds error from an edge case
        if (parents.Count == 0)
        {
            parents.Add(population.ElementAt(Random.Range(0, population.Count)));
        }

        List<EnemyData> nextGen = new List<EnemyData>();
        Enemy mom;
        Enemy dad;
        int mutationChance = 0;
        // generate enough enemies to fill the next level
        for (int i = 0; i < MapGenerator.Instance.numEnemies; i++)
        {
            Debug.Log (population.Count);
            // create a child based on two high fitness enemies
            mom = parents[Random.Range(0, parents.Count)];
            dad = parents[Random.Range(0, parents.Count)];
            EnemyData child = new EnemyData();
            child.SetAttackPower(mom.GetAttackPower() + 1);
            child.SetHealth(dad.GetMaxHealth() + 1);
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
                enemy.SetHealth(enemy.maxHealth + 1);
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

  public List<EnemyData> FirstGen()
  {
    List<EnemyData> nextGen = new List<EnemyData>();
    for (int i = 0; i < 5; i++)
    {
      EnemyData child = new EnemyData();
      child.SetAttackPower(UnityEngine.Random.Range(1,4));
      child.SetHealth(UnityEngine.Random.Range(1, 4));
      child.SetDefense(UnityEngine.Random.Range(1, PlayerMgr.Instance.attackPower));
      child.SetEnergy(UnityEngine.Random.Range(1, 4));
      child.SetAccuracy(.75F);

      // small chance to mutate the child
      int mutationChance = UnityEngine.Random.Range(0, 101);
      if (mutationChance < 5)
      {
        child = mutate(child);
      }
      nextGen.Add(child);

    }

    return nextGen;
  }
}

