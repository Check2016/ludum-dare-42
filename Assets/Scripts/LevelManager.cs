using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private const int MaxEnemyCount = 20;
    private const float MinEnemySpawnDelay = 0.5f;
    private const float MaxEnemySpawnDelay = 2;
    private const float EnemyDespawnDistance = 20;

    public GameObject PlayerPrefab;
    public GameObject PlayerUIPrefab;
    public Transform PlayerSpawnPoint;

    [Space]
    public GameObject EnemyPrefab;
    public Transform[] EnemySpawnPoints = new Transform[0];

    public Crystal[] Crystals = new Crystal[0];

    private List<Enemy> enemies = new List<Enemy>();
    private Player player;
    private PlayerUI playerUI;

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

        StartCoroutine( SpawnEnemies() );
    }

    private IEnumerator SpawnEnemies()
    {
        while ( true )
        {
            for ( int i = 0; i < enemies.Count; i++ )
            {
                if ( enemies[i].enabled && Vector3.Distance( enemies[i].transform.position, player.transform.position ) > EnemyDespawnDistance )
                {
                    if ( IsVisibleFrom( enemies[i].GetComponent<MeshRenderer>(), player.specialCamera.FPSCamera ) == false )
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
