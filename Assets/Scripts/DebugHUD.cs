using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    // Assignables
    public TextMeshProUGUI velocityLabel;
    public GameObject player;
    private Rigidbody rb;

    private Vector3 dir;
    private float speed;


    // Bools
    public bool displayVelocity;

    private void Start()
    {
        velocityLabel = GetComponentInChildren<TextMeshProUGUI>();
        rb = player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (displayVelocity)
        {
            DisplayVelocity();
        }
    }

    private void DisplayVelocity()
    {
        dir = rb.velocity;
        speed = dir.magnitude;
        velocityLabel.text = "Speed: " + Mathf.Round(speed * 100f) / 100f;

    }
}
