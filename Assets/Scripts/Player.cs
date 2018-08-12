using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    private const int MaxHealth = 100;
    private const float DamageOverlayCooldown = 1.0f;

    public SpecialCamera specialCamera;
    public PlayerUI playerUI;

    [Space]
    public Gradient HealthBarColorGradient;

    [Space]
    public GameObject CrystalHUDPrefab;

    private int health = MaxHealth;

    private Coroutine animateDamageOverlayCoroutine;
    private float lastHit;

    private void Start()
    {
        UpdateHealthBar();

        for ( int i = 0; i < LevelManager.instance.Crystals.Length; i++ )
        {
            CrystalHUD crystalHUD = Instantiate( CrystalHUDPrefab, playerUI.transform ).GetComponent<CrystalHUD>();
            crystalHUD.Setup( playerUI.GetComponent<RectTransform>(), specialCamera.FPSCamera.transform, LevelManager.instance.Crystals[i] );
        }
    }

    public void Damage( int damage )
    {
        health -= damage;
        lastHit = Time.time;

        if ( animateDamageOverlayCoroutine == null )
            animateDamageOverlayCoroutine = StartCoroutine( AnimateDamageOverlay() );

        if ( health <= 0 )
        {
            health = 0;
            LevelManager.instance.GameOver();
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        playerUI.HealthBar.fillAmount = ( float )health / ( float )MaxHealth;
        playerUI.HealthBar.color = HealthBarColorGradient.Evaluate( playerUI.HealthBar.fillAmount );

        playerUI.HealthText.text = health.ToString();
    }

    private IEnumerator AnimateDamageOverlay()
    {
        while ( Time.time - lastHit < DamageOverlayCooldown )
        {
            playerUI.DamageOverlayImage.color = new Color( 1, 1, 1, 1 - ( Time.time - lastHit ) / DamageOverlayCooldown );

            yield return null;
        }

        animateDamageOverlayCoroutine = null;
    }
}
