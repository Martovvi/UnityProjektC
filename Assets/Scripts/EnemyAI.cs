using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    
    
    public Transform player;
    
    // Stats
    public float maxHealth = 100f;
    public float health = 100f;
    
    // Layer Checking
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    
    // Patrolling
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    
    // States
    public float sightRange;
    public float attackRange;
    public bool playerInSightRange;
    public bool playerInAttackRange;
    
    // Other

    public GameObject blood;

    private void Awake()
    {
        player = GameObject.Find("PlayerObject").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    // Default enemy state when the player has not been detected
    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        
        // Walkpoint reached -> find new Walkpoint
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Search if there is an actual ground to walk on
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }
    }
    
    // The enemies will be on alert and chase the player
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    
    // The enemies are close enough to the player to attack him
    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);
        
        // Look at player when attacking
        transform.LookAt(player);

        // Check if AI has attacked already and attack
        if (!alreadyAttacked)
        {
            // Attack code here
            //
            //
            Debug.Log("Attack!");
            
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    // Resets the attack of the Enemy AI
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // Reduces health on damage
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        Instantiate(blood,
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            transform.rotation);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void onDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
