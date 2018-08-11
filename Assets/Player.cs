using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    private const int MaxHealth = 100;

    public Image HealthBar;
    public TextMeshProUGUI HealthText;
    public Gradient HealthBarColorGradient;

    private int health = MaxHealth;

    private void Start()
    {
        UpdateHealthBar();
    }

    public void Damage( int damage )
    {
        health -= damage;
    }

    private void UpdateHealthBar()
    {
        HealthBar.fillAmount = ( float )health / ( float )MaxHealth;
        HealthBar.color = HealthBarColorGradient.Evaluate( HealthBar.fillAmount );
    }
}
