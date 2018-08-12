using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour {

    private const float BounceAmount = 0.25f;
    private const float BounceSpeed = 2;
    private const float HoverHeight = 1;

    private const float RotationSpeed = 90;

    private const float DockTime = 0.7f;

    public Transform Mesh;

    public delegate void OnDockCallback();
    public OnDockCallback onDock;

    private Vector3 startPosition;
    private Rigidbody rigid;

    private bool docked = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        startPosition = Mesh.localPosition;
    }

    private void OnEnable()
    {
        rigid.isKinematic = false;

        StartCoroutine( DisableRigidbodyWhenSleeping() );
    }

    private void OnDisable()
    {
        rigid.isKinematic = true;

        docked = false;
    }

    private void Update ()
    {
        Mesh.localPosition = startPosition + new Vector3( 0, Mathf.Sin( Time.time * BounceSpeed ) * BounceAmount, 0 );

        Mesh.Rotate( new Vector3( Time.deltaTime * RotationSpeed * Mathf.Sin( Time.time ), Time.deltaTime * RotationSpeed * Mathf.Sin( Time.time + Mathf.PI/2.0f ), Time.deltaTime * RotationSpeed ), Space.Self );
	}

    private IEnumerator DisableRigidbodyWhenSleeping()
    {
        while ( rigid.IsSleeping() == false )
        {
            yield return null;
        }

        rigid.isKinematic = true;
    }

    public IEnumerator Dock( CrystalDock dock )
    {
        rigid.isKinematic = true;

        float t = 0;
        Vector3 targetPos = dock.transform.position + Vector3.up * CrystalDock.DockYOffset;
        Vector3 startPos = transform.position;

        while ( t < 1 )
        {
            t += Time.deltaTime / DockTime;

            transform.position = Vector3.Lerp( startPos, targetPos, Mathf.SmoothStep( 0, 1, t ) );

            yield return null;
        }

        docked = true;

        if ( onDock != null )
            onDock();
    }

    public bool IsDocked()
    {
        return docked;
    }
}
