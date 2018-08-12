using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private const float MinEnemySpawnDelay = 0.5f;
    private const float MaxEnemySpawnDelay = 2;
    private const float EnemyDespawnDistance = 20;

    private const float FinishLightRaysTime = 2;
    private const float FinishLightRaysPlayerYOffset = -1;
    private const float FinishLightRaysPlayerRadius = 1;
    private const float FinishFloatUpHeight = 60;
    private const float FinishFloatUpTime = 6;

    public int MaxEnemyCount = 10;

    [Space]
    public GameObject PlayerPrefab;
    public GameObject PlayerUIPrefab;
    public Transform PlayerSpawnPoint;

    [Space]
    public GameObject EnemyPrefab;
    public Transform[] EnemySpawnPoints = new Transform[0];

    [Space]
    public Crystal[] Crystals = new Crystal[0];

    [Space]
    public GameObject LightRayPrefab;
    public GameObject WinCanvasPrefab;

    private List<Enemy> enemies = new List<Enemy>();
    private Player player;
    private PlayerUI playerUI;

    private Coroutine spawnEnemiesCoroutine;

    public static LevelManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = Instantiate( PlayerPrefab, PlayerSpawnPoint.transform.position, PlayerSpawnPoint.transform.rotation ).GetComponent<Player>();
        playerUI = Instantiate( PlayerUIPrefab ).GetComponent<PlayerUI>();

        player.playerUI = playerUI;

        spawnEnemiesCoroutine = StartCoroutine( SpawnEnemies() );

        for ( int i = 0; i < Crystals.Length; i++ )
        {
            Crystals[i].onDock += OnCrystalDock;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while ( true )
        {
            for ( int i = 0; i < enemies.Count; i++ )
            {
                if ( enemies[i].enabled && Vector3.Distance( enemies[i].transform.position, player.transform.position ) > EnemyDespawnDistance )
                {
                    MeshRenderer meshRenderer = enemies[i].GetComponent<MeshRenderer>();

                    if ( IsVisibleFrom( meshRenderer, player.specialCamera.FPSCamera ) == false || meshRenderer.isVisible == false )
                    {
                        Destroy( enemies[i].gameObject );
                        enemies.RemoveAt( i );
                    }
                }
            }

            if ( enemies.Count < MaxEnemyCount )
            {
                int index = -1;
                float smallestDistance = 0;

                for ( int i = 0; i < EnemySpawnPoints.Length; i++ )
                {
                    float distance = Vector3.Distance( EnemySpawnPoints[i].position, player.transform.position );

                    if ( index == -1 || distance < smallestDistance )
                    {
                        index = i;
                        smallestDistance = distance;
                    }
                }

                Enemy enemy = Instantiate( EnemyPrefab, EnemySpawnPoints[index].position, EnemySpawnPoints[index].rotation ).GetComponent<Enemy>();
                enemy.Target = player;

                enemies.Add( enemy );

                yield return new WaitForSeconds( Random.Range( MinEnemySpawnDelay, MaxEnemySpawnDelay ) );
            }
            else
            {
                yield return new WaitForSeconds( 0.1f );
            }
        }
    }

    private void OnCrystalDock()
    {
        bool allDocked = true;

        for ( int i = 0; i < Crystals.Length; i++ )
        {
            if ( Crystals[i].IsDocked() == false )
            {
                allDocked = false;
                break;
            }
        }

        if ( allDocked )
        {
            StartCoroutine( LevelFinished() );
        }
    }

    private IEnumerator LevelFinished()
    {
        player.specialCamera.enabled = false;
        player.GetComponent<FPSController>().SetCanMove( false );

        StopCoroutine( spawnEnemiesCoroutine );

        for ( int i = 0; i < enemies.Count; i++ )
        {
            enemies[i].enabled = false;
        }

        LineRenderer[] lightRays = new LineRenderer[Crystals.Length];

        for ( int i = 0; i < Crystals.Length; i++ )
        {
            lightRays[i] = Instantiate( LightRayPrefab ).GetComponent<LineRenderer>();
            lightRays[i].positionCount = 2;
        }

        float t = 0;

        while ( t < 1 )
        {
            t += Time.deltaTime / FinishLightRaysTime;

            float t_pow = Mathf.Pow( t, 3.0f );

            for ( int i = 0; i < Crystals.Length; i++ )
            {
                Vector3 lightTarget = player.transform.position + Vector3.up * FinishLightRaysPlayerYOffset + ( Crystals[i].Mesh.transform.position - player.transform.position ).normalized * FinishLightRaysPlayerRadius;

                lightRays[i].SetPositions( new Vector3[] { Crystals[i].Mesh.transform.position,
                                                           Vector3.Lerp( Crystals[i].Mesh.transform.position, lightTarget, t_pow )
                } );
            }

            yield return null;
        }

        yield return new WaitForSeconds( 0.5f );

        StartCoroutine( InstantiateWinCanvas( FinishFloatUpTime * 0.75f ) );

        t = 0;

        Vector3 startPos = player.transform.position;
        Vector3 targetPos = player.transform.position + FinishFloatUpHeight * Vector3.up;

        while ( t < 1 )
        {
            t += Time.deltaTime / FinishFloatUpTime;

            for ( int i = 0; i < Crystals.Length; i++ )
            {
                Vector3 lightTarget = player.transform.position + Vector3.up * FinishLightRaysPlayerYOffset + ( Crystals[i].Mesh.transform.position - player.transform.position ).normalized * FinishLightRaysPlayerRadius;

                lightRays[i].SetPositions( new Vector3[] { Crystals[i].Mesh.transform.position,
                                                           lightTarget
                } );
            }

            player.transform.position = Vector3.Lerp( startPos, targetPos, Mathf.Pow( t, 3.0f ) );

            yield return null;
        }
    }

    private IEnumerator InstantiateWinCanvas( float delay )
    {
        yield return new WaitForSeconds( delay );

        Instantiate( WinCanvasPrefab );
    }

    public bool IsVisibleFrom( Renderer renderer, Camera camera )
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes( camera );
        return GeometryUtility.TestPlanesAABB( planes, renderer.bounds );
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }
}
