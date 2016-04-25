using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
	public LayerMask obstacleLayer;
	public List<Sprite> enemySprites;
	public float moveSpeed;
	public bool moving;
	int currentEnergy = 0;

	public EnemyData stats = new EnemyData (1, 1, 1, 1, 6, 1.0f);
	Point targetCoordinate;
	public bool playerSighted = false;
	EnemyState currentEnemyState;

	// Use this for initialization
	void Start ()
	{
		currentEnemyState = EnemyState.Search;
		int spriteChoice = Random.Range (0, enemySprites.Count);
		GetComponent<SpriteRenderer> ().sprite = enemySprites [spriteChoice];


		obstacleLayer |= 1 << LayerMask.NameToLayer ("Player");
		obstacleLayer |= 1 << LayerMask.NameToLayer ("Enemy");
	}

	/// <summary>
	/// Try to make a move
	/// </summary>
	public IEnumerator TryMove ()
	{
		currentEnergy = stats.energy;
		Debug.Log ("energy " + currentEnergy);
		// Move the player towards the destination
		while (currentEnergy > 0) 
		{
			if (!moving) 
			{
				//moving = true;
				evaluateFSM ();
				executeFSM ();
			}
			yield return null;
		}

		GameMgr.Instance.EnemyDoneMoving ();

	}

	/// <summary>
	/// Move the specified x and y.
	/// </summary>
	/// <param name="x">The distance to move in the X-Direction</param>
	/// <param name="y">The distance to move in the Y-Direction</param>
	IEnumerator Move (Point direction)
	{
		// Where the player is moving
		Vector3 end = transform.position + new Vector3 (direction.x, direction.y);

		// Move the player towards the destination
		while (Vector3.Distance (transform.position, end) > 0) {
			transform.position = Vector3.MoveTowards (transform.position, end, moveSpeed);
			yield return null;
		}

		//movement over
		currentEnergy--;
		moving = false;
	}

	bool decideBasedOn(float chance)
	{
		if (chance > 1.0f)
			return true;
		if (chance < 0)
			return false;

		int percent = (int)(chance * 100);

		return ((Random.Range (0, 101)) < percent);
	}

	/// <summary>
	/// Process current state of affairs to see if FSM should switch state
	/// </summary>
	void evaluateFSM ()
	{
		//how healthy are we? near 0 being near dead, near 1 being near full health
		float healthRating = stats.currentHealth / stats.maxHealth;

		switch (currentEnemyState) 
		{
		case EnemyState.Combat:
			//have we lost sight of the player?
			if (!playerSighted) {
				//if we have lost the player, should we heal?
				if (!decideBasedOn (healthRating)) 
				{
					currentEnemyState = EnemyState.Idle;
					return;
				}
				//if we're healthy, start searching!
				currentEnemyState = EnemyState.Search;
				return;
			}

			//if we have lost health, we should consider fleeing
			if (!decideBasedOn (healthRating))
				currentEnemyState = EnemyState.Flee;
			break;

		case EnemyState.Flee:
			//if we've lost the player we should heal now
			if (!playerSighted)
				currentEnemyState = EnemyState.Idle;

			//should we heal even if we haven't lost the player?
			if (playerSighted && decideBasedOn (healthRating)) 
			{
				//the player is still near but our health might be high enough to risk healing here
				currentEnemyState = EnemyState.Idle;
			}

			break;

		case EnemyState.Idle:

			//if we've lost the player and our health is adequate, start searching
			if (!playerSighted && decideBasedOn (healthRating))
				currentEnemyState = EnemyState.Search;

			//the player has been spotted again!
			if (playerSighted) 
			{
				//are we so hurt that we should run away?
				if(!decideBasedOn(healthRating))
					currentEnemyState = EnemyState.Flee;

				//are we healthy enough to fight?
				if(decideBasedOn(healthRating))
					currentEnemyState = EnemyState.Combat;
			}

			break;

		case EnemyState.Search:

			//are we so hurt that we should heal more?
			if (!decideBasedOn (healthRating)) 
			{
				currentEnemyState = EnemyState.Flee;
				return;
			}

			//the player has been spotted again, fight!
			if (playerSighted) 
				currentEnemyState = EnemyState.Combat;

			break;
		}
	}

	/// <summary>
	/// execute the current state of the FSM
	/// </summary>
	void executeFSM ()
	{
		//get the location of the player and enemy to figure out their coordinates
		Point playerLocation = new Point(PlayerMgr.Instance.transform.position);
		Point enemyLocation = new Point(transform.position);
		targetCoordinate = enemyLocation;

		switch (currentEnemyState) 
		{
		case EnemyState.Combat:
			//pursue the player
			targetCoordinate = Pathfinding.Instance.MovePickerA (enemyLocation, playerLocation, obstacleLayer);

			//if A* didn't find a path, keep the enemy still
			if (targetCoordinate == new Point ())	targetCoordinate = enemyLocation;
			break;

		case EnemyState.Flee:
			//flee from the player
			targetCoordinate = Pathfinding.Instance.FleeFromPoint (enemyLocation, playerLocation, obstacleLayer);
			break;

		case EnemyState.Idle:
			//if enemy isn't at full health already, regenerate health
			if (stats.currentHealth < stats.maxHealth)
				stats.currentHealth += 1;
			break;

		case EnemyState.Search:
			//look around randomly
			targetCoordinate = Pathfinding.Instance.MovePickerRandom (enemyLocation);
			break;
		}

		// Check if we can move to the next tile
		Vector3 endCoordinate = new Vector3 (targetCoordinate.x, targetCoordinate.y);
		RaycastHit2D checkValid = Physics2D.Linecast (endCoordinate, endCoordinate, obstacleLayer);

		// Collider will be null if the linecast didn't hit an obstacle
		if (checkValid.collider == null || checkValid.collider.transform == this.transform.GetChild (0)) 
		{
			if (targetCoordinate != enemyLocation) {
				moving = true;
				StartCoroutine (Move (targetCoordinate - enemyLocation));
			} else
				moving = false;
		}

		else if (checkValid.collider.gameObject.tag == "Player") 
			Attack ();
		//else	Debug.Log (checkValid.collider.gameObject.tag);
	}

	/// <summary>
	/// Attack the Player
	/// </summary>
	void Attack ()
	{
		stats.combatTurns += 1;
		moving = false;
		currentEnergy--;
		PlayerMgr.Instance.Defend (stats.attackPower);
	}

	/// <summary>
	/// Defend the specified attack.
	/// </summary>
	/// <param name="attack">The attack power from the Player</param>
	public int Defend (int attack)
	{
		stats.combatTurns += 1;
		int damage = Mathf.Max (attack - stats.defense, 0);
		if (damage == 0) {
			if (UnityEngine.Random.Range (1, 101) > 50) {
				damage = 1;
				HUDMgr.Instance.PrintAction ("You barely scratch it for 1 damage!");
			} else {
				HUDMgr.Instance.PrintAction ("The enemy blocks your attack!");
			}
		} else {
			HUDMgr.Instance.PrintAction ("You hit for " + damage + " damage!");
		}
		DataMgr.Instance.currentLevel.damageGiven += damage;
		stats.currentHealth -= damage;
		if (stats.currentHealth < 0) {
			DataMgr.Instance.currentLevel.enemiesKilled += 1;
			stats.alive = false;
			GameMgr.Instance.KillEnemy (this);
			Destroy (gameObject);
			DataMgr.Instance.score += EvolutionMgr.Instance.fitness (this);
			return 10;
		}
		return 0;
	}

	public enum EnemyState
	{
		Idle,
		Search,
		Combat,
		Flee
	};
}
