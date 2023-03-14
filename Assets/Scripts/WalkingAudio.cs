using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAudio : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement pm;
    //Audio
    public AudioSource audioSrc;

    public AudioClip walkingOnConcrete;
    public AudioClip walkingOnMetal;
    

    public void Step(){
        Debug.Log("step entered");
        if(pm.getGrounded() && (pm.getVelocity() > 0.2f) && !pm.isGamePaused){
            //TODO if else based on ground type (tag or something)
            audioSrc.PlayOneShot(walkingOnConcrete);
        }
    }
}
