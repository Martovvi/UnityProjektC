using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject weaponSword; // Can add more in the future
    public bool canAttack;
    public float attackCooldown = 1f;
    public AudioClip swordAttackSound;
    public bool isAttacking = false;
    public PlayerMovement playerScript;
    public GameManager gameManager;
    private int hitCount;
    private Animator anim;
    float lastClickedTime = 0f;
    public float minAnimationDuration = 0.1f;
    float maxComboDelay = 2f;


    private void Start()
    {
        anim = weaponSword.GetComponent<Animator>();
    }

    private void Update()
    {
        ResetAnimateVariables();

        if (Input.GetMouseButtonDown(0))
        {
            // lastClickedTime = Time.time;
            // hitCount++;

            if (canAttack && !playerScript.isGamePaused && !playerScript.isDead && !gameManager.isLevelCompleted)
            {
                //OnClick-Equivalent
                //hitCount++;
                SwordAttack();
                AnimateSword();
            }
        }
    }

    // Sword attack logic
    public void SwordAttack()
    {
        //if (hitCount > 3) hitCount = 1;

        isAttacking = true;
        canAttack = false;

        // anim.SetBool("OnSwordSwing" + (hitCount - 1), false);
        // anim.SetBool("OnSwordSwing" + hitCount, true);

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
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;

    }

    void ResetAnimateVariables()
    {
        //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        // if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > minAnimationDuration && anim.GetBool("OnSwordSwing1"))
        // {
            
        //     Debug.Log("hit1 reset" + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        //     anim.SetBool("OnSwordSwing1", false);
        // }
        // if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > minAnimationDuration && anim.GetCurrentAnimatorStateInfo(0).IsName("OnSwordSwing2"))
        // {
        //     anim.SetBool("OnSwordSwing2", false);
        // }
        // if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > minAnimationDuration && anim.GetCurrentAnimatorStateInfo(0).IsName("OnSwordSwing3"))
        // {
        //     anim.SetBool("OnSwordSwing3", false);
        //     hitCount = 0;
        // }


        if (Time.time - lastClickedTime > maxComboDelay)
        {
            hitCount = 0;
            anim.SetTrigger("ExitSwordCombo");
        }
    }

    void AnimateSword()
    {
        //so it looks at how many clicks have been made and if one animation has finished playing starts another one.
        lastClickedTime = Time.time;
        hitCount++;
        hitCount = Mathf.Clamp(hitCount, 0, 3);
        Debug.Log("hit="+hitCount);
        if (hitCount == 1)
        {
            anim.SetTrigger("OnSwordSwing1");
        }
        
        Debug.Log("Timecheck2 ="+ (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > minAnimationDuration));
        Debug.Log("GetBool2=" + anim.GetBool("OnSwordSwing1"));
        if (hitCount >= 2 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > minAnimationDuration /*&& anim.GetBool("OnSwordSwing1")*/)
        {
            Debug.Log("hit2 entered");
            //anim.SetBool("OnSwordSwing1", false);
            anim.SetTrigger("OnSwordSwing2");
        }
        if (hitCount >= 3 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > minAnimationDuration && anim.GetBool("OnSwordSwing2"))
        {
            //anim.SetBool("OnSwordSwing2", false);
            anim.SetTrigger("OnSwordSwing3");
        }
    }

}
