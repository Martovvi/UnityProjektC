using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            playerComponent.TakeDamage(10);
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
