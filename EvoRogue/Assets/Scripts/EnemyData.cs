using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemyData
{
	public int attackPower;
	public int defense;
	public int currentHealth;
	public int maxHealth;
	public int damageDone;
	public int combatTurns;
	public int energy;
	public int range;
	public float accuracy;
	public bool alive;

	public EnemyData ()
	{
		this.attackPower = 1;
		this.defense = 1;
		this.maxHealth = 1;
		this.currentHealth = 1;
		this.damageDone = 0;
		this.combatTurns = 0;
		this.energy = 1;
		this.accuracy = 1;
		this.range = 2;
		this.alive = true;
	}

	// ADD CONSTRUCTORS, SETTERS, GETTERS
	public EnemyData (int att, int def, int hp, int energy, int range, float accuracy)
	{
		this.attackPower = att;
		this.defense = def;
		this.currentHealth = hp;
		this.maxHealth = hp;
		this.damageDone = 0;
		this.combatTurns = 0;
		this.energy = energy;
		this.range = range;
		this.accuracy = accuracy;
		this.alive = true;
	}

	public void SetAttackPower (int val)
	{
		attackPower = val;
	}

	public void SetDefense (int val)
	{
		defense = val;
	}

	public void SetHealth (int val)
	{
		currentHealth = val;
		maxHealth = val;
	}

	public void SetEnergy (int val)
	{
		energy = val;
	}

	public void SetAccuracy (float val)
	{
		accuracy = val;
	}

	public void SetRange (int val)
	{
		range = val;
	}
}
