using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using TMPro;

public class SpecialCamera : MonoBehaviour
{

    public Camera FPSCamera;
    public float SwayAmount = 1;
    public float SwaySpeed = 1;

    [Space]
    public RenderTexture CameraVisorRT;

    [Space]
    public float CameraPauseCone = 0.1f;
    public float MaxPauseDistance = 1000;

    public RawImage[] PictureSlots;
    public TextMeshProUGUI[] PictureTypeTexts;
    public RectTransform SelectionOutline;
    public TextMeshProUGUI CameraModeText;

    public Animator animator;

    private Vector3 startPosition;

    private Picture[] pictures;
    private int pictureSelection = 0;
    private PictureTypes cameraMode;

    private void Start()
    {
        startPosition = transform.localPosition;

        pictures = new Picture[PictureSlots.Length];

        UpdateSelectionOutline();
        UpdateCameraModeText();
    }

    private void Update()
    {
        if ( Input.GetMouseButtonDown( 0 ) )
        {
            TakePicture();
        }

        if ( Input.GetMouseButtonDown( 2 ) )
        {
            DeletePicture();
        }

        if ( Input.GetKeyDown( KeyCode.Alpha1 ) )
        {
            SwitchCameraMode( PictureTypes.Freeze );
        }

        if ( Input.GetKeyDown( KeyCode.Alpha2 ) )
        {
            SwitchCameraMode( PictureTypes.Capture );
        }

        if ( Input.GetAxis( "Mouse ScrollWheel" ) != 0 )
        {
            if ( Input.GetAxis( "Mouse ScrollWheel" ) > 0 )
            {
                pictureSelection--;
            }
            else
            {
                pictureSelection++;
            }

            if ( pictureSelection < 0 )
            {
                pictureSelection = PictureSlots.Length - 1;
            }
            else if ( pictureSelection >= PictureSlots.Length )
            {
                pictureSelection = 0;
            }

            UpdateSelectionOutline();
        }
    }

    private void FixedUpdate()
    {
        float mouseX = CrossPlatformInputManager.GetAxis( "Mouse X" );
        float mouseY = CrossPlatformInputManager.GetAxis( "Mouse Y" );

        transform.localPosition += new Vector3( mouseX, mouseY, 0 ) * SwayAmount;

        transform.localPosition = Vector3.Lerp( transform.localPosition, startPosition, Time.deltaTime * SwaySpeed );
    }

    private void TakePicture()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = CameraVisorRT;

        Texture2D texture2D = new Texture2D( CameraVisorRT.height, CameraVisorRT.height, TextureFormat.RGB24, true );

        int xOffset = ( int )( ( CameraVisorRT.width - CameraVisorRT.height ) / 2.0f );

        texture2D.ReadPixels( new Rect( xOffset, 0, CameraVisorRT.height, CameraVisorRT.height ), 0, 0 );
        texture2D.Apply();

        RenderTexture.active = currentRT;

        Picture picture = new Picture( texture2D, cameraMode );

        switch( cameraMode )
        {
            case PictureTypes.Freeze:
                {
                    List<Enemy> frozenEnemies = new List<Enemy>();

                    for ( int i = 0; i < GameManager.instance.enemies.Count; i++ )
                    {
                        Vector3 ToEnemyDir = ( GameManager.instance.enemies[i].transform.position - FPSCamera.transform.position ).normalized;

                        Ray ray = new Ray( FPSCamera.transform.position, ToEnemyDir );

                        RaycastHit raycastHit;

                        if ( Physics.Raycast( ray, out raycastHit, MaxPauseDistance ) )
                        {
                            if ( raycastHit.transform.CompareTag( "Enemy" ) && 
                                 1 - Vector3.Dot( FPSCamera.transform.forward.normalized, ToEnemyDir ) <= CameraPauseCone )
                            {
                                frozenEnemies.Add( GameManager.instance.enemies[i] );
                            }
                        }
                    }

                    picture.frozenEnemies = frozenEnemies.ToArray();
                }
                break;
        }

        for ( int i = 0; i < PictureSlots.Length; i++ )
        {
            if ( pictures[i] == null )
            {
                pictures[i] = picture;

                pictureSelection = i;
                UpdateSelectionOutline();

                break;
            }
        }

        UpdatePictureSlots();
        UpdatePausedEnemies();
    }

    private void DeletePicture()
    {
        pictures[pictureSelection] = null;

        UpdatePictureSlots();
        UpdatePausedEnemies();
    }

    private void UpdatePausedEnemies()
    {
        for ( int i = 0; i < GameManager.instance.enemies.Count; i++ )
        {
            GameManager.instance.enemies[i].enabled = true;
        }

        for ( int i = 0; i < PictureSlots.Length; i++ )
        {
            if ( pictures[i] == null || pictures[i].type != PictureTypes.Freeze ) continue;

            for ( int j = 0; j < pictures[i].frozenEnemies.Length; j++ )
            {
                pictures[i].frozenEnemies[j].enabled = false;
            }
        }
    }

    private void UpdatePictureSlots()
    {
        for ( int i = 0; i < pictures.Length; i++ )
        {
            if ( pictures[i] != null )
            {
                PictureSlots[i].texture = pictures[i].texture;
                PictureTypeTexts[i].text = pictures[i].type.ToString();
            }
            else
            {
                PictureSlots[i].texture = null;
                PictureTypeTexts[i].text = "";
            }
        }
    }

    private void UpdateSelectionOutline()
    {
        SelectionOutline.anchoredPosition = new Vector2( PictureSlots[pictureSelection].rectTransform.anchoredPosition.x, SelectionOutline.anchoredPosition.y );
    }

    private void SwitchCameraMode( PictureTypes mode )
    {
        cameraMode = mode;

        UpdateCameraModeText();
    }

    private void UpdateCameraModeText()
    {
        CameraModeText.text = "Mode: " + cameraMode.ToString();
    }
}
