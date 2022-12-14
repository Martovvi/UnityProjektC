using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionDetection : MonoBehaviour
{
    public WeaponController weapon;
    public GameObject hitParticle;
    public bool isColliding;

    // BUGGY - Maybe need to use Raycasts instead of colliders
    // Check if collider just entered a collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && weapon.isAttacking && !isColliding)
        {
            other.gameObject.TryGetComponent<EnemyAI>(out EnemyAI enemyComponent);
            enemyComponent.TakeDamage(50);
            isColliding = true;
            StartCoroutine(ResetCollision());
        }
    }
    
    // BUGGY - Maybe need to use Raycasts instead of colliders
    // Check if collider is staying on collider
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && weapon.isAttacking && !isColliding)
        {
            other.gameObject.TryGetComponent<EnemyAI>(out EnemyAI enemyComponent);
            enemyComponent.TakeDamage(50);
            isColliding = true;
            StartCoroutine(ResetCollision());
        }
    }

    // Resets collision
    IEnumerator ResetCollision()
    {
        yield return new WaitForSeconds(1);
        isColliding = false;
    }
}
