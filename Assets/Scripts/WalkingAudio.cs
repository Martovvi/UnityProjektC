using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAudio : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement pm;
    //Audio
    public AudioSource audioSrc;

    public AudioClip[] stoneClips;
    public AudioClip[] metalClips;

    void Update()
    {
        
    }


    public void Step()
    {
        Debug.Log("step entered");
        if (pm.getGrounded() && (pm.getVelocity() > 0.2f) && !pm.isGamePaused)
        {
            if (pm.metalGround)
            {
                audioSrc.PlayOneShot(metalClips[UnityEngine.Random.Range(0, metalClips.Length)]);
            }
            else
            {
                audioSrc.PlayOneShot(stoneClips[UnityEngine.Random.Range(0, stoneClips.Length)]);
            }
        }
    }
}
