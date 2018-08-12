using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDImage : MonoBehaviour {

    public RectTransform Canvas;
    public Transform FPSCamera;
    public Transform Target;
    public Transform HUDTarget;

    private RectTransform rectTransform;
    private Image image;

    private bool targetVisible = true;

    public void Setup( RectTransform canvas, Transform fpsCamera, Transform target, Color color, Transform hudTarget = null )
    {
        Canvas = canvas;
        Target = target;

        if ( hudTarget == null )
            HUDTarget = target;
        else
            HUDTarget = hudTarget;

        FPSCamera = fpsCamera;

        rectTransform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        float h = 0;
        float s = 0;
        float v = 0;
        Color.RGBToHSV( color, out h, out s, out v );;
        image.color = Color.HSVToRGB( h, 0.75f, v );

        StartCoroutine( UpdateHUD() );
        StartCoroutine( UpdateTargetVisibility() );
    }

    private IEnumerator UpdateHUD()
    {
        while ( true )
        {
            if ( Target.gameObject.activeSelf == false || targetVisible )
            {
                image.enabled = false;
                yield return null;
            }
            else
            {
                image.enabled = true;
            }

            Vector3 viewportPoint = Camera.main.WorldToViewportPoint( HUDTarget.position );

            if ( viewportPoint.z > 0 )
            {
                rectTransform.anchoredPosition = ( Vector2 )viewportPoint * Canvas.sizeDelta - ( Canvas.sizeDelta / 2.0f );
            }

            yield return null;
        }
    }

    private IEnumerator UpdateTargetVisibility()
    {
        while ( true )
        {
            Vector3 ToTargetDir = ( Target.transform.position - FPSCamera.position ).normalized;

            RaycastHit raycastHit;

            if ( Physics.Raycast( FPSCamera.position, ToTargetDir, out raycastHit ) )
            {
                if ( raycastHit.transform == Target.transform )
                {
                    targetVisible = true;
                    goto Wait;
                }
            }

            targetVisible = false;

            Wait:

            yield return new WaitForSeconds( 0.1f );
        }
    }
}
