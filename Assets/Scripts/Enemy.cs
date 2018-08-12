using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    private const int Damage = 15;
    private const float DamageRadius = 1.75f;
    private const float DamageCooldownTime = 1;

    public Material EnabledMaterial;
    public Material DisablesMaterial;
    public Player Target;

    [HideInInspector]
    public float SpawnTime = 0;

    private NavMeshAgent navMeshAgent;
    private float lastDamageTime = -DamageCooldownTime;

    private void Awake()
    {
        SpawnTime = Time.time;

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable ()
    {
        navMeshAgent.enabled = true;

        GetComponent<MeshRenderer>().sharedMaterial = EnabledMaterial;
    }

    private void OnDisable()
    {
        navMeshAgent.enabled = false;

        GetComponent<MeshRenderer>().sharedMaterial = DisablesMaterial;
    }

    private void Update ()
    {
        navMeshAgent.destination = Target.transform.position;

        if ( Vector3.Distance( transform.position, Target.transform.position ) <= DamageRadius && Time.time - lastDamageTime >= DamageCooldownTime )
        {
            Target.Damage( Damage );
            lastDamageTime = Time.time;
        }
    }
}
