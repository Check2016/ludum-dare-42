using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    private const int Damage = 20;
    private const float DamageRadius = 1.75f;
    private const float DamageCooldownTime = 1;

    public Player Target;

    private NavMeshAgent navMeshAgent;
    private float lastDamageTime = -DamageCooldownTime;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable ()
    {
        navMeshAgent.enabled = true;
    }

    private void OnDisable()
    {
        navMeshAgent.enabled = false;
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
