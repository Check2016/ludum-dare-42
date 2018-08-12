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
    public GameObject CrystalDockHUDPrefab;

    private int health = MaxHealth;

    private Coroutine animateDamageOverlayCoroutine;
    private float lastHit;

    private void Start()
    {
        UpdateHealthBar();

        for ( int i = 0; i < LevelManager.instance.Crystals.Length; i++ )
        {
            HUDImage hudImage = Instantiate( CrystalHUDPrefab, playerUI.HUDParent.transform ).GetComponent<HUDImage>();
            hudImage.Setup( playerUI.GetComponent<RectTransform>(),
                specialCamera.FPSCamera.transform, 
                LevelManager.instance.Crystals[i].transform, 
                LevelManager.instance.Crystals[i].Mesh.GetComponent<MeshRenderer>().sharedMaterial.color, 
                LevelManager.instance.Crystals[i].Mesh );
        }

        for ( int i = 0; i < LevelManager.instance.CrystalDocks.Length; i++ )
        {
            HUDImage hudImage = Instantiate( CrystalDockHUDPrefab, playerUI.HUDParent.transform ).GetComponent<HUDImage>();
            hudImage.Setup( playerUI.GetComponent<RectTransform>(),
                specialCamera.FPSCamera.transform, 
                LevelManager.instance.CrystalDocks[i].transform.GetChild( 0 ),
                LevelManager.instance.Crystals[i].Mesh.GetComponent<MeshRenderer>().sharedMaterial.color );
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
