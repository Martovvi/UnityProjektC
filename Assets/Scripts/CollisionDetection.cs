using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController weapon;
    public GameObject hitParticle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && weapon.isAttacking)
        {
            Debug.Log(other.name);
            Instantiate(hitParticle,
                new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z),
                other.transform.rotation);
        }
    }
}
