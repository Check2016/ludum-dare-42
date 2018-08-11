using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Enemy> enemies;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }
}
