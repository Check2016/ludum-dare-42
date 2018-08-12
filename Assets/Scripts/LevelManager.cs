using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private const float MinEnemySpawnDelay = 0.5f;
    private const float MaxEnemySpawnDelay = 1;
    private const float MinEnemyLifetime = 10;
    private const float EnemyDespawnDistance = 20;

    private const float FinishLightRaysTime = 1.5f;
    private const float FinishLightRaysPlayerYOffset = -0.8f;
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
    public CrystalDock[] CrystalDocks = new CrystalDock[0];

    [Space]
    public GameObject LightRayPrefab;
    public GameObject SupernovaPrefab;
    public GameObject WinCanvasPrefab;
    public GameObject GameOverCanvasPrefab;

    [Space]
    public string NextLevel;

    private List<Enemy> enemies = new List<Enemy>();
    private Player player;
    private PlayerUI playerUI;

    private Coroutine spawnEnemiesCoroutine;

    private bool gameOver = false;
    private bool levelFinished = false;

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
                if ( enemies[i].enabled && Time.time - enemies[i].SpawnTime >= MinEnemyLifetime && Vector3.Distance( enemies[i].transform.position, player.transform.position ) > EnemyDespawnDistance )
                {
                    MeshRenderer meshRenderer = enemies[i].GetComponent<MeshRenderer>();

                    if ( IsVisibleFrom( meshRenderer, player.specialCamera.FPSCamera ) == false || meshRenderer.isVisible == false )
                    {
                        Destroy( enemies[i].gameObject );
                        enemies.RemoveAt( i );
                    }
                }
            }

            int Max = MaxEnemyCount;

            int frozenEnemies = 0;

            Picture[] pictures = player.specialCamera.GetPictures();

            if ( pictures != null )
            {
                for ( int i = 0; i < pictures.Length; i++ )
                {
                    if ( pictures[i] != null && pictures[i].type == PictureTypes.Freeze )
                    {
                        frozenEnemies += pictures[i].frozenEnemies.Length;
                    }
                }

                if ( enemies.Count > Max )
                {
                    Max = enemies.Count;
                }

                if ( frozenEnemies == enemies.Count )
                {
                    Max++;
                }
            }

            if ( enemies.Count < Max )
            {
                int index = -1;
                float smallestDistance = Mathf.Infinity;

                for ( int i = 0; i < EnemySpawnPoints.Length; i++ )
                {
                    bool visible = false;

                    Vector3 ToTargetDir = ( EnemySpawnPoints[i].position - player.specialCamera.FPSCamera.transform.position ).normalized;

                    RaycastHit raycastHit;

                    if ( Physics.Raycast( player.specialCamera.FPSCamera.transform.position, ToTargetDir, out raycastHit, 1000, 1 << 0 ) )
                    {
                        if ( raycastHit.transform == EnemySpawnPoints[i] )
                        {
                            visible = true;
                        }
                    }

                    if ( visible == false )
                    {
                        float distance = Vector3.Distance( EnemySpawnPoints[i].position, player.transform.position );

                        if ( index == -1 || distance < smallestDistance )
                        {
                            index = i;
                            smallestDistance = distance;
                        }
                    }
                }

                if ( index > -1 )
                {
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
        if ( levelFinished ) yield break;

        levelFinished = true;

        player.specialCamera.enabled = false;
        player.GetComponent<FPSController>().SetCanMove( false );

        playerUI.HUDParent.gameObject.SetActive( false );

        StopCoroutine( spawnEnemiesCoroutine );

        for ( int i = 0; i < enemies.Count; i++ )
        {
            enemies[i].enabled = false;
        }

        PlayerPrefs.SetInt( "Unlocked_" + NextLevel, 1 );

        LineRenderer[] lightRays = new LineRenderer[Crystals.Length];

        for ( int i = 0; i < Crystals.Length; i++ )
        {
            lightRays[i] = Instantiate( LightRayPrefab ).GetComponent<LineRenderer>();
            lightRays[i].positionCount = 2;
        }

        GameObject supernova = Instantiate( SupernovaPrefab );

        MeshRenderer supernovaMeshRenderer = supernova.GetComponent<MeshRenderer>();
        supernovaMeshRenderer.sharedMaterial = new Material( supernovaMeshRenderer.sharedMaterial );
        Color supernovaColor = supernovaMeshRenderer.sharedMaterial.GetColor( "_TintColor" );
        supernovaMeshRenderer.sharedMaterial.SetColor( "_TintColor", new Color( supernovaColor.r, supernovaColor.g, supernovaColor.b, 0 ) );

        float t = 0;

        while ( t < 1 )
        {
            t += Time.deltaTime / FinishLightRaysTime;

            float t_pow = Mathf.Pow( t, 3.0f );

            for ( int i = 0; i < Crystals.Length; i++ )
            {
                Vector3 lightTarget = player.transform.position + Vector3.up * FinishLightRaysPlayerYOffset;

                lightRays[i].SetPositions( new Vector3[] { Crystals[i].Mesh.transform.position,
                                                           Vector3.Lerp( Crystals[i].Mesh.transform.position, lightTarget, t_pow )
                } );
            }

            supernova.transform.position = player.transform.position + SupernovaPrefab.transform.position;

            if ( t >= 0.95f )
            {
                supernovaMeshRenderer.sharedMaterial.SetColor( "_TintColor", new Color( supernovaColor.r, supernovaColor.g, supernovaColor.b, ( t - 0.95f ) / 0.05f ) );
            }

            yield return null;
        }

        player.GetComponent<FPSController>().SetUseGravity( false );

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
                Vector3 lightTarget = player.transform.position + Vector3.up * FinishLightRaysPlayerYOffset;

                lightRays[i].SetPositions( new Vector3[] { Crystals[i].Mesh.transform.position,
                                                           lightTarget
                } );
            }

            player.transform.position = Vector3.Lerp( startPos, targetPos, Mathf.Pow( t, 3.0f ) );
            supernova.transform.position = player.transform.position + SupernovaPrefab.transform.position;

            yield return null;
        }
    }

    public void GameOver()
    {
        if ( gameOver ) return;

        gameOver = true;

        player.enabled = false;
        player.specialCamera.enabled = false;
        player.GetComponent<FPSController>().SetCanMove( false );

        StopCoroutine( spawnEnemiesCoroutine );

        for ( int i = 0; i < enemies.Count; i++ )
        {
            enemies[i].enabled = false;
        }

        StartCoroutine( InstantiateGameOverCanvas() );
    }

    private IEnumerator InstantiateWinCanvas( float delay )
    {
        yield return new WaitForSeconds( delay );

        Instantiate( WinCanvasPrefab );

        yield return new WaitForSeconds( 2.75f );

        player.GetComponent<FPSController>().SetCursorLock( false );
    }

    private IEnumerator InstantiateGameOverCanvas()
    {
        Instantiate( GameOverCanvasPrefab );

        yield return new WaitForSeconds( 2f );

        player.GetComponent<FPSController>().SetCursorLock( false );
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

    public Player GetPlayer()
    {
        return player;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
    }

    public void GotoNextLevel()
    {
        SceneManager.LoadScene( NextLevel );
    }

    public void GotoMainMenu()
    {
        SceneManager.LoadScene( "MainMenu" );
    }
}
