using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SpecialCamera : MonoBehaviour {

    public Camera FPSCamera;
    public float SwayAmount = 1;
    public float SwaySpeed = 1;

    public Animator animator;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {


        float mouseX = CrossPlatformInputManager.GetAxis( "Mouse X" );
        float mouseY = CrossPlatformInputManager.GetAxis( "Mouse Y" );

        transform.localPosition += new Vector3( mouseX, mouseY, 0 ) * SwayAmount;

        transform.localPosition = Vector3.Lerp( transform.localPosition, startPosition, Time.deltaTime * SwaySpeed );
    }
}
