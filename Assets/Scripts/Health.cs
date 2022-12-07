using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour
{

    private int health = 5;
    public Text healthText;

    void Update()
    {

        healthText.text = "HEALTH :" + health;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            health--;

        }

    }
}

