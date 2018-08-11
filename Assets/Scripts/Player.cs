using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    private const int MaxHealth = 100;

    public SpecialCamera specialCamera;
    public PlayerUI playerUI;

    [Space]
    public Gradient HealthBarColorGradient;

    private int health = MaxHealth;

    private void Start()
    {
        UpdateHealthBar();
    }

    public void Damage( int damage )
    {
        health -= damage;

        if ( health < 0 )
            health = 0;

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        playerUI.HealthBar.fillAmount = ( float )health / ( float )MaxHealth;
        playerUI.HealthBar.color = HealthBarColorGradient.Evaluate( playerUI.HealthBar.fillAmount );

        playerUI.HealthText.text = health.ToString();
    }
}
