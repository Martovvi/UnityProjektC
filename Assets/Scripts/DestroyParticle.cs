using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    public float lifetime = 3f;
    
    private void Update()
    {
        Destroy(gameObject, lifetime);
    }
}
