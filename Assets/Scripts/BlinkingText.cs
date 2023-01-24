using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlinkingText : MonoBehaviour
{
    public string textMessage;
    public float blinkDuration = 0.5f;
    private TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        StartBlinking();
    }
    IEnumerator Blink()
    {
        text.text = "";
        yield return new WaitForSeconds(1f);
        while (true)
        {
            text.text = textMessage;
            yield return new WaitForSeconds(blinkDuration);
            text.text = "";
            yield return new WaitForSeconds(blinkDuration);
        }
    
    }
    
    public void StartBlinking(){
        StartCoroutine("Blink");
    }
    public void StopBlinking()
    {
        StopCoroutine("Blink");
    }
}
