using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletCollisionDetection : MonoBehaviour
{
    public GameObject bullet;
    public bool isColliding;
    
    // Check if collider just entered a collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isColliding)
        {
            other.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement playerComponent);
            playerComponent.TakeDamage(Random.Range(5, 10));
            isColliding = true;
            StartCoroutine(ResetCollision());
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }

    // Resets collision
    IEnumerator ResetCollision()
    {
        yield return new WaitForSeconds(1);
        isColliding = false;
    }
}
