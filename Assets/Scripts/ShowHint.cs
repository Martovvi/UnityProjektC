using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowHint : MonoBehaviour
{
    public GameObject uiObject;
    public TextMeshProUGUI hintLabel;
    public String hintText;

    private void Start()
    {
        uiObject.SetActive(false);
        hintLabel.text = hintText;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hintLabel.text = hintText;
            uiObject.SetActive(true);
            StartCoroutine(WaitForSec());
        }

        IEnumerator WaitForSec()
        {
            yield return new WaitForSeconds(5);
            uiObject.SetActive(false);
            Destroy(gameObject);
        }
        
    }
}
