using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalDock : MonoBehaviour {

    public const float DockYOffset = 2;

    public Crystal Target;

    private bool docked = true;

    private void OnTriggerEnter( Collider other )
    {
        if ( other.gameObject == Target.gameObject )
        {
            StartCoroutine( Target.Dock( this ) );
        }
    }
}
