using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour {

    private const float BounceAmount = 0.25f;
    private const float BounceSpeed = 2;

    private const float RotationSpeed = 90;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update ()
    {
        transform.position = startPosition + new Vector3( 0, Mathf.Sin( Time.time * BounceSpeed ) * BounceAmount, 0 );

        transform.Rotate( new Vector3( Time.deltaTime * RotationSpeed * Mathf.Sin( Time.time ), Time.deltaTime * RotationSpeed * Mathf.Sin( Time.time + Mathf.PI/2.0f ), Time.deltaTime * RotationSpeed ), Space.Self );
	}
}
