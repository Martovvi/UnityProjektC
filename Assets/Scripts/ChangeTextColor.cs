using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ChangeTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text text;
    public Color onEnterColor;
    public Color onExitColor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = onEnterColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = onExitColor;
    }

    public void changeToEnterColor()
    {
        text.color = onEnterColor;
    }
    
    public void changeToExitColor()
    {
        text.color = onExitColor;
    }
}
