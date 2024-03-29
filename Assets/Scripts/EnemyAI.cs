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
    private PlayerMovement playerScript;

    [SerializeField] private GameManager gameManager;
    
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
    public GameObject bullet;
    public Transform shootPoint;
    public Animator animator;
    public float shootSpeed = 50f;
    public float timeToShoot = 1.3f;
    private float originalTime;
    private Rigidbody[] ragdollRigidbodies;
    private CapsuleCollider capsuleCollider;
    private BoxCollider boxCollider;
    public AudioClip swordConnectsSound;
    public AudioSource audioSrc;

    private void Awake()
    {
        player = GameObject.Find("PlayerObject").transform;
        playerScript = player.GetComponentInChildren<PlayerMovement>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
        gameManager.enemyCount++;
    }

    private void Start()
    {
        health = maxHealth;
        originalTime = timeToShoot;
    }

    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
        
        animator.SetFloat("Move", agent.velocity.magnitude);
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
        if (playerScript.isDead == false)
        {
            agent.SetDestination(player.position);
        }
    }
    
    // The enemies are close enough to the player to attack him
    private void AttackPlayer()
    {
        if (playerScript.isDead == false)
        {
            // Make sure enemy doesn't move
            agent.SetDestination(transform.position);
        
            // Look at player when attacking
            transform.LookAt(player);

            // Check if AI has attacked already and attack
            if (!alreadyAttacked)
            {
                // Attack code
                ShootPlayer();
                // End attack code
            
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
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
            EnableRagdoll();
            gameManager.enemyCount--;
            gameManager.LevelComplete();
        }
    }

    private void ShootPlayer()
    {
        GameObject currentBullet = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward*shootSpeed, ForceMode.VelocityChange);
    }

    private void DisableRagdoll()
    {
        foreach (var rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
        }

        this.enabled = true;
        animator.enabled = true;
        agent.enabled = true;
    }

    private void EnableRagdoll()
    {
        foreach (var rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        this.enabled = false;
        animator.enabled = false;
        agent.enabled = false;
        Physics.IgnoreLayerCollision(3, 10);
    }

    private void onDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
