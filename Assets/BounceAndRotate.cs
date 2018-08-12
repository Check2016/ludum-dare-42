using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAndRotate : MonoBehaviour {

    public float RotationSpeed = 90;
    public float BounceAmount = 1;
    public float BounceSpeed = 1;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update ()
    {
        transform.position = startPosition + new Vector3( 0, Mathf.Sin( Time.time * BounceSpeed ) * BounceAmount, 0 );

        transform.Rotate( new Vector3( 0, 0, RotationSpeed ) * Time.deltaTime );
	}
}
