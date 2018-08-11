using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject PlayerUIPrefab;
    public Transform PlayerSpawnPoint;

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
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }
}
