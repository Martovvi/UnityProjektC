using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image backHealthBar;
    public Image frontHealthBar;
    public Gradient gradient;
    public TextMeshProUGUI digits;

    private void Start()
    {
        health = maxHealth;
        backHealthBar.fillAmount = frontHealthBar.fillAmount;
        digits = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        frontHealthBar.color = gradient.Evaluate((frontHealthBar.fillAmount));
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        digits.text = health.ToString(CultureInfo.CurrentCulture);
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float healthNormalized = health / maxHealth;
        if (fillB > healthNormalized)
        {
            frontHealthBar.fillAmount = healthNormalized;
            Color damageHealthColor = new Color(0.49f, 0.06f, 0.06f, 1f);
            backHealthBar.color = damageHealthColor;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, healthNormalized, percentComplete);
        }
        if (fillF < healthNormalized)
        {
            frontHealthBar.fillAmount = healthNormalized;
            Color healHealthColor = new Color(0.06f, 0.49f, 0.06f, 1f);
            backHealthBar.color = healHealthColor;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            backHealthBar.fillAmount = healthNormalized;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

    public void Damage(float amount)
    {
        health -= amount;
        lerpTimer = 0f;
    }

    public void Heal(float amount)
    {
        health += amount;
        lerpTimer = 0f;
    }
}
