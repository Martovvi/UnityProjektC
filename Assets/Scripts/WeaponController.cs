using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject weaponSword; // Can add more in the future
    public bool canAttack;
    public float attackCooldown = 1.0f;
    public AudioClip swordAttackSound;
    public bool isAttacking = false;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canAttack)
            {
                SwordAttack();
            }
        }
    }
    
    // Sword attack logic
    public void SwordAttack()
    {
        isAttacking = true;
        canAttack = false;
        Animator anim = weaponSword.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        StartCoroutine(ResetAttackCooldown());
        AudioSource audioSrc = GetComponent<AudioSource>();
        audioSrc.PlayOneShot(swordAttackSound);
    }

    // Resets attack cooldown for sound
    IEnumerator ResetAttackCooldown()
    {
        StartCoroutine(ResetAttackBool());
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Resets attack boolean for collision detection
    IEnumerator ResetAttackBool()
    {
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;

    }
    
}
