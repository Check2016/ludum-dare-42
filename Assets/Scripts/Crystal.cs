using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour {

    private const float BounceAmount = 0.25f;
    private const float BounceSpeed = 2;
    private const float HoverHeight = 1;

    private const float RotationSpeed = 90;

    public Transform Mesh;

    private Vector3 startPosition;
    private Rigidbody rigid;

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
}
