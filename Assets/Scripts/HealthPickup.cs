using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement playerComponent);
            if (playerComponent.health != playerComponent.maxHealth)
            {
                playerComponent.Heal(25);
                Destroy(gameObject);
            }
        }
    }
}
